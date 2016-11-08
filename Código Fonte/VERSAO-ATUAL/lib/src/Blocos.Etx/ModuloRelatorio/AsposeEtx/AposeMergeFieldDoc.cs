using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Aspose.Words;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx
{
	public class AposeMergeFieldDoc
	{
		Document Doc {get;set;}
		DocumentBuilder Builder{get;set;}
		
		public void Abrir()
		{
			Doc = new Document();
			Builder = new DocumentBuilder(Doc);
		}

		public MemoryStream Retornar()
		{
			MemoryStream ms = new MemoryStream();
			Doc.Save(ms, SaveFormat.Docx);
			return ms;
		}

		public void Gerar(Type tipo)
		{
			List<string> keyObjetos = new List<string>();

			Builder.Writeln(" ################# " + tipo.Name + " ################# ");

			Imprimir(tipo.GetProperties(), keyObjetos, new List<string>());

			foreach (var item in keyObjetos)
			{
				if (item.IndexOf("TableStar") >= 0)
				{
					Builder.InsertBreak(BreakType.LineBreak);
				}
				Builder.InsertField(String.Format(@"MERGEFIELD {0} \* MERGEFORMAT", item));
				Builder.InsertBreak(BreakType.LineBreak);

				if (item.IndexOf("TableEnd") >= 0)
				{
					Builder.InsertBreak(BreakType.LineBreak);
				}
			}
		}

		private static void Imprimir(PropertyInfo[] propertyInfo, List<string> strBuilder, List<string> pai, bool isHieraquia = true)
		{
			List<String> lstLocalPai = (isHieraquia) ? pai : new List<String>();
			
			foreach (var item in propertyInfo)
			{
				if (!item.PropertyType.IsClass || item.PropertyType.IsPrimitive || item.PropertyType.Name == "String")
				{
					if (lstLocalPai.Count > 0)
					{
						List<string> lstString = lstLocalPai.ToList();
						lstString.Add(item.Name);
						strBuilder.Add(String.Join(".", lstString.ToArray()));
					}
					else
					{
						strBuilder.Add(item.Name);
					}
				}
				else
				{
					if (pai.Count(x => x.Equals(item.Name)) > 0)
					{
						continue;
					}

					List<string> atual = lstLocalPai.Take(lstLocalPai.Count).ToList();

					if (item.PropertyType.IsGenericType)
					{
						List<string> tableStartPath = atual.Take(lstLocalPai.Count).ToList();

						tableStartPath.Add(item.Name);

						int qtdStart = strBuilder.Count(x => x == String.Format("TableStart:{0}", String.Join(".", tableStartPath.ToArray())));
						int qtdEnd = strBuilder.Count(x => x == String.Format("TableEnd:{0}", String.Join(".", tableStartPath.ToArray())));

						if (qtdStart != qtdEnd)
						{
							strBuilder.Add(String.Format("TAG Ciclica:{0}", String.Join(".", tableStartPath.ToArray())));
							continue;
						}

						strBuilder.Add(String.Format("TableStart:{0}", String.Join(".", tableStartPath.ToArray())));

						Type[] itemGeneric = item.PropertyType.GetGenericArguments();						

						foreach (var genr in itemGeneric)
						{
							Imprimir(genr.GetProperties(), strBuilder, atual, false);
						}

						strBuilder.Add(String.Format("TableEnd:{0}", String.Join(".", tableStartPath.ToArray())));
					}
					else
					{
						atual.Add(item.Name);
						Imprimir(item.PropertyType.GetProperties(), strBuilder, atual);
					}
				}
			}
		}
	}
}
