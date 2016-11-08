using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloEmail;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.Blocos.Etx.ModuloEmail.Data
{
	class EmailDa
	{
		public String EsquemaBanco { get; set; }

		public EmailDa(string esquema = null)
		{
			if (esquema != null)
			{
				EsquemaBanco = esquema;
			}
		}

		public int Enviar(Email email, BancoDeDados banco=null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();
				
				#region Email

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_email (id, situacao, tipo, codigo, assunto, destinatario, texto, tentativas, data_fila, tid)
					values ({0}seq_email.nextval, 1, :tipo, :codigo, :assunto, :destinatario, :texto, 0, sysdate, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tipo", email.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("codigo", email.Codigo, DbType.Int32);
				comando.AdicionarParametroEntrada("assunto", DbType.String, 200, email.Assunto);
				comando.AdicionarParametroEntrada("destinatario", DbType.String, 200, email.Destinatario);
				comando.AdicionarParametroEntrada("texto", DbType.String, 4000, email.Texto);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				email.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				
				comando = bancoDeDados.CriarComando(@"insert into {0}tab_email_anexo (email, anexo) values (:email, :anexo)", EsquemaBanco);

				foreach (var anexo in email.Anexos)
				{
					if (anexo.Id < 0)
					{
						continue;
					}

					comando.AdicionarParametroEntrada("email", email.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("anexo", anexo.Id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);					
				}

				#endregion

				bancoDeDados.Commit();
			}

			return email.Id;
		}

		internal void Deletar(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Email

				Comando comando = bancoDeDados.CriarComando(@"delete {0}tab_email_anexo e where e.email = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"delete {0}tab_email e where e.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Deletar(eEmailTipo tipo, int codigo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"delete {0}tab_email_anexo a where a.email in (select e.id from {0}tab_email e where e.tipo = :tipo and e.codigo = :codigo)", EsquemaBanco);
				comando.AdicionarParametroEntrada("tipo", (int)tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("codigo", codigo, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				
				comando = bancoDeDados.CriarComando(@"delete {0}tab_email e where e.tipo = :tipo and e.codigo = :codigo", EsquemaBanco);
				comando.AdicionarParametroEntrada("tipo", (int)tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("codigo", codigo, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				
				
				bancoDeDados.Commit();
			}
		}
	}
}
