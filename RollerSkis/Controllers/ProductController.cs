using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Logic;
using Logic.Domain;

using RollerSkis.Models;

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

		public ActionResult GetSkis (string productType, string modelName)
		{
			ProductType type = string.IsNullOrEmpty (productType) ? ProductType.RollerSkis : productTypes[productType];
			bool searchMultipleModels = string.IsNullOrEmpty (modelName);

			List<Product> searchResult = new List<Product>();
			if (searchMultipleModels)
			{
				searchResult.AddRange (ApplicationContext.ProductService.GetProductsByType (type));
			}
			else
			{
				Product p = ApplicationContext.ProductService.GetProduct (type, modelName);
				if (p != null)
				{
					searchResult.Add (p);
				}
			}

			ProductsPageModel model = new ProductsPageModel
			{
				Products = searchResult
			};

			return View (model);
		}
	}
}
