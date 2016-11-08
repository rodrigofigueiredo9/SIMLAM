using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade.Data
{
	public class DescricaoLicenciamentoAtividadeDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }
		private ConfiguracaoSistema _configuracaoSistema = new ConfiguracaoSistema();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		private string EsquemaBanco { get; set; }

		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		#endregion

		public DescricaoLicenciamentoAtividadeDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(DescricaoLicenciamentoAtividade descricaoLicenAtv, BancoDeDados banco = null)
		{
			if (descricaoLicenAtv == null)
			{
				throw new Exception("A descrição de licenciamento de atividade é nulo.");
			}

			if (descricaoLicenAtv.Id <= 0)
			{
				Criar(descricaoLicenAtv, banco);
			}
			else
			{
				Editar(descricaoLicenAtv, banco);
			}
		}

		internal int? Criar(DescricaoLicenciamentoAtividade descricaoLicenAtv, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Descrição de Licenciamento de Atividade
				//caracterizacao
				Comando comando = bancoDeDados.CriarComando(@"
					insert into {0}crt_dsc_lc_atividade
					  (id,
					   resp_atividade,
					   empreendimento,
					   caracterizacao,
					   bacia_hidrografica,
					   existe_app_util,
					   tipo_vegetacao_util,
					   zona_amort_uc,
					   zona_amort_uc_nome,
					   localizada_uc,
					   localizada_uc_nome,
					   patrimonio_historico,
					   residentes_entorno,
					   residentes_enterno_distanci,
					   area_terreno,
					   area_util,
					   total_funcionarios,
					   horas_dias,
					   dias_mes,
					   turnos_dia,
					   consumo_agua_ls,
					   consumo_agua_mh,
					   consumo_agua_mdia,
					   consumo_agua_mmes,
					   tipo_outorga,
					   numero,
					   fontes_abastecimento_agua,
					   ponto_lancamento,
					   tid)
					values
					  ({0}seq_crt_dsc_lc_atv.nextval,
					   :resp_atividade,
					   :empreendimento,
					   :caracterizacao,
					   :bacia_hidrografica,
					   :existe_app_util,
					   :tipo_vegetacao_util,
					   :zona_amort_uc,
					   :zona_amort_uc_nome,
					   :localizada_uc,
					   :localizada_uc_nome,
					   :patrimonio_historico,
					   :residentes_entorno,
					   :residentes_enterno_distanci,
					   :area_terreno,
					   :area_util,
					   :total_funcionarios,
					   :horas_dias,
					   :dias_mes,
					   :turnos_dia,
					   :consumo_agua_ls,
					   :consumo_agua_mh,
					   :consumo_agua_mdia,
					   :consumo_agua_mmes,
					   :tipo_outorga,
					   :numero,
					   :fontes_abastecimento_agua,
					   :ponto_lancamento,
					   :tid)
					returning id into :idOut", EsquemaBanco);

				comando.AdicionarParametroEntrada("resp_atividade", descricaoLicenAtv.RespAtividade, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", descricaoLicenAtv.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", descricaoLicenAtv.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("bacia_hidrografica", descricaoLicenAtv.BaciaHidrografica, DbType.String);
				comando.AdicionarParametroEntrada("existe_app_util", descricaoLicenAtv.ExisteAppUtil, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_vegetacao_util", descricaoLicenAtv.TipoVegetacaoUtilCodigo, DbType.Int32);
				comando.AdicionarParametroEntrada("zona_amort_uc", descricaoLicenAtv.ZonaAmortUC, DbType.Int32);
				comando.AdicionarParametroEntrada("zona_amort_uc_nome", descricaoLicenAtv.ZonaAmortUCNomeOrgaoAdm, DbType.String);
				comando.AdicionarParametroEntrada("localizada_uc", descricaoLicenAtv.LocalizadaUC, DbType.Int32);
				comando.AdicionarParametroEntrada("localizada_uc_nome", descricaoLicenAtv.LocalizadaUCNomeOrgaoAdm, DbType.String);
				comando.AdicionarParametroEntrada("patrimonio_historico", descricaoLicenAtv.PatrimonioHistorico, DbType.Int32);
				comando.AdicionarParametroEntrada("residentes_entorno", descricaoLicenAtv.ResidentesEntorno, DbType.Int32);
				comando.AdicionarParametroEntrada("residentes_enterno_distanci", descricaoLicenAtv.ResidentesEnternoDistancia, DbType.Decimal);
				comando.AdicionarParametroEntrada("area_terreno", descricaoLicenAtv.AreaTerreno, DbType.Decimal);
				comando.AdicionarParametroEntrada("area_util", descricaoLicenAtv.AreaUtil, DbType.Decimal);
				comando.AdicionarParametroEntrada("total_funcionarios", descricaoLicenAtv.TotalFuncionarios, DbType.Int32);
				comando.AdicionarParametroEntrada("horas_dias", DbType.String, 5, descricaoLicenAtv.HorasDias);
				comando.AdicionarParametroEntrada("dias_mes", descricaoLicenAtv.DiasMes, DbType.Int32);
				comando.AdicionarParametroEntrada("turnos_dia", descricaoLicenAtv.TurnosDia, DbType.Int32);
				comando.AdicionarParametroEntrada("consumo_agua_ls", descricaoLicenAtv.ConsumoAguaLs, DbType.Decimal);
				comando.AdicionarParametroEntrada("consumo_agua_mh", descricaoLicenAtv.ConsumoAguaMh, DbType.Decimal);
				comando.AdicionarParametroEntrada("consumo_agua_mdia", descricaoLicenAtv.ConsumoAguaMdia, DbType.Decimal);
				comando.AdicionarParametroEntrada("consumo_agua_mmes", descricaoLicenAtv.ConsumoAguaMmes, DbType.Decimal);
				comando.AdicionarParametroEntrada("tipo_outorga", descricaoLicenAtv.TipoOutorgaId, DbType.Int32);
				comando.AdicionarParametroEntrada("numero", descricaoLicenAtv.Numero, DbType.String);
				comando.AdicionarParametroEntrada("fontes_abastecimento_agua", descricaoLicenAtv.FonteAbastecimentoAguaTipoId, DbType.Int32);
				comando.AdicionarParametroEntrada("ponto_lancamento", descricaoLicenAtv.PontoLancamentoTipoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("idOut", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				descricaoLicenAtv.Id = Convert.ToInt32(comando.ObterValorParametro("idOut"));

				#endregion

				#region Fonte(s) de abastecimento de água

				foreach (var item in descricaoLicenAtv.FontesAbastecimentoAgua)
				{
					comando = bancoDeDados.CriarComando(@"
					insert into {0}crt_dsc_lc_atv_abas_agua
					  (id,
					   dsc_lc_atividade,
					   tipo_abast_agua,
					   descricao,
					   tid)
					values
					  ({0}seq_crt_dsc_lc_atv_ab_agua.nextval,
					   :dsc_lc_atividade,
					   :tipo_abast_agua,
					   :descricao,
					   :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_abast_agua", item.TipoId, DbType.Int32);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Pontos de lançamento de efluente

				foreach (var item in descricaoLicenAtv.PontosLancamentoEfluente)
				{
					comando = bancoDeDados.CriarComando(@"
						insert into {0}crt_dsc_lc_atv_plefluent
						  (id, 
						   dsc_lc_atividade, 
						   tipo_pt_lan_fluente, 
						   descricao, 
						   tid)
						values
						  ({0}seq_crt_dsc_lc_atv_pleflue.nextval,
						   :dsc_lc_atividade,
						   :tipo_pt_lan_fluente,
						   :descricao,
						   :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_pt_lan_fluente", item.TipoId, DbType.Int32);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Efluentes Líquidos

				foreach (var item in descricaoLicenAtv.EfluentesLiquido)
				{
					comando = bancoDeDados.CriarComando(@"
						insert into {0}crt_dsc_lc_atv_efl_liqui
						  (id,
						   dsc_lc_atividade,
						   tipo_fonte_gerecao,
						   vazao,
						   unidade,
						   sistema_tratamento,
						   descricao,
						   tid)
						values
						  ({0}seq_crt_dsc_lc_atv_efl_liq.nextval,
						   :dsc_lc_atividade,
						   :tipo_fonte_gerecao,
						   :vazao,
						   :unidade,
						   :sistema_tratamento,
						   :descricao,
						   :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_fonte_gerecao", item.TipoId, DbType.Int32);
					comando.AdicionarParametroEntrada("vazao", item.Vazao, DbType.Decimal);
					comando.AdicionarParametroEntrada("unidade", item.UnidadeId, DbType.Int32);
					comando.AdicionarParametroEntrada("sistema_tratamento", DbType.String, 100, item.SistemaTratamento);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Resíduos Sólidos não inertes

				foreach (var item in descricaoLicenAtv.ResiduosSolidosNaoInerte)
				{
					comando = bancoDeDados.CriarComando(@"
						insert into {0}crt_dsc_lc_atv_ress_inertes
						  (id,
						   dsc_lc_atividade,
						   classe_residuo,
						   tipo_residuo,
						   acondicionamento,
						   estocagem,
						   tratamento,
						   tratamento_descricao,
						   destino_final,
						   destino_final_descricao,
						   tid)
						values
						  ({0}seq_crt_dsc_lc_atv_res_iner.nextval,
						   :dsc_lc_atividade,
						   :classe_residuo,
						   :tipo_residuo,
						   :acondicionamento,
						   :estocagem,
						   :tratamento,
						   :tratamento_descricao,
						   :destino_final,
						   :destino_final_descricao,
						   :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("classe_residuo", DbType.String, 100, item.ClasseResiduo);
					comando.AdicionarParametroEntrada("tipo_residuo", DbType.String, 100, item.Tipo);
					comando.AdicionarParametroEntrada("acondicionamento", item.AcondicionamentoCodigo, DbType.Int32);
					comando.AdicionarParametroEntrada("estocagem", item.EstocagemCodigo, DbType.Int32);
					comando.AdicionarParametroEntrada("tratamento", item.TratamentoCodigo, DbType.Int32);
					comando.AdicionarParametroEntrada("tratamento_descricao", DbType.String, 100, item.TratamentoDescricao);
					comando.AdicionarParametroEntrada("destino_final", item.DestinoFinalCodigo, DbType.Int32);
					comando.AdicionarParametroEntrada("destino_final_descricao", DbType.String, 100, item.DestinoFinalDescricao);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Emissões Atmosféricas

				foreach (var item in descricaoLicenAtv.EmissoesAtmosfericas)
				{
					comando = bancoDeDados.CriarComando(@"
						insert into {0}crt_dsc_lc_atv_emissoes_atm
							(id,
							dsc_lc_atividade,
							tipo_combustivel,
							substancia_emitida,
							equipamento_controle,
							tid)
						values
							({0}seq_crt_dsc_lc_ems_atm.nextval,
							:dsc_lc_atividade,
							:tipo_combustivel,
							:substancia_emitida,
							:equipamento_controle,
							:tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_combustivel", item.TipoCombustivelId, DbType.Int32);
					comando.AdicionarParametroEntrada("substancia_emitida", DbType.String, 50, item.SubstanciaEmitida);
					comando.AdicionarParametroEntrada("equipamento_controle", DbType.String, 50, item.EquipamentoControle);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion				

				#region Histórico

				Historico.Gerar(descricaoLicenAtv.Id, eHistoricoArtefatoCaracterizacao.desclicencatividade, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return descricaoLicenAtv.Id;
			}
		}

		internal void Editar(DescricaoLicenciamentoAtividade descricaoLicenAtv, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Descrição de Licenciamento de Atividade

				Comando comando = bancoDeDados.CriarComando(@"
				  update {0}crt_dsc_lc_atividade t
					 set t.resp_atividade              = :resp_atividade,
						 t.bacia_hidrografica          = :bacia_hidrografica,
						 t.existe_app_util             = :existe_app_util,
						 t.tipo_vegetacao_util         = :tipo_vegetacao_util,
						 t.zona_amort_uc               = :zona_amort_uc,
						 t.zona_amort_uc_nome          = :zona_amort_uc_nome,
						 t.localizada_uc               = :localizada_uc,
						 t.localizada_uc_nome          = :localizada_uc_nome,
						 t.patrimonio_historico        = :patrimonio_historico,
						 t.residentes_entorno          = :residentes_entorno,
						 t.residentes_enterno_distanci = :residentes_enterno_distanci,
						 t.area_terreno                = :area_terreno,
						 t.area_util                   = :area_util,
						 t.total_funcionarios          = :total_funcionarios,
						 t.horas_dias                  = :horas_dias,
						 t.dias_mes                    = :dias_mes,
						 t.turnos_dia                  = :turnos_dia,
						 t.consumo_agua_ls             = :consumo_agua_ls,
						 t.consumo_agua_mh             = :consumo_agua_mh,
						 t.consumo_agua_mdia           = :consumo_agua_mdia,
						 t.consumo_agua_mmes           = :consumo_agua_mmes,
						 t.tipo_outorga                = :tipo_outorga,
						 t.numero                      = :numero,
						 t.fontes_abastecimento_agua   = :fontes_abastecimento_agua,
						 t.ponto_lancamento            = :ponto_lancamento,
						 t.tid                         = :tid
				   where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("resp_atividade", descricaoLicenAtv.RespAtividade, DbType.Int32);
				comando.AdicionarParametroEntrada("bacia_hidrografica", descricaoLicenAtv.BaciaHidrografica, DbType.String);				
				comando.AdicionarParametroEntrada("existe_app_util", descricaoLicenAtv.ExisteAppUtil, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_vegetacao_util", descricaoLicenAtv.TipoVegetacaoUtilCodigo, DbType.Int32);
				comando.AdicionarParametroEntrada("zona_amort_uc", descricaoLicenAtv.ZonaAmortUC, DbType.Int32);
				comando.AdicionarParametroEntrada("zona_amort_uc_nome", descricaoLicenAtv.ZonaAmortUCNomeOrgaoAdm, DbType.String);
				comando.AdicionarParametroEntrada("localizada_uc", descricaoLicenAtv.LocalizadaUC, DbType.Int32);
				comando.AdicionarParametroEntrada("localizada_uc_nome", descricaoLicenAtv.LocalizadaUCNomeOrgaoAdm, DbType.String);				
				comando.AdicionarParametroEntrada("patrimonio_historico", descricaoLicenAtv.PatrimonioHistorico, DbType.Int32);
				comando.AdicionarParametroEntrada("residentes_entorno", descricaoLicenAtv.ResidentesEntorno, DbType.Int32);
				comando.AdicionarParametroEntrada("residentes_enterno_distanci", descricaoLicenAtv.ResidentesEnternoDistancia, DbType.Decimal);
				comando.AdicionarParametroEntrada("area_terreno", descricaoLicenAtv.AreaTerreno, DbType.Decimal);
				comando.AdicionarParametroEntrada("area_util", descricaoLicenAtv.AreaUtil, DbType.Decimal);
				comando.AdicionarParametroEntrada("total_funcionarios", descricaoLicenAtv.TotalFuncionarios, DbType.Int32);
				comando.AdicionarParametroEntrada("horas_dias", DbType.String, 5, descricaoLicenAtv.HorasDias);
				comando.AdicionarParametroEntrada("dias_mes", descricaoLicenAtv.DiasMes, DbType.Int32);
				comando.AdicionarParametroEntrada("turnos_dia", descricaoLicenAtv.TurnosDia, DbType.Int32);
				comando.AdicionarParametroEntrada("consumo_agua_ls", descricaoLicenAtv.ConsumoAguaLs, DbType.Decimal);
				comando.AdicionarParametroEntrada("consumo_agua_mh", descricaoLicenAtv.ConsumoAguaMh, DbType.Decimal);
				comando.AdicionarParametroEntrada("consumo_agua_mdia", descricaoLicenAtv.ConsumoAguaMdia, DbType.Decimal);
				comando.AdicionarParametroEntrada("consumo_agua_mmes", descricaoLicenAtv.ConsumoAguaMmes, DbType.Decimal);
				comando.AdicionarParametroEntrada("tipo_outorga", descricaoLicenAtv.TipoOutorgaId, DbType.Int32);
				comando.AdicionarParametroEntrada("numero", descricaoLicenAtv.Numero, DbType.String);
				comando.AdicionarParametroEntrada("fontes_abastecimento_agua", descricaoLicenAtv.FonteAbastecimentoAguaTipoId, DbType.Int32);
				comando.AdicionarParametroEntrada("ponto_lancamento", descricaoLicenAtv.PontoLancamentoTipoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", descricaoLicenAtv.Id, DbType.Int32);				

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Fonte(s) de abastecimento de água

				comando = bancoDeDados.CriarComando("delete from {0}crt_dsc_lc_atv_abas_agua t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.dsc_lc_atividade = :dsc_lc_atividade{0}",
					comando.AdicionarNotIn("and", "t.id", DbType.Int32, descricaoLicenAtv.FontesAbastecimentoAgua.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);			 

				foreach (var item in descricaoLicenAtv.FontesAbastecimentoAgua)
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
							update {0}crt_dsc_lc_atv_abas_agua t
							   set t.dsc_lc_atividade  = :dsc_lc_atividade,
								   t.tipo_abast_agua   = :tipo_abast_agua,
								   t.descricao         = :descricao,
								   t.tid               = :tid
							 where t.id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_dsc_lc_atv_abas_agua
							  (id,
							   dsc_lc_atividade,
							   tipo_abast_agua,
							   descricao,
							   tid)
							values
							  ({0}seq_crt_dsc_lc_atv_ab_agua.nextval,
							   :dsc_lc_atividade,
							   :tipo_abast_agua,
							   :descricao,
							   :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_abast_agua", item.TipoId, DbType.Int32);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Pontos de lançamento de efluente

				comando = bancoDeDados.CriarComando("delete from {0}crt_dsc_lc_atv_plefluent t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.dsc_lc_atividade = :dsc_lc_atividade{0}",
					comando.AdicionarNotIn("and", "t.id", DbType.Int32, descricaoLicenAtv.PontosLancamentoEfluente.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);			 

				foreach (var item in descricaoLicenAtv.PontosLancamentoEfluente)
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
							update {0}crt_dsc_lc_atv_plefluent t
							   set dsc_lc_atividade    = :dsc_lc_atividade,
								   tipo_pt_lan_fluente = :tipo_pt_lan_fluente,
								   descricao           = :descricao,
								   tid                 = :tid
							 where t.id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_dsc_lc_atv_plefluent
							  (id, 
							   dsc_lc_atividade, 
							   tipo_pt_lan_fluente, 
							   descricao, 
							   tid)
							values
							  ({0}seq_crt_dsc_lc_atv_pleflue.nextval,
							   :dsc_lc_atividade,
							   :tipo_pt_lan_fluente,
							   :descricao,
							   :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_pt_lan_fluente", item.TipoId, DbType.Int32);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Efluentes Líquidos

				comando = bancoDeDados.CriarComando("delete from {0}crt_dsc_lc_atv_efl_liqui t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.dsc_lc_atividade = :dsc_lc_atividade{0}",
					comando.AdicionarNotIn("and", "t.id", DbType.Int32, descricaoLicenAtv.EfluentesLiquido.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);			 

				foreach (var item in descricaoLicenAtv.EfluentesLiquido)
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
							update {0}crt_dsc_lc_atv_efl_liqui t
							   set t.dsc_lc_atividade   = :dsc_lc_atividade,
								   t.tipo_fonte_gerecao = :tipo_fonte_gerecao,
								   t.vazao              = :vazao,
								   t.unidade            = :unidade,
								   t.sistema_tratamento = :sistema_tratamento,
								   t.descricao          = :descricao,
								   t.tid                = :tid
							 where t.id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_dsc_lc_atv_efl_liqui
							  (id,
							   dsc_lc_atividade,
							   tipo_fonte_gerecao,
							   vazao,
							   unidade,
							   sistema_tratamento,
							   descricao,
							   tid)
							values
							  ({0}seq_crt_dsc_lc_atv_efl_liq.nextval,
							   :dsc_lc_atividade,
							   :tipo_fonte_gerecao,
							   :vazao,
							   :unidade,
							   :sistema_tratamento,
							   :descricao,
							   :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_fonte_gerecao", item.TipoId, DbType.Int32);
					comando.AdicionarParametroEntrada("vazao", item.Vazao, DbType.Decimal);
					comando.AdicionarParametroEntrada("unidade", item.UnidadeId, DbType.Int32);
					comando.AdicionarParametroEntrada("sistema_tratamento", DbType.String, 100, item.SistemaTratamento);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Resíduos Sólidos não inertes

				comando = bancoDeDados.CriarComando("delete from {0}crt_dsc_lc_atv_ress_inertes t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.dsc_lc_atividade = :dsc_lc_atividade{0}",
					comando.AdicionarNotIn("and", "t.id", DbType.Int32, descricaoLicenAtv.ResiduosSolidosNaoInerte.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);			 

				foreach (var item in descricaoLicenAtv.ResiduosSolidosNaoInerte)
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
							update {0}crt_dsc_lc_atv_ress_inertes t
							   set t.dsc_lc_atividade        = :dsc_lc_atividade,
								   t.classe_residuo          = :classe_residuo,
								   t.tipo_residuo            = :tipo_residuo,
								   t.acondicionamento        = :acondicionamento,
								   t.estocagem               = :estocagem,
								   t.tratamento              = :tratamento,
								   t.tratamento_descricao    = :tratamento_descricao,
								   t.destino_final           = :destino_final,
								   t.destino_final_descricao = :destino_final_descricao,
								   t.tid                     = :tid
							 where t.id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_dsc_lc_atv_ress_inertes
							  (id,
							   dsc_lc_atividade,
							   classe_residuo,
							   tipo_residuo,
							   acondicionamento,
							   estocagem,
							   tratamento,
							   tratamento_descricao,
							   destino_final,
							   destino_final_descricao,
							   tid)
							values
							  ({0}seq_crt_dsc_lc_atv_res_iner.nextval,
							   :dsc_lc_atividade,
							   :classe_residuo,
							   :tipo_residuo,
							   :acondicionamento,
							   :estocagem,
							   :tratamento,
							   :tratamento_descricao,
							   :destino_final,
							   :destino_final_descricao,
							   :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("classe_residuo", DbType.String, 100, item.ClasseResiduo);
					comando.AdicionarParametroEntrada("tipo_residuo", DbType.String, 100, item.Tipo);
					comando.AdicionarParametroEntrada("acondicionamento", item.AcondicionamentoCodigo, DbType.Int32);
					comando.AdicionarParametroEntrada("estocagem", item.EstocagemCodigo, DbType.Int32);
					comando.AdicionarParametroEntrada("tratamento", item.TratamentoCodigo, DbType.Int32);
					comando.AdicionarParametroEntrada("tratamento_descricao", DbType.String, 100, item.TratamentoDescricao);
					comando.AdicionarParametroEntrada("destino_final", item.DestinoFinalCodigo, DbType.Int32);
					comando.AdicionarParametroEntrada("destino_final_descricao", DbType.String, 100, item.DestinoFinalDescricao);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Emissões Atmosféricas

				comando = bancoDeDados.CriarComando("delete from {0}crt_dsc_lc_atv_emissoes_atm t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.dsc_lc_atividade = :dsc_lc_atividade{0}",
					comando.AdicionarNotIn("and", "t.id", DbType.Int32, descricaoLicenAtv.EmissoesAtmosfericas.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);			 

				foreach (var item in descricaoLicenAtv.EmissoesAtmosfericas)
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
							update {0}crt_dsc_lc_atv_emissoes_atm t
							   set t.dsc_lc_atividade     = :dsc_lc_atividade,
								   t.tipo_combustivel     = :tipo_combustivel,
								   t.substancia_emitida   = :substancia_emitida,
								   t.equipamento_controle = :equipamento_controle,
								   t.tid                  = :tid
							 where t.id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_dsc_lc_atv_emissoes_atm
							  (id,
							   dsc_lc_atividade,
							   tipo_combustivel,
							   substancia_emitida,
							   equipamento_controle,
							   tid)
							values
							  ({0}seq_crt_dsc_lc_ems_atm.nextval,
							   :dsc_lc_atividade,
							   :tipo_combustivel,
							   :substancia_emitida,
							   :equipamento_controle,
							   :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("dsc_lc_atividade", descricaoLicenAtv.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_combustivel", item.TipoCombustivelId, DbType.Int32);
					comando.AdicionarParametroEntrada("substancia_emitida", DbType.String, 50, item.SubstanciaEmitida);
					comando.AdicionarParametroEntrada("equipamento_controle", DbType.String, 50, item.EquipamentoControle);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Histórico

				Historico.Gerar(descricaoLicenAtv.Id, eHistoricoArtefatoCaracterizacao.desclicencatividade, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimento, int tipo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_dsc_lc_atividade c set c.tid = :tid 
				where c.empreendimento = :empreendimento and c.caracterizacao = :caracterizacao returning c.id into :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				if (comando.LinhasAfetadas == 0)
				{
					return;
				}

				int id = 0;
				id = Convert.ToInt32(comando.ObterValorParametro("id"));

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.desclicencatividade, eHistoricoAcao.excluir, bancoDeDados);

				comando = bancoDeDados.CriarComando(@"" +
				"begin " +
					"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :id and d.dependente_caracterizacao = :dependente_caracterizacao; " +
					"delete from {0}crt_dsc_lc_atv_abas_agua r where r.dsc_lc_atividade = :id; " +
					"delete from {0}crt_dsc_lc_atv_plefluent r where r.dsc_lc_atividade = :id; " +
					"delete from {0}crt_dsc_lc_atv_efl_liqui r where r.dsc_lc_atividade = :id; " +
					"delete from {0}crt_dsc_lc_atv_ress_inertes r where r.dsc_lc_atividade = :id; " +
					"delete from {0}crt_dsc_lc_atv_emissoes_atm r where r.dsc_lc_atividade = :id; " +
					"delete from {0}crt_dsc_lc_atividade r where r.id = :id; " +
				"end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter

		public DescricaoLicenciamentoAtividade Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			DescricaoLicenciamentoAtividade descricaoLicenAtv = new DescricaoLicenciamentoAtividade();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Descrição de Licenciamento de Atividade

				Comando comando = bancoDeDados.CriarComando(@"
					select c.id Id,
						   c.resp_atividade RespAtividade,
						   c.empreendimento EmpreendimentoId,
						   c.caracterizacao Tipo,
						   c.bacia_hidrografica BaciaHidrografica,
						   c.existe_app_util ExisteAppUtil,
						   c.tipo_vegetacao_util TipoVegetacaoUtilCodigo,
						   (select stragg(t.texto) texto from {0}lov_crt_dsc_lc_atv_vgarutil t where bitand(t.codigo, c.tipo_vegetacao_util) > 0) TipoVegetacaoUtilTexto,
						   c.zona_amort_uc ZonaAmortUC,
						   c.zona_amort_uc_nome ZonaAmortUCNomeOrgaoAdm,
						   c.localizada_uc LocalizadaUC,
						   c.localizada_uc_nome LocalizadaUCNomeOrgaoAdm,
						   c.patrimonio_historico PatrimonioHistorico,
						   c.residentes_entorno ResidentesEntorno,
						   c.residentes_enterno_distanci ResidentesEnternoDistancia,
						   c.area_terreno AreaTerreno,
						   c.area_util AreaUtil,
						   c.total_funcionarios TotalFuncionarios,
						   c.horas_dias HorasDias,
						   c.dias_mes DiasMes,
						   c.turnos_dia TurnosDia,
						   c.consumo_agua_ls ConsumoAguaLs,
						   c.consumo_agua_mh ConsumoAguaMh,
						   c.consumo_agua_mdia ConsumoAguaMdia,
						   c.consumo_agua_mmes ConsumoAguaMmes,
						   c.tipo_outorga TipoOutorgaId,
						   lb.texto TipoOutorgaTexto,
						   c.numero Numero,
						   c.fontes_abastecimento_agua FonteAbastecimentoAguaTipoId,
						   c.ponto_lancamento PontoLancamentoTipoId,
						   c.tid Tid
					  from {0}crt_dsc_lc_atividade c, 
						   {0}lov_crt_dsc_lc_atv_vgarutil l,
						   {0}lov_crt_dsc_lc_atv_out_agua lb
					 where c.tipo_vegetacao_util = l.id(+)
					   and c.tipo_outorga = lb.id(+)
					   and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				descricaoLicenAtv = bancoDeDados.ObterEntity<DescricaoLicenciamentoAtividade>(comando);

				#endregion

				if (simplificado)
				{
					return descricaoLicenAtv;
				}

				#region Fonte(s) de abastecimento de água

				comando = bancoDeDados.CriarComando(@"
					select ta.id Id,
						   ta.id IdRelacionamento,
						   ta.tipo_abast_agua TipoId,
						   la.texto TipoTexto,
						   ta.descricao Descricao,
						   ta.tid Tid
					  from {0}crt_dsc_lc_atv_abas_agua    ta,
						   {0}crt_dsc_lc_atividade        tb,
						   {0}lov_crt_dsc_lc_atv_foabagua la
					 where ta.dsc_lc_atividade = tb.id
					   and ta.tipo_abast_agua = la.id(+)					   
					   and tb.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				descricaoLicenAtv.FontesAbastecimentoAgua = bancoDeDados.ObterEntityList<FonteAbastecimentoAgua>(comando);

				#endregion

				#region Pontos de lançamento de efluente

				comando = bancoDeDados.CriarComando(@"
					select ta.id Id, 
						   ta.id IdRelacionamento,
						   ta.tipo_pt_lan_fluente TipoId, 
						   la.texto TipoTexto, 
						   ta.descricao Descricao, 
						   ta.tid Tid
					  from {0}crt_dsc_lc_atv_plefluent    ta,
						   {0}crt_dsc_lc_atividade        tb,
						   {0}lov_crt_dsc_lc_atv_ptlanefl la
					 where ta.dsc_lc_atividade = tb.id
					   and ta.tipo_pt_lan_fluente = la.id(+)
					   and tb.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				descricaoLicenAtv.PontosLancamentoEfluente = bancoDeDados.ObterEntityList<PontoLancamentoEfluente>(comando);

				#endregion

				#region Efluentes Líquidos

				comando = bancoDeDados.CriarComando(@"
					select ta.id Id,
						   ta.id IdRelacionamento,
						   ta.tipo_fonte_gerecao TipoId,
						   la.texto TipoTexto,
						   ta.vazao Vazao,
						   ta.unidade UnidadeId,
						   lb.texto UnidadeTexto,
						   ta.sistema_tratamento SistemaTratamento,
						   ta.descricao Descricao,
						   ta.tid Tid
					  from {0}crt_dsc_lc_atv_efl_liqui    ta,
						   {0}crt_dsc_lc_atividade        tb,
						   {0}lov_crt_dsc_lc_atv_font_ger la,
						   {0}lov_crt_dsc_lc_atv_unidade  lb
					 where ta.dsc_lc_atividade = tb.id
					   and ta.tipo_fonte_gerecao = la.id(+)
					   and ta.unidade = lb.id(+)
					   and tb.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				descricaoLicenAtv.EfluentesLiquido = bancoDeDados.ObterEntityList<EfluenteLiquido>(comando);

				#endregion

				#region Resíduos Sólidos não inertes

				comando = bancoDeDados.CriarComando(@"
					select ta.id Id,
						   ta.id IdRelacionamento,
						   ta.classe_residuo ClasseResiduo,
						   ta.tipo_residuo Tipo,
						   ta.acondicionamento AcondicionamentoCodigo,
						   (select stragg(t.texto)
							  from {0}lov_crt_dsc_lc_atv_acondici t
							 where bitand(t.codigo, ta.acondicionamento) > 0) AcondicionamentoTexto,
						   ta.estocagem EstocagemCodigo,
						   (select stragg(t.texto)
							  from {0}lov_crt_dsc_lc_atv_estocage t
							 where bitand(t.codigo, ta.estocagem) > 0) EstocagemTexto,
						   ta.tratamento TratamentoCodigo,
						   (select stragg(t.texto)
							  from {0}lov_crt_dsc_lc_atv_tratamen t
							 where bitand(t.codigo, ta.tratamento) > 0) TratamentoTexto,
						   ta.tratamento_descricao TratamentoDescricao,
						   ta.destino_final DestinoFinalCodigo,
						   (select stragg(t.texto)
							  from {0}lov_crt_dsc_lc_atv_dt_final t
							 where bitand(t.codigo, ta.destino_final) > 0) DestinoFinalTexto,
						   ta.destino_final_descricao DestinoFinalDescricao,
						   ta.tid Tid
					  from {0}crt_dsc_lc_atv_ress_inertes ta, 
						   {0}crt_dsc_lc_atividade tb
					 where ta.dsc_lc_atividade = tb.id
					   and tb.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				descricaoLicenAtv.ResiduosSolidosNaoInerte = bancoDeDados.ObterEntityList<ResiduoSolidoNaoInerte>(comando);

				#endregion

				#region Emissões Atmosféricas

				comando = bancoDeDados.CriarComando(@"
					select ta.id Id,
						   ta.id IdRelacionamento,
						   ta.tipo_combustivel TipoCombustivelId,
						   la.texto TipoCombustivelTexto,
						   ta.substancia_emitida SubstanciaEmitida,
						   ta.equipamento_controle EquipamentoControle,
						   ta.tid Tid
					  from {0}crt_dsc_lc_atv_emissoes_atm ta,
						   {0}crt_dsc_lc_atividade        tb,
						   {0}lov_crt_dsc_lc_atv_combust  la
					 where ta.dsc_lc_atividade = tb.id
					   and ta.tipo_combustivel = la.id(+)
					   and tb.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				descricaoLicenAtv.EmissoesAtmosfericas = bancoDeDados.ObterEntityList<EmissaoAtmosferica>(comando);

				#endregion
			}

			return descricaoLicenAtv;
		}

		public DescricaoLicenciamentoAtividade ObterPorEmpreendimento(int empreendimentoId, eCaracterizacao tipo, bool simplificado = false, BancoDeDados banco = null)
		{
			object objId = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select c.id
					  from {0}crt_dsc_lc_atividade c
					 where c.empreendimento = :empreendimentoId
					   and c.caracterizacao = :caracterizacaoTipo", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimentoId", empreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacaoTipo", tipo, DbType.Int32);

				objId = bancoDeDados.ExecutarScalar(comando);

				if (objId == null || objId == DBNull.Value)
				{
					return new DescricaoLicenciamentoAtividade();
				}

				return Obter(Convert.ToInt32(objId), simplificado, banco);
			}
		
		}

		public DescricaoLicenciamentoAtividade ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			DescricaoLicenciamentoAtividade descricaoLicenAtv = new DescricaoLicenciamentoAtividade();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Descrição de Licenciamento de Atividade

				Comando comando = bancoDeDados.CriarComando(@"
				  select c.id                          Id,
						 c.resp_atividade_id           RespAtividade,
						 c.bacia_hidrografica          BaciaHidrografica,
						 c.existe_app_util             ExisteAppUtil,
						 c.tipo_vegetacao_util         TipoVegetacaoUtilCodigo,
						 c.tipo_vegetacao_util_texto   TipoVegetacaoUtilTexto,
						 c.zona_amort_uc               ZonaAmortUC,
						 c.zona_amort_uc_nome          ZonaAmortUCNomeOrgaoAdm,
						 c.localizada_uc               LocalizadaUC,
						 c.localizada_uc_nome          LocalizadaUCNomeOrgaoAdm,
						 c.patrimonio_historico        PatrimonioHistorico,
						 c.residentes_entorno          ResidentesEntorno,
						 c.residentes_enterno_distanci ResidentesEnternoDistancia,
						 c.area_terreno                AreaTerreno,
						 c.area_util                   AreaUtil,
						 c.total_funcionarios          TotalFuncionarios,
						 c.horas_dias                  HorasDias,
						 c.dias_mes                    DiasMes,
						 c.turnos_dia                  TurnosDia,
						 c.consumo_agua_ls             ConsumoAguaLs,
						 c.consumo_agua_mh             ConsumoAguaMh,
						 c.consumo_agua_mdia           ConsumoAguaMdia,
						 c.consumo_agua_mmes           ConsumoAguaMmes,
						 c.tipo_outorga_id             TipoOutorgaId,
						 c.tipo_outorga_texto          TipoOutorgaTexto,
						 c.numero                      Numero,
						 c.fontes_abastecimento_agua   FonteAbastecimentoAguaTipoId,
						 c.ponto_lancamento            PontoLancamentoTipoId,
						 c.tid                         Tid
					from {0}hst_crt_dsc_lc_atividade c
				   where c.id = :id
					 and c.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				descricaoLicenAtv = bancoDeDados.ObterEntity<DescricaoLicenciamentoAtividade>(comando);

				#endregion

				if (simplificado)
				{
					return descricaoLicenAtv;
				}

				#region Fonte(s) de abastecimento de água

				comando = bancoDeDados.CriarComando(@"
					select ta.id                    Id,
						   ta.tipo_abast_agua_id    TipoId,
						   ta.tipo_abast_agua_texto TipoTexto,
						   ta.descricao             Descricao,
						   ta.tid                   Tid
					  from {0}hst_crt_dsc_lc_atv_ab_agua ta, 
						   {0}hst_crt_dsc_lc_atividade tb
					 where ta.id_hst = tb.id
					   and tb.dsc_lc_atividade_id = :id
					   and tb.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				descricaoLicenAtv.FontesAbastecimentoAgua = bancoDeDados.ObterEntityList<FonteAbastecimentoAgua>(comando);

				#endregion

				#region Pontos de lançamento de efluente

				comando = bancoDeDados.CriarComando(@"
					select ta.id                        Id,
						   ta.tipo_pt_lan_fluente_id    TipoId,
						   ta.tipo_pt_lan_fluente_texto TipoTexto,
						   ta.descricao                 Descricao,
						   ta.tid                       Tid
					  from {0}hst_crt_dsc_lc_atv_plefl    ta,
						   {0}hst_crt_dsc_lc_atividade    tb
					 where ta.id_hst = tb.id
					   and tb.dsc_lc_atividade_id = :id
					   and tb.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				descricaoLicenAtv.PontosLancamentoEfluente = bancoDeDados.ObterEntityList<PontoLancamentoEfluente>(comando);

				#endregion

				#region Efluentes Líquidos

				comando = bancoDeDados.CriarComando(@"
					select ta.id                       Id,
						   ta.tipo_fonte_gerecao_id    TipoId,
						   ta.tipo_fonte_gerecao_texto TipoTexto,
						   ta.vazao                    Vazao,
						   ta.unidade_id               UnidadeId,
						   ta.unidade_texto            UnidadeTexto,
						   ta.sistema_tratamento       SistemaTratamento,
						   ta.descricao                Descricao,
						   ta.tid                      Tid
					  from {0}hst_crt_dsc_lc_atv_eflliqui ta, 
						   {0}hst_crt_dsc_lc_atividade tb
					 where ta.id_hst = tb.id
					   and tb.dsc_lc_atividade_id = :id
					   and tb.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				descricaoLicenAtv.EfluentesLiquido = bancoDeDados.ObterEntityList<EfluenteLiquido>(comando);

				#endregion

				#region Resíduos Sólidos não inertes

				comando = bancoDeDados.CriarComando(@"
					select ta.id                      Id,
						   ta.classe_residuo          ClasseResiduo,
						   ta.tipo_residuo            Tipo,
						   ta.acondicionamento        AcondicionamentoCodigo,
						   ta.acondicionamento_texto  AcondicionamentoTexto,
						   ta.estocagem               EstocagemCodigo,
						   ta.estocagem_texto         EstocagemTexto,
						   ta.tratamento              TratamentoCodigo,
						   ta.tratamento_texto        TratamentoTexto,
						   ta.tratamento_descricao    DestinoFinalDescricao,
						   ta.destino_final           DestinoFinal,
						   ta.destino_final_texto     DestinoFinalTexto,
						   ta.destino_final_descricao DestinoFinalDescricao,
						   ta.tid                     Tid
					  from {0}hst_crt_dsc_lc_atv_res_iner ta, 
						   {0}hst_crt_dsc_lc_atividade tb
					 where ta.id_hst = tb.id
					   and tb.dsc_lc_atividade_id = :id
					   and tb.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				descricaoLicenAtv.ResiduosSolidosNaoInerte = bancoDeDados.ObterEntityList<ResiduoSolidoNaoInerte>(comando);

				#endregion

				#region Emissões Atmosféricas

				comando = bancoDeDados.CriarComando(@"
					select ta.id                   Id,
						   ta.tipo_combustivel     TipoCombustivelId,
						   la.texto                TipoCombustivelTexto,
						   ta.substancia_emitida   SubstanciaEmitida,
						   ta.equipamento_controle EquipamentoControle,
						   ta.tid                  Tid
					  from {0}hst_crt_dsc_lc_atv_ems_atm ta, 
						   {0}hst_crt_dsc_lc_atividade tb
					 where ta.id_hst = tb.id
					   and tb.dsc_lc_atividade_id = :id
					   and tb.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				descricaoLicenAtv.EmissoesAtmosfericas = bancoDeDados.ObterEntityList<EmissaoAtmosferica>(comando);

				#endregion
			}

			return descricaoLicenAtv;
		}

		public DescricaoLicenciamentoAtividade ObterDadosGeo(int id, eCaracterizacao tipo, BancoDeDados banco = null)
		{
			DescricaoLicenciamentoAtividade descricaoLicenAtv = new DescricaoLicenciamentoAtividade();
			int idProjeto = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Descrição de Licenciamento de Atividade

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_projeto_geo c where c.empreendimento = :empreendimento and c.caracterizacao = :caracterizacao", EsquemaBanco);
				comando.AdicionarParametroEntrada(":empreendimento", id, DbType.Int32);
				comando.AdicionarParametroEntrada(":caracterizacao", (int)tipo, DbType.Int32);

				idProjeto = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando = bancoDeDados.CriarComando(@"
					select (select a.area_m2 from {1}geo_atp a where a.projeto = ( select g.id from {0}crt_projeto_geo g where g.empreendimento = :empreendimento and g.caracterizacao =  1)) AreaTerreno,
						(select s.identificacao
								from {0}crt_projeto_geo_sobrepos s
							   where s.projeto = :projeto
								 and s.tipo = 2) BaciaHidrografica,
							   (select count(*)
								from {1}geo_areas_calculadas c
							   where c.tipo = 'APP_APMP'
								 and c.projeto = ( select g.id from {0}crt_projeto_geo g where g.empreendimento = :empreendimento and g.caracterizacao =  1)) ExisteAppUtil,
							   (select sum(k.valor_tipo)
								from (select 1 valor_tipo
									from dual
								   where exists (select 1 from (
										  select count(*) qtd from {1}geo_aativ c where c.projeto = :projeto and c.avn <> 'N'
												 union
										  select count(*) qtd from {1}geo_pativ c where c.projeto = :projeto and c.avn <> 'N' 
												 union
										  select count(*) qtd from {1}geo_lativ c where c.projeto = :projeto and c.avn <> 'N') where qtd > 0)
								union all
								  select 2 valor_tipo
									from dual
								   where exists ( 
									  select 1
										from (
										   select count(*) qtd from {1}geo_aativ c where c.projeto = :projeto and c.aa <> 'N'
												  union
										   select count(*) qtd from {1}geo_pativ c where c.projeto = :projeto and c.aa <> 'N'
												  union
										   select count(*) qtd from {1}geo_lativ c where c.projeto = :projeto and c.aa <> 'N') where qtd > 0)
										   ) k) TipoVegetacaoUtilCodigo,
							   (select stragg_barra(s.identificacao)
								from {0}crt_projeto_geo_sobrepos s
							   where s.projeto = :projeto
								 and s.tipo in (4, 5, 6, 7)
							   group by s.projeto) LocalizadaUCNomeOrgaoAdm
							from dual", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada(":empreendimento", id, DbType.Int32);
				comando.AdicionarParametroEntrada(":projeto", idProjeto, DbType.Int32);

				descricaoLicenAtv = bancoDeDados.ObterEntity<DescricaoLicenciamentoAtividade>(comando);

				#endregion		
			}

			return descricaoLicenAtv;
		}

		public List<PessoaLst> ObterResponsaveis(int id, BancoDeDados banco = null)
		{
			List<PessoaLst> responsaveis = new List<PessoaLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select p.id Id, 
						   nvl(p.nome, p.razao_social) Texto, 
						   r.tipo VinculoTipo
					  from {0}tab_empreendimento_responsavel r, 
						   {0}tab_pessoa p
					 where r.responsavel = p.id
					   and r.empreendimento = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				responsaveis = bancoDeDados.ObterEntityList<PessoaLst>(comando);
			}

			return responsaveis;
		}

		#endregion
	}
}

