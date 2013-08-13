using System;
using System.Data.SqlClient;

namespace Tools.Test.Northwind
{
	public class Order
	{
		public int OrderID { get; set; }
		public string CustomerID { get; set; }
		public Nullable<int> EmployeeID { get; set; }
		public Nullable<DateTime> OrderDate { get; set; }
		public Nullable<Decimal> Freight { get; set; }

		public static Order Map(SqlDataReader reader)
		{
			Order o = new Order();
			o.OrderID = !reader.IsDBNull(0)	? reader.GetInt32(0) : 0;

			o.CustomerID = !reader.IsDBNull(1) ? reader.GetString(1) : null;

			o.EmployeeID = !reader.IsDBNull(2)
				? new Nullable<int>(reader.GetInt32(2))
				: default(int?);

			o.OrderDate = !reader.IsDBNull(3)
				? new Nullable<DateTime>(reader.GetDateTime(3))
				: default(DateTime?);

			o.Freight = !reader.IsDBNull(4)
				? new Nullable<Decimal>(reader.GetInt32(4))
				: default(decimal?);

			return o;
		}
	}
}
