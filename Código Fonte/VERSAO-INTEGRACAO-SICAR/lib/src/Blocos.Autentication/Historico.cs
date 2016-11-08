using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.Blocos.Autenticacao
{
	class Historico
	{
		private string EsquemaBanco { get; set; }

		public Historico(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		public void Gerar(string login, int aplicacao, int acao, AutenticacaoExecutor executor, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"begin {0}usuario.historico(:login, :acao, :executorId, :executorTipo, :executorTid, :p_aplicacao); end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);
				comando.AdicionarParametroEntrada("acao", acao, DbType.Int32);
				comando.AdicionarParametroEntrada("executorId", executor.UsuarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("executorTipo", executor.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("executorTid", DbType.String, 36, executor.Tid);
				comando.AdicionarParametroEntrada("p_aplicacao", aplicacao, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}

		}

		public void Gerar(string login, int aplicacao, int acao, string executorLogin, int executorTipo, string executorTid, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando("declare v_executorId number; "+
					"begin select u.id into v_executorId from {0}tab_usuario u where u.login = :executorLogin;"+
					"{0}usuario.historico(:login, :acao, :executorId, :executorTipo, :executorTid, :p_aplicacao); end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);
				comando.AdicionarParametroEntrada("acao", acao, DbType.Int32);
				comando.AdicionarParametroEntrada("executorLogin", DbType.String, 30, executorLogin);
				comando.AdicionarParametroEntrada("executorTipo", executorTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("executorTid", DbType.String, 36, executorTid);
				comando.AdicionarParametroEntrada("p_aplicacao", aplicacao, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}
	}
}