using System;
using System.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using System.Collections.Generic;
using System.Linq;

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
				comando.AdicionarParametroEntrada("precipitacao", caracterizacao.precipitacao, DbType.Decimal);
				comando.AdicionarParametroEntrada("periodo_retorno", caracterizacao.periodoRetorno, DbType.Int32);
				comando.AdicionarParametroEntrada("coeficiente_escoamento", caracterizacao.coeficienteEscoamento, DbType.Decimal);
				comando.AdicionarParametroEntrada("tempo_concentracao", caracterizacao.tempoConcentracao, DbType.Decimal);
				comando.AdicionarParametroEntrada("equacao_calculo", caracterizacao.tempoConcentracaoEquacaoUtilizada, DbType.String);
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
					if (!String.IsNullOrWhiteSpace(x.nome))
					{
						comando = bancoDeDados.CriarComando(@"
						insert into crt_barragem_responsavel (id, barragem, tipo, nome, profissao, registro_crea,
							numero_art, autorizacao_crea, proprio_declarante)
						values (seq_crt_barragem_responsavel.nextval, :barragem, :tipo, :nome, :profissao, :registro_crea,
							:numero_art, :autorizacao_crea, :proprio_declarante) ", EsquemaCredenciadoBanco);
						var arquivo = (x.autorizacaoCREA != null) ? x.autorizacaoCREA.Id : null;
						comando.AdicionarParametroEntrada("barragem", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo", (int)x.tipo, DbType.Int32);
						comando.AdicionarParametroEntrada("nome", x.nome, DbType.String);
						comando.AdicionarParametroEntrada("profissao", x.profissao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("registro_crea", x.registroCREA, DbType.String);
						comando.AdicionarParametroEntrada("numero_art", x.numeroART, DbType.String);
						comando.AdicionarParametroEntrada("autorizacao_crea", arquivo, DbType.String);
						comando.AdicionarParametroEntrada("proprio_declarante", x.proprioDeclarante, DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);
					}
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

				#region Barragem
				Comando comando = bancoDeDados.CriarComando(@"update crt_barragem_dispensa_lic set
					tipo_barragem = :tipo_barragem, curso_hidrico = :curso_hidrico, vazao_enchente = :vazao_enchente, 
					area_bacia_contribuicao = :area_bacia_contribuicao, precipitacao = :precipitacao, periodo_retorno = :periodo_retorno, 
					coeficiente_escoamento = :coeficiente_escoamento, tempo_concentracao = :tempo_concentracao, equacao_calculo = :equacao_calculo, 
					area_alagada = :area_alagada, volume_armazenado = :volume_armazenado, fase = :fase,	possui_barragem_contigua = :possui_barragem_contigua, 
					altura_barramento = :altura_barramento, comprimento_barramento = :comprimento_barramento, 
					largura_base_barramento = :largura_base_barramento, largura_crista_barramento = :largura_crista_barramento,
					fonte_precipitacao = :fonte_precipitacao, fonte_coeficiente_escoamento = :fonte_coeficiente_escoamento, fonte_vazao_enchente = :fonte_vazao_enchente
				where id = :id ", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_barragem", (int)caracterizacao.BarragemTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("curso_hidrico", caracterizacao.cursoHidrico, DbType.String);
				comando.AdicionarParametroEntrada("vazao_enchente", caracterizacao.vazaoEnchente, DbType.Decimal);
				comando.AdicionarParametroEntrada("area_bacia_contribuicao", caracterizacao.areaBaciaContribuicao, DbType.Decimal);
				comando.AdicionarParametroEntrada("precipitacao", caracterizacao.precipitacao, DbType.Decimal);
				comando.AdicionarParametroEntrada("periodo_retorno", caracterizacao.periodoRetorno, DbType.Int32);
				comando.AdicionarParametroEntrada("coeficiente_escoamento", caracterizacao.coeficienteEscoamento, DbType.Decimal);
				comando.AdicionarParametroEntrada("tempo_concentracao", caracterizacao.tempoConcentracao, DbType.Decimal);
				comando.AdicionarParametroEntrada("equacao_calculo", caracterizacao.tempoConcentracaoEquacaoUtilizada, DbType.String);
				comando.AdicionarParametroEntrada("area_alagada", caracterizacao.areaAlagada, DbType.Decimal);
				comando.AdicionarParametroEntrada("volume_armazenado", caracterizacao.volumeArmazanado, DbType.Decimal);
				comando.AdicionarParametroEntrada("fase", caracterizacao.Fase, DbType.Int32);

				comando.AdicionarParametroEntrada("possui_barragem_contigua", caracterizacao.barragemContiguaMesmoNivel, DbType.Int32);
				comando.AdicionarParametroEntrada("altura_barramento", caracterizacao.alturaBarramento, DbType.Decimal);
				comando.AdicionarParametroEntrada("comprimento_barramento", caracterizacao.comprimentoBarramento, DbType.Decimal);
				comando.AdicionarParametroEntrada("largura_base_barramento", caracterizacao.larguraBaseBarramento, DbType.Decimal);
				comando.AdicionarParametroEntrada("largura_crista_barramento", caracterizacao.larguraCristaBarramento, DbType.Decimal);
				comando.AdicionarParametroEntrada("fonte_precipitacao", caracterizacao.fonteDadosPrecipitacao, DbType.String);
				comando.AdicionarParametroEntrada("fonte_coeficiente_escoamento", caracterizacao.fonteDadosCoeficienteEscoamento, DbType.String);
				comando.AdicionarParametroEntrada("fonte_vazao_enchente", caracterizacao.fonteDadosVazaoEnchente, DbType.String);

				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Construida/A Construir
				comando = bancoDeDados.CriarComando(@"
				update crt_barragem_construida_con set 
					supressao_app = :supressao_app, largura_demarcada = :largura_demarcada, largura_demarcada_legislacao = :largura_demarcada_legislacao,
					faixa_cercada = :faixa_cercada, descricao_desen_app = :descricao_desen_app, demarcacao_app = :demarcacao_app, 
					barramento_normas = :barramento_normas, barramento_adequacoes = :barramento_adequacoes, vazao_min_tipo = :vazao_min_tipo, 
					vazao_min_diametro = :vazao_min_diametro, vazao_min_instalado = :vazao_min_instalado, vazao_min_normas = :vazao_min_normas, 
					vazao_min_adequacoes = :vazao_min_adequacoes, vazao_max_tipo = :vazao_max_tipo, vazao_max_diametro = :vazao_max_diametro,
					vazao_max_instalado = :vazao_max_instalado, vazao_max_normas = :vazao_max_normas, vazao_max_adequacoes = :vazao_max_adequacoes, 
					mes_inicio_obra = :mes_inicio_obra, ano_inicio_obra = :ano_inicio_obra
				where barragem = :barragem ", EsquemaCredenciadoBanco);

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

				#region Finalidade Atividade
				comando = bancoDeDados.CriarComando(@" delete crt_barragem_finaldiade_ativ where barragem = :barragem", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("barragem", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.finalidade.ForEach(x => {
					comando = bancoDeDados.CriarComando(@"
					insert into crt_barragem_finaldiade_ativ(id, barragem, atividade)
						values(seq_crt_barragem_final_ativ.nextval, :barragem, :atividade) ", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("barragem", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("atividade", x, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
				});
				#endregion
				
				#region Coordenadas
				caracterizacao.coordenadas.ForEach(x => {
					comando = bancoDeDados.CriarComando(@"
					update crt_barragem_coordenada set northing = :northing, easting = :easting where barragem = :barragem and tipo = :tipo", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("barragem", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", (int)x.tipo, DbType.Int32);
					comando.AdicionarParametroEntrada("northing", x.northing, DbType.Int32);
					comando.AdicionarParametroEntrada("easting", x.easting, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
				});
				#endregion

				#region Responsaveis Técnicos
				var listId = String.Join(", ", caracterizacao.responsaveisTecnicos.Select(x => x.id).Where(x => x > 0).ToList());

				if (!String.IsNullOrWhiteSpace(listId))
				{
					comando = bancoDeDados.CriarComando($@" delete crt_barragem_responsavel where id not in ({listId}) and barragem = :barragem", EsquemaCredenciadoBanco);
					comando.AdicionarParametroEntrada("barragem", caracterizacao.Id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}

				caracterizacao.responsaveisTecnicos.ForEach(x => {
					int ?arquivo = null;
					if (!String.IsNullOrWhiteSpace(x.nome))
					{
						if (x.id > 0)
						{
							comando = bancoDeDados.CriarComando(@"
						update crt_barragem_responsavel set
 							nome = :nome, profissao = :profissao, registro_crea = :registro_crea,
							numero_art = :numero_art, autorizacao_crea = :autorizacao_crea, proprio_declarante = :proprio_declarante
						where barragem = :barragem  ", EsquemaCredenciadoBanco);
							comando.AdicionarParametroEntrada("barragem", caracterizacao.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"
						insert into crt_barragem_responsavel (id, barragem, tipo, nome, profissao, registro_crea,
							numero_art, autorizacao_crea, proprio_declarante)
						values (seq_crt_barragem_responsavel.nextval, :barragem, :tipo, :nome, :profissao, :registro_crea,
							:numero_art, :autorizacao_crea, :proprio_declarante) ", EsquemaCredenciadoBanco);
							comando.AdicionarParametroEntrada("barragem", caracterizacao.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("tipo", (int)x.tipo, DbType.Int32);
						}

						arquivo = (x.autorizacaoCREA != null) ? x.autorizacaoCREA.Id : null;
						comando.AdicionarParametroEntrada("nome", x.nome, DbType.String);
						comando.AdicionarParametroEntrada("profissao", x.profissao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("registro_crea", x.registroCREA, DbType.String);
						comando.AdicionarParametroEntrada("numero_art", x.numeroART, DbType.String);
						comando.AdicionarParametroEntrada("autorizacao_crea", arquivo, DbType.String);
						comando.AdicionarParametroEntrada("proprio_declarante", x.proprioDeclarante, DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				});
				#endregion
				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.barragemdispensalicenca, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		public void Excluir(int id, BancoDeDados banco = null)
		{
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao(); 

				#region Histórico

				//Atualizar o tid para a nova ação
                Comando comando = bancoDeDados.CriarComando(@"update {0}crt_barragem_dispensa_lic c set c.tid = :tid where c.id = :id", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.barragemdispensalicenca, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComandoPlSql(
				@"begin
					delete from crt_barragem_finaldiade_ativ where barragem = :id;
					delete from crt_barragem_finaldiade_ativ where barragem = :id;
					delete from crt_barragem_coordenada where barragem = :id;
					delete from crt_barragem_responsavel where barragem = :id;
					delete from crt_barragem_construida_con where barragem = :id;
					delete from crt_barragem_dispensa_lic where id = :id;
				end;", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();

				#endregion
			}
		}

		public void ExcluirPorId(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();
				
				#region Histórico

				//Atualizar o tid para a nova ação
				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_barragem_dispensa_lic c set c.tid = :tid where c.id = :id", EsquemaCredenciadoBanco);
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
                comando.AdicionarParametroEntrada("precipitacao", caracterizacao.precipitacao, DbType.Decimal);
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

		public List<BarragemDispensaLicenca> ObterLista(int empreendimentoId, int projetoDigitalId, bool simplificado = false, BancoDeDados banco = null)
		{
			List<BarragemDispensaLicenca> ListaDeBarragens = new List<BarragemDispensaLicenca>();
			BarragemDispensaLicenca caracterizacao;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.tid, c.interno_id, c.interno_tid, c.empreendimento,
					c.area_alagada, c.volume_armazenado
                from crt_barragem_dispensa_lic c, lov_crt_bdla_barragem_tipo lt
					where exists
					(

						select 1 from TAB_PROJ_DIGITAL_DEPENDENCIAS d
						where d.DEPENDENCIA_ID = c.id
						and d.DEPENDENCIA_TIPO = :dependencia_tipo
						and d.DEPENDENCIA_CARACTERIZACAO = :dependencia_caracterizacao
						and exists 
						(
							select 1 from TAB_PROJETO_DIGITAL p
							where d.PROJETO_DIGITAL_ID = p.id							
							and c.EMPREENDIMENTO = p.EMPREENDIMENTO
							and exists 
							(
								select 1 from TAB_TITULO t 
								where t.situacao = :titulo_situacao
								and p.REQUERIMENTO = t.REQUERIMENTO
							)
						)			
					)
					and not exists 
					(
						select 1 from TAB_PROJ_DIGITAL_DEPENDENCIAS d
						where d.DEPENDENCIA_ID = c.id
						and d.DEPENDENCIA_TIPO = :dependencia_tipo
						and d.DEPENDENCIA_CARACTERIZACAO = :dependencia_caracterizacao
						and d.PROJETO_DIGITAL_ID = :projeto_digital_id
					)
					and c.EMPREENDIMENTO = :empreendimentoId and lt.id = c.TIPO_BARRAGEM", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("empreendimentoId", empreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("projeto_digital_id", projetoDigitalId, DbType.Int32);
				comando.AdicionarParametroEntrada("dependencia_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependencia_caracterizacao", (int)eCaracterizacao.BarragemDispensaLicenca, DbType.Int32);
				comando.AdicionarParametroEntrada("titulo_situacao", (int)eTituloSituacao.Valido, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while(reader.Read())
					{
						caracterizacao = new BarragemDispensaLicenca();

						caracterizacao.Id = reader.GetValue<int>("id");
						caracterizacao.CredenciadoID = caracterizacao.Id;
						caracterizacao.Tid = reader.GetValue<string>("tid");
						caracterizacao.InternoID = reader.GetValue<int>("interno_id");
						caracterizacao.InternoTID = reader.GetValue<string>("interno_tid");

						caracterizacao.EmpreendimentoID = reader.GetValue<int>("empreendimento");
						caracterizacao.areaAlagada = reader.GetValue<decimal>("area_alagada");
						caracterizacao.volumeArmazanado = reader.GetValue<decimal>("volume_armazenado");

						caracterizacao.Atividade = String.Join(" / ", ObterListaFinalidadeAtividade(caracterizacao.Id));

						ListaDeBarragens.Add(caracterizacao);
					}

					reader.Close();
				}
			}
			

			return ListaDeBarragens;
		}

		public BarragemDispensaLicenca Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			BarragemDispensaLicenca caracterizacao = new BarragemDispensaLicenca();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select  b.id, b.tid, b.empreendimento, b.atividade, b.tipo_barragem, b.curso_hidrico, 
							b.vazao_enchente, b.area_bacia_contribuicao, b.precipitacao, b.periodo_retorno, 
							b.coeficiente_escoamento, b.tempo_concentracao, b.equacao_calculo, b.area_alagada, 
							b.volume_armazenado, b.fase, b.interno_id, b.interno_tid, b.possui_barragem_contigua, 
							b.altura_barramento, b.comprimento_barramento, b.largura_base_barramento, b.largura_crista_barramento,
							b.fonte_precipitacao, b.fonte_coeficiente_escoamento, b.fonte_vazao_enchente,
							c.id contruida_construir, c.supressao_app, c.largura_demarcada, c.largura_demarcada_legislacao,
							c.faixa_cercada, c.descricao_desen_app, c.demarcacao_app, c.barramento_normas, 
							c.barramento_adequacoes, c.vazao_min_tipo, c.vazao_min_diametro, c.vazao_min_instalado, 
							c.vazao_min_normas, c.vazao_min_adequacoes, c.vazao_max_tipo, c.vazao_max_diametro,
							c.vazao_max_instalado, c.vazao_max_normas, c.vazao_max_adequacoes, c.mes_inicio_obra, c.ano_inicio_obra
					from crt_barragem_dispensa_lic b
					inner join crt_barragem_construida_con c on b.id = c.barragem
					where b.id = :id", EsquemaCredenciadoBanco);

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
						caracterizacao.BarragemTipo = (eBarragemTipo)reader.GetValue<int>("tipo_barragem");
						//caracterizacao.BarragemTipoTexto = reader.GetValue<string>("tipo_barragem_texto");
						//caracterizacao.FinalidadeAtividade = reader.GetValue<int>("finalidade_atividade");
						caracterizacao.cursoHidrico = reader.GetValue<string>("curso_hidrico");
						caracterizacao.vazaoEnchente = reader.GetValue<decimal>("vazao_enchente");
						caracterizacao.areaBaciaContribuicao = reader.GetValue<decimal>("area_bacia_contribuicao");
						caracterizacao.precipitacao = reader.GetValue<decimal>("precipitacao");
						caracterizacao.periodoRetorno = reader.GetValue<int>("periodo_retorno");
						caracterizacao.coeficienteEscoamento = reader.GetValue<decimal>("coeficiente_escoamento");
						caracterizacao.tempoConcentracao = reader.GetValue<decimal>("tempo_concentracao");
						caracterizacao.tempoConcentracaoEquacaoUtilizada = reader.GetValue<string>("equacao_calculo");
						caracterizacao.areaAlagada = reader.GetValue<decimal>("area_alagada");
						caracterizacao.volumeArmazanado = reader.GetValue<decimal>("volume_armazenado");
						caracterizacao.Fase = reader.GetValue<int>("fase");

						caracterizacao.barragemContiguaMesmoNivel = reader.GetValue<bool>("possui_barragem_contigua");
						caracterizacao.alturaBarramento = reader.GetValue<decimal>("altura_barramento");
						caracterizacao.comprimentoBarramento = reader.GetValue<decimal>("comprimento_barramento");
						caracterizacao.larguraBaseBarramento = reader.GetValue<decimal>("largura_base_barramento");
						caracterizacao.larguraCristaBarramento = reader.GetValue<decimal>("largura_crista_barramento");
						caracterizacao.fonteDadosPrecipitacao = reader.GetValue<string>("fonte_precipitacao");
						caracterizacao.fonteDadosCoeficienteEscoamento = reader.GetValue<string>("fonte_coeficiente_escoamento");
						caracterizacao.fonteDadosVazaoEnchente = reader.GetValue<string>("fonte_vazao_enchente");
						caracterizacao.construidaConstruir.id = reader.GetValue<int>("contruida_construir");
						caracterizacao.construidaConstruir.isSupressaoAPP = reader.GetValue<bool>("supressao_app");
						caracterizacao.construidaConstruir.larguraDemarcada = reader.GetValue<decimal>("largura_demarcada");
						caracterizacao.construidaConstruir.larguraDemarcadaLegislacao = reader.GetValue<bool>("largura_demarcada_legislacao");
						caracterizacao.construidaConstruir.faixaCercada = reader.GetValue<int>("faixa_cercada");
						caracterizacao.construidaConstruir.descricacaoDesenvolvimentoAPP = reader.GetValue<string>("descricao_desen_app");
						caracterizacao.construidaConstruir.isDemarcacaoAPP = reader.GetValue<int>("demarcacao_app");
						caracterizacao.construidaConstruir.barramentoNormas = reader.GetValue<bool>("barramento_normas");
						caracterizacao.construidaConstruir.barramentoAdequacoes = reader.GetValue<string>("barramento_adequacoes");
						caracterizacao.construidaConstruir.vazaoMinTipo = reader.GetValue<int>("vazao_min_tipo");
						caracterizacao.construidaConstruir.vazaoMinDiametro = reader.GetValue<decimal>("vazao_min_diametro");
						caracterizacao.construidaConstruir.vazaoMinInstalado = reader.GetValue<bool>("vazao_min_instalado");
						caracterizacao.construidaConstruir.vazaoMinNormas = reader.GetValue<bool>("vazao_min_normas");
						caracterizacao.construidaConstruir.vazaoMinAdequacoes = reader.GetValue<string>("vazao_min_adequacoes");
						caracterizacao.construidaConstruir.vazaoMaxTipo = reader.GetValue<int>("vazao_max_tipo");
						caracterizacao.construidaConstruir.vazaoMaxDiametro = reader.GetValue<decimal>("vazao_max_diametro");
						caracterizacao.construidaConstruir.vazaoMaxInstalado = reader.GetValue<bool>("vazao_max_instalado");
						caracterizacao.construidaConstruir.vazaoMaxNormas = reader.GetValue<bool>("vazao_max_normas");
						caracterizacao.construidaConstruir.vazaoMaxAdequacoes = reader.GetValue<string>("vazao_max_adequacoes");
						caracterizacao.construidaConstruir.mesInicioObra = reader.GetValue<int>("mes_inicio_obra");
						caracterizacao.construidaConstruir.anoInicioObra = reader.GetValue<int>("ano_inicio_obra");
					}

					reader.Close();
				}

				#region Coordenadas
				comando = bancoDeDados.CriarComando(@"
					select c.id, c.tipo, c.northing, c.easting from crt_barragem_coordenada c
						where c.barragem = :barragem order by tipo", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("barragem", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						var obj = caracterizacao.coordenadas.FirstOrDefault(x => (int)x.tipo == reader.GetValue<int>("tipo"));
						obj.id = reader.GetValue<int>("id");
						obj.tipo = (eTipoCoordenadaBarragem)reader.GetValue<int>("tipo");
						obj.northing = reader.GetValue<int>("northing");
						obj.easting = reader.GetValue<int>("easting");
					}
				}
				#endregion

				#region Responsaveis Tecnicos
				comando = bancoDeDados.CriarComando(@"
					select r.id, r.tipo, r.nome, r.profissao, r.registro_crea, r.numero_art, r.autorizacao_crea, r.proprio_declarante
						from crt_barragem_responsavel r where r.barragem = :barragem", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("barragem", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						var obj = caracterizacao.responsaveisTecnicos.FirstOrDefault(x => (int)x.tipo == reader.GetValue<int>("tipo"));
						obj.id = reader.GetValue<int>("id");
						obj.tipo = (eTipoRT)reader.GetValue<int>("tipo");
						obj.nome = reader.GetValue<string>("nome");
						obj.profissao.Id = reader.GetValue<int>("profissao");
						obj.registroCREA = reader.GetValue<string>("registro_crea");
						obj.numeroART = reader.GetValue<string>("numero_art");
						if(obj.tipo == eTipoRT.ElaboracaoProjeto)
							obj.autorizacaoCREA.Id = reader.GetValue<int>("autorizacao_crea");
						obj.proprioDeclarante = reader.GetValue<bool>("proprio_declarante");
					}
				}
				#endregion

				#region Finalidade Atividade
				comando = bancoDeDados.CriarComando(@"
					select  f.atividade from crt_barragem_finaldiade_ativ f where f.barragem = :barragem", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("barragem", id, DbType.Int32);

				caracterizacao.finalidade = bancoDeDados.ExecutarList<int>(comando);
				#endregion


			}
			return caracterizacao;
		}

		public List<BarragemDispensaLicenca> ObterBarragemAssociada(int projetoDigitalId, bool simplificado = false, BancoDeDados banco = null)
		{
			List<BarragemDispensaLicenca> ListaBarragem = new List<BarragemDispensaLicenca>();
			BarragemDispensaLicenca caracterizacao = new BarragemDispensaLicenca();
			Dependencia dependenciaCaracterizacao = new Dependencia();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				int retornoComando = 0;

				Comando comando = bancoDeDados.CriarComando(@"select dependencia_id from tab_proj_digital_dependencias WHERE PROJETO_DIGITAL_ID = :projetoDigitalId", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("projetoDigitalId", projetoDigitalId, DbType.UInt32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read()) { dependenciaCaracterizacao.DependenciaId = reader.GetValue<int>("dependencia_id"); };
				}

				retornoComando = dependenciaCaracterizacao.DependenciaId;

				comando = bancoDeDados.CriarComando(@"select c.id, c.tid, c.interno_id, c.interno_tid, c.empreendimento,
					c.area_alagada, c.volume_armazenado, 
				(select count(*) from TAB_PROJ_DIGITAL_DEPENDENCIAS d
				where d.DEPENDENCIA_ID = c.id
				and d.DEPENDENCIA_TIPO = :dependencia_tipo
				and d.DEPENDENCIA_CARACTERIZACAO = :dependencia_caracterizacao
				and exists
				(
					select 1 from TAB_PROJETO_DIGITAL p
					where c.EMPREENDIMENTO = p.EMPREENDIMENTO
					and p.id = d.PROJETO_DIGITAL_ID
				)) as possui_associacao_externa
                from crt_barragem_dispensa_lic c, lov_crt_bdla_barragem_tipo lt
				where lt.id = c.tipo_barragem and c.id = :retornoComando", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("retornoComando", retornoComando, DbType.Int32);
				comando.AdicionarParametroEntrada("dependencia_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependencia_caracterizacao", (int)eCaracterizacao.BarragemDispensaLicenca, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						caracterizacao.Id = reader.GetValue<int>("id");
						caracterizacao.CredenciadoID = retornoComando;
						caracterizacao.Tid = reader.GetValue<string>("tid");
						caracterizacao.InternoID = reader.GetValue<int>("interno_id");
						caracterizacao.InternoTID = reader.GetValue<string>("interno_tid");

						caracterizacao.EmpreendimentoID = reader.GetValue<int>("empreendimento");
						caracterizacao.areaAlagada = reader.GetValue<decimal>("area_alagada");
						caracterizacao.volumeArmazanado = reader.GetValue<decimal>("volume_armazenado");
						caracterizacao.PossuiAssociacaoExterna = reader.GetValue<int>("possui_associacao_externa") > 1;

						caracterizacao.Atividade = String.Join(" / ", ObterListaFinalidadeAtividade(caracterizacao.Id));

						ListaBarragem.Add(caracterizacao);

					}
					reader.Close();
				}
			}
			return ListaBarragem;
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
                        caracterizacao.precipitacao = reader.GetValue<decimal>("precipitacao");
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

		internal bool PossuiAssociacaoExterna(int empreendimento, int projetoDigitalId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}TAB_PROJ_DIGITAL_DEPENDENCIAS d
				where d.DEPENDENCIA_TIPO = :dependencia_tipo
				and d.DEPENDENCIA_CARACTERIZACAO = :dependencia_caracterizacao
				and exists
				(
					select 1 from {0}TAB_PROJETO_DIGITAL p
					where p.EMPREENDIMENTO = :empreendimento
					and p.id = d.PROJETO_DIGITAL_ID
					and p.id <> :projeto_digital
				)", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("projeto_digital", projetoDigitalId, DbType.Int32);
				comando.AdicionarParametroEntrada("dependencia_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependencia_caracterizacao", (int)eCaracterizacao.BarragemDispensaLicenca, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal BarragemRT ObterResponsavelTecnicoRequerimento(int projetoDigital, BancoDeDados banco = null)
		{

			BarragemRT rt = new BarragemRT();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select pe.nome, pp.profissao, pp.registro, rr.numero_art 
					from tab_projeto_digital pd
						inner join tab_requerimento_responsavel rr on rr.requerimento = pd.requerimento
						inner join tab_pessoa pe on pe.id = rr.responsavel
						inner join tab_pessoa_profissao pp on pe.id = pp.pessoa
					where pd.id = :projetoDigital", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("projetoDigital", projetoDigital, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						rt.nome = reader.GetValue<string>("nome");
						rt.profissao.Id = reader.GetValue<int>("profissao");
						rt.registroCREA = reader.GetValue<string>("registro");
						rt.numeroART = reader.GetValue<string>("numero_art");
					}

					reader.Close();
				}

			}
			return rt;
		}

		internal bool ObterBarragemContiguaMesmoNivel(int projetoDigital, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select possui_barragem_contigua from tab_projeto_digital p 
						inner join tab_requerimento_barragem r on p.requerimento = r.requerimento
					where p.id = :projetoDigital", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("projetoDigital", projetoDigital, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal List<string> ObterListaFinalidadeAtividade(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select a.texto from crt_barragem_finaldiade_ativ f
						inner join lov_crt_bdla_finalidade_atv a on f.atividade = a.id
					where f.barragem = :barragem", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("barragem", id, DbType.Int32);

				return bancoDeDados.ExecutarList<string>(comando);
			}
		}
		#endregion

		#region Validações

		internal decimal AreaAlagadaConfiguracao(decimal area, BancoDeDados banco = null)
		{

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select area_alagada from tab_titulo_configuracao", EsquemaCredenciadoBanco);

				return bancoDeDados.ExecutarScalar<decimal>(comando);
			}
		}

		internal decimal VolumeArmazenadoConfiguracao(decimal area, BancoDeDados banco = null)
		{

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select volume_armazenado from tab_titulo_configuracao", EsquemaCredenciadoBanco);

				return bancoDeDados.ExecutarScalar<decimal>(comando);
			}
		}

		internal bool VerificarElaboracaoRT(int projetoDigital, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select count(1) from tab_projeto_digital pd
						inner join tab_requerimento_barragem rb on rb.requerimento = pd.requerimento
					where pd.id = :projetoDigital and rb.rt_elaboracao in (1,3)", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("projetoDigital", projetoDigital, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}
		#endregion
	}
}