using System.Collections.Generic;
using System.IO;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloProtocolo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloProtocolo.Pdf
{
	public class PdfProtocoloRecebimento : PdfPadraoRelatorio
	{
		public MemoryStream Gerar(int id)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Registro_Recebimento.docx";

			RelatorioRecebimentoDa _da = new RelatorioRecebimentoDa();

			ObterArquivoTemplate();

			ProtocoloRelatorio dataSource = _da.Obter(id);

			#region Configurar Cabecario Rodapé

			ConfigurarCabecarioRodape(dataSource.SetorId);

			#endregion

			dataSource.Titulo = "Registro de Recebimento";
			
			ConfiguracaoDefault.AddLoadAcao((doc, dataSourceCnf) =>
			{
				List<Row> itensRemover = new List<Row>();

				if (dataSource.ProtocoloProcDoc == (int)eTipoProtocolo.Processo)
				{
					#region Processo

					switch ((eProtocoloTipoRelatorio)dataSource.ProtocoloTipo)
					{
						case eProtocoloTipoRelatorio.Fiscalizacao:
							dataSource.Titulo = "Registro de Fiscalização";

							itensRemover.Add(doc.Last<Row>("Objetivo:"));
							itensRemover.Add(doc.Last<Row>("Nome:"));
							itensRemover.Add(doc.Last<Row>("Interessado:"));
							itensRemover.Add(doc.Last<Row>("Empreendimento:"));
							itensRemover.Add(doc.Last<Row>("Checagem de Itens:"));
							itensRemover.Add(doc.Last<Row>("Checagem de Pendência:"));
							itensRemover.Add(doc.Last<Row>("Nº de registro de"));
							break;

						default:
							itensRemover.Add(doc.Last<Row>("Nome:"));
							itensRemover.Add(doc.Last<Row>("Nº Fiscalização"));
							itensRemover.Add(doc.Last<Row>("Checagem de Pendência:"));
							itensRemover.Add(doc.Last<Row>("Nº de registro de"));
							itensRemover.Add(doc.Last<Row>("Autuado:"));
							break;
					}

					#endregion
				}
				else
				{
					#region Documento

					switch ((eProtocoloTipoRelatorio)dataSource.ProtocoloTipo)
					{
						case eProtocoloTipoRelatorio.Complementacao:
							itensRemover.Add(doc.Last<Row>("Objetivo:"));
							itensRemover.Add(doc.Last<Row>("Autuado:"));
							itensRemover.Add(doc.Last<Row>("Empreendimento:"));
							itensRemover.Add(doc.Last<Row>("Checagem de Itens:"));
							itensRemover.Add(doc.Last<Row>("Nº Fiscalização"));
							break;

						case eProtocoloTipoRelatorio.Oficio:
						case eProtocoloTipoRelatorio.CartaConsulta:
							itensRemover.Add(doc.Last<Row>("Objetivo:"));
							itensRemover.Add(doc.Last<Row>("Autuado:"));
							itensRemover.Add(doc.Last<Row>("Empreendimento:"));
							itensRemover.Add(doc.Last<Row>("Checagem de Itens:"));
							itensRemover.Add(doc.Last<Row>("Nº Fiscalização"));
							itensRemover.Add(doc.Last<Row>("Checagem de Pendência:"));
							itensRemover.Add(doc.Last<Row>("Nº de registro de"));
							break;

						case eProtocoloTipoRelatorio.OficioUsucapiao:
						case eProtocoloTipoRelatorio.Requerimento:
							itensRemover.Add(doc.Last<Row>("Nome:"));
							itensRemover.Add(doc.Last<Row>("Autuado:"));
							itensRemover.Add(doc.Last<Row>("Nº Fiscalização"));
							itensRemover.Add(doc.Last<Row>("Checagem de Pendência:"));
							itensRemover.Add(doc.Last<Row>("Nº de registro de"));
							break;

						case eProtocoloTipoRelatorio.Condicionante:
						case eProtocoloTipoRelatorio.DefesaAdministrativa:
							itensRemover.Add(doc.Last<Row>("Objetivo:"));
							itensRemover.Add(doc.Last<Row>("Autuado:"));
							itensRemover.Add(doc.Last<Row>("Empreendimento:"));
							itensRemover.Add(doc.Last<Row>("Checagem de Itens:"));
							itensRemover.Add(doc.Last<Row>("Nº Fiscalização"));
							itensRemover.Add(doc.Last<Row>("Checagem de Pendência:"));
							break;

						case eProtocoloTipoRelatorio.Declaracao:
							itensRemover.Add(doc.Last<Row>("Nome:"));
							itensRemover.Add(doc.Last<Row>("Autuado:"));
							itensRemover.Add(doc.Last<Row>("Nº Fiscalização"));
							itensRemover.Add(doc.Last<Row>("Checagem de Pendência:"));
							itensRemover.Add(doc.Last<Row>("Nº de registro de"));
							break;

						case eProtocoloTipoRelatorio.FiscalizacaoSem_AI_TEI_TAD:
							dataSource.Titulo = "Registro de Fiscalização";

							itensRemover.Add(doc.Last<Row>("Objetivo:"));
							itensRemover.Add(doc.Last<Row>("Nome:"));
							itensRemover.Add(doc.Last<Row>("Interessado:"));
							itensRemover.Add(doc.Last<Row>("Empreendimento:"));
							itensRemover.Add(doc.Last<Row>("Checagem de Itens:"));
							itensRemover.Add(doc.Last<Row>("Checagem de Pendência:"));
							itensRemover.Add(doc.Last<Row>("Nº de registro de"));
							break;

						default:
							break;
					}

					#endregion
				}

				itensRemover.ForEach(x => x.Remove());
			});

			return GerarPdf(dataSource);
		}
	}
}
