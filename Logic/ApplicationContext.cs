using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Logic.Service;

namespace Logic
{
	public static class ApplicationContext
	{
		public static ProductServices ProductService { get; set; }
	}
}
