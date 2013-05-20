using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logic.Domain
{
	public class Product
	{
		internal static string[] FieldsNames { get; private set; }
		internal static string[] EscapedFieldsNames { get; private set; }

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public ProductType ProductType { get; set; }
		public string ImageName { get; set; }
		public Dictionary<string, string> Specification { get; protected set; }

		public Product ()
		{
			Specification = new Dictionary<string, string> ();
		}

		static Product ()
		{
			FieldsNames = new string[] { "id", "name", "price", "product_type", "description", "image_name" };
			EscapedFieldsNames = FieldsNames.Select (fn => string.Format ("[{0}]", fn)).ToArray ();
		}
	}

	//TODO: Придумать механизм, который поддерживал бы синхронизацию значений с БД
	public enum ProductType
	{
		SpeedReducers = 1,
		Brakes = 2,
		Wheels = 3,
		Accessories = 4,
		Nordixc = 5,

		RollerSkis = 6,
		Combi = 7,
		Classic = 8,
		Skate = 9
	}

	public class ProductTypeValue
	{
		public ProductType ProductType { get; set; }
		public ProductType? ParentType { get; set; }
		public string ProductTypeName { get; set; }
	}
}
