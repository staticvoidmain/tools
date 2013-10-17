//
//  Mapper.cs, simple property mapping from one object to another using IL Emit caching.
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
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading;

	public class Mapper
	{
		private static readonly Dictionary<int, object> _cache = new Dictionary<int, object>();
		private static readonly MethodInfo _emitMapper = typeof(Mapper).GetMethod("EmitMapper", BindingFlags.Static | BindingFlags.NonPublic);
		private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
		public static Func<TSource, TResult> CreateMapping<TSource, TResult>()
		{
			DynamicMethod m = CreateDynamicMethod<TSource, TResult>();
			ILGenerator il = m.GetILGenerator();

			EmitMapper<TSource, TResult>(il);

			return (Func<TSource, TResult>)m.CreateDelegate(typeof(Func<TSource, TResult>));
		}

		public static TResult Map<TSource, TResult>(TSource source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			int key = CreateKey<TSource, TResult>();
			Func<TSource, TResult> mapper;

			_lock.EnterUpgradeableReadLock();

			try
			{
				mapper = GetMapping<TSource, TResult>(key);

				if (mapper == null)
				{
					mapper = CreateMapping<TSource, TResult>();

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

			return mapper(source);
		}

		/// <summary>
		/// Emits a mapper assembly, mostly for debugging.
		/// </summary>
		/// <param name="types"></param>
		/// <param name="sourceAssembly"></param>
		/// <param name="output"></param>
		internal static void CreateMapperAssembly(IEnumerable<Tuple<Type, Type>> types, string sourceAssembly, string output)
		{
			var name = string.Concat(sourceAssembly, ".Generated");
			var assemblyName = new AssemblyName() { Name = name };
			var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save);
			var module = assembly.DefineDynamicModule(name, string.Concat(name, ".dll"), true);

			const MethodAttributes attrs = MethodAttributes.Public | MethodAttributes.Static;
			const CallingConventions cc = CallingConventions.Standard;

			foreach (var pair in types)
			{
				var builder = module.DefineType(string.Concat(pair.Item1.Name, "Mapper"), TypeAttributes.Public);
				var method = builder.DefineMethod("Map", attrs, cc, pair.Item2, new Type[] { pair.Item1 });
				var il = method.GetILGenerator();

				var emitter = _emitMapper.MakeGenericMethod(new Type[] { pair.Item1, pair.Item2 });

				emitter.Invoke(null, new[] { il });

				builder.CreateType();
			}

			assembly.Save(output);
		}

		private static DynamicMethod CreateDynamicMethod<TSource, TResult>()
		{
			string name = CreateMethodName<TSource, TResult>();

			return new DynamicMethod(
				name: name,
				attributes: MethodAttributes.Static | MethodAttributes.Public,
				callingConvention: CallingConventions.Standard,
				returnType: typeof(TResult),
				parameterTypes: new Type[] { typeof(TSource) },
				owner: typeof(Mapper),
				skipVisibility: true);
		}

		// inspired by tuple's get hashcode
		private static int CreateKey<T1, T2>()
		{
			unchecked
			{
				int h1 = (int)typeof(T1).GetHashCode();
				int h2 = (int)typeof(T2).GetHashCode();

				return (h1 << 5) + h1 ^ h2;
			}
		}

		private static string CreateMethodName<T1, T2>()
		{
			return string.Concat("MapFrom", typeof(T1).Name, "To", typeof(T2).Name);
		}

		private static void EmitGet(ILGenerator il, MethodInfo getter)
		{
			FieldInfo getField;
			if (MapperUtils.IsSimpleGetter(getter, out getField))
			{
				il.Emit(OpCodes.Ldfld, getField);
				return;
			}

			il.Emit(OpCodes.Callvirt, getter);
		}

		private static void EmitMapper<TSource, TResult>(ILGenerator il)
		{
			il.DeclareLocal(typeof(TResult));
			il.Emit(OpCodes.Newobj, typeof(TResult).GetConstructor(Type.EmptyTypes));
			il.Emit(OpCodes.Stloc_0);

			foreach (PropertyInfo destinationProperty in typeof(TResult).GetProperties())
			{
				if (MapperUtils.IsIndexer(destinationProperty))
					continue;

				PropertyInfo sourceProperty = GetSourceProperty<TSource>(destinationProperty);

				if (sourceProperty != null)
				{
					if (MapperUtils.IsIndexer(sourceProperty))
						continue;

					MethodInfo setter = destinationProperty.GetSetMethod(false);
					MethodInfo getter = sourceProperty.GetGetMethod(false);

					il.Emit(OpCodes.Ldloc_0);
					il.Emit(OpCodes.Ldarg_0);

					EmitGet(il, getter);
					EmitSet(il, setter);
				}
			}

			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Ret);
		}
		private static void EmitSet(ILGenerator il, MethodInfo setter)
		{
			FieldInfo setField;
			if (MapperUtils.IsSimpleSetter(setter, out setField))
			{
				il.Emit(OpCodes.Stfld, setField);
				return;
			}

			il.Emit(OpCodes.Callvirt, setter);
		}

		private static Func<T1, T2> GetMapping<T1, T2>(int key)
		{
			object func;
			Func<T1, T2> result = null;

			if (_cache.TryGetValue(key, out func))
			{
				result = (Func<T1, T2>)func;
			}

			return result;
		}

		private static PropertyInfo GetSourceProperty<TSource>(PropertyInfo destinationProperty)
		{
			const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

			return typeof(TSource).GetProperty(
				name: destinationProperty.Name,
				bindingAttr: flags,
				binder: null,
				returnType: destinationProperty.PropertyType,
				types: Type.EmptyTypes,
				modifiers: null);
		}
	}
}