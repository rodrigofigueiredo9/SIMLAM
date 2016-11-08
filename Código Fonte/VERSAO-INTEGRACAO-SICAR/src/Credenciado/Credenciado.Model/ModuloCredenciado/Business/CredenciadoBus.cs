using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Tecnomapas.Blocos.Autenticacao;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloEmail;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloEmail.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business
{
	public class CredenciadoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		GerenciadorConfiguracao<ConfiguracaoCredenciado> _config;
		GerenciadorConfiguracao<ConfiguracaoUsuario> _configUsuario;

		EmailBus _emailBus;
		PessoaInternoBus _pessoaInternoBus;
		PessoaCredenciadoBus _pessoaCredenciadoBus;
		UsuarioBus _busUsuario;
		CredenciadoValidar _validar;
		CredenciadoDa _da;
		PessoaCredenciadoValidar _validarPessoa;

		public String CredenciadoLinkAcesso
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyCredenciadoLinkAcesso); }
		}

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public List<QuantPaginacao> LstQuantPaginacao
		{
			get { return _configSys.Obter<List<QuantPaginacao>>(ConfiguracaoSistema.KeyLstQuantPaginacao); }
		}

		public List<Papel> PapeisCredenciado
		{
			get
			{
				if (GerenciadorCache.PapeisFuncionario == null)
				{
					GerenciadorCache.PapeisFuncionario = _da.ObterPapeis();
				}

				return GerenciadorCache.PapeisFuncionario as List<Papel>;
			}
		}

		public List<Situacao> CredenciadoSituacoes
		{
			get { return _config.Obter<List<Situacao>>(ConfiguracaoCredenciado.KeyCredenciadoSituacoes); }
		}

		#endregion

		public CredenciadoBus() : this(new CredenciadoValidar()) { }

		public CredenciadoBus(CredenciadoValidar credenciadoValidar)
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_config = new GerenciadorConfiguracao<ConfiguracaoCredenciado>(new ConfiguracaoCredenciado());
			_configUsuario = new GerenciadorConfiguracao<ConfiguracaoUsuario>(new ConfiguracaoUsuario());
			_validar = credenciadoValidar;
			_pessoaInternoBus = new PessoaInternoBus();
			_pessoaCredenciadoBus = new PessoaCredenciadoBus();
			_emailBus = new EmailBus(UsuarioInterno);
			_busUsuario = new UsuarioBus(HistoricoAplicacao.CREDENCIADO, UsuarioCredenciado);
			_da = new CredenciadoDa(UsuarioCredenciado);
			_validarPessoa = new PessoaCredenciadoValidar();
		}

		#region Ações de DML

		public bool SalvarPublico(CredenciadoPessoa credenciado)
		{
			try
			{
				if (credenciado.Pessoa.IsFisica)
				{
					if (credenciado.Pessoa.Fisica.Conjuge != null && !String.IsNullOrWhiteSpace(credenciado.Pessoa.Fisica.Conjuge.CPFCNPJ))
					{
						_validarPessoa.Salvar(credenciado.Pessoa.Fisica.Conjuge, true);
					}
				}

				_validar.Salvar(credenciado, true);

				if (Validacao.EhValido)
				{
					credenciado.Situacao = credenciado.Tipo == (int)eCredenciadoTipo.OrgaoParceiroConveniado ? (int)eCredenciadoSituacao.Cadastrado : (int)eCredenciadoSituacao.AguardandoAtivacao;

					Executor executor = new Executor();
					executor.Nome = credenciado.Nome;
					executor.Tipo = eExecutorTipo.Credenciado;

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						credenciado.Chave = GerarChaveAcesso(credenciado.Email, credenciado.Pessoa.NomeRazaoSocial);

						_da.Salvar(credenciado, bancoDeDados, executor);

						if (credenciado.Pessoa.IsJuridica)
						{
							foreach (Pessoa pessoa in credenciado.Pessoa.Juridica.Representantes)
							{
								if (pessoa.Fisica.Conjuge != null && !string.IsNullOrEmpty(pessoa.Fisica.Conjuge.CPFCNPJ))
								{
									pessoa.Fisica.Conjuge.CredenciadoId = credenciado.Id;
									_pessoaCredenciadoBus.SalvarPublico(pessoa.Fisica.Conjuge, bancoDeDados, executor);
									pessoa.Fisica.ConjugeId = pessoa.Fisica.Conjuge.Id;
								}

								pessoa.CredenciadoId = credenciado.Id;
								_pessoaCredenciadoBus.SalvarPublico(pessoa, bancoDeDados, executor);
							}
						}
						else if (credenciado.Pessoa.Fisica.Conjuge != null && !string.IsNullOrEmpty(credenciado.Pessoa.Fisica.Conjuge.CPFCNPJ))
						{
							credenciado.Pessoa.Fisica.Conjuge.CredenciadoId = credenciado.Id;
							_pessoaCredenciadoBus.SalvarPublico(credenciado.Pessoa.Fisica.Conjuge, bancoDeDados, executor);
							credenciado.Pessoa.Fisica.ConjugeId = credenciado.Pessoa.Fisica.Conjuge.Id;
						}

						credenciado.Pessoa.CredenciadoId = credenciado.Id;
						credenciado.Pessoa.IsCredenciado = true;
						_pessoaCredenciadoBus.SalvarPublico(credenciado.Pessoa, bancoDeDados, executor);

						_da.Editar(credenciado, bancoDeDados, gerarHistorico: false);

						Historico historico = new Historico();
						historico.Gerar(credenciado.Id, eHistoricoArtefato.credenciado, eHistoricoAcao.criar, bancoDeDados, executor);

						if (credenciado.Tipo != (int)eCredenciadoTipo.OrgaoParceiroConveniado)
						{
							EnviarEmail(credenciado, credenciado.Email);
						}
						
						if (!Validacao.EhValido)
						{
							bancoDeDados.Rollback();
						}
						else
						{
							bancoDeDados.Commit();
						}
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool EnviarEmail(CredenciadoPessoa credenciado, string email, bool cadastro = true, BancoDeDados banco = null)
		{
			try
			{
				Email mail = new Email();
				mail.Assunto = "Chave de acesso Módulo Credenciado";
				mail.Destinatario = email;
				mail.Tipo = eEmailTipo.CredenciadoAtivacao;
				mail.Codigo = credenciado.Id;
				String urlChave = CredenciadoLinkAcesso + "/Credenciado?chave=" + credenciado.Chave;

				if (cadastro)
				{
					mail.Texto = @"<p><strong>Prezado(a)</strong>&nbsp;" + credenciado.Pessoa.NomeRazaoSocial + @"!</p>
					<p>Informamos que seu cadastro foi realizado com sucesso!</p>
					<p>Para acessar o <strong>Módulo Credenciado</strong>, utilize as seguintes informações:</p>
					<ol>
						<li>Acesse o site utilizando o Link:&nbsp;&nbsp;<a href='" + urlChave + @"'>Link do Credenciado</a></li>
						<li>Caso o link acima não esteja funcionando, copie e cole o URL a seguir no seu Navegador: " + urlChave + @"</li>
					</ol>
					<p><font color='#0066ff'><strong>Atenção:</strong></font>
					<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;A chave de acesso gerada acima possui uma validade de 5 dias.
					<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Após este período será necessário realizar um novo cadastro.
					</p>
					<p><font color='#ff0000'><strong>Importante:</strong></font></p>
					<ol>
						<li>Ao informar o login e senha, por favor, verifique se não há espaços em branco.</li>
						<li>O anti pop-up deve estar desativado na sua máquina.</li>
					</ol>";
				}
				else
				{
					mail.Texto = @"<p><strong>Prezado(a)</strong>&nbsp;" + credenciado.Pessoa.NomeRazaoSocial + @"!</p>
					<p>Informamos que uma nova chave de acesso foi gerada para seu usuário no <strong>Módulo Credenciado</strong> devido a 
					uma solicitação de nova senha.</p>
					<p>Você não poderá acessar o sistema antes de criar uma nova senha.</p>
					<p>Para acessar o <strong>Módulo Credenciado</strong>, utilize as seguintes informações:</p>
					<ol>
						<li>Acesse o site utilizando o Link:&nbsp;&nbsp;<a href='" + urlChave + @"'>Link do Credenciado</a></li>
						<li>Caso o link acima não esteja funcionando, copie e cole o URL a seguir no seu Navegador: " + urlChave + @"</li>
					</ol>
					<p><font color='#0066ff'><strong>Atenção:</strong></font></p>
					<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;A chave de acesso gerada acima possui uma validade de 5 dias.
					<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Após este período será necessário realizar um novo cadastro.
					</p>
					<p><font color='#ff0000'><strong>Importante:</strong></font></p>
					<ol>
						<li>Ao informar a senha, por favor, verifique se não há espaços em branco.</li>
						<li>O anti pop-up deve estar desativado na sua máquina.</li>
					</ol>";
				}

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_emailBus.Deletar(mail.Tipo, mail.Codigo, bancoDeDados);
					_emailBus.Enviar(mail, bancoDeDados);

					bancoDeDados.Commit();
					return true;
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public bool AlterarSenha(String login, String senha, String novaSenha, String confirmarNovaSenha, BancoDeDados banco = null)
		{
			if (!_validar.AlterarSenha(login, senha, novaSenha, confirmarNovaSenha))
			{
				return false;
			}

			Usuario usuario = _busUsuario.ValidarUsuario(login, GerenciarAutenticacao.Criptografar(login, senha));

			if (usuario == null || usuario.Id == 0)
			{
				Validacao.Add(Mensagem.Login.LoginSenhaInvalido);
				return false;
			}

			string senhaNovaHash = GerenciarAutenticacao.Criptografar(login, novaSenha);
			if (_busUsuario.VerificarHistoricoSenha(usuario.Id, senhaNovaHash, _configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyQtdVerificaoUltimaSenha)))
			{
				Validacao.Add(Mensagem.Login.HistoricoSenha(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyQtdVerificaoUltimaSenha)));
				return false;
			}

			CredenciadoPessoa credenciado = _da.ObterCredenciadoExecutor(login);
			AutenticacaoExecutor executor = GerenciarAutenticacao.ObterAutenticacaoExecutor();

			if (executor == null)
			{
				executor = new AutenticacaoExecutor();
				executor.Tipo = (int)eCredenciadoSituacao.Ativo;
				executor.UsuarioId = credenciado.Usuario.Id;
				executor.Tid = credenciado.Tid;
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();
				_busUsuario.AlterarSenha(usuario, senhaNovaHash, executor, bancoDeDados);

				_da.AlterarSenha(usuario.Id, credenciado, bancoDeDados);
				bancoDeDados.Commit();
			}

			Validacao.Add(Mensagem.Login.SenhaAlterada);

			return true;
		}

		public bool GerenciarAcesso(String chave, String login, String senha, String confirmarSenha)
		{
			CredenciadoPessoa credenciado = ObterCredenciado(chave);
			credenciado.Usuario.Login = login;
			credenciado.Chave = chave;

			if (credenciado.Situacao == (int)eCredenciadoSituacao.AguardandoAtivacao)
			{
				return Ativar(credenciado, senha, confirmarSenha);
			}

			return Reativar(credenciado, senha, confirmarSenha);
		}

		public bool AlterarSituacao(int id, string nome, int novaSituacao, string motivo, BancoDeDados banco = null)
		{
			try
			{
				CredenciadoPessoa credenciado = Obter(id, true, banco);
				credenciado.Situacao = novaSituacao;

				if (_validar.AlterarSituacao(id, novaSituacao, motivo))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						_da.AlterarSituacao(credenciado, motivo);

						bancoDeDados.Commit();
					}

					if (credenciado.Tipo != (int)eCredenciadoTipo.OrgaoParceiroConveniado)
					{
						Validacao.Add(Mensagem.Credenciado.AlterarSituacao(ListaCredenciadoBus.CredenciadoSituacoes.Single(x => x.Id == novaSituacao).Texto, nome));
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public void AlterarSituacao(CredenciadoPessoa credenciado, BancoDeDados banco = null)
		{
			try
			{
				if (_validar.AlterarSituacao(credenciado.Id, credenciado.Situacao, string.Empty))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						_da.AlterarSituacao(credenciado, bancoDeDados);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void RegerarChave(int id, BancoDeDados banco = null, eCredenciadoSituacao situacao = eCredenciadoSituacao.AguardandoChave)
		{
			try
			{
				CredenciadoPessoa credenciado = Obter(id, true, banco);

				credenciado.Id = id;
				credenciado.Situacao = (int)situacao;

				credenciado.Pessoa = ObterPessoaCredenciado(credenciado.Pessoa.Id, banco);

				if (!_validar.RegerarChave(credenciado.Pessoa))
				{
					return;
				}

				string nome = credenciado.Pessoa.NomeRazaoSocial;
				string email = credenciado.Pessoa.MeiosContatos.Find(x => x.TipoContato == eTipoContato.Email).Valor;

				credenciado.Chave = GerarChaveAcesso(email, nome);

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					GerenciadorTransacao.ObterIDAtual();

					_da.RegerarChave(credenciado, bancoDeDados);

					EnviarEmail(credenciado, email, false);

					bancoDeDados.Commit();

					Validacao.Add(Mensagem.Credenciado.RegerarChave);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public bool Ativar(CredenciadoPessoa credenciado, String senha, String confirmarSenha)
		{
			try
			{
				if (_validar.ValidarAtivar(credenciado, senha, confirmarSenha))
				{
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						Executor executorHistorico = new Executor();
						executorHistorico.Id = credenciado.Id;
						executorHistorico.Tid = credenciado.Tid;
						executorHistorico.Nome = credenciado.Nome;
						executorHistorico.Login = credenciado.Usuario.Login;
						executorHistorico.Tipo = eExecutorTipo.Credenciado;

						bancoDeDados.IniciarTransacao();
						GerenciadorTransacao.ObterIDAtual();

						#region Pessoa

						using (BancoDeDados bancoDeDadosInterno = BancoDeDados.ObterInstancia())
						{
							bancoDeDadosInterno.IniciarTransacao();

							if (!_pessoaInternoBus.Existe(credenciado.Pessoa.CPFCNPJ, bancoDeDadosInterno))
							{
								credenciado.Pessoa = _pessoaCredenciadoBus.Obter(credenciado.Pessoa.Id, bancoDeDados);

								int id = credenciado.Pessoa.Id;
								credenciado.Pessoa.Id = 0;

								#region Juridica Pessoa/Representantes

								if (credenciado.Pessoa.IsJuridica && credenciado.Pessoa.Juridica.Representantes != null)
								{
									Pessoa pessoaAux = null;

									for (int i = 0; i < credenciado.Pessoa.Juridica.Representantes.Count; i++)
									{
										pessoaAux = credenciado.Pessoa.Juridica.Representantes[i];
										int representanteInternoID = _pessoaInternoBus.ObterId(pessoaAux.CPFCNPJ, bancoDeDadosInterno);

										if (representanteInternoID == 0)
										{
											//Cria a pessoa do representante no interno
											pessoaAux = _pessoaCredenciadoBus.Obter(pessoaAux.Id, bancoDeDados);
											pessoaAux.Id = 0;

											#region Conjuge

											if (pessoaAux.Fisica.ConjugeId > 0)
											{
												int conjugeInternoId = _pessoaInternoBus.ObterId(pessoaAux.Fisica.ConjugeCPF, bancoDeDadosInterno);

												if (conjugeInternoId == 0)
												{
													// Cria Conjuge do representante no interno
													Pessoa pessoaConjuge = _pessoaCredenciadoBus.Obter(pessoaAux.Fisica.ConjugeId.GetValueOrDefault(), bancoDeDados);
													pessoaConjuge.Id = 0;
													pessoaConjuge.Fisica.ConjugeId = 0;

													_pessoaInternoBus.Salvar(pessoaConjuge, bancoDeDadosInterno, executorHistorico);

													pessoaAux.Fisica.ConjugeId = pessoaConjuge.Id;
												}
												else
												{
													pessoaAux.Fisica.ConjugeId = conjugeInternoId;
												}
											}

											#endregion

											_pessoaInternoBus.Salvar(pessoaAux, bancoDeDadosInterno, executorHistorico);

											credenciado.Pessoa.Juridica.Representantes[i].Id = pessoaAux.Id;
										}
										else
										{
											credenciado.Pessoa.Juridica.Representantes[i].Id = representanteInternoID;
										}
									}
								}

								#endregion

								#region Fisica Conjuge

								if (credenciado.Pessoa.IsFisica && credenciado.Pessoa.Fisica.ConjugeId > 0)
								{
									int conjugeInternoId = _pessoaInternoBus.ObterId(credenciado.Pessoa.Fisica.ConjugeCPF, bancoDeDadosInterno);

									if (conjugeInternoId == 0)
									{
										// Cria Conjuge do representante no interno
										Pessoa pessoaConjuge = _pessoaCredenciadoBus.Obter(credenciado.Pessoa.Fisica.ConjugeId.GetValueOrDefault(), bancoDeDados);
										pessoaConjuge.Id = 0;
										pessoaConjuge.Fisica.ConjugeId = 0;

										_pessoaInternoBus.Salvar(pessoaConjuge, bancoDeDadosInterno, executorHistorico);

										credenciado.Pessoa.Fisica.ConjugeId = pessoaConjuge.Id;
									}
									else
									{
										credenciado.Pessoa.Fisica.ConjugeId = conjugeInternoId;
									}
								}

								#endregion

								_pessoaInternoBus.Salvar(credenciado.Pessoa, bancoDeDadosInterno, executorHistorico);
								credenciado.Pessoa.Id = id;
							}
						}

						#endregion

						#region Usuário

						string hashSenha = GerenciarAutenticacao.Criptografar(credenciado.Usuario.Login, senha);

						if (credenciado.Usuario.Id <= 0)
						{
							AutenticacaoExecutor executor = new AutenticacaoExecutor();
							executor.Tipo = (int)eExecutorTipo.Credenciado;
							executor.UsuarioId = credenciado.Id;
							executor.Tid = credenciado.Tid;
							_busUsuario.Salvar(credenciado.Usuario, hashSenha, executor, bancoDeDados);
						}

						_da.Ativar(credenciado, bancoDeDados, executorHistorico);

						bancoDeDados.Commit();

						#endregion
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool Reativar(CredenciadoPessoa credenciado, String senha, String confirmarSenha)
		{
			try
			{
				if (_validar.ValidarReativar(credenciado.Chave, senha, confirmarSenha))
				{
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();
						GerenciadorTransacao.ObterIDAtual();

						string hashSenha = GerenciarAutenticacao.Criptografar(credenciado.Usuario.Login, senha);

						AutenticacaoExecutor executor = new AutenticacaoExecutor();
						executor.Tipo = (int)eExecutorTipo.Credenciado;
						executor.Tid = credenciado.Tid;
						executor.UsuarioId = credenciado.Usuario.Id;

						_busUsuario.AlterarSenha(credenciado.Usuario, hashSenha, executor, bancoDeDados);

						Executor executorHistorico = new Executor();
						executorHistorico.Id = credenciado.Id;
						executorHistorico.Tid = credenciado.Tid;
						executorHistorico.Nome = credenciado.Nome;
						executorHistorico.Login = credenciado.Usuario.Login;
						executorHistorico.Tipo = eExecutorTipo.Credenciado;

						_da.Ativar(credenciado, bancoDeDados, executorHistorico);

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

		public bool AlterarDados(CredenciadoPessoa credenciado, string senha, string confirmarSenha)
		{
			try
			{
				credenciado.Id = credenciado.Pessoa.CredenciadoId.GetValueOrDefault();
				credenciado.Usuario.Id = User.UsuarioId;
				credenciado.Usuario.Login = User.Login;

				if (_validar.VerificarAlterarDados(credenciado, senha, confirmarSenha))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						#region Alterar senha

						if (credenciado.AlterarSenha)
						{
							string hashSenha = GerenciarAutenticacao.Criptografar(credenciado.Usuario.Login, senha);
							_busUsuario.AlterarSenha(credenciado.Usuario, hashSenha, GerenciarAutenticacao.ObterAutenticacaoExecutor(), bancoDeDados);
						}

						#endregion

						#region Pessoa

						_pessoaCredenciadoBus.Salvar(credenciado.Pessoa, bancoDeDados);

						CredenciadoPessoa aux = Obter(credenciado.Id, true);
						_da.Salvar(aux, bancoDeDados);

						bancoDeDados.Commit();

						#endregion
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		internal static string GerarChaveAcesso(string email, string nome)
		{
			string strTexto = (email + string.Empty).ToLower() + "$" + DateTime.Now.Ticks.ToString() + "$" + nome;
			UTF8Encoding encoder = new UTF8Encoding();
			SHA512 sha512 = SHA512.Create();
			byte[] byteHash = sha512.ComputeHash(encoder.GetBytes(strTexto));

			return string.Join("", byteHash.Select(bin => bin.ToString("X2")).ToArray());
		}

		public bool AlterarEmail(CredenciadoPessoa credenciado)
		{
			try
			{
				if (_validar.VerificarEmail(credenciado.Pessoa))
				{
					credenciado.Situacao = credenciado.Tipo == (int)eCredenciadoTipo.OrgaoParceiroConveniado ? (int)eCredenciadoSituacao.Cadastrado : (int)eCredenciadoSituacao.AguardandoAtivacao;

					Executor executor = new Executor();
					executor.Id = credenciado.Id;
					executor.Tid = credenciado.Tid;
					executor.Nome = credenciado.Nome;
					executor.Tipo = eExecutorTipo.Credenciado;

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						credenciado.Chave = GerarChaveAcesso(credenciado.Email, credenciado.Pessoa.NomeRazaoSocial);

						_pessoaCredenciadoBus.AlterarEmail(credenciado.Pessoa, bancoDeDados, executor);

						_da.Editar(credenciado, bancoDeDados, executor);

						if (credenciado.Tipo != (int)eCredenciadoTipo.OrgaoParceiroConveniado)
						{
							EnviarEmail(credenciado, credenciado.Email);
						}

						if (!Validacao.EhValido)
						{
							bancoDeDados.Rollback();
						}
						else
						{
							bancoDeDados.Commit();
						}
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Comandos Autenticação

		internal bool Autenticar(string login, String senhaHash, int timeout, out String userSessionId, BancoDeDados banco)
		{
			bool usuarioValido = false;
			userSessionId = string.Empty;
			Usuario usuarioAuditoria = new Usuario();
			usuarioAuditoria.Login = login;//Necessario para auditoria

			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();
					usuarioAuditoria.Ip = HttpContext.Current.Request.UserHostAddress;

					#region Busca Usuario/Credenciado e verifica validade

					Usuario usuario = _busUsuario.Obter(login, bancoDeDados);

					//Deve retornar APENAS se usuario não existir
					if (usuario == null)
					{
						Validacao.Add(Mensagem.Login.LoginSenhaInvalido);
						return false;
					}

					usuarioValido = String.Equals(usuario.senhaHash, senhaHash);
					usuarioAuditoria.Id = usuario.Id;
					usuarioAuditoria.TID = usuario.TID;

					//Buscar Credenciado e grava num de tentativas
					CredenciadoPessoa credenciado = _da.ObterCredenciadoLogin(usuario.Id, timeout);

					//Deve retornar APENAS se Credenciado não existir
					if (credenciado == null)
					{
						Validacao.Add(Mensagem.Login.LoginSenhaInvalido);
						return false;
					}

					credenciado.Usuario = usuario;

					#endregion

					#region Execução Obrigatoria [Independe de status do credenciado]

					string strSessionForcarLogoff = string.Empty;

					//Não se pode char busUsuario Autenticar antes do metodo no finally!!!
					if (usuarioValido && credenciado.Logado)
					{
						GerenciarAutenticacao.Deslogar(login, true);

						if (usuarioValido && credenciado.ForcarLogout)
						{
							strSessionForcarLogoff = credenciado.SessionId;

							if (!String.Equals(credenciado.Usuario.Ip, usuarioAuditoria.Ip))
							{
								Validacao.Add(Mensagem.Login.SessaoDerrubada);
							}
						}
					}

					#endregion

					#region Bloqueio e mensagem de bloqueado

					if (credenciado.Tentativa > _config.Obter<Int32>(ConfiguracaoCredenciado.KeyNumTentativas))
					{
						credenciado.Situacao = (int)eCredenciadoSituacao.Bloqueado;
						credenciado.Usuario = new Usuario() { Login = login };
						_da.AlterarSituacao(credenciado, bancoDeDados);
						Validacao.Add(Mensagem.Login.FuncionarioBloqueado);
						return false;
					}

					if (credenciado.Situacao == (int)eCredenciadoSituacao.Bloqueado)
					{
						Validacao.Add(Mensagem.Login.SituacaoInvalida(CredenciadoSituacoes.Single(x => x.Id == credenciado.Situacao).Nome));
						return false;
					}

					#endregion

					#region Aguardando Chave

					if (credenciado.Situacao == (int)eCredenciadoSituacao.AguardandoAtivacao || credenciado.Situacao == (int)eCredenciadoSituacao.AguardandoChave)
					{
						Validacao.Add(Mensagem.Login.AguardandoChave);
						return false;
					}

					#endregion

					//Fazer aqui
					//Acesso não permitido nestes horários e/ou dia! Entre em contato com o administrador do sistema

					#region Valida senha vencida

					// 4 -Senha Vencida
					if (_da.VerificarSenhaVencida(usuario.Id))
					{
						//Mensagem gerada na interface
						return false;
					}

					#endregion

					#region Mensagem Número de tentativas

					if (!usuarioValido && credenciado.Situacao != (int)eCredenciadoSituacao.Bloqueado)
					{
						Validacao.Add(Mensagem.Login.NumTentativas(credenciado.Tentativa, _config.Obter<Int32>(ConfiguracaoCredenciado.KeyNumTentativas)));
					}

					#endregion

					//Efetiva a autenticação de credenciado [Atenção para o finally]
					if (usuarioValido)
					{
						credenciado.SessionId = Guid.NewGuid().ToString();
						userSessionId = credenciado.SessionId; //Parâmetro out deste metodo!!! 
						_da.Autenticar(credenciado, strSessionForcarLogoff, bancoDeDados);
					}

					bancoDeDados.Commit();
				}
			}
			catch
			{
				usuarioValido = false;
				throw;
			}
			finally
			{
				//Autentica Usuario, gera historico e linha de auditoria 
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_busUsuario.Autenticar(usuarioAuditoria, usuarioValido, (int)eExecutorTipo.Credenciado, bancoDeDados);

					bancoDeDados.Commit();
				}
			}

			return usuarioValido;
		}

		internal void Deslogar(string login, BancoDeDados banco = null)
		{
			_da.Deslogar(login, banco);
		}

		#endregion

		#region Validações/Auxiliares

		public string AlterarSenhaMensagem(String login)
		{
			int situacao = _da.ObterSituacao(login);

			if (situacao == (int)eCredenciadoSituacao.SenhaVencida)
			{
				return Mensagem.Login.AlterarSenhaExpirada(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keySenhaExpiracaoDias)).Texto;
			}
			return String.Empty;
		}

		public bool ValidarPessoaCpfCnpj(Pessoa pessoa)
		{
			return _validar.ValidarPessoaCpfCnpj(pessoa);
		}

		public bool ExisteCredenciado(string cpfCnpj)
		{
			return _da.ExisteCredenciado(cpfCnpj);
		}

		public bool ExisteInterno(string cpfCnpj)
		{
			return _da.ExisteInterno(cpfCnpj);
		}

		public bool IsCredenciadoAtivo(string cpfCnpj)
		{
			return _da.IsCredenciadoAtivo(cpfCnpj);
		}

		public bool IsCredenciadoAtivoAlgumaVez(int credenciadoID)
		{
			return _da.IsCredenciadoAtivoAlgumaVez(credenciadoID);
		}

		public bool IsBloqueado(string cpfCnpj)
		{
			return _da.IsBloqueado(cpfCnpj);
		}

		public bool VerificarChaveAtiva(String chave)
		{
			bool flag = false;
			try
			{
				flag = _validar.VerificarChave(chave);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return flag;
		}

		public bool VerificarSeDeveDeslogar(String login, String sessionId, int timeout)
		{
			return _da.VerificarSeDeveDeslogar(login, sessionId, timeout);
		}

		public bool IsCredenciadoOrgaoParceiroPublico(string cpfCnpj)
		{
			return _da.IsCredenciadoOrgaoParceiroPublico(cpfCnpj);
		}

		#endregion

		#region Obter/Listar

		public List<int> ObterIdsCredenciadosParceiros(int idOrgaoParceiro, int idUnidade)
		{
			List<int> idsCredenciados = null;
			try
			{
				using(BancoDeDados banco = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					idsCredenciados = _da.ObterIdsCredenciadosParceiros(idOrgaoParceiro, idUnidade, banco);
				}
				
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return idsCredenciados;
		}

		public Resultados<Cred.ListarFiltro> Filtrar(Cred.ListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				var filtros = new Filtro<Cred.ListarFiltro>(filtrosListar, paginacao);
				var resultados = _da.Filtrar(filtros);

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

		public string ObterProfissao(int profissaoId)
		{
			var prof = ListaCredenciadoBus.Profissoes.Where(x => x.IsAtivo).FirstOrDefault(x => x.Id == profissaoId);
			if (prof == null) return "";
			return prof.Texto;
		}

		public Pessoa ObterPessoaCredenciado(string cpfCnpj)
		{
			return _pessoaCredenciadoBus.Obter(cpfCnpj);
		}

		public Pessoa ObterPessoaCredenciado(int id, BancoDeDados banco = null)
		{
			return _pessoaCredenciadoBus.Obter(id, banco);
		}

		public Pessoa ObterPessoaInterno(string cpfCnpj)
		{
			return _pessoaInternoBus.Obter(cpfCnpj);
		}

		public Pessoa ObterPessoaInterno(int id)
		{
			return _pessoaInternoBus.Obter(id);
		}

		public void ObterCredenciadoInterno(Pessoa pessoa)
		{
			_da.ObterCredenciadoInterno(pessoa);
		}

		public CredenciadoPessoa ObterCredenciado(String chave)
		{
			return _da.ObterCredenciado(chave);
		}

		public CredenciadoPessoa Obter(String cpfCnpj, bool simplificado = false)
		{
			CredenciadoPessoa credenciado = _da.Obter(cpfCnpj, simplificado);

			if (credenciado != null && credenciado.Id > 0)
			{
				credenciado.Pessoa = _pessoaCredenciadoBus.Obter(credenciado.Pessoa.Id);
			}

			return credenciado;
		}

		public CredenciadoPessoa Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					return _da.Obter(id, simplificado, bancoDeDados);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		internal CredenciadoPessoa ObterCredenciadoAutenticacao(String login)
		{
			var cred = _da.ObterCredenciadoAutenticacao(login);
			if (cred != null)
			{
				//Busca permissões extras
				var permissoes = ObterPermissoesExtras(cred.Id);
				cred.Permissoes.AddRange(permissoes);
			}
			return cred;
		}

		internal List<Blocos.Entities.Credenciado.Security.Permissao> ObterPermissoesExtras(int id)
		{
			#region Obter as permissões extras do credenciado

			var permissoes = _da.ObterPermissoesExtras(id);
			
			#endregion

			return permissoes;
		}

		public List<Lista> ObterUnidadesLst(int orgao, List<Unidade> lista = null)
		{
			List<Lista> list = null;

			try
			{
				if (lista != null && lista.Count > 0)
				{
					list = new List<Lista>();
					lista.ForEach(x => list.Add(new Lista()
					{
						Texto = x.Sigla + " - " + x.Nome,
						Id = x.Id.ToString(),
						IsAtivo = true
					}));
				}
				else
				{
					list = _da.ObterUnidadesLst(orgao);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return list;
		}

		public List<Lista> ObterOrgaosParceirosLst()
		{
			List<Lista> list = null;

			try
			{
				list = _da.ObterOrgaosParceirosLst();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return list;

		}

		#endregion

	}
}