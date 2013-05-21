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

			AddToMap(ProductType.SpeedReducers, "speed-reducers");
			AddToMap(ProductType.Brakes, "brakes");
			AddToMap(ProductType.Wheels, "wheels");
			AddToMap(ProductType.Accessories, "accessories");
			AddToMap(ProductType.Nordixc, "nordixc");

			AddToMap (ProductType.RollerSkis, "roller-skis");
				AddToMap (ProductType.Skate, "skate");
				AddToMap (ProductType.Classic, "classic");
				AddToMap (ProductType.Combi, "combi");
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

		public static void GetTypeNamesFromUrlSheme(string highType, string lowType, out string parentTypeName, out string productTypeName)
		{
			parentTypeName = highType;
			productTypeName = lowType;

			if (string.IsNullOrEmpty(lowType))
			{
				parentTypeName = null;
				productTypeName = highType;
			}
		}

		public static void GetUrlShemeByProductType (List<ProductType> typesFromHighToLow, out string hightType, out string lowType)
		{
			hightType = productTypesNameMap[typesFromHighToLow[0]];
			lowType = typesFromHighToLow.Count > 1 ? productTypesNameMap[typesFromHighToLow[1]] : null;
		}
	}
}