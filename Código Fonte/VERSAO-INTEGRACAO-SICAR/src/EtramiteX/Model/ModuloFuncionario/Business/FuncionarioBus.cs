using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Autenticacao;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.Security;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business
{
	public class FuncionarioBus
	{
		public FuncionarioBus() 
		{
			_validar = new FuncionarioValidar();
		}
		public FuncionarioBus(IFuncionarioValidar validacao)
		{
			_validar = validacao;
		}

		#region Propriedades

		ListaBus _busLista = new ListaBus();
		UsuarioBus _busUsuario = new UsuarioBus(HistoricoAplicacao.INTERNO);
		IFuncionarioValidar _validar = null;
		FuncionarioDa _da = new FuncionarioDa();
		GerenciadorConfiguracao<ConfiguracaoFuncionario> _config = new GerenciadorConfiguracao<ConfiguracaoFuncionario>(new ConfiguracaoFuncionario());
		GerenciadorConfiguracao<ConfiguracaoUsuario> _configUsuario = new GerenciadorConfiguracao<ConfiguracaoUsuario>(new ConfiguracaoUsuario());
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public EtramiteIdentity User
		{
			get 
			{
				if (HttpContext.Current.User == null)
				{
					return null;
				}
				return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; 
			}
		}

		public List<Papel> PapeisFuncionario
		{
			get
			{
				return _da.ObterPapeis().OrderBy(x => x.Nome).ToList();
			}
		}

		public List<Situacao> Situacoes
		{
			get { return _da.ObterSituacaoFuncionario(); }
		}

		#endregion

		#region Acoes

		public bool Salvar(Funcionario funcionario, String senha, String ConfirmarSenha)
		{
			try
			{
				if (_validar.Salvar(funcionario, senha, ConfirmarSenha))
				{
					#region Mensagem

					Mensagem msgSucesso = Mensagem.Funcionario.Salvar;

					if (funcionario.Id > 0)
					{
						msgSucesso = Mensagem.Funcionario.Editar;
					}

					if (funcionario.Id > 0 && funcionario.Tipo != 3)
					{
						throw new Exception("Tipo de funcionário inválido");
					}

					#endregion


                    #region Arquivos/Diretorio
                    ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

                    if (funcionario.Arquivo != null && !String.IsNullOrWhiteSpace(funcionario.Arquivo.TemporarioNome))
                    {
                        if (funcionario.Arquivo.Id == 0)
                        {
                            funcionario.Arquivo = _busArquivo.Copiar(funcionario.Arquivo);
                        }
                    }
                    #endregion

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						#region Usuario

						string hashSenha = GerenciarAutenticacao.Criptografar(funcionario.Usuario.Login, senha);
						AutenticacaoExecutor executor = GerenciarAutenticacao.ObterAutenticacaoExecutor();

						if (funcionario.Usuario.Id <= 0)
						{
							_busUsuario.Salvar(funcionario.Usuario, hashSenha, executor, bancoDeDados);
						}
						else
						{
							if (!String.IsNullOrEmpty(senha))
							{
								_busUsuario.AlterarSenha(funcionario.Usuario, hashSenha, executor, bancoDeDados);
								funcionario.Situacao = 5;//Alterar senha
							}
						}

						#endregion


                        #region Arquivos/Banco
                        ArquivoDa arquivoDa = new ArquivoDa();

                        if (funcionario.Arquivo != null && !String.IsNullOrWhiteSpace(funcionario.Arquivo.TemporarioNome))
                        {
                            if (funcionario.Arquivo.Id == 0)
                            {
                                arquivoDa.Salvar(funcionario.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
                            }
                        }
                        #endregion


						_da.Salvar(funcionario, bancoDeDados);
						bancoDeDados.Commit();
					}

					Validacao.Add(msgSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		internal bool Autenticar(string login, String senhaHash, int timeout, out String userSessionId)
		{
			bool usuarioValido = false;

			userSessionId = string.Empty;
			Usuario usuarioAuditoria = new Usuario();
			usuarioAuditoria.Login = login;//Necessario para auditoria

			try
			{
				usuarioAuditoria.Ip = HttpContext.Current.Request.UserHostAddress;

				#region Busca Usuario/Credenciado e verifica validade
				Usuario usuario = _busUsuario.Obter(login);

				//Deve retornar APENAS se usuario não existir
				if (usuario == null)
				{
					Validacao.Add(Mensagem.Login.LoginSenhaInvalido);
					return false;
				}

				usuarioValido = String.Equals(usuario.senhaHash, senhaHash);
				usuarioAuditoria.Id = usuario.Id;
				usuarioAuditoria.TID = usuario.TID;

				//Buscar funcionario e grava num de tentativas
				Funcionario funcionario = _da.ObterFuncionarioLogin(usuario.Id, timeout);

				//Deve retornar APENAS se funcionario não existir
				if (funcionario == null)
				{
					Validacao.Add(Mensagem.Login.LoginSenhaInvalido);
					return false;
				}

				funcionario.Usuario = usuario;
				#endregion

				#region Execução Obrigatoria [Independe de status do funcionario]
				string strSessionForcarLogoff = string.Empty;

				//Não se pode char busUsuario Autenticar antes do metodo no finally!!!
				if (usuarioValido && funcionario.Logado)
				{
					GerenciarAutenticacao.Deslogar(login, true);

					if (usuarioValido && funcionario.ForcarLogout)
					{
						strSessionForcarLogoff = funcionario.SessionId;

						if (!String.Equals(funcionario.Usuario.Ip, usuarioAuditoria.Ip))
						{
							Validacao.Add(Mensagem.Login.SessaoDerrubada);
						}
					}
				}

				#endregion

				#region 2 - Bloqueio 
				if (funcionario.Tentativa > _config.Obter<Int32>(ConfiguracaoCredenciado.KeyNumTentativas))
				{
					funcionario.Situacao = 2;// 2 - Bloqueado
					funcionario.Usuario = new Usuario() { Login = login };
					_da.AlterarSituacao(funcionario.Id, funcionario.Situacao, null, funcionario);
					Validacao.Add(Mensagem.Login.FuncionarioBloqueado);
					return false;
				}

				if (funcionario.Situacao == 2)// 2-Ausente
				{
					Validacao.Add(Mensagem.Login.SituacaoInvalida(Situacoes.Single(x => x.Id == funcionario.Situacao).Nome));
					return false;
				}
				#endregion				

				//Fazer aqui
				//Acesso não permitido nestes horários e/ou dia! Entre em contato com o administrador do sistema				

				#region 6 - Senha Vencida
				// 6 -Senha Vencida
				if (_da.VerificarSenhaVencida(usuario.Id, funcionario))
				{
					//Mensagem gerada na interface
					return false;
				}
				#endregion

				#region 4 - Ausente
				if (funcionario.Situacao == 4)// 4-Ausente
				{
					Validacao.Add(Mensagem.Login.SituacaoInvalida(Situacoes.Single(x => x.Id == funcionario.Situacao).Nome));
					return false;
				}
				#endregion

				#region Mensagem Número de tentativas
				if (!usuarioValido && funcionario.Situacao != 2)//2 - Bloqueado
				{
					Validacao.Add(Mensagem.Login.NumTentativas(funcionario.Tentativa, _config.Obter<Int32>(ConfiguracaoCredenciado.KeyNumTentativas)));
				}
				#endregion

				//Efetiva a autenticação de credenciado [Atenção para o finally]
				if (usuarioValido)
				{
					funcionario.SessionId = Guid.NewGuid().ToString();
					userSessionId = funcionario.SessionId; //Parâmetro out deste metodo!!! 
					_da.Autenticar(funcionario, strSessionForcarLogoff);
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
				_busUsuario.Autenticar(usuarioAuditoria, usuarioValido, (int)eExecutorTipo.Interno);
			}

			return usuarioValido;
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

			//Caso alterar a senha quando logado
			Funcionario funcionario = _da.ObterFuncionarioExecutor(login);
			AutenticacaoExecutor executor = GerenciarAutenticacao.ObterAutenticacaoExecutor();

			if (executor == null)
			{
				executor = new AutenticacaoExecutor();
				executor.Tipo = (int)eExecutorTipo.Interno;
				executor.UsuarioId = funcionario.Usuario.Id;
				executor.Tid = funcionario.Tid;
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_busUsuario.AlterarSenha(usuario, senhaNovaHash, executor, bancoDeDados);
				
				_da.AlterarSenha(usuario.Id, funcionario, banco);
				bancoDeDados.Commit();
			}

			Validacao.Add(Mensagem.Login.SenhaAlterada);

			return true;
		}

		public bool AlterarSenhaFuncionario(Funcionario funcionario, String novaSenha, String confirmarNovaSenha, BancoDeDados banco = null)
		{
			if (!VerificarAlterarFuncionario(funcionario.Id))
			{
				return false;
			}
			if (!_validar.AlterarSenhaFuncionario(User.Login, novaSenha, confirmarNovaSenha))
			{
				return false;
			}

			string senhaNovaHash = GerenciarAutenticacao.Criptografar(User.Login, novaSenha);
			if (_busUsuario.VerificarHistoricoSenha(funcionario.Usuario.Id, senhaNovaHash, _configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyQtdVerificaoUltimaSenha)))
			{
				Validacao.Add(Mensagem.Login.HistoricoSenha(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyQtdVerificaoUltimaSenha)));
				return false;
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_busUsuario.AlterarSenha(funcionario.Usuario, GerenciarAutenticacao.Criptografar(User.Login, novaSenha), GerenciarAutenticacao.ObterAutenticacaoExecutor(), bancoDeDados);
				_da.AlterarSenha(funcionario.Usuario.Id, null, banco);

				bancoDeDados.Commit();
			}

			Validacao.Add(Mensagem.Funcionario.AlterarSenhaFuncionario);
			return true;
		}

		internal void Deslogar(string login, BancoDeDados banco = null)
		{
			_da.Deslogar(login, banco);
		}

		internal void AdicionarPermissaoTramitarRegistro(List<int> funcionarios, List<string> codigos, BancoDeDados banco = null)
		{
			_da.AdicionarPermissaoTramitarRegistro(funcionarios, codigos, banco);
		}

		public bool AlterarSituacao(int id, int situacao, string motivo)
		{
			if (_validar.AlterarSituacao(situacao, motivo))
			{
				AutenticacaoExecutor executor = GerenciarAutenticacao.ObterAutenticacaoExecutor();

				_da.AlterarSituacao(id, situacao, motivo);
				Validacao.Add(Mensagem.Funcionario.AlterarSituacao);
				return true;
			}
			return false;
		}

		#endregion

		#region Obter / Filtrar

		public Funcionario Obter(String Cpf)
		{
			try
			{
				return _da.Obter(Cpf);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Funcionario ObterLogado()
		{
			return Obter(User.FuncionarioId);
		}

		public Funcionario Obter(int id)
		{
			try
			{
				Funcionario func = _da.Obter(id);
				if (func == null)
				{
					Validacao.Add(Mensagem.Funcionario.Inexistente);
				}
				return func;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<Funcionario> Filtrar(FuncionarioListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<FuncionarioListarFiltro> filtro = new Filtro<FuncionarioListarFiltro>(filtrosListar, paginacao);
				Resultados<Funcionario> resultados = _da.Filtrar(filtro);

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

		internal Funcionario ObterFuncionarioAutenticacao(String login)
		{
			return _da.ObterFuncionarioAutenticacao(login);
		}
		
		internal Funcionario ObterFuncionarioExecutor(String login)
		{
			return _da.ObterFuncionarioExecutor(login);
		}
		
		public string AlterarSenhaMensagem(String login)
		{
			int situacao = _da.ObterSituacao(login);

			if (situacao == 3)
			{
				return Mensagem.Login.AlterarSenhaCadastroNovo.Texto;
			}
			else if (situacao == 5)
			{
				return Mensagem.Login.AlterarSenhaAlteradaPorAdmin.Texto;
			}
			else if (situacao == 6)
			{
				return Mensagem.Login.AlterarSenhaExpirada(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keySenhaExpiracaoDias)).Texto;
			}
			return String.Empty;
		}

		public List<Setor> ObterSetoresRegistrador(int funcionarioId)
		{
			return _da.ObterSetoresRegistrador(funcionarioId);
		}

		public List<Setor> ObterSetoresFuncionario(int funcionarioId = 0)
		{
			if (funcionarioId == 0)			
			{
				if (User == null)
				{
					return new List<Setor>();
				}

				funcionarioId = User.FuncionarioId;
			}

			try
			{
				return _da.ObterSetoresFuncionario(funcionarioId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<Setor> ObterSetoresPorTipo(int funcionarioId, int tipoTramitacaoSetor)
		{
			return _da.ObterSetoresPorTipo(funcionarioId, tipoTramitacaoSetor);
		}

		public List<FuncionarioLst> ObterFuncionariosSetor(int setorId)
		{
			return _da.ObterFuncionariosSetor(setorId);
		}

		public List<PessoaLst> ObterFuncionariosPorSetorFuncao(int setor, int cargoCodigo)
		{
			return _da.ObterFuncionariosPorSetorFuncao(setor, cargoCodigo);
		}

		public List<Funcionario> ObterFuncionarios(List<int> ids)
		{
			return _da.ObterFuncionarios(ids);
		}

		internal FuncionarioLst ObterResponsavelSetor(int setorId)
		{
			return _da.ObterResponsavelSetor(setorId);
		}

		internal List<Cargo> ObterFuncionarioCargos(int funcionarioId)
		{
			return _da.ObterFuncionarioCargos(funcionarioId);
		}

		#endregion

		#region Verificar / Validar

		public bool VerificarCpf(String cpf)
		{
			try
			{
				return _validar.Cpf(cpf);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return Validacao.EhValido;
		}

		public bool VerificarAlterarFuncionario(int id)
		{
			return _validar.VerificarAlterarFuncionario(id);
		}

		public bool VerificarSeDeveDeslogar(String login, String sessionId, int timeout)
		{
			return _da.VerificarSeDeveDeslogar(login, sessionId, timeout);
		}

		public bool VerificarFuncionarioContidoSetor(int funcionarioId, int setorId, BancoDeDados banco = null)
		{
			return _da.VerificarFuncionarioContidoSetor(funcionarioId, setorId, banco);
		}

		public bool VerificarResponsavelSetor(int idSetor)
		{
			try
			{
				return _da.VerificarResponsavelSetor(idSetor);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return false;
		}

		public bool VerificarPossuiPermissao(String login, ePermissao permissao)
		{
			Funcionario funcionario = ObterFuncionarioAutenticacao(login);

			//Deve retornar APENAS se funcionario não existir
			if (funcionario == null || funcionario.Id <= 0)
			{
				Validacao.Add(Mensagem.Login.LoginSenhaInvalido);
				return false;
			}

			if (!funcionario.Permissoes.Exists(x => x.Codigo == permissao))
			{
				Validacao.Add(Mensagem.Padrao.SemPermissao);
			}

			return Validacao.EhValido;
		}

		#endregion
	}
}