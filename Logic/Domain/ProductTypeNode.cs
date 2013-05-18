using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics.Contracts;

namespace Logic.Domain
{
	public class ProductTypeNode
	{
		internal int Id { get; set; }
		internal int? ParentId { get; set; }

		public ProductTypeNode Parent { get; internal set; }

		public ProductType ProductTypeValue
		{
			get { return (ProductType)Id; }
		}
		public string Name { get; internal set; }

		internal List<ProductTypeNode> Children { get; private set; }
		public IEnumerable<ProductTypeNode> TypeChildren
		{
			get
			{
				return Children;
			}
		}

		public ProductTypeNode ()
		{
			Children = new List<ProductTypeNode> ();
		}

		internal void AddChild (ProductTypeNode node)
		{
			Contract.Requires (node.Parent == null);

			node.Parent = this;
			this.Children.Add (node);
		}

		internal void AddRange (IEnumerable<ProductTypeNode> children)
		{
			Contract.Requires (children != null);

			foreach (ProductTypeNode ptn in children)
			{
				AddChild (ptn);
			}
		}
	}
}
