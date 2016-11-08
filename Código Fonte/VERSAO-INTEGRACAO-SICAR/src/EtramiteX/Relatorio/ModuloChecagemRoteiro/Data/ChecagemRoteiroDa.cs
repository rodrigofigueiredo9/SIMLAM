using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRoteiro;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloChecagemRoteiro.Data
{
	class ChecagemRoteiroDa
	{
		internal ChecagemRoteiroRelatorio Obter(int id)
		{
			ChecagemRoteiroRelatorio checagem = new ChecagemRoteiroRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Checagem de item de roteiro

				Comando comando = bancoDeDados.CriarComando(@"select c.interessado, ls.id situacao_id, ls.texto situacao_texto, c.tid from tab_checagem c, 
				lov_checagem_situacao ls where c.situacao = ls.id and c.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						checagem.Id = id;
						checagem.Tid = reader["tid"].ToString();
						checagem.Interessado = reader["interessado"].ToString();
						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							checagem.Situacao = Convert.ToInt32(reader["situacao_id"]);
							checagem.SituacaoTexto = reader["situacao_texto"].ToString();
						}
					}
					reader.Close();
				}

				#endregion

				#region Checagem - Roteiros

				comando = bancoDeDados.CriarComando(@"select r.nome, r.versao from tab_checagem_roteiro c, tab_roteiro r where c.roteiro = r.id and c.checagem = :checagem");

				comando.AdicionarParametroEntrada("checagem", checagem.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						RoteiroRelatorio roteiro = new RoteiroRelatorio();

						roteiro.Nome = reader["nome"].ToString();
						roteiro.Versao = Convert.ToInt32(reader["versao"]);

						checagem.Roteiros.Add(roteiro);
					}
					reader.Close();
				}

				#endregion

				#region Checagem - Itens
				comando = bancoDeDados.CriarComando(@"
					select ri.nome, c.situacao, c.motivo, ri.condicionante from tab_checagem_itens c, tab_roteiro_item ri where c.item_id = ri.id and c.checagem = :checagem");

				comando.AdicionarParametroEntrada("checagem", checagem.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ItemRelatorio item;
					while (reader.Read())
					{
						item = new ItemRelatorio();
						item.SituacaoId = Convert.ToInt32(reader["situacao"]);
						item.Nome = reader["nome"].ToString();
						item.Condicionante = reader["condicionante"].ToString();
						item.Motivo = reader["motivo"].ToString();
						checagem.Itens.Add(item);
					}
					reader.Close();
				}
				#endregion
			}
			return checagem;
		}
	}
}