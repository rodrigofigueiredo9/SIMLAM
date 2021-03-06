﻿using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{
	public class ObjetoInfracaoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private string EsquemaBanco { get; set; }

		#endregion

		public ObjetoInfracaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		public ObjetoInfracao Salvar(ObjetoInfracao objetoInfracao, BancoDeDados banco = null)
		{
			if (objetoInfracao == null)
			{
				throw new Exception("Objeto Infração é nulo.");
			}

			if (objetoInfracao.Id <= 0) 
			{
				objetoInfracao = Criar(objetoInfracao, banco);
			}
			else
			{
				objetoInfracao = Editar(objetoInfracao, banco);
			}

			return objetoInfracao;
		}

		internal ObjetoInfracao Criar(ObjetoInfracao objetoInfracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Objeto de Infracao

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into tab_fisc_obj_infracao i(id, fiscalizacao, area_embargada_atv_intermed, 
															tei_gerado_pelo_sist, tei_gerado_pelo_sist_serie, num_tei_bloco, data_lavratura_termo, 
															opniao_area_danificada, desc_termo_embargo, existe_atv_area_degrad, existe_atv_area_degrad_especif, fundament_infracao,
															uso_solo_area_danif, declividade_media_area,infracao_result_erosao, caract_solo_area_danif, arquivo, tid, infr_result_er_especifique) values({0}seq_tab_fisc_obj_infracao.nextval, 
															:fiscalizacao, :area_embargada_atv_intermed, :tei_gerado_pelo_sist, :tei_gerado_pelo_sist_serie, 
															:num_tei_bloco, :data_lavratura_termo, :opniao_area_danificada, :desc_termo_embargo, 
															:existe_atv_area_degrad, :existe_atv_area_degrad_especif, :fundament_infracao, :uso_solo_area_danif, :declividade_media_area,
															:infracao_result_erosao, :caract_solo_area_danif, :arquivo, :tid, :infr_result_er_especifique) returning i.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", objetoInfracao.FiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("area_embargada_atv_intermed", objetoInfracao.AreaEmbargadaAtvIntermed, DbType.Int32);
				comando.AdicionarParametroEntrada("tei_gerado_pelo_sist", objetoInfracao.TeiGeradoPeloSistema, DbType.Int32);
				comando.AdicionarParametroEntrada("tei_gerado_pelo_sist_serie", objetoInfracao.TeiGeradoPeloSistemaSerieTipo == 0 ? (Object)DBNull.Value : objetoInfracao.TeiGeradoPeloSistemaSerieTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("num_tei_bloco", String.IsNullOrWhiteSpace(objetoInfracao.NumTeiBloco) ? (Object)DBNull.Value : objetoInfracao.NumTeiBloco, DbType.Int32);
				comando.AdicionarParametroEntrada("data_lavratura_termo",  String.IsNullOrWhiteSpace(objetoInfracao.DataLavraturaTermo.DataTexto) ? (Object)DBNull.Value : objetoInfracao.DataLavraturaTermo.DataTexto, DbType.Date);
				comando.AdicionarParametroEntrada("opniao_area_danificada", String.IsNullOrWhiteSpace(objetoInfracao.OpniaoAreaDanificada) ? (Object)DBNull.Value : objetoInfracao.OpniaoAreaDanificada, DbType.String);
				comando.AdicionarParametroEntrada("desc_termo_embargo", String.IsNullOrWhiteSpace(objetoInfracao.DescricaoTermoEmbargo) ? (Object)DBNull.Value : objetoInfracao.DescricaoTermoEmbargo, DbType.String);
				comando.AdicionarParametroEntrada("existe_atv_area_degrad", objetoInfracao.ExisteAtvAreaDegrad, DbType.Int32);
				comando.AdicionarParametroEntrada("existe_atv_area_degrad_especif", String.IsNullOrWhiteSpace(objetoInfracao.ExisteAtvAreaDegradEspecificarTexto) ? (Object)DBNull.Value : objetoInfracao.ExisteAtvAreaDegradEspecificarTexto, DbType.String);
				comando.AdicionarParametroEntrada("fundament_infracao", objetoInfracao.FundamentoInfracao, DbType.String);
				comando.AdicionarParametroEntrada("uso_solo_area_danif", objetoInfracao.UsoSoloAreaDanificada, DbType.String);
				comando.AdicionarParametroEntrada("declividade_media_area", objetoInfracao.AreaDeclividadeMedia, DbType.Decimal);
				comando.AdicionarParametroEntrada("infracao_result_erosao", objetoInfracao.InfracaoResultouErosaoTipo == 1 ? 1 : objetoInfracao.InfracaoResultouErosaoTipo == 2 ? 0 : (Object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("caract_solo_area_danif", objetoInfracao.CaracteristicaSoloAreaDanificada, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("arquivo", objetoInfracao.Arquivo == null ? (Object)DBNull.Value : objetoInfracao.Arquivo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("infr_result_er_especifique", DbType.String, 150, objetoInfracao.InfracaoResultouErosaoTipoTexto);
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				objetoInfracao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				Historico.Gerar(objetoInfracao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

				Consulta.Gerar(objetoInfracao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

				bancoDeDados.Commit();
			}

			return objetoInfracao;
		}

		internal ObjetoInfracao Editar(ObjetoInfracao objetoInfracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Objeto de Infracao

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_fisc_obj_infracao i set i.fiscalizacao = :fiscalizacao, 
															i.area_embargada_atv_intermed = :area_embargada_atv_intermed,
															i.tei_gerado_pelo_sist = :tei_gerado_pelo_sist, i.tei_gerado_pelo_sist_serie = :tei_gerado_pelo_sist_serie,
															i.num_tei_bloco = :num_tei_bloco, i.data_lavratura_termo = :data_lavratura_termo, i.opniao_area_danificada = :opniao_area_danificada,
															i.desc_termo_embargo = :desc_termo_embargo, i.existe_atv_area_degrad = :existe_atv_area_degrad,
															i.existe_atv_area_degrad_especif = :existe_atv_area_degrad_especif, i.fundament_infracao = :fundament_infracao,
															i.uso_solo_area_danif = :uso_solo_area_danif, i.declividade_media_area = :declividade_media_area, 
															i.infracao_result_erosao = :infracao_result_erosao, i.caract_solo_area_danif = :caract_solo_area_danif, i.arquivo = :arquivo,
															i.tid = :tid, i.infr_result_er_especifique = :infr_result_er_especifique where i.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", objetoInfracao.FiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("area_embargada_atv_intermed", objetoInfracao.AreaEmbargadaAtvIntermed, DbType.Int32);
				comando.AdicionarParametroEntrada("tei_gerado_pelo_sist", objetoInfracao.TeiGeradoPeloSistema, DbType.Int32);
				comando.AdicionarParametroEntrada("tei_gerado_pelo_sist_serie", objetoInfracao.TeiGeradoPeloSistemaSerieTipo == 0 ? (Object)DBNull.Value : objetoInfracao.TeiGeradoPeloSistemaSerieTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("num_tei_bloco", String.IsNullOrWhiteSpace(objetoInfracao.NumTeiBloco) ? (Object)DBNull.Value : objetoInfracao.NumTeiBloco, DbType.Int32);
				comando.AdicionarParametroEntrada("data_lavratura_termo", String.IsNullOrWhiteSpace(objetoInfracao.DataLavraturaTermo.DataTexto) ? (Object)DBNull.Value : objetoInfracao.DataLavraturaTermo.DataTexto, DbType.Date);
				comando.AdicionarParametroEntrada("opniao_area_danificada", String.IsNullOrWhiteSpace(objetoInfracao.OpniaoAreaDanificada) ? (Object)DBNull.Value : objetoInfracao.OpniaoAreaDanificada, DbType.String);
				comando.AdicionarParametroEntrada("desc_termo_embargo", String.IsNullOrWhiteSpace(objetoInfracao.DescricaoTermoEmbargo) ? (Object)DBNull.Value : objetoInfracao.DescricaoTermoEmbargo, DbType.String);
				comando.AdicionarParametroEntrada("existe_atv_area_degrad", objetoInfracao.ExisteAtvAreaDegrad, DbType.Int32);
				comando.AdicionarParametroEntrada("existe_atv_area_degrad_especif", String.IsNullOrWhiteSpace(objetoInfracao.ExisteAtvAreaDegradEspecificarTexto) ? (Object)DBNull.Value : objetoInfracao.ExisteAtvAreaDegradEspecificarTexto, DbType.String);
				comando.AdicionarParametroEntrada("fundament_infracao", objetoInfracao.FundamentoInfracao, DbType.String);
				comando.AdicionarParametroEntrada("uso_solo_area_danif", objetoInfracao.UsoSoloAreaDanificada, DbType.String);
				comando.AdicionarParametroEntrada("declividade_media_area", objetoInfracao.AreaDeclividadeMedia, DbType.Decimal);
				comando.AdicionarParametroEntrada("infracao_result_erosao", objetoInfracao.InfracaoResultouErosaoTipo == 1 ? 1 : objetoInfracao.InfracaoResultouErosaoTipo == 2 ? 0 : (Object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("caract_solo_area_danif", objetoInfracao.CaracteristicaSoloAreaDanificada, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", objetoInfracao.Arquivo == null ? (Object)DBNull.Value : objetoInfracao.Arquivo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("infr_result_er_especifique", DbType.String, 150, objetoInfracao.InfracaoResultouErosaoTipoTexto);
				comando.AdicionarParametroEntrada("id", objetoInfracao.Id, DbType.Int32);
				
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(objetoInfracao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

				Consulta.Gerar(objetoInfracao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

				bancoDeDados.Commit();
			}

			return objetoInfracao;
		}

		public void Excluir(int fiscalizacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"delete {0}tab_fisc_obj_infracao t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);
				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		internal ObjetoInfracao Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
			ObjetoInfracao objetoInfracao = new ObjetoInfracao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				#region Objeto de infracao

				Comando comando = bancoDeDados.CriarComando(@"select i.id, t.situacao situacao_id, i.area_embargada_atv_intermed, i.tei_gerado_pelo_sist, 
															i.tei_gerado_pelo_sist_serie, i.num_tei_bloco, data_lavratura_termo,
															i.opniao_area_danificada, i.desc_termo_embargo, i.existe_atv_area_degrad,
															i.existe_atv_area_degrad_especif, i.fundament_infracao, i.uso_solo_area_danif, i.declividade_media_area,
															i.infracao_result_erosao, i.caract_solo_area_danif, i.arquivo, a.nome arquivo_nome, i.tid, i.infr_result_er_especifique
															from tab_fisc_obj_infracao i, tab_fiscalizacao t, tab_arquivo a where i.fiscalizacao = :fiscalizacao 
															and i.arquivo = a.id(+) and t.id = i.fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						objetoInfracao.Id = Convert.ToInt32(reader["id"]);
						objetoInfracao.FiscalizacaoId = fiscalizacaoId;
						objetoInfracao.FiscalizacaoSituacaoId = Convert.ToInt32(reader["situacao_id"]);
						objetoInfracao.AreaEmbargadaAtvIntermed = Convert.ToInt32(reader["area_embargada_atv_intermed"]);
						objetoInfracao.ExisteAtvAreaDegrad = Convert.ToInt32(reader["existe_atv_area_degrad"]);
						objetoInfracao.ExisteAtvAreaDegradEspecificarTexto = reader["existe_atv_area_degrad_especif"].ToString();
						objetoInfracao.FundamentoInfracao = reader["fundament_infracao"].ToString();
						objetoInfracao.UsoSoloAreaDanificada = reader["uso_solo_area_danif"].ToString();
						objetoInfracao.AreaDeclividadeMedia = reader.GetValue<decimal>("declividade_media_area").ToStringTrunc();
						
						objetoInfracao.Tid = reader["tid"].ToString();

						if (reader["infracao_result_erosao"] != null && !Convert.IsDBNull(reader["infracao_result_erosao"]))
						{
							objetoInfracao.InfracaoResultouErosaoTipo = Convert.ToInt32(reader["infracao_result_erosao"]) == 0 ? 2 : 1;
						}
						else 
						{
							objetoInfracao.InfracaoResultouErosaoTipo = 0;
						}

						if (reader["tei_gerado_pelo_sist_serie"] != null && !Convert.IsDBNull(reader["tei_gerado_pelo_sist_serie"]))
						{
							objetoInfracao.TeiGeradoPeloSistemaSerieTipo = Convert.ToInt32(reader["tei_gerado_pelo_sist_serie"]);
						}

						if (reader["tei_gerado_pelo_sist"] != null && !Convert.IsDBNull(reader["tei_gerado_pelo_sist"]))
						{
							objetoInfracao.TeiGeradoPeloSistema = Convert.ToInt32(reader["tei_gerado_pelo_sist"]);
						}

						if (reader["caract_solo_area_danif"] != null && !Convert.IsDBNull(reader["caract_solo_area_danif"]))
						{
							objetoInfracao.CaracteristicaSoloAreaDanificada = Convert.ToInt32(reader["caract_solo_area_danif"]);
						}

						if (reader["data_lavratura_termo"] != null && !Convert.IsDBNull(reader["data_lavratura_termo"]))
						{
							objetoInfracao.DataLavraturaTermo.Data = Convert.ToDateTime(reader["data_lavratura_termo"]);
						}

						if (reader["num_tei_bloco"] != null && !Convert.IsDBNull(reader["num_tei_bloco"]))
						{
							objetoInfracao.NumTeiBloco = reader["num_tei_bloco"].ToString();
						}

						if (reader["opniao_area_danificada"] != null && !Convert.IsDBNull(reader["opniao_area_danificada"]))
						{
							objetoInfracao.OpniaoAreaDanificada = reader["opniao_area_danificada"].ToString();
						}

						if (reader["desc_termo_embargo"] != null && !Convert.IsDBNull(reader["desc_termo_embargo"]))
						{
							objetoInfracao.DescricaoTermoEmbargo = reader["desc_termo_embargo"].ToString();
						}

						if (reader["arquivo"] != null && !Convert.IsDBNull(reader["arquivo"]))
						{
							objetoInfracao.Arquivo.Id = Convert.ToInt32(reader["arquivo"]);
							objetoInfracao.Arquivo.Nome = reader["arquivo_nome"].ToString();
						}

						objetoInfracao.InfracaoResultouErosaoTipoTexto = reader.GetValue<string>("infr_result_er_especifique");
					}
					reader.Close();
				}

				#endregion

			}
			return objetoInfracao;
		}

		internal int ObterID(int fiscalizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from tab_fisc_obj_infracao t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

				var retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno == null || Convert.IsDBNull(retorno)) ? 0 : Convert.ToInt32(retorno);
			}
		}

		#endregion
	}
}