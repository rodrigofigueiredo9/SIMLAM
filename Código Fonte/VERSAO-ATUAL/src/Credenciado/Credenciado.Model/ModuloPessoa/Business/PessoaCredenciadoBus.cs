using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business
{
	public class PessoaCredenciadoBus
	{
		#region Propriedades

		PessoaCredenciadoValidar _validar = null;
		PessoaCredenciadoDa _da = null;
		GerenciadorConfiguracao<ConfiguracaoPessoa> _configPessoa;
		GerenciadorConfiguracao<ConfiguracaoEndereco> _configEndereco;
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		PessoaInternoBus _busInterno;

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Ações de DML

		public PessoaCredenciadoBus() : this(new PessoaCredenciadoValidar()) { }

		public PessoaCredenciadoBus(PessoaCredenciadoValidar pessoaValidar)
		{
			_configPessoa = new GerenciadorConfiguracao<ConfiguracaoPessoa>(new ConfiguracaoPessoa());
			_configEndereco = new GerenciadorConfiguracao<ConfiguracaoEndereco>(new ConfiguracaoEndereco());
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_validar = pessoaValidar;
			_da = new PessoaCredenciadoDa(UsuarioCredenciado);
			_busInterno = new PessoaInternoBus();
		}

		public void SalvarPublico(Pessoa pessoa, BancoDeDados banco = null, Executor executor = null)
		{
			#region Conjuge

			if (pessoa.Id > 0 && pessoa.IsFisica)
			{
				Pessoa pessoaBanco = _da.Obter(pessoa.Id);

				//Remover conjuge anterior
				if ((pessoaBanco.Fisica.ConjugeId ?? 0) > 0 && (pessoaBanco.Fisica.ConjugeId ?? 0) != (pessoa.Fisica.ConjugeId ?? 0))
				{
					//Volta estado Civil anterior [Default - 1 Solteiro(a)]
					int estadoCivil = _da.ObterEstadoCivilAnterior(pessoaBanco.Fisica.ConjugeId.GetValueOrDefault()) ?? 1;
					_da.AlterarEstadoCivil(pessoaBanco.Fisica.ConjugeId.GetValueOrDefault(), estadoCivil, banco, executor);
				}
			}

			//Alterar o estado civil do conjuge
			if (pessoa.IsFisica && (pessoa.Fisica.ConjugeId ?? 0) > 0)
			{
				_da.AlterarEstadoCivil(pessoa.Fisica.ConjugeId.GetValueOrDefault(), pessoa.Fisica.EstadoCivil.GetValueOrDefault(), banco, executor);
			}

			#endregion

			pessoa.Ativa = 1;
			_da.Salvar(pessoa, banco, executor);
		}

		public bool Salvar(Pessoa pessoa, BancoDeDados banco = null, bool isConjuge = false, bool isAtividadeDeCorte = false)
		{
			try
			{
				pessoa.Ativa = 1;
				pessoa.CredenciadoId = User.FuncionarioId;

				#region Configurar Copiar dados

				#region Fisica

				if (pessoa.IsFisica)
				{
					if (pessoa.IsCopiado && pessoa.Fisica.ConjugeId.GetValueOrDefault() <= 0)
					{
						pessoa.Fisica.Conjuge = _busInterno.Obter(pessoa.Fisica.ConjugeInternoId.GetValueOrDefault());
						pessoa.Fisica.Conjuge.InternoId = pessoa.Fisica.ConjugeId;

						pessoa.Fisica.Conjuge.Id = Obter(pessoa.Fisica.ConjugeCPF, simplificado:true, credenciadoId: User.FuncionarioId).Id;
						pessoa.Fisica.ConjugeId = pessoa.Fisica.Conjuge.Id;
						pessoa.Fisica.Conjuge.Fisica.ConjugeId = 0;
						pessoa.Fisica.Conjuge.IsCopiado = true;
					}
				}

				#endregion

				#region Juridica

				if (pessoa.IsJuridica)
				{
					Pessoa representante = null;

					for (int i = 0; i < pessoa.Juridica.Representantes.Count; i++)
					{
						representante = pessoa.Juridica.Representantes[i];

						if (representante.IsCopiado)
						{
							/*Pessoa*/
							representante = _busInterno.Obter(representante.InternoId.GetValueOrDefault());
							representante.InternoId = representante.Id;
							representante.Id = Obter(representante.CPFCNPJ, simplificado:true, credenciadoId: User.FuncionarioId).Id;
							representante.CredenciadoId = User.FuncionarioId;
							representante.IsCopiado = true;

							if (representante.Fisica.EstadoCivil.GetValueOrDefault() == (int)ePessoaEstadoCivil.Casado ||
								representante.Fisica.EstadoCivil.GetValueOrDefault() == (int)ePessoaEstadoCivil.Amasiado)
							{
								/*Conjuge*/
								Pessoa conjuge = Obter(representante.Fisica.ConjugeCPF, credenciadoId: User.FuncionarioId);
								if (conjuge.Id > 0)
								{
									representante.Fisica.Conjuge = conjuge;
									representante.Fisica.ConjugeCPF = representante.Fisica.Conjuge.CPFCNPJ;
									representante.Fisica.ConjugeId = representante.Fisica.Conjuge.Id;
									pessoa.Juridica.Representantes[i] = representante;
									continue;
								}

								representante.Fisica.Conjuge = _busInterno.Obter(representante.Fisica.ConjugeId.GetValueOrDefault());
								representante.Fisica.Conjuge.InternoId = representante.Fisica.ConjugeId;

								representante.Fisica.Conjuge.Id = conjuge.Id;
								representante.Fisica.ConjugeId = representante.Fisica.Conjuge.Id;
								representante.Fisica.Conjuge.Fisica.ConjugeId = 0;
								representante.Fisica.Conjuge.IsCopiado = true;
							}

							pessoa.Juridica.Representantes[i] = representante;
						}
					}
				}

				#endregion

				#endregion
				if (!isAtividadeDeCorte)
					_validar.Salvar(pessoa);

				if (Validacao.EhValido)
				{
					GerenciadorTransacao.GerarNovoID();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
					{
						_validar.BancoValidar = bancoDeDados;
						bancoDeDados.IniciarTransacao();

						Pessoa aux;

						if (pessoa.IsJuridica)
						{
							#region Juridica

							foreach (Pessoa representante in pessoa.Juridica.Representantes)
							{
								if (representante.IsCopiado)
								{
									representante.CredenciadoId = User.FuncionarioId;

									if (representante.Fisica.Conjuge != null && representante.Fisica.Conjuge.IsCopiado)
									{
										representante.Fisica.Conjuge.IsValidarConjuge = !string.IsNullOrWhiteSpace(representante.Fisica.Conjuge.CPFCNPJ);
										if (representante.Fisica.Conjuge.IsValidarConjuge || !isAtividadeDeCorte)
										{
											if (!_validar.Salvar(representante.Fisica.Conjuge, true))
											{
												bancoDeDados.Rollback();
												return false;
											}

											representante.Fisica.Conjuge.CredenciadoId = User.FuncionarioId;

											_da.Salvar(representante.Fisica.Conjuge, bancoDeDados);

											representante.Fisica.ConjugeId = representante.Fisica.Conjuge.Id;
										}
									}

									if (!_validar.Salvar(representante))
									{
										bancoDeDados.Rollback();
										return false;
									}

									_da.Salvar(representante, bancoDeDados);
									representante.Id = representante.Id;
									continue;
								}

								if (representante.Id <= 0)
								{
									aux = _busInterno.Obter(representante.InternoId.GetValueOrDefault());

									if (!string.IsNullOrEmpty(aux.Fisica.ConjugeCPF))
									{
										aux.Fisica.ConjugeId = _da.ExistePessoa(aux.Fisica.ConjugeCPF, pessoa.CredenciadoId.GetValueOrDefault());

										if (aux.Fisica.ConjugeId.GetValueOrDefault() <= 0)
										{
											Pessoa aux2 = _busInterno.Obter(aux.Fisica.ConjugeCPF);

											aux2.CredenciadoId = User.FuncionarioId;
											aux2.InternoId = aux.Id;

											aux2.Fisica.ConjugeId = 0;
											aux2.Fisica.ConjugeCPF = string.Empty;
											aux2.Id = 0;

											if (!_validar.Salvar(aux2, true))
											{
												bancoDeDados.Rollback();
												return false;
											}

											_da.Salvar(aux2, bancoDeDados);

											aux.Fisica.ConjugeId = aux2.Id;
										}
									}

									aux.CredenciadoId = User.FuncionarioId;
									aux.InternoId = aux.Id;
									aux.Id = 0;

									if (!_validar.Salvar(aux))
									{
										bancoDeDados.Rollback();
										return false;
									}

									_da.Salvar(aux, bancoDeDados);

									representante.Id = aux.Id;
								}
							}

							#endregion
						}
						else
						{
							#region Conjuge

							if (pessoa.IsValidarConjuge && (pessoa.Fisica.EstadoCivil.GetValueOrDefault() == (int)ePessoaEstadoCivil.Casado || 
								pessoa.Fisica.EstadoCivil.GetValueOrDefault() == (int)ePessoaEstadoCivil.Amasiado))
							{
								if (pessoa.Fisica.Conjuge != null && pessoa.Fisica.Conjuge.IsCopiado)
								{
									if (!_validar.Salvar(pessoa.Fisica.Conjuge, true))
									{
										bancoDeDados.Rollback();
										return false;
									}

									pessoa.Fisica.Conjuge.CredenciadoId = User.FuncionarioId;

									_da.Salvar(pessoa.Fisica.Conjuge, bancoDeDados);

									pessoa.Fisica.ConjugeId = pessoa.Fisica.Conjuge.Id;
								}

								if (pessoa.Fisica.ConjugeId <= 0 && !string.IsNullOrEmpty(pessoa.Fisica.ConjugeCPF))
								{
									pessoa.Fisica.ConjugeId = _da.ExistePessoa(pessoa.Fisica.ConjugeCPF, pessoa.CredenciadoId.GetValueOrDefault());

									if (pessoa.Fisica.ConjugeId.GetValueOrDefault() <= 0)
									{
										aux = _busInterno.Obter(pessoa.Fisica.ConjugeCPF);

										aux.CredenciadoId = User.FuncionarioId;
										aux.InternoId = aux.Id;

										aux.Fisica.ConjugeId = 0;
										aux.Fisica.ConjugeCPF = string.Empty;
										aux.Id = 0;

										if (!_validar.Salvar(aux, true))
										{
											bancoDeDados.Rollback();
											return false;
										}

										_da.Salvar(aux, bancoDeDados);

										pessoa.Fisica.ConjugeId = aux.Id;
									}
								}
							}

							#endregion
						}

						#region Conjuge

						if (pessoa.Id > 0 && pessoa.IsFisica)
						{
							Pessoa pessoaBanco = _da.Obter(pessoa.Id);

							//Remover conjuge anterior
							if ((pessoaBanco.Fisica.ConjugeId ?? 0) > 0 && (pessoaBanco.Fisica.ConjugeId ?? 0) != (pessoa.Fisica.ConjugeId ?? 0))
							{
								//Volta estado Civil anterior [Default - 1 Solteiro(a)]
								int estadoCivil = _da.ObterEstadoCivilAnterior(pessoaBanco.Fisica.ConjugeId.GetValueOrDefault()) ?? 1;
								_da.AlterarEstadoCivil(pessoaBanco.Fisica.ConjugeId.GetValueOrDefault(), estadoCivil, bancoDeDados);
							}
						}

						//Alterar o estado civil do conjuge
						if (pessoa.IsFisica && (pessoa.Fisica.ConjugeId ?? 0) > 0)
						{
							_da.AlterarEstadoCivil(pessoa.Fisica.ConjugeId.GetValueOrDefault(), pessoa.Fisica.EstadoCivil.GetValueOrDefault(), bancoDeDados);
						}

						#endregion

						_da.Salvar(pessoa, bancoDeDados);

						if (!Validacao.EhValido)
						{
							bancoDeDados.Rollback();
							return false;
						}

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return Validacao.EhValido;
		}

		public Boolean Excluir(int id)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				if (_validar.VerificarExcluirPessoa(id))
				{
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						_da.Excluir(id);

						Validacao.Add(Mensagem.Pessoa.Excluir);
						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public void SalvarConjuge(int id, int conjuge, BancoDeDados banco = null)
		{
			_da.SalvarConjuge(id, conjuge, banco);
		}

		public void AlterarEmail(Pessoa pessoa, BancoDeDados banco = null, Executor executor = null)
		{
			_da.AlterarEmail(pessoa, banco, executor);
		}

		//public void DesassociarConjuge(Pessoa pessoa)
		//{
		//	Pessoa pessoaBanco = Obter(pessoa.Id);

		//	if (pessoaBanco.Fisica.ConjugeId.GetValueOrDefault() != pessoa.Id)
		//	{
		//		int estadoCivil = _da.ObterEstadoCivilAnterior(conjugeBanco.Id) ?? 0;
		//		_da.AlterarEstadoCivil(conjugeBanco.Id, estadoCivil);
		//	}
		//}

		#endregion

		#region Obter/Filtrar

		public Resultados<Pessoa> Filtrar(ListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ListarFiltro> filtro = new Filtro<ListarFiltro>(filtrosListar, paginacao);
				Resultados<Pessoa> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<ProfissaoLst> FiltrarProfissao(ProfissaoFiltro filtrosListar, Paginacao paginacao)
		{
			return _busInterno.FiltrarProfissao(filtrosListar, paginacao);
		}

		public Pessoa Obter(String cpfCnpj, BancoDeDados banco = null, bool simplificado = false, int? credenciadoId = null)
		{
			Pessoa pessoa = _da.Obter(cpfCnpj, banco, simplificado, credenciadoId);

			if (!simplificado)
			{
				CarregarDadosListas(pessoa);
			}

			return pessoa;
		}

		public Pessoa Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			Pessoa pessoa = _da.Obter(id, banco, simplificado);

			if (!simplificado)
			{
				CarregarDadosListas(pessoa);
			}

			return pessoa;
		}

		public Pessoa ObterPessoa(string cpfCnpj = null, int interno = 0)
		{
			Pessoa pessoa;
			if (interno > 0)
			{
				pessoa = _busInterno.Obter(interno);
				ConfigurarPessoa(pessoa);
			}
			else
			{
				pessoa = Obter(cpfCnpj, credenciadoId: User.FuncionarioId);
				if (pessoa.Id > 0)
				{
					return pessoa;
				}

				pessoa = _busInterno.Obter(cpfCnpj);
				ConfigurarPessoa(pessoa);
			}

			pessoa.InternoId = pessoa.Id;
			pessoa.Id = 0;
			return pessoa;
		}

		public String ObterProfissaoTexto(int id)
		{
			return _da.ObterProfissaoTexto(id);
		}

		#endregion

		#region Validar

		public bool Existe(int id)
		{
			try
			{
				return _da.ExistePessoa(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public bool VerificarCriarCpfCnpj(Pessoa pessoa)
		{
			if (pessoa.IsFisica)
			{
				_validar.VerificarCriarCpf(pessoa);
			}
			else
			{
				_validar.VerificarCriarCnpj(pessoa);
			}

			return Validacao.EhValido;
		}

		public bool ExisteProfissao(int pessoa, BancoDeDados banco = null)
		{
			return _da.ExisteProfissao(pessoa, banco);
		}
		public bool ExisteEndereco(int pessoa, BancoDeDados banco = null)
		{
			return _da.ExisteEndereco(pessoa, banco);
		}

		public bool ValidarAssociarRepresentante(int pessoaId)
		{
			return _validar.ValidarAssociarRepresentante(pessoaId);
		}

		internal bool ValidarAssociarResponsavelTecnico(int id)
		{
			return _validar.ValidarAssociarResponsavelTecnico(id);
		}

		#endregion

		#region Auxiliares

		public void ConfigurarPessoa(Pessoa pessoa)
		{
			Pessoa aux;

			if (pessoa.IsJuridica)
			{
				foreach (Pessoa representante in pessoa.Juridica.Representantes)
				{
					aux = Obter(representante.CPFCNPJ, simplificado: true, credenciadoId: User.FuncionarioId);

					if (aux.Id > 0)
					{
						representante.Id = aux.Id;
						representante.InternoId = aux.InternoId;
						representante.Fisica.Nome = aux.NomeRazaoSocial;
					}
					else
					{
						representante.InternoId = representante.Id;
						representante.Id = 0;
					}
				}
			}
			else if (!string.IsNullOrEmpty(pessoa.Fisica.ConjugeCPF))
			{
				aux = Obter(pessoa.Fisica.ConjugeCPF, simplificado: true, credenciadoId: User.FuncionarioId);

				if (aux.Id > 0)
				{
					pessoa.Fisica.ConjugeInternoId = pessoa.Fisica.ConjugeId;
					pessoa.Fisica.ConjugeId = aux.Id;
					pessoa.Fisica.ConjugeNome = aux.NomeRazaoSocial;
				}
				else
				{
					pessoa.Fisica.ConjugeInternoId = pessoa.Fisica.ConjugeId;
					pessoa.Fisica.ConjugeId = 0;
				}
			}
		}

		void CarregarDadosListas(Pessoa pessoa)
		{
			if (pessoa.Id > 0)
			{
				var listaContatos = ListaCredenciadoBus.ListarMeiosContato;
				ContatoLst contatoAux;

				pessoa.MeiosContatos.ForEach(x =>
				{
					contatoAux = listaContatos.SingleOrDefault(y => y.Id == (int)x.TipoContato) ?? new ContatoLst();
					x.Mascara = contatoAux.Mascara;
					x.TipoTexto = contatoAux.Texto;
				});

				if (pessoa.Endereco.EstadoId > 0)
				{
					pessoa.Endereco.EstadoTexto = (_configEndereco.Obter<List<Estado>>(ConfiguracaoEndereco.KeyEstados).SingleOrDefault(x => x.Id == pessoa.Endereco.EstadoId) ?? new Estado()).Sigla;
					pessoa.Endereco.MunicipioTexto = (_configEndereco.Obter<Dictionary<int, List<Municipio>>>(ConfiguracaoEndereco.KeyMunicipios)[pessoa.Endereco.EstadoId].SingleOrDefault(x => x.Id == pessoa.Endereco.MunicipioId) ?? new Municipio()).Texto;
				}

				if (pessoa.IsFisica)
				{
					pessoa.Fisica.EstadoCivilTexto = (_configPessoa.Obter<List<EstadoCivil>>(ConfiguracaoPessoa.KeyEstadosCivis).SingleOrDefault(x => x.Id == pessoa.Fisica.EstadoCivil) ?? new EstadoCivil()).Texto;
					pessoa.Fisica.SexoTexto = (_configPessoa.Obter<List<Sexo>>(ConfiguracaoPessoa.KeySexos).SingleOrDefault(x => x.Id == pessoa.Fisica.Sexo) ?? new Sexo()).Texto;
					pessoa.Fisica.Profissao.ProfissaoTexto = (_configPessoa.Obter<List<ProfissaoLst>>(ConfiguracaoPessoa.KeyProfissoes).SingleOrDefault(x => x.Id == pessoa.Fisica.Profissao.Id) ?? new ProfissaoLst()).Texto;
					pessoa.Fisica.Profissao.OrgaoClasseTexto = (_configPessoa.Obter<List<OrgaoClasse>>(ConfiguracaoPessoa.KeyOrgaoClasses).SingleOrDefault(x => x.Id == pessoa.Fisica.Profissao.OrgaoClasseId) ?? new OrgaoClasse()).Texto;
				}
			}
		}

		#endregion
	}
}