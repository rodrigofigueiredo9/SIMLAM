using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCadastroAmbientalRural.Data
{
	public class CadastroAmbientalRuralDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		CaracterizacaoDa _caracterizacaoDa = new CaracterizacaoDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		internal Historico Historico { get { return _historico; } }

		private String EsquemaBanco { get; set; }

		internal int Tipo { get { return (int)eCaracterizacao.CadastroAmbientalRural; } }

		#endregion

		public CadastroAmbientalRuralDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal int Processar(CadastroAmbientalRural caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				declare
					v_projeto number(38) := 0;
					v_fila number(38) := 0;
				begin
					select nvl((select p.id from {0}tmp_projeto_geo p where p.empreendimento = :empreendimento and p.caracterizacao = :caracterizacao), 0) into v_projeto from dual;

					if(v_projeto > 0) then
						update {0}tmp_projeto_geo p set p.tid = :tid where p.id = v_projeto;
					else
						insert into {0}tmp_projeto_geo p (id, empreendimento, caracterizacao, situacao, tid) 
						values ({0}seq_tmp_projeto_geo.nextval, :empreendimento, :caracterizacao, :situacao, :tid) returning p.id into v_projeto;
					end if;
					
					update {0}tmp_cad_ambiental_rural c set c.projeto_geo_id = v_projeto where c.empreendimento = :empreendimento;
					
					select nvl((select f.id from {1}tab_fila f where f.projeto = v_projeto), 0) into v_fila from dual;

					if(v_fila > 0) then
						update {1}tab_fila f set f.etapa = :etapa, f.situacao = :situacao_fila where f.projeto = v_projeto and f.tipo = :tipo;
					else
						insert into {1}tab_fila f (id, empreendimento, projeto, tipo, mecanismo_elaboracao, etapa, situacao, data_fila)
						(select {1}seq_fila.nextval, :empreendimento, t.id, :tipo, :mecanismo, :etapa, :situacao_fila, sysdate from {0}tmp_projeto_geo t where t.id = v_projeto);
					end if;

					:projetoGeoOut := v_projeto;
				end;", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)eProjetoGeograficoSituacao.EmElaboracao, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", caracterizacao.Arquivo.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("mecanismo", caracterizacao.Arquivo.Mecanismo, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", caracterizacao.Arquivo.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_fila", caracterizacao.Arquivo.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("projetoGeoOut", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.ProjetoGeoId = comando.ObterValorParametro<Int32>("projetoGeoOut");
				#endregion

				bancoDeDados.Commit();

				return caracterizacao.ProjetoGeoId;
			}
		}

		internal void SalvarTemporaria(CadastroAmbientalRural caracterizacao, BancoDeDados banco)
		{
			if (caracterizacao == null)
			{
				throw new Exception("A Caracterização de Cadastro Ambiental Rural é nula.");
			}

			if (caracterizacao.Id <= 0)
			{
				CriarTemporaria(caracterizacao, banco);
			}
			else
			{
				EditarTemporaria(caracterizacao, banco);
			}
		}

		internal void CriarTemporaria(CadastroAmbientalRural caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Caracterização

				bancoDeDados.IniciarTransacao();

				Comando comando;

				if (caracterizacao.Id <= 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tmp_cad_ambiental_rural c (
					id, empreendimento, ocorreu_alteracao_apos_2008, atp_documento_2008, situacao, municipio, modulo_fiscal, atp_qtd_modulo_fiscal, tid, dispensa_arl, vistoria_aprovacao,
					data_vistoria, empreendimento_cedente, empreendimento_receptor) values 
					({0}seq_tmp_cad_ambiental_rural.nextval, :empreendimento, :ocorreu_alteracao_apos_2008, :atp_documento_2008, :situacao, :municipio, :modulo_fiscal, 
					:atp_qtd_modulo_fiscal, :tid, :dispensa_arl, :vistoria_aprovacao, :data_vistoria, :empreendimento_cedente, :empreendimento_receptor) returning id into :id", EsquemaBanco);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tmp_cad_ambiental_rural c (
					id, empreendimento, projeto_geo_id, ocorreu_alteracao_apos_2008, atp_documento_2008, situacao, municipio, modulo_fiscal, atp_qtd_modulo_fiscal, tid, dispensa_arl, vistoria_aprovacao,
					data_vistoria, empreendimento_cedente, empreendimento_receptor) values 
					(:caracterizacao_id, :empreendimento, :projeto_geo_id, :ocorreu_alteracao_apos_2008, :atp_documento_2008, :situacao, :municipio, :modulo_fiscal, 
					:atp_qtd_modulo_fiscal, :tid, :dispensa_arl, :vistoria_aprovacao, :data_vistoria, :empreendimento_cedente, :empreendimento_receptor) returning id into :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("caracterizacao_id", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("projeto_geo_id", caracterizacao.ProjetoGeoId, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("ocorreu_alteracao_apos_2008", caracterizacao.OcorreuAlteracaoApos2008, DbType.Int32);
				comando.AdicionarParametroEntrada("atp_documento_2008", caracterizacao.OcorreuAlteracaoApos2008 > 0 ? caracterizacao.ATPDocumento2008 : null, DbType.Decimal);
				comando.AdicionarParametroEntrada("situacao", caracterizacao.Situacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", caracterizacao.MunicipioId, DbType.Int32);
				comando.AdicionarParametroEntrada("modulo_fiscal", caracterizacao.ModuloFiscalId, DbType.Int32);
				comando.AdicionarParametroEntrada("atp_qtd_modulo_fiscal", caracterizacao.ATPQuantidadeModuloFiscal, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				comando.AdicionarParametroEntrada("dispensa_arl", Convert.ToInt32(caracterizacao.DispensaARL), DbType.Int32);
				comando.AdicionarParametroEntrada("vistoria_aprovacao", caracterizacao.VistoriaAprovacaoCAR < 0 ? (object)DBNull.Value : caracterizacao.VistoriaAprovacaoCAR, DbType.Int32);
				comando.AdicionarParametroEntrada("data_vistoria", caracterizacao.VistoriaAprovacaoCAR > 0 ? caracterizacao.DataVistoriaAprovacao.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("empreendimento_cedente", caracterizacao.EmpreendimentoCedenteId.GetValueOrDefault() > 0 ? caracterizacao.EmpreendimentoCedenteId : null, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento_receptor", caracterizacao.EmpreendimentoReceptorId.GetValueOrDefault() > 0 ? caracterizacao.EmpreendimentoReceptorId : null, DbType.Int32);

				comando.AdicionarParametroSaida("id", DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion Caracterização

				#region Áreas

				//Não precisa salvar as Areas na tabela temporária

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void EditarTemporaria(CadastroAmbientalRural caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Caracterização

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tmp_cad_ambiental_rural c set c.ocorreu_alteracao_apos_2008 = :ocorreu_alteracao_apos_2008, 
				c.atp_documento_2008 = :atp_documento_2008, c.situacao = :situacao, c.municipio = :municipio, c.modulo_fiscal = :modulo_fiscal, 
				c.atp_qtd_modulo_fiscal = :atp_qtd_modulo_fiscal, c.dispensa_arl = :dispensa_arl, c.tid = :tid, vistoria_aprovacao = :vistoria_aprovacao, 
				data_vistoria = :data_vistoria, empreendimento_cedente =:empreendimento_cedente, empreendimento_receptor =: empreendimento_receptor where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("ocorreu_alteracao_apos_2008", caracterizacao.OcorreuAlteracaoApos2008, DbType.Int32);
				comando.AdicionarParametroEntrada("atp_documento_2008", caracterizacao.OcorreuAlteracaoApos2008 > 0 ? caracterizacao.ATPDocumento2008 : null, DbType.Decimal);
				comando.AdicionarParametroEntrada("situacao", caracterizacao.Situacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", caracterizacao.MunicipioId, DbType.Int32);
				comando.AdicionarParametroEntrada("modulo_fiscal", caracterizacao.ModuloFiscalId, DbType.Int32);
				comando.AdicionarParametroEntrada("atp_qtd_modulo_fiscal", caracterizacao.ATPQuantidadeModuloFiscal, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				comando.AdicionarParametroEntrada("dispensa_arl", Convert.ToInt32(caracterizacao.DispensaARL), DbType.Int32);
				comando.AdicionarParametroEntrada("vistoria_aprovacao", caracterizacao.VistoriaAprovacaoCAR < 0 ? (object)DBNull.Value : caracterizacao.VistoriaAprovacaoCAR, DbType.Int32);
				comando.AdicionarParametroEntrada("data_vistoria", caracterizacao.VistoriaAprovacaoCAR > 0 ? caracterizacao.DataVistoriaAprovacao.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("empreendimento_cedente", caracterizacao.EmpreendimentoCedenteId.GetValueOrDefault() > 0 ? caracterizacao.EmpreendimentoCedenteId : null, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento_receptor", caracterizacao.EmpreendimentoReceptorId.GetValueOrDefault() > 0 ? caracterizacao.EmpreendimentoReceptorId : null, DbType.Int32);

				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion Caracterização

				#region Áreas

				//Não precisa salvar as Areas na tabela temporária

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				int caracterizacao = 0;
				int projeto = 0;

				bancoDeDados.IniciarTransacao();

				#region Obter ID da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.projeto_geo_id from {0}crt_cad_ambiental_rural c where c.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				bancoDeDados.ExecutarReader(comando, (reader) =>
				{
					caracterizacao = reader.GetValue<int>("id");
					projeto = reader.GetValue<int>("projeto_geo_id");
				});

				#endregion

				#region Histórico

				//Atualizar o tid para a nova ação
				comando = bancoDeDados.CriarComando(@"update {0}crt_cad_ambiental_rural c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(caracterizacao, eHistoricoArtefatoCaracterizacao.cadastroambientalrural, eHistoricoAcao.excluir, bancoDeDados, null);

				if (caracterizacao > 0)
				{
					Historico.GerarGeo(projeto, (int)eHistoricoArtefatoCaracterizacao.cadastroambientalrural, eHistoricoAcao.excluir, bancoDeDados);
				}

				#endregion

				#region Apaga todos os dados

				comando = bancoDeDados.CriarComandoPlSql(
				@"declare 
					v_projeto_id number(38) := 0;
					v_caracterizacao_id number(38) := 0;
				begin 
					select p.id into v_projeto_id from {0}tmp_projeto_geo p where p.empreendimento = :empreendimento and p.caracterizacao = :caracterizacao_tipo;

					{0}geo_operacoesprocessamentogeo.ApagarGeometriasTMP(v_projeto_id, :fila_tipo);
					{0}geo_operacoesprocessamentogeo.ApagarGeometriasOficial(v_projeto_id, :fila_tipo);

					delete from {1}tab_fila f where f.projeto = v_projeto_id;

					select (select c.id from tmp_cad_ambiental_rural c where c.empreendimento = :empreendimento 
					union select c.id from crt_cad_ambiental_rural c where c.empreendimento = :empreendimento) into v_caracterizacao_id from dual;

					delete from {0}tmp_projeto_geo_arquivos r where r.projeto = v_projeto_id;
					delete from {0}tmp_projeto_geo r where r.id = v_projeto_id;
					delete from {0}tmp_cad_ambiental_rural c where c.id = v_caracterizacao_id;

					delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = v_caracterizacao_id and d.dependente_caracterizacao = :caracterizacao_tipo;
					delete from {0}crt_cad_amb_rural_prj_geo_arq c where c.cad_ambiental_rural = v_caracterizacao_id;
					delete from {0}crt_cad_amb_rural_areas c where c.cad_ambiental_rural = v_caracterizacao_id;
					delete from {0}crt_cad_ambiental_rural c where c.id = v_caracterizacao_id;
				end;", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao_tipo", Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("fila_tipo", (int)eFilaTipoGeo.CAR, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Finalizar(CadastroAmbientalRural caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Apagar dados

				Comando comando = bancoDeDados.CriarComandoPlSql(
				@"begin 
					delete from {0}crt_cad_amb_rural_prj_geo_arq t where t.cad_ambiental_rural = :id;
					delete from {0}crt_cad_amb_rural_areas t where t.cad_ambiental_rural = :id;
					delete from {0}crt_cad_ambiental_rural c where c.id = :id;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion Apagar dados

				#region Caracterização

				comando = bancoDeDados.CriarComando(@"insert into {0}crt_cad_ambiental_rural 
				(id, empreendimento, projeto_geo_id, ocorreu_alteracao_apos_2008, atp_documento_2008, situacao, municipio, modulo_fiscal, atp_qtd_modulo_fiscal, tid, dispensa_arl, 
				vistoria_aprovacao, data_vistoria, empreendimento_cedente, empreendimento_receptor) 
				(select c.id, c.empreendimento, c.projeto_geo_id, c.ocorreu_alteracao_apos_2008, c.atp_documento_2008, :situacao, c.municipio, c.modulo_fiscal, 
				c.atp_qtd_modulo_fiscal, :tid, c.dispensa_arl, c.vistoria_aprovacao, c.data_vistoria, c.empreendimento_cedente, c.empreendimento_receptor from {0}tmp_cad_ambiental_rural c where c.id = :id)", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", caracterizacao.Situacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion Caracterização

				#region Áreas

				if (caracterizacao.Areas != null && caracterizacao.Areas.Count > 0)
				{
					foreach (Area item in caracterizacao.Areas)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_cad_amb_rural_areas(id, cad_ambiental_rural, tipo, valor, tid)
							values (:id, :caracterizacao, :tipo, :valor, :tid)", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_cad_amb_rural_areas(id, cad_ambiental_rural, tipo, valor, tid)
							values ({0}seq_crt_cad_amb_rural_areas.nextval, :caracterizacao, :tipo, :valor, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo", item.Tipo, DbType.Int32);
						comando.AdicionarParametroEntrada("valor", item.Valor, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Arquivos/GEO/TMP

				comando = bancoDeDados.CriarComandoPlSql(
				@"declare 
					v_projeto number(38) := 0;
					v_caracterizacao_id number(38) := 0;
				begin 
					select p.id into v_projeto from {0}tmp_projeto_geo p where p.empreendimento = :empreendimento and p.caracterizacao = :caracterizacao_tipo;
					select c.id into v_caracterizacao_id from tmp_cad_ambiental_rural c where c.empreendimento = :empreendimento;

					insert into {0}crt_cad_amb_rural_prj_geo_arq (id, cad_ambiental_rural, projeto, tipo, arquivo, valido, arquivo_fila_tipo, tid)
					(select t.id, v_caracterizacao_id, t.projeto, t.tipo, t.arquivo, t.valido, t.arquivo_fila_tipo, :tid from {0}tmp_projeto_geo_arquivos t where t.projeto = v_projeto);

					delete from {1}geo_car_areas_calculadas a where a.projeto = v_projeto;

					insert into {1}geo_car_areas_calculadas (id, projeto, cod_apmp, tipo, area_m2, data, tid, geometry) 
					(select id, projeto, cod_apmp, tipo, area_m2, sysdate, :tid, geometry from {1}tmp_car_areas_calculadas a where a.projeto = v_projeto);

					delete from {0}tmp_cad_ambiental_rural c where c.id = v_caracterizacao_id;

					:projetoID := v_projeto;
				end;", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao_tipo", Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("projetoID", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.ProjetoGeoId = comando.ObterValorParametro<int>("projetoID");

				#endregion

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.cadastroambientalrural, eHistoricoAcao.finalizar, bancoDeDados);
				Historico.GerarGeo(caracterizacao.ProjetoGeoId, (int)eHistoricoArtefatoCaracterizacao.cadastroambientalrural, eHistoricoAcao.finalizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Ações de DML da base GEO

		internal void InserirFila(ArquivoProjeto arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {1}tab_fila f (id, projeto, tipo, mecanismo_elaboracao, etapa, situacao, data_fila)
				(select {1}seq_fila.nextval, t.id, :tipo, :mecanismo, :etapa, :situacao, sysdate from {0}tmp_projeto_geo t where t.id = :projeto)", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"select f.id from {0}tab_fila f where f.projeto = :projeto and f.tipo = :tipo", EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("mecanismo", arquivo.Mecanismo, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", arquivo.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", arquivo.Situacao, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					arquivo.IdRelacionamento = Convert.ToInt32(valor);
				}

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacaoFila(ArquivoProjeto arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando("begin " +
				"update {0}tab_fila t set t.etapa = :etapa, t.situacao = :situacao, t.data_fila = sysdate, t.data_inicio = null, t.data_fim = null " +
				"where t.projeto = :projeto and t.tipo = :fila_tipo returning t.id into :id; end;", EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("fila_tipo", arquivo.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", arquivo.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", arquivo.Situacao, DbType.Int32);
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				arquivo.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));
			}
		}

		internal void InvalidarFila(int projetoId, List<int> arquivos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_fila f set f.etapa = 1, f.situacao = 5 where f.projeto = :projeto", EsquemaBancoGeo);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "f.tipo", DbType.Int32, arquivos);
				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"update {0}tab_fila f set f.situacao = 5 where f.projeto = :projeto", EsquemaBancoGeo);
				comando.DbCommand.CommandText += comando.AdicionarIn("and", "f.tipo", DbType.Int32, arquivos);
				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter

		internal List<Area> ObterAreasProcessadas(int projetoId)
		{
			List<Area> areas = new List<Area>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Areas

				Comando comando = bancoDeDados.CriarComando(@"select 
				(select sum(atp.area_m2) from {1}geo_atp atp where atp.projeto = cpg.id) GEO_ATP,
				(select sum(c.area_m2) from {1}tmp_car_areas_calculadas c where c.projeto = tc.projeto_geo_id and c.tipo = 'CAR_APP_AA_USO' ) CAR_APP_AA_USO, 
				(select sum(a.area_m2) from {1}geo_aa a where a.projeto = cpg.id and a.tipo = 'USO' ) AA_USO,
				(select sum(a.area_m2) from {1}geo_areas_calculadas a where a.projeto = cpg.id and a.tipo = 'APP_AA_USO' ) APP_AA_USO,
				(select sum(a.area_m2) from {1}geo_areas_calculadas a where a.projeto = cpg.id and a.tipo = 'APP_AA_REC' ) APP_AA_REC, 
				(select sum(a.area_m2) from {1}geo_areas_calculadas a where a.projeto = cpg.id and a.tipo = 'APP_AVN' ) APP_AVN
				from {0}tmp_cad_ambiental_rural tc, {0}crt_projeto_geo cpg 
				where cpg.empreendimento = tc.empreendimento 
				and cpg.caracterizacao = :caracterizacao
				and  tc.projeto_geo_id = :projeto", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						areas.Add(new Area()
						{
							Tipo = (int)eCadastroAmbientalRuralArea.CALC_GEO_ATP,
							Valor = reader.GetValue<decimal>("GEO_ATP")
						});

						areas.Add(new Area()
						{
							Tipo = (int)eCadastroAmbientalRuralArea.APP_RECUPERAR_CALCULADO,
							Valor = reader.GetValue<decimal>("CAR_APP_AA_USO")
						});

						areas.Add(new Area()
						{
							Tipo = (int)eCadastroAmbientalRuralArea.AA_USO,
							Valor = reader.GetValue<decimal>("AA_USO")
						});

						areas.Add(new Area()
						{
							Tipo = (int)eCadastroAmbientalRuralArea.APP_USO,
							Valor = reader.GetValue<decimal>("APP_AA_USO")
						});

						areas.Add(new Area()
						{
							Tipo = (int)eCadastroAmbientalRuralArea.CALC_APP_AA_REC,
							Valor = reader.GetValue<decimal>("APP_AA_REC")
						});

						areas.Add(new Area()
						{
							Tipo = (int)eCadastroAmbientalRuralArea.CALC_APP_AVN,
							Valor = reader.GetValue<decimal>("APP_AVN")
						});
					}

					reader.Close();
				}

				#endregion
			}

			return areas;
		}

		internal CadastroAmbientalRural ObterPorEmpreendimento(int empreendimento, bool simplificado = false, bool finalizado = true, BancoDeDados banco = null)
		{
			CadastroAmbientalRural caracterizacao = new CadastroAmbientalRural();
			string tabela = finalizado ? "crt_cad_ambiental_rural" : "tmp_cad_ambiental_rural";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}" + tabela + " c where c.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado, finalizado);
				}
			}

			return caracterizacao;
		}

		internal CadastroAmbientalRural Obter(int id, BancoDeDados banco = null, bool simplificado = false, bool finalizado = true)
		{
			CadastroAmbientalRural caracterizacao = new CadastroAmbientalRural();
			string tabela = finalizado ? "crt_cad_ambiental_rural" : "tmp_cad_ambiental_rural";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Caracterização

				Comando comando = bancoDeDados.CriarComando(@"
				select c.dispensa_arl, c.tid, c.empreendimento, c.projeto_geo_id, c.ocorreu_alteracao_apos_2008, c.atp_documento_2008, c.atp_qtd_modulo_fiscal, c.situacao, ls.texto situacao_texto, 
				c.municipio, c.modulo_fiscal, cm.modulo_ha, vistoria_aprovacao, data_vistoria, empreendimento_cedente, empreendimento_receptor, ec.codigo emp_cedente_codigo, 
				er.codigo emp_receptor_codigo  from {0}" + tabela + @" c, {0}lov_crt_cad_amb_rural_situacao ls, {0}cnf_municipio_mod_fiscal  cm, 
				{0}tab_empreendimento ec, {0}tab_empreendimento er 
				where c.empreendimento_cedente = ec.id(+) and c.empreendimento_receptor = er.id(+) and c.municipio = cm.municipio and c.situacao = ls.id and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.Tid = reader.GetValue<string>("tid");
						caracterizacao.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						caracterizacao.ProjetoGeoId = reader.GetValue<int>("projeto_geo_id");
						caracterizacao.OcorreuAlteracaoApos2008 = reader.GetValue<int>("ocorreu_alteracao_apos_2008");
						caracterizacao.ATPDocumento2008 = reader.GetValue<decimal>("atp_documento_2008");
						caracterizacao.ATPQuantidadeModuloFiscal = reader.GetValue<decimal>("atp_qtd_modulo_fiscal");
						caracterizacao.Situacao.Id = reader.GetValue<int>("situacao");
						caracterizacao.Situacao.Nome = reader.GetValue<string>("situacao_texto");
						caracterizacao.MunicipioId = reader.GetValue<int>("municipio");
						caracterizacao.ModuloFiscalId = reader.GetValue<int>("modulo_fiscal");
						caracterizacao.ModuloFiscalHA = reader.GetValue<decimal>("modulo_ha");

						caracterizacao.DispensaARL = reader.GetValue<bool>("dispensa_arl");
						caracterizacao.VistoriaAprovacaoCAR = reader.GetValue<int>("vistoria_aprovacao");
						caracterizacao.DataVistoriaAprovacao.DataTexto = reader.GetValue<string>("data_vistoria");
						caracterizacao.EmpreendimentoReceptorId = reader.GetValue<int>("empreendimento_receptor");
						caracterizacao.EmpreendimentoCedenteId = reader.GetValue<int>("empreendimento_cedente");
						caracterizacao.CodigoEmpreendimentoCedente = reader.GetValue<int>("emp_cedente_codigo");
						caracterizacao.CodigoEmpreendimentoReceptor = reader.GetValue<int>("emp_receptor_codigo");
						caracterizacao.ReservaLegalEmOutroCAR = caracterizacao.CodigoEmpreendimentoReceptor.GetValueOrDefault() > 0;
						caracterizacao.ReservaLegalDeOutroCAR = caracterizacao.CodigoEmpreendimentoCedente.GetValueOrDefault() > 0;
					}

					reader.Close();
				}

				#endregion Caracterização

				if (simplificado || !finalizado)
				{
					return caracterizacao;
				}

				#region Áreas

				comando = bancoDeDados.CriarComando(@"select a.id Id, a.tid Tid, a.tipo Tipo, la.texto TipoTexto, a.valor Valor 
				from {0}crt_cad_amb_rural_areas a, {0}lov_crt_cad_amb_rural_area la where a.tipo = la.id and a.cad_ambiental_rural = :caracterizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);

				caracterizacao.Areas.AddRange(bancoDeDados.ObterEntityList<Area>(comando));

				#endregion
			}

			return caracterizacao;
		}

		internal CadastroAmbientalRural ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			CadastroAmbientalRural caracterizacao = new CadastroAmbientalRural();
			int historico = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Caracterização

				Comando comando = bancoDeDados.CriarComando(@"
				select h.id, h.tid, h.empreendimento_id, h.projeto_geo_id, h.ocorreu_alteracao_apos_2008, h.atp_documento_2008, h.atp_qtd_modulo_fiscal, h.situacao_id, ec.codigo emp_cedente_codigo, er.codigo emp_receptor_codigo,  
				h.situacao_texto, h.municipio_id, h.municipio_texto, h.modulo_fiscal_id, h.modulo_fiscal_ha, h.vistoria_aprovacao, h.data_vistoria , h.empreendimento_cedente_id, 
				h.empreendimento_cedente_tid, h.empreendimento_receptor_id, h.dispensa_arl from {0}hst_crt_cad_ambiental_rural h, hst_empreendimento ec, hst_empreendimento er,  
				where ec.empreendimento_id(+) = h.empreendimento_cedente_id and ec.tid = h.empreendimento_cedente_tid and
				er.empreendimento_id(+) = h.empreendimento_receptor_id and er.tid = h.empreendimento_receptor_tid and h.cad_ambiental_rural = :id and h.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						historico = reader.GetValue<int>("id");
						caracterizacao.Id = id;
						caracterizacao.Tid = reader.GetValue<string>("tid");
						caracterizacao.EmpreendimentoId = reader.GetValue<int>("empreendimento_id");
						caracterizacao.ProjetoGeoId = reader.GetValue<int>("projeto_geo_id");
						caracterizacao.OcorreuAlteracaoApos2008 = reader.GetValue<int>("ocorreu_alteracao_apos_2008");
						caracterizacao.ATPDocumento2008 = reader.GetValue<decimal>("atp_documento_2008");
						caracterizacao.ATPQuantidadeModuloFiscal = reader.GetValue<decimal>("atp_qtd_modulo_fiscal");
						caracterizacao.Situacao.Id = reader.GetValue<int>("situacao_id");
						caracterizacao.Situacao.Nome = reader.GetValue<string>("situacao_texto");
						caracterizacao.MunicipioId = reader.GetValue<int>("municipio_id");
						caracterizacao.MunicipioTexto = reader.GetValue<string>("municipio_texto");
						caracterizacao.ModuloFiscalId = reader.GetValue<int>("modulo_fiscal_id");
						caracterizacao.ModuloFiscalHA = reader.GetValue<decimal>("modulo_fiscal_ha");

						caracterizacao.DispensaARL = reader.GetValue<bool>("dispensa_arl");
						caracterizacao.VistoriaAprovacaoCAR = reader.GetValue<int>("vistoria_aprovacao");
						caracterizacao.DataVistoriaAprovacao.DataTexto = reader.GetValue<string>("data_vistoria");
						caracterizacao.EmpreendimentoCedenteId = reader.GetValue<int>("empreendimento_cedente_id");
						caracterizacao.EmpreendimentoReceptorId = reader.GetValue<int>("empreendimento_receptor_id");
						caracterizacao.CodigoEmpreendimentoCedente = reader.GetValue<int>("emp_cedente_codigo");
						caracterizacao.CodigoEmpreendimentoReceptor = reader.GetValue<int>("emp_receptor_codigo");
					}

					reader.Close();
				}

				#endregion Caracterização

				if (simplificado)
				{
					return caracterizacao;
				}

				#region Áreas

				comando = bancoDeDados.CriarComando(@"select a.id Id, a.tid Tid, a.tipo_id Tipo, a.tipo_texto TipoTexto, a.valor Valor 
				from {0}hst_crt_cad_amb_rural_areas a where a.id_hst = :historico", EsquemaBanco);

				comando.AdicionarParametroEntrada("historico", historico, DbType.Int32);

				caracterizacao.Areas.AddRange(bancoDeDados.ObterEntityList<Area>(comando));

				#endregion
			}

			return caracterizacao;
		}

		internal List<Area> ObterAreasGeo(int empreendimentoId, BancoDeDados banco = null)
		{
			List<Area> areas = new List<Area>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select t.*, (t.app_apmp - t.app_avn - t.app_aa_rec - t.app_aa_uso) app_nao_caracterizada from (
				select (select a.area_m2 from {1}geo_atp a where a.projeto = p.id) atp_croqui,
					(select sum(ac.area_m2) from {1}geo_areas_calculadas ac where ac.tipo = 'APP_APMP' and ac.projeto = p.id 
					and ac.cod_apmp in (select a.id from {1}geo_apmp a where a.projeto = p.id and a.tipo != 'D')) app_croqui,
					d.documento_area,
					d.ccri_area,
					(select sum(a.area_m2) from {1}geo_arl a where a.projeto = p.id) arl_croqui,
					d.arl_documento,
					(select sum(a.area_m2) from {1}geo_arl a where a.projeto = p.id and a.situacao = 'PRESERV') arl_preservada,
					(select sum(a.area_m2) from {1}geo_arl a where a.projeto = p.id and a.situacao = 'REC') arl_recuperacao,
					(select sum(a.area_m2) from {1}geo_arl a where a.projeto = p.id and a.situacao = 'USO') arl_recuperar,
					(select sum(a.area_m2) from {1}geo_arl a where a.projeto = p.id and a.situacao = 'D') arl_nao_caracterizada,
					(select sum(a.area_m2) from {1}geo_areas_calculadas a where a.projeto = p.id and a.tipo = 'APP_APMP') app_apmp,
					(select nvl(sum(a.area_m2), 0) from {1}geo_areas_calculadas a where a.projeto = p.id and a.tipo = 'APP_AA_USO') app_aa_uso,
					(select nvl(sum(a.area_m2), 0) from {1}geo_areas_calculadas a where a.projeto = p.id and a.tipo = 'APP_AVN') app_avn,
					(select nvl(sum(a.area_m2), 0) from {1}geo_areas_calculadas a where a.projeto = p.id and a.tipo = 'APP_AA_REC') app_aa_rec,
					(select sum(a.area_m2) from {1}geo_areas_calculadas a where a.projeto = p.id and a.tipo = 'APP_ARL') app_arl,
					(select sum(a.area_m2) from {1}geo_rest_declividade a where a.projeto = p.id and a.tipo = 'RESTRICAO') area_uso_rest_decli
				from {0}crt_projeto_geo p, {0}crt_dominialidade d
					where p.empreendimento = d.empreendimento
						and p.empreendimento = :empreendimento
						and p.caracterizacao = :caracterizacao_tipo) t", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao_tipo", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ATP_CROQUI, Valor = reader.GetValue<decimal>("atp_croqui") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.APP_TOTAL_CROQUI, Valor = reader.GetValue<decimal>("app_croqui") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.AREA_DOCUMENTO, Valor = reader.GetValue<decimal>("documento_area") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.AREA_CCRI, Valor = reader.GetValue<decimal>("ccri_area") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_CROQUI, Valor = reader.GetValue<decimal>("arl_croqui") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_DOCUMENTO, Valor = reader.GetValue<decimal>("arl_documento") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_PRESERVADA, Valor = reader.GetValue<decimal>("arl_preservada") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_RECUPERACAO, Valor = reader.GetValue<decimal>("arl_recuperacao") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_RECUPERAR, Valor = reader.GetValue<decimal>("arl_recuperar") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_NAO_CARACTERIZADA, Valor = reader.GetValue<decimal>("arl_nao_caracterizada") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.AREA_USO_RESTRITO_DECLIVIDADE, Valor = reader.GetValue<decimal>("area_uso_rest_decli") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.APP_PRESERVADA, Valor = reader.GetValue<decimal>("app_avn") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.APP_RECUPERACAO, Valor = reader.GetValue<decimal>("app_aa_rec") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.APP_USO, Valor = reader.GetValue<decimal>("app_aa_uso") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_APP, Valor = reader.GetValue<decimal>("app_arl") });
						areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.APP_NAO_CARACTERIZADA, Valor = reader.GetValue<decimal>("app_nao_caracterizada") });
					}

					reader.Close();
				}
			}

			return areas;
		}

		internal CadastroAmbientalRural ObterDadosGeo(int empreendimentoId, BancoDeDados banco = null)
		{
			CadastroAmbientalRural caracterizacao = new CadastroAmbientalRural();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Areas

				Comando comando = bancoDeDados.CriarComando(@"
				select a.tipo, sum(a.area_m2)area_m2
					from {1}geo_areas_calculadas a, {0}crt_projeto_geo p
				where p.id = a.projeto
					and p.empreendimento = :empreendimento
					and p.caracterizacao = :caracterizacao
					and a.tipo = 'CAR_APP_AA_USO'
				group by a.tipo", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", Tipo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						caracterizacao.Areas.Add(new Area()
						{
							Tipo = (int)eCadastroAmbientalRuralArea.APP_RECUPERAR_CALCULADO,
							Valor = reader.GetValue<decimal>("area_m2")
						});
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		internal MBR ObterAbrangencia(int empreendimentoId, BancoDeDados banco = null)
		{
			MBR mbr = new MBR();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Areas

				Comando comando = bancoDeDados.CriarComando(@"select c.menor_x, c.menor_y, c.maior_x, c.maior_y
					 from {0}crt_projeto_geo c 
					where c.empreendimento = :empreendimento 
					and c.caracterizacao = :caracterizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						mbr.MaiorX = reader.GetValue<decimal>("MAIOR_X");
						mbr.MaiorY = reader.GetValue<decimal>("MAIOR_Y");
						mbr.MenorX = reader.GetValue<decimal>("MENOR_X");
						mbr.MenorY = reader.GetValue<decimal>("MENOR_Y");
					}

					reader.Close();
				}

				#endregion
			}

			return mbr;
		}

		internal CaracterizacaoPDF ObterDadosPdfTitulo(int id, BancoDeDados banco = null)
		{
			CaracterizacaoPDF caracterizacao = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao = new CaracterizacaoPDF();
					}

					reader.Close();
				}
			}

			return caracterizacao;
		}

		internal Situacao ObterSituacaoProcessamento(int empreendimentoID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select l.id Id, l.texto Nome
					from {0}tmp_projeto_geo              p,
						{1}tab_fila                      f,
						{0}lov_crt_projeto_geo_sit_proce l
				where p.id = f.projeto
					and f.etapa = l.etapa
					and f.situacao = l.situacao
					and p.empreendimento = :empreendimento
					and p.caracterizacao = :caracterizacao", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", Tipo, DbType.Int32);

				return bancoDeDados.ObterEntity<Situacao>(comando);
			}
		}

		public List<ArquivoProjeto> ObterArquivosProjeto(int projetoId, BancoDeDados banco = null, bool finalizado = false)
		{
			List<ArquivoProjeto> arquivos = new List<ArquivoProjeto>();

			string tabela = finalizado ? "crt_cad_amb_rural_prj_geo_arq" : "tmp_projeto_geo_arquivos";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico/Arquivos

				Comando comando = bancoDeDados.CriarComando(@"select  tf.id, t.tipo, lc.texto  tipo_texto, tf.tipo fila_tipo, t.arquivo, t.valido, 
				lcp.id situacao_id, lcp.texto situacao_texto from {0}" + tabela + @" t, {0}lov_crt_projeto_geo_arquivos lc, {1}tab_fila tf, 
				{0}lov_crt_projeto_geo_sit_proce lcp where t.tipo = lc.id and t.projeto = tf.projeto(+) and t.tipo = tf.tipo(+) 
				and tf.etapa = lcp.etapa(+) and tf.situacao = lcp.situacao(+) and t.projeto = :projeto and t.valido = 1 and t.tipo <> 5 order by lc.id", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						ArquivoProjeto arq = new ArquivoProjeto();

						if (reader["id"] != null && !Convert.IsDBNull(reader["id"]))
						{
							arq.IdRelacionamento = Convert.ToInt32(reader["id"]);
						}

						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							arq.Nome = reader["tipo_texto"].ToString();
							arq.Tipo = Convert.ToInt32(reader["tipo"]);
						}

						if (reader["fila_tipo"] != null && !Convert.IsDBNull(reader["fila_tipo"]))
						{
							arq.FilaTipo = Convert.ToInt32(reader["fila_tipo"]);
						}

						if (reader["arquivo"] != null && !Convert.IsDBNull(reader["arquivo"]))
						{
							arq.Id = Convert.ToInt32(reader["arquivo"]);
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							arq.Situacao = Convert.ToInt32(reader["situacao_id"]);
							arq.SituacaoTexto = reader["situacao_texto"].ToString();
						}

						if (reader["valido"] != null && !Convert.IsDBNull(reader["valido"]))
						{
							arq.isValido = Convert.ToBoolean(reader["valido"]);
						}

						arquivos.Add(arq);
					}

					reader.Close();
				}

				#endregion
			}

			return arquivos;
		}

		internal void CarregarAreasReservaLegalCompensacao(CadastroAmbientalRural car, BancoDeDados banco = null)
		{

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Cedente

				Comando comando = bancoDeDados.CriarComando(@"select r.situacao_vegetal, r.arl_croqui from crt_dominialidade_reserva r, crt_dominialidade_dominio d, 
				crt_dominialidade cd  where d.id = r.dominio and r.compensada = 1 and r.cedente_receptor = 1 and cd.id = d.dominialidade and cd.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", car.EmpreendimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						SetarAreasCompensacao(car, (eReservaLegalSituacaoVegetal)reader.GetValue<int>("situacao_vegetal"), reader.GetValue<decimal>("arl_croqui"), 1); 
					}
				}

				#endregion

				#region Receptor

				#region Registrada sem empreendimento cedente
				//Registrada
				comando = bancoDeDados.CriarComando(@"select r.arl_cedida, r.situacao_vegetal from crt_dominialidade_reserva r, crt_dominialidade_dominio d, 
				crt_dominialidade cd  where d.id = r.dominio and r.cedente_receptor = 2 and r.situacao = 3 and cd.id = d.dominialidade and r.cedente_possui_emp = 0 and
				cd.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", car.EmpreendimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						SetarAreasCompensacao(car, (eReservaLegalSituacaoVegetal)reader.GetValue<int>("situacao_vegetal"), reader.GetValue<decimal>("arl_cedida"), 2);
					}
				}

				#endregion

				#region Com empreendimento cedente

				comando = bancoDeDados.CriarComando(@"select rc.arl_croqui, rc.situacao_vegetal from crt_dominialidade_reserva r, crt_dominialidade_reserva rc, 
				crt_dominialidade_dominio d, crt_dominialidade cd  where d.id = r.dominio and r.cedente_receptor = 2 and r.cedente_possui_emp = 1 and r.arl_cedente = rc.id 
				and cd.id = d.dominialidade and  cd.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", car.EmpreendimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						SetarAreasCompensacao(car, (eReservaLegalSituacaoVegetal)reader.GetValue<int>("situacao_vegetal"), reader.GetValue<decimal>("arl_croqui"), 2);
					}
				}

				#endregion

				#endregion
			}
		}

		#endregion

		#region Validações

		internal int ExisteArquivoFila(ArquivoProjeto arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}tab_fila t where t.projeto = :projeto and t.tipo = :tipo", EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.FilaTipo, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				return (valor != null && !Convert.IsDBNull(valor)) ? Convert.ToInt32(valor) : 0;
			}
		}

		internal bool IsTemporario(int empreendimentoId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(c.id) from {0}tmp_cad_ambiental_rural c where c.empreendimento = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool PossuiTCPFRLCConcluido(int empreendimentoId) 
		{
			using (BancoDeDados banco = BancoDeDados.ObterInstancia())
			{
				Comando comando = banco.CriarComando(@"select count(*) from tab_titulo tt where tt.modelo = (select id from tab_titulo_modelo where codigo = 44) 
				and tt.situacao = 3 and tt.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);
				return banco.ExecutarScalar<int>(comando) > 0;
			}
		}

		#endregion

		private static void SetarAreasCompensacao(CadastroAmbientalRural car, eReservaLegalSituacaoVegetal situacaoVegetal, decimal areaARL, int tipoCompensacao)
		{
			if (tipoCompensacao == 1)
			{
				#region Cedente

				switch (situacaoVegetal)
				{
					case eReservaLegalSituacaoVegetal.Preservada:

						if (car.Areas.Exists(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_PRESERVADA))
						{
							car.Areas.Single(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_PRESERVADA).Valor += areaARL;
						}
						else
						{
							car.Areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_PRESERVADA, Valor = areaARL });
						}

						break;

					case eReservaLegalSituacaoVegetal.EmRecuperacao:

						if (car.Areas.Exists(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_EM_RECUPERACAO))
						{
							car.Areas.Single(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_EM_RECUPERACAO).Valor += areaARL;
						}
						else
						{
							car.Areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_EM_RECUPERACAO, Valor = areaARL });
						}

						break;

					case eReservaLegalSituacaoVegetal.EmUso:
						if (car.Areas.Exists(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_RECUPERAR))
						{
							car.Areas.Single(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_RECUPERAR).Valor += areaARL;
						}
						else
						{
							car.Areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_RECUPERAR, Valor = areaARL });
						}
						break;

					case eReservaLegalSituacaoVegetal.NaoCaracterizada:
						if (car.Areas.Exists(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_NAO_CARACTERIZADA))
						{
							car.Areas.Single(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_NAO_CARACTERIZADA).Valor += areaARL;
						}
						else
						{
							car.Areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_CEDENTE_NAO_CARACTERIZADA, Valor = areaARL });
						}
						break;
				}

				#endregion

				return;
			}

			#region Receptora

			switch (situacaoVegetal)
			{
				case eReservaLegalSituacaoVegetal.Preservada:

					if (car.Areas.Exists(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_PRESERVADA))
					{
						car.Areas.Single(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_PRESERVADA).Valor += areaARL;
					}
					else
					{
						car.Areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_PRESERVADA, Valor = areaARL });
					}

					break;

				case eReservaLegalSituacaoVegetal.EmRecuperacao:

					if (car.Areas.Exists(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_EM_RECUPERACAO))
					{
						car.Areas.Single(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_EM_RECUPERACAO).Valor += areaARL;
					}
					else
					{
						car.Areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_EM_RECUPERACAO, Valor = areaARL });
					}

					break;

				case eReservaLegalSituacaoVegetal.EmUso:
					if (car.Areas.Exists(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_RECUPERAR))
					{
						car.Areas.Single(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_RECUPERAR).Valor += areaARL;
					}
					else
					{
						car.Areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_RECUPERAR, Valor = areaARL });
					}
					break;

				case eReservaLegalSituacaoVegetal.NaoCaracterizada:
					if (car.Areas.Exists(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_NAO_CARACTERIZADA))
					{
						car.Areas.Single(area => area.Tipo == (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_NAO_CARACTERIZADA).Valor += areaARL;
					}
					else
					{
						car.Areas.Add(new Area() { Tipo = (int)eCadastroAmbientalRuralArea.ARL_RECEPTORA_NAO_CARACTERIZADA, Valor = areaARL });
					}
					break;
			}

			#endregion
		}
	}
}