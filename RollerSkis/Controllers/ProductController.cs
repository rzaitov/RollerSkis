using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics.Contracts;

using Logic;
using Logic.Domain;
using Logic.Service;

using RollerSkis.Models;


namespace RollerSkis.Controllers
{
	public class ProductController : Controller
	{
		//
		// GET: /Product/
		private static readonly ProductTypeHelper ProductTypeHelper;
		private static readonly ProductServices ProductService;
		static ProductController ()
		{
			ProductTypeHelper = new ProductTypeHelper ();
			ProductService = ApplicationContext.ProductService;
		}

		public string Index ()
		{
			IEnumerable<Product> products = ApplicationContext.ProductService.GetProductsByType (ProductType.RollerSkis);

			return string.Join ("\n\n", products.Select (p => string.Format ("name: {0}	price: {1}\ndescription: {2}", p.Name, p.Price, p.Description)));
		}

		#region ActioMethods
		public ActionResult GetProducts (string highType, string lowType)
		{
			string parentTypeName, productTypeName;
			ProductTypeHelper.GetTypeNamesFromUrlSheme(highType, lowType, out parentTypeName, out productTypeName);

			bool isSearchWithExactType = !string.IsNullOrEmpty(parentTypeName);

			CheckParentType (parentTypeName, productTypeName);

			ProductType type = ProductTypeHelper[productTypeName];
			ViewResult view = GetProductSearchResult (type);

			return view;
		}

		public ActionResult GetProduct (string highType, string lowType, string modelName)
		{
			string parentTypeName, productTypeName;
			ProductTypeHelper.GetTypeNamesFromUrlSheme(highType, lowType, out parentTypeName, out productTypeName);

			CheckParentType(parentTypeName, productTypeName);

			ProductType productType = ProductTypeHelper[productTypeName];
			return ProductView (productType, modelName);
		}
		#endregion

		#region Logic
		private void CheckParentType (string parentTypeName, string productTypeName)
		{
			// Проверяем действительно ли parentTypeName является родителем для productTypeName
			bool isParentIncorrect = !IsParentValid (parentTypeName, productTypeName);

			// например ChildType/ParentType
			if (isParentIncorrect)
			{
				throw new HttpException (404, "NotFound");
			}
		}

		private bool IsParentValid (string parentTypeName, string productTypeName)
		{
			bool isSearchByTwoTypes = !string.IsNullOrEmpty(parentTypeName);

			// Проверяем действительно ли parentTypeName является родителем для productTypeName
			ProductType productType = ProductTypeHelper[productTypeName];
			bool isParentCorrect = !isSearchByTwoTypes
				|| ProductService.IsValidParentTypeFor(ProductTypeHelper[parentTypeName], productType);

			return isParentCorrect;
		}
		#endregion

		#region Render
		private ViewResult GetProductSearchResult (ProductType type)
		{
			List<Product> searchResult = new List<Product> ();
			searchResult.AddRange (ApplicationContext.ProductService.GetProductsByType (type));

			ProductsPageModel model = new ProductsPageModel
			{
				Products = searchResult
			};

			return View ("SearchResult", model);
		}

		private ViewResult ProductView (ProductType exactType, string modelName)
		{
			Product p = ApplicationContext.ProductService.GetProduct (exactType, modelName);
			return View ("Product", p);
		}
		#endregion
	}
}
