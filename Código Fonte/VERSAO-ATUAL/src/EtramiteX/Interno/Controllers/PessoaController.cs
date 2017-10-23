using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa;

namespace Tecnomapas.EtramiteX.Interno.Controllers
{
	public class PessoaController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		PessoaBus _bus = new PessoaBus(new PessoaValidar());

		private string QuantidadePorPagina
		{
			get { return (Request.Cookies.Get("QuantidadePorPagina") != null) ? Request.Cookies.Get("QuantidadePorPagina").Value : "5"; }
		}

		#endregion

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar, ePermissao.PessoaEditar, ePermissao.PessoaListar })]
		public ActionResult Index()
		{
			ListarVM vm = new ListarVM(_busLista.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(QuantidadePorPagina);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PessoaListar })]
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

			Resultados<Pessoa> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeEditar = User.IsInRole(ePermissao.PessoaEditar.ToString());
			vm.PodeExcluir = User.IsInRole(ePermissao.PessoaExcluir.ToString());
			vm.PodeVisualizar = User.IsInRole(ePermissao.PessoaVisualizar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "ListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Criar

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult CriarVerificarCpfCnpj(SalvarVM vmVerificar)
		{
			bool isCpfCnpjValido = false;
			int pessoaId = 0;
			String urlAcao = Url.Action("Criar", "Pessoa");

			try
			{
				SalvarVM vm = new SalvarVM(_busLista.EstadosCivil, _busLista.Sexos, _busLista.Profissoes, _busLista.OrgaosClasse, _busLista.Estados);

				isCpfCnpjValido = _bus.VerificarCriarCpfCnpj(vmVerificar.Pessoa);

				if (isCpfCnpjValido)
				{
					Pessoa pessoa = _bus.Obter(vmVerificar.Pessoa.CPFCNPJ);
					pessoaId = pessoa.Id;

					if (pessoa.Id > 0)
					{
						urlAcao = Url.Action("Editar", "Pessoa");
						if (pessoa.IsFisica)
						{
							Validacao.Add(Mensagem.Pessoa.CpfCadastrado);
						}
						else
						{
							Validacao.Add(Mensagem.Pessoa.CnpjCadastrado);
						}
					} else {
						if (vmVerificar.Pessoa.Tipo == 1) {
							Validacao.Add(Mensagem.Pessoa.CpfNaoCadastrado);
						} else {
							Validacao.Add(Mensagem.Pessoa.CnpjNaoCadastrado);
						}
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Json(new { IsCpfCnpjValido = isCpfCnpjValido, PessoaId = pessoaId, UrlAcao = urlAcao, Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar })]
		public ActionResult Criar(string cpfCnpj, int? tipoPessoa, int tipoCadastro = 0, bool fiscalizacao = false)
		{
			SalvarVM vm = new SalvarVM(_busLista.EstadosCivil, _busLista.Sexos, _busLista.Profissoes, _busLista.OrgaosClasse, _busLista.Estados);
			vm.ExibirMensagensPartial = true;
			vm.ExibirBotoes = true;
			vm.UrlAcao = Url.Action("Criar", "Pessoa");
			vm.TipoCadastro = tipoCadastro;

			vm.CpfCnpjValido = !String.IsNullOrEmpty(cpfCnpj);

			if (vm.CpfCnpjValido)
			{
				vm.Pessoa.Tipo = tipoPessoa.Value;
				if (vm.Pessoa.Tipo == 1)//Fisica
				{
					vm.Pessoa.Fisica.CPF = cpfCnpj;
				}
				else
				{
					vm.Pessoa.Juridica.CNPJ = cpfCnpj;
				}
			}

			if (Request.IsAjaxRequest())
			{
                if (fiscalizacao == false)
                {
                    return PartialView("PessoaPartial", vm);
                }
                else
                {
                    return PartialView("PessoaPartialFiscalizacao", vm);
                }
			}
			else
			{
				return View(vm);
			}
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar })]
		public ActionResult Criar(SalvarVM vm, bool fiscalizacao = false)
		{
			string urlRedireciona = Url.Action("Criar", "Pessoa");
			vm.CpfCnpjValido = true;
			vm.Pessoa.Ativa = 1; //Indica que pessoa está ativa no sistema

			if (vm.TipoCadastro != 0)
			{
				vm.Pessoa.Tipo = (vm.TipoCadastro == 2) ? PessoaTipo.JURIDICA : PessoaTipo.FISICA;
			}

            if (!fiscalizacao)
            {
                CarregaCampos(vm);
            }

			if (vm.TipoCadastro == 1 && (vm.Pessoa.Fisica.EstadoCivil == 2 || vm.Pessoa.Fisica.EstadoCivil == 5))
			{
				vm.Pessoa.Fisica.ConjugeId = -1;
			}

            if (!fiscalizacao)
            {
                if (_bus.Salvar(vm.Pessoa))
                {
                    Validacao.Add(Mensagem.Pessoa.Salvar);
                    urlRedireciona += "?Msg=" + Validacao.QueryParam();
                }
            }
			return Json(new { IsPessoaSalva = Validacao.EhValido, UrlRedireciona = urlRedireciona, @Pessoa = vm.Pessoa, Msg = Validacao.Erros });
		}

		#endregion
		
		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.PessoaEditar })]
		public ActionResult Editar(int id, int tipoCadastro = 0)
		{
			SalvarVM vm = new SalvarVM();

			vm.CpfCnpjValido = true;
			vm.Pessoa = _bus.Obter(id);
			vm.TipoCadastro = tipoCadastro;
			
			CarregaCampos(vm);
			PreencheSalvarVM(vm);

			vm.UrlAcao = Url.Action("Editar", "Pessoa");
			vm.ExibirMensagensPartial = true;
			vm.ExibirBotoes = true;
			vm.ExibirLimparPessoa = false;

			if (Request.IsAjaxRequest())
			{
				return PartialView("PessoaPartial", vm);
			}

			return View(vm);
		}
		
		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PessoaEditar })]
		public ActionResult Editar(SalvarVM vm)
		{
			CarregaCampos(vm);

			if (vm.TipoCadastro == 1 && (vm.Pessoa.Fisica.EstadoCivil == 2 || vm.Pessoa.Fisica.EstadoCivil == 5))
			{
				vm.Pessoa.Fisica.ConjugeId = -1;
			}
			
			if (_bus.Salvar(vm.Pessoa))
			{
				Validacao.Add(Mensagem.Pessoa.Editar);
				string urlRedireciona = Url.Action("Index", "Pessoa");
				urlRedireciona += "?Msg=" + Validacao.QueryParam();
				return Json(new { IsPessoaSalva = true, @Pessoa = vm.Pessoa, Msg = Validacao.Erros, UrlRedireciona = urlRedireciona });
			}
			else
			{
				if (Request.IsAjaxRequest())
				{
					return Json(new { IsPessoaSalva = false, Msg = Validacao.Erros });
				}
				else
				{
					vm.CpfCnpjValido = true;
					PreencheSalvarVM(vm);
					vm.ExibirMensagensPartial = true;
					vm.ExibirBotoes = true;
					vm.ExibirLimparPessoa = false;
					return View(vm);
				}
			}
		}

		#endregion

		#region Visualizar
		
		[Permite(RoleArray = new Object[] { ePermissao.PessoaVisualizar })]
		public ActionResult Visualizar(int id)
		{
			SalvarVM vm = new SalvarVM(_busLista.EstadosCivil, _busLista.Sexos, _busLista.Profissoes, _busLista.OrgaosClasse, _busLista.Estados);

			vm.CpfCnpjValido = true;
			vm.IsVisualizar = true;
			vm.Pessoa = _bus.Obter(id);
			CarregaCampos(vm);
			PreencheSalvarVM(vm);			

			if (Request.IsAjaxRequest())
			{
				return PartialView("PessoaPartialVisualizar", vm);
			}
			
			return View(vm);
		}
		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.PessoaExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ExcluirVM vm = new ExcluirVM();

			Pessoa pessoa = _bus.Obter(id);
			vm.Id = id;
			vm.Mensagem = Mensagem.Pessoa.MensagemExcluir(pessoa.NomeRazaoSocial);
			vm.Titulo = "Excluir Pessoa";
			return PartialView("Excluir", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PessoaExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Associar

		[Permite(RoleArray = new Object[] { ePermissao.PessoaListar })]
		public ActionResult RepresentanteAssociar()
		{
			ListarVM vm = new ListarVM();
			vm.SetListItens(_busLista.QuantPaginacao, Convert.ToInt32(QuantidadePorPagina));
			return PartialView("RepresentanteListarFiltros", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PessoaListar })]
		public ActionResult RepresentanteFiltrar(ListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<ListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Filtros.Tipo = 1;
			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(_busLista.QuantPaginacao, vm.Paginacao.QuantPaginacao);

			Resultados<Pessoa> resultados = _bus.Filtrar(vm.Filtros, vm.Paginacao);
			if (resultados == null)
			{
				return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
			}

			vm.PodeAssociar = true;//User.IsInRole(ePermissao.PessoaAssociar.ToString());

			vm.Paginacao.QuantidadeRegistros = resultados.Quantidade;
			vm.Paginacao.EfetuarPaginacao();
			vm.Resultados = resultados.Itens;

			return Json(new { @Msg = Validacao.Erros, @Html = ViewModelHelper.RenderPartialViewToString(ControllerContext, "RepresentanteListarResultados", vm) }, JsonRequestBehavior.AllowGet);
		}
		
		#endregion

		#region Validar

		[HttpPost]
		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult ValidarAssociarRepresentante(int pessoaId)
		{
			_bus.ValidarAssociarRepresentante(pessoaId);
			return Json(new { IsPessoaSalva = Validacao.EhValido, Msg = Validacao.Erros });
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public List<Contato> PreencheContato(ContatoVME contato)
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

				if (!string.IsNullOrEmpty(contato.Nome))
				{
					meiosContatos.Add(new Contato { Valor = contato.Nome, TipoContato = eTipoContato.NomeContato });
				}

				return meiosContatos;
			}
			catch (Exception exc)
			{
				Validacao.AddAdvertencia(exc.Message);
			}
			return null;
		}

		private ContatoVME CarregaMeiosContatos(List<Contato> meiosContatos)
		{
			ContatoVME contatoVME = new ContatoVME();

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
						case eTipoContato.NomeContato:
							contatoVME.Nome = cont.Valor;
							break;
					}
				}
			}
			return contatoVME;
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public SalvarVM CarregaCampos(SalvarVM vm)
		{
			if (vm.Pessoa.MeiosContatos == null || vm.Pessoa.MeiosContatos.Count == 0 )
			{
				vm.Pessoa.MeiosContatos = PreencheContato(vm.Contato);
			}

			if (!vm.Pessoa.IsJuridica)
			{
				if (vm.Pessoa.Fisica.DataNascimento != null)
				{
					vm.DataNascimento = vm.Pessoa.Fisica.DataNascimento.Value.ToShortDateString();
				}
				else
				{
					vm.Pessoa.Fisica.DataNascimento = ValidacoesGenericasBus.ParseData(vm.DataNascimento);
				}
			}

			if (vm.Pessoa.MeiosContatos != null && vm.Pessoa.MeiosContatos.Count > 0)
			{
				vm.Contato = CarregaMeiosContatos(vm.Pessoa.MeiosContatos);
			}
			return vm;
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public Pessoa PreencheSalvarVM(SalvarVM vm)
		{
			vm.EstadosCivis = ViewModelHelper.CriarSelectList(_busLista.EstadosCivil, true);
			vm.Sexos = ViewModelHelper.CriarSelectList(_busLista.Sexos, true);
			vm.Profissoes = ViewModelHelper.CriarSelectList(_busLista.Profissoes, true);
			vm.OrgaoClasses = ViewModelHelper.CriarSelectList(_busLista.OrgaosClasse, true);
			vm.Estados = ViewModelHelper.CriarSelectList(_busLista.Estados, true);

			if (vm.Pessoa != null || vm.Pessoa.Endereco.EstadoId > 0)
			{
				vm.Municipios = (List<SelectListItem>)(from m in ViewModelHelper.CriarSelectList(_busLista.Municipios(vm.Pessoa.Endereco.EstadoId), true) orderby m.Text ascending select m).ToList();
			}
			else
			{
				vm.Municipios = new List<SelectListItem>() { ViewModelHelper.SelecionePadrao };
			}

			if (vm.Pessoa.Fisica != null && vm.Pessoa.Fisica.Profissao != null)
			{
				if (vm.Pessoa.Fisica.Profissao.Id > 0)
				{
					vm.Pessoa.Fisica.Profissao.ProfissaoTexto = _bus.ObterProfissao(vm.Pessoa.Fisica.Profissao.Id);
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

		#region Temporário modal

		[Permite(RoleArray = new Object[] { ePermissao.PessoaVisualizar })]
		public ActionResult PessoaModalVisualizar(int id, string tid = null)
		{
			SalvarVM vm = new SalvarVM();

			vm.CpfCnpjValido = true;
			vm.Pessoa =  (String.IsNullOrWhiteSpace(tid)) ? _bus.Obter(id) : _bus.ObterHistorico(id, tid);
			CarregaCampos(vm);
			PreencheSalvarVM(vm);

			vm.UrlAcao = Url.Action("Visualizar", "Pessoa");
			vm.ExibirMensagensPartial = true;
			vm.ExibirBotoes = true;
			vm.ExibirLimparPessoa = false;
			vm.IsVisualizar = true;

			return PartialView("PessoaModal", vm);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		public ActionResult PessoaModal(string cpfCnpj, int? tipoPessoa, int tipoCadastro = 0)
		{
			SalvarVM vm = new SalvarVM(_busLista.EstadosCivil, _busLista.Sexos, _busLista.Profissoes, _busLista.OrgaosClasse, _busLista.Estados);
			vm.ExibirMensagensPartial = true;
			vm.ExibirBotoes = true;

			vm.TipoCadastro = tipoCadastro;
			if (vm.TipoCadastro != 0)
			{
				vm.Pessoa.Tipo = (vm.TipoCadastro == 2) ? PessoaTipo.JURIDICA : PessoaTipo.FISICA;
			}

			return PessoaModal(vm, cpfCnpj, tipoPessoa);
		}

		[Permite(Tipo = ePermiteTipo.Logado)]
		private ActionResult PessoaModal(SalvarVM vm, string cpfCnpj = "", int? tipoPessoa = null)
		{
			vm.UrlAcao = Url.Action("Criar", "Pessoa");

			vm.CpfCnpjValido = !String.IsNullOrEmpty(cpfCnpj);

			if (vm.CpfCnpjValido)
			{
				vm.Pessoa.Tipo = tipoPessoa.Value;
				if (vm.Pessoa.Tipo == 1)//Fisica
				{
					vm.Pessoa.Fisica.CPF = cpfCnpj;
				}
				else
				{
					vm.Pessoa.Juridica.CNPJ = cpfCnpj;
				}
			}

			if (Request.IsAjaxRequest())
			{
				return PartialView(vm);
			}
			else
			{
				return View(vm);
			}
		}

		#endregion

		#region Inline

		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar, ePermissao.PessoaEditar })]
		public ActionResult PessoaInline(int? id, string cpfCnpj = "", int? tipoPessoa = null)
		{
			SalvarVM vm = new SalvarVM(_busLista.EstadosCivil, _busLista.Sexos, _busLista.Profissoes, _busLista.OrgaosClasse, _busLista.Estados);

			if ((id??0) != 0)
			{
				vm.CpfCnpjValido = true;
				vm.IsVisualizar = true;
				vm.Pessoa = _bus.Obter(id.Value);
				CarregaCampos(vm);
				PreencheSalvarVM(vm);
				return PartialView(vm);
			}

			vm.ExibirMensagensPartial = false;
			vm.ExibirBotoes = false;

			vm.UrlAcao = Url.Action("Criar", "Pessoa");

			vm.CpfCnpjValido = !String.IsNullOrEmpty(cpfCnpj);

			if (vm.CpfCnpjValido)
			{
				vm.Pessoa.Tipo = tipoPessoa.Value;
				if (vm.Pessoa.Tipo == 1)//Fisica
				{
					vm.Pessoa.Fisica.CPF = cpfCnpj;
				}
				else
				{
					vm.Pessoa.Juridica.CNPJ = cpfCnpj;
				}
			}

			return PartialView(vm);
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar, ePermissao.PessoaEditar })]
		public ActionResult ObterEndereco(int pessoaId)
		{

			Endereco endereco = _bus.Obter(pessoaId).Endereco;

			return Json(new
			{
				@Msg = Validacao.Erros,
				@EhValido = Validacao.EhValido,
				@Endereco = endereco
			}, JsonRequestBehavior.AllowGet);

		}

		#endregion
	}
}