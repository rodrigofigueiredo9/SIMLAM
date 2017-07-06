using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.Blocos.Autenticacao
{
	class UsuarioDa
	{
		Historico _historico = new Historico();
		private string EsquemaBanco { get; set; }
		public int Aplicacao { get; set; }

		public UsuarioDa(int aplicacao, string strBancoDeDados = null)
		{
			Aplicacao = aplicacao;

			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
				_historico = new Historico(EsquemaBanco);
			}
		}

		internal Int32 Salvar(Usuario usuario, String senhaHash, AutenticacaoExecutor executor, BancoDeDados banco = null)
		{
			if (banco == null)
			{
				GerenciadorTransacao.GerarNovoID();
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando("insert into {0}tab_usuario (id, login, senha, senha_data, tid) values ({0}seq_usuario.nextval, :login, :senha, sysdate, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("login", DbType.String, 30, usuario.Login);
				comando.AdicionarParametroEntrada("senha", DbType.String, 150, senhaHash);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				usuario.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				_historico.Gerar(usuario.Login, Aplicacao, HistoricoAcao.CRIAR, executor, bancoDeDados);

				bancoDeDados.Commit();
			}

			return usuario.Id;
		}

		internal Usuario Obter(String login, BancoDeDados banco = null)
		{
			Usuario usuario = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id, senha, senha_data, logon_data, logon_ip, tid from {0}tab_usuario t where t.login = :login", EsquemaBanco);
				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						usuario = new Usuario();
						usuario.Id = Convert.ToInt32(reader["id"]);
						usuario.TID = reader["tid"].ToString();
						usuario.Login = login;
						usuario.DataUltimaAlteracaoSenha = (!(reader["senha_data"] == DBNull.Value)) ? Convert.ToDateTime(reader["senha_data"]) : (DateTime?)(null);
						usuario.IpUltimoLogon = reader["logon_ip"].ToString();
						usuario.senhaHash = reader["senha"].ToString();
					}
					reader.Close();
				}
			}

			return usuario;
		}

		internal Int32 ObterLoginId(string login)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando("select t.id from {0}tab_usuario t where t.login = :login", EsquemaBanco);
				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

        internal Int32 ObterUsuarioIdCPF(String cpf, BancoDeDados banco = null)
        {
            int? idUsuario = 0;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select tu.id id_usuario
                                                              from tab_pessoa tp,
                                                                   tab_credenciado tc,
                                                                   tab_usuario tu,
                                                                   lov_credenciado_situacao ls
                                                              where tp.cpf = :cpf
                                                                    and tp.usuario = 1
                                                                    and tc.pessoa = tp.id
                                                                    and tu.id = tc.usuario
                                                                    and ls.id = tc.SITUACAO
                                                                    and (ls.texto = 'Ativo' or ls.texto = 'Senha vencida')", EsquemaBanco);
                
                comando.AdicionarParametroEntrada("cpf", DbType.String, 15, cpf);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        idUsuario = Convert.ToInt32(reader["id_usuario"]);
                    }
                    reader.Close();
                }
            }

            return idUsuario ?? 0;
        }

        internal Int32 ObterUsuarioIdEmail(String email, BancoDeDados banco = null)
        {
            int? idUsuario = 0;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select tu.id id_usuario
                                                              from tab_pessoa_meio_contato tpc,
                                                                   tab_meio_contato tmc,
                                                                   tab_pessoa tp,
                                                                   tab_credenciado tc,
                                                                   tab_usuario tu,
                                                                   lov_credenciado_situacao ls
                                                              where tmc.texto = 'Email'
                                                                    and tpc.meio_contato = tmc.id
                                                                    and tpc.valor = :email
                                                                    and tp.id = tpc.pessoa
                                                                    and tp.usuario = 1
                                                                    and tc.pessoa = tp.id
                                                                    and tu.id = tc.usuario
                                                                    and ls.id = tc.situacao
                                                                    and (ls.texto = 'Ativo' or ls.texto = 'Senha vencida')", EsquemaBanco);

                comando.AdicionarParametroEntrada("email", DbType.String, 100, email);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        idUsuario = Convert.ToInt32(reader["id_usuario"]);
                    }
                    reader.Close();
                }
            }

            return idUsuario ?? 0;
        }

		internal Usuario ValidarUsuario(string login, string senhaHash)
		{
			Usuario usuario = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando("select count(t.id) from {0}tab_usuario t where t.login = :login and t.senha = :senha", EsquemaBanco);

				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);
				comando.AdicionarParametroEntrada("senha", DbType.String, 150, senhaHash);

				if (Convert.ToInt32(bancoDeDados.ExecutarScalar<Decimal>(comando)) > 0)
				{
					comando = bancoDeDados.CriarComando(@"select id, senha_data, logon_data, logon_ip from {0}tab_usuario t where t.login = :login", EsquemaBanco);

					comando.AdicionarParametroEntrada("login", DbType.String, 30, login);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							usuario = new Usuario();
							usuario.Id = Convert.ToInt32(reader["id"]);
							usuario.Login = login;
							usuario.DataUltimaAlteracaoSenha = (!(reader["senha_data"] == DBNull.Value)) ? Convert.ToDateTime(reader["senha_data"]) : (DateTime?)(null);
							usuario.IpUltimoLogon = reader["logon_ip"].ToString();
						}
						reader.Close();
					}
				}
			}

			return usuario;
		}

		internal bool VerificarLoginExistente(string login, int usuarioId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando("select count(t.id) from {0}tab_usuario t where t.login = :login", EsquemaBanco);
				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);

				if (usuarioId > 0)
				{
					comando.DbCommand.CommandText += " and  t.id <> :usuarioId ";
					comando.AdicionarParametroEntrada("usuarioId", usuarioId, DbType.Int32);
				}
				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal void AlterarSenha(Usuario usuario, string senhaHash, AutenticacaoExecutor executor, BancoDeDados banco = null)
		{
			if (banco == null)
			{
				GerenciadorTransacao.GerarNovoID();
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_usuario u set senha = :senha, senha_data = sysdate, tid = :tid
					where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", usuario.Id, DbType.Int32);
				//Não se alterar Login
				comando.AdicionarParametroEntrada("senha", DbType.String, 150, senhaHash);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				
				bancoDeDados.ExecutarNonQuery(comando);

				_historico.Gerar(usuario.Login, Aplicacao, HistoricoAcao.ATUALIZAR, executor, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Autenticar(string login, int id, string ipLogon, AutenticacaoExecutor executor, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando("update {0}tab_usuario t set t.logon_data = sysdate, t.logon_ip = :logon_ip where t.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("logon_ip", DbType.String, 40, ipLogon);
				bancoDeDados.ExecutarNonQuery(comando);

				executor.UsuarioId = id;

				_historico.Gerar(login, Aplicacao, HistoricoAcao.LOGON, executor, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Deslogar(string login, AutenticacaoExecutor executor, bool deslogarForcado = false, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_usuario t set t.logout_data = ultima_acao, 
					t.logon_anterior_data = t.logon_data, t.logon_anterior_ip = t.logon_ip where t.login = :login returning id into :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				int id = Convert.ToInt32(comando.ObterValorParametro("id"));

				executor.UsuarioId = id;
				int acao = (deslogarForcado) ? HistoricoAcao.LOGOUT_FOCADO : HistoricoAcao.LOGOUT;

				_historico.Gerar(login, Aplicacao, acao, executor, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void SalvarDataUltimaAcao(string login, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComando("update {0}tab_usuario t set t.ultima_acao = sysdate where t.login = :login", EsquemaBanco);
				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal bool VerificarHistoricoSenha(int id, string senhaHash, int qtd, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ord.senha from 
					(select t.* from (
						(select distinct u.senha_data, u.senha 
						   from {0}hst_usuario u 
					      where u.usuario_id = :id and u.acao_executada = 7)) 
					t order by t.senha_data desc ) ord 
					where rownum <= :qtd", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("qtd", qtd, DbType.Int32);

				List<string> lstSenha = bancoDeDados.ExecutarList<String>(comando);

				return lstSenha.Any(x => senhaHash.Equals(x));
			}
		}
	}
}