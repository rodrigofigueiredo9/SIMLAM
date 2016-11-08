using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using Tecnomapas.Blocos.Autenticacao;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Security
{
	public class GerenciarAutenticacao
	{
		private static FuncionarioBus _busFunc = new FuncionarioBus(new FuncionarioValidar());

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

			string hash = Criptografar(login, senha);
			senha = string.Empty;

			return _busFunc.Autenticar(login, hash, FormsAuthentication.Timeout.Minutes, out sessionId);
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

				Funcionario funcionario = _busFunc.ObterFuncionarioAutenticacao(login);

				ePermissao[] arrayPerm = funcionario.Permissoes.Select(x => x.Codigo).ToArray();

				EtramiteIdentity userIndentity = new EtramiteIdentity(funcionario.Nome, funcionario.Usuario.Login, funcionario.Email,
					funcionario.Usuario.DataUltimoLogon, funcionario.Usuario.IpUltimoLogon, funcionario.Id, funcionario.Tipo, 
					funcionario.TipoTexto, funcionario.Tid, funcionario.Usuario.Id, (int)eExecutorTipo.Interno);

				EtramitePrincipal userPrincipal = new EtramitePrincipal<ePermissao>(userIndentity, arrayPerm);
				HttpContext.Current.User = userPrincipal;
			}

			if (HttpContext.Current.User.Identity.IsAuthenticated)
			{
				UsuarioBus busUsuario = new UsuarioBus(HistoricoAplicacao.INTERNO);
				busUsuario.SalvarDataUltimaAcao(login);
			}
		}

		internal static String Criptografar(string login, string senha)
		{
			string strTexto = login.ToLower() + "*" + senha;
			UTF8Encoding encoder = new UTF8Encoding();
			SHA512 sha512 = SHA512.Create();
			byte[] byteHash = sha512.ComputeHash(encoder.GetBytes(strTexto));

			return string.Join("", byteHash.Select(bin => bin.ToString("X2")).ToArray());
		}

		public static void Deslogar(string login = null, bool isDeslogarForcado = false)
		{
			if (login == null && HttpContext.Current.User == null && !(HttpContext.Current.User is EtramitePrincipal) )
			{
				return;
			}

			AutenticacaoExecutor executor = new AutenticacaoExecutor();
			executor.Tipo = (int)eExecutorTipo.Interno;//Executor Interno
			UsuarioBus busUsuario = new UsuarioBus(HistoricoAplicacao.INTERNO);

			if (HttpContext.Current.User != null && (HttpContext.Current.User is EtramitePrincipal))
			{
				EtramitePrincipal user = (HttpContext.Current.User as EtramitePrincipal);
				login = user.EtramiteIdentity.Login;
				executor.Tid = user.EtramiteIdentity.FuncionarioTid;
				executor.UsuarioId = user.EtramiteIdentity.UsuarioId;
			}
			else
			{
				Funcionario funcionario = _busFunc.ObterFuncionarioExecutor(login);				
				executor.Tid = funcionario.Tid;
				executor.UsuarioId = funcionario.Usuario.Id;
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				bancoDeDados.IniciarTransacao();
				busUsuario.Deslogar(login, executor, isDeslogarForcado, bancoDeDados);
				_busFunc.Deslogar(login, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		public static bool VerificarDeslogar(string login, string sessionId)
		{
			if (_busFunc.VerificarSeDeveDeslogar(login, sessionId, FormsAuthentication.Timeout.Minutes))
			{
				Validacao.Add(Mensagem.Login.AcessoSimultaneo);
				Deslogar(login);
				FormsAuthentication.SignOut();

				if (HttpContext.Current.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
				{
					FormsAuthentication.RedirectToLoginPage("msg=" + Validacao.QueryParam());
				}
				return true;
			}

			return false;
		}

		internal static AutenticacaoExecutor ObterAutenticacaoExecutor()
		{
			if (HttpContext.Current.User == null || !(HttpContext.Current.User is EtramitePrincipal) )
			{
				return null;
			}

			AutenticacaoExecutor executor = new AutenticacaoExecutor();

			EtramitePrincipal user = (HttpContext.Current.User as EtramitePrincipal);
			executor.Tipo = (int)eExecutorTipo.Interno;
			executor.UsuarioId = user.EtramiteIdentity.UsuarioId;
			executor.Tid = user.EtramiteIdentity.FuncionarioTid;

			return executor;
		}
	}
}

