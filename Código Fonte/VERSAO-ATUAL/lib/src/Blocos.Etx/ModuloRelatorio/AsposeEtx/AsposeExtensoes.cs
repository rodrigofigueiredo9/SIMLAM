using System;
using System.Collections.Generic;
using System.Linq;
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Tables;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx
{
	public static class AsposeExtensoes
	{
		public static T Parent<T>(this Node node)
		{
			if (node.ParentNode == null)
			{
				return default(T);
			}

			if (node.ParentNode is T)
			{
				return (T)(object)node.ParentNode;// Convert.ChangeType(node.ParentNode, typeof(T));//(T)node.ParentNode;
			}

			return node.ParentNode.Parent<T>();
		}

		public static T Find<T>(this CompositeNode node, string textoContido = null)
		{
			if ((!node.HasChildNodes))
			{
				return default(T);
			}

			foreach (Node item in node.GetChildNodes(NodeType.Any, true))
			{
				if (String.IsNullOrEmpty(textoContido) && item is T)
				{
					return (T)(Object)item;
				}

				if (!(item is T))
				{
					continue;
				}

                string valor = item.ToString(SaveFormat.Text);
				if (String.IsNullOrEmpty(valor))
				{
					continue;
				}

				valor = valor.Replace("\n", "").Replace("\r", "");

				if (valor.IndexOf(textoContido, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					continue;
				}
				return (T)(Object)item;
			}

			return default(T);
		}

		public static List<T> Any<T>(this CompositeNode node, string textoContido = null, bool isDeep = false)
		{
			if ((!node.HasChildNodes))
			{
				return default(List<T>);
			}

			Node[] nodes = node.GetChildNodes(NodeType.Any, isDeep).ToArray();

			if (String.IsNullOrEmpty(textoContido))
			{
				return nodes.OfType<T>().ToList();
			}

			return nodes.Where(item =>
			{
				if (!(item is T))
				{
					return false;
				}
                string valor = item.ToString(SaveFormat.Text);

				if (String.IsNullOrEmpty(valor))
				{
					return false;
				}

				valor = valor.Replace("\n", "").Replace("\r", "");

				if (valor.IndexOf(textoContido, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					return false;
				}

				return true;
			}).Cast<T>().ToList();
		}

		public static T Last<T>(this CompositeNode node, string textoContido = null)
		{
			if ((!node.HasChildNodes))
			{
				return default(T);
			}

			Node[] nodes = node.GetChildNodes(NodeType.Any, true).ToArray();

			if (String.IsNullOrEmpty(textoContido))
			{
				return nodes.OfType<T>().LastOrDefault();
			}

			T retorno = nodes.OfType<T>().LastOrDefault(item =>
			{
                string valor = (item as Node).ToString(SaveFormat.Text);

				if (String.IsNullOrEmpty(valor))
				{
					return false;
				}

				valor = valor.Replace("\n", "").Replace("\r", "");

				if (valor.IndexOf(textoContido, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					return false;
				}

				return true;
			});

			return retorno;
		}

		public static T Next<T>(this Node node, string textoContido = null)
		{
			Node atual = node;

			while (atual != null)
			{
				atual = atual.NextSibling;

				if (!(atual is T))
				{
					continue;
				}

                string valor = atual.ToString(SaveFormat.Text); 

				if (String.IsNullOrEmpty(valor))
				{
					continue;
				}

				valor = valor.Replace("\n", "").Replace("\r", "");

				if (valor.IndexOf(textoContido, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					continue;
				}

				return (T)(Object)atual;
			}

			return default(T);
		}

		public static T Previous<T>(this Node node, string textoContido = null)
		{
			Node atual = node;

			while (atual != null)
			{
				atual = atual.PreviousSibling;

				if (!(atual is T))
				{
					continue;
				}

                string valor = atual.ToString(SaveFormat.Text);

				if (String.IsNullOrEmpty(valor))
				{
					continue;
				}

				valor = valor.Replace("\n", "").Replace("\r", "");

				if (valor.IndexOf(textoContido, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					continue;
				}

				return (T)(Object)atual;
			}

			return default(T);
		}

		// Não Gerericos
		public static Shape FindShape(this Document doc, string textoContido)
		{
			foreach (Shape item in doc.GetChildNodes(NodeType.Shape, true))
			{
                string valor = item.ToString(SaveFormat.Text);

				if (String.IsNullOrEmpty(valor) && item.TextPath != null)
				{
					valor = item.TextPath.Text;
				}

				if (valor.IndexOf(textoContido, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					continue;
				}
				return item;
			}
			return null;
		}

		public static List<Shape> FindShapes(this Document doc, string textoContido)
		{
			List<Shape> itens = new List<Shape>();

			foreach (Shape item in doc.GetChildNodes(NodeType.Shape, true))
			{
                string valor = item.ToString(SaveFormat.Text);

				if (String.IsNullOrEmpty(valor) && item.TextPath != null)
				{
					valor = item.TextPath.Text;
				}

				if (valor.IndexOf(textoContido, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					continue;
				}

				itens.Add(item);
			}
			return itens;
		}

		public static Table FindTable(this Document doc, string textoContido)
		{
			foreach (Table item in doc.GetChildNodes(NodeType.Table, true))
			{
                string valor = item.ToString(SaveFormat.Text);

				if (String.IsNullOrEmpty(valor))
				{
					continue;
				}

				valor = valor.Replace("\n", "").Replace("\r", "");

				if (valor.IndexOf(textoContido, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					continue;
				}
				return item;
			}
			return null;
		}

		public static Table LastTable(this Document doc, string textoContido)
		{
			Node[] nodes = doc.GetChildNodes(NodeType.Table, true).ToArray();

			Node retorno = nodes.LastOrDefault(x =>
			{
				Table item = x as Table;
                string valor = item.ToString(SaveFormat.Text);

				if (String.IsNullOrEmpty(valor))
				{
					return false;
				}

				valor = valor.Replace("\n", "").Replace("\r", "");
				
				if (valor.IndexOf(textoContido, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					return false;
				}

				return true;
			});
			
			return retorno as Table;
		}

		public static Table FindTable(this Table table, string textoContido)
		{
			foreach (Table item in table.GetChildNodes(NodeType.Table, true))
			{
                string valor = item.ToString(SaveFormat.Text);

				if (String.IsNullOrEmpty(valor))
				{
					continue;
				}

				valor = valor.Replace("\n", "").Replace("\r", "");

				if (valor.IndexOf(textoContido, StringComparison.InvariantCultureIgnoreCase) < 0)
				{
					continue;
				}
				return item;
			}
			return null;
		}

		public static bool IsLineEmpty(this Node node)
		{
			if (node == null)
			{
				return false;
			}


            if (node.ToString(SaveFormat.Text) == null ||
                node.ToString(SaveFormat.Text) == String.Empty ||
                node.ToString(SaveFormat.Text) == "\r\n")
			{
				return true;
			}

			return false;
		}

		public static bool IsLineTrimEmpty(this Node node)
		{
			if (node == null)
			{
				return false;
			}

            if (node.ToString(SaveFormat.Text) == null ||
                node.ToString(SaveFormat.Text).Trim() == String.Empty)
			{
				return true;
			}

			return false;
		}

		public static void RemoverParagrafos(this Table table)
		{
			foreach (Node item in table.GetChildNodes(NodeType.Table, true))
			{
				while (item.PreviousSibling.IsLineTrimEmpty())
				{
					item.PreviousSibling.Remove();
				}
			}
		}

		public static void RemoveTables(List<Table> tables)
		{
			tables.ForEach(x =>
			{
				if (x != null)
				{
					if (x.PreviousSibling.IsLineEmpty())
					{
						x.PreviousSibling.Remove();
					}
					x.Remove();
				}
			});
		}

		public static void RemoveTable(Table table)
		{
			if (table != null)
			{
				if (table.PreviousSibling.IsLineEmpty())
				{
					table.PreviousSibling.Remove();
				}
				table.Remove();
			}
		}

		public static void RemovePageBreak(this Document doc)
		{
			Node[] nodes = doc.GetChildNodes(NodeType.Paragraph, true).ToArray();
			List<Paragraph> paragraphs = nodes.Cast<Paragraph>().ToList();
			
			paragraphs.ForEach((x) => 
			{
				x.ParagraphFormat.PageBreakBefore = false;

				foreach (Run run in x.Runs)
				{
					if (run.Text.Contains(ControlChar.PageBreak))
					{
						run.Text = run.Text.Replace(ControlChar.PageBreak, string.Empty);
					}
				}
			});
		}

		public static void RemovePageBreak(this Paragraph paragraph)
		{
			paragraph.ParagraphFormat.PageBreakBefore = false;

			foreach (Run run in paragraph.Runs)
			{
				if (run.Text.Contains(ControlChar.PageBreak))
				{
					run.Text = run.Text.Replace(ControlChar.PageBreak, string.Empty);
				}
			}
		}

		private static bool TryRemovePageBreak(this Paragraph paragraph)
		{
			bool retorno = false;
			paragraph.ParagraphFormat.PageBreakBefore = false;

			foreach (Run run in paragraph.Runs)
			{
				if (run.Text.Contains(ControlChar.PageBreak))
				{
					run.Text = run.Text.Replace(ControlChar.PageBreak, string.Empty);
					retorno = true;
				}
			}
			return retorno;
		}

		public static void RemovePageBreakAnterior(this Node node)
		{
			Paragraph paragraph = node.Previous<Paragraph>(ControlChar.PageBreak);

		
			while (paragraph != null)
			{
				if (paragraph.TryRemovePageBreak())
				{
					return;
				}
				paragraph = node.Previous<Paragraph>();
			}

			Node parentNode = node.ParentNode;

			if (parentNode == null) return;

			while (true)
			{
				while (paragraph == null && parentNode.ParentNode != null)
				{
					parentNode = parentNode.ParentNode;
					paragraph = parentNode.Previous<Paragraph>(ControlChar.PageBreak);
				}

				if (paragraph == null && parentNode.ParentNode == null)
				{
					return;
				}

				while (paragraph != null)
				{
					if (paragraph.TryRemovePageBreak())
					{
						return;
					}
					paragraph = node.Previous<Paragraph>();
				}

				paragraph = null;
			}
		}
	}
}