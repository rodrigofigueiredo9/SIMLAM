using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRoteiro.Data
{
	public class RoteiroDa : IRoteiroDa
	{
		private string EsquemaBanco { get; set; }

		public RoteiroDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		public RoteiroRelatorio Obter(int id)
		{
			RoteiroRelatorio roteiro = new RoteiroRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Roteiro

				Comando comando = bancoDeDados.CriarComando(@"
												select a.numero,
														a.versao,
														a.nome,
														a.observacoes,
														a.setor setor_id,
														ts.nome setor_texto
													from {0}tab_roteiro a, {0}tab_setor ts
													where a.setor = ts.id
													and a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						roteiro.Numero = Convert.ToInt32(reader["numero"]);
						roteiro.Versao = Convert.ToInt32(reader["versao"]);
						roteiro.Nome = reader["nome"].ToString();
						roteiro.SetorId = Convert.ToInt32(reader["setor_id"]);
						roteiro.SetorTexto = reader["setor_texto"].ToString();
						roteiro.Observacoes = reader["observacoes"].ToString();
					}
					reader.Close();
				}

				#endregion

				#region Itens do roteiro

				comando = bancoDeDados.CriarComando(@"
											select tri.nome, tri.condicionante, lrit.id tipo, tris.ordem
											  from {0}tab_roteiro_itens     tris,
												   {0}tab_roteiro_item      tri,
												   {0}lov_roteiro_item_tipo lrit
											 where tris.item = tri.id
											   and tri.tipo = lrit.id
											   and tris.roteiro = :roteiro
											 order by tris.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("roteiro", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ItemRelatorio item;
					while (reader.Read())
					{
						item = new ItemRelatorio();
						item.Nome = reader["nome"].ToString();
						item.Condicionante = reader["condicionante"].ToString();
						item.Tipo = Convert.ToInt32(reader["tipo"]);
						item.Ordem = Convert.ToInt32(reader["ordem"]);
						roteiro.Itens.Add(item);
					}
					reader.Close();
				}

				#endregion

				#region Arquivos do roteiro

				comando = bancoDeDados.CriarComando(@"
											select a.descricao, b.nome, b.extensao, b.id arquivo_id, b.caminho
												from {0}tab_roteiro_arquivo a, {0}tab_arquivo b
												where a.arquivo = b.id
												and a.roteiro = :roteiro
												order by a.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("roteiro", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					AnexoRelatorio item;
					while (reader.Read())
					{
						item = new AnexoRelatorio();
						item.Descricao = reader["descricao"].ToString();

						item.Arquivo.Id = Convert.ToInt32(reader["arquivo_id"]);
						item.Arquivo.Caminho = reader["caminho"].ToString();
						item.Arquivo.Nome = reader["nome"].ToString();
						item.Arquivo.Extensao = reader["extensao"].ToString();

						roteiro.Anexos.Add(item);
					}
					reader.Close();
				}

				#endregion
			}
			return roteiro;
		}

		public RoteiroRelatorio Obter(int id, string tid)
		{
			RoteiroRelatorio roteiro = new RoteiroRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Roteiro

				int historicoRoteiroId = 0;
				Comando comando = bancoDeDados.CriarComando(@"
											select a.id,
													a.numero,
													a.versao,
													a.nome,
													a.observacoes,
													a.setor_id,
													ts.nome setor_texto
												from {0}hst_roteiro a, {0}tab_setor ts
												where a.setor_id = ts.id
												and a.roteiro_id = :id
												and a.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						historicoRoteiroId = Convert.ToInt32(reader["id"]);
						roteiro.Numero = Convert.ToInt32(reader["numero"]);
						roteiro.Versao = Convert.ToInt32(reader["versao"]);
						roteiro.Nome = reader["nome"].ToString();
						roteiro.Observacoes = reader["observacoes"].ToString();
						roteiro.SetorId = Convert.ToInt32(reader["setor_id"]);
						roteiro.SetorTexto = reader["setor_texto"].ToString();
					}
					reader.Close();
				}

				#endregion

				#region Itens do roteiro

				comando = bancoDeDados.CriarComando(@" 
									select tr.nome, tr.condicionante, tr.tipo_id, t.ordem
									   from {0}hst_roteiro_itens t, {0}hst_roteiro_item tr
									  where t.item_id = tr.item_id
										and t.item_tid = tr.tid
										and t.id_hst = :historico_id
									  order by t.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("historico_id", historicoRoteiroId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ItemRelatorio item;
					while (reader.Read())
					{
						item = new ItemRelatorio();

						item.Nome = reader["nome"].ToString();
						item.Condicionante = reader["condicionante"].ToString();
						item.Tipo = Convert.ToInt32(reader["tipo_id"]);
						item.Ordem = Convert.ToInt32(reader["ordem"]);

						roteiro.Itens.Add(item);
					}
					reader.Close();
				}

				#endregion

				#region Arquivos do roteiro

				comando = bancoDeDados.CriarComando(@"
									select t.descricao, ha.arquivo_id, ha.caminho, ha.nome, ha.extensao
									  from {0}hst_roteiro_arquivo t, {0}hst_arquivo ha
									 where t.arquivo_tid = ha.tid
									   and t.id_hst = :historico_id
									 order by t.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("historico_id", historicoRoteiroId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					AnexoRelatorio item;
					while (reader.Read())
					{
						item = new AnexoRelatorio();

						item.Descricao = reader["descricao"].ToString();
						item.Arquivo.Id = Convert.ToInt32(reader["arquivo_id"]);
						item.Arquivo.Caminho = reader["caminho"].ToString();
						item.Arquivo.Nome = reader["nome"].ToString();
						item.Arquivo.Extensao = reader["extensao"].ToString();

						roteiro.Anexos.Add(item);
					}
					reader.Close();
				}

				#endregion
			}
			return roteiro;
		}
	}
}