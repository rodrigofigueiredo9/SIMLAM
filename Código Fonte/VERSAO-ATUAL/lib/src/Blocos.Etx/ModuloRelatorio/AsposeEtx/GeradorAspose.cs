using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Aspose.Words;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Document = Aspose.Words.Document;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx
{
	public class GeradorAspose
	{
		IConfiguradorPdf _configuracao = new ConfiguracaoDefault();
		Document _doc = null;

		public GeradorAspose()
		{
		}

		public GeradorAspose(IConfiguradorPdf configuracao)
		{
			_configuracao = configuracao;
		}

		public MemoryStream PdfAnexo<T>(Arquivo.Arquivo arquivoTemplate, T dataSource, string documento)
		{
			MemoryStream file = null;
			try
			{
				_doc = new Document(arquivoTemplate.Buffer);

				_configuracao.Load(_doc, dataSource);
				_configuracao.Configurar(_doc);

				_doc.MailMerge.FieldMergingCallback = new HandleField();
				_doc.NodeChangingCallback = new HandleNodeChanging();

				ObjectMailMerge objDataSourceCabecalhoRodape = new ObjectMailMerge(_configuracao.CabecalhoRodape);

				_doc.MailMerge.Execute(objDataSourceCabecalhoRodape);

				dataSource = Assinantes(_configuracao.Assinantes, dataSource);

				ObjectMailMerge objDataSource = new ObjectMailMerge(dataSource);
				_doc.MailMerge.ExecuteWithRegions(objDataSource);

				ObjectMailMerge objDataSourceAnexo = new ObjectMailMerge(dataSource, documento);
				_doc.MailMerge.ExecuteWithRegions(objDataSourceAnexo);

				_configuracao.Executed(_doc, dataSource);

				file = new MemoryStream();
				_doc.Save(file, SaveFormat.Pdf);
			}
			finally
			{
				if (arquivoTemplate.Buffer != null)
				{
					arquivoTemplate.Buffer.Close();
				}
			}

			return file;
		}

		public System.Int32 PdfAnexoDoc<T>(Arquivo.Arquivo arquivoTemplate, T dataSource, string documento)
		{
			try
			{
				var local = new Document(arquivoTemplate.Buffer);

				_configuracao.Load(local, dataSource);
				_configuracao.Configurar(local);

				local.MailMerge.FieldMergingCallback = new HandleField();
				local.NodeChangingCallback = new HandleNodeChanging();

				ObjectMailMerge objDataSourceCabecalhoRodape = new ObjectMailMerge(_configuracao.CabecalhoRodape);

				local.MailMerge.Execute(objDataSourceCabecalhoRodape);

				dataSource = Assinantes(_configuracao.Assinantes, dataSource);

				ObjectMailMerge objDataSource = new ObjectMailMerge(dataSource);
				local.MailMerge.ExecuteWithRegions(objDataSource);

				ObjectMailMerge objDataSourceAnexo = new ObjectMailMerge(dataSource, documento);
				local.MailMerge.ExecuteWithRegions(objDataSourceAnexo);

				_configuracao.Executed(local, dataSource);

				return local.PageCount;
			}
			finally
			{
			}
		}

		public MemoryStream Pdf<T>(Arquivo.Arquivo arquivoTemplate, T dataSource)
		{
			MemoryStream file = null;
			try
			{
				_doc = new Document(arquivoTemplate.Buffer);

				_configuracao.Load(_doc, dataSource);
				_configuracao.Configurar(_doc);

				_doc.MailMerge.FieldMergingCallback = new HandleField();
				_doc.NodeChangingCallback = new HandleNodeChanging();

				ObjectMailMerge objDataSourceCabecalhoRodape = new ObjectMailMerge(_configuracao.CabecalhoRodape);

				_doc.MailMerge.Execute(objDataSourceCabecalhoRodape);

				dataSource = Assinantes(_configuracao.Assinantes, dataSource);

				if (dataSource is DataSet)
				{
					_doc.MailMerge.ExecuteWithRegions((dataSource as DataSet));
				}
				else
				{
					ObjectMailMerge objDataSource = new ObjectMailMerge(dataSource); 
					_doc.MailMerge.ExecuteWithRegions(objDataSource);
				}

				_configuracao.Executed(_doc, dataSource);

				file = new MemoryStream();
				_doc.Save(file, SaveFormat.Pdf);
			}
			finally
			{
				if (arquivoTemplate.Buffer != null)
				{
					arquivoTemplate.Buffer.Close();
				}
			}

			return file;
		}

		public static T Assinantes<T>(List<IAssinante> lstAssinantes, T dataSoruce)
		{
			if (dataSoruce == null || lstAssinantes == null || lstAssinantes.Count == 0)
			{
				return dataSoruce;
			}

			IAssinante Assinante = null;
			List<IAssinante> Assinantes1 = new List<IAssinante>();
			List<IAssinante> Assinantes2 = new List<IAssinante>();

			if ((lstAssinantes.Count % 2) != 0)
			{
				Assinante = lstAssinantes.Last();
				lstAssinantes.Remove(Assinante);
			}

			if (lstAssinantes.Count > 0)
			{
				lstAssinantes.ForEach(assinante =>
				{
					if ((lstAssinantes.IndexOf(assinante) % 2) == 0)
					{
						Assinantes1.Add(assinante);
					}
					else
					{
						Assinantes2.Add(assinante);
					}
				});
			}

			IAssinanteDataSource assDataSource;
			dynamic dyData = dataSoruce;

			if (dataSoruce is IAssinanteDataSource)
			{
				assDataSource = dataSoruce as IAssinanteDataSource;
			}
			else
			{
				assDataSource = dyData.Titulo as IAssinanteDataSource;
			}

			assDataSource.Assinante = Assinante;
			assDataSource.Assinantes1 = Assinantes1;
			assDataSource.Assinantes2 = Assinantes2;

			return (T)dyData;
		}

		public static MemoryStream AnexarPdf(MemoryStream pdfAspose, List<Arquivo.Arquivo> arquivo)
		{
			if (arquivo == null || arquivo.Count == 0)
			{
				return pdfAspose;
			}

			iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4);

			MemoryStream ms = new MemoryStream();
			PdfWriter wrt = PdfWriter.GetInstance(doc, ms);

			doc.Open();

			PdfMetodosAuxiliares.AnexarPdf(pdfAspose, doc, wrt);

			PdfMetodosAuxiliares.AnexarPdf(arquivo, doc, wrt);

			doc.Close();
			doc.Dispose();

			//cria um Stream de saida que mantenha o fluxo aberto.
			MemoryStream msOut = new MemoryStream(ms.ToArray());
			ms.Close();
			ms.Dispose();

			return msOut;
		}

		public static void Autorizacao()
		{
			//License license = new License();
			//license.SetLicense("Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.Aspose.Words.lic");
		}
	}
}