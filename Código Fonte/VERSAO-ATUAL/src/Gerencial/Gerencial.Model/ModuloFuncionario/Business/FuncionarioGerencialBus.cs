using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Autenticacao;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Gerencial.Model.ModuloFuncionario.Data;
using Tecnomapas.EtramiteX.Gerencial.Model.Security;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;

namespace Tecnomapas.EtramiteX.Gerencial.Model.ModuloFuncionario.Business
{
	public class FuncionarioGerencialBus
	{
		#region Propriedades

		IFuncionarioValidar _validar;
		FuncionarioGerencialDa _da;
		UsuarioBus _busUsuario;
		GerenciadorConfiguracao<ConfiguracaoFuncionario> _config;
		GerenciadorConfiguracao<ConfiguracaoUsuario> _configUsuario;

		public List<Situacao> Situacoes
		{
			get { return _da.ObterSituacaoFuncionario(); }
		}

		public List<String> PermissoesGerencial
		{
			get
			{
				return new List<String>() { 
				ePermissao.GerencialAcessar.ToString(),
				ePermissao.RelatorioPersonalizadoListar.ToString(),
				ePermissao.RelatorioPersonalizadoExecutar.ToString()
			};
			}
		}

		#endregion Propriedades

		public FuncionarioGerencialBus() : this(new FuncionarioValidar()) { }

		public FuncionarioGerencialBus(IFuncionarioValidar validacao)
		{
			_validar = validacao;
			_da = new FuncionarioGerencialDa();
			_busUsuario = new UsuarioBus(HistoricoAplicacao.INTERNO);
			_config = new GerenciadorConfiguracao<ConfiguracaoFuncionario>(new ConfiguracaoFuncionario());
			_configUsuario = new GerenciadorConfiguracao<ConfiguracaoUsuario>(new ConfiguracaoUsuario());
		}

		#region Ações

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

		internal void Deslogar(string login, BancoDeDados banco = null)
		{
			_da.Deslogar(login, banco);
		}

		#endregion Ações

		#region Obter

		internal Funcionario ObterFuncionarioAutenticacao(String login)
		{
			return _da.ObterFuncionarioAutenticacao(login, PermissoesGerencial);
		}

		internal Funcionario ObterFuncionarioExecutor(String login)
		{
			return _da.ObterFuncionarioExecutor(login);
		}

		#endregion Obter

		#region Verificar

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

		public bool VerificarSeDeveDeslogar(String login, String sessionId, int timeout)
		{
			return _da.VerificarSeDeveDeslogar(login, sessionId, timeout);
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

		#endregion Verificar
	}
}