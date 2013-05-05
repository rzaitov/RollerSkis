using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Logic;
using Logic.Service;

namespace RollerSkis
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters (GlobalFilterCollection filters)
		{
			filters.Add (new HandleErrorAttribute ());
		}

		public static void RegisterRoutes (RouteCollection routes)
		{
			routes.IgnoreRoute ("{resource}.axd/{*pathInfo}");

			routes.MapRoute (
				null,
				"products/roller-skis/{productType}/{modelName}",
				new { controller = "Product", action = "GetSkis", productType = UrlParameter.Optional, modelName = UrlParameter.Optional },
				new { productType = "^combi$|^classic$|^skate|^$"}
				);

			routes.MapRoute (
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);
		}

		protected void Application_Start ()
		{
			AreaRegistration.RegisterAllAreas ();

			RegisterGlobalFilters (GlobalFilters.Filters);
			RegisterRoutes (RouteTable.Routes);

			InitAppContext ();
		}

		private void InitAppContext ()
		{
			ApplicationContext.ProductService = new ProductServices ("skiadmin", "skiadmin", @"ZAITOVCOMPUTER\HOSTSQLSERVER", "RollerSkis");
		}
	}
}