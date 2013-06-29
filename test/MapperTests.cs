using System;
using System.Diagnostics;
using Xunit;
using Tools;

namespace Tools.Test.MapperTests
{
	#region Test Classes

	public class LargeType
	{
		public DateTime Fizz0 { get; set; }
		public DateTime Fizz1 { get; set; }
		public DateTime Fizz2 { get; set; }
		public DateTime Fizz3 { get; set; }
		public DateTime Fizz4 { get; set; }
		public DateTime Fizz5 { get; set; }
		public DateTime Fizz6 { get; set; }
		public DateTime Fizz7 { get; set; }
		public DateTime Fizz8 { get; set; }
		public DateTime Fizz9 { get; set; }
		public DateTime Fizz10 { get; set; }
		public DateTime Fizz11 { get; set; }
		public DateTime Fizz12 { get; set; }
		public DateTime Fizz13 { get; set; }
		public DateTime Fizz14 { get; set; }
		public DateTime Fizz15 { get; set; }
		public DateTime Fizz16 { get; set; }
		public DateTime Fizz17 { get; set; }
		public DateTime Fizz18 { get; set; }
		public DateTime Fizz19 { get; set; }
		public Decimal Foo0 { get; set; }
		public Decimal Foo1 { get; set; }
		public Decimal Foo2 { get; set; }
		public Decimal Foo3 { get; set; }
		public Decimal Foo4 { get; set; }
		public Decimal Foo5 { get; set; }
		public Decimal Foo6 { get; set; }
		public Decimal Foo7 { get; set; }
		public Decimal Foo8 { get; set; }
		public Decimal Foo9 { get; set; }
		public Decimal Foo10 { get; set; }
		public Decimal Foo11 { get; set; }
		public Decimal Foo12 { get; set; }
		public Decimal Foo13 { get; set; }
		public Decimal Foo14 { get; set; }
		public Decimal Foo15 { get; set; }
		public Decimal Foo16 { get; set; }
		public Decimal Foo17 { get; set; }
		public Decimal Foo18 { get; set; }
		public Decimal Foo19 { get; set; }
		public int Bar0 { get; set; }
		public int Bar1 { get; set; }
		public int Bar2 { get; set; }
		public int Bar3 { get; set; }
		public int Bar4 { get; set; }
		public int Bar5 { get; set; }
		public int Bar6 { get; set; }
		public int Bar7 { get; set; }
		public int Bar8 { get; set; }
		public int Bar9 { get; set; }
		public int Bar10 { get; set; }
		public int Bar11 { get; set; }
		public int Bar12 { get; set; }
		public int Bar13 { get; set; }
		public int Bar14 { get; set; }
		public int Bar15 { get; set; }
		public int Bar16 { get; set; }
		public int Bar17 { get; set; }
		public int Bar18 { get; set; }
		public int Bar19 { get; set; }
		public string Baz0 { get; set; }
		public string Baz1 { get; set; }
		public string Baz2 { get; set; }
		public string Baz3 { get; set; }
		public string Baz4 { get; set; }
		public string Baz5 { get; set; }
		public string Baz6 { get; set; }
		public string Baz7 { get; set; }
		public string Baz8 { get; set; }
		public string Baz9 { get; set; }
		public string Baz10 { get; set; }
		public string Baz11 { get; set; }
		public string Baz12 { get; set; }
		public string Baz13 { get; set; }
		public string Baz14 { get; set; }
		public string Baz15 { get; set; }
		public string Baz16 { get; set; }
		public string Baz17 { get; set; }
		public string Baz18 { get; set; }
		public string Baz19 { get; set; }
		public DateTime Fuzz0 { get; set; }
		public DateTime Fuzz1 { get; set; }
		public DateTime Fuzz2 { get; set; }
		public DateTime Fuzz3 { get; set; }
		public DateTime Fuzz4 { get; set; }
		public DateTime Fuzz5 { get; set; }
		public DateTime Fuzz6 { get; set; }
		public DateTime Fuzz7 { get; set; }
		public DateTime Fuzz8 { get; set; }
		public DateTime Fuzz9 { get; set; }
		public DateTime Fuzz10 { get; set; }
		public DateTime Fuzz11 { get; set; }
		public DateTime Fuzz12 { get; set; }
		public DateTime Fuzz13 { get; set; }
		public DateTime Fuzz14 { get; set; }
		public DateTime Fuzz15 { get; set; }
		public DateTime Fuzz16 { get; set; }
		public DateTime Fuzz17 { get; set; }
		public DateTime Fuzz18 { get; set; }
		public DateTime Fuzz19 { get; set; }
		public Decimal Fandango0 { get; set; }
		public Decimal Fandango1 { get; set; }
		public Decimal Fandango2 { get; set; }
		public Decimal Fandango3 { get; set; }
		public Decimal Fandango4 { get; set; }
		public Decimal Fandango5 { get; set; }
		public Decimal Fandango6 { get; set; }
		public Decimal Fandango7 { get; set; }
		public Decimal Fandango8 { get; set; }
		public Decimal Fandango9 { get; set; }
		public Decimal Fandango10 { get; set; }
		public Decimal Fandango11 { get; set; }
		public Decimal Fandango12 { get; set; }
		public Decimal Fandango13 { get; set; }
		public Decimal Fandango14 { get; set; }
		public Decimal Fandango15 { get; set; }
		public Decimal Fandango16 { get; set; }
		public Decimal Fandango17 { get; set; }
		public Decimal Fandango18 { get; set; }
		public Decimal Fandango19 { get; set; }
		public int Frob0 { get; set; }
		public int Frob1 { get; set; }
		public int Frob2 { get; set; }
		public int Frob3 { get; set; }
		public int Frob4 { get; set; }
		public int Frob5 { get; set; }
		public int Frob6 { get; set; }
		public int Frob7 { get; set; }
		public int Frob8 { get; set; }
		public int Frob9 { get; set; }
		public int Frob10 { get; set; }
		public int Frob11 { get; set; }
		public int Frob12 { get; set; }
		public int Frob13 { get; set; }
		public int Frob14 { get; set; }
		public int Frob15 { get; set; }
		public int Frob16 { get; set; }
		public int Frob17 { get; set; }
		public int Frob18 { get; set; }
		public int Frob19 { get; set; }
		public string Bizzle0 { get; set; }
		public string Bizzle1 { get; set; }
		public string Bizzle2 { get; set; }
		public string Bizzle3 { get; set; }
		public string Bizzle4 { get; set; }
		public string Bizzle5 { get; set; }
		public string Bizzle6 { get; set; }
		public string Bizzle7 { get; set; }
		public string Bizzle8 { get; set; }
		public string Bizzle9 { get; set; }
		public string Bizzle10 { get; set; }
		public string Bizzle11 { get; set; }
		public string Bizzle12 { get; set; }
		public string Bizzle13 { get; set; }
		public string Bizzle14 { get; set; }
		public string Bizzle15 { get; set; }
		public string Bizzle16 { get; set; }
		public string Bizzle17 { get; set; }
		public string Bizzle18 { get; set; }
		public string Bizzle19 { get; set; }
	}

	public class TypeA
	{
		public Decimal Foo { get; set; }
		public int Bar { get; set; }
		public string Baz { get; set; }
		public DateTime Fizz { get; set; }
	}

	public class TypeB
	{
		public Decimal Foo { get; set; }
		public int Bar { get; set; }
		public string Baz { get; set; }
		public DateTime Fizz { get; set; }
	}

	public class TypeC
	{
		// expect these two not to be mapped.
		public string Foo { get; set; }
		public string Bar { get; set; }

		// expect to be mapped
		public string Baz { get; set; }
	}

	#endregion

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
}
