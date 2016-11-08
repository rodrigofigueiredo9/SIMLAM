using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Autenticacao;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAdministrador;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAdministrador.Data;
using Tecnomapas.EtramiteX.Interno.Model.Security;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAdministrador.Business
{
	public class AdministradorBus
	{
		#region Propriedades

		UsuarioBus _busUsuario = new UsuarioBus(HistoricoAplicacao.INTERNO);
		AdministradorValidar _validar = null;
		AdministradorDa _da = new AdministradorDa();
		GerenciadorConfiguracao<ConfiguracaoAdministrador> _config = new GerenciadorConfiguracao<ConfiguracaoAdministrador>(new ConfiguracaoAdministrador());
		GerenciadorConfiguracao<ConfiguracaoUsuario> _configUsuario = new GerenciadorConfiguracao<ConfiguracaoUsuario>(new ConfiguracaoUsuario());
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public List<Situacao> Situacoes
		{
			get { return _da.ObterSituacaoAdministrador(); }
		}

		private EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public List<Papel> PapeisAdministrador
		{
			get
			{
				if (GerenciadorCache.PapeisAdministrador == null)
				{
					GerenciadorCache.PapeisAdministrador = _da.ObterPapeis();
				}

				return GerenciadorCache.PapeisAdministrador as List<Papel>;
			}
		}

		#endregion

		public AdministradorBus(AdministradorValidar validacao)
		{
			_validar = validacao;
		}

		#region Acoes

		public void Salvar(Administrador obj, String senha, String ConfirmarSenha, bool AlterarSenha)
		{
			BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia();
			try
			{
				if (_validar.Salvar(obj, senha, ConfirmarSenha, AlterarSenha))
				{
					#region Mensagem
					Mensagem msgSucesso = Mensagem.Administrador.Salvar;

					if (obj.Id > 0)
					{
						msgSucesso = Mensagem.Administrador.Editar;
					}
					#endregion

					bancoDeDados = BancoDeDados.ObterInstancia();
					bancoDeDados.IniciarTransacao();
					GerenciadorTransacao.ObterIDAtual();

					#region Usuario

					string hashSenha = GerenciarAutenticacao.Criptografar(obj.Usuario.Login, senha);
					AutenticacaoExecutor executor = GerenciarAutenticacao.ObterAutenticacaoExecutor();

					if (obj.Usuario.Id <= 0)
					{
						_busUsuario.Salvar(obj.Usuario, hashSenha, executor, bancoDeDados);
					}
					else
					{
						if (!String.IsNullOrEmpty(senha))
						{
							_busUsuario.AlterarSenha(obj.Usuario, hashSenha, executor, bancoDeDados);
							obj.Situacao = 3;//Alterar senha
						}
					}
					#endregion

					#region Funcionário
					_da.Salvar(obj, bancoDeDados);
					#endregion

					bancoDeDados.Commit();

					Validacao.Add(msgSucesso);
				}
			}
			catch (Exception exc)
			{
				bancoDeDados.Rollback();
				Validacao.AddErro(exc);
			}
			finally
			{
				bancoDeDados.Dispose();
			}
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

			AutenticacaoExecutor executor = GerenciarAutenticacao.ObterAutenticacaoExecutor();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_busUsuario.AlterarSenha(usuario, senhaNovaHash, executor, bancoDeDados);
				_da.AlterarSenha(usuario.Id, banco);

				bancoDeDados.Commit();
			}

			Validacao.Add(Mensagem.Login.SenhaAlterada);

			return true;
		}

		public bool AlterarSenhaAdministrador(Administrador obj, String novaSenha, String confirmarNovaSenha, BancoDeDados banco = null)
		{
			if (!VerificarAlterarAdministrador(obj.Id))
			{
				return false;
			}
			if (!_validar.AlterarSenhaAdministrador(User.Login, novaSenha, confirmarNovaSenha))
			{
				return false;
			}

			string senhaNovaHash = GerenciarAutenticacao.Criptografar(User.Login, novaSenha);
			if (_busUsuario.VerificarHistoricoSenha(obj.Usuario.Id, senhaNovaHash, _configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyQtdVerificaoUltimaSenha)))
			{
				Validacao.Add(Mensagem.Login.HistoricoSenha(_configUsuario.Obter<Int32>(ConfiguracaoUsuario.keyQtdVerificaoUltimaSenha)));
				return false;
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_busUsuario.AlterarSenha(obj.Usuario, GerenciarAutenticacao.Criptografar(User.Login, novaSenha), GerenciarAutenticacao.ObterAutenticacaoExecutor(), bancoDeDados);
				_da.AlterarSenha(obj.Usuario.Id, banco);

				bancoDeDados.Commit();
			}

			Validacao.Add(Mensagem.Administrador.AlterarSenhaAdministrador);
			return true;
		}

		public bool AlterarSituacao(int id, int situacao, string motivo)
		{
			if (_validar.AlterarSituacao(situacao, motivo))
			{
				_da.AlterarSituacao(id, situacao, motivo);
				Validacao.Add(Mensagem.Administrador.AlterarSituacao);
				return true;
			}
			return false;
		}

		public bool TransferirSistema(int id, string motivo)
		{
			if (_validar.ValidarTransferirSistema(id, motivo))
			{
				_da.TransferirSistema(id, motivo);
			}
			return Validacao.EhValido;
		}

		#endregion

		#region Obter / Filtrar

		public Administrador Obter(String Cpf)
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

		public Administrador Obter(int id)
		{
			try
			{
				Administrador func = _da.Obter(id);
				if (func == null)
				{
					Validacao.Add(Mensagem.Administrador.Inexistente);
				}
				return func;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		internal Administrador ObterAdministradorAutenticacao(String login)
		{
			return _da.ObterAdministradorAutenticacao(login);
		}

		public Resultados<Administrador> Filtrar(AdministradorListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<AdministradorListarFiltro> filtro = new Filtro<AdministradorListarFiltro>(filtrosListar, paginacao);
				Resultados<Administrador> resultados = _da.Filtrar(filtro);

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

		#endregion

		#region Verificar / Validar

		public bool VerificarAlterarAdministrador(int id)
		{
			return _validar.VerificarAlterarAdministrador(id);
		}

		public bool ValidarTransferirSistema(int id)
		{
			_validar.ValidarTransferirSistema(id);
			return Validacao.EhValido;
		}

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

		#endregion
	}
}