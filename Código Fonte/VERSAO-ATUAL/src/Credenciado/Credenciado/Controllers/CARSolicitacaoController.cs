using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Pdf;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCARSolicitacao;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class CARSolicitacaoController : DefaultController
	{
		#region Propriedades

		CARSolicitacaoBus _bus = new CARSolicitacaoBus();
		CARSolicitacaoInternoBus _busInterno = new CARSolicitacaoInternoBus();
		ProjetoDigitalCredenciadoBus _busProjetoDigital = new ProjetoDigitalCredenciadoBus();
		EmpreendimentoCredenciadoBus _busEmpreendimentoCredenciado = new EmpreendimentoCredenciadoBus();
		RequerimentoCredenciadoBus _busRequerimentoCredenciado = new RequerimentoCredenciadoBus();
		CARSolicitacaoValidar _validar = new CARSolicitacaoValidar();

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(ListaCredenciadoBus.QuantPaginacao, ListaCredenciadoBus.Municipios(ViewModelHelper.EstadoDefaultId()));

			vm.Situacoes = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.CARSolicitacaoSituacoes, true, true);

			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoListar })]
		public ActionResult Filtrar(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<SolicitacaoListarResultados> resultados = _bus.Filtrar(vm.Filtros, paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeVisualizar = User.IsInRole(ePermissao.CadastroAmbientalRuralSolicitacaoVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoCriar })]
		public ActionResult Criar()
		{
			CARSolicitacao solicitacao = new CARSolicitacao();
			List<PessoaLst> declarantes = new List<PessoaLst>();
			List<Lista> atividades = new List<Lista>();

			solicitacao.DataEmissao.DataTexto = DateTime.Now.ToShortDateString();

			CARSolicitacaoVM vm = new CARSolicitacaoVM(solicitacao, ListaCredenciadoBus.CARSolicitacaoSituacoes, atividades, declarantes);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoCriar })]
		public ActionResult Criar(CARSolicitacao car)
		{
			_bus.Salvar(car);

			string urlRetorno = Url.Action("Index", "CARSolicitacao") + "?Msg=" + Validacao.QueryParam();

            #region Carga das tabelas APP Caculada e APP Escadinha
            var qtdModuloFiscal = 0.0;
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {

                Comando comando = bancoDeDados.CriarComando(@"SELECT ATP_QTD_MODULO_FISCAL FROM CRT_CAD_AMBIENTAL_RURAL WHERE EMPREENDIMENTO = :empreendimentoID");

                comando.AdicionarParametroEntrada("empreendimentoID", car.Empreendimento.Id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        qtdModuloFiscal = Convert.ToDouble(reader["ATP_QTD_MODULO_FISCAL"]);
                    }

                    reader.Close();
                }
            }
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia("idafgeo"))
            {
                #region Chamada Procedure
                bancoDeDados.IniciarTransacao();
                Comando command = bancoDeDados.CriarComando(@"begin OPERACOESPROCESSAMENTOGEO.CalcularAppClassificadaCAR(:id, :emp, :tid); end;");

                command.AdicionarParametroEntrada("emp", car.Empreendimento.Id, System.Data.DbType.Int32);
               
                bancoDeDados.ExecutarNonQuery(command);

                bancoDeDados.Commit();

                bancoDeDados.IniciarTransacao();
                Comando com = bancoDeDados.CriarComando(@"begin OPERACOESPROCESSAMENTOGEO.CalcularEscadinhaCAR(:emp, :moduloFiscal); end;");

                com.AdicionarParametroEntrada("emp", car.Empreendimento.Id, System.Data.DbType.Int32);
                com.AdicionarParametroEntrada("moduloFiscal", qtdModuloFiscal, System.Data.DbType.Double);
                
                bancoDeDados.ExecutarNonQuery(com);

                bancoDeDados.Commit();
                #endregion

            }
            #endregion

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @urlRetorno = urlRetorno }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoVisualizar })]
		public ActionResult Visualizar(int id = 0, int internoId = 0)
		{
			CARSolicitacao solicitacao = new CARSolicitacao();
			CARSolicitacaoVM vm = null;

			if (internoId > 0 && id < 1)
			{
				#region Interno

				solicitacao = _busInterno.Obter(internoId, false);

				#region Configurar

				List<Protocolos> lstProcessosDocumentos = new List<Protocolos>(){new Protocolos()
				{
					IsAtivo = true,
					Id = solicitacao.ProtocoloSelecionado.Id.ToString() + "@" + (solicitacao.ProtocoloSelecionado.IsProcesso ? 1 : 2) + "@" + solicitacao.Requerimento.Id.ToString(),
					Texto = solicitacao.Requerimento.Id.ToString() + " - " + solicitacao.Requerimento.DataCadastroTexto
				}};

				List<PessoaLst> lstResponsaveis = new List<PessoaLst>(){new PessoaLst()
				{
					IsAtivo = true,
					Id = solicitacao.Declarante.Id,
					Texto = solicitacao.Declarante.NomeRazaoSocial
				}};

				List<Lista> lstAtividades = new List<Lista>(){new Lista()
				{
					IsAtivo = true,
					Id = solicitacao.Atividade.Id.ToString(),
					Texto = solicitacao.Atividade.NomeAtividade
				}};

				vm = new CARSolicitacaoVM(
					solicitacao,
					ListaCredenciadoBus.CARSolicitacaoSituacoes,
					lstAtividades,
					lstResponsaveis,
					lstProcessosDocumentos,
					isVisualizar: true);

				#endregion Configurar

				vm.IsInterno = true;

				return View(vm);

				#endregion
			}

			solicitacao = _bus.Obter(id);
			List<PessoaLst> declarantes = _bus.ObterDeclarantesLst(solicitacao.Requerimento.Id);
			List<Lista> atividades = _bus.ObterAtividadesLista(solicitacao.Requerimento.Id);
			solicitacao.Empreendimento = _busEmpreendimentoCredenciado.Obter(solicitacao.Empreendimento.Id);
			solicitacao.Empreendimento.NomeRazao = solicitacao.Empreendimento.Denominador;

			vm = new CARSolicitacaoVM(solicitacao, ListaCredenciadoBus.CARSolicitacaoSituacoes, atividades, declarantes);
			vm.IsVisualizar = true;
			vm.IsInterno = false;
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoVisualizar })]
		public ActionResult VisualizarMotivo(CARSolicitacao entidade)
		{
			return PartialView("VisualizarMotivo", entidade);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoCriar })]
		public ActionResult ObterProjetoDigital(int projetoId)
		{
			ProjetoDigital projeto = _busProjetoDigital.Obter(idProjeto: projetoId);
			Empreendimento empreendimento = _busEmpreendimentoCredenciado.Obter(projeto.EmpreendimentoId.GetValueOrDefault(), simplificado: true);
			List<Lista> atividades = _bus.ObterAtividadesLista(projeto.RequerimentoId);
			List<PessoaLst> declarantes = _bus.ObterDeclarantesLst(projeto.RequerimentoId);

			_validar.AssociarProjetoDigital(projeto, atividades);

			return Json(
				new
				{
					@EhValido = Validacao.EhValido,
					@Msg = Validacao.Erros,
					@ProjetoDigital = projeto,
					@Empreendimento = empreendimento,
					@AtividadesLst = atividades,
					@DeclaranteLst = declarantes
				}, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoListar })]
		public ActionResult GerarPdf(int id, bool isCredenciado)
		{
			try
			{
				MemoryStream resultado = null;

				if (isCredenciado)
				{
					CARSolicitacao solicitacao = _bus.Obter(id);
					int situacaoId = solicitacao.SituacaoId;
					string situacaoTexto = solicitacao.SituacaoTexto;

					resultado = new PdfCARSolicitacaoCredenciado().Gerar(id, situacaoId, situacaoTexto);
				}
				else
				{
					CARSolicitacao solicitacao = _busInterno.Obter(id, true);
					int situacaoId = solicitacao.SituacaoId;
					string situacaoTexto = solicitacao.SituacaoTexto;

					resultado = new PdfCARSolicitacaoInterno().Gerar(id, situacaoId, situacaoTexto);
				}

				if (!Validacao.EhValido)
				{
					return RedirectToAction("Index", Validacao.QueryParamSerializer());
				}

				return ViewModelHelper.GerarArquivoPdf(resultado, "Solicitacao Inscricao CAR");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		public ActionResult GerarTituloPdf(int id)
		{
			try
			{
				TituloInternoBus tituloInternoBus = new TituloInternoBus();
				Arquivo arquivo = tituloInternoBus.GerarPdf(id);

				DateTime dataAtual = DateTime.Now;
				String mensagemTarja = "Consultado em " + dataAtual.ToShortDateString() + " às " + dataAtual.ToString(@"HH\hmm\min");

				arquivo.Buffer = Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.PdfMetodosAuxiliares.TarjaVerde(arquivo.Buffer, mensagemTarja);

				if (arquivo != null && Validacao.EhValido)
				{
					return ViewModelHelper.GerarArquivo(arquivo);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return RedirectToAction("Index", Validacao.QueryParamSerializer());
		}

		public ActionResult GerarPdfPendencia(int id, bool isCredenciado)
		{
			try
			{
				MemoryStream resultado = null;

				if (isCredenciado)
				{
					resultado = new PdfCARSolicitacaoPendenciasCredenciado().Gerar(id);
				}
				else
				{
					resultado = new PdfCARSolicitacaoPendenciasInterno().Gerar(id);
				}

				if (!Validacao.EhValido)
				{
					return RedirectToAction("Index", Validacao.QueryParamSerializer());
				}

				return ViewModelHelper.GerarArquivoPdf(resultado, "Solicitacao Inscricao CAR Pendencias");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		public ActionResult EnviarReenviarArquivoSICAR(int solicitacaoId, int origem, bool isEnviar)
		{
			CARSolicitacao carSolicitacao = null;
            origem = 2;
			if (origem == (int)eCARSolicitacaoOrigem.Credenciado)
			{
				carSolicitacao = _bus.Obter(solicitacaoId);
			}

			if (!_validar.AcessoEnviarArquivoSICAR(carSolicitacao, origem))
			{
                return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros}, JsonRequestBehavior.AllowGet);
			}

            if (origem == (int)eCARSolicitacaoOrigem.Credenciado)
            {
                _bus.EnviarReenviarArquivoSICAR(solicitacaoId, isEnviar);
            }

			string urlRetorno = Url.Action("Index", "CARSolicitacao") + "?Msg=" + Validacao.QueryParam();
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRetorno = urlRetorno }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult MensagemErroEnviarArquivoSICAR(bool isEnviar, string solicitacaoSituacao, string arquivoSituacao)
		{
			Validacao.Add(Mensagem.CARSolicitacao.ErroEnviarArquivoSICAR(isEnviar, solicitacaoSituacao, (string.IsNullOrEmpty(arquivoSituacao)) ? "Nulo" : arquivoSituacao));

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}
		
		public ActionResult GerarPdfComprovanteSICAR(int id)
		{
			var schemaSolicitacao = _busInterno.ExisteCredenciado(id) ? 2 : 1;

			var url = _busInterno.ObterUrlRecibo(id, schemaSolicitacao);

			return Json(new { @UrlPdfReciboSICAR = url }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult BaixarAquivoSICAR(int id)
		{
			return ViewModelHelper.BaixarArquivoInterno(id);
		}
	}
}