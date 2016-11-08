using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.WindowsService.SVCEmail.Entities;
using Tecnomapas.EtramiteX.WindowsService.SVCEmail.Properties;

namespace Tecnomapas.EtramiteX.WindowsService.SVCEmail.Data
{
	class EmailDa
	{
		public String EsquemaBanco { get; set; }

		public EmailDa(string esquema = null)
		{
			EsquemaBanco = esquema;
		}

		internal ConfiguracaoEmail ObterConfiguracao()
		{
			ConfiguracaoEmail config = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Email

				Comando comando = bancoDeDados.CriarComando(@"select c.servidor_smtp, c.usuario_smtp, c.porta, c.ssl, c.remetente, c.senha, c.num_tentativa from cnf_email c");

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						config = new ConfiguracaoEmail();
						config.SmtpServer = reader["servidor_smtp"].ToString();
						config.SmtpUser = reader["usuario_smtp"].ToString();
						config.Remetente = reader["remetente"].ToString();
						config.SmtpSenha = reader["senha"].ToString();
						config.NumeroTentativaEnvio = Convert.ToInt32(reader["num_tentativa"]);
						config.Porta = ((Convert.IsDBNull(reader["porta"]) || reader["porta"] == null) ? 25 : Convert.ToInt32(reader["porta"]));
						config.EnableSsl = ((Convert.IsDBNull(reader["ssl"]) || reader["ssl"] == null) ? false : Convert.ToBoolean(reader["ssl"]));
					}

					reader.Close();
				}

				#endregion
			}

			return config;
		}

		internal Email ObterEmail(int id, BancoDeDados banco = null)
		{
			Email email = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select e.situacao, e.tipo, e.codigo, e.assunto, e.destinatario, e.texto, e.tentativas from tab_email e where e.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						email = new Email();
						email.Id = id;
						email.Situacao = Convert.ToInt32(reader["situacao"]);
						email.Tipo = Convert.ToInt32(reader["tipo"]);
						email.Codigo = Convert.ToInt32(reader["codigo"]);
						email.Assunto = reader["assunto"].ToString();
						email.Destinatario = reader["destinatario"].ToString();
						email.Texto = reader["texto"].ToString();
						email.NumTentativas = Convert.ToInt32(reader["tentativas"]);
					}

					reader.Close();
				}

				if (email == null)
				{
					return email;
				}

				comando = bancoDeDados.CriarComando(@"select a.anexo from tab_email_anexo a where a.email = :email");
				comando.AdicionarParametroEntrada("email", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						email.Anexos.Add(new Blocos.Arquivo.Arquivo()
						{
							Id = Convert.ToInt32(reader["anexo"])
						});
					}

					reader.Close();
				}
			}

			return email;
		}

		internal Email ObterEmailDeEnvio(int numTentativas)
		{
			Email email = null;
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				comando = bancoDeDados.CriarComando(@"select * from (
				select te.id from tab_email te where te.data_fila <= sysdate and te.situacao = 1 and te.tentativas <= :numeroTent order by te.data_fila) 
				where rownum = 1");

				comando.AdicionarParametroEntrada("numeroTent", numTentativas, DbType.Int32);

				int id = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				if (id > 0)
				{
					comando = bancoDeDados.CriarComando(@"update tab_email t 
						set t.tentativas = t.tentativas+1 where t.id = :email");

					comando.AdicionarParametroEntrada("email", id, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);

					email = ObterEmail(id, bancoDeDados);
				}
			}

			return email;
		}

		internal void AlterarSituacao(int id, int situacao)
		{
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				comando = bancoDeDados.CriarComando(@"update tab_email t set t.situacao = :situacao where t.id = :email");

				comando.AdicionarParametroEntrada("email", id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
			}
		}

		internal void FimDaFila(int id, ConfiguracaoEmail configEmail)
		{
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				comando = bancoDeDados.CriarComando(@"update tab_email t set t.data_fila = sysdate + numtodsinterval(:CheckIntervalMinuto, 'MINUTE') where t.id = :email");

				comando.AdicionarParametroEntrada("email", id, DbType.Int32);
				comando.AdicionarParametroEntrada("CheckIntervalMinuto", Settings.Default.CheckIntervalMinuto, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
			}
		}
	}
}