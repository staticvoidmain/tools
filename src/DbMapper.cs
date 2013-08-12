//  DbMapper.cs - cached IL Emit delegates from SqlDataReader to some result type.
//  Copyright (C) 2013  Ross Jennings
//
//	This program is free software: you can redistribute it and/or modify
//	it under the terms of the GNU General Public License as published by
//	the Free Software Foundation, either version 3 of the License, or
//	(at your option) any later version.
//
//	This program is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//	GNU General Public License for more details.
//
//	You should have received a copy of the GNU General Public License
//	along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Tools
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Data.Common;
	using System.Data.SqlClient;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading;

	public class DbMapper
	{
		private static readonly Dictionary<int, object> _cache = new Dictionary<int, object>();
		private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		/// <summary>
		///		Maps from a data reader result to the requested TResult entity.
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="reader"></param>
		/// <returns>The resulting entity</returns>
		public static TResult Map<TResult>(SqlDataReader reader)
			where TResult : new()
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}

			int key = typeof(TResult).GetHashCode();
			Func<SqlDataReader, TResult> mapper;

			_lock.EnterUpgradeableReadLock();
			
			try
			{
				mapper = GetMapping<TResult>(key);

				if (mapper == null)
				{
					mapper = CreateMapping<TResult>();

					_lock.EnterWriteLock();

					try
					{
						_cache.Add(key, mapper);
					}
					finally { _lock.ExitWriteLock(); }
				}
			}
			finally
			{
				_lock.ExitUpgradeableReadLock();
			}

			return mapper(reader);
		}

		internal static MethodInfo GetReaderMethod(Type type)
		{
			MethodInfo result = null;

			switch (Type.GetTypeCode(type))
			{
				case TypeCode.String:
					result = SqlDataReaderMethods.GetString;
					break;
				case TypeCode.Int16:
					result = SqlDataReaderMethods.GetInt16;
					break;
				case TypeCode.Int32:
					result = SqlDataReaderMethods.GetInt32;
					break;
				case TypeCode.Int64:
					result = SqlDataReaderMethods.GetInt64;
					break;
				case TypeCode.Boolean:
					result = SqlDataReaderMethods.GetBool;
					break;
				case TypeCode.Decimal:
					result = SqlDataReaderMethods.GetDecimal;
					break;
				case TypeCode.Double:
					result = SqlDataReaderMethods.GetDouble;
					break;
				case TypeCode.Single:
					result = SqlDataReaderMethods.GetFloat;
					break;
				case TypeCode.DateTime:
					result = SqlDataReaderMethods.GetDateTime;
					break;
				case TypeCode.Byte:
					result = SqlDataReaderMethods.GetByte;
					break;
				default:
					break;
			}

			if (result == null)
			{
				// ok now we're getting into no-man's land
				result = typeof(Guid) == type 
					? SqlDataReaderMethods.GetGuid
					: SqlDataReaderMethods.GetValue;

				// todo: maybe support converters or something.
			}

			return result;
		}

		private static DynamicMethod CreateDynamicMethod<TResult>()
		{
			return new DynamicMethod(
				name: string.Concat("DbMapper_", typeof(TResult).Name),
				attributes: MethodAttributes.Static | MethodAttributes.Public,
				callingConvention: CallingConventions.Standard,
				returnType: typeof(TResult),
				parameterTypes: new Type[] { typeof(SqlDataReader) },
				owner: typeof(DbMapper),
				skipVisibility: true);
		}

		private static Func<SqlDataReader, TResult> CreateMapping<TResult>()
		{
			DynamicMethod m = CreateDynamicMethod<TResult>();
			ILGenerator il = m.GetILGenerator();

			il.DeclareLocal(typeof(TResult));
			il.Emit(OpCodes.Newobj, typeof(TResult).GetConstructor(Type.EmptyTypes));
			il.Emit(OpCodes.Stloc_0);
	
			// critical: SqlDataReader results must be in the correct order!
			// note: there will be *some* duplication for deep object heirarchies, but it's probably not a big deal.
			// uses embedded const ordinal positions instead of locals in the generated method.
			int ordinalPosition = 0;
			Lazy<Collection<Type>> localDefaults = new Lazy<Collection<Type>>();

			foreach (PropertyInfo destinationProperty in typeof(TResult).GetProperties())
			{
				MethodInfo setter = destinationProperty.GetSetMethod(false);

				if (setter == null)
					continue;

				Type propertyType = destinationProperty.PropertyType;
				Type nullableType = Nullable.GetUnderlyingType(destinationProperty.PropertyType);

				MethodInfo reader = GetReaderMethod(nullableType ?? propertyType);

				Label beforeSetter = il.DefineLabel();
				Label beforeReader = il.DefineLabel();

				il.Emit(OpCodes.Ldloc_0);
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldc_I4, ordinalPosition);

				il.Emit(OpCodes.Callvirt, SqlDataReaderMethods.IsDbNull);
				il.Emit(OpCodes.Brfalse_S, beforeReader);

				// set default value if IsDBNull.
				if (destinationProperty.PropertyType.IsValueType == false)
				{
					il.Emit(OpCodes.Ldnull);
				}
				else
				{
					int index = localDefaults.Value.IndexOf(destinationProperty.PropertyType);

					if (index == -1)
					{
						index = localDefaults.Value.Count;

						localDefaults.Value.Add(destinationProperty.PropertyType);
						il.DeclareLocal(destinationProperty.PropertyType);
					}

					// Ldloca_S only works for 0-255, but we are only interested in values we support, of which there are only a few.
					il.Emit(OpCodes.Ldloca_S, (byte)index + 1);
					il.Emit(OpCodes.Initobj, destinationProperty.PropertyType);
					il.Emit(OpCodes.Ldloca_S, (byte)index + 1);
				}
				
				il.Emit(OpCodes.Br_S, beforeSetter);

				il.MarkLabel(beforeReader);

				il.Emit(OpCodes.Ldc_I4, ordinalPosition);
				il.Emit(OpCodes.Callvirt, reader);

				if (nullableType != null)
				{
					il.Emit(OpCodes.Newobj, propertyType);
				}
				
				il.MarkLabel(beforeSetter);
				il.Emit(OpCodes.Callvirt, setter);

				ordinalPosition++;
			}

			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Ret);

			return (Func<SqlDataReader, TResult>)m.CreateDelegate(typeof(Func<SqlDataReader, TResult>));
		}

		private static Func<SqlDataReader, TResult> GetMapping<TResult>(int key)
		{
			object func;
			Func<SqlDataReader, TResult> result = null;

			if (_cache.TryGetValue(key, out func))
			{
				result = (Func<SqlDataReader, TResult>)func;
			}

			return result;
		}

		private class SqlDataReaderMethods
		{
			public static readonly MethodInfo GetBool = typeof(SqlDataReader).GetMethod("GetBoolean");
			public static readonly MethodInfo GetByte = typeof(SqlDataReader).GetMethod("GetByte");
			public static readonly MethodInfo GetChar = typeof(SqlDataReader).GetMethod("GetChar");
			public static readonly MethodInfo GetDateTime = typeof(SqlDataReader).GetMethod("GetDateTime");
			public static readonly MethodInfo GetDecimal = typeof(SqlDataReader).GetMethod("GetDecimal");
			public static readonly MethodInfo GetDouble = typeof(SqlDataReader).GetMethod("GetDouble");
			public static readonly MethodInfo GetFloat = typeof(SqlDataReader).GetMethod("GetFloat");
			public static readonly MethodInfo GetGuid = typeof(SqlDataReader).GetMethod("GetGuid");
			public static readonly MethodInfo GetInt16 = typeof(SqlDataReader).GetMethod("GetInt16");
			public static readonly MethodInfo GetInt32 = typeof(SqlDataReader).GetMethod("GetInt32");
			public static readonly MethodInfo GetInt64 = typeof(SqlDataReader).GetMethod("GetInt64");
			public static readonly MethodInfo GetString = typeof(SqlDataReader).GetMethod("GetString");
			public static readonly MethodInfo GetTimeSpan = typeof(SqlDataReader).GetMethod("GetTimeSpan");
			public static readonly MethodInfo GetValue = typeof(SqlDataReader).GetMethod("GetValue");
			public static readonly MethodInfo IsDbNull = typeof(SqlDataReader).GetMethod("IsDBNull");
		}
	}
}
