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
				"products/{highType}/{lowType}/{modelName}",
				new
				{
					controller = "Product",
					action = "GetProduct",
				},
				new
				{
					highType = "^roller-skis$|^speed-reducers$|^brakes$|^wheels$|^accessories$|^nordixc$",
					lowType = "^combi$|^classic$|^skate$"
				});

			routes.MapRoute(
				null,
				"products/{highType}/{modelName}",
				new
				{
					controller = "Product",
					action = "GetProduct",
				},
				new
				{
					highType = "^speed-reducers$|^brakes$|^wheels$|^accessories$|^nordixc$",
				});


			routes.MapRoute (
				null,
				"products/{highType}/{lowType}",
				new
				{
					controller = "Product",
					action = "GetProducts",
					lowType = UrlParameter.Optional,
				},
				new
				{
					highType = "^roller-skis$|^speed-reducers$|^brakes$|^wheels$|^accessories$|^nordixc$",
					lowType = "^combi$|^classic$|^skate$|^$"
				}
				);

			//routes.MapRoute ();
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