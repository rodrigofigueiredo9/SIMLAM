﻿using System;
using System.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Data
{
	public class BarragemDispensaLicencaDa
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

		private String EsquemaCredenciadoBanco { get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); } }

		#endregion

		#region Açoes de DML

		internal int Salvar(BarragemDispensaLicenca caracterizacao, BancoDeDados banco)
		{

			if (caracterizacao == null)
			{
				throw new Exception("A Caracterização é nula.");
			}

			if (caracterizacao.Id <= 0)
			{
				return Criar(caracterizacao, banco);
			}
			else
			{
				Editar(caracterizacao, banco);
				return caracterizacao.Id;
			}
		}

		internal int Criar(BarragemDispensaLicenca caracterizacao, BancoDeDados banco = null)
		{
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region Barragem
				Comando comando = bancoDeDados.CriarComando(@"
				insert into crt_barragem_dispensa_lic (id, tid, empreendimento, atividade, tipo_barragem, curso_hidrico, 
					vazao_enchente, area_bacia_contribuicao, precipitacao, periodo_retorno, coeficiente_escoamento, tempo_concentracao, 
					equacao_calculo, area_alagada, volume_armazenado, fase, interno_id, interno_tid,
					possui_barragem_contigua, altura_barramento, comprimento_barramento, largura_base_barramento, largura_crista_barramento,
					fonte_precipitacao, fonte_coeficiente_escoamento, fonte_vazao_enchente)
				values (seq_crt_barragem_dispensa_lic.nextval, :tid, :empreendimento, :atividade, :tipo_barragem, :curso_hidrico, 
						:vazao_enchente, :area_bacia_contribuicao, :precipitacao, :periodo_retorno, :coeficiente_escoamento, :tempo_concentracao, 
						:equacao_calculo, :area_alagada, :volume_armazenado, :fase, :interno_id, :interno_tid,
						:possui_barragem_contigua, :altura_barramento, :comprimento_barramento, :largura_base_barramento, :largura_crista_barramento,
						:fonte_precipitacao, :fonte_coeficiente_escoamento, :fonte_vazao_enchente) 
					returning id into :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.AtividadeID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_barragem", (int)caracterizacao.BarragemTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("curso_hidrico", caracterizacao.cursoHidrico, DbType.String);
				comando.AdicionarParametroEntrada("vazao_enchente", caracterizacao.vazaoEnchente, DbType.Decimal);
				comando.AdicionarParametroEntrada("area_bacia_contribuicao", caracterizacao.areaBaciaContribuicao, DbType.Decimal);
				comando.AdicionarParametroEntrada("precipitacao", caracterizacao.Precipitacao, DbType.Decimal);
				comando.AdicionarParametroEntrada("periodo_retorno", caracterizacao.periodoRetorno, DbType.Int32);
				comando.AdicionarParametroEntrada("coeficiente_escoamento", caracterizacao.coeficienteEscoamento, DbType.Decimal);
				comando.AdicionarParametroEntrada("tempo_concentracao", caracterizacao.tempoConcentracao, DbType.Decimal);
				comando.AdicionarParametroEntrada("equacao_calculo", caracterizacao.EquacaoCalculo, DbType.String);
				comando.AdicionarParametroEntrada("area_alagada", caracterizacao.areaAlagada, DbType.Decimal);
				comando.AdicionarParametroEntrada("volume_armazenado", caracterizacao.volumeArmazanado, DbType.Decimal);
				comando.AdicionarParametroEntrada("fase", caracterizacao.Fase, DbType.Int32);

				comando.AdicionarParametroEntrada("interno_id", caracterizacao.InternoID, DbType.Int32);
				comando.AdicionarParametroEntrada("interno_tid", DbType.String, 36, caracterizacao.InternoTID);

				comando.AdicionarParametroEntrada("possui_barragem_contigua", caracterizacao.barragemContiguaMesmoNivel, DbType.Int32);
				comando.AdicionarParametroEntrada("altura_barramento", caracterizacao.alturaBarramento, DbType.Decimal);
				comando.AdicionarParametroEntrada("comprimento_barramento", caracterizacao.comprimentoBarramento, DbType.Decimal);
				comando.AdicionarParametroEntrada("largura_base_barramento", caracterizacao.larguraBaseBarramento, DbType.Decimal);
				comando.AdicionarParametroEntrada("largura_crista_barramento", caracterizacao.larguraCristaBarramento, DbType.Decimal);
				comando.AdicionarParametroEntrada("fonte_precipitacao", caracterizacao.fonteDadosPrecipitacao, DbType.String);
				comando.AdicionarParametroEntrada("fonte_coeficiente_escoamento", caracterizacao.fonteDadosCoeficienteEscoamento, DbType.String);
				comando.AdicionarParametroEntrada("fonte_vazao_enchente", caracterizacao.fonteDadosVazaoEnchente, DbType.String);

				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				#endregion

				#region Construida/A Construir
				comando = bancoDeDados.CriarComando(@"
				insert into crt_barragem_construida_con (id, barragem, supressao_app, largura_demarcada, largura_demarcada_legislacao,
						faixa_cercada, descricao_desen_app, demarcacao_app, barramento_normas, barramento_adequacoes, vazao_min_tipo, vazao_min_diametro,
						vazao_min_instalado, vazao_min_normas, vazao_min_adequacoes, vazao_max_tipo, vazao_max_diametro,
						vazao_max_instalado, vazao_max_normas, vazao_max_adequacoes, mes_inicio_obra, ano_inicio_obra)
				values (seq_crt_barragem_const.nextval, :barragem, :supressao_app, :largura_demarcada, :largura_demarcada_legislacao,
						:faixa_cercada, :descricao_desen_app, :demarcacao_app, :barramento_normas, :barramento_adequacoes, :vazao_min_tipo, :vazao_min_diametro,
						:vazao_min_instalado, :vazao_min_normas, :vazao_min_adequacoes, :vazao_max_tipo, :vazao_max_diametro,
						:vazao_max_instalado, :vazao_max_normas, :vazao_max_adequacoes, :mes_inicio_obra, :ano_inicio_obra)  ", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("barragem", caracterizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("supressao_app", caracterizacao.construidaConstruir.isSupressaoAPP, DbType.Int32);
				comando.AdicionarParametroEntrada("largura_demarcada", caracterizacao.construidaConstruir.larguraDemarcada, DbType.Int32);
				comando.AdicionarParametroEntrada("largura_demarcada_legislacao", caracterizacao.construidaConstruir.larguraDemarcadaLegislacao, DbType.Decimal);
				comando.AdicionarParametroEntrada("faixa_cercada", caracterizacao.construidaConstruir.faixaCercada, DbType.Decimal);
				comando.AdicionarParametroEntrada("descricao_desen_app", caracterizacao.construidaConstruir.descricacaoDesenvolvimentoAPP, DbType.String);
				comando.AdicionarParametroEntrada("demarcacao_app", caracterizacao.construidaConstruir.isDemarcacaoAPP, DbType.Int32);
				comando.AdicionarParametroEntrada("barramento_normas", caracterizacao.construidaConstruir.barramentoNormas, DbType.Int32);
				comando.AdicionarParametroEntrada("barramento_adequacoes", caracterizacao.construidaConstruir.barramentoAdequacoes, DbType.String);
				comando.AdicionarParametroEntrada("vazao_min_tipo", (int)caracterizacao.construidaConstruir.vazaoMinTipo, DbType.Decimal);
				comando.AdicionarParametroEntrada("vazao_min_diametro", caracterizacao.construidaConstruir.vazaoMinDiametro, DbType.Decimal);
				comando.AdicionarParametroEntrada("vazao_min_instalado", caracterizacao.construidaConstruir.vazaoMinInstalado, DbType.Decimal);
				comando.AdicionarParametroEntrada("vazao_min_normas", caracterizacao.construidaConstruir.vazaoMinNormas, DbType.Int32);
				comando.AdicionarParametroEntrada("vazao_min_adequacoes", caracterizacao.construidaConstruir.vazaoMinAdequacoes, DbType.String);
				comando.AdicionarParametroEntrada("vazao_max_tipo", (int)caracterizacao.construidaConstruir.vazaoMaxTipo, DbType.Decimal);
				comando.AdicionarParametroEntrada("vazao_max_diametro", caracterizacao.construidaConstruir.vazaoMaxDiametro, DbType.Decimal);
				comando.AdicionarParametroEntrada("vazao_max_instalado", caracterizacao.construidaConstruir.vazaoMaxInstalado, DbType.Int32);
				comando.AdicionarParametroEntrada("vazao_max_normas", caracterizacao.construidaConstruir.vazaoMaxNormas, DbType.Int32);
				comando.AdicionarParametroEntrada("vazao_max_adequacoes", caracterizacao.construidaConstruir.vazaoMaxAdequacoes, DbType.String);
				comando.AdicionarParametroEntrada("mes_inicio_obra", caracterizacao.construidaConstruir.mesInicioObra, DbType.Int32);
				comando.AdicionarParametroEntrada("ano_inicio_obra", caracterizacao.construidaConstruir.anoInicioObra, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Coordenadas
				caracterizacao.coordenadas.ForEach(x => {
					comando = bancoDeDados.CriarComando(@"
					insert into crt_barragem_coordenada (id, barragem, tipo, northing, easting)
						values (seq_crt_barragem_coordenada.nextval, :barragem, :tipo, :northing, :easting)", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("barragem", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", (int)x.tipo, DbType.Int32);
					comando.AdicionarParametroEntrada("northing", x.northing, DbType.Int32);
					comando.AdicionarParametroEntrada("easting", x.easting, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
				});
				#endregion

				#region Responsaveis Técnicos
				caracterizacao.responsaveisTecnicos.ForEach(x => {
					comando = bancoDeDados.CriarComando(@"
					insert into crt_barragem_responsavel (id, barragem, tipo, nome, profissao, registro_crea,
						numero_art, autorizacao_crea)
					values (seq_crt_barragem_responsavel.nextval, :barragem, :tipo, :nome, :profissao, :registro_crea,
						:numero_art, :autorizacao_crea) ", EsquemaCredenciadoBanco);
					var arquivo = (x.autorizacaoCREA != null) ? x.autorizacaoCREA.Id : null;
					comando.AdicionarParametroEntrada("barragem", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", (int)x.tipo, DbType.Int32);
					comando.AdicionarParametroEntrada("nome", x.nome, DbType.String);
					comando.AdicionarParametroEntrada("profissao", x.profissao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("registro_crea", x.registroCREA, DbType.String);
					comando.AdicionarParametroEntrada("numero_art", x.numeroART, DbType.String);
					comando.AdicionarParametroEntrada("autorizacao_crea", arquivo, DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);
				});
				#endregion

				#region Finalidade Atividade
				caracterizacao.finalidade.ForEach(x => {
					comando = bancoDeDados.CriarComando(@"
					insert into crt_barragem_finaldiade_ativ(id, barragem, atividade)
						values(seq_crt_barragem_final_ativ.nextval, :barragem, :atividade) ", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("barragem", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("atividade", x, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
				});
				#endregion

				//Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.barragemdispensalicenca, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(BarragemDispensaLicenca caracterizacao, BancoDeDados banco = null)
		{
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update crt_barragem_dispensa_lic c set c.tid = :tid, c.empreendimento = :empreendimento, c.atividade = :atividade, c.tipo_barragem = :tipo_barragem, 
				c.finalidade_atividade = :finalidade_atividade, c.curso_hidrico = :curso_hidrico, c.vazao_enchente = :vazao_enchente, c.area_bacia_contribuicao = :area_bacia_contribuicao, c.precipitacao = :precipitacao, 
				c.periodo_retorno = :periodo_retorno, c.coeficiente_escoamento = :coeficiente_escoamento, c.tempo_concentracao = :tempo_concentracao, c.equacao_calculo = :equacao_calculo, c.area_alagada = :area_alagada, 
				c.volume_armazenado = :volume_armazenado, c.fase = :fase, c.possui_monge = :possui_monge, c.tipo_monge = :tipo_monge, c.especificacao_monge = :especificacao_monge, c.possui_vertedouro = :possui_vertedouro, 
				c.tipo_vertedouro = :tipo_vertedouro, c.especificacao_vertedouro = :especificacao_vertedouro, c.possui_estrutura_hidrau = :possui_estrutura_hidrau, c.adequacoes_realizada = :adequacoes_realizada, 
				c.data_inicio_obra = :data_inicio_obra, c.data_previsao_obra = :data_previsao_obra, c.easting = :easting, c.northing = :northing, c.formacao_resp_tec = :formacao_resp_tec, c.especificacao_rt = :especificacao_rt, 
				c.autorizacao = :autorizacao, c.numero_art_elaboracao = :numero_art_elaboracao, c.numero_art_execucao = :numero_art_execucao where c.id = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.AtividadeID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_barragem", caracterizacao.BarragemTipo, DbType.Int32);
				//comando.AdicionarParametroEntrada("finalidade_atividade", caracterizacao.FinalidadeAtividade, DbType.Int32);
				comando.AdicionarParametroEntrada("curso_hidrico", caracterizacao.cursoHidrico, DbType.String);
				comando.AdicionarParametroEntrada("vazao_enchente", caracterizacao.vazaoEnchente, DbType.Decimal);
				comando.AdicionarParametroEntrada("area_bacia_contribuicao", caracterizacao.areaBaciaContribuicao, DbType.Decimal);
				comando.AdicionarParametroEntrada("precipitacao", caracterizacao.Precipitacao, DbType.Decimal);
				comando.AdicionarParametroEntrada("periodo_retorno", caracterizacao.periodoRetorno, DbType.Int32);
				comando.AdicionarParametroEntrada("coeficiente_escoamento", caracterizacao.coeficienteEscoamento, DbType.String);
				comando.AdicionarParametroEntrada("tempo_concentracao", caracterizacao.tempoConcentracao, DbType.String);
				comando.AdicionarParametroEntrada("equacao_calculo", caracterizacao.EquacaoCalculo, DbType.String);
				comando.AdicionarParametroEntrada("area_alagada", caracterizacao.areaAlagada, DbType.Decimal);
				comando.AdicionarParametroEntrada("volume_armazenado", caracterizacao.volumeArmazanado, DbType.Decimal);
				comando.AdicionarParametroEntrada("fase", caracterizacao.Fase, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_monge", caracterizacao.PossuiMonge, DbType.Int32);//TODO
				comando.AdicionarParametroEntrada("tipo_monge", caracterizacao.MongeTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("especificacao_monge", caracterizacao.EspecificacaoMonge, DbType.String);
				comando.AdicionarParametroEntrada("possui_vertedouro", caracterizacao.PossuiVertedouro, DbType.Int32);//TODO
				comando.AdicionarParametroEntrada("tipo_vertedouro", caracterizacao.VertedouroTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("especificacao_vertedouro", caracterizacao.EspecificacaoVertedouro, DbType.String);
				comando.AdicionarParametroEntrada("possui_estrutura_hidrau", caracterizacao.PossuiEstruturaHidraulica, DbType.Int32);//TODO
				comando.AdicionarParametroEntrada("adequacoes_realizada", caracterizacao.AdequacoesRealizada, DbType.String);
				comando.AdicionarParametroEntrada("data_inicio_obra", caracterizacao.DataInicioObra, DbType.String);
				comando.AdicionarParametroEntrada("data_previsao_obra", caracterizacao.DataPrevisaoTerminoObra, DbType.String);
				comando.AdicionarParametroEntrada("easting", caracterizacao.Coordenada.EastingUtmTexto, DbType.Int64);
				comando.AdicionarParametroEntrada("northing", caracterizacao.Coordenada.NorthingUtmTexto, DbType.Int64);
				comando.AdicionarParametroEntrada("formacao_resp_tec", caracterizacao.FormacaoRT, DbType.Int32);
				comando.AdicionarParametroEntrada("especificacao_rt", caracterizacao.EspecificacaoRT, DbType.String);
                comando.AdicionarParametroEntrada("autorizacao", caracterizacao.Autorizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_art_elaboracao", caracterizacao.NumeroARTElaboracao, DbType.String);
				comando.AdicionarParametroEntrada("numero_art_execucao", caracterizacao.NumeroARTExecucao, DbType.String);

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.barragemdispensalicenca, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		public void Excluir(int empreendimento, BancoDeDados banco = null)
		{
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao(); 

				#region Obter id da caracterização

                Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_barragem_dispensa_lic c where c.empreendimento = :empreendimento", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				int id = 0;
				object retorno = bancoDeDados.ExecutarScalar(comando);

				if (retorno != null && !Convert.IsDBNull(retorno))
				{
					id = Convert.ToInt32(retorno);
				}

				#endregion

				#region Histórico

				//Atualizar o tid para a nova ação
                comando = bancoDeDados.CriarComando(@"update {0}crt_barragem_dispensa_lic c set c.tid = :tid where c.id = :id", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.barragemdispensalicenca, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComandoPlSql(
				@"begin 
					delete from crt_barragem_dispensa_lic where id = :id;
				end;", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();

				#endregion
			}
		}

        internal void CopiarDadosInstitucional(BarragemDispensaLicenca caracterizacao, BancoDeDados banco)
        {

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
            {
                bancoDeDados.IniciarTransacao();
                Comando comando;

                if (caracterizacao.Id <= 0)
                {
                    comando = bancoDeDados.CriarComando(@"
				    insert into crt_barragem_dispensa_lic (id, tid, empreendimento, atividade, tipo_barragem, finalidade_atividade, curso_hidrico, vazao_enchente, area_bacia_contribuicao, 
				    precipitacao, periodo_retorno, coeficiente_escoamento, tempo_concentracao, equacao_calculo, area_alagada, volume_armazenado, fase, possui_monge, tipo_monge, especificacao_monge, 
				    possui_vertedouro, tipo_vertedouro, especificacao_vertedouro, possui_estrutura_hidrau, adequacoes_realizada, data_inicio_obra, data_previsao_obra, easting, northing, formacao_resp_tec, 
				    especificacao_rt, autorizacao, numero_art_elaboracao, numero_art_execucao, interno_id, interno_tid)
				    values (seq_crt_barragem_dispensa_lic.nextval, :tid, :empreendimento, :atividade, :tipo_barragem, :finalidade_atividade, :curso_hidrico, :vazao_enchente, :area_bacia_contribuicao, 
				    :precipitacao, :periodo_retorno, :coeficiente_escoamento, :tempo_concentracao, :equacao_calculo, :area_alagada, :volume_armazenado, :fase, :possui_monge, :tipo_monge, :especificacao_monge, 
				    :possui_vertedouro, :tipo_vertedouro, :especificacao_vertedouro, :possui_estrutura_hidrau, :adequacoes_realizada, :data_inicio_obra, :data_previsao_obra, :easting, :northing, :formacao_resp_tec, 
				    :especificacao_rt, :autorizacao, :numero_art_elaboracao, :numero_art_execucao, :interno_id, :interno_tid) returning id into :id", EsquemaCredenciadoBanco);

                    comando.AdicionarParametroSaida("id", DbType.Int32); 
                    comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoID, DbType.Int32);
                }
                else
                {
                    comando = bancoDeDados.CriarComando(@"update crt_barragem_dispensa_lic c set c.tid = :tid, c.atividade = :atividade, c.tipo_barragem = :tipo_barragem, 
				    c.finalidade_atividade = :finalidade_atividade, c.curso_hidrico = :curso_hidrico, c.vazao_enchente = :vazao_enchente, c.area_bacia_contribuicao = :area_bacia_contribuicao, c.precipitacao = :precipitacao, 
				    c.periodo_retorno = :periodo_retorno, c.coeficiente_escoamento = :coeficiente_escoamento, c.tempo_concentracao = :tempo_concentracao, c.equacao_calculo = :equacao_calculo, c.area_alagada = :area_alagada, 
				    c.volume_armazenado = :volume_armazenado, c.fase = :fase, c.possui_monge = :possui_monge, c.tipo_monge = :tipo_monge, c.especificacao_monge = :especificacao_monge, c.possui_vertedouro = :possui_vertedouro, 
				    c.tipo_vertedouro = :tipo_vertedouro, c.especificacao_vertedouro = :especificacao_vertedouro, c.possui_estrutura_hidrau = :possui_estrutura_hidrau, c.adequacoes_realizada = :adequacoes_realizada, 
				    c.data_inicio_obra = :data_inicio_obra, c.data_previsao_obra = :data_previsao_obra, c.easting = :easting, c.northing = :northing, c.formacao_resp_tec = :formacao_resp_tec, c.especificacao_rt = :especificacao_rt, 
				    c.autorizacao = :autorizacao, c.numero_art_elaboracao = :numero_art_elaboracao, c.numero_art_execucao = :numero_art_execucao, c.interno_id = :interno_id, c.interno_tid = :interno_tid
                    where c.id = :id", EsquemaCredenciadoBanco);

                    comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
                }

                comando.AdicionarParametroEntrada("atividade", caracterizacao.AtividadeID, DbType.Int32);
                comando.AdicionarParametroEntrada("tipo_barragem", caracterizacao.BarragemTipo, DbType.Int32);
                //comando.AdicionarParametroEntrada("finalidade_atividade", caracterizacao.FinalidadeAtividade, DbType.Int32);
                comando.AdicionarParametroEntrada("curso_hidrico", caracterizacao.cursoHidrico, DbType.String);
                comando.AdicionarParametroEntrada("vazao_enchente", caracterizacao.vazaoEnchente, DbType.Decimal);
                comando.AdicionarParametroEntrada("area_bacia_contribuicao", caracterizacao.areaBaciaContribuicao, DbType.Decimal);
                comando.AdicionarParametroEntrada("precipitacao", caracterizacao.Precipitacao, DbType.Decimal);
                comando.AdicionarParametroEntrada("periodo_retorno", caracterizacao.periodoRetorno, DbType.Int32);
                comando.AdicionarParametroEntrada("coeficiente_escoamento", caracterizacao.coeficienteEscoamento, DbType.String);
                comando.AdicionarParametroEntrada("tempo_concentracao", caracterizacao.tempoConcentracao, DbType.String);
                comando.AdicionarParametroEntrada("equacao_calculo", caracterizacao.EquacaoCalculo, DbType.String);
                comando.AdicionarParametroEntrada("area_alagada", caracterizacao.areaAlagada, DbType.Decimal);
                comando.AdicionarParametroEntrada("volume_armazenado", caracterizacao.volumeArmazanado, DbType.Decimal);
                comando.AdicionarParametroEntrada("fase", caracterizacao.Fase, DbType.Int32);
                comando.AdicionarParametroEntrada("possui_monge", caracterizacao.PossuiMonge, DbType.Int32);//TODO
                comando.AdicionarParametroEntrada("tipo_monge", caracterizacao.MongeTipo, DbType.Int32);
                comando.AdicionarParametroEntrada("especificacao_monge", caracterizacao.EspecificacaoMonge, DbType.String);
                comando.AdicionarParametroEntrada("possui_vertedouro", caracterizacao.PossuiVertedouro, DbType.Int32);//TODO
                comando.AdicionarParametroEntrada("tipo_vertedouro", caracterizacao.VertedouroTipo, DbType.Int32);
                comando.AdicionarParametroEntrada("especificacao_vertedouro", caracterizacao.EspecificacaoVertedouro, DbType.String);
                comando.AdicionarParametroEntrada("possui_estrutura_hidrau", caracterizacao.PossuiEstruturaHidraulica, DbType.Int32);//TODO
                comando.AdicionarParametroEntrada("adequacoes_realizada", caracterizacao.AdequacoesRealizada, DbType.String);
                comando.AdicionarParametroEntrada("data_inicio_obra", caracterizacao.DataInicioObra, DbType.String);
                comando.AdicionarParametroEntrada("data_previsao_obra", caracterizacao.DataPrevisaoTerminoObra, DbType.String);
                comando.AdicionarParametroEntrada("easting", caracterizacao.Coordenada.EastingUtmTexto, DbType.Int64);
                comando.AdicionarParametroEntrada("northing", caracterizacao.Coordenada.NorthingUtmTexto, DbType.Int64);
                comando.AdicionarParametroEntrada("formacao_resp_tec", caracterizacao.FormacaoRT, DbType.Int32);
                comando.AdicionarParametroEntrada("especificacao_rt", caracterizacao.EspecificacaoRT, DbType.String);
                comando.AdicionarParametroEntrada("autorizacao", caracterizacao.Autorizacao.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("numero_art_elaboracao", caracterizacao.NumeroARTElaboracao, DbType.String);
                comando.AdicionarParametroEntrada("numero_art_execucao", caracterizacao.NumeroARTExecucao, DbType.String);

                comando.AdicionarParametroEntrada("interno_id", caracterizacao.InternoID, DbType.Int32);
                comando.AdicionarParametroEntrada("interno_tid", DbType.String, 36, caracterizacao.InternoTID);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());                

                bancoDeDados.ExecutarNonQuery(comando);

                if (caracterizacao.Id <= 0)
                {
                    caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
                }

                Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.barragemdispensalicenca, eHistoricoAcao.copiar, bancoDeDados, null);

                bancoDeDados.Commit();
            }
        }

        internal void AtualizarInternoIdTid(int barragemDispensaLicencaId, int barragemDispensaLicencaInternoId, string barragemDispensaLicencaInternoTid, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();

                #region Atualização do Tid

                Comando comando = bancoDeDados.CriarComandoPlSql(@"update crt_barragem_dispensa_lic set tid = :tid, interno_id = :interno_id, interno_tid = :interno_tid where id = :barragem_id", EsquemaCredenciadoBanco);

                comando.AdicionarParametroEntrada("barragem_id", barragemDispensaLicencaId, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("interno_id", barragemDispensaLicencaInternoId, DbType.Int32);
                comando.AdicionarParametroEntrada("interno_tid", DbType.String, 36, barragemDispensaLicencaInternoTid);

                bancoDeDados.ExecutarNonQuery(comando);

                #endregion

                Historico.Gerar(barragemDispensaLicencaId, eHistoricoArtefatoCaracterizacao.barragemdispensalicenca, eHistoricoAcao.atualizaridtid, bancoDeDados);

                bancoDeDados.Commit();

            }
        }

		#endregion

		#region Obter

		internal BarragemDispensaLicenca ObterPorEmpreendimento(int empreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			BarragemDispensaLicenca caracterizacao = new BarragemDispensaLicenca();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
                Comando comando = bancoDeDados.CriarComando(@"select id from {0}crt_barragem_dispensa_lic where empreendimento = :empreendimento", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), simplificado, bancoDeDados);
				}
			}

			return caracterizacao;
		}

		public BarragemDispensaLicenca Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			BarragemDispensaLicenca caracterizacao = new BarragemDispensaLicenca();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
                Comando comando = bancoDeDados.CriarComando(@"select c.id, c.tid, c.interno_id, c.interno_tid, c.empreendimento, c.atividade, c.tipo_barragem, 
                lt.texto tipo_barragem_texto, c.finalidade_atividade, c.curso_hidrico, c.vazao_enchente, c.area_bacia_contribuicao, c.precipitacao, 
                c.periodo_retorno, c.coeficiente_escoamento, c.tempo_concentracao, c.equacao_calculo, c.area_alagada, c.volume_armazenado, 
				c.fase, c.possui_monge, c.tipo_monge, c.especificacao_monge, c.possui_vertedouro, c.tipo_vertedouro, c.especificacao_vertedouro, 
                c.possui_estrutura_hidrau, c.adequacoes_realizada, c.data_inicio_obra, c.data_previsao_obra, c.easting, c.northing, c.formacao_resp_tec, 
                c.especificacao_rt, c.autorizacao, c.numero_art_elaboracao, c.numero_art_execucao 
                from crt_barragem_dispensa_lic c, lov_crt_bdla_barragem_tipo lt where lt.id = c.tipo_barragem and c.id = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = reader.GetValue<int>("id");
                        caracterizacao.CredenciadoID = id;
						caracterizacao.Tid = reader.GetValue<string>("tid");
                        caracterizacao.InternoID = reader.GetValue<int>("interno_id");
                        caracterizacao.InternoTID = reader.GetValue<string>("interno_tid");

						caracterizacao.EmpreendimentoID = reader.GetValue<int>("empreendimento");
						caracterizacao.AtividadeID = reader.GetValue<int>("atividade");
						//caracterizacao.BarragemTipo = reader.GetValue<int>("tipo_barragem");
						//caracterizacao.BarragemTipoTexto = reader.GetValue<string>("tipo_barragem_texto");
						//caracterizacao.FinalidadeAtividade = reader.GetValue<int>("finalidade_atividade");
						caracterizacao.cursoHidrico = reader.GetValue<string>("curso_hidrico");
						caracterizacao.vazaoEnchente = reader.GetValue<decimal>("vazao_enchente");
						caracterizacao.areaBaciaContribuicao = reader.GetValue<decimal>("area_bacia_contribuicao");
						caracterizacao.Precipitacao = reader.GetValue<decimal>("precipitacao");
						caracterizacao.periodoRetorno = reader.GetValue<int>("periodo_retorno");
						caracterizacao.coeficienteEscoamento = reader.GetValue<decimal>("coeficiente_escoamento");
						caracterizacao.tempoConcentracao = reader.GetValue<decimal>("tempo_concentracao");
						caracterizacao.EquacaoCalculo = reader.GetValue<string>("equacao_calculo");
						caracterizacao.areaAlagada = reader.GetValue<decimal>("area_alagada");
						caracterizacao.volumeArmazanado = reader.GetValue<decimal>("volume_armazenado");
						caracterizacao.Fase = reader.GetValue<int?>("fase");
						caracterizacao.PossuiMonge = reader.GetValue<int?>("possui_monge");
						caracterizacao.MongeTipo = reader.GetValue<int>("tipo_monge");
						caracterizacao.EspecificacaoMonge = reader.GetValue<string>("especificacao_monge");
						caracterizacao.PossuiVertedouro = reader.GetValue<int?>("possui_vertedouro");
						caracterizacao.VertedouroTipo = reader.GetValue<int>("tipo_vertedouro");
						caracterizacao.EspecificacaoVertedouro = reader.GetValue<string>("especificacao_vertedouro");
						caracterizacao.PossuiEstruturaHidraulica = reader.GetValue<int?>("possui_estrutura_hidrau");
						caracterizacao.AdequacoesRealizada = reader.GetValue<string>("adequacoes_realizada");
						caracterizacao.DataInicioObra = reader.GetValue<string>("data_inicio_obra");
						caracterizacao.DataPrevisaoTerminoObra = reader.GetValue<string>("data_previsao_obra");
						caracterizacao.Coordenada.EastingUtmTexto = reader.GetValue<string>("easting");
						caracterizacao.Coordenada.NorthingUtmTexto = reader.GetValue<string>("northing");
						caracterizacao.FormacaoRT = reader.GetValue<int>("formacao_resp_tec");
						caracterizacao.EspecificacaoRT = reader.GetValue<string>("especificacao_rt");
                        caracterizacao.Autorizacao.Id = reader.GetValue<int>("autorizacao");
						caracterizacao.NumeroARTElaboracao = reader.GetValue<string>("numero_art_elaboracao");
						caracterizacao.NumeroARTExecucao = reader.GetValue<string>("numero_art_execucao");
					}

					reader.Close();
				}
			}

			return caracterizacao;
		}

        public BarragemDispensaLicenca ObterHistorico(int id, string tid, BancoDeDados banco = null)
        {
            BarragemDispensaLicenca caracterizacao = new BarragemDispensaLicenca();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select c.id, c.tid, c.interno_id, c.interno_tid, c.empreendimento_id, c.atividade_id, 
                c.tipo_barragem_id, c.tipo_barragem_texto, c.finalidade_atividade, c.curso_hidrico, c.vazao_enchente, c.area_bacia_contribuicao, 
                c.precipitacao, c.periodo_retorno, c.coeficiente_escoamento, c.tempo_concentracao, c.equacao_calculo, c.area_alagada, 
                c.volume_armazenado, c.fase_id, c.possui_monge, c.tipo_monge_id, c.especificacao_monge, c.possui_vertedouro, c.tipo_vertedouro_id, 
                c.especificacao_vertedouro, c.possui_estrutura_hidrau, c.adequacoes_realizada, c.data_inicio_obra, c.data_previsao_obra, 
                c.easting, c.northing, c.formacao_resp_tec, c.especificacao_rt, c.autorizacao_id, c.numero_art_elaboracao, c.numero_art_execucao 
                from hst_crt_barragem_dispe_lic c where c.tid = :tid and c.caracterizacao_id = :id", EsquemaCredenciadoBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", tid, DbType.String);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        caracterizacao.Id = reader.GetValue<int>("id");
                        caracterizacao.CredenciadoID = id;
                        caracterizacao.Tid = reader.GetValue<string>("tid");
                        caracterizacao.InternoID = reader.GetValue<int>("interno_id");
                        caracterizacao.InternoTID = reader.GetValue<string>("interno_tid");

                        caracterizacao.EmpreendimentoID = reader.GetValue<int>("empreendimento_id");
                        caracterizacao.AtividadeID = reader.GetValue<int>("atividade_id");
                        //caracterizacao.BarragemTipo = reader.GetValue<int>("tipo_barragem_id");
                        //caracterizacao.BarragemTipoTexto = reader.GetValue<string>("tipo_barragem_texto");
                        //caracterizacao.FinalidadeAtividade = reader.GetValue<int>("finalidade_atividade");
                        caracterizacao.cursoHidrico = reader.GetValue<string>("curso_hidrico");
                        caracterizacao.vazaoEnchente = reader.GetValue<decimal>("vazao_enchente");
                        caracterizacao.areaBaciaContribuicao = reader.GetValue<decimal>("area_bacia_contribuicao");
                        caracterizacao.Precipitacao = reader.GetValue<decimal>("precipitacao");
                        caracterizacao.periodoRetorno = reader.GetValue<int>("periodo_retorno");
                        caracterizacao.coeficienteEscoamento = reader.GetValue<decimal>("coeficiente_escoamento");
                        caracterizacao.tempoConcentracao = reader.GetValue<decimal>("tempo_concentracao");
                        caracterizacao.EquacaoCalculo = reader.GetValue<string>("equacao_calculo");
                        caracterizacao.areaAlagada = reader.GetValue<decimal>("area_alagada");
                        caracterizacao.volumeArmazanado = reader.GetValue<decimal>("volume_armazenado");
                        caracterizacao.Fase = reader.GetValue<int?>("fase_id");
                        caracterizacao.PossuiMonge = reader.GetValue<int?>("possui_monge");
                        caracterizacao.MongeTipo = reader.GetValue<int>("tipo_monge_id");
                        caracterizacao.EspecificacaoMonge = reader.GetValue<string>("especificacao_monge");
                        caracterizacao.PossuiVertedouro = reader.GetValue<int?>("possui_vertedouro");
                        caracterizacao.VertedouroTipo = reader.GetValue<int>("tipo_vertedouro_id");
                        caracterizacao.EspecificacaoVertedouro = reader.GetValue<string>("especificacao_vertedouro");
                        caracterizacao.PossuiEstruturaHidraulica = reader.GetValue<int?>("possui_estrutura_hidrau");
                        caracterizacao.AdequacoesRealizada = reader.GetValue<string>("adequacoes_realizada");
                        caracterizacao.DataInicioObra = reader.GetValue<string>("data_inicio_obra");
                        caracterizacao.DataPrevisaoTerminoObra = reader.GetValue<string>("data_previsao_obra");
                        caracterizacao.Coordenada.EastingUtmTexto = reader.GetValue<string>("easting");
                        caracterizacao.Coordenada.NorthingUtmTexto = reader.GetValue<string>("northing");
                        caracterizacao.FormacaoRT = reader.GetValue<int>("formacao_resp_tec");
                        caracterizacao.EspecificacaoRT = reader.GetValue<string>("especificacao_rt");
                        caracterizacao.Autorizacao.Id = reader.GetValue<int>("autorizacao_id");
                        caracterizacao.NumeroARTElaboracao = reader.GetValue<string>("numero_art_elaboracao");
                        caracterizacao.NumeroARTExecucao = reader.GetValue<string>("numero_art_execucao");
                    }

                    reader.Close();
                }
            }

            return caracterizacao;
        }

		internal CaracterizacaoPDF ObterDadosPdfTitulo(int empreendimento, int atividade, BancoDeDados banco)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}