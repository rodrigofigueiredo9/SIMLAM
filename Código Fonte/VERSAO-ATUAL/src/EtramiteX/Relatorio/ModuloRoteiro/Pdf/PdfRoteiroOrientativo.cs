using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRoteiro.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRoteiro.Pdf.RoteiroPdf
{
	public class PdfRoteiroOrientativo : PdfPadraoRelatorio
	{
		public MemoryStream Gerar(int id, string tid)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Roteiro.doc";

			RoteiroDa _da = new RoteiroDa();
			RoteiroRelatorio dataSource = (tid == string.Empty) ? _da.Obter(id) : _da.Obter(id, tid);
			dataSource.ObservacoesHtml = dataSource.Observacoes;
	
			dataSource.ItensTecnicos = dataSource.Itens.Where(x => x.Tipo == 1).OrderBy(x => x.Ordem).ToList();
			dataSource.ItensAdministrativos = dataSource.Itens.Where(x => x.Tipo == 2).OrderBy(x => x.Ordem).ToList();
			
			int num = 1;
			dataSource.ItensTecnicos.ForEach(x => x.Numero = num++);
			num = 1;
			dataSource.ItensAdministrativos.ForEach(x => x.Numero = num++);
			

			ObterArquivoTemplate();

			#region Configurar Assinantes

			ConfiguracaoDefault.TextoTagAssinante = "«Assinante.Nome»";
			ConfiguracaoDefault.TextoTagAssinantes1 = "«TableStart:Assinantes1»";
			ConfiguracaoDefault.TextoTagAssinantes2 = "«TableStart:Assinantes2»";

			#endregion

			#region Configurar Cabecalho Rodapé

			ConfigurarCabecarioRodape(dataSource.SetorId);

			#endregion

			#region Configurar Tabelas

			ConfiguracaoDefault.AddLoadAcao((doc, dataSrc) =>
			{
				List<Table> tabelas = new List<Table>();

				if (dataSource.ItensTecnicos == null || dataSource.ItensTecnicos.Count <= 0)
				{
					tabelas.Add(doc.LastTable("«TableStart:ItensTecnicos»"));
				}

				if (dataSource.ItensAdministrativos == null || dataSource.ItensAdministrativos.Count <= 0)
				{
					tabelas.Add(doc.LastTable("«TableStart:ItensAdministrativos»"));
				}

				if (dataSource.Anexos == null || dataSource.Anexos.Count <= 0)
				{
					tabelas.Add(doc.FindTable("«TableEnd:Anexos»"));
				}

				if (string.IsNullOrEmpty(dataSource.ObservacoesHtml))
				{
					tabelas.Add(doc.FindTable("«ObservacoesHtml»"));
				}

				AsposeExtensoes.RemoveTables(tabelas);
			});

			#endregion

			#region Assinantes

			/*AssinanteDefault assinante = null;

			if (dataSource.Interessado.Id > 0)
			{
				assinante = new AssinanteDefault();
				assinante.Nome = dataSource.Interessado.NomeRazaoSocial;
				ConfiguracaoDefault.Assinantes.Add(assinante);
			}

			foreach (ResponsavelTecnico responsavel in dataSource.Responsaveis)
			{
				assinante = new AssinanteDefault();
				assinante.Nome = responsavel.NomeRazao;
				ConfiguracaoDefault.Assinantes.Add(assinante);
			}*/

			#endregion

			return MergePdf(GerarPdf(dataSource), dataSource.Anexos.Select(x => x.Arquivo).ToList());
		}

		#region ITextSharp
		
		//public MemoryStream GerarRoteiroPdf(int id, string tid)
		//{
		//    RoteiroDa _da = new RoteiroDa();

		//    Roteiro roteiro = (tid == string.Empty) ? _da.Obter(id) : _da.Obter(id, tid);

		//    Document doc = new Document(PageSize.A4, 45, 45, 85, 50);
		//    MemoryStream str = new MemoryStream();

		//    PdfWriter wrt = PdfWriter.GetInstance(doc, str);

		//    wrt.SetEncryption(PdfWriter.STRENGTH128BITS, null, null, PdfWriter.AllowPrinting);

		//    wrt.PageEvent = CabecalhoRodapeFactory.Criar(roteiro.SetorId);

		//    doc.Open();

		//    PdfMetodosAuxiliares.AddTituloData(doc, "Roteiro Orientativo");

		//    PdfPTable tabelaDocumento = PdfMetodosAuxiliares.CriarTabela(2, new float[] { 80, 20 });
		//    PdfPTable tabelaInterior = PdfMetodosAuxiliares.CriarTabela();

		//    tabelaDocumento.AddCell(PdfMetodosAuxiliares.AddCampoValor("Nome", roteiro.Nome));

		//    tabelaDocumento.DefaultCell.Padding = 0;
		//    tabelaInterior.DefaultCell.Padding = 0.5f;

		//    tabelaInterior.AddCell(PdfMetodosAuxiliares.AddCampoValor("Número", roteiro.Numero.ToString()));
		//    tabelaInterior.AddCell(PdfMetodosAuxiliares.AddCampoValor("Versão", roteiro.Versao.ToString()));

		//    tabelaDocumento.AddCell(tabelaInterior);

		//    doc.Add(tabelaDocumento);

		//    PdfMetodosAuxiliares.PularLinha(doc);

		//    tabelaDocumento.DefaultCell.Padding = 0.5f;

		//    tabelaDocumento = GerarTabelaItens(roteiro.Itens.FindAll(x => x.Tipo == 2), "Itens Administrativos");

		//    doc.Add(tabelaDocumento);

		//    PdfMetodosAuxiliares.PularLinha(doc);

		//    tabelaDocumento = GerarTabelaItens(roteiro.Itens.FindAll(x => x.Tipo == 1), "Itens Técnicos");

		//    doc.Add(tabelaDocumento);

		//    PdfMetodosAuxiliares.PularLinha(doc);

		//    tabelaDocumento = PdfMetodosAuxiliares.CriarTabela();

		//    PdfMetodosAuxiliares.AddCabecarioTabela(tabelaDocumento, "Observações");

		//    tabelaDocumento.AddCell(PdfMetodosAuxiliares.AddTexto(roteiro.Observacoes, PdfMetodosAuxiliares.arial10));


		//    doc.Add(tabelaDocumento);

		//    if (roteiro.Anexos.Count > 0 && roteiro.Anexos.Exists(y => y.Arquivo.Extensao == ".pdf"))
		//    {
		//        doc.NewPage();

		//        tabelaDocumento = PdfMetodosAuxiliares.CriarTabela();
		//        tabelaDocumento.DefaultCell.Border = 0;

		//        PdfMetodosAuxiliares.AddTituloData(doc, "Anexos");

		//        PdfMetodosAuxiliares.PularLinha(doc);

		//        for (int i = 0; i < roteiro.Anexos.Count; i++)
		//        {
		//            if (roteiro.Anexos[i].Arquivo.Extensao.ToLower() == ".pdf")
		//            {
		//                Phrase frase = PdfMetodosAuxiliares.AddTexto((i + 1) + " - ", PdfMetodosAuxiliares.arial10);
		//                frase.Add(PdfMetodosAuxiliares.AddTextoChunk(roteiro.Anexos[i].Descricao, PdfMetodosAuxiliares.arial10));
		//                tabelaDocumento.AddCell(frase);
		//            }
		//        }

		//        doc.Add(tabelaDocumento);
		//        doc.NewPage();

		//        for (int i = 0; i < roteiro.Anexos.Count; i++)
		//        {
		//            PdfMetodosAuxiliares.AnexarPdf(roteiro.Anexos[i].Arquivo, doc, wrt);
		//        }
		//    }

		//    doc.Close();
		//    doc.Dispose();

		//    return str;
		//}
		//private  PdfPTable GerarTabelaItens(List<Item> itens, string tituloTabela)
		//{
		//    PdfPTable tabela = PdfMetodosAuxiliares.CriarTabela(3, new float[] { 10, 45, 45 });
		//    tabela.DefaultCell.Colspan = 3;

		//    PdfMetodosAuxiliares.AddCabecarioTabela(tabela, tituloTabela);

		//    tabela.DefaultCell.Colspan = 1;

		//    for (int i = 0; i < itens.Count; i++)
		//    {
		//        tabela.AddCell(PdfMetodosAuxiliares.AddTexto((i + 1).ToString(), PdfMetodosAuxiliares.arial10));
		//        tabela.AddCell(PdfMetodosAuxiliares.AddTexto(itens[i].Nome, PdfMetodosAuxiliares.arial10));
		//        tabela.AddCell(PdfMetodosAuxiliares.AddTexto(itens[i].Condicionante, PdfMetodosAuxiliares.arial10));
		//    }

		//    return tabela;
		//} 
		 
		#endregion
	}
}