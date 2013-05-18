using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Logic.Domain;

namespace RollerSkis.Models
{
	public class ProductTypeHelper
	{
		/*
		private class TypeMap
		{
			public ProductType? Parent { get; set; }
			public ProductType ProductType { get; set; }
		}

		private class NameMap
		{
			public ProductType? Parent { get; set; }
			public string TypeName { get; set; }
		}
		*/
		private static Dictionary<string, ProductType> productNameTypesMap;
		private static Dictionary<ProductType, string> productTypesNameMap;

		static ProductTypeHelper ()
		{
			productNameTypesMap = new Dictionary<string, ProductType> ();
			productTypesNameMap = new Dictionary<ProductType, string> ();

			AddToMap (ProductType.RollerSkis, "roller-skis");
				AddToMap (ProductType.Skate, "skate");
				AddToMap (ProductType.Classic, "classic");
				AddToMap (ProductType.Combi, "combi");

			AddToMap (ProductType.Accessories, "accessories");
			//AddToMap (ProductType.N, "nordixc");
			AddToMap (ProductType.Brakes, "brakes");
			AddToMap (ProductType.SpeedReducers, "speed-reducers");
		}

		static void AddToMap (ProductType type, string typeName)
		{
			productNameTypesMap[typeName] = type;
			productTypesNameMap[type] = typeName;
		}

		public ProductType this[string key]
		{
			get { return productNameTypesMap[key]; }
		}

		public string this[ProductType key]
		{
			get { return productTypesNameMap[key]; }
		}
	}
}