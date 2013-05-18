using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Logic;
using Logic.Domain;
using Logic.Service;

using RollerSkis.Models;

namespace RollerSkis.Controllers
{
	public class NavigationController : Controller
	{
		private static readonly ProductServices ProductServices;

		static NavigationController ()
		{
			ProductServices = ApplicationContext.ProductService;
		}

		public PartialViewResult Menu ()
		{

			IEnumerable<ProductTypeNode> hierarchy = ProductServices.GetProductTypesHierarchy ();
			return PartialView ("Menu", hierarchy);
		}
	}
}
