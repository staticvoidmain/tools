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
		public Nullable<OrderStatus> Status { get; set; }
	}
}
