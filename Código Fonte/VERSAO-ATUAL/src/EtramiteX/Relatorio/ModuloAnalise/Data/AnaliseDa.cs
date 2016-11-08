using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloAnalise;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloAnalise.Data
{
	public class AnaliseDa
	{
		private string EsquemaBanco { get; set; }

		public AnaliseDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal int Existe(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}tab_analise t where t.protocolo = :id", EsquemaBanco);
				
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				object retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno != null && !Convert.IsDBNull(retorno)) ? Convert.ToInt32(retorno) : 0;
			}
		}

		internal AnaliseItemRelatorio Obter(int analiseId)
		{
			AnaliseItemRelatorio analise = new AnaliseItemRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Protocolo e Requerimento

				Comando comando = bancoDeDados.CriarComando(@"select ta.protocolo id, p.id interessado_id, nvl(p.nome, p.razao_social) interessado_nome, 
															nvl(p.cpf, p.cnpj) interessado_cpfcnpj, tp.numero || '/' || tp.ano numero, tp.protocolo, tp.setor, tp.checagem,
															lp.texto tipo, tr.autor, tr.numero requerimento_numero, to_char(tr.data_criacao, 'dd/mm/yyyy') requerimento_data
															from {0}tab_analise ta, {0}tab_protocolo tp, {0}lov_protocolo lp,
															{0}tab_requerimento tr, {0}tab_pessoa p where ta.protocolo = tp.id and tp.protocolo = lp.id
															and tp.requerimento = tr.id and p.id = tp.interessado and ta.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", analiseId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						analise.Id = reader["id"].ToString();

						if (reader["setor"] != null && !Convert.IsDBNull(reader["setor"]))
						{
							analise.SetorId = Convert.ToInt32(reader["setor"]);
						}

						analise.Numero = reader["numero"].ToString();
						analise.Tipo = reader["tipo"].ToString();
						analise.ChecagemNumero = reader["checagem"].ToString();

						analise.InteressadoId = Convert.ToInt32(reader["interessado_id"]);
						analise.InteressadoNome = reader["interessado_nome"].ToString();
						analise.InteressadoCPFCNPJ = reader["interessado_cpfcnpj"].ToString();

						analise.RequerimentoNumero = reader["requerimento_numero"].ToString();
						analise.RequerimentoData = reader["requerimento_data"].ToString();
						analise.RequerimentoOrigem = String.IsNullOrEmpty(reader["autor"].ToString())? "Institucional" : "Credenciado";

						analise.IsProcesso = Convert.ToInt32(reader["protocolo"]) == 1;
					}
					reader.Close();
				}

				#endregion

				if (!String.IsNullOrWhiteSpace(analise.Id))
				{
					#region Atividades do Requerimento

					comando = bancoDeDados.CriarComando(@"select tpa.atividade, lpas.texto situacao from {0}tab_protocolo_atividades tca,
					{0}tab_atividade tpa, {0}lov_atividade_situacao lpas where tca.atividade = tpa.id(+)
					and tca.situacao = lpas.id and tca.requerimento = :requerimento and tca.protocolo= :protocolo", EsquemaBanco);

					comando.AdicionarParametroEntrada("protocolo", analise.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("requerimento", analise.RequerimentoNumero, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							AtividadeRelatorio atividade = new AtividadeRelatorio();

							atividade.Nome = reader["atividade"].ToString();
							atividade.Situacao = reader["situacao"].ToString();

							analise.Atividades.Add(atividade);
						}
						reader.Close();
					}

					#endregion

					#region Roteiros

					comando = bancoDeDados.CriarComando(@"select hr.numero, hr.versao, hr.nome from {0}tab_analise_roteiro t, {0}hst_roteiro hr 
					where t.roteiro_tid = hr.tid and t.roteiro = hr.roteiro_id and t.analise = :analise", EsquemaBanco);

					comando.AdicionarParametroEntrada("analise", analiseId, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							RoteiroRelatorio roteiro = new RoteiroRelatorio();

							roteiro.Numero = Convert.ToInt32(reader["numero"]);
							roteiro.Versao = Convert.ToInt32(reader["versao"]);
							roteiro.Nome = reader["nome"].ToString();

							analise.Roteiros.Add(roteiro);
						}
						reader.Close();
					}
					#endregion

					#region Itens

					comando = bancoDeDados.CriarComando(@"select t.item_id, hri.tipo_id, hri.tipo_texto, hri.nome, hri.condicionante,
					to_char(t.data_analise, 'dd/mm/yyyy') data_analise, lais.texto situacao, t.situacao situacao_id,
					t.analista analista, t.descricao, t.motivo, s.nome setor_nome from {0}tab_analise_itens t,
					{0}hst_roteiro_item hri, {0}lov_analise_item_situacao lais, {0}tab_setor s where t.item_tid = hri.tid
					and t.situacao = lais.id and t.analise = :analise and s.id(+) = t.setor order by t.data_analise", EsquemaBanco);

					comando.AdicionarParametroEntrada("analise", analiseId, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{

						while (reader.Read())
						{
							ItemRelatorio item = new ItemRelatorio();
							item.Id = Convert.ToInt32(reader["item_id"]);
							item.Tipo = Convert.ToInt32(reader["tipo_id"]);
							item.TipoTexto = reader["tipo_texto"].ToString();
							item.Nome = reader["nome"].ToString();
							item.Condicionante = reader["condicionante"].ToString();
							item.DataAnalise = reader["data_analise"].ToString();
							item.Situacao = reader["situacao"].ToString();
							item.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
							item.Analista = reader["analista"].ToString();
							item.Descricao = reader["descricao"].ToString();
							item.Motivo = reader["motivo"].ToString();
							item.SetorNome = reader["setor_nome"].ToString();
							analise.Itens.Add(item);
						}
						reader.Close();
					}
					#endregion
				}
			}
			return analise;
		}
	}
}