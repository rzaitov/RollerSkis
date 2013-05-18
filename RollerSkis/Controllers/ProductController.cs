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
		public ActionResult GetProducts (string parentTypeName, string productTypeName = null)
		{
			bool isSearchWithExactType = !string.IsNullOrEmpty (productTypeName);

			CheckParentType (parentTypeName, productTypeName);

			string typeNameKey = isSearchWithExactType ? productTypeName : parentTypeName;
			ProductType type = ProductTypeHelper[typeNameKey];

			ViewResult view = GetProductSearchResult (type);
			return view;
		}

		public ActionResult GetProduct (string parentTypeName, string productTypeName, string modelName)
		{
			CheckParentType (parentTypeName, productTypeName);

			ProductType productType = ProductTypeHelper[productTypeName];
			return ProductView (productType, modelName);
		}
		#endregion

		#region Login
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
			bool isSearchByTwoTypes = !string.IsNullOrEmpty (productTypeName);

			// Проверяем действительно ли parentTypeName является родителем для productTypeName
			ProductType parentType = ProductTypeHelper[parentTypeName];
			bool isParentCorrect = !isSearchByTwoTypes
				|| ProductService.IsValidParentTypeFor (parentType, ProductTypeHelper[productTypeName]);

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
