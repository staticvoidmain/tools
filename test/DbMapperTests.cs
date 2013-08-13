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
		public DbMapperIntegrationTests()
		{
			Debugger.Launch();
		}

		[Fact]
		[Trait("TestCategory", "IntegrationTest")]
		public void When_ReaderHasResults_Mapper_MapsAll_Objects()
		{
			List<Order> results = new List<Order>();

			using (var connection = CreateConnection())
			{
				var cmd = connection.CreateCommand();

				cmd.CommandText = "select OrderID, CustomerID, EmployeeID, OrderDate, Freight from [Orders]";

				var reader = cmd.ExecuteReader();
				var mapper = DbMapper.GetMapper<Order>();

				while (reader.Read())
				{
					results.Add(mapper(reader));
				}
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
