using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Data
{
	public class ConsideracoesFinaisDa
	{
		#region Propriedade e Atributos

		private String EsquemaBanco { get; set; }

		#endregion

		public ConsideracoesFinaisDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter
		
		public ConsideracoesFinaisRelatorio Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
			ConsideracoesFinaisRelatorio objeto = new ConsideracoesFinaisRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				comando = bancoDeDados.CriarComando(@" select c.id Id,
                                                              c.justificar JustificativaValorPenalidade,
                                                              c.descrever DescricaoInfracao,
                                                              (case c.tem_reparacao when 1 then 'Sim' when 0 then 'Não' end) IsReparacao,
                                                              c.reparacao OpniaoFormaReparacao,
                                                              (case when c.tem_reparacao = 0 then c.reparacao end) ReparacaoJustificativa,
                                                              (case c.tem_termo_comp when 1 then 'Sim' when 0 then 'Não' end) IsTermoCompromisso,
                                                              c.tem_termo_comp_justificar TermoCompromissoJustificativa
                                                       from {0}tab_fisc_consid_final c
                                                       where c.fiscalizacao = :fiscalizacaoId ", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);

				objeto = bancoDeDados.ObterEntity<ConsideracoesFinaisRelatorio>(comando);


				objeto.Assinantes = ObterAssinantes(objeto.Id, bancoDeDados);

				objeto.Anexos = ObterAnexos(objeto.Id, bancoDeDados);
			}

			return objeto;
		}

		public ConsideracoesFinaisRelatorio ObterHistorico(int historicoId, BancoDeDados banco = null)
		{
			ConsideracoesFinaisRelatorio objeto = new ConsideracoesFinaisRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				comando = bancoDeDados.CriarComando(@" select c.id HistoricoId, c.consid_final_id Id, c.justificar JustificativaValorPenalidade, c.descrever DescricaoInfracao, 
					 (case c.tem_reparacao when 1 then 'Sim' when 0 then 'Não' end)
					 IsReparacao, c.reparacao OpniaoFormaReparacao, 
					 (case when c.tem_reparacao = 0 then c.reparacao end) ReparacaoJustificativa,
					 (case c.tem_termo_comp when 1 then 'Sim' when 0 then 'Não' end) IsTermoCompromisso, 
					 c.tem_termo_comp_justificar TermoCompromissoJustificativa 
					 from hst_fisc_consid_final c 
					 where c.id_hst = :historicoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				objeto = bancoDeDados.ObterEntity<ConsideracoesFinaisRelatorio>(comando);


				objeto.Assinantes = ObterAssinantesHistorico(objeto.HistoricoId, bancoDeDados);

				objeto.Anexos = ObterAnexosHistorico(objeto.HistoricoId, bancoDeDados);
			}

			return objeto;
		}

		private List<AssinanteDefault> ObterAssinantes(int consideracoesFinaisId, BancoDeDados banco = null)
		{
			List<AssinanteDefault> colecao = new List<AssinanteDefault>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.id, f.nome, c.nome cargo 
						from {0}tab_fisc_consid_final_ass a, {0}tab_funcionario f, {0}tab_cargo c 
						where a.funcionario = f.id and a.cargo = c.id and a.consid_final = :consid_final order by f.nome", EsquemaBanco);

				comando.AdicionarParametroEntrada("consid_final", consideracoesFinaisId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					AssinanteDefault item;
					while (reader.Read())
					{
						item = new AssinanteDefault();
						item.Id = reader.GetValue<Int32>("id");
						item.Nome = reader.GetValue<String>("nome");
						item.Cargo = reader.GetValue<String>("cargo");
						colecao.Add(item);
					}

					reader.Close();
				}
			}

			return colecao;
		}

		private List<AssinanteDefault> ObterAssinantesHistorico(int historicoConsideracoesFinaisId, BancoDeDados banco = null)
		{
			List<AssinanteDefault> colecao = new List<AssinanteDefault>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select max(f.id) id, f.nome, c.nome cargo 
					from {0}hst_fisc_consid_final_ass a, {0}hst_funcionario f, {0}tab_cargo c 
					where a.funcionario_id = f.funcionario_id
					and a.funcionario_tid = f.tid
					and a.cargo_id = c.id
					and a.id_hst = :consid_final 
					group by f.nome, c.nome
					order by f.nome", EsquemaBanco);

				comando.AdicionarParametroEntrada("consid_final", historicoConsideracoesFinaisId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					AssinanteDefault item;
					while (reader.Read())
					{
						item = new AssinanteDefault();
						item.Id = reader.GetValue<Int32>("id");
						item.Nome = reader.GetValue<String>("nome");
						item.Cargo = reader.GetValue<String>("cargo");
						colecao.Add(item);
					}

					reader.Close();
				}
			}

			return colecao;
		}

		private List<ConsideracoesFinaisAnexoRelatorio> ObterAnexos(int consideracoesFinaisId, BancoDeDados banco = null)
		{
			List<ConsideracoesFinaisAnexoRelatorio> colecao = new List<ConsideracoesFinaisAnexoRelatorio>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id, a.descricao, b.nome, b.extensao, b.id arquivo_id, b.caminho, a.tid 
				from {0}tab_fisc_consid_final_arq a, {0}tab_arquivo b where a.arquivo = b.id and a.consid_final = :consideracoesFinaisId order by a.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("consideracoesFinaisId", consideracoesFinaisId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ConsideracoesFinaisAnexoRelatorio item;
					while (reader.Read())
					{
						item = new ConsideracoesFinaisAnexoRelatorio();
						item.Descricao = reader["descricao"].ToString();
						item.Arquivo.Id = Convert.ToInt32(reader["arquivo_id"]);
						item.Arquivo.Caminho = reader["caminho"].ToString();
						item.Arquivo.Nome = reader["nome"].ToString();
						item.Arquivo.Extensao = reader["extensao"].ToString();
						colecao.Add(item);
					}

					reader.Close();
				}
			}

			return colecao;
		}

		private List<ConsideracoesFinaisAnexoRelatorio> ObterAnexosHistorico(int historicoConsideracoesFinaisId, BancoDeDados banco = null)
		{
			List<ConsideracoesFinaisAnexoRelatorio> colecao = new List<ConsideracoesFinaisAnexoRelatorio>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id, a.descricao, b.nome, b.extensao, b.id arquivo_id, b.caminho, a.tid 
					from {0}hst_fisc_consid_final_arq a, {0}tab_arquivo b 
					where a.arquivo_id = b.id 
					and a.arquivo_tid = b.tid
					and a.id_hst = :consideracoesFinaisId 
					order by a.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("consideracoesFinaisId", historicoConsideracoesFinaisId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ConsideracoesFinaisAnexoRelatorio item;
					while (reader.Read())
					{
						item = new ConsideracoesFinaisAnexoRelatorio();
						item.Descricao = reader["descricao"].ToString();
						item.Arquivo.Id = Convert.ToInt32(reader["arquivo_id"]);
						item.Arquivo.Caminho = reader["caminho"].ToString();
						item.Arquivo.Nome = reader["nome"].ToString();
						item.Arquivo.Extensao = reader["extensao"].ToString();
						colecao.Add(item);
					}

					reader.Close();
				}
			}

			return colecao;
		}

		#endregion
	}
}
