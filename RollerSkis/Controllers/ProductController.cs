using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Logic;
using Logic.Domain;

namespace RollerSkis.Controllers
{
	public class ProductController : Controller
	{
		//
		// GET: /Product/
		private static Dictionary<string, ProductType> productTypes;
		
		static ProductController ()
		{
			productTypes = new Dictionary<string, ProductType>
			{
				{ "skate", ProductType.Skate },
				{ "classic", ProductType.Classic },
				{ "combi", ProductType.Combi }
			};
		}

		public string Index ()
		{
			IEnumerable<Product> products = ApplicationContext.ProductService.GetProductsByType (ProductType.RollerSkis);

			return string.Join ("\n\n", products.Select (p => string.Format ("name: {0}	price: {1}\ndescription: {2}", p.Name, p.Price, p.Description)));
		}

		public string GetSkis (string productType, string modelName)
		{
			return string.Format ("productType: {0}	modelName: {1}", productType, modelName);
		}
	}
}
