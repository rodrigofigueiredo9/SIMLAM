using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Tecnomapas.ArcGis;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Business.PDF.CabecalhoRodape;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Data;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.ArcGIS
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

		internal void GerarPdf(Document doc, PdfWriter wrt, Int32 id, Hashtable hashData)
		{
			GerarPdf(doc, wrt, id, hashData, true);
		}

		private string FormatNumber(object value, int precision = 2)
		{
			return (value != null) ? Convert.ToDecimal(value).ToString("N" + precision) : "";
		}

		public List<LayerItem> ObterLayers()
		{
			if (_tipo == ArquivoMxd.MAPA_DOMINIALIDADE ||
				_tipo == ArquivoMxd.MAPA_PECA_TECNICA)
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

			if (_tipo == ArquivoMxd.MAPA_ATIVIDADE)
			{
				return new List<LayerItem>()
				{
					new LayerItem(){ Grupo=2, Name="Ponto da atividade", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=2, Name="Linha da atividade", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=2, Name="Área de influência da atividade", Query="PROJETO={0}"},
					new LayerItem(){ Grupo=2, Name="Área da atividade", Query="PROJETO={0}"},
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
					new LayerItem(){ Grupo=1, Name="ARL em APP", Query="PROJETO={0} and TIPO = 'APP_ARL"},
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

			if (_tipo == ArquivoMxd.MAPA_CAR)
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
					new LayerItem(){ Grupo=1, Name="APP a recuperar (Calculado)", Query="PROJETO={0} and TIPO='CAR_APP_AA_USO'"},
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

			return null;
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
				_mxd.SetQueryDefinition("Ponto empreendimento", "EMPREENDIMENTO=" + hashData["EMPREENDIMENTO"]);
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
				string[] coord = hashData["COORDENADA"].ToString().Split(';');
				int centerX = Convert.ToInt32(coord[0]);
				int centerY = Convert.ToInt32(coord[1]);
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
			else if (_tipo == ArquivoMxd.MAPA_ATIVIDADE)
			{
				int dominialidade = Convert.ToInt32(hashData["DOMINIALIDADE"]);

				//Aplicando filtros
				_mxd.ActivateMap("MAPA_PRINCIPAL");

				foreach (var layer in ObterLayers())
				{
					_mxd.SetLayerVisibility(layer.Name, true);
					if (layer.Grupo == 1)//Dominialidade
					{
						_mxd.SetQueryDefinition(layer.Name, String.Format(layer.Query, dominialidade));
					}
					else
					{
						_mxd.SetQueryDefinition(layer.Name, String.Format(layer.Query, projetoId));
					}
				}

				_mxd.ActivateMap("MINI_MAPA");
				_mxd.SetQueryDefinition("Ponto empreendimento", "EMPREENDIMENTO=" + hashData["EMPREENDIMENTO"]);

				//Setar as informações no mxd
				_mxd.SetElementText("atividade", hashData["ATIVIDADE"].ToString());
				_mxd.SetElementText("municipio", hashData["MUNICIPIO"].ToString());
				_mxd.SetElementText("uf", hashData["UF"].ToString());
				_mxd.SetElementText("data", DateTime.Today.ToString("dd/MM/yyyy"));
				_mxd.SetElementText("precisao", hashData["PRECISAO"].ToString());

				//Definir Zoom
				_mxd.ZoomToFeature("Área total da propriedade", "PROJETO", dominialidade.ToString());
				_mxd.ZoomPercentage(1.5);
				_mxd.ZoomToBestScale("MAPA_PRINCIPAL", 250D);
				_mxd.AdjustGrid("MAPA_PRINCIPAL", 3, 3);

				_mxd.ActivateMap("MINI_MAPA");
				string[] coord = hashData["COORDENADA"].ToString().Split(';');
				int centerX = Convert.ToInt32(coord[0]);
				int centerY = Convert.ToInt32(coord[1]);
				_mxd.ZoomToEnvelope(new Envelope(centerX - 2000, centerX + 2000, centerY - 2000, centerY + 2000));
				_mxd.ZoomToBestScale("MINI_MAPA", 800000D);

				_mxd.ActivateMap("MAPA_PRINCIPAL");
				//----------------------------------------
				//Mapa Tematico
				_mxd.SetLayerVisibility("IMAGEM", false);
				_mxd.RefreshLegends(scaleSymbols: true);

				_mxd.SetElementText("nome_croqui", "Croqui da Atividade");
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
			else if (_tipo == ArquivoMxd.MAPA_PECA_TECNICA)
			{
				//Aplicando filtros
				_mxd.ActivateMap("MAPA_PRINCIPAL");

				foreach (var layer in ObterLayers())
				{
					_mxd.SetQueryDefinition(layer.Name, String.Format(layer.Query, projetoId));
					_mxd.SetLayerVisibility(layer.Name, true);
				}

				//Setar as informações no mxd
				_mxd.SetElementText("processo", " ");
				_mxd.SetElementText("precisao", hashData["PRECISAO"].ToString());
				_mxd.SetElementText("data_emissao", DateTime.Today.ToString("dd/MM/yyyy"));

				List<Hashtable> lstQuadroTotal = hashData["QUADRO_TOTAL"] as List<Hashtable>;
				var hashATP = lstQuadroTotal.SingleOrDefault(x => x["CLASSE"].ToString() == "ATP");

				_mxd.SetElementText("nome_interessado", " ");

				_mxd.SetElementText("municipio_uf", " ");
				_mxd.SetElementText("bairro_gleba_comunidade", " ");
				_mxd.SetElementText("distrito_localidade", " ");
				_mxd.SetElementText("area_m2", FormatNumber(hashATP["AREA_M2"]));
				_mxd.SetElementText("perimetro_m", FormatNumber(hashData["ATP_PERIMETRO"], 3));
				_mxd.SetElementText("nome_responsavel_tecnico", " ");
				_mxd.SetElementText("profissao_responsavel_tecnico", " ");
				_mxd.SetElementText("orgao_classe_art_responsavel_tecnico", " ");

				//Definir Zoom
				_mxd.ZoomToFeature("Área total da propriedade", "PROJETO", projetoId.ToString());
				_mxd.ZoomPercentage(1.1);
				_mxd.ZoomToBestScale("MAPA_PRINCIPAL", 250D);
				_mxd.AdjustGrid("MAPA_PRINCIPAL", 3, 3);

				//----------------------------------------
				//Mapa Tematico
				_mxd.SetLayerVisibility("IMAGEM", false);
				_mxd.RefreshLegends(scaleSymbols: true);

				_mxd.SetElementText("titulo_croqui", "Croqui da Peça Técnica");
				_mxd.SetElementText("imagem", String.Empty);
				_mxd.ExportToPDF(tempFile1, 300);

				loadPdfToDocument(doc, wrt, tempFile1, isUsePageEvent);

				doc.NewPage();
				//----------------------------------------

				//----------------------------------------
				//Mapa Imagem
				_mxd.SetLayerVisibility("IMAGEM", true);
				_mxd.RefreshLegends(scaleSymbols: true);

				_mxd.SetElementText("titulo_croqui", "Croqui da Peça Técnica com Imagem");
				_mxd.SetElementText("imagem", "Aerolevantamento: ano 2007/2008");
				_mxd.ExportToPDF(tempFile2, 300);

				loadPdfToDocument(doc, wrt, tempFile2, isUsePageEvent);

				doc.NewPage();
				//----------------------------------------
			}
			else if (_tipo == ArquivoMxd.MAPA_FISCALIZACAO)
			{
				//Aplicando filtros
				_mxd.ActivateMap("MAPA_PRINCIPAL");

				_mxd.SetQueryDefinition("Ponto fiscalizado", "PROJETO=" + projetoId);
				_mxd.SetQueryDefinition("Linha fiscalizada", "PROJETO=" + projetoId);
				_mxd.SetQueryDefinition("Área fiscalizada", "PROJETO=" + projetoId);

				_mxd.SetLayerVisibility("Ponto fiscalizado", true);
				_mxd.SetLayerVisibility("Linha fiscalizada", true);
				_mxd.SetLayerVisibility("Área fiscalizada", true);

				_mxd.ActivateMap("MINI_MAPA");
				_mxd.SetQueryDefinition("Ponto de fiscalização", "FISCALIZACAO=" + hashData["FISCALIZACAO"]);

				//Setar as informações no mxd
				_mxd.SetElementText("municipio", hashData["MUNICIPIO"].ToString());
				_mxd.SetElementText("uf", hashData["UF"].ToString());
				_mxd.SetElementText("data", DateTime.Today.ToString("dd/MM/yyyy"));
				_mxd.SetElementText("precisao", hashData["PRECISAO"].ToString());
				_mxd.SetElementText("total_ponto", FormatNumber(hashData["TOTAL_PONTOS"], 0));
				_mxd.SetElementText("total_linha", FormatNumber(hashData["TOTAL_LINHAS"], 0) + " ( " + FormatNumber(hashData["SOMA_LINHAS"], 3) + " m)");
				_mxd.SetElementText("total_areas", FormatNumber(hashData["TOTAL_AREAS"], 0) + " ( " + FormatNumber(hashData["SOMA_AREAS"], 2) + " m²)");

				string[] envelope = hashData["ENVELOPE"].ToString().Split(';');
				//Definir Zoom

				Envelope envelopeRect = new Envelope(Convert.ToInt32(envelope[0]), Convert.ToInt32(envelope[2]), Convert.ToInt32(envelope[1]), Convert.ToInt32(envelope[3]));

				if (envelope[0] == envelope[2] && envelope[1] == envelope[3])
				{
					Envelope envelopeZoom = new Envelope(envelopeRect.Xmin - 100, envelopeRect.Xmax + 100, envelopeRect.Ymin - 100, envelopeRect.Ymax + 100);
					_mxd.ZoomToEnvelope(envelopeZoom);
					_mxd.ZoomToScale("MAPA_PRINCIPAL", 5000D);
				}
				else
				{
					_mxd.ZoomToEnvelope(envelopeRect);
					_mxd.ZoomPercentage(1.5);
					_mxd.ZoomToBestScale("MAPA_PRINCIPAL", 250D);
				}

				_mxd.AdjustGrid("MAPA_PRINCIPAL", 3, 3);

				_mxd.ActivateMap("MINI_MAPA");
				int centerX = Convert.ToInt32((envelopeRect.Xmax + envelopeRect.Xmin) / 2);
				int centerY = Convert.ToInt32((envelopeRect.Ymax + envelopeRect.Ymin) / 2);
				_mxd.ZoomToEnvelope(new Envelope(centerX - 2000, centerX + 2000, centerY - 2000, centerY + 2000));
				_mxd.ZoomToBestScale("MINI_MAPA", 800000D);

				_mxd.ActivateMap("MAPA_PRINCIPAL");
				//----------------------------------------
				//Mapa Tematico
				_mxd.SetLayerVisibility("IMAGEM", false);
				_mxd.RefreshLegends(scaleSymbols: true);

				_mxd.SetElementText("nome_croqui", "CROQUI TEMÁTICO DE FISCALIZAÇÃO");
				_mxd.SetElementText("imagem", String.Empty);
				_mxd.ExportToPDF(tempFile1, 300);

				loadPdfToDocument(doc, wrt, tempFile1, isUsePageEvent);

				doc.NewPage();
				//----------------------------------------

				if (_mxd.GetMapScale("MAPA_PRINCIPAL") <= 1750D)
				{
					_mxd.ZoomToScale("MAPA_PRINCIPAL", 1750);
				}

				//----------------------------------------
				//Mapa Imagem
				_mxd.SetLayerVisibility("IMAGEM", true);
				_mxd.RefreshLegends(scaleSymbols: true);

				_mxd.SetElementText("nome_croqui", "CROQUI IMAGEM DE FISCALIZAÇÃO");
				_mxd.SetElementText("imagem", "Aerolevantamento: ano 2007/2008");
				_mxd.ExportToPDF(tempFile2, 300);

				loadPdfToDocument(doc, wrt, tempFile2, isUsePageEvent);

				doc.NewPage();
				//----------------------------------------
			}
			else if (_tipo == ArquivoMxd.MAPA_CAR)
			{
				int projetoDomId = Convert.ToInt32(((Hashtable)hashData["CAR"])["PRJ_DOM_ID"]);

				//Aplicando filtros
				_mxd.ActivateMap("MAPA_PRINCIPAL");

				foreach (var layer in ObterLayers())
				{
					_mxd.SetQueryDefinition(layer.Name, String.Format(layer.Query, projetoDomId));
					layer.Source = _mxd.GetDataSource(layer.Name);

					if (String.IsNullOrEmpty(layer.Query))
					{
						_mxd.SetLayerVisibility(layer.Name, true);
					}
					else
					{
						_mxd.SetLayerVisibility(layer.Name, _da.ObterQuantidade(layer, projetoDomId) > 0);
					}
				}

				var layerAppRec = ObterLayers().Find(x => x.Name == "APP a recuperar (Calculado)");
				layerAppRec.Source = _mxd.GetDataSource(layerAppRec.Name);
				_mxd.SetQueryDefinition(layerAppRec.Name, String.Format(layerAppRec.Query, projetoId));
				_mxd.SetLayerVisibility(layerAppRec.Name, _da.ObterQuantidade(layerAppRec, projetoId) > 0);

				_mxd.ActivateMap("MINI_MAPA");
				_mxd.SetQueryDefinition("Ponto empreendimento", "EMPREENDIMENTO=" + hashData["EMPREENDIMENTO"]);
				_mxd.ActivateMap("MAPA_PRINCIPAL");

				//Setar as informações no mxd
				_mxd.SetElementText("municipio", hashData["MUNICIPIO"].ToString());
				_mxd.SetElementText("uf", hashData["UF"].ToString());
				_mxd.SetElementText("data", DateTime.Today.ToString("dd/MM/yyyy"));
				_mxd.SetElementText("precisao", hashData["PRECISAO"].ToString());

				//Definir Zoom
				_mxd.ZoomToFeature("Área total da propriedade", "PROJETO", projetoDomId.ToString());
				_mxd.ZoomPercentage(1.1);
				_mxd.ZoomToBestScale("MAPA_PRINCIPAL", 250D);
				_mxd.AdjustGrid("MAPA_PRINCIPAL", 3, 3);

				_mxd.ActivateMap("MINI_MAPA");
				string[] coord = hashData["COORDENADA"].ToString().Split(';');
				int centerX = Convert.ToInt32(coord[0]);
				int centerY = Convert.ToInt32(coord[1]);
				_mxd.ZoomToEnvelope(new Envelope(centerX - 2000, centerX + 2000, centerY - 2000, centerY + 2000));
				_mxd.ZoomToBestScale("MINI_MAPA", 800000D);

				_mxd.ActivateMap("MAPA_PRINCIPAL");
				//----------------------------------------
				//Mapa Tematico
				_mxd.SetLayerVisibility("IMAGEM", false);
				_mxd.RefreshLegends(scaleSymbols: true);

				_mxd.SetElementText("nome_croqui", "Croqui do CAR");
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

		private void loadPdfToDocument(Document doc, PdfWriter wrt, String fileUrl, bool isUsePageEvent, float paddingLeft = float.MinValue)
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

				if (paddingLeft != float.MinValue)
				{
					doc.SetMargins(LeftMargin + paddingLeft, RightMargin - paddingLeft, TopMargin, BottomMargin);
					doc.NewPage();
				}

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

				if (paddingLeft != float.MinValue)
				{
					doc.SetMargins(LeftMargin, RightMargin, TopMargin, BottomMargin);
					doc.SetPageSize(psizeOrg);
					doc.NewPage();
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