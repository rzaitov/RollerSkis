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

		public int ChildrenCount { get { return TypeChildren.Count; } }

		internal List<ProductTypeNode> TypeChildren { get; private set; }
		public IEnumerable<ProductTypeNode> Children
		{
			get
			{
				return TypeChildren;
			}
		}

		public ProductTypeNode ()
		{
			TypeChildren = new List<ProductTypeNode> ();
		}

		internal void AddChild (ProductTypeNode node)
		{
			Contract.Requires (node.Parent == null);

			node.Parent = this;
			this.TypeChildren.Add (node);
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
