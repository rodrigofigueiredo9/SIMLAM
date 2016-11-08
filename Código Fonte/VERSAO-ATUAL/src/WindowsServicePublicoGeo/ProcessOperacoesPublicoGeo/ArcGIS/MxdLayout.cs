using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Tecnomapas.ArcGis;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesPublicoGeo.Business.PDF.CabecalhoRodape;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesPublicoGeo.Data;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesPublicoGeo.ArcGIS
{
	internal class MxdLayout
	{
		private MxdService _mxd;
		private Rectangle _pageSize;
		private ArquivoMxd _tipo;
		private MxdLayoutDa _da;
		private string tempFile1 = string.Empty;
		private string tempFile2 = string.Empty;
		private string tempFile3 = string.Empty;

		private string _PathServico = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf('\\'));

		private string TempPath
		{
			get
			{
				string path = Path.GetTempPath() + "Tecnomapas\\" + Assembly.GetExecutingAssembly().GetName().Name + "\\";
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				return path;
			}
		}

		public MxdService Mxd
		{
			get { return _mxd; }
		}

		public Rectangle MxdPageSize
		{
			get { return _pageSize; }
			set { _pageSize = value; }
		}

		public void AbrirMxdLayout(ArquivoMxd tipo, Rectangle pageSize)
		{
			if (!File.Exists(_PathServico + "\\ArcGIS\\" + tipo.ToString() + ".mxd"))
			{
				throw new Exception("Arquivo " + tipo.ToString() + ".mxd não foi encontrado na pasta ArcGIS.");
			}

			_mxd = new MxdService(_PathServico + "\\ArcGIS\\" + tipo.ToString() + ".mxd");

			_mxd.Open();

			_pageSize = pageSize;
			_tipo = tipo;
		}

		public void FecharMxdLayout()
		{
			if (_mxd != null)
			{
				_mxd.Close();
			}
		}

		private string FormatNumber(object value, int precision = 2)
		{
			return (value != null) ? Convert.ToDecimal(value).ToString("N" + precision) : "";
		}

		public List<LayerItem> ObterLayers()
		{
			if (_tipo == ArquivoMxd.MAPA_DOMINIALIDADE)
			{
				return new List<LayerItem>()
				{
					new LayerItem(){ Grupo=1, Name="Vértice matrícula/posse", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Nascente", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Rio", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Linha de transm.", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Ferrovia", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Estrada", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Duto", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Rocha", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Massa d'água", Query="PROJETO={0} and TIPO= 'MASSA_DAGUA_APMP'"},
					new LayerItem(){ Grupo=1, Name="Escarpa", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Rest. de declividade", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Área construída", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="ARL em APP", Query="PROJETO={0} and TIPO = 'APP_ARL'"},
					new LayerItem(){ Grupo=1, Name="RPPN", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Duna", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Faixa de servidão", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="APP preservada", Query="PROJETO={0} and TIPO = 'APP_AVN'"},
					new LayerItem(){ Grupo=1, Name="APP em recuperação", Query="PROJETO={0} and TIPO = 'APP_AA_REC'"},
					new LayerItem(){ Grupo=1, Name="APP em uso", Query="PROJETO={0} and TIPO = 'APP_AA_USO'"},
					new LayerItem(){ Grupo=1, Name="APP não caracterizada", Query="PROJETO={0} and TIPO = 'APP_APMP'"},
					new LayerItem(){ Grupo=1, Name="ARL preservada", Query="PROJETO={0} and SITUACAO = 'PRESERV'"},
					new LayerItem(){ Grupo=1, Name="ARL em recuperação", Query="PROJETO={0} and SITUACAO = 'REC'"},
					new LayerItem(){ Grupo=1, Name="ARL em uso", Query="PROJETO={0} and SITUACAO = 'USO'"},
					new LayerItem(){ Grupo=1, Name="ARL não caracterizada", Query="PROJETO={0} and SITUACAO = 'D'"},
					new LayerItem(){ Grupo=1, Name="Área de vegetação nativa", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Área alterada", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Área de matrícula/posse", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Área total da propriedade", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=1, Name="Faixa de domínio", Query="PROJETO={0}"}
				};
			}

			//if (_tipo == ArquivoMxd.MAPA_ATIVIDADE)
			//{
			//	return new List<LayerItem>()
			//	{
			//		new LayerItem(){ Grupo=2, Name="Ponto da atividade", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=2, Name="Linha da atividade", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=2, Name="Área de influência da atividade", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=2, Name="Área da atividade", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Vértice matrícula/posse", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Nascente", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Rio", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Linha de transm.", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Ferrovia", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Estrada", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Duto", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Rocha", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Massa d'água", Query="PROJETO={0} and TIPO= 'MASSA_DAGUA_APMP'"},
			//		new LayerItem(){ Grupo=1, Name="Escarpa", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Rest. de declividade", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Área construída", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="ARL em APP", Query="PROJETO={0} and TIPO = 'APP_ARL"},
			//		new LayerItem(){ Grupo=1, Name="RPPN", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Duna", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Faixa de servidão", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="APP preservada", Query="PROJETO={0} and TIPO = 'APP_AVN'"},
			//		new LayerItem(){ Grupo=1, Name="APP em recuperação", Query="PROJETO={0} and TIPO = 'APP_AA_REC'"},
			//		new LayerItem(){ Grupo=1, Name="APP em uso", Query="PROJETO={0} and TIPO = 'APP_AA_USO'"},
			//		new LayerItem(){ Grupo=1, Name="APP não caracterizada", Query="PROJETO={0} and TIPO = 'APP_APMP'"},
			//		new LayerItem(){ Grupo=1, Name="ARL preservada", Query="PROJETO={0} and SITUACAO = 'PRESERV'"},
			//		new LayerItem(){ Grupo=1, Name="ARL em recuperação", Query="PROJETO={0} and SITUACAO = 'REC'"},
			//		new LayerItem(){ Grupo=1, Name="ARL em uso", Query="PROJETO={0} and SITUACAO = 'USO'"},
			//		new LayerItem(){ Grupo=1, Name="ARL não caracterizada", Query="PROJETO={0} and SITUACAO = 'D'"},
			//		new LayerItem(){ Grupo=1, Name="Área de vegetação nativa", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Área alterada", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Área de matrícula/posse", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Área total da propriedade", Query="PROJETO={0}"},
			//		new LayerItem(){ Grupo=1, Name="Faixa de domínio", Query="PROJETO={0}"}
			//	};
			//}

			return null;
		}

		internal void GerarPdf(Document doc, PdfWriter wrt, Int32 id, Hashtable hashData)
		{
			GerarPdf(doc, wrt, id, hashData, true);
		}

		internal void GerarPdf(Document doc, PdfWriter wrt, Int32 projetoId, Hashtable hashData, bool isUsePageEvent)
		{
			tempFile1 = TempPath + Path.GetRandomFileName() + ".pdf";
			tempFile2 = TempPath + Path.GetRandomFileName() + ".pdf";
			tempFile3 = TempPath + Path.GetRandomFileName() + ".pdf";

			_da = new MxdLayoutDa();
			_mxd.ZoomPercentage(1);

			if (_tipo == ArquivoMxd.MAPA_DOMINIALIDADE)
			{
				//Aplicando filtros
				_mxd.ActivateMap("MAPA_PRINCIPAL");

				foreach (var layer in ObterLayers())
				{
					_mxd.SetQueryDefinition(layer.Name, String.Format(layer.Query, projetoId));
					layer.Source = _mxd.GetDataSource(layer.Name);

					if (String.IsNullOrEmpty(layer.Query))
					{
						_mxd.SetLayerVisibility(layer.Name, true);
					}
					else
					{
						_mxd.SetLayerVisibility(layer.Name, _da.ObterQuantidade(layer, projetoId) > 0);
					}
				}

				_mxd.ActivateMap("MINI_MAPA");
				_mxd.SetQueryDefinition("Ponto empreendimento", "PROJETO=" + projetoId.ToString());

				string[] coord = hashData["COORDENADA"].ToString().Split(';');
				int centerX = Convert.ToInt32(coord[0]);
				int centerY = Convert.ToInt32(coord[1]);
				_mxd.ZoomToEnvelope(new Envelope(centerX - 2000, centerX + 2000, centerY - 2000, centerY + 2000));
				_mxd.ZoomToBestScale("MINI_MAPA", 800000D);

				_mxd.ActivateMap("MAPA_PRINCIPAL");

				//Setar as informações no mxd
				_mxd.SetElementText("municipio", hashData["MUNICIPIO"].ToString());
				_mxd.SetElementText("uf", hashData["UF"].ToString());
				_mxd.SetElementText("data", DateTime.Today.ToString("dd/MM/yyyy"));
				_mxd.SetElementText("precisao", hashData["PRECISAO"].ToString());

				//Definir Zoom
				_mxd.ZoomToFeature("Área total da propriedade", "PROJETO", projetoId.ToString());
				_mxd.ZoomPercentage(1.1);
				_mxd.ZoomToBestScale("MAPA_PRINCIPAL", 250D);
				_mxd.AdjustGrid("MAPA_PRINCIPAL", 3, 3);

				_mxd.ActivateMap("MINI_MAPA");
				coord = hashData["COORDENADA"].ToString().Split(';');
				centerX = Convert.ToInt32(coord[0]);
				centerY = Convert.ToInt32(coord[1]);
				_mxd.ZoomToEnvelope(new Envelope(centerX - 2000, centerX + 2000, centerY - 2000, centerY + 2000));
				_mxd.ZoomToBestScale("MINI_MAPA", 800000D);

				_mxd.ActivateMap("MAPA_PRINCIPAL");

				//----------------------------------------
				//Mapa Tematico
				_mxd.SetLayerVisibility("IMAGEM", false);
				_mxd.RefreshLegends(scaleSymbols: true);

				_mxd.SetElementText("nome_croqui", "Croqui da Dominialidade");
				_mxd.SetElementText("imagem", String.Empty);
				_mxd.ExportToPDF(tempFile1, 300);

				loadPdfToDocument(doc, wrt, tempFile1, isUsePageEvent);

				doc.NewPage();
				//----------------------------------------

				//----------------------------------------
				//Mapa Imagem
				_mxd.SetLayerVisibility("IMAGEM", true);
				_mxd.RefreshLegends(scaleSymbols: true);


				_mxd.SetElementText("nome_croqui", "Croqui com Imagem");
				_mxd.SetElementText("imagem", "Aerolevantamento: ano 2007/2008");
				_mxd.ExportToPDF(tempFile2, 300);

				loadPdfToDocument(doc, wrt, tempFile2, isUsePageEvent);

				doc.NewPage();
				//----------------------------------------
			}
		}

		private void loadPdfToDocument(Document doc, PdfWriter wrt, String fileUrl, bool isUsePageEvent)
		{
			FileStream fs = File.OpenRead(fileUrl);
			try
			{
				PdfReader reader = new PdfReader(fs);

				PdfContentByte cb;
				PdfImportedPage page;
				Rectangle psizeOrg = doc.PageSize;

				float TopMargin = doc.TopMargin;
				float BottomMargin = doc.BottomMargin;
				float LeftMargin = doc.LeftMargin;
				float RightMargin = doc.RightMargin;

				for (int i = 1; i <= reader.NumberOfPages; i++)
				{
					Rectangle pageMeasures = reader.GetPageSize(i);
					float px = (this._pageSize.Width - pageMeasures.Width) / 2;
					float py = (this._pageSize.Height - pageMeasures.Height) / 2;

					if (px < 0 || py < 0)
					{
						px = 0;
						py = 0;
					}

					IPageMirroring pageEventMirroring = wrt.PageEvent as IPageMirroring;
					if (pageEventMirroring != null && pageEventMirroring.IsPageMirroring)
					{
						px = px - (doc.LeftMargin - (((wrt.PageNumber % 2) != 0) ? doc.LeftMargin : doc.RightMargin));
					}

					if (!isUsePageEvent)
					{
						wrt.PageEvent = null;
					}

					cb = wrt.DirectContent;
					cb.SaveState();

					page = wrt.GetImportedPage(reader, i);

					cb.AddTemplate(page, px, py);

					cb.RestoreState();
				}

			}
			finally
			{
				if (fs != null)
				{
					fs.Close();
					fs.Dispose();
				}
			}
		}

		internal void ApagarTempFile()
		{
			if (File.Exists(tempFile1))
				File.Delete(tempFile1);

			if (File.Exists(tempFile2))
				File.Delete(tempFile2);

			if (File.Exists(tempFile3))
				File.Delete(tempFile3);
		}
	}
}