using System;
using Tecnomapas.Blocos.Autenticacao.ModuloAuditoria;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Autenticacao.ModuloAuditoria;

namespace Tecnomapas.Blocos.Autenticacao
{
	public class UsuarioBus
	{
		UsuarioDa _da = null;
		private string EsquemaBanco { get; set; }
		public int Aplicacao { get; set; }

		public UsuarioBus(int aplicacao, string strBancoDeDados = null)
		{
			Aplicacao = aplicacao;
			
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
				_da = new UsuarioDa(aplicacao, EsquemaBanco);
			}
			else
			{
				_da = new UsuarioDa(aplicacao);
			}
		}

		public Int32 Salvar(Usuario usuario, string senhaHash, AutenticacaoExecutor executor, BancoDeDados banco = null)
		{
			int ret = 0;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bool flagAuditavel = false;
				try
				{
					if (usuario == null)
						throw new ApplicationException("Objeto usuário é nulo");

					if (String.IsNullOrEmpty(senhaHash))
						throw new ApplicationException("senha de usuário é nula");

					if (String.IsNullOrEmpty(usuario.Login))
						throw new ApplicationException("login de usuário é nulo");

					if (executor.UsuarioId <= 0)
						throw new ApplicationException("Executor Id de usuário é zero");

					if (executor.Tipo <= 0)
						throw new ApplicationException("Executor Tipo é zero");

					if (String.IsNullOrEmpty(executor.Tid))
						throw new ApplicationException("Executor Tid é nulo");

					if (!UsuarioValidacao.FormatoLogin(usuario.Login))
						throw new ApplicationException("Formato do login é inválido");


					ret = _da.Salvar(usuario, senhaHash, executor, bancoDeDados);

					flagAuditavel = true;
				}
				finally
				{
					Auditoria.Evento(EventoAuditavel.Criar, usuario, flagAuditavel);
				}
			}

			return ret;
		}

		public void AlterarSenha(Usuario usuario, String senhaHash, AutenticacaoExecutor executor, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bool flagAuditavel = false;
				try
				{
					_da.AlterarSenha(usuario, senhaHash, executor, bancoDeDados);

					flagAuditavel = true;
				}
				finally
				{
					Auditoria.Evento(EventoAuditavel.AlterarSenha, usuario, flagAuditavel);
				}
			}
		}

		public Usuario ValidarUsuario(string login, String senhaHash)
		{
			Usuario usuario = _da.ValidarUsuario(login, senhaHash);
			senhaHash = string.Empty;
			return usuario;
		}

		public Usuario Obter(string login, BancoDeDados banco = null)
		{
			return _da.Obter(login, banco);
		}

        public Int32 ObterUsuarioIDEmailCPF(string email, string cpf, BancoDeDados banco = null)
        {
            int idUsuarioEmail = _da.ObterUsuarioIdEmail(email, banco);

            int idUsuarioCPF = _da.ObterUsuarioIdCPF(cpf, banco);

            if (idUsuarioCPF == idUsuarioEmail)
            {
                return idUsuarioCPF;
            }

            return 0;
        }

		public void Autenticar(Usuario usuario, bool ehUsuarioValido, int executorTipo, BancoDeDados banco = null)
		{
			try
			{
				if (ehUsuarioValido)
				{
					AutenticacaoExecutor executor = new AutenticacaoExecutor() { UsuarioId = usuario.Id, Tipo = executorTipo, Tid = usuario.TID };
					_da.Autenticar(usuario.Login, usuario.Id, usuario.Ip, executor, banco);
				}
			}
			finally
			{
				Auditoria.Evento((ehUsuarioValido) ? EventoAuditavel.Logon : EventoAuditavel.LogonFalha, usuario, ehUsuarioValido);
			}
		}

		public void SalvarDataUltimaAcao(string login, BancoDeDados banco = null)
		{
			_da.SalvarDataUltimaAcao(login, banco);
		}

		public void Deslogar(string login, AutenticacaoExecutor executor, bool deslogarForcado = false, BancoDeDados banco = null)
		{
			bool flagAuditavel = false;
			Usuario usuario = new Usuario();
			usuario.Login = login;			
			try
			{
				_da.Deslogar(login, executor, deslogarForcado, banco);
				flagAuditavel = true;
			}
			finally
			{
				Auditoria.Evento(EventoAuditavel.Logout, usuario, flagAuditavel);
			}
		}

		public bool VerificarLoginExistente(string login, int usuarioId)
		{
			return _da.VerificarLoginExistente(login, usuarioId);
		}

		public int ObterIdPorLogin(string login)
		{
			Usuario usuario = new Usuario();
			try
			{
				usuario.Login = login;
				return _da.ObterLoginId(login);
			}
			finally
			{
				Auditoria.Evento(EventoAuditavel.Buscar, usuario);
			}
		}

		public bool VerificarHistoricoSenha(int id, string senhaHash, int qtd)
		{
			return _da.VerificarHistoricoSenha(id, senhaHash, qtd);
		}
	}
}