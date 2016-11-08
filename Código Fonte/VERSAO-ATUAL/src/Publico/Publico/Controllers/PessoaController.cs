using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloOrgaoParceiroConveniado.Bussiness;
//using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Publico.ViewModels;
using Tecnomapas.EtramiteX.Publico.ViewModels.VMPessoa;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class PessoaController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
        PessoaBus _bus = new PessoaBus();
		OrgaoParceiroConveniadoBus _busOrgaoParceiro = new OrgaoParceiroConveniadoBus();
		Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business.PessoaValidar _validar = new Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business.PessoaValidar();
		PessoaCredenciadoValidar _pessoaCredenciadoValidar = new PessoaCredenciadoValidar();
		CredenciadoBus _credenciadoBus = new CredenciadoBus(new CredenciadoValidar());

		#endregion

		#region Criar

		public ActionResult CriarVerificarCpfCnpj(PessoaVM vm)
		{
			bool isCpfCnpjValido = false;
			String urlAcao = Url.Action("Criar", "Pessoa");

			try
			{
				isCpfCnpjValido = _credenciadoBus.ValidarPessoaCpfCnpj(vm.Pessoa);

				if (isCpfCnpjValido)
				{
					string tipoDocumento = vm.Pessoa.IsFisica ? "CPF" : "CNPJ";
					vm.ExisteInterno = _credenciadoBus.ExisteInterno(vm.Pessoa.CPFCNPJ);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Json(new
			{
				IsCpfCnpjValido = isCpfCnpjValido,
				PessoaId = 0,
				Parametros = new { cpfCnpj = vm.Pessoa.CPFCNPJ, tipoPessoa = vm.Pessoa.Tipo, tipoCadastro = vm.TipoCadastro, existeCredenciado = vm.ExisteCredenciado, existeInterno = vm.ExisteInterno },
				UrlAcao = urlAcao,
				Msg = Validacao.Erros
			}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Criar(string cpfCnpj, int? tipoPessoa, int tipoCadastro = 0, bool existeCredenciado = false, bool existeInterno = false)
		{
			PessoaVM vm = new PessoaVM(_busLista.EstadosCivil, _busLista.Sexos, _busLista.Profissoes, _busLista.OrgaosClasse, _busLista.Estados);
			vm.ExibirMensagensPartial = true;
			vm.ExibirBotoes = true;
			vm.UrlAcao = Url.Action("Criar", "Pessoa");
			vm.TipoCadastro = tipoCadastro;

			vm.CpfCnpjValido = !String.IsNullOrEmpty(cpfCnpj);

			if (vm.CpfCnpjValido)
			{
				vm.Pessoa.Tipo = tipoPessoa.GetValueOrDefault();
				vm.Pessoa.Fisica.CPF = cpfCnpj;

				if (existeCredenciado)
				{
					vm.Pessoa = _credenciadoBus.ObterPessoaCredenciado(vm.Pessoa.CPFCNPJ);

					if (vm.Pessoa.Fisica.ConjugeId > 0)
					{
						vm.Pessoa.Fisica.Conjuge = _credenciadoBus.ObterPessoaCredenciado(vm.Pessoa.Fisica.ConjugeId.Value);
						vm.Pessoa.Fisica.Conjuge.Id = 0;
					}
				}
				else if (existeInterno)
				{
					vm.Pessoa = _credenciadoBus.ObterPessoaInterno(vm.Pessoa.CPFCNPJ);
					vm.Pessoa.InternoId = vm.Pessoa.Id;

					if (vm.Pessoa.Fisica.ConjugeId > 0)
					{
						vm.Pessoa.Fisica.Conjuge = _credenciadoBus.ObterPessoaInterno(vm.Pessoa.Fisica.ConjugeId.Value);
						vm.Pessoa.Fisica.Conjuge.InternoId = vm.Pessoa.Fisica.ConjugeId;
						vm.Pessoa.Fisica.Conjuge.Fisica.ConjugeId = 0;
						vm.Pessoa.Fisica.Conjuge.Id = 0;
						vm.Pessoa.Fisica.ConjugeId = 0;
					}
				}

				if (!existeInterno) 
				{
					vm.Pessoa.InternoId = -1;
				}

				vm.Pessoa.Id = 0;
				CarregaCampos(vm);
				vm.CarregarMunicipios();
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
		public ActionResult Criar(PessoaVM vm)
		{
			vm.CpfCnpjValido = true;
			vm.Pessoa.Ativa = 1; //Indica que pessoa está ativa no sistema


			if (vm.TipoCadastro != 0)
			{
				vm.Pessoa.Tipo = (vm.TipoCadastro == 2) ? PessoaTipo.JURIDICA : PessoaTipo.FISICA;
			}

			CarregaCampos(vm);

			if (vm.TipoCadastro == 1 && (vm.Pessoa.Fisica.EstadoCivil == 2 || vm.Pessoa.Fisica.EstadoCivil == 5))
			{
				vm.Pessoa.Fisica.ConjugeId = -1;
			}

			if (vm.Pessoa.IsFisica)
			{
				if (vm.Pessoa.Fisica.Conjuge != null && !String.IsNullOrWhiteSpace(vm.Pessoa.Fisica.Conjuge.CPFCNPJ))
				{
					_pessoaCredenciadoValidar.Salvar(vm.Pessoa.Fisica.Conjuge, true);
				}
			}

			_pessoaCredenciadoValidar.Salvar(vm.Pessoa);

			return Json(new { IsPessoaSalva = Validacao.EhValido, @Pessoa = vm.Pessoa, Msg = Validacao.Erros });
		}

		#endregion

		#region Editar

		public ActionResult Editar(PessoaVM vm)
		{
			Pessoa pessoa = vm.Pessoa;
			vm = new PessoaVM(_busLista.EstadosCivil, _busLista.Sexos, _busLista.Profissoes, _busLista.OrgaosClasse, _busLista.Estados, vm.IsConjuge);
			vm.CpfCnpjValido = true;
			vm.Pessoa = pessoa;

			CarregaCampos(vm);
			PreencheSalvarVM(vm);

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
		public ActionResult SalvarConjuge(PessoaVM vm)
		{
			CarregaCampos(vm);
			vm.Pessoa.IsValidarConjuge = false;
			vm.Pessoa.Fisica.ConjugeId = 0;
			vm.Pessoa.IsCopiado = true;
			vm.IsConjuge = true;

			_pessoaCredenciadoValidar.Salvar(vm.Pessoa);

			return Json(new { IsValido = Validacao.EhValido, @Pessoa = vm.Pessoa, Msg = Validacao.Erros });

		}


		#endregion

		#region Visualizar

		public ActionResult Visualizar(int id)
		{
			PessoaVM vm = new PessoaVM(_busLista.EstadosCivil, _busLista.Sexos, _busLista.Profissoes, _busLista.OrgaosClasse, _busLista.Estados);

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

		public ActionResult PessoaModalVisualizarConjuge(Pessoa pessoa)
		{
			PessoaVM vm = new PessoaVM();
			vm.Pessoa = pessoa;

			vm.CpfCnpjValido = true;
			vm.ExibirMensagensPartial = true;
			vm.OcultarLimparPessoa = true;
			vm.IsVisualizar = true;
			vm.ExibirBotoes = true;

			vm.IsConjuge = true;
			vm.TipoCadastro = vm.Pessoa.Tipo;

			CarregaCampos(vm);
			PreencheSalvarVM(vm);

			vm.UrlAcao = Url.Action("Visualizar", "Pessoa");

			return PartialView("PessoaModal", vm);

		}

		public ActionResult PessoaModalVisualizar(int id, string cpf_cnpj = null)
		{
			PessoaVM vm = new PessoaVM();

			vm.CpfCnpjValido = true;
			vm.Pessoa = String.IsNullOrWhiteSpace(cpf_cnpj) ? _bus.Obter(id) : _bus.Obter(cpf_cnpj);
			CarregaCampos(vm);
			PreencheSalvarVM(vm);

			vm.UrlAcao = Url.Action("Visualizar", "Pessoa");
			vm.ExibirMensagensPartial = true;
			vm.ExibirBotoes = true;
			vm.ExibirLimparPessoa = false;
			vm.IsVisualizar = true;

			return PartialView("PessoaModal", vm);
		}

		#endregion

		#region Auxiliares

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

		public Pessoa PreencheSalvarVM(PessoaVM vm)
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

		public ActionResult ObterOrgaoParceiroUnidades(int orgaoId)
		{
			List<Lista> unidades = _busOrgaoParceiro.ObterUnidadesLst(orgaoId);

			return Json(unidades, JsonRequestBehavior.AllowGet);
		}

		#endregion

		#region Associar Pessoa

		public ActionResult PessoaModalVisualizarObjeto(Pessoa pessoa)
		{
			PessoaVM vm = new PessoaVM(_busLista.EstadosCivil, _busLista.Sexos, _busLista.Profissoes, _busLista.OrgaosClasse, _busLista.Estados);

			vm.CpfCnpjValido = true;
			vm.Pessoa = pessoa;
			CarregaCampos(vm);
			PreencheSalvarVM(vm);

			vm.UrlAcao = Url.Action("Visualizar", "Pessoa");
			vm.ExibirMensagensPartial = true;
			vm.ExibirBotoes = true;
			vm.ExibirLimparPessoa = false;
			vm.IsVisualizar = true;

			return PartialView("PessoaModal", vm);
		}

		public ActionResult PessoaModal(string cpfCnpj, int? tipoPessoa, bool isAssociarConjuge = false)
		{
			PessoaVM vm = new PessoaVM(_busLista.EstadosCivil, _busLista.Sexos, _busLista.Profissoes, _busLista.OrgaosClasse, _busLista.Estados);
			vm.ExibirMensagensPartial = true;
			vm.ExibirBotoes = true;
			vm.IsAssociarConjuge = isAssociarConjuge;
			vm.TipoCadastro = 1;

			return PessoaModal(vm, cpfCnpj, tipoPessoa);
		}

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

			CarregaCampos(vm);
			vm.CarregarMunicipios();

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
	}
}