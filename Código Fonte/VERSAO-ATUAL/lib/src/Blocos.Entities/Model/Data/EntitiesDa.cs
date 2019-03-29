using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;

namespace Tecnomapas.Blocos.Entities.Model.Data
{
	class EntitiesDa
	{
		internal int ObterAtividadeId(int atividadeCodigo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id from tab_atividade a where a.codigo = :codigo");

				comando.AdicionarParametroEntrada("codigo", atividadeCodigo, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal List<Lista> ObterRegularizacaoFundiariaTipoLimite()
		{
			List<Lista> lst = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.texto from lov_crt_regularizacao_limite c order by c.texto");

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lst.Add(new Lista()
						{
							Id = reader["id"].ToString(),
							Texto = reader["texto"].ToString(),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}

			return lst;
		}

		internal List<RelacaoTrabalho> ObterRegularizacaoFundiariaRelacaoTrabalho()
		{
			List<RelacaoTrabalho> lst = new List<RelacaoTrabalho>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.texto, c.codigo from lov_crt_regularizacao_rel_tab c order by c.texto");

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lst.Add(new RelacaoTrabalho()
						{
							Id = Convert.ToInt32(reader["id"]),
							Texto = reader["texto"].ToString(),
							Codigo = Convert.ToInt32(reader["codigo"]),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}

			return lst;
		}

		internal List<Finalidade> ObterFinalidadesExploracao()
		{
			List<Finalidade> lst = new List<Finalidade>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.texto, t.codigo from lov_crt_exp_flores_finalidade t order by t.texto");

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lst.Add(new Finalidade()
						{
							Id = Convert.ToInt32(reader["id"]),
							Texto = reader["texto"].ToString(),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}

			return lst;
		}

	}
}