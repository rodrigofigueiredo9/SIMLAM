using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Data
{
	public class AcompanhamentoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		public AcompanhamentoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter / Filtrar

		internal AcompanhamentoRelatorio Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			AcompanhamentoRelatorio acompanhamento = new AcompanhamentoRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Acompanhamento da Fiscalização

				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.fiscalizacao, t.numero_sufixo, t.data_vistoria, t.situacao, ls.texto situacao_texto, t.data_situacao, 
				t.agente_fiscal, f.nome agente_fiscal_nome, t.setor, t.area_total, t.area_florestal_nativa, (select stragg(lr.texto) from lov_acomp_fisc_reserva_leg lr 
				where bitand(lr.codigo, t.reserva_legal) > 0) reserva_legal, t.opniao_area_embargo, t.ativ_area_embargada, t.atviv_area_embargada_especific, t.uso_area_solo, 
				(select stragg(lr.texto) from lov_acomp_fisc_area_danif lr where bitand(lr.codigo, t.caract_solo_area_danificada) > 0) caract_solo_area_danificada, 
				t.declividade_media_area, t.infr_resultou_erosao, t.infr_resultou_erosao_especific, t.opniao_destin_material_apreend, t.houve_desrespeito_tad, 
				t.houve_desrespeito_tad_especifi, t.informacoes_relevante_processo, t.neces_repar_dano_amb, t.neces_repar_dano_amb_especific, t.firmou_termo_comprom, 
				t.firmou_termo_comprom_especific, t.tid from tab_acompanhamento_fisc t, lov_acomp_fisc_situacao ls, tab_funcionario f 
				where t.id = :id and t.situacao = ls.id and t.agente_fiscal = f.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						acompanhamento.Id = id;
						acompanhamento.FiscalizacaoId = reader.GetValue<int>("fiscalizacao");
						acompanhamento.NumeroSufixo = reader.GetValue<string>("numero_sufixo");
						acompanhamento.DataVistoria.DataTexto = reader.GetValue<string>("data_vistoria");
						acompanhamento.SituacaoId = reader.GetValue<int>("situacao");
						acompanhamento.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						acompanhamento.DataSituacao.DataTexto = reader.GetValue<string>("data_situacao");
						acompanhamento.AgenteId = reader.GetValue<int>("agente_fiscal");
						acompanhamento.AgenteNome = reader.GetValue<string>("agente_fiscal_nome");
						acompanhamento.SetorId = reader.GetValue<int>("setor");
						acompanhamento.AreaTotal = reader.GetValue<decimal>("area_total").ToStringTrunc(4);
						acompanhamento.AreaFlorestalNativa = reader.GetValue<decimal>("area_florestal_nativa").ToStringTrunc(4);
						acompanhamento.ReservalegalTipo = reader.GetValue<string>("reserva_legal");
						acompanhamento.OpniaoAreaEmbargo = reader.GetValue<string>("opniao_area_embargo");
						acompanhamento.AtividadeAreaEmbargada = reader.GetValue<int>("ativ_area_embargada") == 1 ? "Sim" : "Não";
						acompanhamento.AtividadeAreaEmbargadaEspecificarTexto = reader.GetValue<string>("atviv_area_embargada_especific");
						acompanhamento.UsoAreaSoloDescricao = reader.GetValue<string>("uso_area_solo");
						acompanhamento.CaracteristicaSoloAreaDanificada = reader.GetValue<string>("caract_solo_area_danificada");
						acompanhamento.AreaDeclividadeMedia = reader.GetValue<decimal>("declividade_media_area").ToStringTrunc();
						acompanhamento.InfracaoResultouErosao = reader.GetValue<int>("infr_resultou_erosao") == 1 ? "Sim" : "Não";
						acompanhamento.InfracaoResultouErosaoEspecificar = reader.GetValue<string>("infr_resultou_erosao_especific");
						acompanhamento.OpniaoDestMaterialApreend = reader.GetValue<string>("opniao_destin_material_apreend");
						acompanhamento.HouveDesrespeitoTAD = reader.GetValue<int>("houve_desrespeito_tad") == 1 ? "Sim" : "Não";
						acompanhamento.HouveDesrespeitoTADEspecificar = reader.GetValue<string>("houve_desrespeito_tad_especifi");
						acompanhamento.InformacoesRelevanteProcesso = reader.GetValue<string>("informacoes_relevante_processo");
						acompanhamento.RepararDanoAmbiental = reader.GetValue<int>("neces_repar_dano_amb") == 1 ? "Sim" : "Não";
						acompanhamento.RepararDanoAmbientalEspecificar = reader.GetValue<string>("neces_repar_dano_amb_especific");
						acompanhamento.FirmouTermoRepararDanoAmbiental = reader.GetValue<int>("firmou_termo_comprom") == 1 ? "Sim" : "Não";
						acompanhamento.FirmouTermoRepararDanoAmbientalEspecificar = reader.GetValue<string>("firmou_termo_comprom_especific");
						acompanhamento.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion Acompanhamento da Fiscalização

				if (acompanhamento == null || acompanhamento.Id <= 0 || simplificado)
				{
					return acompanhamento;
				}

				#region Assinantes

				comando = bancoDeDados.CriarComando(@"select ta.id, f.id func_id, f.nome func_nome, ta.cargo, c.nome cargo_nome, ta.tid 
				from {0}tab_acomp_fisc_assinante ta, {0}tab_funcionario f, {0}tab_cargo c where ta.funcionario = f.id and ta.cargo = c.id 
				and ta.acompanhamento = :acompanhamento", EsquemaBanco);

				comando.AdicionarParametroEntrada("acompanhamento", acompanhamento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					AssinanteDefault item;

					while (reader.Read())
					{
						item = new AssinanteDefault();
						item.Id = reader.GetValue<int>("func_id");
						item.Nome = reader.GetValue<string>("func_nome");
						item.Cargo = reader.GetValue<string>("cargo_nome");

						acompanhamento.Assinantes.Add(item);
					}

					reader.Close();
				}

				#endregion Assinantes

				#region Anexos

				comando = bancoDeDados.CriarComando(@"select a.id Id, a.ordem Ordem, a.descricao Descricao, b.nome, b.extensao, b.id arquivo_id, b.caminho, a.tid Tid
				from {0}tab_acomp_fisc_arquivo a, {0}tab_arquivo b where a.arquivo = b.id and a.acompanhamento = :acompanhamento order by a.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("acompanhamento", acompanhamento.Id, DbType.Int32);

				acompanhamento.Anexos = bancoDeDados.ObterEntityList<ConsideracoesFinaisAnexoRelatorio>(comando, (IDataReader reader, ConsideracoesFinaisAnexoRelatorio item) =>
				{
					item.Arquivo.Id = reader.GetValue<int>("arquivo_id");
					item.Arquivo.Caminho = reader.GetValue<string>("caminho");
					item.Arquivo.Nome = reader.GetValue<string>("nome");
					item.Arquivo.Extensao = reader.GetValue<string>("extensao");
				});

				#endregion Anexos
			}

			return acompanhamento;
		}

		#endregion
	}
}
