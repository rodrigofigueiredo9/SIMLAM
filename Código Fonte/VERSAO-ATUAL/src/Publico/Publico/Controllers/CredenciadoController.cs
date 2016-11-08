using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloOrgaoParceiroConveniado.Bussiness;
using Tecnomapas.EtramiteX.Publico.ViewModels;
using Tecnomapas.EtramiteX.Publico.ViewModels.VMPessoa;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class CredenciadoController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		CredenciadoBus _bus = new CredenciadoBus(new CredenciadoValidar());
		OrgaoParceiroConveniadoBus _busOrgaoParceiro = new OrgaoParceiroConveniadoBus();

		#endregion

		#region Criar

		public ActionResult CriarVerificarCpfCnpj(PessoaVM vm)
		{
			bool isCpfCnpjValido = false;
			String urlAcao = Url.Action("Criar", "Credenciado");

			try
			{
				isCpfCnpjValido = _bus.ValidarPessoaCpfCnpj(vm.Pessoa);

				if (isCpfCnpjValido)
				{
					string tipoDocumento = vm.Pessoa.IsFisica ? "CPF" : "CNPJ";
					vm.ExisteCredenciado = _bus.ExisteCredenciado(vm.Pessoa.CPFCNPJ);

					if (!vm.ExisteCredenciado)
					{
						vm.ExisteInterno = _bus.ExisteInterno(vm.Pessoa.CPFCNPJ);

						if (vm.ExisteInterno)
						{
							Validacao.Add(Mensagem.Credenciado.PessoaExistenteInterno(tipoDocumento));
						}
					}
					else
					{
						if (_bus.IsCredenciadoOrgaoParceiroPublico(vm.Pessoa.CPFCNPJ)) 
						{
							Validacao.Add(Mensagem.Credenciado.AtualizarEmail(tipoDocumento));
							return Json(new
							{
								IsRedirecionar = true,
								UrlAcao = Url.Action("Reenviar", "Credenciado", Validacao.QueryParamSerializer(new { cpfCnpj = vm.Pessoa.CPFCNPJ })),
								Msg = Validacao.Erros
							}, JsonRequestBehavior.AllowGet);
						}

						if (!_bus.IsBloqueado(vm.Pessoa.CPFCNPJ))
						{
							if (!_bus.IsCredenciadoAtivo(vm.Pessoa.CPFCNPJ))
							{
								Validacao.Add(Mensagem.Credenciado.ReenviarChave(tipoDocumento));
								return Json(new
								{
									IsRedirecionar = true,
									UrlAcao = Url.Action("Reenviar", "Credenciado", Validacao.QueryParamSerializer(new { cpfCnpj = vm.Pessoa.CPFCNPJ })),
									Msg = Validacao.Erros
								}, JsonRequestBehavior.AllowGet);
							}
							else
							{
								isCpfCnpjValido = false;
								Validacao.Add(Mensagem.Credenciado.ExistenteAtivado(tipoDocumento));
							}
						}
						else 
						{
							isCpfCnpjValido = false;
							Validacao.Add(Mensagem.Credenciado.CredenciadoBloqueado(tipoDocumento));
						}
					}
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

			vm.CpfCnpjValido = !String.IsNullOrEmpty(cpfCnpj);

			if (vm.CpfCnpjValido)
			{
				vm.Pessoa.Tipo = tipoPessoa.GetValueOrDefault();
				if (vm.Pessoa.Tipo == PessoaTipo.FISICA)
				{
					vm.Pessoa.Fisica.CPF = cpfCnpj;
					vm.ProfissaoObrigatoria = true;
					vm.NomePaiObrigatorio = true;
					vm.NomeMaeObrigatorio = true;

					vm.OrgaosParceiros = ViewModelHelper.CriarSelectList(_busOrgaoParceiro.ObterOrgaosParceirosLst(), true);
					vm.OrgaosParceirosUnidades = ViewModelHelper.CriarSelectList(new List<Lista>(), true);
				}
				else
				{
					vm.Pessoa.Juridica.CNPJ = cpfCnpj;
				}

				if (existeInterno)
				{
					vm.Pessoa = _bus.ObterPessoaInterno(vm.Pessoa.CPFCNPJ);
					vm.Pessoa.InternoId = vm.Pessoa.Id;
					vm.Pessoa.Id = 0;

					if (vm.Pessoa.IsJuridica)
					{
						int internoId = 0;
						for (int i = 0; i < vm.Pessoa.Juridica.Representantes.Count; i++)
						{
							internoId = vm.Pessoa.Juridica.Representantes[i].Id;
							vm.Pessoa.Juridica.Representantes[i] = _bus.ObterPessoaInterno(vm.Pessoa.Juridica.Representantes[i].CPFCNPJ);
							vm.Pessoa.Juridica.Representantes[i].InternoId = internoId;
							vm.Pessoa.Juridica.Representantes[i].Id = 0;

							if (vm.Pessoa.Juridica.Representantes[i].Fisica.ConjugeId > 0)
							{
								vm.Pessoa.Juridica.Representantes[i].Fisica.Conjuge = _bus.ObterPessoaInterno(vm.Pessoa.Juridica.Representantes[i].Fisica.ConjugeId.Value);
								vm.Pessoa.Juridica.Representantes[i].Fisica.Conjuge.InternoId = vm.Pessoa.Juridica.Representantes[i].Fisica.ConjugeId;
								vm.Pessoa.Juridica.Representantes[i].Fisica.Conjuge.Fisica.ConjugeId = 0;
								vm.Pessoa.Juridica.Representantes[i].Fisica.Conjuge.Id = 0;
								vm.Pessoa.Juridica.Representantes[i].Fisica.ConjugeId = 0;
							}
						}
					}
					else if (vm.Pessoa.Fisica.ConjugeId > 0)
					{
						vm.Pessoa.Fisica.Conjuge = _bus.ObterPessoaInterno(vm.Pessoa.Fisica.ConjugeId.Value);
						vm.Pessoa.Fisica.Conjuge.InternoId = vm.Pessoa.Fisica.ConjugeId;
						vm.Pessoa.Fisica.Conjuge.Fisica.ConjugeId = 0;
						vm.Pessoa.Fisica.Conjuge.Id = 0;
						vm.Pessoa.Fisica.ConjugeId = 0;
					}
				}

				CarregaCampos(vm);
				vm.CarregarMunicipios();
			}

			if (Request.IsAjaxRequest())
			{
				return PartialView("CredenciadoPartial", vm);
			}
			else
			{
				return View(vm);
			}
		}

		[HttpPost]
		public ActionResult Criar(PessoaVM vm)
		{
			string urlRedireciona = Url.Action("Criar", "Credenciado");
			vm.CpfCnpjValido = true;
			vm.Pessoa.Ativa = 1; //Indica que pessoa está ativa no sistema
			vm.Pessoa.Id = 0;
			vm.Credenciado.Id = 0;

			if (vm.TipoCadastro != 0)
			{
				vm.Pessoa.Tipo = (vm.TipoCadastro == 2) ? PessoaTipo.JURIDICA : PessoaTipo.FISICA;
			}

			CarregaCampos(vm);

			if (vm.TipoCadastro == 1 && (vm.Pessoa.Fisica.EstadoCivil == 2 || vm.Pessoa.Fisica.EstadoCivil == 5))
			{
				vm.Pessoa.Fisica.ConjugeId = -1;
			}

			if (_bus.SalvarPublico(vm.Credenciado))
			{
				if (vm.Credenciado.Tipo == (int)eCredenciadoTipo.OrgaoParceiroConveniado)
				{
					Validacao.Add(Mensagem.Credenciado.SalvarOrgaoParceiroConveniado);
				}
				else
				{
					Validacao.Add(Mensagem.Credenciado.SalvarPublico(vm.Credenciado.Email));
				} 
				urlRedireciona += "?Msg=" + Validacao.QueryParam();
			}

			return Json(new { IsPessoaSalva = Validacao.EhValido, UrlRedireciona = urlRedireciona, @Pessoa = vm.Pessoa, Msg = Validacao.Erros });
		}

		#endregion

		#region Reenviar Email

		public ActionResult Reenviar(string cpfCnpj)
		{
			PessoaVM vm = new PessoaVM();
			vm.Credenciado = _bus.Obter(cpfCnpj);

			return View(vm);
		}

		[HttpPost]
		public ActionResult Reenviar(string cpfCnpj, string email)
		{
			CredenciadoPessoa credenciado = _bus.Obter(cpfCnpj);
			credenciado.Pessoa.MeiosContatos.Single(x => x.TipoContato == eTipoContato.Email).Valor = email;

			if (_bus.IsCredenciadoAtivo(cpfCnpj))
			{
				Validacao.Add(Mensagem.Credenciado.ExistenteAtivado(cpfCnpj));
			}

			if (_bus.IsBloqueado(cpfCnpj))
			{
				Validacao.Add(Mensagem.Credenciado.ExistenteBloqueado);
			}

			if (Validacao.Erros.Count <= 0 && _bus.AlterarEmail(credenciado))
			{
				Validacao.Add(Mensagem.Credenciado.ReenviarChaveSucesso(credenciado.Email));
			}

			return Json(new { @EhValido = Validacao.EhValido, UrlRedireciona = Url.Action("Criar", "Credenciado", Validacao.QueryParamSerializer()), Msg = Validacao.Erros });
		}

		#endregion

		#region Atualizar Dados

		public ActionResult AtualizarDados(string cpfCnpj, string email)
		{
			CredenciadoPessoa credenciado = _bus.Obter(cpfCnpj);
			credenciado.Pessoa.MeiosContatos.Single(x => x.TipoContato == eTipoContato.Email).Valor = email;

			if (_bus.IsCredenciadoAtivo(cpfCnpj))
			{
				Validacao.Add(Mensagem.Credenciado.ExistenteAtivado(cpfCnpj));
			}

			if (_bus.IsBloqueado(cpfCnpj))
			{
				Validacao.Add(Mensagem.Credenciado.ExistenteBloqueado);
			}

			if (Validacao.Erros.Count <= 0 && _bus.AlterarEmail(credenciado))
			{
				Validacao.Add(Mensagem.Credenciado.ReenviarChaveSucessoOrgaoParceiro);
			}

			return Json(new { @EhValido = Validacao.EhValido, UrlRedireciona = Url.Action("Criar", "Credenciado", Validacao.QueryParamSerializer()), Msg = Validacao.Erros });
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

		#endregion
	}
}