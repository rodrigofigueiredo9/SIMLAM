using System;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{
	public class AcompanhamentoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }

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

		#region Ações de DML

		public Acompanhamento Salvar(Acompanhamento acompanhamento, BancoDeDados banco = null)
		{
			if (acompanhamento == null)
			{
				throw new Exception("Acompanhamento é nulo.");
			}

			if (acompanhamento.Id <= 0)
			{
				acompanhamento = Criar(acompanhamento, banco);
			}
			else
			{
				acompanhamento = Editar(acompanhamento, banco);
			}

			return acompanhamento;
		}

		internal Acompanhamento Criar(Acompanhamento acompanhamento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Acompanhamento da Fiscalização

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into tab_acompanhamento_fisc(id, fiscalizacao, numero_sufixo, data_vistoria, situacao, data_situacao, 
				agente_fiscal, setor, area_total, area_florestal_nativa, reserva_legal, possui_area_embargada, opniao_area_embargo, ativ_area_embargada, atviv_area_embargada_especific, uso_area_solo, 
				caract_solo_area_danificada, declividade_media_area, infr_resultou_erosao, infr_resultou_erosao_especific, houve_apreensao_material, opniao_destin_material_apreend, 
				houve_desrespeito_tad, houve_desrespeito_tad_especifi, informacoes_relevante_processo, neces_repar_dano_amb, neces_repar_dano_amb_especific, 
				firmou_termo_comprom, firmou_termo_comprom_especific, arquivo_termo, tid) 
				values (seq_tab_acompanhamento_fisc.nextval, :fiscalizacao, :numero_sufixo, :data_vistoria, 1, :data_situacao, :agente_fiscal, :setor, 
				:area_total, :area_florestal_nativa, :reserva_legal, :possui_area_embargada, :opniao_area_embargo, :ativ_area_embargada, :atviv_area_embargada_especific, :uso_area_solo, 
				:caract_solo_area_danificada, :declividade_media_area, :infr_resultou_erosao, :infr_resultou_erosao_especific, :houve_apreensao_material, :opniao_destin_material_apreend, 
				:houve_desrespeito_tad, :houve_desrespeito_tad_especifi, :informacoes_relevante_processo, :neces_repar_dano_amb, :neces_repar_dano_amb_especific, 
				:firmou_termo_comprom, :firmou_termo_comprom_especific, :arquivo_termo, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", acompanhamento.FiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_sufixo", acompanhamento.NumeroSufixo, DbType.String);
				comando.AdicionarParametroEntrada("data_vistoria", acompanhamento.DataVistoria.DataTexto, DbType.Date);
				comando.AdicionarParametroEntrada("data_situacao", acompanhamento.DataVistoria.DataTexto, DbType.Date);
				comando.AdicionarParametroEntrada("agente_fiscal", acompanhamento.AgenteId, DbType.Int32);
				comando.AdicionarParametroEntrada("setor", acompanhamento.SetorId, DbType.Int32);
				comando.AdicionarParametroEntrada("area_total", acompanhamento.AreaTotal, DbType.Decimal);
				comando.AdicionarParametroEntrada("area_florestal_nativa", acompanhamento.AreaFlorestalNativa, DbType.Decimal);
				comando.AdicionarParametroEntrada("reserva_legal", acompanhamento.ReservalegalTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_area_embargada", acompanhamento.PossuiAreaEmbargadaOuAtividadeInterditada.HasValue ? Convert.ToInt32(acompanhamento.PossuiAreaEmbargadaOuAtividadeInterditada.Value): (Object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("opniao_area_embargo", acompanhamento.OpniaoAreaEmbargo, DbType.String);
				comando.AdicionarParametroEntrada("ativ_area_embargada", acompanhamento.AtividadeAreaEmbargada, DbType.Int32);
				comando.AdicionarParametroEntrada("atviv_area_embargada_especific", acompanhamento.AtividadeAreaEmbargadaEspecificarTexto, DbType.String);
				comando.AdicionarParametroEntrada("uso_area_solo", acompanhamento.UsoAreaSoloDescricao, DbType.String);
				comando.AdicionarParametroEntrada("caract_solo_area_danificada", acompanhamento.CaracteristicaSoloAreaDanificada, DbType.Int32);
				comando.AdicionarParametroEntrada("declividade_media_area", acompanhamento.AreaDeclividadeMedia, DbType.Decimal);
				comando.AdicionarParametroEntrada("infr_resultou_erosao", acompanhamento.InfracaoResultouErosao, DbType.Int32);
				comando.AdicionarParametroEntrada("infr_resultou_erosao_especific", acompanhamento.InfracaoResultouErosaoEspecificar, DbType.String);
				comando.AdicionarParametroEntrada("houve_apreensao_material", acompanhamento.HouveApreensaoMaterial.HasValue ? Convert.ToInt32(acompanhamento.HouveApreensaoMaterial.Value) : (Object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("opniao_destin_material_apreend", acompanhamento.OpniaoDestMaterialApreend, DbType.String);
				comando.AdicionarParametroEntrada("houve_desrespeito_tad", acompanhamento.HouveDesrespeitoTAD, DbType.Int32);
				comando.AdicionarParametroEntrada("houve_desrespeito_tad_especifi", acompanhamento.HouveDesrespeitoTADEspecificar, DbType.String);
				comando.AdicionarParametroEntClob("informacoes_relevante_processo", acompanhamento.InformacoesRelevanteProcesso);
				comando.AdicionarParametroEntrada("neces_repar_dano_amb", acompanhamento.RepararDanoAmbiental, DbType.Int32);
				comando.AdicionarParametroEntrada("neces_repar_dano_amb_especific", acompanhamento.RepararDanoAmbientalEspecificar, DbType.String);
				comando.AdicionarParametroEntrada("firmou_termo_comprom", acompanhamento.FirmouTermoRepararDanoAmbiental, DbType.Int32);
				comando.AdicionarParametroEntrada("firmou_termo_comprom_especific", acompanhamento.FirmouTermoRepararDanoAmbientalEspecificar, DbType.String);
				comando.AdicionarParametroEntrada("arquivo_termo", (acompanhamento.Arquivo ?? new Arquivo()).Id, DbType.Int32);				
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				acompanhamento.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion Acompanhamento da Fiscalização

				#region Assinantes

				foreach (var item in acompanhamento.Assinantes)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_acomp_fisc_assinante(id, acompanhamento, funcionario, cargo, tid)
					values ({0}seq_tab_acomp_fisc_assinante.nextval, :acompanhamento, :funcionario, :cargo, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("acompanhamento", acompanhamento.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("funcionario", item.FuncionarioId, DbType.Int32);
					comando.AdicionarParametroEntrada("cargo", item.FuncionarioCargoId, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion Assinantes

				#region Anexos

				foreach (var item in acompanhamento.Anexos)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_acomp_fisc_arquivo a (id, acompanhamento, arquivo, ordem, descricao, tid) 
					values ({0}seq_tab_acomp_fisc_arquivo.nextval, :acompanhamento, :arquivo, :ordem, :descricao, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("acompanhamento", acompanhamento.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("arquivo", item.Arquivo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("ordem", item.Ordem, DbType.Int32);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion Anexos

				Historico.Gerar(acompanhamento.Id, eHistoricoArtefato.acompanhamento, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}

			return acompanhamento;
		}

		internal Acompanhamento Editar(Acompanhamento acompanhamento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Acompanhamento da Fiscalização

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_acompanhamento_fisc a set a.data_vistoria = :data_vistoria, a.agente_fiscal = :agente_fiscal, 
				a.setor = :setor, a.area_total = :area_total, a.area_florestal_nativa = :area_florestal_nativa, a.reserva_legal = :reserva_legal,  a.possui_area_embargada = :possui_area_embargada,
				a.opniao_area_embargo = :opniao_area_embargo, a.ativ_area_embargada = :ativ_area_embargada, a.atviv_area_embargada_especific = :atviv_area_embargada_especific, 
				a.uso_area_solo = :uso_area_solo, a.caract_solo_area_danificada = :caract_solo_area_danificada, a.declividade_media_area = :declividade_media_area, 
				a.infr_resultou_erosao = :infr_resultou_erosao, a.infr_resultou_erosao_especific = :infr_resultou_erosao_especific,  a.houve_apreensao_material = :houve_apreensao_material,
				a.opniao_destin_material_apreend = :opniao_destin_material_apreend, a.houve_desrespeito_tad = :houve_desrespeito_tad, 
				a.houve_desrespeito_tad_especifi = :houve_desrespeito_tad_especifi, a.informacoes_relevante_processo = :informacoes_relevante_processo, 
				a.neces_repar_dano_amb = :neces_repar_dano_amb, a.neces_repar_dano_amb_especific = :neces_repar_dano_amb_especific, a.firmou_termo_comprom = :firmou_termo_comprom, 
				a.firmou_termo_comprom_especific = :firmou_termo_comprom_especific, a.arquivo_termo = :arquivo_termo, a.tid = :tid where a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("data_vistoria", acompanhamento.DataVistoria.DataTexto, DbType.Date);
				comando.AdicionarParametroEntrada("agente_fiscal", acompanhamento.AgenteId, DbType.Int32);
				comando.AdicionarParametroEntrada("setor", acompanhamento.SetorId, DbType.Int32);
				comando.AdicionarParametroEntrada("area_total", acompanhamento.AreaTotal, DbType.Decimal);
				comando.AdicionarParametroEntrada("area_florestal_nativa", acompanhamento.AreaFlorestalNativa, DbType.Decimal);
				comando.AdicionarParametroEntrada("reserva_legal", acompanhamento.ReservalegalTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_area_embargada", acompanhamento.PossuiAreaEmbargadaOuAtividadeInterditada.HasValue ? Convert.ToInt32(acompanhamento.PossuiAreaEmbargadaOuAtividadeInterditada.Value) : (Object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("opniao_area_embargo", acompanhamento.OpniaoAreaEmbargo, DbType.String);
				comando.AdicionarParametroEntrada("ativ_area_embargada", acompanhamento.AtividadeAreaEmbargada, DbType.Int32);
				comando.AdicionarParametroEntrada("atviv_area_embargada_especific", acompanhamento.AtividadeAreaEmbargadaEspecificarTexto, DbType.String);
				comando.AdicionarParametroEntrada("uso_area_solo", acompanhamento.UsoAreaSoloDescricao, DbType.String);
				comando.AdicionarParametroEntrada("caract_solo_area_danificada", acompanhamento.CaracteristicaSoloAreaDanificada, DbType.Int32);
				comando.AdicionarParametroEntrada("declividade_media_area", acompanhamento.AreaDeclividadeMedia, DbType.Decimal);
				comando.AdicionarParametroEntrada("infr_resultou_erosao", acompanhamento.InfracaoResultouErosao, DbType.Int32);
				comando.AdicionarParametroEntrada("infr_resultou_erosao_especific", acompanhamento.InfracaoResultouErosaoEspecificar, DbType.String);
				comando.AdicionarParametroEntrada("houve_apreensao_material", acompanhamento.HouveApreensaoMaterial.HasValue ? Convert.ToInt32(acompanhamento.HouveApreensaoMaterial.Value) : (Object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("opniao_destin_material_apreend", acompanhamento.OpniaoDestMaterialApreend, DbType.String);
				comando.AdicionarParametroEntrada("houve_desrespeito_tad", acompanhamento.HouveDesrespeitoTAD, DbType.Int32);
				comando.AdicionarParametroEntrada("houve_desrespeito_tad_especifi", acompanhamento.HouveDesrespeitoTADEspecificar, DbType.String);
				comando.AdicionarParametroEntClob("informacoes_relevante_processo", acompanhamento.InformacoesRelevanteProcesso);
				comando.AdicionarParametroEntrada("neces_repar_dano_amb", acompanhamento.RepararDanoAmbiental, DbType.Int32);
				comando.AdicionarParametroEntrada("neces_repar_dano_amb_especific", acompanhamento.RepararDanoAmbientalEspecificar, DbType.String);
				comando.AdicionarParametroEntrada("firmou_termo_comprom", acompanhamento.FirmouTermoRepararDanoAmbiental, DbType.Int32);
				comando.AdicionarParametroEntrada("firmou_termo_comprom_especific", acompanhamento.FirmouTermoRepararDanoAmbientalEspecificar, DbType.String);
				comando.AdicionarParametroEntrada("arquivo_termo", (acompanhamento.Arquivo ?? new Arquivo()).Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", acompanhamento.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion Acompanhamento da Fiscalização

				#region Limpar Dados

				comando = bancoDeDados.CriarComando("delete from {0}tab_acomp_fisc_assinante a ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where a.acompanhamento = :acompanhamento{0}",
					comando.AdicionarNotIn("and", "a.id", DbType.Int32, acompanhamento.Assinantes.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("acompanhamento", acompanhamento.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando("delete from {0}tab_acomp_fisc_arquivo a ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where a.acompanhamento = :acompanhamento{0}",
					comando.AdicionarNotIn("and", "a.id", DbType.Int32, acompanhamento.Anexos.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("acompanhamento", acompanhamento.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion Limpar Dados

				#region Assinantes

				foreach (var item in acompanhamento.Assinantes)
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_acomp_fisc_assinante t 
						set t.funcionario = :funcionario, t.cargo = :cargo, t.tid = :tid where t.id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_acomp_fisc_assinante(id, acompanhamento, funcionario, cargo, tid)
						values ({0}seq_tab_acomp_fisc_assinante.nextval, :acompanhamento, :funcionario, :cargo, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("acompanhamento", acompanhamento.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("funcionario", item.FuncionarioId, DbType.Int32);
					comando.AdicionarParametroEntrada("cargo", item.FuncionarioCargoId, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion Assinantes

				#region Anexos

				foreach (var item in acompanhamento.Anexos)
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_acomp_fisc_arquivo t 
						set t.arquivo = :arquivo, t.ordem = :ordem, t.descricao = :descricao, t.tid = :tid where t.id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_acomp_fisc_arquivo a (id, acompanhamento,  arquivo, ordem, descricao, tid) 
						values ({0}seq_tab_acomp_fisc_arquivo.nextval, :acompanhamento, :arquivo, :ordem, :descricao,:tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("acompanhamento", acompanhamento.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("arquivo", item.Arquivo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("ordem", item.Ordem, DbType.Int32);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion Anexos

				Historico.Gerar(acompanhamento.Id, eHistoricoArtefato.acompanhamento, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}

			return acompanhamento;
		}

		public void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_acompanhamento_fisc t set t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.acompanhamento, eHistoricoAcao.excluir, bancoDeDados);

				comando = bancoDeDados.CriarComandoPlSql(@"begin 
					delete {0}tab_acomp_fisc_arquivo t where t.acompanhamento = :acompanhamento;
					delete {0}tab_acomp_fisc_assinante t where t.acompanhamento = :acompanhamento;
					delete {0}tab_acompanhamento_fisc t where t.id = :acompanhamento;
					end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("acompanhamento", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(Acompanhamento acompanhamento, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_acompanhamento_fisc t set 
				t.situacao = :situacao, t.data_situacao = :data_situacao, t.motivo_situacao = :motivo_situacao, t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", acompanhamento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", acompanhamento.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("data_situacao", acompanhamento.DataSituacao.DataTexto, DbType.Date);
				comando.AdicionarParametroEntrada("motivo_situacao", DbType.String, 500, acompanhamento.Motivo);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void SalvarArquivo(Acompanhamento acompanhamento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_acompanhamento_fisc t set t.arquivo = :arquivo, t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("arquivo", acompanhamento.PdfLaudo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", acompanhamento.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		internal Acompanhamento Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			Acompanhamento acompanhamento = new Acompanhamento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Acompanhamento da Fiscalização

				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.fiscalizacao, t.numero_sufixo, t.data_vistoria, t.situacao, ls.texto situacao_texto, t.data_situacao, 
				t.motivo_situacao, t.agente_fiscal, f.nome agente_fiscal_nome, t.setor, t.area_total, t.area_florestal_nativa, t.reserva_legal, t.possui_area_embargada, 
				t.opniao_area_embargo, t.ativ_area_embargada, t.atviv_area_embargada_especific, t.uso_area_solo, t.caract_solo_area_danificada, t.declividade_media_area, 
				t.infr_resultou_erosao, t.infr_resultou_erosao_especific, t.houve_apreensao_material, t.opniao_destin_material_apreend, t.houve_desrespeito_tad, 
				t.houve_desrespeito_tad_especifi, t.informacoes_relevante_processo, t.neces_repar_dano_amb, t.neces_repar_dano_amb_especific, t.firmou_termo_comprom, 
				t.firmou_termo_comprom_especific, t.arquivo_termo arquivo_termo_id, ter.nome arquivo_termo_nome, t.arquivo, t.tid from tab_acompanhamento_fisc t, 
				lov_acomp_fisc_situacao ls, tab_funcionario f, tab_arquivo ter where t.id = :id and t.situacao = ls.id and t.agente_fiscal = f.id 
				and t.arquivo_termo = ter.id(+)", EsquemaBanco);

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
						acompanhamento.Motivo = reader.GetValue<string>("motivo_situacao");
						acompanhamento.AgenteId = reader.GetValue<int>("agente_fiscal");
						acompanhamento.AgenteNome = reader.GetValue<string>("agente_fiscal_nome");
						acompanhamento.SetorId = reader.GetValue<int>("setor");
						acompanhamento.AreaTotal = reader.GetValue<decimal>("area_total").ToStringTrunc(4);
						acompanhamento.AreaFlorestalNativa = reader.GetValue<decimal>("area_florestal_nativa").ToStringTrunc(4);
						acompanhamento.ReservalegalTipo = reader.GetValue<int>("reserva_legal");
						acompanhamento.PossuiAreaEmbargadaOuAtividadeInterditada = reader.GetValue<bool?>("possui_area_embargada");
						acompanhamento.OpniaoAreaEmbargo = reader.GetValue<string>("opniao_area_embargo");
						acompanhamento.AtividadeAreaEmbargada = reader.GetValue<int>("ativ_area_embargada");
						acompanhamento.AtividadeAreaEmbargadaEspecificarTexto = reader.GetValue<string>("atviv_area_embargada_especific");
						acompanhamento.UsoAreaSoloDescricao = reader.GetValue<string>("uso_area_solo");
						acompanhamento.CaracteristicaSoloAreaDanificada = reader.GetValue<int>("caract_solo_area_danificada");
						acompanhamento.AreaDeclividadeMedia = reader.GetValue<decimal>("declividade_media_area").ToStringTrunc();
						acompanhamento.InfracaoResultouErosao = reader.GetValue<int>("infr_resultou_erosao");
						acompanhamento.InfracaoResultouErosaoEspecificar = reader.GetValue<string>("infr_resultou_erosao_especific");
						acompanhamento.HouveApreensaoMaterial = reader.GetValue<bool?>("houve_apreensao_material");
						acompanhamento.OpniaoDestMaterialApreend = reader.GetValue<string>("opniao_destin_material_apreend");
						acompanhamento.HouveDesrespeitoTAD = reader.GetValue<int>("houve_desrespeito_tad");
						acompanhamento.HouveDesrespeitoTADEspecificar = reader.GetValue<string>("houve_desrespeito_tad_especifi");
						acompanhamento.InformacoesRelevanteProcesso = reader.GetValue<string>("informacoes_relevante_processo");
						acompanhamento.RepararDanoAmbiental = reader.GetValue<int>("neces_repar_dano_amb");
						acompanhamento.RepararDanoAmbientalEspecificar = reader.GetValue<string>("neces_repar_dano_amb_especific");
						acompanhamento.FirmouTermoRepararDanoAmbiental = reader.GetValue<int>("firmou_termo_comprom");
						acompanhamento.FirmouTermoRepararDanoAmbientalEspecificar = reader.GetValue<string>("firmou_termo_comprom_especific");
						acompanhamento.PdfLaudo.Id = reader.GetValue<int>("arquivo");
						acompanhamento.Tid = reader.GetValue<string>("tid");


						if (reader["arquivo_termo_id"] != null && !Convert.IsDBNull(reader["arquivo_termo_id"]))
						{
							Arquivo arquivo = new Arquivo();
							arquivo.Id = reader.GetValue<Int32>("arquivo_termo_id");
							arquivo.Nome = reader.GetValue<String>("arquivo_termo_nome");
							acompanhamento.Arquivo = arquivo;
						}
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
					FiscalizacaoAssinante item;

					while (reader.Read())
					{
						item = new FiscalizacaoAssinante();
						item.Id = reader.GetValue<int>("id");
						item.Tid = reader.GetValue<string>("tid");
						item.FuncionarioId = reader.GetValue<int>("func_id");
						item.FuncionarioNome = reader.GetValue<string>("func_nome");
						item.FuncionarioCargoId = reader.GetValue<int>("cargo");
						item.FuncionarioCargoNome = reader.GetValue<string>("cargo_nome");
						item.Selecionado = true;

						acompanhamento.Assinantes.Add(item);
					}

					reader.Close();
				}

				#endregion Assinantes

				#region Anexos

				comando = bancoDeDados.CriarComando(@"select a.id Id, a.ordem Ordem, a.descricao Descricao, b.nome, b.extensao, b.id arquivo_id, b.caminho, a.tid Tid
				from {0}tab_acomp_fisc_arquivo a, {0}tab_arquivo b where a.arquivo = b.id and a.acompanhamento = :acompanhamento order by a.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("acompanhamento", acompanhamento.Id, DbType.Int32);

				acompanhamento.Anexos = bancoDeDados.ObterEntityList<Anexo>(comando, (IDataReader reader, Anexo item) =>
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

		internal Resultados<Acompanhamento> ObterAcompanhamentos(int fiscalizacao, BancoDeDados banco = null)
		{
			Resultados<Acompanhamento> retorno = new Resultados<Acompanhamento>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.numero_sufixo, ter.id arquivo_termo_id, ter.nome arquivo_termo_nome,
															t.data_vistoria, t.situacao, ls.texto situacao_texto, t.data_situacao 
															from tab_acompanhamento_fisc t, lov_acomp_fisc_situacao ls, tab_arquivo ter
															where t.situacao = ls.id and t.arquivo_termo = ter.id(+) and t.fiscalizacao = :fiscalizacao
															order by t.id");

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Acompanhamento acompanhamento;

					while (reader.Read())
					{
						acompanhamento = new Acompanhamento();
						acompanhamento.FiscalizacaoId = fiscalizacao;
						acompanhamento.Id = reader.GetValue<int>("id");
						acompanhamento.NumeroSufixo = reader.GetValue<string>("numero_sufixo");
						acompanhamento.DataVistoria.DataTexto = reader.GetValue<string>("data_vistoria");
						acompanhamento.SituacaoId = reader.GetValue<int>("situacao");
						acompanhamento.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						acompanhamento.DataSituacao.DataTexto = reader.GetValue<string>("data_situacao");

						if (reader["arquivo_termo_id"] != null && !Convert.IsDBNull(reader["arquivo_termo_id"]))
						{
							Arquivo arquivo = new Arquivo();
							arquivo.Id = reader.GetValue<int>("arquivo_termo_id");
							arquivo.Nome = reader.GetValue<string>("arquivo_termo_nome");
							acompanhamento.Arquivo = arquivo;
						}

						retorno.Itens.Add(acompanhamento);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		#endregion

		#region Validação

		internal bool ExisteAcompanhamento(int fiscalizacao, BancoDeDados banco = null)
		{
			Acompanhamento acompanhamento = new Acompanhamento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_acompanhamento_fisc a where a.fiscalizacao = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion
	}
}