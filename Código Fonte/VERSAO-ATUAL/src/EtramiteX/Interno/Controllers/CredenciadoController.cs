using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloOrgaoParceiroConveniado.Bussiness;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloCredenciado.Pdf;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado;
using Pes = Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class CredenciadoController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		CredenciadoIntBus _bus = new CredenciadoIntBus();
		HabilitarEmissaoCFOCFOCBus _busHabilitar = new HabilitarEmissaoCFOCFOCBus();
		LiberacaoNumeroCFOCFOCBus _busLiberacao = new LiberacaoNumeroCFOCFOCBus();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		OrgaoParceiroConveniadoBus _busOrgaoParceiro = new OrgaoParceiroConveniadoBus();
		#endregion

		#region Filtrar

		[Permite(RoleArray = new Object[] { ePermissao.CredenciadoListar, ePermissao.CredenciadoVisualizar, ePermissao.CredenciadoRegerarChave, ePermissao.CredenciadoAlterarSituacao })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busLista.CredenciadoTipos, _busLista.CredenciadoSituacoes);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CredenciadoListar, ePermissao.CredenciadoVisualizar, ePermissao.CredenciadoRegerarChave, ePermissao.CredenciadoAlterarSituacao })]
		public ActionResult CredenciadoAssociar()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busLista.CredenciadoTipos, _busLista.CredenciadoSituacoes);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.IsAssociar = true;
			return PartialView("ListarFiltros", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.CredenciadoListar, ePermissao.CredenciadoVisualizar, ePermissao.CredenciadoRegerarChave, ePermissao.CredenciadoAlterarSituacao })]
		public ActionResult Filtrar(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, _busLista.CredenciadoTipos, _busLista.CredenciadoSituacoes, vm.Paginacao.QuantPaginacao);

			var resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;
			vm.PodeVisualizar = User.IsInRole(ePermissao.CredenciadoVisualizar.ToString()) && !vm.IsAssociar;

			vm.PodeHabilitar = (User.IsInRole(ePermissao.HabilitarEmissaoCFOCFOCCriar.ToString()) ||
								User.IsInRole(ePermissao.HabilitarEmissaoCFOCFOCEditar.ToString())) &&
								!vm.IsAssociar;

			vm.PodeAlterarSituacao = User.IsInRole(ePermissao.CredenciadoAlterarSituacao.ToString()) && !vm.IsAssociar;
			vm.PodeRegerarChave = User.IsInRole(ePermissao.CredenciadoRegerarChave.ToString()) && !vm.IsAssociar;

			return Json(new { Msg = Validacao.Erros, Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		[Permite(RoleArray = new Object[] { ePermissao.CredenciadoVisualizar, ePermissao.HabilitarEmissaoCFOCFOCCriar })]
		public ActionResult Obter(String CpfCnpj)
		{
			var pessoa = _bus.Obter(CpfCnpj);

			if (pessoa != null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, Responsavel = pessoa }, JsonRequestBehavior.AllowGet);

			}
			else
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.CredenciadoVisualizar })]
		public ActionResult Visualizar(int id)
		{
			CredenciadoVM vm = new CredenciadoVM();

			vm.Credenciado = _bus.Obter(id);
			vm.Credenciado.Pessoa = _bus.ObterPessoaCredenciado(vm.Credenciado.Pessoa.Id);

			vm.PessoaVM.Pessoa = vm.Credenciado.Pessoa;
			vm.PessoaVM.CpfCnpjValido = true;
			vm.PessoaVM.IsCredenciado = true;
			vm.PessoaVM.ExibirMensagensPartial = true;
			vm.PessoaVM.ExibirBotoes = true;
			vm.PessoaVM.OcultarLimparPessoa = true;
			vm.PessoaVM.UrlAcao = Url.Action("AlterarDados", "Credenciado");
			vm.PessoaVM.IsVisualizar = true;
			vm.OrgaosParceiros = ViewModelHelper.CriarSelectList(_busOrgaoParceiro.ObterOrgaosParceirosLst(), true, true, vm.Credenciado.OrgaoParceiroId.ToString());
			vm.OrgaosParceirosUnidades = ViewModelHelper.CriarSelectList(_busOrgaoParceiro.ObterUnidadesLst(vm.Credenciado.OrgaoParceiroId), true, true, vm.Credenciado.OrgaoParceiroUnidadeId.ToString());

			CarregaCampos(vm);
			PreencheSalvarVM(vm.PessoaVM);

			if (AjaxRequestExtensions.IsAjaxRequest(this.Request))
			{
				return View("CredenciadoPartial", vm);
			}

			return View(vm);
		}

		#endregion

		#region Alterar Situação

		[Permite(RoleArray = new Object[] { ePermissao.CredenciadoAlterarSituacao })]
		public ActionResult AlterarSituacao(int id)
		{
			AlterarSituacaoVM viewModel = new AlterarSituacaoVM(_busLista.CredenciadoSituacoes.Where(x => x.Id == 3).ToList());


			CredenciadoPessoa credenciado = _bus.Obter(id);

			if (credenciado != null)
			{
				viewModel.Id = id;
				viewModel.Nome = credenciado.Pessoa.NomeRazaoSocial;
				viewModel.CpfCnpj = credenciado.Pessoa.CPFCNPJ;
				viewModel.Situacao = _busLista.CredenciadoSituacoes.Single(x => x.Id == credenciado.Situacao).Texto;
				viewModel.SituacaoId = credenciado.Situacao;

			}

			return View(viewModel);

		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CredenciadoAlterarSituacao })]
		public ActionResult AlterarSituacao(int credenciadoId, string nome, int situacao, string motivo)
		{
			bool sucesso = _bus.AlterarSituacao(credenciadoId, nome, situacao, motivo);

			return Json(new { EhValido = sucesso, Msg = Validacao.Erros });
		}

		#endregion

		#region Regerar Chave

		[Permite(RoleArray = new Object[] { ePermissao.CredenciadoRegerarChave })]
		public ActionResult RegerarChave(string id)
		{
			int idCred = Convert.ToInt32(id);

			SalvarVM vm = new SalvarVM();
			vm.Credenciado = _bus.Obter(idCred);
			vm.Credenciado.Id = idCred;
			vm.Credenciado.Nome = vm.Credenciado.Pessoa.NomeRazaoSocial;

			return View("RegerarChave", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.CredenciadoRegerarChave })]
		public ActionResult RegerarChave(int id)
		{
			_bus.RegerarChave(id);

			string situacao = _busLista.CredenciadoSituacoes.Single(x => x.Id == 5).Nome; // 5 - Alterar Senha

			return Json(new { Msg = Validacao.Erros, situacaoAtual = situacao, EhValido = Validacao.EhValido });
		}

		#endregion

		#region Auxiliares

		public List<Contato> PreencheContato(Pes.ContatoVME contato)
		{
			List<Contato> meiosContatos = new List<Contato>();

			try
			{
				if (!string.IsNullOrEmpty(contato.TelefoneResidencial))
				{
					meiosContatos.Add(new Contato { Valor = contato.TelefoneResidencial, TipoContato = eTipoContato.TelefoneResidencial });
				}

				if (!string.IsNullOrEmpty(contato.TelefoneComercial))
				{
					meiosContatos.Add(new Contato { Valor = contato.TelefoneComercial, TipoContato = eTipoContato.TelefoneComercial });
				}

				if (!string.IsNullOrEmpty(contato.TelefoneCelular))
				{
					meiosContatos.Add(new Contato { Valor = contato.TelefoneCelular, TipoContato = eTipoContato.TelefoneCelular });
				}

				if (!string.IsNullOrEmpty(contato.TelefoneFax))
				{
					meiosContatos.Add(new Contato { Valor = contato.TelefoneFax, TipoContato = eTipoContato.TelefoneFax });
				}

				if (!string.IsNullOrEmpty(contato.Email))
				{
					meiosContatos.Add(new Contato { Valor = contato.Email, TipoContato = eTipoContato.Email });
				}

				return meiosContatos;
			}
			catch (Exception exc)
			{
				Validacao.AddAdvertencia(exc.Message);
			}

			return null;
		}

		private Pes.ContatoVME CarregaMeiosContatos(List<Contato> meiosContatos)
		{
			Pes.ContatoVME contatoVME = new Pes.ContatoVME();

			if (meiosContatos != null && meiosContatos.Count > 0)
			{
				foreach (Contato cont in meiosContatos)
				{
					switch (cont.TipoContato)
					{
						case eTipoContato.TelefoneResidencial:
							contatoVME.TelefoneResidencial = cont.Valor;
							break;

						case eTipoContato.TelefoneCelular:
							contatoVME.TelefoneCelular = cont.Valor;
							break;

						case eTipoContato.TelefoneFax:
							contatoVME.TelefoneFax = cont.Valor;
							break;

						case eTipoContato.TelefoneComercial:
							contatoVME.TelefoneComercial = cont.Valor;
							break;

						case eTipoContato.Email:
							contatoVME.Email = cont.Valor;
							break;
					}
				}
			}

			return contatoVME;
		}

		public CredenciadoVM CarregaCampos(CredenciadoVM vm)
		{
			if (vm.Pessoa.MeiosContatos != null && vm.Pessoa.MeiosContatos.Count > 0)
			{
				vm.Contato = CarregaMeiosContatos(vm.Pessoa.MeiosContatos);
			}
			else
			{
				vm.Pessoa.MeiosContatos = PreencheContato(vm.Contato);
			}

			if (!vm.Pessoa.IsJuridica)
			{
				if (vm.Pessoa.Fisica.DataNascimento != null)
				{
					vm.PessoaVM.DataNascimento = vm.Pessoa.Fisica.DataNascimento.Value.ToShortDateString();
				}
				else
				{
					vm.Pessoa.Fisica.DataNascimento = ValidacoesGenericasBus.ParseData(vm.DataNascimento);
				}
			}

			return vm;
		}

		public Pessoa PreencheSalvarVM(Pes.SalvarVM vm)
		{
			vm.EstadosCivis = ViewModelHelper.CriarSelectList(_busLista.EstadosCivil, true);
			vm.Sexos = ViewModelHelper.CriarSelectList(_busLista.Sexos, true);
			vm.Profissoes = ViewModelHelper.CriarSelectList(_busLista.Profissoes, true);
			vm.OrgaoClasses = ViewModelHelper.CriarSelectList(_busLista.OrgaosClasse, true);
			vm.Estados = ViewModelHelper.CriarSelectList(_busLista.Estados, true);

			if (vm.Pessoa != null || vm.Pessoa.Endereco.EstadoId > 0)
			{
				vm.Municipios = ViewModelHelper.CriarSelectList(_busLista.Municipios(vm.Pessoa.Endereco.EstadoId), true);
			}
			else
			{
				vm.Municipios = ViewModelHelper.CriarSelectList(new List<Municipio>(), true);
			}

			if (vm.Pessoa.Fisica != null && vm.Pessoa.Fisica.Profissao != null)
			{
				if (vm.Pessoa.Fisica.Profissao.Id > 0)
				{
					vm.Pessoa.Fisica.Profissao.ProfissaoTexto = _busLista.Profissoes.Single(x => x.Id == vm.Pessoa.Fisica.Profissao.Id).Texto;
				}
				else
				{
					vm.Pessoa.Fisica.Profissao.Id = 0;
					vm.Pessoa.Fisica.Profissao.IdRelacionamento = 0;
					vm.Pessoa.Fisica.Profissao.ProfissaoTexto = "*** Associar uma profissão ***";
				}
			}

			return vm.Pessoa;
		}

		#endregion

		#region Habilitar Emissão de CFO e CFOC

		[Permite(RoleArray = new Object[] { ePermissao.HabilitarEmissaoCFOCFOCCriar })]
		public ActionResult CriarHabilitarEmissaoCFOCFOC()
		{
			HabilitarEmissaoCFOCFOCVM viewModel = new HabilitarEmissaoCFOCFOCVM(_busLista.HabilitacaoCFOSituacoes, _busLista.Estados, _busLista.HabilitacaoCFOMotivos);
			viewModel.HabilitarEmissao.UF = ViewModelHelper.EstadoDefaultId();
			viewModel.IsVisualizar = false;
			viewModel.IsEditar = false;
			viewModel.IsAjaxRequest = false;

			return View("HabilitarEmissaoCFOCFOC", viewModel);
		}

		[Permite(RoleArray = new Object[] { ePermissao.HabilitarEmissaoCFOCFOCCriar, ePermissao.HabilitarEmissaoCFOCFOCEditar })]
		public ActionResult ObterCredenciadoHabilitar(int id)
		{
			var habilitar = _busHabilitar.Obter(id, true);
			if (Validacao.EhValido && habilitar != null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros, Habilitar = habilitar }, JsonRequestBehavior.AllowGet);
			}
			else
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}
		}

		[Permite(RoleArray = new Object[] { ePermissao.HabilitarEmissaoCFOCFOCEditar })]
		public ActionResult EditarHabilitarEmissaoCFOCFOC(int id)
		{
			HabilitarEmissaoCFOCFOCVM viewModel = new HabilitarEmissaoCFOCFOCVM(_busLista.HabilitacaoCFOSituacoes, _busLista.Estados, _busLista.HabilitacaoCFOMotivos);
			viewModel.HabilitarEmissao = _busHabilitar.Obter(id, isEditar: true);
			viewModel.IsEditar = true;
			viewModel.IsVisualizar = false;
			viewModel.IsAjaxRequest = false;

			return View("HabilitarEmissaoCFOCFOC", viewModel);
		}

		[Permite(RoleArray = new Object[] { ePermissao.HabilitarEmissaoCFOCFOCVisualizar })]
		public ActionResult VisualizarHabilitarEmissaoCFOCFOC(int id)
		{
			HabilitarEmissaoCFOCFOCVM viewModel = new HabilitarEmissaoCFOCFOCVM(_busLista.HabilitacaoCFOSituacoes, _busLista.Estados, _busLista.HabilitacaoCFOMotivos);
			viewModel.HabilitarEmissao = _busHabilitar.Obter(id);
			Validacao.Erros.Clear();

			viewModel.IsVisualizar = true;
			viewModel.IsEditar = false;
			viewModel.IsAjaxRequest = false;

			return View("HabilitarEmissaoCFOCFOC", viewModel);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.HabilitarEmissaoCFOCFOCCriar, ePermissao.HabilitarEmissaoCFOCFOCEditar })]
		public ActionResult SalvarHabilitarEmissao(HabilitarEmissaoCFOCFOCVM vm)
		{
			if (_busHabilitar.Salvar(vm.HabilitarEmissao))
			{
				Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.Salvar);
				string urlRedireciona = Url.Action(((vm.HabilitarEmissao.Id > 0) ? "IndexHabilitarEmissaoCFOCFOC" : "CriarHabilitarEmissaoCFOCFOC"), "Credenciado");
				urlRedireciona += "?Msg=" + Validacao.QueryParam();

				return Json(new { IsSalvo = Validacao.EhValido, UrlRedireciona = urlRedireciona, @HabilitarEmissao = vm.HabilitarEmissao, Msg = Validacao.Erros });
			}
			else
			{
				return Json(new { IsSalvo = false, Msg = Validacao.Erros });
			}
		}

		[Permite(RoleArray = new Object[] { ePermissao.HabilitarEmissaoCFOCFOCListar, ePermissao.HabilitarEmissaoCFOCFOCVisualizar, ePermissao.HabilitarEmissaoCFOCFOCEditar, ePermissao.HabilitarEmissaoCFOCFOCAlterarSituacao })]
		public ActionResult FiltrarHabilitarEmissaoCFOCFOC(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, _busLista.CredenciadoTipos, _busLista.CredenciadoSituacoes, vm.Paginacao.QuantPaginacao);

			var resultados = _busHabilitar.Filtrar(vm.Filtros, vm.Paginacao);

			if (resultados == null)
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;
			vm.PodeVisualizar = User.IsInRole(ePermissao.HabilitarEmissaoCFOCFOCVisualizar.ToString());
			vm.PodeAlterarSituacao = User.IsInRole(ePermissao.HabilitarEmissaoCFOCFOCAlterarSituacao.ToString());
			vm.PodeEditar = User.IsInRole(ePermissao.HabilitarEmissaoCFOCFOCEditar.ToString());

			return Json(new { Msg = Validacao.Erros, Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultadosHabilitarEmissaoCFOCFOC", vm) }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.HabilitarEmissaoCFOCFOCListar, ePermissao.HabilitarEmissaoCFOCFOCVisualizar, ePermissao.HabilitarEmissaoCFOCFOCEditar, ePermissao.HabilitarEmissaoCFOCFOCAlterarSituacao })]
		public ActionResult IndexHabilitarEmissaoCFOCFOC()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao, _busLista.CredenciadoTipos, _busLista.CredenciadoSituacoes);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View("IndexHabilitarEmissaoCFOCFOC", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.HabilitarEmissaoCFOCFOCCriar, ePermissao.HabilitarEmissaoCFOCFOCEditar })]
		public ActionResult ValidarAdicionarPraga(HabilitarEmissaoCFOCFOC habilitar, PragaHabilitarEmissao praga)
		{
			_busHabilitar.ValidarPraga(habilitar, praga);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.HabilitarEmissaoCFOCFOCEditar })]
		public ActionResult RenovarPraga()
		{
			HabilitarEmissaoCFOCFOCVM vm = new HabilitarEmissaoCFOCFOCVM(_busLista.HabilitacaoCFOSituacoes, _busLista.Estados, _busLista.HabilitacaoCFOMotivos);

			return PartialView("RenovarDataHabilitacaoCFO", vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.HabilitarEmissaoCFOCFOCAlterarSituacao })]
		public ActionResult RenovarDataHabilitacaoCFO(PragaHabilitarEmissao praga)
		{
			if (_busHabilitar.RenovarData(praga))
			{
				return Json(new { EhValido = Validacao.EhValido, Msg = Validacao.Erros });
			}
			else
			{
				return Json(new { EhValido = false, Msg = Validacao.Erros });
			}
		}

		#region Alterar Situação

		[Permite(RoleArray = new Object[] { ePermissao.HabilitarEmissaoCFOCFOCCriar, ePermissao.HabilitarEmissaoCFOCFOCEditar })]
		public ActionResult AlterarHabilitarEmissaoCFOCFOC(HabilitarEmissaoCFOCFOC habilitar)
		{
			HabilitarEmissaoCFOCFOCVM viewModel = new HabilitarEmissaoCFOCFOCVM(_busLista.HabilitacaoCFOSituacoes, _busLista.Estados, _busLista.HabilitacaoCFOMotivos);
			if (Validacao.EhValido && habilitar != null)
			{
				viewModel.HabilitarEmissao = habilitar;
				viewModel.HabilitarEmissao.UF = viewModel.HabilitarEmissao.UF == 0 ? ViewModelHelper.EstadoDefaultId() : viewModel.HabilitarEmissao.UF;
				viewModel.IsEditar = true;
				viewModel.IsVisualizar = false;
				viewModel.IsAjaxRequest = true;
			}
			return PartialView("HabilitarEmissaoCFOCFOCSalvar", viewModel);
		}

        //Carrega e preenche o modal "Alterar Situação da Habilitação"
		[Permite(RoleArray = new Object[] { ePermissao.HabilitarEmissaoCFOCFOCAlterarSituacao })]
		public ActionResult AlterarSituacaoHabilitacaoCFO(int id)
		{
            var motivos = _busLista.HabilitacaoCFOMotivos;
            var situacoes = _busLista.HabilitacaoCFOSituacoes.Where(x => x.Texto.ToLower() != "advertido").ToList();
			HabilitarEmissaoCFOCFOCVM vm = new HabilitarEmissaoCFOCFOCVM(situacoes, _busLista.Estados, motivos);
			vm.HabilitarEmissao = _busHabilitar.Obter(id);

			var sit = vm.Situacoes.FirstOrDefault(x => x.Value == vm.HabilitarEmissao.Situacao.ToString());
			if (sit != null)
			{
				vm.Situacoes.Remove(sit);
			}

			return PartialView("HabilitacaoCFOAlterarSituacao", vm);
		}

        //Salvar na tela de "Alterar Situação da Habilitação"
		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.HabilitarEmissaoCFOCFOCAlterarSituacao })]
		public ActionResult AlterarSituacaoHabilitacaoCFO(HabilitarEmissaoCFOCFOC habilitar)
		{
			if (_busHabilitar.AlterarSituacao(habilitar))
			{
				Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.Alterado);
				string urlRedireciona = Url.Action("IndexHabilitarEmissaoCFOCFOC", "Credenciado");
				urlRedireciona += "?Msg=" + Validacao.QueryParam();
				return Json(new { EhValido = Validacao.EhValido, UrlRedireciona = urlRedireciona, Msg = Validacao.Erros });
			}
			else
			{
				return Json(new { EhValido = false, Msg = Validacao.Erros });
			}
		}

		#endregion

		[Permite(RoleArray = new Object[] { ePermissao.HabilitarEmissaoCFOCFOCListar, ePermissao.HabilitarEmissaoCFOCFOCVisualizar })]
		public ActionResult GerarPdfHabilitarEmissaoCFOCFOC(int id)
		{
			try
			{
				return ViewModelHelper.GerarArquivoPdf(new PdfHabilitarEmissaoCFOCFOC().Gerar(id), "Habilitar Emissao de CFO e CFOC");
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return RedirectToAction("IndexHabilitarEmissaoCFOCFOC", Validacao.QueryParamSerializer());
			}
		}

		#endregion
	}
}