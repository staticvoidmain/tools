using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Test.MapperTests;
using Tools.Test.Northwind;
using Xunit;

namespace Tools.Test
{
	public class DbMapperTests
	{	
		[Fact]
		public void When_Reader_IsNull_Throws_ArgumentNullException()
		{
			SqlDataReader reader = null;

			Assert.Throws(typeof(ArgumentNullException), () => 
			{
				DbMapper.Map<Order>(reader);
			});
		}
	}

	public class DbMapperIntegrationTests
	{
		[Fact]
		[Trait("TestCategory", "CodeGen")]
		public void Generate_Mapper_Assembly()
		{
			DbMapper.CreateMapperAssembly(new[] { typeof(Order) }, "Northwind.Data", "Northwind.Data.Generated"); 
		}

		[Fact]
		[Trait("TestCategory", "IntegrationTest")]
		public void When_ReaderHasResults_Mapper_MapsAll_Objects()
		{
			List<Order> results = new List<Order>();

			Stopwatch watch = Stopwatch.StartNew();

			using (var connection = CreateConnection())
			{
				watch.Stop();

				Debug.WriteLine("connection took {0}ms", watch.ElapsedMilliseconds);

				watch.Reset();
				watch.Start();

				var cmd = connection.CreateCommand();

				cmd.CommandText = "select OrderID, CustomerID, EmployeeID, OrderDate, Freight, Status from [Orders]";

				var reader = cmd.ExecuteReader();

				watch.Stop();
				Debug.WriteLine("execute-reader took {0}ms", watch.ElapsedMilliseconds);
				watch.Reset();

				Func<SqlDataReader, Order> mapper = DbMapper.GetMapper<Order>();

				while (reader.Read())
				{
					results.Add(mapper(reader));
				}

				watch.Stop();
				Debug.WriteLine("mapping took {0}ms", watch.ElapsedMilliseconds);
				watch.Reset();
			}

			Assert.True(results.All(r => 
			{
				return r != null
					&& r.OrderID > 0
					&& r.CustomerID != null
					&& r.EmployeeID.HasValue
					&& r.Freight.HasValue
					&& r.OrderDate.HasValue;
			}));
		}

		private SqlConnection CreateConnection()
		{
			SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

			builder.DataSource = @".\sqlexpress";
			builder.InitialCatalog = "Northwind";
			builder.IntegratedSecurity = true;

			var connection = new SqlConnection(builder.ToString());

			connection.Open();

			return connection;
		}
	}
}
