using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using iTextSharp.text;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.WindowsService.ProcessControl;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.ArcGIS;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Business;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Entities;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;
using Tecnomapas.TecnoGeo.Acessadores;
using Tecnomapas.TecnoGeo.Acessadores.OracleSpatial;
using Tecnomapas.TecnoGeo.Acessadores.Shape;
using Tecnomapas.TecnoGeo.Ferramentas;
using Tecnomapas.TecnoGeo.Geografico;
using Tecnomapas.TecnoGeo.Geometria;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo
{
	class ProcessoOperacoesGeoBus : ExecucaoProcesso
	{
		public Projeto Project { get; set; }

		private LogProcessamento log = null;
		private ProjetoBus _bus;
		private PdfRelatorioCroquiBus _pdfCroqui;

		private const int OPERACAO_BASEREF_INTERNA = 1;
		private const int OPERACAO_BASEREF_GEOBASES = 2;
		private const int OPERACAO_DOMINIALIDADE = 3;
		private const int OPERACAO_ATIVIDADE = 4;
		private const int OPERACAO_FISCALIZACAO = 5;
		private const int OPERACAO_BASEREF_FISCAL = 6;
		private const int OPERACAO_CAR = 7;
		private const int OPERACAO_TITULO = 8;

		private const int ETAPA_VALIDACAO = 1;
		private const int ETAPA_PROCESSAMENTO = 2;
		private const int ETAPA_GERACAO_DE_PDF = 3;

		private const int ORIGEM_SHAPE_TRACKMAKER = 1;
		private const int ORIGEM_TABLE_DESENHADOR = 2;

		public ProcessoOperacoesGeoBus(Projeto project, string mutexServico, string mutexProcesso) : base(mutexServico, mutexProcesso)
		{
			Project = project;
		}

		//Inicie Arqui
		protected override void ExecutarProcesso()
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				try
				{
					log = new LogProcessamento();

					_bus = new ProjetoBus(bancoDeDados);
					_pdfCroqui = new PdfRelatorioCroquiBus(bancoDeDados, Project);

					//Debugger.Launch();

					switch (Project.Type)
					{
						case OPERACAO_BASEREF_GEOBASES:
							ExecutarBaseRef();
							break;

						case OPERACAO_BASEREF_INTERNA:
							ExecutarBaseRef();
							break;

						case OPERACAO_BASEREF_FISCAL:
							ExecutarBaseRef();
							break;
						default:
							switch (Project.Step)
							{
								case ETAPA_VALIDACAO:
									ExecutarValidacao();
									break;

								case ETAPA_PROCESSAMENTO:
									ExecutarProcessamento();
									break;

								case ETAPA_GERACAO_DE_PDF:
									ExecutarPDFs();
									break;

								default:
									//Desalocar Ticket setando ERRO
									_bus.SetFalhaNaFila(Project.Id, Project.Type);
									break;
							}
							break;
					}
				}
				catch (Exception exc)
				{
					_bus.SetFalhaNaFila(Project.Id, Project.Type);

					Log.GerarLog(exc);

					throw;
				}
			}
		}

		#region Executar

		protected void ExecutarValidacao()
		{
			Hashtable htImportacao = new Hashtable();
			bool temErros = false;

			List<string> lstErros;
			List<Hashtable> lstHashtable;

			int mecanismo = _bus.ObterMecanismoDeEnvio(Project.Id, Project.Type);

			//Origem
			switch (mecanismo)
			{
				case ORIGEM_TABLE_DESENHADOR:
					_bus.FinalizarImportacaoDesenhador(Project.Id, Project.Type);

					lstErros = new List<string>();
					break;

				case ORIGEM_SHAPE_TRACKMAKER:
					lstErros = ImportarShapeTrackmaker();
					break;
				default:
					throw new Exception("Mecanismo de Envio Inválido");
			}

			//Etapa de Validacao
			htImportacao = new Hashtable();

			_bus.ProcessarValidacao(Project.Id, Project.Type);

			#region Validar Erros Espaciais

			log.IniciarTime("Validar Erros Espaciais");

			lstHashtable = _bus.ValidarErrosEspaciais(Project.Id, Project.Type);

			if (lstHashtable != null)
			{
				htImportacao.Add("ERROS_ESPACIAIS", lstHashtable);
			}

			temErros = temErros || (lstHashtable != null && lstHashtable.Count > 0);

			log.FinalizarTime();

			#endregion

			#region Validar Obrigatoriedades

			log.IniciarTime("Validar Obrigatoriedades");

			lstHashtable = _bus.ValidarObrigatoriedades(Project.Id, Project.Type);

			if (lstHashtable != null)
			{
				htImportacao.Add("OBRIGATORIEDADES", lstHashtable);
			}

			temErros = temErros || (lstHashtable != null && lstHashtable.Count > 0);

			log.FinalizarTime();

			#endregion

			#region Validar Atributos

			log.IniciarTime("Validar Atributos");

			lstHashtable = _bus.ValidarAtributos(Project.Id, Project.Type);

			if (lstHashtable != null)
			{
				htImportacao.Add("ATRIBUTOS", lstHashtable);
			}

			temErros = temErros || (lstHashtable != null && lstHashtable.Count > 0);

			log.FinalizarTime();

			#endregion

			#region Contabilizar Geometrias

			log.IniciarTime("Contabilizar Geometrias");

			lstHashtable = _bus.ContabilizarGeometrias(Project.Id, Project.Type);

			if (lstHashtable != null)
			{
				htImportacao.Add("GEOMETRIAS", lstHashtable);
			}

			log.FinalizarTime();

			#endregion

			#region Pontos Duplicados (DESABILITADO)
			/*
            if (temErros)
            {
                log.IniciarTime("Gerar shape de pontos duplicados");

                zipShapePontosDuplicados = GerarShapePontosDuplicados(idCAR, sisCoordId, sridBase, strConnectionGeoTecnico);
            }
            else
            {
                log.IniciarTime("Limpar pontos duplicados");

                daValidacaoBaseGeo.LimparPontosDuplicados(idCAR);
            }

            log.FinalizarTime();
            */
			#endregion

			#region Gerar Pdf do Relatório de Importação

			log.IniciarTime("Gerar Pdf do Relatório de Importação");

			Hashtable htHeadeFooter = _pdfCroqui.ObterDadosCabecalhoRodapePDF();

			//PDF de Validacao = 4
			Arquivo file = _bus.ObterArquivo(Project.Id, 4);
			file.Buffer = new MemoryStream(PdfRelatorioValidacao.GerarPdf(temErros, htImportacao, htHeadeFooter).ToArray());

			log.FinalizarTime();

			#endregion

			#region Salvar Pdf do Relatório de Importação

			//PDF de Validacao = 4
			_bus.SalvarArquivo(file, Project.Id, Project.Type, 4);

			#endregion

			#region Finalizar validação

			if (!temErros)
			{
				log.IniciarTime("Avancar para proxima etapa");

				_bus.SetAguardandoEtapaNaFila(Project.Id, Project.Type, ETAPA_PROCESSAMENTO);
			}
			else
			{
				log.IniciarTime("Alterar situação para Reprovado");

				if (mecanismo == ORIGEM_TABLE_DESENHADOR)
				{
					_bus.LimparErrosGeometricos(Project.Id, Project.Type);
				}

				_bus.SetConcluidoNaFila(Project.Id, Project.Type, Project.Step);

				//save zipShapePontosDuplicados
			}

			log.FinalizarTime();

			#endregion


			#region Salvar Log

			_bus.SalvarLogOperacoes(Project.Id, Project.Type, Project.Step, log.Execucoes);

			#endregion
		}

		protected List<string> ImportarShapeTrackmaker()
		{
			List<string> lstRelatorioImportacao = null;

			log.IniciarTime("Importar zip dos Shapes do Trackmaker");

			//string fileURL = _bus.ObterCaminhoDoArquivoEnviado(Project.Id);
			string fileURL = @"C:\Users\jhonny.correa\Documents\Shape\Shape.zip";
			_bus.ApagarGeometriasTemporariasTrackmaker(Project.Id, Project.Type);

			using (FileStream fs = File.OpenRead(fileURL))
			{
				ZipFile zip = new ZipFile(fs);

				if (zip == null || zip.Count == 0)
				{
					throw new Exception("Arquivo zip não encontrado.");
				}

				FonteFeicaoShapeStream fonte = CarregarShapesDoZip(zip);
				lstRelatorioImportacao = GravarFeicoesShape(fonte);
			}

			_bus.FinalizarImportacaoTrackmaker(Project.Id, Project.Type);

			log.FinalizarTime();

			return lstRelatorioImportacao;
		}

		protected void ExecutarProcessamento()
		{
			#region Processar

			log.IniciarTime("Processar Geometrias");

			_bus.ProcessarGeometrias(Project.Id, Project.Type);

			log.FinalizarTime();

			#endregion

			#region Gerar arquivos processados

			log.IniciarTime("Gerar arquivos processados");

			List<byte[]> fileList = GerarZipProcessado();

			log.FinalizarTime();

			#endregion

			#region Salvar arquivo processado

			Arquivo file;

			//Zip Shape normal = 5
			file = _bus.ObterArquivo(Project.Id, 5);
			file.Buffer = new MemoryStream(fileList[0]);
			_bus.SalvarArquivo(file, Project.Id, Project.Type, 5);

			//Zip Shape Trackmaker = 6
			file = _bus.ObterArquivo(Project.Id, 6);
			file.Buffer = new MemoryStream(fileList[1]);
			_bus.SalvarArquivo(file, Project.Id, Project.Type, 6);

			#endregion

			#region Alterar situação para Processado

			log.IniciarTime("Avançar para a proxima etapa");

			_bus.SetAguardandoEtapaNaFila(Project.Id, Project.Type, ETAPA_GERACAO_DE_PDF);

			log.FinalizarTime();

			#endregion
		}

		protected void ExecutarBaseRef()
		{
			#region Gerar arquivo baseref

			log.IniciarTime("Gerar arquivo baseref");

			byte[] fileByteArray = GerarZipBaseRef();

			log.FinalizarTime();

			#endregion

			#region Salvar arquivo baseref

			//1 - Base de referência interna
			//2 - Base de referência GEOBASES
			int fileType = (Project.Type == OPERACAO_BASEREF_GEOBASES) ? 2 : 1;

			Arquivo file = _bus.ObterArquivo(Project.Id, fileType);
			file.Buffer = new MemoryStream(fileByteArray);
			_bus.SalvarArquivo(file, Project.Id, Project.Type, fileType);

			#endregion

			#region Alterar situação para Processado

			log.IniciarTime("Alterar situação baseref");

			_bus.SetConcluidoNaFila(Project.Id, Project.Type, Project.Step);

			log.FinalizarTime();

			#endregion
		}

		protected void ExecutarPDFs()
		{
			MxdLayout mxd = null;

			try
			{
				Arquivo file;

				switch (Project.Type)
				{
					case OPERACAO_DOMINIALIDADE:
						mxd = new MxdLayout();
						mxd.AbrirMxdLayout(ArquivoMxd.MAPA_DOMINIALIDADE, PageSize.A4);

						#region Salvar arquivo

						using (MemoryStream ms = _pdfCroqui.GerarPdfDominialidade(mxd))
						{
							//PDF com mapa = 7
							file = _bus.ObterArquivo(Project.Id, 7);
							file.Buffer = new MemoryStream(ms.ToArray());
							_bus.SalvarArquivo(file, Project.Id, Project.Type, 7);
						}

						#endregion

						mxd.FecharMxdLayout();
						mxd = new MxdLayout();
						mxd.AbrirMxdLayout(ArquivoMxd.MAPA_PECA_TECNICA, PageSize.A4);

						#region Salvar arquivo

						using (MemoryStream ms = _pdfCroqui.GerarPdfPecaTecnica(mxd))
						{
							//PDF com mapa final = 8
							file = _bus.ObterArquivo(Project.Id, 8);
							file.Buffer = new MemoryStream(ms.ToArray());
							_bus.SalvarArquivo(file, Project.Id, Project.Type, 8);
						}

						#endregion

						#region Alterar situação

						log.IniciarTime("Alterar situação para PDF gerado");

						_bus.SetConcluidoNaFila(Project.Id, Project.Type, Project.Step);
						//_bus.SetProcessado(Project.Id);

						log.FinalizarTime();

						#endregion

						break;
					case OPERACAO_ATIVIDADE:
						mxd = new MxdLayout();
						mxd.AbrirMxdLayout(ArquivoMxd.MAPA_ATIVIDADE, PageSize.A4);

						#region Salvar arquivo
						int? idArquivo;

						using (MemoryStream ms = _pdfCroqui.GerarPdfAtividade(mxd))
						{
							//PDF com mapa = 7
							file = _bus.ObterArquivo(Project.Id, 7);
							file.Buffer = new MemoryStream(ms.ToArray());
							idArquivo = _bus.SalvarArquivo(file, Project.Id, Project.Type, 7);
						}
						#endregion

						#region Alterar situação
						log.IniciarTime("Alterar situação para PDF gerado");

						_bus.SetConcluidoNaFila(Project.Id, Project.Type, Project.Step);
						//_bus.SetProcessado(Project.Id);

						log.FinalizarTime();
						#endregion

						break;

					case OPERACAO_FISCALIZACAO:
						mxd = new MxdLayout();
						mxd.AbrirMxdLayout(ArquivoMxd.MAPA_FISCALIZACAO, PageSize.A4);

						#region Salvar arquivo
						using (MemoryStream ms = _pdfCroqui.GerarPdfFiscal(mxd))
						{
							//PDF com mapa = 7
							file = _bus.ObterArquivo(Project.Id, 7);
							file.Buffer = new MemoryStream(ms.ToArray());
							_bus.SalvarArquivo(file, Project.Id, Project.Type, 7);
						}
						#endregion

						#region Alterar situação
						log.IniciarTime("Alterar situação para PDF gerado");

						_bus.SetConcluidoNaFila(Project.Id, Project.Type, Project.Step);
						//_bus.SetProcessado(Project.Id);

						log.FinalizarTime();
						#endregion

						break;

					case OPERACAO_CAR:
						mxd = new MxdLayout();
						mxd.AbrirMxdLayout(ArquivoMxd.MAPA_CAR, PageSize.A4);

						#region Salvar arquivo
						using (MemoryStream ms = _pdfCroqui.GerarPdfCAR(mxd))
						{
							//PDF com mapa = 7
							file = _bus.ObterArquivo(Project.Id, 7);
							file.Buffer = new MemoryStream(ms.ToArray());
							_bus.SalvarArquivo(file, Project.Id, Project.Type, 7);
						}
						#endregion

						#region Alterar situação
						log.IniciarTime("Alterar situação para PDF gerado");

						_bus.SetConcluidoNaFila(Project.Id, Project.Type, Project.Step);
						//_bus.SetProcessado(Project.Id);

						log.FinalizarTime();
						#endregion

						break;

					case OPERACAO_TITULO:
						mxd = new MxdLayout();
						mxd.AbrirMxdLayout(ArquivoMxd.MAPA_ATIVIDADE_TITULO, PageSize.A4);

						#region Salvar arquivo

						int titulo;
						using (MemoryStream ms = _pdfCroqui.GerarPdfAtividadePorTitulo(mxd, out titulo))
						{
							//PDF com mapa = 7
							file = _bus.ObterArquivo(Project.Id, 7);
							file.Buffer = new MemoryStream(ms.ToArray());
							_bus.SalvarArquivo(file, Project.Id, Project.Type, 7, titulo);
						}
						#endregion

						#region Alterar situação
						log.IniciarTime("Alterar situação para PDF gerado");

						_bus.SetConcluidoNaFila(Project.Id, Project.Type, Project.Step);
						//_bus.SetProcessado(Project.Id);

						log.FinalizarTime();
						#endregion

						break;
				}

			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);

				_bus.SetFalhaNaFila(Project.Id, Project.Type);
			}
			finally
			{
				if (mxd != null)
					mxd.FecharMxdLayout();
			}

		}

		#endregion

		#region Funções Auxiliares

		private static void SomarHashAreas(string key, Hashtable hashData, Hashtable hashSoma, bool somar = true)
		{
			decimal numValue = 0;

			if (hashData[key] == DBNull.Value)
			{
				hashData[key] = "0,00";
			}
			else
			{
				numValue = Convert.ToDecimal(hashData[key]);
				hashData[key] = numValue.ToString("N2");
			}

			if (hashSoma.ContainsKey(key))
			{
				if (somar)
					hashSoma[key] = Convert.ToDecimal(hashSoma[key]) + numValue;
			}
			else
			{
				hashSoma[key] = numValue;
			}
		}

		private string convertToString(object value)
		{
			return (value != null) ? value.ToString() : "";
		}

		private bool IsGeometriaValida(Feicao feicao)
		{
			IList elementos = null;

			switch (feicao.Geometria.ObterTipo())
			{
				case TipoGeometria.Complexa:
					elementos = (feicao.Geometria as Tecnomapas.TecnoGeo.Geometria.Complexa.Complexa).Elementos;
					break;

				case TipoGeometria.Linha:
					elementos = (feicao.Geometria as Tecnomapas.TecnoGeo.Geometria.Primitiva.Linha).Segmentos;
					break;

				case TipoGeometria.MultiLinha:
					elementos = (feicao.Geometria as Tecnomapas.TecnoGeo.Geometria.Agregada.MultiLinha).Elementos;
					break;

				case TipoGeometria.MultiPoligono:
					elementos = (feicao.Geometria as Tecnomapas.TecnoGeo.Geometria.Agregada.MultiPoligono).Elementos;
					break;

				case TipoGeometria.MultiPonto:
					elementos = (feicao.Geometria as Tecnomapas.TecnoGeo.Geometria.Agregada.MultiPonto).Elementos;
					break;

				case TipoGeometria.Poligono:
					elementos = (feicao.Geometria as Tecnomapas.TecnoGeo.Geometria.Primitiva.Poligono).Aneis;
					break;

				case TipoGeometria.Ponto:
					Posicao pos = (feicao.Geometria as Tecnomapas.TecnoGeo.Geometria.Primitiva.Ponto).Posicao;

					if (pos != null)
					{
						elementos = pos.ToArray();
						decimal tt = Convert.ToDecimal(pos.X.ToString());
						tt = Convert.ToDecimal(pos.Y.ToString());
					}
					break;
			}

			return elementos == null || elementos.Count == 0;
		}

		private FonteFeicaoOracleSpatial GetDatabaseFontFeicao(string connectionKey = "default")
		{
			FonteFeicaoOracleSpatial fonteFeicao = new FonteFeicaoOracleSpatial();
			bool[] parameters = new bool[] { false, false, false };


			if (ConfigurationManager.ConnectionStrings[connectionKey] == null)
			{
				throw new Exception(String.Format("Chave {0} não encontrada.", connectionKey));
			}

			string[] arrayStrConnection = ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString.Split(';');

			for (int i = 0; i < arrayStrConnection.Length; i++)
			{
				if (arrayStrConnection[i].IndexOf('=') < 0)
				{
					continue;
				}

				string[] param = arrayStrConnection[i].Split('=');

				param[0] = param[0].ToLower();

				if (param[0].IndexOf("source") >= 0)
				{
					fonteFeicao.Fonte = param[1];
					parameters[0] = true;
				}
				else if (param[0].IndexOf("user") >= 0)
				{
					fonteFeicao.Usuario = param[1];
					parameters[1] = true;
				}
				else if (param[0].IndexOf("password") >= 0)
				{
					fonteFeicao.Senha = param[1];
					parameters[2] = true;
				}
			}


			if (!(parameters[0] && parameters[1] && parameters[2]))
			{
				throw new Exception("Conexão não encontrada.");
			}

			return fonteFeicao;
		}

		#endregion

		#region Shape para Oracle

		private static FonteFeicaoShapeStream CarregarShapesDoZip(ZipFile zip)
		{
			FonteFeicaoShapeStream fonte = new FonteFeicaoShapeStream();

			foreach (ZipEntry zen in zip)
			{
				if (Path.GetExtension(zen.Name).ToLower() == ".shp")
				{
					string nome = Path.GetFileNameWithoutExtension(zen.Name).ToLower();
					ZipEntry shx = zip.GetEntry(nome + ".shx");
					ZipEntry dbf = zip.GetEntry(nome + ".dbf");

					if (shx != null && dbf != null && !fonte.ExisteFeicao(nome))
					{
						Stream sShp = zip.GetInputStream(zen);
						Stream sShx = zip.GetInputStream(shx);
						Stream sDbf = zip.GetInputStream(dbf);

						byte[] bShp = new byte[zen.Size];
						byte[] bShx = new byte[shx.Size];
						byte[] bDbf = new byte[dbf.Size];

						sShp.Read(bShp, 0, bShp.Length);
						sShx.Read(bShx, 0, bShx.Length);
						sDbf.Read(bDbf, 0, bDbf.Length);

						fonte.AdicionarClasseFeicao(nome, new MemoryStream(bShp), new MemoryStream(bShx), new MemoryStream(bDbf));
					}
				}
			}

			return fonte;
		}

		private List<string> GravarFeicoesShape(FonteFeicaoShapeStream fonte)
		{
			StringCollection relatorio = new StringCollection();

			FonteFeicaoOracleSpatial destino = GetDatabaseFontFeicao();

			destino.Abrir();

			string[] shapes = fonte.ListarClassesFeicao();

			foreach (string shapeName in shapes)
			{
				GravarFeicoesShape(fonte, destino, shapeName, relatorio);
			}

			destino.Fechar();

			return relatorio.Cast<string>().ToList();
		}

		private void GravarFeicoesShape(FonteFeicaoShapeStream fonte, FonteFeicaoOracleSpatial destino, string shapeName, StringCollection relatorio)
		{
			ClasseFeicao classeFonte = fonte.ObterClasseFeicao(shapeName);
			if (classeFonte == null) return;

			ClasseFeicao classeDestino = destino.ObterClasseFeicao("TMP_RASC_TRACKMAKER");
			if (classeDestino == null) return;

			FeicaoAdapter adpt = new FeicaoAdapter(classeDestino);

			if (!adpt.EhTransformavel(classeDestino, relatorio))
			{
				relatorio.Add("Arquivo " + shapeName + " desconsiderado por causa das observações acima");
				return;
			}

			LeitorFeicao leitorFeicao = fonte.ObterLeitorFeicao(shapeName);
			OperadorFeicaoOracleSpatial escritorFeicao = (OperadorFeicaoOracleSpatial)destino.ObterOperadorFeicao("TMP_RASC_TRACKMAKER");

			if (leitorFeicao == null) return;
			if (escritorFeicao == null) return;

			adpt.Adaptadores["PROJETO"].Origem = TipoOrigem.Manual;
			adpt.Adaptadores["PROJETO"].Valor = Project.Id;

			adpt.Adaptadores["TIPO_PROJETO"].Origem = TipoOrigem.Manual;
			adpt.Adaptadores["TIPO_PROJETO"].Valor = Project.Type;

			int cont = 0;

			try
			{
				while (true)
				{
					try
					{
						if (!leitorFeicao.Ler()) return;

						cont++;

						if (leitorFeicao.Atual.Atributos.IndiceDe("NAME") >= 0)
						{
							Atributo atributo = leitorFeicao.Atual.Atributos["NAME"];
							leitorFeicao.Atual.Atributos.RemoveAt(leitorFeicao.Atual.Atributos.IndiceDe("NAME"));
							atributo.Nome = "NOME";
							leitorFeicao.Atual.Atributos.Adicionar(atributo);
						}

						if (leitorFeicao.Atual.Atributos.IndiceDe("TIPOEXP") < 0)
						{
							Atributo atributo = new Atributo();
							atributo.Nome = "TIPOEXP";
							atributo.Valor = "";
							leitorFeicao.Atual.Atributos.Adicionar(atributo);
						}

						Feicao otherFeicao = adpt.Transformar(leitorFeicao.Atual);

						escritorFeicao.Inserir(otherFeicao);
					}
					catch
					{
						relatorio.Add(String.Format("A geometria {0} da feição {1} é inválida ou nula, deve ser redesenhada ou removida.", cont, shapeName));
					}
				}
			}
			catch (TecnoGeoException exc)
			{
				throw new Exception(string.Format("Erro ao transportar geometria {0}/{1} da feição {2}. Erro subjacente era: {3}", cont, Project.Id, shapeName, exc.Message));
			}
			finally
			{
				escritorFeicao.Fechar();
				leitorFeicao.Fechar();
			}
		}

		#endregion

		#region Oracle para Shape

		public byte[] AdicionarArquivosPrj(MemoryStream msZip, string[] featureList)
		{
			ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

			string strPrj = "PROJCS[\"SIRGAS 2000 / UTM zone 24S\",GEOGCS[\"SIRGAS 2000\",DATUM[\"D_SIRGAS_2000\",SPHEROID[\"GRS_1980\",6378137,298.257222101]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]],PROJECTION[\"Transverse_Mercator\"],PARAMETER[\"latitude_of_origin\",0],PARAMETER[\"central_meridian\",-39],PARAMETER[\"scale_factor\",0.9996],PARAMETER[\"false_easting\",500000],PARAMETER[\"false_northing\",10000000],UNIT[\"Meter\",1]]";
			byte[] arqPrj = encoding.GetBytes(strPrj);

			ZipFile zipFile = new ZipFile(new MemoryStream(msZip.ToArray()));

			MemoryStream newStream = new MemoryStream();
			ZipOutputStream zipOut = new ZipOutputStream(newStream);
			zipOut.SetLevel(5);

			Stream str = null;
			byte[] bytes = null;
			foreach (ZipEntry item in zipFile)
			{
				bytes = new byte[item.Size];
				str = zipFile.GetInputStream(item.ZipFileIndex);
				str.Read(bytes, 0, bytes.Length);
				str.Close();

				zipOut.PutNextEntry(item);
				zipOut.Write(bytes, 0, bytes.Length);
			}


			foreach (string featureName in featureList)
			{
				ZipEntry zipEntry = new ZipEntry(featureName.ToLower() + ".prj");
				zipOut.PutNextEntry(zipEntry);
				zipOut.Write(arqPrj, 0, arqPrj.Length);
			}

			zipOut.Finish();
			zipOut.Close();

			return newStream.ToArray();
		}

		private byte[] GerarZipBaseRef()
		{
			string[] featureNameList;
			string[] featureTypeList;
			string[] featureAliasList;
			int sridBase = 0;

			string connectionKey;

			Hashtable config = _bus.ObterConfiguracoesBaseRef();

			if (Project.Type == OPERACAO_BASEREF_GEOBASES)
			{
				featureNameList = config["GEOBASES_FEATURE_NAMES"].ToString().Split(',');
				featureTypeList = config["GEOBASES_FEATURE_TYPES"].ToString().Split(',');
				featureAliasList = config["GEOBASES_FEATURE_ALIASES"].ToString().Split(',');

				connectionKey = config["GEOBASES_CONNECTION_KEY"].ToString();

				sridBase = 31999;
			}
			else if (Project.Type == OPERACAO_BASEREF_INTERNA)
			{
				featureNameList = config["INTERNO_FEATURE_NAMES"].ToString().Split(',');
				featureTypeList = config["INTERNO_FEATURE_TYPES"].ToString().Split(',');
				featureAliasList = config["INTERNO_FEATURE_ALIASES"].ToString().Split(',');

				connectionKey = config["INTERNO_CONNECTION_KEY"].ToString();

				sridBase = Convert.ToInt32(config["SRID_BASE"]);
			}
			else if (Project.Type == OPERACAO_BASEREF_FISCAL)
			{
				featureNameList = config["FISCAL_FEATURE_NAMES"].ToString().Split(',');
				featureTypeList = config["FISCAL_FEATURE_TYPES"].ToString().Split(',');
				featureAliasList = config["FISCAL_FEATURE_ALIASES"].ToString().Split(',');

				connectionKey = config["FISCAL_CONNECTION_KEY"].ToString();

				sridBase = Convert.ToInt32(config["SRID_BASE"]);
			}
			else
			{
				throw new Exception("Operacao Inválida ao gerar Base de Referência");
			}

			List<int> pnts_envelope = _bus.ObterEnvelope(Project.Id);
			if (pnts_envelope.Count != 4)
				throw new Exception("Envelope do Project " + Project.Id + " inválido");


			FonteFeicaoOracleSpatial origem = GetDatabaseFontFeicao(connectionKey);
			FonteFeicaoShapeStream destino = new FonteFeicaoShapeStream();

			origem.Abrir();
			destino.Abrir();

			LeitorFeicao leitor = null;
			ClasseFeicao classeFeicao = null;

			Expressao envelope = new Campo(String.Format("mdsys.sdo_geometry(2003,{0},null,sdo_elem_info_array(1,1003,3), sdo_ordinate_array({1},{2},{3},{4}))",
												sridBase,
												pnts_envelope[0],
												pnts_envelope[1],
												pnts_envelope[2],
												pnts_envelope[3]));

			OperacaoEspacial operacao;
			Expressao filtro;

			List<OperadorFeicaoShape> lstEscritores = new List<OperadorFeicaoShape>();
			List<string> srtNames = new List<string>();

			for (int i = 0; i < featureNameList.Length; i++)
			{
				string alias = featureAliasList[i];
				string feicao = featureNameList[i];
				TipoGeometria eTipoGeo = (TipoGeometria)Enum.Parse(typeof(TipoGeometria), featureTypeList[i], true);

				try
				{
					classeFeicao = origem.ObterClasseFeicao(feicao);
				}
				catch
				{
					classeFeicao = null;
				}

				if (classeFeicao == null)
					continue;

				string campoGeo = classeFeicao.CampoGeometrico;
				operacao = new OperacaoEspacialInterseccao(new Campo(campoGeo), envelope);
				operacao = new OperacaoEspacialExtracao(operacao, eTipoGeo);
				operacao = new OperacaoEspacialDensificacao(operacao);

				filtro = new ExpressaoRelacionalOracleSpatial(new OperacaoEspacialRelacao(new Campo(campoGeo), envelope, TipoRelacaoEspacial.ANYINTERACT), TipoOperadorRelacional.Igual, new ConstanteOracleSpatial(DbType.String, "TRUE"));

				try
				{
					leitor = origem.ObterLeitorFeicao(feicao, operacao, filtro);


					OperadorFeicaoShape escritor = null;

					while (leitor.Ler())
					{
						if (leitor.Atual.Geometria == null)
							continue;

						if (escritor == null)
						{
							escritor = (OperadorFeicaoShape)destino.CriarClasseFeicao(alias, eTipoGeo, leitor.Atual.Geometria.Dimensoes, leitor.Atual.Geometria.EhLrs, leitor.Atributos);

							lstEscritores.Add(escritor);
							srtNames.Add(alias);
						}

						escritor.Inserir(leitor.Atual);
					}

					leitor.Fechar();
				}
				catch (Exception exc)
				{
					throw new Exception(String.Format("Feição: {0}", feicao), exc);
				}
			}

			byte[] byteReturn = null;

			if (lstEscritores.Count > 0)
			{
				using (MemoryStream ms = new MemoryStream())
				{
					destino.ExportarParaZip(ms);
					byteReturn = AdicionarArquivosPrj(ms, srtNames.ToArray());
				}

				foreach (OperadorFeicaoShape escritorShp in lstEscritores)
				{
					escritorShp.Fechar();
				}
			}

			origem.Fechar();
			destino.Fechar();

			if (byteReturn == null)
			{
				string message = "Nenhuma Geometria nas Feições a seguir cruza com a Área de Abrangencia selecionada:";
				foreach (string featureName in featureAliasList)
				{
					message += "\r\n   - " + featureName;
				}

				byteReturn = GerarZipComMensagem("Nenhuma Geometria Encontrada", message);
			}

			return byteReturn;
		}

		public byte[] GerarZipComMensagem(string nomeArquivo, string mensagem)
		{
			//arquivo
			byte[] arquivoTxt = System.Text.Encoding.Default.GetBytes(mensagem);

			//zip
			MemoryStream zipStream = new MemoryStream();
			ZipOutputStream zipOut = new ZipOutputStream(zipStream);
			zipOut.SetLevel(5);

			//add
			ZipEntry zipEntry = new ZipEntry(nomeArquivo + ".txt");
			zipOut.PutNextEntry(zipEntry);
			zipOut.Write(arquivoTxt, 0, arquivoTxt.Length);

			//close
			zipOut.Finish();
			zipOut.Close();

			return zipStream.ToArray();
		}

		private string generateTrackMakerNameValue(string featureName, AtributoCollection atributos)
		{
			string result = featureName.Substring(4);
			switch (featureName)
			{
				case "TMP_RIO_LINHA":
				case "TMP_RIO_AREA":
					result = "RIO=" + convertToString(atributos["LARGURA"].Valor) + "=" + convertToString(atributos["NOME"].Valor);
					break;
				case "TMP_LAGOA":
					result = result + "=" + convertToString(atributos["ZONA"].Valor) + "=" + convertToString(atributos["NOME"].Valor);
					break;
				case "TMP_REPRESA":
					result = result + "=" + convertToString(atributos["AMORTECIMENTO"].Valor) + "=" + convertToString(atributos["NOME"].Valor);
					break;
				case "TMP_APMP":
					result = result + "=" + convertToString(atributos["TIPO"].Valor) + "=" + convertToString(atributos["NOME"].Valor);
					break;
				case "TMP_VERTICE":
					result = result + "=" + convertToString(atributos["NOME"].Valor);
					break;
				case "TMP_ARL":
					result = result + "=" + convertToString(atributos["CODIGO"].Valor) + "=" + convertToString(atributos["COMPENSADA"].Valor);
					break;
				case "TMP_AVN":
					result = result + "=" + convertToString(atributos["ESTAGIO"].Valor) + "=" + convertToString(atributos["VEGETACAO"].Valor);
					break;
				case "TMP_AA":
					result = result + "=" + convertToString(atributos["TIPO"].Valor) + "=" + convertToString(atributos["VEGETACAO"].Valor);
					break;
				case "TMP_REST_DECLIVIDADE":
					result = result + "=" + convertToString(atributos["TIPO"].Valor);
					break;

				case "TMP_AATIV":
				case "TMP_AIATIV":
				case "TMP_LATIV":
				case "TMP_PATIV":
					result = result + "=" + convertToString(atributos["CODIGO"].Valor);
					break;

				default:
					break;
			}

			return result;
		}

		private List<byte[]> GerarZipProcessado()
		{
			List<string> prjList = new List<string>();
			string[] featureList = null;
			string[] aliasList = null;

			switch (Project.Type)
			{
				case OPERACAO_DOMINIALIDADE:
					featureList = "TMP_ATP,TMP_APMP,TMP_AFD,TMP_ROCHA,TMP_VERTICE,TMP_ARL,TMP_RPPN,TMP_AFS,TMP_AVN,TMP_AA,TMP_ACONSTRUIDA,TMP_DUTO,TMP_LTRANSMISSAO,TMP_ESTRADA,TMP_FERROVIA,TMP_NASCENTE,TMP_RIO_LINHA,TMP_RIO_AREA,TMP_LAGOA,TMP_REPRESA,TMP_DUNA,TMP_REST_DECLIVIDADE,TMP_ESCARPA,TMP_AREAS_CALCULADAS".Split(',');
					aliasList = "ATP,APMP,AFD,ROCHA,VERTICE,ARL,RPPN,AFS,AVN,AA,ACONSTRUIDA,DUTO,LTRANSMISSAO,ESTRADA,FERROVIA,NASCENTE,RIO_LINHA,RIO_AREA,LAGOA,REPRESA,DUNA,REST_DECLIVIDADE,ESCARPA,AREAS_CALCULADAS".Split(',');
					break;
				case OPERACAO_ATIVIDADE:
					featureList = "TMP_PATIV,TMP_LATIV,TMP_AATIV,TMP_AIATIV".Split(',');
					aliasList = "PATIV,LATIV,AATIV,AIATIV".Split(',');
					break;
				case OPERACAO_FISCALIZACAO:
					featureList = "TMP_FISCAL_PONTO,TMP_FISCAL_LINHA,TMP_FISCAL_AREA".Split(',');
					aliasList = "FISCAL_PONTO,FISCAL_LINHA,FISCAL_AREA".Split(',');
					break;
				case OPERACAO_CAR:
					featureList = "GEO_ATP,GEO_APMP,GEO_AFD,GEO_ROCHA,GEO_VERTICE,GEO_ARL,GEO_RPPN,GEO_AFS,GEO_AVN,GEO_AA,GEO_ACONSTRUIDA,GEO_DUTO,GEO_LTRANSMISSAO,GEO_ESTRADA,GEO_FERROVIA,GEO_NASCENTE,GEO_RIO_LINHA,GEO_RIO_AREA,GEO_LAGOA,GEO_REPRESA,GEO_DUNA,GEO_REST_DECLIVIDADE,GEO_ESCARPA,GEO_AREAS_CALCULADAS,TMP_CAR_AREAS_CALCULADAS".Split(',');
					aliasList = "ATP,APMP,AFD,ROCHA,VERTICE,ARL,RPPN,AFS,AVN,AA,ACONSTRUIDA,DUTO,LTRANSMISSAO,ESTRADA,FERROVIA,NASCENTE,RIO_LINHA,RIO_AREA,LAGOA,REPRESA,DUNA,REST_DECLIVIDADE,ESCARPA,AREAS_CALCULADAS,CAR_AREAS_CALCULADAS".Split(',');
					break;
			}

			FonteFeicaoOracleSpatial origem = GetDatabaseFontFeicao();
			FonteFeicaoShapeStream destino = new FonteFeicaoShapeStream();
			FonteFeicaoShapeStream destinoTrackmaker = new FonteFeicaoShapeStream();

			origem.Abrir();
			destino.Abrir();
			destinoTrackmaker.Abrir();

			LeitorFeicao leitor = null;
			ClasseFeicao classeFeicao = null;

			List<OperadorFeicaoShape> lstEscritores = new List<OperadorFeicaoShape>();
			List<OperadorFeicaoShape> lstEscritoresTrackmaker = new List<OperadorFeicaoShape>();

			int count = featureList.Length;
			for (int i = 0; i < count; i++)
			{
				string feicao = featureList[i];
				string alias = aliasList[i];

				classeFeicao = origem.ObterClasseFeicao(feicao);
				if (classeFeicao == null)
					continue;

				OperadorFeicaoShape escritor = null;
				AtributoCollection atributos = null;

				OperadorFeicaoShape escritorTrackmaker = null;
				AtributoCollection atributosTrackmaker = null;

				Expressao filtro = null;
				Atributo atributo;
				Feicao data;

				if (feicao == "TMP_AREAS_CALCULADAS" || feicao == "GEO_AREAS_CALCULADAS" || feicao == "TMP_CAR_AREAS_CALCULADAS")
				{
					int projetoIdFiltro = Project.Id;
					string[] tipos = null;

					if (Project.Type == OPERACAO_CAR)
					{
						tipos = "APP_APMP,APP_AA_USO,APP_AA_REC,APP_AVN,APP_ARL,CAR_APP_APMP,CAR_APP_AA_USO".Split(',');

						if (feicao == "GEO_AREAS_CALCULADAS")
						{
							projetoIdFiltro = _bus.ObterProjetoIdDominialidade(Project.Id);
						}
					}
					else
					{
						tipos = "APP_APMP,APP_AA_USO,APP_AA_REC,APP_AVN,APP_ARL".Split(',');
					}

					foreach (string tipoArea in tipos)
					{
						ExpressaoRelacionalOracleSpatial termo1 = new ExpressaoRelacionalOracleSpatial(new Campo("projeto"), TipoOperadorRelacional.Igual, new ParametroOracleSpatial(DbType.Int32, projetoIdFiltro));
						ExpressaoRelacionalOracleSpatial termo2 = new ExpressaoRelacionalOracleSpatial(new Campo("TIPO"), TipoOperadorRelacional.Igual, new ParametroOracleSpatial(DbType.String, tipoArea, 50));
						filtro = new ExpressaoLogicaOracleSpatial(termo1, TipoOperadorLogico.E, termo2);

						try
						{
							leitor = origem.ObterLeitorFeicao(feicao, null, filtro);
						}
						catch (Exception exc)
						{
							for (int k = 0; k < 5; k++)
							{
								try
								{
									ExpressaoRelacionalOracleSpatial termoAux = new ExpressaoRelacionalOracleSpatial(new Campo("1"), TipoOperadorRelacional.Igual, new Campo("1"));
									filtro = new ExpressaoLogicaOracleSpatial(filtro, TipoOperadorLogico.E, termoAux);
									leitor = origem.ObterLeitorFeicao(feicao, null, filtro);
								}
								catch (Exception)
								{
									if (k < 4)
									{
										continue;
									}

									throw new Exception(string.Format("Feicao: {0} / Project.Id: {1} / tipo {2} / where: {3}", feicao, projetoIdFiltro, tipoArea, filtro.GerarComando()), exc);
								}

								break;
							}
						}

						escritor = null;

						while (leitor.Ler())
						{
							if (escritor == null)
							{
								atributos = new AtributoCollection();
								atributos.Adicionar(classeFeicao.Atributos["AREA_M2"]);

								escritor = (OperadorFeicaoShape)destino.CriarClasseFeicao(tipoArea, leitor.Atual.Geometria.ObterTipo(), 2, false, atributos);
								prjList.Add(tipoArea);

								lstEscritores.Add(escritor);


								//Trackmaker
								atributo = new Atributo();
								atributo.Nome = "NOME";
								atributo.Tamanho = 200;
								atributo.Tipo = DbType.String;

								atributosTrackmaker = new AtributoCollection();
								atributosTrackmaker.Adicionar(atributo);
								escritorTrackmaker = (OperadorFeicaoShape)destinoTrackmaker.CriarClasseFeicao(tipoArea, leitor.Atual.Geometria.ObterTipo(), 2, false, atributosTrackmaker);

								lstEscritoresTrackmaker.Add(escritorTrackmaker);
							}

							//Normal information
							data = new Feicao();
							data.ID = leitor.Atual.ID;
							data.Geometria = leitor.Atual.Geometria;

							data.Atributos = new AtributoCollection();
							foreach (Atributo attr in atributos)
							{
								atributo = attr.Clonar();
								atributo.Valor = leitor.Atual.Atributos[atributo.Nome].Valor;
								data.Atributos.Adicionar(atributo);
							}

							escritor.Inserir(data);


							//Trackmaker information
							data = new Feicao();
							data.ID = leitor.Atual.ID;
							data.Geometria = leitor.Atual.Geometria;

							data.Atributos = new AtributoCollection();
							atributo = atributosTrackmaker[0].Clonar();
							atributo.Valor = generateTrackMakerNameValue("TMP_" + tipoArea, leitor.Atual.Atributos);
							data.Atributos.Adicionar(atributo);

							escritorTrackmaker.Inserir(data);
						}
					}
				}
				else
				{
					filtro = new ExpressaoRelacionalOracleSpatial(new Campo("projeto"), TipoOperadorRelacional.Igual, new ParametroOracleSpatial(DbType.Int32, Project.Id));

					if (feicao.IndexOf("GEO", StringComparison.InvariantCultureIgnoreCase) > -1 && Project.Type != OPERACAO_DOMINIALIDADE)
					{
						int prjIdDominialidade = _bus.ObterProjetoIdDominialidade(Project.Id);
						filtro = new ExpressaoRelacionalOracleSpatial(new Campo("projeto"), TipoOperadorRelacional.Igual, new ParametroOracleSpatial(DbType.Int32, prjIdDominialidade));
					}

					try
					{
						leitor = origem.ObterLeitorFeicao(feicao, null, filtro);
					}
					catch (Exception)
					{
						for (int k = 0; k < 5; k++)
						{
							try
							{
								ExpressaoRelacionalOracleSpatial termoAux = new ExpressaoRelacionalOracleSpatial(new Campo("1"), TipoOperadorRelacional.Igual, new Campo("1"));
								filtro = new ExpressaoLogicaOracleSpatial(filtro, TipoOperadorLogico.E, termoAux);
								leitor = origem.ObterLeitorFeicao(feicao, null, filtro);
							}
							catch (Exception)
							{
								if (k < 4)
								{
									continue;
								}

								throw;
							}

							break;
						}
					}

					while (leitor.Ler())
					{
						if (escritor == null)
						{
							atributos = new AtributoCollection();

							atributos.Adicionar(classeFeicao.Atributos["ID"]);

							if (classeFeicao.Atributos.IndiceDe("LARGURA") >= 0)
							{
								atributos.Adicionar(classeFeicao.Atributos["LARGURA"]);
							}

							if (classeFeicao.Atributos.IndiceDe("COMPRIMENTO") >= 0)
							{
								atributos.Adicionar(classeFeicao.Atributos["COMPRIMENTO"]);
							}

							if (classeFeicao.Atributos.IndiceDe("NOME") >= 0)
							{
								atributos.Adicionar(classeFeicao.Atributos["NOME"]);
							}

							if (classeFeicao.Atributos.IndiceDe("AMORTECIMENTO") >= 0)
							{
								atributos.Adicionar(classeFeicao.Atributos["AMORTECIMENTO"]);
							}

							if (classeFeicao.Atributos.IndiceDe("COD_APMP") >= 0)
							{
								atributos.Adicionar(classeFeicao.Atributos["COD_APMP"]);
							}

							if (classeFeicao.Atributos.IndiceDe("COMPENSADA") >= 0)
							{
								atributos.Adicionar(classeFeicao.Atributos["COMPENSADA"]);
							}

							if (classeFeicao.Atributos.IndiceDe("ESTAGIO") >= 0)
							{
								atributos.Adicionar(classeFeicao.Atributos["ESTAGIO"]);
							}

							//if (classeFeicao.Atributos.IndiceDe("PROJETO") >= 0)
							//{
							//    atributos.Adicionar(classeFeicao.Atributos["PROJETO"]);
							//}

							if (classeFeicao.Atributos.IndiceDe("SITUACAO") >= 0)
							{
								atributos.Adicionar(classeFeicao.Atributos["SITUACAO"]);
							}

							if (classeFeicao.Atributos.IndiceDe("TIPO") >= 0)
							{
								atributos.Adicionar(classeFeicao.Atributos["TIPO"]);
							}

							if (classeFeicao.Atributos.IndiceDe("ZONA") >= 0)
							{
								atributos.Adicionar(classeFeicao.Atributos["ZONA"]);
							}

							if (classeFeicao.Atributos.IndiceDe("AREA_M2") >= 0)
							{
								atributos.Adicionar(classeFeicao.Atributos["AREA_M2"]);
							}

							if (classeFeicao.Atributos.IndiceDe("CODIGO") >= 0)
							{
								atributos.Adicionar(classeFeicao.Atributos["CODIGO"]);
							}

							if (classeFeicao.Atributos.IndiceDe("VEGETACAO") >= 0)
							{
								atributos.Adicionar(classeFeicao.Atributos["VEGETACAO"]);
							}

							escritor = (OperadorFeicaoShape)destino.CriarClasseFeicao(alias, leitor.Atual.Geometria.ObterTipo(), 2, false, atributos);
							prjList.Add(alias);

							lstEscritores.Add(escritor);


							//Trackmaker
							atributo = new Atributo();
							atributo.Nome = "NOME";
							atributo.Tamanho = 200;
							atributo.Tipo = DbType.String;

							atributosTrackmaker = new AtributoCollection();
							atributosTrackmaker.Adicionar(atributo);
							escritorTrackmaker = (OperadorFeicaoShape)destinoTrackmaker.CriarClasseFeicao(alias, leitor.Atual.Geometria.ObterTipo(), 2, false, atributosTrackmaker);

							lstEscritoresTrackmaker.Add(escritorTrackmaker);
						}



						//Normal information
						data = new Feicao();
						data.ID = leitor.Atual.ID;
						data.Geometria = leitor.Atual.Geometria;

						data.Atributos = new AtributoCollection();
						foreach (Atributo attr in atributos)
						{
							atributo = attr.Clonar();
							atributo.Valor = leitor.Atual.Atributos[atributo.Nome].Valor;
							data.Atributos.Adicionar(atributo);
						}

						escritor.Inserir(data);


						//Trackmaker information
						data = new Feicao();
						data.ID = leitor.Atual.ID;
						data.Geometria = leitor.Atual.Geometria;

						data.Atributos = new AtributoCollection();
						atributo = atributosTrackmaker[0].Clonar();
						atributo.Valor = generateTrackMakerNameValue(feicao, leitor.Atual.Atributos);
						data.Atributos.Adicionar(atributo);

						escritorTrackmaker.Inserir(data);
					}
				}

				leitor.Fechar();
			}

			byte[] byteReturn = null;
			byte[] byteTrackmakerReturn = null;

			if (lstEscritores.Count > 0)
			{
				using (MemoryStream ms = new MemoryStream())
				{
					destino.ExportarParaZip(ms);
					byteReturn = AdicionarArquivosPrj(ms, prjList.ToArray());
				}

				using (MemoryStream ms = new MemoryStream())
				{
					destinoTrackmaker.ExportarParaZip(ms);
					byteTrackmakerReturn = AdicionarArquivosPrj(ms, prjList.ToArray());
				}

				foreach (OperadorFeicaoShape escritorShp in lstEscritores)
				{
					escritorShp.Fechar();
				}

				foreach (OperadorFeicaoShape escritorShp in lstEscritoresTrackmaker)
				{
					escritorShp.Fechar();
				}
			}



			origem.Fechar();
			destino.Fechar();
			destinoTrackmaker.Fechar();

			List<byte[]> result = new List<byte[]>();
			result.Add(byteReturn);
			result.Add(byteTrackmakerReturn);

			return result;
		}

		#endregion
	}
}