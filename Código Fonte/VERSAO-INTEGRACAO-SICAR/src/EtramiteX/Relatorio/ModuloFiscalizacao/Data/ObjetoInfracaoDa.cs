using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Data
{
	public class ObjetoInfracaoDa
	{
		#region Propriedade e Atributos

		private String EsquemaBanco { get; set; }

		#endregion

		public ObjetoInfracaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter
		
		public ObjetoInfracaoRelatorio Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
			ObjetoInfracaoRelatorio objeto = new ObjetoInfracaoRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"
					select (case o.area_embargada_atv_intermed
							 when 1 then
							  'Sim'
							 when 0 then
							  'Não'
						   end) AreaEmbargadaInterditada,
						   o.TEI_GERADO_PELO_SIST IsGeradoSistema,
						   nvl(o.num_tei_bloco, f.autos) NumeroTEI,
						   to_char(o.data_lavratura_termo, 'DD/MM/YYYY') DataLavraturaTEI,
						   o.opniao_area_danificada OpinarEmbargo,
						   ls.texto SerieTexto,
						   (case o.existe_atv_area_degrad
							 when 1 then
							  'Sim'
							 when 0 then
							  'Não'
						   end) AtividadeAreaDegradado,
						   o.existe_atv_area_degrad_especif AtividadeAreaDegradadoEspecif,
						   o.fundament_infracao FundamentoCaracterizacaoInfra,
						   o.uso_solo_area_danif UsoOcupacaoSolo,
						   (select stragg(b.texto)
							  from {0}lov_fisc_obj_infra_carac_solo b
							 where bitand(o.caract_solo_area_danif, b.codigo) != 0) CaracteristicaoAreaSoloDanifi,
						   o.declividade_media_area DeclividadeMedia,
						   (case o.infracao_result_erosao
							 when 1 then
							  'Sim'
							 when 0 then
							  'Não'
							 else
							  ''
						   end) IsInfracaoErosaoSolo,
						   O.infr_result_er_especifique EspecificarIsInfracao
					  from {0}tab_fisc_obj_infracao  o,
						   {0}lov_fiscalizacao_serie ls,
						   {0}tab_fiscalizacao       f
					 where o.fiscalizacao = f.id(+)
					   and o.tei_gerado_pelo_sist_serie = ls.id(+)
					   and f.id = :fiscalizacaoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);

				objeto = bancoDeDados.ObterEntity<ObjetoInfracaoRelatorio>(comando, (IDataReader reader, ObjetoInfracaoRelatorio item) => 
				{
					item.EspecificarIsInfracaoErosaoSolo = reader.GetValue<string>("EspecificarIsInfracao");
					item.DeclividadeMedia = reader.GetValue<decimal>("DeclividadeMedia").ToStringTrunc();
				});

				objeto.SerieTexto = String.IsNullOrWhiteSpace(objeto.NumeroTEI) ? String.Empty : objeto.SerieTexto;

				if (objeto.DataLavraturaTEI == null) 
				{
					objeto.DataLavraturaTEI = new FiscalizacaoDa().ObterDataConclusao(fiscalizacaoId, bancoDeDados).DataTexto;
				}

				if (!objeto.IsGeradoSistema.HasValue)
				{
					objeto.NumeroTEI = null;
					objeto.DataLavraturaTEI = null;
				}
			}

			return objeto;
		}

		public ObjetoInfracaoRelatorio ObterHistorico(int historicoId, BancoDeDados banco = null)
		{
			ObjetoInfracaoRelatorio objeto = new ObjetoInfracaoRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"
					select (case o.area_embargada_atv_intermed
								when 1 then
								'Sim'
								when 0 then
								'Não'
							end) AreaEmbargadaInterditada,
							o.TEI_GERADO_PELO_SIST IsGeradoSistema,
							nvl(o.num_tei_bloco, f.autos) NumeroTEI,
							to_char(nvl(o.data_lavratura_termo, f.situacao_data), 'DD/MM/YYYY') DataLavraturaTEI,
							o.opniao_area_danificada OpinarEmbargo,
					ls.texto SerieTexto,
					(case o.existe_atv_area_degrad
					when 1 then
					'Sim'
					when 0 then
					'Não'
					end) AtividadeAreaDegradado,
					o.existe_atv_area_degrad_especif AtividadeAreaDegradadoEspecif,
					o.fundament_infracao FundamentoCaracterizacaoInfra,
					o.uso_solo_area_danif UsoOcupacaoSolo,
					(select stragg(b.texto)
					from lov_fisc_obj_infra_carac_solo b
					where bitand(o.caract_solo_area_danif, b.codigo) != 0) CaracteristicaoAreaSoloDanifi,
					o.declividade_media_area DeclividadeMedia,
					(case o.infracao_result_erosao
					when 1 then
					'Sim'
					when 0 then
					'Não'
					else
					''
					end) IsInfracaoErosaoSolo,
					O.infr_result_er_especifique EspecificarIsInfracao
					from hst_fisc_obj_infracao  o,
					lov_fiscalizacao_serie ls,
					hst_fiscalizacao       f
					where o.fiscalizacao_id_hst = f.id
					and o.tei_gerado_pelo_sist_serie = ls.id(+)
					and f.id =  :historicoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				objeto = bancoDeDados.ObterEntity<ObjetoInfracaoRelatorio>(comando, (IDataReader reader, ObjetoInfracaoRelatorio item) =>
				{
					item.EspecificarIsInfracaoErosaoSolo = reader.GetValue<string>("EspecificarIsInfracao");
					item.DeclividadeMedia = reader.GetValue<decimal>("DeclividadeMedia").ToStringTrunc();
				});

				objeto.SerieTexto = String.IsNullOrWhiteSpace(objeto.NumeroTEI) ? String.Empty : objeto.SerieTexto;

				if (!objeto.IsGeradoSistema.HasValue)
				{
					objeto.NumeroTEI = null;
					objeto.DataLavraturaTEI = null;
				}
			}

			return objeto;
		}

		#endregion
	}
}