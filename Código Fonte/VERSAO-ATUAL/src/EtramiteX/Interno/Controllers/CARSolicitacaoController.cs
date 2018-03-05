using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMCARSolicitacao;
using CARSolicitacaoCredenciadoBus = Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business.CARSolicitacaoBus;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class CARSolicitacaoController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		CARSolicitacaoBus _bus = new CARSolicitacaoBus();
		CARSolicitacaoCredenciadoBus _busCredenciado = new CARSolicitacaoCredenciadoBus();

		TituloBus _busTitulo = new TituloBus();
		AtividadeBus _busAtividade = new AtividadeBus();
		ProtocoloBus _busProtocolo = new ProtocoloBus();

		public static EtramitePrincipal Usuario
		{
			get { return (System.Web.HttpContext.Current.User as EtramitePrincipal); }
		}

		#endregion

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busLista.Municipios(ViewModelHelper.EstadoDefaultId()), _busLista.CadastroAmbientalRuralSolicitacaoSituacao, _busLista.CadastroAmbientalRuralSolicitacaoOrigem);
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
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<SolicitacaoListarResultados> resultados = _bus.Filtrar(vm.Filtros, paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			if (!vm.PodeAssociar)
			{
				vm.PodeEditar = User.IsInRole(ePermissao.CadastroAmbientalRuralSolicitacaoEditar.ToString());
				vm.PodeExcluir = User.IsInRole(ePermissao.CadastroAmbientalRuralSolicitacaoExcluir.ToString());
				vm.PodeAlterarSituacao = User.IsInRole(ePermissao.CadastroAmbientalRuralSolicitacaoAlterarSituacao.ToString());
			}
			vm.PodeVisualizar = User.IsInRole(ePermissao.CadastroAmbientalRuralSolicitacaoVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoCriar })]
		public ActionResult Criar()
		{
			CARSolicitacao solicitacao = new CARSolicitacao();
			solicitacao.DataEmissao.Data = DateTime.Now;
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(solicitacao.Protocolo.Id.GetValueOrDefault(0));
			CARSolicitacaoVM vm = new CARSolicitacaoVM(solicitacao, _busLista.CadastroAmbientalRuralSolicitacaoSituacao, lstProcessosDocumentos, new List<ProcessoAtividadeItem>(), new List<PessoaLst>());

            return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoCriar })]
		public ActionResult Criar(CARSolicitacao entidade)
		{
			_bus.Salvar(entidade);         
            
			string urlRetorno = Url.Action("Index", "CARSolicitacao") + "?Msg=" + Validacao.QueryParam();
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @urlRetorno = urlRetorno }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoEditar, ePermissao.CadastroAmbientalRuralCriar })]
		public ActionResult Editar(int id)
		{

			CARSolicitacao solicitacao = _bus.Obter(id);

			if (!_bus.Validar.AcessarEditar(solicitacao))
			{
				return RedirectToAction("Index", "CARSolicitacao", Validacao.QueryParamSerializer());
			}

			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(solicitacao.Protocolo.Id.GetValueOrDefault(0));

			CARSolicitacaoVM vm = new CARSolicitacaoVM(solicitacao, _busLista.CadastroAmbientalRuralSolicitacaoSituacao, lstProcessosDocumentos, _busLista.AtividadesSolicitada, _bus.ObterResponsaveis(solicitacao.Empreendimento.Id));
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoEditar })]
		public ActionResult Editar(CARSolicitacao entidade)
		{
			_bus.Salvar(entidade);

			string urlRetorno = Url.Action("Index", "CARSolicitacao") + "?Msg=" + Validacao.QueryParam() + "&acaoId=" + entidade.Id.ToString();
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @urlRetorno = urlRetorno }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoVisualizar })]
		public ActionResult Visualizar(int id)
		{
			bool existeCredenciado = _bus.ExisteCredenciado(id);
            CARSolicitacao solicitacao = null;
			CARSolicitacao solicitacaoAux = existeCredenciado ? _busCredenciado.Obter(id) : _bus.Obter(id, false);
            if (solicitacaoAux.SituacaoId != (int)eCARSolicitacaoSituacao.EmCadastro && solicitacaoAux.SituacaoId != (int)eCARSolicitacaoSituacao.Pendente)
			{
				//Obtem ultimo valido, porem mantem a situacao atual
				int situacaoAtual = solicitacaoAux.SituacaoId;

                solicitacao = existeCredenciado ? _busCredenciado.ObterHistoricoPrimeiraSituacao(solicitacaoAux.Id, eCARSolicitacaoSituacao.Valido) :
                                                  _bus.ObterHistoricoPrimeiraSituacao(solicitacaoAux.Id, eCARSolicitacaoSituacao.Valido);
                if (solicitacao.Id <= 0)
                {
                    solicitacao = existeCredenciado ? _busCredenciado.ObterHistoricoPrimeiraSituacao(solicitacaoAux.Id, eCARSolicitacaoSituacao.SubstituidoPeloTituloCAR) :
                                                  _bus.ObterHistoricoPrimeiraSituacao(solicitacaoAux.Id, eCARSolicitacaoSituacao.SubstituidoPeloTituloCAR);
                }

                if (solicitacao.Id <= 0)
                {
                    solicitacao = solicitacaoAux;
                }

				solicitacao.SituacaoId = situacaoAtual;
			}

			#region Configurar

            if (solicitacao != null)
            {
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

                CARSolicitacaoVM vm = new CARSolicitacaoVM(
                    solicitacao,
                    _busLista.CadastroAmbientalRuralSolicitacaoSituacao,
                    lstProcessosDocumentos,
                    _busLista.AtividadesSolicitada,
                    lstResponsaveis,
                    isVisualizar: true);

                vm.IsCredenciado = existeCredenciado;
                return View(vm);
            }
            else
            {
                List<Protocolos> lstProcessosDocumentos = new List<Protocolos>(){new Protocolos()
			{
				IsAtivo = true,
				Id = solicitacaoAux.ProtocoloSelecionado.Id.ToString() + "@" + (solicitacaoAux.ProtocoloSelecionado.IsProcesso ? 1 : 2) + "@" + solicitacaoAux.Requerimento.Id.ToString(),
				Texto = solicitacaoAux.Requerimento.Id.ToString() + " - " + solicitacaoAux.Requerimento.DataCadastroTexto
			}};

                    List<PessoaLst> lstResponsaveis = new List<PessoaLst>(){new PessoaLst()
			{
				IsAtivo = true,
				Id = solicitacaoAux.Declarante.Id,
				Texto = solicitacaoAux.Declarante.NomeRazaoSocial
			}};

                CARSolicitacaoVM vm = new CARSolicitacaoVM(
                    solicitacaoAux,
                    _busLista.CadastroAmbientalRuralSolicitacaoSituacao,
                    lstProcessosDocumentos,
                    _busLista.AtividadesSolicitada,
                    lstResponsaveis,
                    isVisualizar: true);

                vm.IsCredenciado = existeCredenciado;
                return View(vm);                
            }

			#endregion Configurar

            //return null;
		}

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoVisualizar })]
		public ActionResult VisualizarMotivo(int id)
		{
			bool existeCredenciado = _bus.ExisteCredenciado(id);

			CARSolicitacao solicitacao = existeCredenciado ? _busCredenciado.Obter(id) : _bus.Obter(id, false);

			return PartialView("VisualizarMotivo", solicitacao);
		}

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();
			CARSolicitacao solicitacao = _bus.Obter(id, true);
			vm.Id = solicitacao.Id;
			vm.Mensagem = Mensagem.CARSolicitacao.SolicitacaoMensagemExcluir(solicitacao.Numero);
			vm.Titulo = "Excluir Solicitação";
			return PartialView("Confirmar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Alterar Situação

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoAlterarSituacao })]
		public ActionResult AlterarSituacao(int id)
		{
			CARSolicitacao solicitacao = _bus.Obter(id);

			if (solicitacao.Id == 0)
			{
				solicitacao = _busCredenciado.Obter(id);
			}

			if (!_bus.Validar.AcessarAlterarSituacao(solicitacao))
			{
				return RedirectToAction("Index", "CARSolicitacao", Validacao.QueryParamSerializer());
			}

			CARSolicitacaoAlterarSituacaoVM vm = new CARSolicitacaoAlterarSituacaoVM(solicitacao, _bus.ObterSituacoes(solicitacao.SituacaoId));
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoAlterarSituacao })]
		public ActionResult AlterarSituacao(CARSolicitacao solicitacao)
		{
			_bus.AlterarSituacao(solicitacao);

			string urlRetorno = Url.Action("Index", "CARSolicitacao") + "?Msg=" + Validacao.QueryParam() + "&acaoId=" + solicitacao.Id + "&situacaoId=" + solicitacao.SituacaoId;
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @UrlRedirecionar = urlRetorno }, JsonRequestBehavior.AllowGet);
		}


		#endregion Alterar Situação

		#region Enviar/Reenviar

		public ActionResult EnviarReenviarArquivoSICAR(int solicitacaoId, int origem, bool isEnviar)
		{

			CARSolicitacao solicitacao = null;

			if (origem == (int)eCARSolicitacaoOrigem.Credenciado)
				solicitacao = _busCredenciado.Obter(solicitacaoId);
			else
				solicitacao = _bus.Obter(solicitacaoId);

			if (!_bus.Validar.AcessoEnviarReenviarArquivoSICAR(solicitacao, origem))
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);

				//return RedirectToAction("Index", "CARSolicitacao", Validacao.QueryParamSerializer());
			}

			if (origem == (int)eCARSolicitacaoOrigem.Credenciado)
			{
                _busCredenciado.EnviarReenviarArquivoSICAR(solicitacao, isEnviar);
			}
			else
			{
				_bus.EnviarReenviarArquivoSICAR(solicitacaoId, isEnviar);
			}

			string urlRetorno = Url.Action("Index", "CARSolicitacao") + "?Msg=" + Validacao.QueryParam();
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros, urlRetorno = urlRetorno }, JsonRequestBehavior.AllowGet);
		}

		#endregion Enviar/Reenviar

		#region GerarPdf

		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarpdf, Artefato = (int)eHistoricoArtefato.carsolicitacao)]
		public ActionResult GerarPdf(int id)
		{
			try
			{
				MemoryStream resultado = null;

				if (_bus.ExisteCredenciado(id))
				{
					CARSolicitacao solicitacao = _busCredenciado.Obter(id);
					int situacaoId = solicitacao.SituacaoId;
					string situacaoTexto = solicitacao.SituacaoTexto;
					resultado = new PdfCARSolicitacaoCredenciado().Gerar(id, situacaoId, situacaoTexto);
				}
				else
				{
					CARSolicitacao solicitacao = _bus.Obter(id, true);
					int situacaoId = solicitacao.SituacaoId;
					string situacaoTexto = solicitacao.SituacaoTexto;
					resultado = new PdfCARSolicitacao().Gerar(id, situacaoId, situacaoTexto);
				}

				if (!Validacao.EhValido)
				{
					return RedirectToAction("Index", Validacao.QueryParamSerializer());
				}

				return ViewModelHelper.GerarArquivo("Solicitacao Inscricao CAR.pdf", resultado, "application/pdf", dataHoraControleAcesso: true);

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		[ControleAcesso(Acao = (int)eControleAcessoAcao.visualizarpdftitulocar, Artefato = (int)eHistoricoArtefato.titulo)]
		public ActionResult GerarTituloPdf(int id)
		{
			try
			{
				Arquivo arquivo = _busTitulo.GerarPdf(id);
				arquivo.Nome = arquivo.Nome.RemoverAcentos() + ".pdf";

				DateTime dataAtual = DateTime.Now;
				String mensagemTarja = "Consultado em " + dataAtual.ToShortDateString() + " às " + dataAtual.ToString(@"HH\hmm\min");

				arquivo.Buffer = Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.PdfMetodosAuxiliares.TarjaVerde(arquivo.Buffer, mensagemTarja);

				if (arquivo != null && Validacao.EhValido)
				{
					return ViewModelHelper.GerarArquivo(arquivo, dataHoraControleAcesso: true);
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

				return ViewModelHelper.GerarArquivo("Solicitacao Inscricao CAR Pendencias".RemoverAcentos() + ".pdf", resultado, "application/pdf");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", Validacao.QueryParamSerializer());
			}
		}

		public ActionResult MensagemErroEnviarArquivoSICAR(bool isEnviar, string solicitacaoSituacao, string arquivoSituacao)
		{
			Validacao.Add(Mensagem.CARSolicitacao.ErroEnviarArquivoSICAR(isEnviar, solicitacaoSituacao, (string.IsNullOrEmpty(arquivoSituacao)) ? "Nulo" : arquivoSituacao));

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GerarPdfComprovanteSICAR(int id)
		{
			var schemaSolicitacao = _bus.ExisteCredenciado(id) ? 2 : 1;

			var url = _bus.ObterUrlRecibo(id, schemaSolicitacao);

			return Json(new { @UrlPdfReciboSICAR = url }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult BaixarAquivoSICAR(int id)
		{
			return ViewModelHelper.BaixarArquivo(id);
		}


		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoCriar, ePermissao.CadastroAmbientalRuralSolicitacaoEditar })]
		public ActionResult ObterAtividades(int protocoloId)
		{
			List<AtividadeSolicitada> lstAtividades = new List<AtividadeSolicitada>();

			lstAtividades = _busAtividade.ObterAtividadesLista(new Protocolo() { Id = protocoloId });

			return Json(new { @Atividades = lstAtividades }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CadastroAmbientalRuralSolicitacaoCriar, ePermissao.CadastroAmbientalRuralSolicitacaoEditar })]
		public ActionResult ObterProcessosDocumentos(int protocoloId)
		{
			List<Protocolos> lstProcessosDocumentos = _busTitulo.ObterProcessosDocumentos(protocoloId);
			IProtocolo proc = _busProtocolo.Obter(protocoloId);

			if (!_bus.ValidarAssociar(proc, Usuario.EtramiteIdentity.FuncionarioId))
			{
				return Json(new
				{
					@EhValido = Validacao.EhValido,
					@Msg = Validacao.Erros
				},
				JsonRequestBehavior.AllowGet);
			}



			Empreendimento emp = new EmpreendimentoBus().Obter(proc.Empreendimento.Id);

			return Json(new
			{
				@EhValido = Validacao.EhValido,
				@Msg = Validacao.Erros,
				@ProcessosDocumentos = lstProcessosDocumentos,
				@Empreendimento = emp,
				@DeclaranteLst = _bus.ObterResponsaveis(emp.Id)
			},
				JsonRequestBehavior.AllowGet);
		}

		#endregion Auxiliares
	}
}