using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Test.MapperTests;
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
				DbMapper.Map<TypeA>(null);
			});
		}
	}

	public class DbMapperIntegrationTests
	{
		[Fact]
		public void When_ReaderHasResults_Mapper_MapsAll_Objects()
		{
			List<TypeA> results = new List<TypeA>();

			using (var connection = new SqlConnection())
			{
				var cmd = connection.CreateCommand();
				var reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					TypeA a = DbMapper.Map<TypeA>(reader);

					results.Add(a);
				}
			}

			Assert.Equal(10, results.Count);
			Assert.True(results.All(r => r != null));
		}
	}
}
