using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Gerencial.Model.ModuloFuncionario.Data
{
	class FuncionarioGerencialDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		public Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion Propriedades

		public FuncionarioGerencialDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region DMLs

		internal void Autenticar(Funcionario func, string sessionIdForcarLogoff)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				bancoDeDados.IniciarTransacao();
				try
				{
					Comando comando = bancoDeDados.CriarComando(@"update tab_funcionario f set f.tentativa = 0, logado = 1, f.session_Id = :sessionId where f.id = :id");

					comando.AdicionarParametroEntrada("id", func.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("sessionId", func.SessionId, DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);

					if (!String.IsNullOrEmpty(sessionIdForcarLogoff))
					{
						comando = bancoDeDados.CriarComando(@"insert into tab_funcionario_deslogar (id, login, session_Id, data_acao) 
							values (seq_funcionario_deslogar.nextval, :login, :sessionForcaLogoff, sysdate)");

						comando.AdicionarParametroEntrada("login", func.Usuario.Login, DbType.String);
						comando.AdicionarParametroEntrada("sessionForcaLogoff", sessionIdForcarLogoff, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
					bancoDeDados.Commit();
				}
				catch
				{
					bancoDeDados.Rollback();
					throw;
				}
			}
		}

		internal void AlterarSenha(int usuarioId, Funcionario executor = null, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_funcionario f set f.situacao = 1, f.tid = :tid where f.usuario = :usuario");
				comando.AdicionarParametroEntrada("usuario", usuarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				comando.DbCommand.Parameters.Clear();
				comando.DbCommand.CommandText = @"select f.id from tab_funcionario f where f.usuario = :usuario";
				comando.AdicionarParametroEntrada("usuario", usuarioId, DbType.Int32);
				int idFunc = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				Historico.Gerar(idFunc, eHistoricoArtefato.funcionario, eHistoricoAcao.alterarsenha, bancoDeDados, executor != null ? executor.Executor() : null);
				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(int id, int situacao, string motivo = null, Funcionario executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				GerenciadorTransacao.ObterIDAtual();
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_funcionario f set f.situacao = :situacao, f.situacao_motivo = :situacao_motivo, 
				f.tentativa = 0, f.tid = :tid where f.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_motivo", DbType.String, 100, motivo);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);
				Historico.Gerar(id, eHistoricoArtefato.funcionario, eHistoricoAcao.alterarsituacao, bancoDeDados, executor != null ? executor.Executor() : null);
				bancoDeDados.Commit();
			}
		}

		internal void Deslogar(string login, BancoDeDados banco = null)
		{

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_funcionario f set f.logado = 0 where f.usuario = (select u.id from tab_usuario u where u.login = :login)");
				comando.AdicionarParametroEntrada("login", login, DbType.String);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();
			}
		}

		#endregion DMLs

		#region Obter

		internal int ObterSituacao(string login)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.situacao from tab_funcionario f, tab_usuario u where f.usuario = u.id and u.login = :login");
				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);
				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal List<Situacao> ObterSituacaoFuncionario()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select s.id, s.texto nome from lov_funcionario_situacao s");
			foreach (var item in daReader)
			{
				lst.Add(new Situacao()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["nome"].ToString(),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal Funcionario ObterFuncionarioLogin(int usuarioId, int timeout)
		{
			Funcionario funcionario = new Funcionario();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"update tab_funcionario f set f.tentativa = (nvl(f.tentativa,0)+1) where f.usuario = :usuarioId");
				comando.AdicionarParametroEntrada("usuarioId", usuarioId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"select f.id, f.nome, f.tipo, f.situacao, f.tentativa, f.logado, f.session_id, 
					(case when (u.ultima_acao+NUMTODSINTERVAL(:timeout, 'MINUTE'))  > sysdate then 1 else 0 end) forcar_logout 
					from tab_funcionario f, tab_usuario u where f.usuario = u.id and u.id = :usuarioId");
				comando.AdicionarParametroEntrada("usuarioId", usuarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("timeout", timeout, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						funcionario.Id = Convert.ToInt32(reader["id"]);
						funcionario.Usuario.Id = usuarioId;
						funcionario.Situacao = Convert.ToInt32(reader["situacao"]);
						funcionario.Tentativa = Convert.ToInt32((reader["tentativa"] != DBNull.Value) ? reader["tentativa"] : 0);
						funcionario.SessionId = ((reader["session_id"] != DBNull.Value) ? reader["session_id"].ToString() : string.Empty);
						funcionario.Logado = ((reader["logado"] != DBNull.Value) ? Convert.ToBoolean(reader["logado"]) : false);
						funcionario.ForcarLogout = ((reader["forcar_logout"] != DBNull.Value) ? Convert.ToBoolean(reader["forcar_logout"]) : false);
						funcionario.Nome = reader["nome"].ToString();
						funcionario.Tipo = Convert.ToInt32(reader["tipo"]);
					}

					reader.Close();
				}
			}

			return funcionario;
		}

		internal Funcionario ObterFuncionarioExecutor(string login)
		{
			Funcionario funcionario = new Funcionario();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				comando = bancoDeDados.CriarComando(@"select f.id, f.tid, f.usuario, f.nome, f.tipo, u.login, f.email, u.logon_anterior_ip, u.logon_anterior_data, 
					t.texto tipo_texto from tab_funcionario f, tab_usuario u, lov_funcionario_tipo t where u.login = :login and f.usuario = u.id 
					and f.tipo = t.id");

				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						funcionario.Id = Convert.ToInt32(reader["id"]);
						funcionario.Usuario.Id = Convert.ToInt32(reader["usuario"]);
						funcionario.Usuario.Login = reader["login"].ToString();
						funcionario.Usuario.IpUltimoLogon = reader["logon_anterior_ip"].ToString();
						funcionario.Usuario.DataUltimoLogon = (reader["logon_anterior_data"] != DBNull.Value) ? Convert.ToDateTime(reader["logon_anterior_data"]) : new DateTime?();
						funcionario.Nome = reader["nome"].ToString();
						funcionario.Tipo = Convert.ToInt32(reader["tipo"]);
						funcionario.TipoTexto = reader["tipo_texto"].ToString();
						funcionario.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}
			}

			return funcionario;
		}

		internal Funcionario ObterFuncionarioAutenticacao(string login, List<String> permissoesGerencial)
		{
			Funcionario funcionario = new Funcionario();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				comando = bancoDeDados.CriarComando(@"select f.id, f.usuario, f.nome, f.tipo, u.login, f.email, u.logon_anterior_ip, u.logon_anterior_data, 
					t.texto tipo_texto, f.tid from tab_funcionario f, tab_usuario u, lov_funcionario_tipo t where u.login = :login and f.usuario = u.id 
					and f.situacao = 1 and f.tipo = t.id");

				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						funcionario.Id = Convert.ToInt32(reader["id"]);
						funcionario.Usuario.Id = Convert.ToInt32(reader["usuario"]);
						funcionario.Usuario.Login = reader["login"].ToString();
						funcionario.Usuario.IpUltimoLogon = reader["logon_anterior_ip"].ToString();
						funcionario.Usuario.DataUltimoLogon = (reader["logon_anterior_data"] != DBNull.Value) ? Convert.ToDateTime(reader["logon_anterior_data"]) : new DateTime?();
						funcionario.Nome = reader["nome"].ToString();
						funcionario.Email = reader["email"].ToString();
						funcionario.Tipo = Convert.ToInt32(reader["tipo"]);
						funcionario.TipoTexto = reader["tipo_texto"].ToString();
						funcionario.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}

				#region Papel / Permissao

				comando = bancoDeDados.CriarComando(@"select da.codigo from (
				select p.codigo from tab_funcionario_papel f, tab_autenticacao_papel_perm app, lov_autenticacao_permissao p 
				where f.funcionario = :id and f.papel = app.papel and app.permissao = p.id 
				union
				select p.codigo from tab_funcionario_permissao t, lov_autenticacao_permissao p where t.permissao = p.id and t.funcionario = :id) da");

				comando.AdicionarParametroEntrada("id", funcionario.Id, DbType.Int32);
				comando.DbCommand.CommandText += comando.AdicionarIn("where", "da.codigo", DbType.String, permissoesGerencial);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						if (!Enum.IsDefined(typeof(ePermissao), reader["codigo"].ToString()))
						{
							Validacao.Add(new Mensagem() { Tipo = eTipoMensagem.Erro, Texto = String.Format("Permissão \"{0}\" não definida! [Verifique Enumerado e commite !]", reader["codigo"].ToString()) });
							continue;
						}

						funcionario.Permissoes.Add(new Permissao()
						{
							Codigo = (ePermissao)Enum.Parse(typeof(ePermissao), reader["codigo"].ToString())
						});
					}

					reader.Close();
				}

				#endregion
			}

			return funcionario;
		}

		#endregion Obter

		#region Verificar

		internal bool VerificarSenhaVencida(int usuarioId, Funcionario executor = null)
		{
			bool vencido = false;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				bancoDeDados.IniciarTransacao();
				GerenciadorTransacao.ObterIDAtual();

				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_usuario u, tab_funcionario f, 
				(select to_number(c.valor) dias from cnf_sistema c where c.campo = 'validadesenha') prazo
				where u.id = f.usuario and f.situacao <> 6 and u.id = :usuarioId and trunc(sysdate) > trunc(u.senha_data+(prazo.dias))");

				comando.AdicionarParametroEntrada("usuarioId", usuarioId, DbType.Int32);

				vencido = bancoDeDados.ExecutarScalar(comando).ToString() != "0";

				if (vencido)
				{
					comando.DbCommand.CommandText = @"update tab_funcionario f set f.situacao = 6 where f.usuario = :usuarioId"; //Vencida
					bancoDeDados.ExecutarNonQuery(comando);

					comando.DbCommand.CommandText = @"select f.id from tab_funcionario f where f.usuario = :usuarioId";

					int idFunc = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

					Historico.Gerar(idFunc, eHistoricoArtefato.funcionario, eHistoricoAcao.atualizar, bancoDeDados, executor != null ? executor.Executor() : null);
				}

				bancoDeDados.Commit();
			}

			return vencido;
		}

		internal bool VerificarSeDeveDeslogar(String login, String sessionId, int timeout)
		{
			Comando comando = null;
			bool forcarLogoff = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				if (!String.IsNullOrEmpty(sessionId))
				{
					comando = bancoDeDados.CriarComando(@"select t.id from tab_funcionario_deslogar t where t.login = :login and t.session_Id = :session_Id
					union
					select f.id from tab_funcionario f, tab_usuario u where f.usuario = u.id and u.login = :login and f.session_id <> :session_Id");

					comando.AdicionarParametroEntrada("login", DbType.String, 30, login);
					comando.AdicionarParametroEntrada("session_Id", DbType.String, 36, sessionId);

					int idLogoff = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
					forcarLogoff = (idLogoff != 0);

					if (forcarLogoff)
					{
						comando = bancoDeDados.CriarComando(@"delete from tab_funcionario_deslogar t where t.id = :id or (t.data_acao+NUMTODSINTERVAL(:timeout, 'MINUTE')) < sysdate");
						comando.AdicionarParametroEntrada("id", idLogoff, DbType.Int32);
						comando.AdicionarParametroEntrada("timeout", timeout, DbType.Int32);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}
			}

			return forcarLogoff;
		}

		#endregion Verificar
	}
}