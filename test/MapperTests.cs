using System;
using System.Diagnostics;
using Tools.Test.InternalTestClasses;
using Xunit;

namespace Tools.Test.MapperTests
{
	public class WhenObjectHasReadableProperties
	{
		[Fact]
		public void Map_Copies_All_Properties()
		{
			var a = new TypeA()
			{
				Foo = 42,
				Bar = 22,
				Baz = "ross",
				Fizz = new DateTime(1986, 8, 22)
			};

			var b = Mapper.Map<TypeA, TypeB>(a);

			Assert.Equal(a.Foo, b.Foo);
			Assert.Equal(a.Bar, b.Bar);
			Assert.Equal(a.Baz, b.Baz);
			Assert.Equal(a.Fizz, b.Fizz);
		}

		[Fact]
		public void Map_Copies_All_Properties_Reverse()
		{
			var b = new TypeB()
			{
				Foo = 42,
				Bar = 22,
				Baz = "ross",
				Fizz = new DateTime(1986, 8, 22)
			};

			var a = Mapper.Map<TypeB, TypeA>(b);

			Assert.Equal(b.Foo, a.Foo);
			Assert.Equal(b.Bar, a.Bar);
			Assert.Equal(b.Baz, a.Baz);
			Assert.Equal(b.Fizz, a.Fizz);
		}

		[Fact]
		public void Map_Copies_All_Properties_LoadTest()
		{
			var b = new TypeB()
			{
				Foo = 42,
				Bar = 22,
				Baz = "ross",
				Fizz = new DateTime(1986, 8, 22)
			};

			var watch = Stopwatch.StartNew();

			for (int i = 0; i < 100000; i++)
			{
				var a = Mapper.Map<TypeB, TypeA>(b);
			}

			watch.Stop();

			Debug.WriteLine("elapsed: {0}ms", watch.ElapsedMilliseconds);
		}

		[Fact]
		public void Map_Copies_All_Properties_LargeObject_LoadTest()
		{
			var large = new LargeType();
			var watch = Stopwatch.StartNew();

			for (int i = 0; i < 10000; i++)
			{
				var result = Mapper.Map<LargeType, LargeType>(large);
			}

			watch.Stop();
			Debug.WriteLine("elapsed: {0}ms", watch.ElapsedMilliseconds);
		}
	}

	public class WhenSourceObjectIsNull
	{
		[Fact]
		public void Map_Will_Throw()
		{
			Assert.Throws(typeof(ArgumentNullException), () => Mapper.Map<TypeA, TypeB>(null));
		}
	}

	public class MapperIntegrationTests
	{
		[Fact]
		[Trait("TestCategory", "CodeGen")]
		public void Generate_Mapper_Assembly()
		{
			var mappers = new[] 
			{ 
				Tuple.Create(typeof(TypeA), typeof(TypeB)),
				Tuple.Create(typeof(TypeB), typeof(TypeA))
			};

			Mapper.CreateMapperAssembly(mappers, "Example.Objects", "Example.Objects.Generated");
		}
	}
}