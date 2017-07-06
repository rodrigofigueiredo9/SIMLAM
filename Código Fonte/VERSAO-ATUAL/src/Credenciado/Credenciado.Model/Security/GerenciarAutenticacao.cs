using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using Tecnomapas.Blocos.Autenticacao;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Security.Interfaces;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Security
{
	public class GerenciarAutenticacao : IGerenciarAutenticacao
	{
		#region Propriedades

		private static CredenciadoBus _busCred = new CredenciadoBus(new CredenciadoValidar());
		public static GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public static String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		internal static AutenticacaoExecutor ObterAutenticacaoExecutor()
		{
			if (HttpContext.Current.User == null || !(HttpContext.Current.User is EtramitePrincipal))
			{
				return null;
			}

			AutenticacaoExecutor executor = new AutenticacaoExecutor();

			EtramitePrincipal user = (HttpContext.Current.User as EtramitePrincipal);
			executor.Tipo = (int)eExecutorTipo.Credenciado;
			executor.UsuarioId = user.EtramiteIdentity.UsuarioId;
			executor.Tid = user.EtramiteIdentity.FuncionarioTid;

			return executor;
		}

		internal static String Criptografar(string login, string senha)
		{
			string strTexto = login.ToLower() + "*" + senha;
			UTF8Encoding encoder = new UTF8Encoding();
			SHA512 sha512 = SHA512.Create();
			byte[] byteHash = sha512.ComputeHash(encoder.GetBytes(strTexto));

			return string.Join("", byteHash.Select(bin => bin.ToString("X2")).ToArray());
		}

        public bool ValidarLogin(string login, string senha, out string sessionId)
        {
            return GerenciarAutenticacao.ValidarLogOn(login, senha, out sessionId);
        }

		public static bool ValidarLogOn(string login, string senha, out string sessionId)
		{
			#region Validacao de Obrigatoriedade

			if (string.IsNullOrEmpty(login))
			{
				Validacao.Add(Mensagem.Login.ObrigatorioLogin);
			}

			if (string.IsNullOrEmpty(senha))
			{
				Validacao.Add(Mensagem.Login.ObrigatorioSenha);
			}

			if (!Validacao.EhValido)
			{
				sessionId = string.Empty;
				return false;
			}

			#endregion

			bool retorno = false;
			string hash = Criptografar(login, senha);
			senha = string.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();
				GerenciadorTransacao.ObterIDAtual();

				retorno = _busCred.Autenticar(login, hash, FormsAuthentication.Timeout.Minutes, out sessionId, bancoDeDados);

				bancoDeDados.Commit();
			}

			return retorno;
		}

        public static bool ValidarRecuperacaoSenha(string cpf, string email)
        {
            #region Validacao de Obrigatoriedade

            if (string.IsNullOrEmpty(cpf))
            {
                Validacao.Add(Mensagem.Login.ObrigatorioCpf);
            }

            if (string.IsNullOrEmpty(email))
            {
                Validacao.Add(Mensagem.Login.ObrigatorioEmail);
            }

            if (!Validacao.EhValido)
            {
                return false;
            }

            #endregion

            bool retorno = false;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
            {
                retorno = _busCred.PodeSolicitarSenha(cpf, email, bancoDeDados);
            }

            return retorno;
        }

        public static void RecuperarSenha(string cpf, string email)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
            {
                CredenciadoPessoa cred = _busCred.Obter(cpf);

                if (cred != null && cred.Id != 0 && (cred.Situacao == 2 || cred.Situacao == 4))
                {
                    eCredenciadoSituacao situacao = cred.Situacao == 2 ? eCredenciadoSituacao.Ativo : eCredenciadoSituacao.SenhaVencida;

                    _busCred.RegerarChave(cred.Id, bancoDeDados, situacao);
                }
                else
                {
                    Validacao.Add(Mensagem.Login.SenhaNaoEnviada);
                }
            }
        }

		public static void CarregarUser(string login, string sessionId = null)
		{
			if (HttpContext.Current.User == null || !(HttpContext.Current.User is EtramitePrincipal))
			{
				if (!String.IsNullOrEmpty(sessionId))
				{
					if (VerificarDeslogar(login, sessionId))
					{
						HttpContext.Current.User = null;
						return;
					}
				}

				CredenciadoPessoa credenciado = _busCred.ObterCredenciadoAutenticacao(login);

				VerificarAtivo(credenciado);

				ePermissao[] arrayPerm = credenciado.Permissoes.Select(x => x.Codigo).ToArray();

				EtramiteIdentity userIndentity = new EtramiteIdentity(credenciado.Nome, credenciado.Usuario.Login, credenciado.Email,
					credenciado.Usuario.DataUltimoLogon, credenciado.Usuario.IpUltimoLogon, credenciado.Id, credenciado.Tipo, credenciado.TipoTexto, credenciado.Tid, credenciado.Usuario.Id, 2);

				EtramitePrincipal userPrincipal = new EtramitePrincipal<ePermissao>(userIndentity, arrayPerm);
				HttpContext.Current.User = userPrincipal;
			}

			if (HttpContext.Current.User.Identity.IsAuthenticated)
			{
				UsuarioBus busUsuario = new UsuarioBus(HistoricoAplicacao.CREDENCIADO, UsuarioCredenciado);

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();
					busUsuario.SalvarDataUltimaAcao(login, bancoDeDados);
					bancoDeDados.Commit();
				}
			}
		}

		private static void VerificarAtivo(CredenciadoPessoa credenciado)
		{
			if (credenciado.Id == 0)
			{
				Validacao.Add(Mensagem.Login.UsuarioBloqueado);
				FormsAuthentication.SignOut();
				FormsAuthentication.RedirectToLoginPage("msg=" + Validacao.QueryParam());
			}
		}

		public static bool VerificarDeslogar(string login, string sessionId)
		{
			if (_busCred.VerificarSeDeveDeslogar(login, sessionId, FormsAuthentication.Timeout.Minutes))
			{
				Validacao.Add(Mensagem.Login.AcessoSimultaneo);
				Deslogar();
				FormsAuthentication.SignOut();
				FormsAuthentication.RedirectToLoginPage("msg=" + Validacao.QueryParam());
			}

			return false;
		}

		public static void Deslogar(string login = null, bool isDeslogarForcado = false)
		{
			if (login == null && HttpContext.Current.User == null && !(HttpContext.Current.User is EtramitePrincipal))
			{
				return;
			}

			AutenticacaoExecutor executor = new AutenticacaoExecutor();
			executor.Tipo = (int)eExecutorTipo.Credenciado;
			UsuarioBus busUsuario = new UsuarioBus(HistoricoAplicacao.CREDENCIADO, UsuarioCredenciado);

			if (HttpContext.Current.User != null && (HttpContext.Current.User is EtramitePrincipal))
			{
				EtramitePrincipal user = (HttpContext.Current.User as EtramitePrincipal);
				login = user.EtramiteIdentity.Login;
				executor.Tid = user.EtramiteIdentity.FuncionarioTid;
				executor.UsuarioId = user.EtramiteIdentity.UsuarioId;
			}
			else
			{
				CredenciadoPessoa credenciado = _busCred.Obter(login);
				executor.Tid = credenciado.Tid;
				executor.UsuarioId = credenciado.Usuario.Id;
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				busUsuario.Deslogar(login, executor, isDeslogarForcado, bancoDeDados);
				_busCred.Deslogar(login, bancoDeDados);

				bancoDeDados.Commit();
			}
		}
	}
}