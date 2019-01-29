using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPessoa;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
	public class PessoaController : DefaultController
	{
		#region Propriedades

		PessoaCredenciadoBus _bus;
		PessoaInternoBus _busInterno;
		PessoaCredenciadoValidar _validar;
		RequerimentoCredenciadoBus _requerimentoBus;

		#endregion

		public PessoaController()
		{
			_bus = new PessoaCredenciadoBus();
			_busInterno = new PessoaInternoBus();
			_validar = new PessoaCredenciadoValidar();
			_requerimentoBus = new RequerimentoCredenciadoBus();
		}

		#region Listar

		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar, ePermissao.PessoaEditar, ePermissao.PessoaListar })]
		public ActionResult Index()
		{
			PessoaListarVM vm = new PessoaListarVM(ListaCredenciadoBus.QuantPaginacao);
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PessoaListar })]
		public ActionResult Filtrar(PessoaListarVM vm, Paginacao paginacao)
		{
			if (!String.IsNullOrEmpty(vm.UltimaBusca))
			{
				vm.Filtros = ViewModelHelper.JsSerializer.Deserialize<PessoaListarVM>(vm.UltimaBusca).Filtros;
			}

			vm.Paginacao = paginacao;
			vm.UltimaBusca = HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(vm.Filtros));
			vm.Paginacao.QuantPaginacao = Convert.ToInt32(ViewModelHelper.CookieQuantidadePorPagina);
			vm.SetListItens(ListaCredenciadoBus.QuantPaginacao, vm.Paginacao.QuantPaginacao);

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

		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar })]
		public ActionResult CriarVerificarCpfCnpj(PessoaVM vmVerificar)
		{
			//var requerimentoId = Request.UrlReferrer.AbsolutePath.Substring(Request.UrlReferrer.AbsolutePath.LastIndexOf(Convert.ToChar("/")) + 1);

			var _busRequerimento = new RequerimentoCredenciadoBus(new RequerimentoCredenciadoValidar());
			var  isAtividadeCorteAssociada = _busRequerimento.IsRequerimentoAtividadeCorte(Convert.ToInt32(vmVerificar.requerimentoId));

			PessoaVM vm = null;
			Pessoa pessoa = new Pessoa();
			bool isCpfCnpjValido = false;
			String urlAcao = Url.Action("Criar", "Pessoa");

			try
			{
				vm = new PessoaVM(ListaCredenciadoBus.EstadosCivil, ListaCredenciadoBus.Sexos, ListaCredenciadoBus.Profissoes, ListaCredenciadoBus.OrgaosClasse, ListaCredenciadoBus.Estados);
				vm.IsAtividadeCorteAssociada = isAtividadeCorteAssociada;
				isCpfCnpjValido = _bus.VerificarCriarCpfCnpj(vmVerificar.Pessoa);
				if (isCpfCnpjValido)
				{
					if(!vm.IsAtividadeCorteAssociada)
						pessoa = _bus.Obter(vmVerificar.Pessoa.CPFCNPJ, simplificado: true, credenciadoId: _bus.User.FuncionarioId);
					pessoa.InternoId = _busInterno.ObterId(vmVerificar.Pessoa.CPFCNPJ);

					if (pessoa.InternoId > 0 && pessoa.Id <= 0)
					{
						urlAcao = Url.Action("Visualizar", "Pessoa");
						vm.Pessoa.IsCopiado = true;
						if (!vm.IsAtividadeCorteAssociada)
							Validacao.Add(Mensagem.Credenciado.PessoaExistenteInterno(pessoa.IsFisica ? "CPF" : "CNPJ"));
					}
					else
					{
						if (pessoa.Id > 0)
						{
							urlAcao = Url.Action("Visualizar", "Pessoa");

							if (pessoa.IsFisica)
							{
								Validacao.Add(Mensagem.Pessoa.CpfCadastrado);
							}
							else
							{
								Validacao.Add(Mensagem.Pessoa.CnpjCadastrado);
							}
						}
						else
						{
							if (vmVerificar.Pessoa.IsFisica)
							{
								Validacao.Add(Mensagem.Pessoa.CpfNaoCadastrado);
							}
							else
							{
								Validacao.Add(Mensagem.Pessoa.CnpjNaoCadastrado);
							}
						}
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Json(new { IsCpfCnpjValido = isCpfCnpjValido, PessoaId = pessoa.Id, InternoId = pessoa.InternoId, isCopiado = vm.Pessoa.IsCopiado, UrlAcao = urlAcao, Msg = Validacao.Erros, isAtividadeCorteAssociada = vm.IsAtividadeCorteAssociada  }, JsonRequestBehavior.AllowGet);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar })]
		public ActionResult Criar(string cpfCnpj, int? tipoPessoa, int tipoCadastro = 0)
		{
			PessoaVM vm = new PessoaVM(ListaCredenciadoBus.EstadosCivil, ListaCredenciadoBus.Sexos, ListaCredenciadoBus.Profissoes, ListaCredenciadoBus.OrgaosClasse, ListaCredenciadoBus.Estados);
			vm.ExibirMensagensPartial = true;
			vm.UrlAcao = Url.Action("Criar", "Pessoa");
			vm.TipoCadastro = tipoPessoa.GetValueOrDefault();

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
				return PartialView("PessoaPartial", vm);
			}
			else
			{
				return View(vm);
			}
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar })]
		public ActionResult Criar(PessoaVM vm)
		{
			string urlRedireciona = Url.Action("Criar", "Pessoa");
			vm.CpfCnpjValido = true;
			vm.Pessoa.Ativa = 1; //Indica que pessoa está ativa no sistema

			if (vm.TipoCadastro != 0)
			{
				vm.Pessoa.Tipo = (vm.TipoCadastro == 2) ? PessoaTipo.JURIDICA : PessoaTipo.FISICA;
			}

			CarregaCampos(vm);

			if (vm.TipoCadastro == 1 && !vm.Pessoa.IsCopiado && (vm.Pessoa.Fisica.EstadoCivil == 2 || vm.Pessoa.Fisica.EstadoCivil == 5))
			{
				vm.Pessoa.Fisica.ConjugeId = -1;
			}

			if (vm.Pessoa.IsFisica)
			{
				vm.Pessoa.IsValidarConjuge = !vm.IsConjuge;
			}

			bool isAtividadeDeCorte = _requerimentoBus.IsRequerimentoAtividadeCorte(vm.requerimentoId);

			if (_bus.Salvar(vm.Pessoa, isAtividadeDeCorte: isAtividadeDeCorte))
			{
				Validacao.Add(Mensagem.Pessoa.Salvar);
				urlRedireciona += "?Msg=" + Validacao.QueryParam();
			}

			return Json(new { IsPessoaSalva = Validacao.EhValido, UrlRedireciona = urlRedireciona, @Pessoa = vm.Pessoa, Msg = Validacao.Erros });
		}

		#endregion

		#region Editar

		[Permite(RoleArray = new Object[] { ePermissao.PessoaEditar })]
		public ActionResult Editar(int id, int tipoCadastro = 0, int internoId = 0, bool isCopiado = false, bool isConjuge = false)
		{
			PessoaVM vm = new PessoaVM();
			vm.IsConjuge = isConjuge;

			if (id > 0)
			{
				vm.Pessoa = _bus.Obter(id);
				if (!_validar.EmPosseCredenciado(vm.Pessoa))
				{
					Validacao.Add(Mensagem.Pessoa.Posse);

					if (Request.IsAjaxRequest())
					{
						return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
					}

					return RedirectToAction("Index", Validacao.QueryParamSerializer());
				}
			}
			else
			{
				vm.Pessoa = _busInterno.Obter(internoId);
				vm.Pessoa.IsCopiado = true;

				Pessoa pesCredenciado = _bus.Obter(vm.Pessoa.CPFCNPJ, credenciadoId: _bus.User.FuncionarioId);
				vm.Pessoa.Id = pesCredenciado.Id;
				vm.Pessoa.InternoId = internoId;

				if (vm.Pessoa.IsJuridica)
				{
					vm.Pessoa.Juridica.Representantes.ForEach(representante =>
					{
						representante.InternoId = representante.Id;
						representante.Id = 0;
						representante.IsCopiado = true;
					});
				}
				else
				{
					if (vm.Pessoa.Fisica.ConjugeId > 0)
					{
						Pessoa pesCredenciadoAux = _bus.Obter(vm.Pessoa.Fisica.ConjugeCPF, simplificado: true, credenciadoId: _bus.User.FuncionarioId);
						vm.Pessoa.Fisica.ConjugeInternoId = vm.Pessoa.Fisica.ConjugeId;
						vm.Pessoa.Fisica.ConjugeId = pesCredenciadoAux.Id;
					}
				}

				if (!isCopiado && pesCredenciado.Fisica.ConjugeId.GetValueOrDefault() > 0)
				{
					vm.Pessoa.Fisica.ConjugeId = pesCredenciado.Fisica.ConjugeId.GetValueOrDefault();
				}
			}

			vm.CpfCnpjValido = true;
			vm.ExibirMensagensPartial = true;
			vm.OcultarLimparPessoa = true;
			vm.TipoCadastro = tipoCadastro;

			CarregaCampos(vm);
			PreencheSalvarVM(vm);

			vm.UrlAcao = Url.Action("Editar", "Pessoa");

			if (Request.IsAjaxRequest())
			{
				return PartialView("PessoaPartial", vm);
			}

			return View(vm);
		}

		[HttpPost]
		[Permite(RoleArray = new Object[] { ePermissao.PessoaEditar })]
		public ActionResult Editar(PessoaVM vm)
		{
			CarregaCampos(vm);

			if (vm.Pessoa.IsFisica)
			{
				vm.Pessoa.IsValidarConjuge = !vm.IsConjuge;
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
					vm.OcultarLimparPessoa = true;
					return View(vm);
				}
			}
		}

		[Permite(RoleArray = new Object[] { ePermissao.PessoaEditar })]
		public ActionResult CopiarDadosIdaf(int id, int internoId = 0)
		{
			PessoaVM vm = new PessoaVM();

			vm.Pessoa = _busInterno.Obter(internoId);
			vm.Pessoa.Id = _bus.Obter(vm.Pessoa.CPFCNPJ, simplificado: true, credenciadoId: _bus.User.FuncionarioId).Id;
			vm.Pessoa.InternoId = internoId;
			vm.TipoCadastro = vm.Pessoa.Tipo;

			if (!string.IsNullOrEmpty(vm.Pessoa.Fisica.ConjugeCPF))
			{
				Pessoa conjuge = _bus.Obter(vm.Pessoa.Fisica.ConjugeCPF, simplificado: true, credenciadoId: _bus.User.FuncionarioId);

				vm.Pessoa.Fisica.Conjuge = new Pessoa()
				{
					Id = conjuge.Id,
					InternoId = vm.Pessoa.Fisica.ConjugeId
				};

				vm.Pessoa.Fisica.ConjugeInternoId = vm.Pessoa.Fisica.ConjugeId;
				vm.Pessoa.Fisica.ConjugeId = conjuge.Id;
			}

			if (vm.Pessoa.IsJuridica)
			{
				vm.Pessoa.Juridica.Representantes.ForEach(representante =>
				{
					representante.Fisica.ConjugeId = 0;
					representante.InternoId = representante.Id;
					representante.Id = 0;
				});
			}

			vm.CpfCnpjValido = true;
			vm.ExibirMensagensPartial = true;
			vm.OcultarLimparPessoa = true;

			CarregaCampos(vm);
			PreencheSalvarVM(vm);

			vm.UrlAcao = Url.Action("Editar", "Pessoa");

			if (Request.IsAjaxRequest())
			{
				return PartialView("PessoaPartial", vm);
			}

			return View(vm);
		}

		#endregion

		#region Visualizar

		[Permite(RoleArray = new Object[] { ePermissao.PessoaVisualizar })]
		public ActionResult Visualizar(int id, int internoId = 0)
		{
			PessoaVM vm = new PessoaVM(ListaCredenciadoBus.EstadosCivil, ListaCredenciadoBus.Sexos, ListaCredenciadoBus.Profissoes, ListaCredenciadoBus.OrgaosClasse, ListaCredenciadoBus.Estados);

			if (id > 0)
			{
				vm.Pessoa = _bus.Obter(id);
			}
			else
			{
				vm.Pessoa = _busInterno.Obter(internoId);
				vm.Pessoa.IsCopiado = true;

				Pessoa pesCredenciado = _bus.Obter(vm.Pessoa.CPFCNPJ, simplificado: true, credenciadoId: _bus.User.FuncionarioId);
				vm.Pessoa.Id = pesCredenciado.Id;
				vm.Pessoa.InternoId = internoId;

				if (vm.Pessoa.IsJuridica)
				{
					vm.Pessoa.Juridica.Representantes.ForEach(representante =>
					{
						representante.InternoId = representante.Id;
						representante.Id = 0;
						representante.IsCopiado = true;
					});
				}
				else
				{
					if (vm.Pessoa.Fisica.ConjugeId > 0)
					{
						Pessoa pesCredenciadoAux = _bus.Obter(vm.Pessoa.Fisica.ConjugeCPF, simplificado: true, credenciadoId: _bus.User.FuncionarioId);
						vm.Pessoa.Fisica.ConjugeInternoId = vm.Pessoa.Fisica.ConjugeId;
						vm.Pessoa.Fisica.ConjugeId = pesCredenciadoAux.Id;
					}
				}
			}

			vm.CpfCnpjValido = true;
			vm.IsVisualizar = true;

			CarregaCampos(vm);
			PreencheSalvarVM(vm);

			if (Request.IsAjaxRequest())
			{
				return PartialView("PessoaPartialVisualizar", vm);
			}

			return View(vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PessoaVisualizar })]
		public ActionResult PessoaModalVisualizar(int id, int internoId = 0, bool isConjuge = false)
		{
			PessoaVM vm = new PessoaVM();

			if (id > 0)
			{
				vm.Pessoa = _bus.Obter(id);
			}
			else
			{
				vm.Pessoa = _bus.ObterPessoa(interno: internoId);
				vm.Pessoa.Id = _bus.Obter(vm.Pessoa.CPFCNPJ, simplificado: true, credenciadoId: _bus.User.FuncionarioId).Id;

				if (vm.Pessoa.IsJuridica)
				{
					vm.Pessoa.Juridica.Representantes.ForEach(representante =>
					{
						representante.IsCopiado = true;
					});
				}
			}

			vm.CpfCnpjValido = true;
			vm.ExibirMensagensPartial = true;
			vm.OcultarLimparPessoa = true;
			vm.IsVisualizar = true;

			vm.IsConjuge = isConjuge;
			vm.TipoCadastro = vm.Pessoa.Tipo;

			CarregaCampos(vm);
			PreencheSalvarVM(vm);

			vm.UrlAcao = Url.Action("Visualizar", "Pessoa");

			return PartialView("PessoaModal", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PessoaVisualizar })]
		public ActionResult PessoaInternoModalVisualizar(int id)
		{
			PessoaVM vm = new PessoaVM();

			vm.Pessoa = _bus.ObterPessoa(interno: id);

			vm.CpfCnpjValido = true;
			vm.IsVisualizar = true;

			CarregaCampos(vm);
			PreencheSalvarVM(vm);

			return PartialView("PessoaModal", vm);
		}

		#endregion

		#region Excluir

		[Permite(RoleArray = new Object[] { ePermissao.PessoaExcluir })]
		public ActionResult ExcluirConfirm(int id)
		{
			ConfirmarVM vm = new ConfirmarVM();

			Pessoa pessoa = _bus.Obter(id);
			vm.Id = id;
			vm.Mensagem = Mensagem.Pessoa.MensagemExcluir(pessoa.NomeRazaoSocial);
			vm.Titulo = "Excluir Pessoa";
			return PartialView("Confirmar", vm);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PessoaExcluir })]
		public ActionResult Excluir(int id)
		{
			_bus.Excluir(id);
			return Json(new { @EhValido = Validacao.EhValido, @Msg = Validacao.Erros }, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Auxiliares

		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar, ePermissao.PessoaEditar, ePermissao.PessoaVisualizar, ePermissao.PessoaAssociar })]
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

		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar, ePermissao.PessoaEditar, ePermissao.PessoaVisualizar, ePermissao.PessoaAssociar })]
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

		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar, ePermissao.PessoaEditar, ePermissao.PessoaVisualizar, ePermissao.PessoaAssociar })]
		public PessoaVM CarregaCampos(PessoaVM vm)
		{
			if (vm.Pessoa.MeiosContatos == null || vm.Pessoa.MeiosContatos.Count == 0)
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

		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar, ePermissao.PessoaEditar, ePermissao.PessoaVisualizar, ePermissao.PessoaAssociar })]
		public Pessoa PreencheSalvarVM(PessoaVM vm)
		{
			vm.EstadosCivis = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.EstadosCivil, true);
			vm.Sexos = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.Sexos, true);
			vm.Profissoes = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.Profissoes, true);
			vm.OrgaoClasses = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.OrgaosClasse, true);
			vm.Estados = ViewModelHelper.CriarSelectList(ListaCredenciadoBus.Estados, true);

			if (vm.Pessoa != null || vm.Pessoa.Endereco.EstadoId > 0)
			{
				vm.Municipios = (List<SelectListItem>)(from m in ViewModelHelper.CriarSelectList(ListaCredenciadoBus.Municipios(vm.Pessoa.Endereco.EstadoId), true) orderby m.Text ascending select m).ToList();
			}
			else
			{
				vm.Municipios = new List<SelectListItem>() { ViewModelHelper.SelecionePadrao };
			}

			if (vm.Pessoa.Fisica != null && vm.Pessoa.Fisica.Profissao != null)
			{
				if (vm.Pessoa.Fisica.Profissao.Id > 0)
				{
					vm.Pessoa.Fisica.Profissao.ProfissaoTexto = _bus.ObterProfissaoTexto(vm.Pessoa.Fisica.Profissao.Id);
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

		#region Modal

		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar })]
		public ActionResult PessoaModal(string cpfCnpj, int? tipoPessoa, int tipoCadastro = 0)
		{
			PessoaVM vm = new PessoaVM(ListaCredenciadoBus.EstadosCivil, ListaCredenciadoBus.Sexos, ListaCredenciadoBus.Profissoes, ListaCredenciadoBus.OrgaosClasse, ListaCredenciadoBus.Estados);
			vm.ExibirMensagensPartial = true;

			vm.TipoCadastro = tipoCadastro;
			if (vm.TipoCadastro != 0)
			{
				vm.Pessoa.Tipo = (vm.TipoCadastro == 2) ? PessoaTipo.JURIDICA : PessoaTipo.FISICA;
			}

			return PessoaModal(vm, cpfCnpj, tipoPessoa);
		}

		[Permite(RoleArray = new Object[] { ePermissao.PessoaCriar })]
		private ActionResult PessoaModal(PessoaVM vm, string cpfCnpj = "", int? tipoPessoa = null)
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
		public ActionResult PessoaInline(int? id, string cpfCnpj = "", int? tipoPessoa = null, int internoId = 0)
		{
			PessoaVM vm = new PessoaVM(ListaCredenciadoBus.EstadosCivil, ListaCredenciadoBus.Sexos, ListaCredenciadoBus.Profissoes, ListaCredenciadoBus.OrgaosClasse, ListaCredenciadoBus.Estados);

			if ((id ?? 0) != 0)
			{
				vm.CpfCnpjValido = true;
				vm.IsVisualizar = true;
				vm.Pessoa = _bus.Obter(id.Value);
				CarregaCampos(vm);
				PreencheSalvarVM(vm);
				return PartialView(vm);
			}
			else if (internoId > 0)
			{
				vm.CpfCnpjValido = true;
				vm.IsVisualizar = true;
				vm.Pessoa = _bus.ObterPessoa(interno: internoId);
				CarregaCampos(vm);
				PreencheSalvarVM(vm);
				return PartialView(vm);
			}

			vm.ExibirMensagensPartial = false;
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
	}
}