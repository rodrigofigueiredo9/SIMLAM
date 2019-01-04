using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloRequerimento.Pdf;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMRequerimento;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class RequerimentoController : DefaultController
	{
		#region Propriedades

		RequerimentoCredenciadoBus _busRequerimento = new RequerimentoCredenciadoBus(new RequerimentoCredenciadoValidar());
		PessoaCredenciadoBus _busPessoa = new PessoaCredenciadoBus();
		EmpreendimentoCredenciadoBus _busEmpreendimento = new EmpreendimentoCredenciadoBus();
		CredenciadoBus _busCredenciado = new CredenciadoBus();
		RequerimentoCredenciadoValidar _validar = new RequerimentoCredenciadoValidar();
		ProjetoDigitalCredenciadoBus _busProjetoDigital = new ProjetoDigitalCredenciadoBus();

		#endregion

		#region Criar

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult Criar()
		{
			RequerimentoVM vm = new RequerimentoVM();
			vm.CarregarListas(ListaCredenciadoBus.ResponsavelFuncoes, ListaCredenciadoBus.AgendamentoVistoria);
			return View(vm);
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult Editar(int id, int projetoDigitalId)
		{
			Requerimento requerimento = _busRequerimento.Obter(id);
			requerimento.ProjetoDigitalId = projetoDigitalId;

			ProjetoDigital projetoDigital = _busProjetoDigital.Obter(requerimento.ProjetoDigitalId, requerimento.Id);

			_busRequerimento.ValidarRoteiroRemovido(requerimento);

			if (Validacao.Erros.Count <= 0)
			{
				_busRequerimento.ValidarSituacaoVersaoRoteiro(requerimento.Roteiros);
			}

			_busRequerimento.ValidarEditar(requerimento);

			if (!Validacao.EhValido)
			{
				return RedirectToAction("Index", "ProjetoDigital", Validacao.QueryParamSerializer());
			}

			RequerimentoVM vm = new RequerimentoVM(requerimento);
			vm.CarregarListas(ListaCredenciadoBus.ResponsavelFuncoes, ListaCredenciadoBus.AgendamentoVistoria);

			return View(vm);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoVisualizar })]
		public ActionResult Visualizar(int id, int projetoDigitalId, bool isVisualizar = true)
		{
			Requerimento requerimento = _busRequerimento.Obter(id);
			requerimento.ProjetoDigitalId = projetoDigitalId;

			if (requerimento == null)
			{
				return RedirectToAction("Index", "ProjetoDigital", Validacao.QueryParamSerializer());
			}

			RequerimentoVM vm = new RequerimentoVM(requerimento, isVisualizar);
			vm.CarregarListas(ListaCredenciadoBus.ResponsavelFuncoes, ListaCredenciadoBus.AgendamentoVistoria);

			if (Request.IsAjaxRequest())
			{
                vm.IsRequestAjax = true;
				return PartialView("VisualizarPartial", vm);
			}
			else
			{
				return View(vm);
			}
		}

		#endregion

		#region Objetivo do Pedido

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ObterObjetivoPedidoVisualizar(int id)
		{
			RequerimentoVM vm = new RequerimentoVM();

			if (id != 0)
			{
				Requerimento requerimento = _busRequerimento.Obter(id);

				if (requerimento != null)
				{
					vm.CarregarRequerimentoVM(requerimento);

					vm.CarregarListas(ListaCredenciadoBus.ResponsavelFuncoes, ListaCredenciadoBus.AgendamentoVistoria);
				}
			}

			return PartialView("ObjetivoPedidoVisualizar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ObterObjetivoPedido(int id)
		{
			RequerimentoVM vm = new RequerimentoVM();
			if (id != 0)
			{
				Requerimento requerimento = _busRequerimento.Obter(id);

				if (requerimento != null)
				{
					vm.CarregarRequerimentoVM(requerimento);

					vm.CarregarListas(ListaCredenciadoBus.ResponsavelFuncoes, ListaCredenciadoBus.AgendamentoVistoria);
				}
			}

			return PartialView("ObjetivoPedido", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult CriarObjetivoPedido(RequerimentoVM vm)
		{
			Requerimento requerimento = null;
			if (vm.Id != 0)
			{
				requerimento = _busRequerimento.Obter(vm.Id);

				if (requerimento != null)
				{
					vm.DataCriacao = requerimento.DataCadastro.ToString();
					vm.Empreendimento = requerimento.Empreendimento;
					vm.Responsaveis = requerimento.Responsaveis;
					vm.Interessado = requerimento.Interessado;
				}
			}

			requerimento = GerarRequerimento(vm);
			requerimento.SituacaoId = (int)eRequerimentoSituacao.EmAndamento;
			_busRequerimento.SalvarObjetivoPedido(requerimento);

			List<string> acoesErros = new List<string>();
			bool temBarragemDispensada = false;

			//327 == Barragem dispensada de licenciamento ambiental
			if (requerimento.Atividades.Count(x => x.Id == 327) > 0)
			{
				temBarragemDispensada = true;
				if (Validacao.Erros.Find(x => x.Texto == Mensagem.Requerimento.RTFaltandoInformacoesProfissao.Texto) != null)
				{
					acoesErros.Add("RTFaltandoInformacoesProfissao");
				}
			}
				

			return Json(new
			{
				id = requerimento.Id,
				projetoDigitalId = requerimento.ProjetoDigitalId,
				Msg = Validacao.Erros,
				acoes = acoesErros,
				temBarragemDeclaratoria  = temBarragemDispensada,
				idUsuario = (User.Identity as EtramiteIdentity).FuncionarioId
			});
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult VerificarPassoDois(int projetoDigitalID)
		{

			return Json(new
			{
				@IsPreenchidoPassoDois = _busProjetoDigital.ObterDependencias(projetoDigitalID).Count > 0
			}, JsonRequestBehavior.AllowGet);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult ObterTituloModelo(int atividadeId, int finalidade)
		{
			List<TituloModeloLst> modelos = _busRequerimento.ObterModelosAtividades(new List<AtividadeSolicitada>() { new AtividadeSolicitada() { Id = atividadeId } }, finalidade == 3);//3 Renovação

			return Json(new { @Lista = modelos, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult ObterTituloModeloAnterior(int titulo)
		{
			List<TituloModeloLst> tituloModelosFaseAnterior = _busRequerimento.ObterModelosAnteriores(titulo);

			List<TituloModeloLst> tituloModelosRenovacao = _busRequerimento.ObterModelosRenovacao(titulo);

			tituloModelosRenovacao.AddRange(tituloModelosFaseAnterior);

			TituloModelo modelo = _busRequerimento.VerficarTituloPassivelRenovação(titulo);

			TituloModelo modeloAtual = new TituloModeloInternoBus().Obter(titulo);

			Boolean faseAnteriorObrigatoria = Convert.ToBoolean((modeloAtual.Resposta(eRegra.FaseAnterior, eResposta.TituloAnteriorObrigatorio) ?? new TituloModeloResposta()).Valor);

			return Json(new { @Lista = tituloModelosFaseAnterior, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, @EhFaseAnterior = modelo.Regra(eRegra.FaseAnterior), @FaseAnteriorEhObrigatoria = faseAnteriorObrigatoria.ToString().ToLower(), @ListaRenovacao = tituloModelosRenovacao }, JsonRequestBehavior.AllowGet);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult ObterNumerosTitulos(string numero, int modeloId)
		{
			List<TituloModeloLst> titulos = _busRequerimento.ObterNumerosTitulos(numero, modeloId);

			return Json(new { @Lista = titulos, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult AlterarSituacao(int requerimentoId)
		{
			_busRequerimento.AlterarSituacao(new Requerimento() { Id = requerimentoId, SituacaoId = 1 });// Em andamento

			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult ObterRoteirosAtividade(List<Atividade> AtividadesSolicitadas)
		{
			List<Roteiro> listaRoteiro = _busRequerimento.ObterRoteirosPorAtividades(AtividadesSolicitadas);

			return Json(new { @Lista = listaRoteiro, @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ValidarNumeroProcesso(string numero)
		{
			_busRequerimento.ValidarNumeroProcesso(numero);

			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ValidarNumeroModeloAnterior(int tituloAnteriorId, int tituloAnteriorTipo)
		{
			_busRequerimento.ValidarModeloAnteriorNumero(tituloAnteriorId, tituloAnteriorTipo);
			return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOAlterarSituacao })]
		public ActionResult ResponsabilidadeRTBarragem(RequerimentoVM vm)
		{
			Requerimento requerimento = GerarRequerimento(vm);

			return PartialView("RTBarragemPartial", requerimento);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CFOAlterarSituacao })]
		public ActionResult InformacoesBarragem(RequerimentoVM vm)
		{
			Requerimento requerimento = GerarRequerimento(vm);

			return PartialView("InfoBarragemPartial", requerimento);
		}

		#region  Atividade Solicitada

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult CriarAtividadeSolicitada(int id)
		{
			return View("AtividadeSolicitada", new AtividadeSolicitadaVM(ListaCredenciadoBus.Finalidades));
		}

		#endregion

		#endregion

		#region Associar Interessado

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult AssociarInteressado(int requerimentoId, int interessadoId)
		{
			Requerimento requerimento = new Requerimento() { Id = requerimentoId };

			requerimento.Interessado.Id = interessadoId;

			_busRequerimento.AssociarInteressado(requerimento);

			return Json(new
			{
				EhValido = Validacao.EhValido,
				Msg = Validacao.Erros,
				RedirecionarListar = Validacao.Erros.Exists(x => x.Tipo == eTipoMensagem.Erro),
				urlRedirecionar = Url.Action("Index", "ProjetoDigital", Validacao.QueryParamSerializer())
			});
		}

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult LimparInteressado(int requerimentoId)
		{
			_busRequerimento.LimparInteressado(new Requerimento() { Id = requerimentoId });

			return Json(new { Msg = Validacao.Erros });
		}

		#endregion

		#region Responsavel Tecnico

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ObterResponsavelVisualizar(int id)
		{
			string view = "CriarRespTecnico";
			RequerimentoVM vm = new RequerimentoVM();
			if (id != 0)
			{
				Requerimento requerimento = _busRequerimento.Obter(id);
				if (requerimento != null)
				{
					vm.CarregarRequerimentoVM(requerimento);
					vm.CarregarListas(ListaCredenciadoBus.ResponsavelFuncoes, ListaCredenciadoBus.AgendamentoVistoria);

					if (requerimento.Atividades.FirstOrDefault(x => x.Id == 327) != null && requerimento.Responsaveis.FirstOrDefault()?.IdRelacionamento == 0)
					{
						view = "CriarRespTecnico";
					}
					else
					{
						view = (requerimento.Responsaveis.Count > 0) ? "ResponsavelTecnicoVisualizar" : "CriarRespTecnico";
					}
				}
			}

			return PartialView(view, vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoExcluir })]
		public ActionResult ExcluirResponsaveis(int id)
		{
			_busRequerimento.ExcluirResponsaveis(new Requerimento() { Id = id });
			return Json(new { @EhValido = Validacao.EhValido, Msg = Validacao.Erros });
		}

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ObterResponsavel(int id)
		{
			RequerimentoVM vm = new RequerimentoVM();
			if (id != 0)
			{
				Requerimento requerimento = _busRequerimento.Obter(id);
				if (requerimento != null)
				{
					vm.CarregarRequerimentoVM(requerimento);
					vm.CarregarListas(ListaCredenciadoBus.ResponsavelFuncoes, ListaCredenciadoBus.AgendamentoVistoria);
				}
			}
			return PartialView("CriarRespTecnico", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult CriarResponsavel(int id, List<ResponsavelTecnico> responsaveis)
		{
			Requerimento requerimento = _busRequerimento.Obter(id);

			if (requerimento != null)
			{
				requerimento.Responsaveis = (responsaveis == null ? new List<ResponsavelTecnico>() : responsaveis);

				_busRequerimento.SalvarResponsavelTecnico(requerimento);
			}

			return Json(new { Msg = Validacao.Erros });
		}

		#endregion

		#region Empreendimento

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult AssociarEmpreendimento(int requerimentoId, int empreendimentoId)
		{
			Requerimento requerimento = new Requerimento() { Id = requerimentoId };
			requerimento.Empreendimento = _busEmpreendimento.Obter(empreendimentoId, true);

			bool isAssociado = _busRequerimento.AssociarEmpreendimento(requerimento);


			return Json(new
			{
				empAssociado = isAssociado,
				Msg = Validacao.Erros,
				RedirecionarListar = Validacao.Erros.Exists(x => x.Tipo == eTipoMensagem.Erro),
				urlRedirecionar = Url.Action("Index", "ProjetoDigital", Validacao.QueryParamSerializer())
			});
		}

		#endregion

		#region Finalizar

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ObterFinalizar(int id)
		{
			RequerimentoVM vm = new RequerimentoVM(_busRequerimento.ObterFinalizar(id));
			vm.IsAbaFinalizar = true;
			vm.CarregarListas(ListaCredenciadoBus.ResponsavelFuncoes, ListaCredenciadoBus.AgendamentoVistoria);

			return PartialView("Finalizar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult Finalizar(Requerimento requerimento)
		{
			_busRequerimento.Finalizar(requerimento);

			if (!Validacao.EhValido)
			{
				return Json(new { id = requerimento.Id, Msg = Validacao.Erros });
			}

			string urlRedirect = Url.Action("Operar", "ProjetoDigital", Validacao.QueryParamSerializer(new { Id = requerimento.ProjetoDigitalId }));
			return Json(new { id = requerimento.Id, redirect = urlRedirect, Msg = Validacao.Erros });
		}

		#endregion

		#region Excluir

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();
			vm.Id = id;
			vm.Titulo = "Excluir Requerimento";
			vm.Mensagem = Mensagem.Requerimento.ExcluirConfirmacao(id);
			return View("Confirmar", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoExcluir })]
		public ActionResult Excluir(int id)
		{
			_busRequerimento.Excluir(id);
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido });
		}

		#endregion

		#region GerarPdf

		[Permite(RoleArray = new Object[] { ePermissao.RequerimentoCriar, ePermissao.RequerimentoEditar })]
		public ActionResult ObterReqInterEmp(int id)
		{
			Requerimento requerimento = _busRequerimento.ObterSimplificado(id);
			return Json(new { @Msg = Validacao.Erros, @Requerimento = new { requerimentoId = id, interessadoId = requerimento.Interessado.Id, empreendimentoId = requerimento.Empreendimento.Id } }, JsonRequestBehavior.AllowGet);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		private Requerimento GerarRequerimento(RequerimentoVM vm)
		{
			Requerimento req = new Requerimento();
			req.Id = vm.Numero;
			req.DataCadastro = Convert.ToDateTime(vm.DataCriacao);
			req.Roteiros = vm.Roteiros;
			req.Atividades = vm.AtividadesSolicitadas;
			req.Responsaveis = vm.Responsaveis;
			req.Empreendimento = vm.Empreendimento;
			req.Interessado = vm.Interessado;
			req.AgendamentoVistoria = vm.AgendamentoVistoriaId;
			req.SetorId = vm.SetorId;
			req.Informacoes = vm.InformacaoComplementar;
			req.InfoPreenchidas = (vm.AbastecimentoPublico.HasValue && vm.AbastecimentoPublico == 0
								   && vm.UnidadeConservacao.HasValue && vm.UnidadeConservacao == 0
								   && vm.SupressaoVegetacao.HasValue && vm.SupressaoVegetacao == 0
								   && vm.Realocacao.HasValue && vm.Realocacao == 0);

			return req;
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult GerarPdf(int id)
		{
			if (!_validar.Posse(id))
			{
				return RedirectToAction("Index", "ProjetoDigital", Validacao.QueryParamSerializer());
			}

			try
			{
				bool isCredenciado = false;
				PdfRequerimentoPadraoCredenciado pdf = new PdfRequerimentoPadraoCredenciado();
				return ViewModelHelper.GerarArquivoPdf(pdf.Gerar(id, out isCredenciado), (isCredenciado ? "Requerimento Digital" : "Requerimento Padrao"));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", "ProjetoDigital", Validacao.QueryParamSerializer());
			}
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult GerarPdfInterno(int id)
		{
			if (!_validar.PosseInterno(id))
			{
				return RedirectToAction("Index", "ProjetoDigital", Validacao.QueryParamSerializer());
			}

			try
			{
				bool isCredenciado = false;
				PdfRequerimentoPadraoCredenciado pdf = new PdfRequerimentoPadraoCredenciado();
				return ViewModelHelper.GerarArquivoPdf(pdf.Gerar(id, out isCredenciado, true), (isCredenciado ? "Requerimento Digital" : "Requerimento Padrao"));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("Index", "ProjetoDigital", Validacao.QueryParamSerializer());
			}
		}

		#endregion
	}
}