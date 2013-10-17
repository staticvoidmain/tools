using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
	internal static class MapperUtils
	{
		internal static bool IsIndexer(PropertyInfo property)
		{
			var indexers = property.GetIndexParameters();

			return indexers.Length > 0;
		}

		internal static bool IsSimpleSetter(MethodInfo setter, out FieldInfo field)
		{
			MethodBody body = setter.GetMethodBody();
			bool isSimple = false;

			// out param
			field = null;

			if (body.ExceptionHandlingClauses.Count == 0 && body.LocalVariables.Count == 0)
			{
				byte[] il = body.GetILAsByteArray();

				if (il.Length == 8)
				{
					if (il[0] == OpCodes.Ldarg_0.Value
						&& il[1] == OpCodes.Ldarg_1.Value
						&& il[2] == OpCodes.Stfld.Value
						&& il[7] == OpCodes.Ret.Value)
					{
						int fieldToken = BitConverter.ToInt32(il, 3);
						FieldInfo info = setter.DeclaringType.Module.ResolveField(fieldToken);

						if (info != null
							&& info.DeclaringType.IsAssignableFrom(setter.DeclaringType))
						{
							isSimple = true;
							field = info;
						}
					}
				}
			}

			return isSimple;
		}

		// these are very similar, but the layout is a bit different.
		internal static bool IsSimpleGetter(MethodInfo getter, out FieldInfo field)
		{
			MethodBody body = getter.GetMethodBody();
			bool isSimple = false;

			// out param
			field = null;

			if (body.ExceptionHandlingClauses.Count == 0 && body.LocalVariables.Count == 1)
			{
				byte[] il = body.GetILAsByteArray();

				if (il.Length > 8)
				{
					if (il[0] == OpCodes.Ldarg_0.Value
						&& il[1] == OpCodes.Ldfld.Value
						&& il[6] == OpCodes.Stloc_0.Value)
					{
						int fieldToken = BitConverter.ToInt32(il, 2);
						FieldInfo info = getter.DeclaringType.Module.ResolveField(fieldToken);

						if (info != null
							&& info.DeclaringType.IsAssignableFrom(getter.DeclaringType))
						{
							isSimple = true;
							field = info;
						}
					}
				}
			}

			return isSimple;
		}
	}
}
