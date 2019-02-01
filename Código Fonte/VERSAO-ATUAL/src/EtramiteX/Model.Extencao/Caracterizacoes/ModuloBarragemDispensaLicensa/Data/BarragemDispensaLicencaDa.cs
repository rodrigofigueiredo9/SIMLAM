using System;
using System.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Data
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

		private String EsquemaBanco { get; set; }

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
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				insert into crt_barragem_dispensa_lic (id, tid, empreendimento, atividade, tipo_barragem, finalidade_atividade, curso_hidrico, vazao_enchente, area_bacia_contribuicao, 
				precipitacao, periodo_retorno, coeficiente_escoamento, tempo_concentracao, equacao_calculo, area_alagada, volume_armazenado, fase, possui_monge, tipo_monge, especificacao_monge, 
				possui_vertedouro, tipo_vertedouro, especificacao_vertedouro, possui_estrutura_hidrau, adequacoes_realizada, data_inicio_obra, data_previsao_obra, easting, northing, formacao_resp_tec, 
				especificacao_rt, autorizacao, numero_art_elaboracao, numero_art_execucao)
				values (seq_crt_barragem_dispensa_lic.nextval, :tid, :empreendimento, :atividade, :tipo_barragem, :finalidade_atividade, :curso_hidrico, :vazao_enchente, :area_bacia_contribuicao, 
				:precipitacao, :periodo_retorno, :coeficiente_escoamento, :tempo_concentracao, :equacao_calculo, :area_alagada, :volume_armazenado, :fase, :possui_monge, :tipo_monge, :especificacao_monge, 
				:possui_vertedouro, :tipo_vertedouro, :especificacao_vertedouro, :possui_estrutura_hidrau, :adequacoes_realizada, :data_inicio_obra, :data_previsao_obra, :easting, :northing, :formacao_resp_tec, 
				:especificacao_rt, :autorizacao, :numero_art_elaboracao, :numero_art_execucao) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.AtividadeID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_barragem", caracterizacao.BarragemTipo, DbType.Int32);
				//comando.adicionarparametroentrada("finalidade_atividade", caracterizacao.finalidadeatividade, dbtype.int32);
				//comando.adicionarparametroentrada("curso_hidrico", caracterizacao.cursohidrico, dbtype.string);
				//comando.adicionarparametroentrada("vazao_enchente", caracterizacao.vazaoenchente, dbtype.decimal);
				//comando.adicionarparametroentrada("area_bacia_contribuicao", caracterizacao.areabaciacontribuicao, dbtype.decimal);
				//comando.adicionarparametroentrada("precipitacao", caracterizacao.precipitacao, dbtype.decimal);
				//comando.adicionarparametroentrada("periodo_retorno", caracterizacao.periodoretorno, dbtype.int32);
				//comando.adicionarparametroentrada("coeficiente_escoamento", caracterizacao.coeficienteescoamento, dbtype.string);
				//comando.adicionarparametroentrada("tempo_concentracao", caracterizacao.tempoconcentracao, dbtype.string);
				//comando.adicionarparametroentrada("equacao_calculo", caracterizacao.equacaocalculo, dbtype.string);
				//comando.adicionarparametroentrada("area_alagada", caracterizacao.areaalagada, dbtype.decimal);
				//comando.adicionarparametroentrada("volume_armazenado", caracterizacao.volumearmazanado, dbtype.decimal);
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
				
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.barragemdispensalicenca, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(BarragemDispensaLicenca caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update crt_barragem_dispensa_lic c set c.tid = :tid, c.empreendimento = :empreendimento, c.atividade = :atividade, c.tipo_barragem = :tipo_barragem, 
				c.finalidade_atividade = :finalidade_atividade, c.curso_hidrico = :curso_hidrico, c.vazao_enchente = :vazao_enchente, c.area_bacia_contribuicao = :area_bacia_contribuicao, c.precipitacao = :precipitacao, 
				c.periodo_retorno = :periodo_retorno, c.coeficiente_escoamento = :coeficiente_escoamento, c.tempo_concentracao = :tempo_concentracao, c.equacao_calculo = :equacao_calculo, c.area_alagada = :area_alagada, 
				c.volume_armazenado = :volume_armazenado, c.fase = :fase, c.possui_monge = :possui_monge, c.tipo_monge = :tipo_monge, c.especificacao_monge = :especificacao_monge, c.possui_vertedouro = :possui_vertedouro, 
				c.tipo_vertedouro = :tipo_vertedouro, c.especificacao_vertedouro = :especificacao_vertedouro, c.possui_estrutura_hidrau = :possui_estrutura_hidrau, c.adequacoes_realizada = :adequacoes_realizada, 
				c.data_inicio_obra = :data_inicio_obra, c.data_previsao_obra = :data_previsao_obra, c.easting = :easting, c.northing = :northing, c.formacao_resp_tec = :formacao_resp_tec, c.especificacao_rt = :especificacao_rt, 
				c.autorizacao = :autorizacao, c.numero_art_elaboracao = :numero_art_elaboracao, c.numero_art_execucao = :numero_art_execucao where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.AtividadeID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_barragem", caracterizacao.BarragemTipo, DbType.Int32);
				//comando.AdicionarParametroEntrada("finalidade_atividade", caracterizacao.FinalidadeAtividade, DbType.Int32);
				//comando.AdicionarParametroEntrada("curso_hidrico", caracterizacao.CursoHidrico, DbType.String);
				//comando.AdicionarParametroEntrada("vazao_enchente", caracterizacao.VazaoEnchente, DbType.Decimal);
				//comando.AdicionarParametroEntrada("area_bacia_contribuicao", caracterizacao.AreaBaciaContribuicao, DbType.Decimal);
				//comando.AdicionarParametroEntrada("precipitacao", caracterizacao.Precipitacao, DbType.Decimal);
				//comando.AdicionarParametroEntrada("periodo_retorno", caracterizacao.PeriodoRetorno, DbType.Int32);
				//comando.AdicionarParametroEntrada("coeficiente_escoamento", caracterizacao.CoeficienteEscoamento, DbType.String);
				//comando.AdicionarParametroEntrada("tempo_concentracao", caracterizacao.TempoConcentracao, DbType.String);
				//comando.AdicionarParametroEntrada("equacao_calculo", caracterizacao.EquacaoCalculo, DbType.String);
				//comando.AdicionarParametroEntrada("area_alagada", caracterizacao.AreaAlagada, DbType.Decimal);
				//comando.AdicionarParametroEntrada("volume_armazenado", caracterizacao.VolumeArmazanado, DbType.Decimal);
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
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_barragem_dispensa_lic c where c.empreendimento = :empreendimento", EsquemaBanco);
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
				comando = bancoDeDados.CriarComando(@"update {0}crt_barragem_dispensa_lic c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.barragemdispensalicenca, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComandoPlSql(
				@"begin 
					delete from crt_barragem_dispensa_lic where id = :id;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();

				#endregion
			}
		}

        internal void CopiarDadosCredenciado(BarragemDispensaLicenca caracterizacao, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
            {
                bancoDeDados.IniciarTransacao();
                Comando comando;

                if (caracterizacao.Id <= 0)
                {
                    comando = bancoDeDados.CriarComando(@"
				    insert into crt_barragem_dispensa_lic (id, tid, empreendimento, atividade, tipo_barragem, finalidade_atividade, curso_hidrico, vazao_enchente, area_bacia_contribuicao, 
				    precipitacao, periodo_retorno, coeficiente_escoamento, tempo_concentracao, equacao_calculo, area_alagada, volume_armazenado, fase, possui_monge, tipo_monge, especificacao_monge, 
				    possui_vertedouro, tipo_vertedouro, especificacao_vertedouro, possui_estrutura_hidrau, adequacoes_realizada, data_inicio_obra, data_previsao_obra, easting, northing, formacao_resp_tec, 
				    especificacao_rt, autorizacao, numero_art_elaboracao, numero_art_execucao)
				    values (seq_crt_barragem_dispensa_lic.nextval, :tid, :empreendimento, :atividade, :tipo_barragem, :finalidade_atividade, :curso_hidrico, :vazao_enchente, :area_bacia_contribuicao, 
				    :precipitacao, :periodo_retorno, :coeficiente_escoamento, :tempo_concentracao, :equacao_calculo, :area_alagada, :volume_armazenado, :fase, :possui_monge, :tipo_monge, :especificacao_monge, 
				    :possui_vertedouro, :tipo_vertedouro, :especificacao_vertedouro, :possui_estrutura_hidrau, :adequacoes_realizada, :data_inicio_obra, :data_previsao_obra, :easting, :northing, :formacao_resp_tec, 
				    :especificacao_rt, :autorizacao, :numero_art_elaboracao, :numero_art_execucao) returning id into :id", EsquemaBanco);

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
				    c.autorizacao = :autorizacao, c.numero_art_elaboracao = :numero_art_elaboracao, c.numero_art_execucao = :numero_art_execucao
                    where c.id = :id", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
                }

                comando.AdicionarParametroEntrada("atividade", caracterizacao.AtividadeID, DbType.Int32);
                comando.AdicionarParametroEntrada("tipo_barragem", caracterizacao.BarragemTipo, DbType.Int32);
                //comando.AdicionarParametroEntrada("finalidade_atividade", caracterizacao.FinalidadeAtividade, DbType.Int32);
                //comando.AdicionarParametroEntrada("curso_hidrico", caracterizacao.CursoHidrico, DbType.String);
                //comando.AdicionarParametroEntrada("vazao_enchente", caracterizacao.VazaoEnchente, DbType.Decimal);
                //comando.AdicionarParametroEntrada("area_bacia_contribuicao", caracterizacao.AreaBaciaContribuicao, DbType.Decimal);
                //comando.AdicionarParametroEntrada("precipitacao", caracterizacao.Precipitacao, DbType.Decimal);
                //comando.AdicionarParametroEntrada("periodo_retorno", caracterizacao.PeriodoRetorno, DbType.Int32);
                //comando.AdicionarParametroEntrada("coeficiente_escoamento", caracterizacao.CoeficienteEscoamento, DbType.String);
                //comando.AdicionarParametroEntrada("tempo_concentracao", caracterizacao.TempoConcentracao, DbType.String);
                //comando.AdicionarParametroEntrada("equacao_calculo", caracterizacao.EquacaoCalculo, DbType.String);
                //comando.AdicionarParametroEntrada("area_alagada", caracterizacao.AreaAlagada, DbType.Decimal);
                //comando.AdicionarParametroEntrada("volume_armazenado", caracterizacao.VolumeArmazanado, DbType.Decimal);
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

                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                bancoDeDados.ExecutarNonQuery(comando);

                if (caracterizacao.Id <= 0)
                {
                    caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
                }

                Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.barragemdispensalicenca, eHistoricoAcao.importar, bancoDeDados, null);

                bancoDeDados.Commit();
            }
        }

		#endregion

		#region Obter

		internal BarragemDispensaLicenca ObterPorEmpreendimento(int empreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			BarragemDispensaLicenca caracterizacao = new BarragemDispensaLicenca();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from {0}crt_barragem_dispensa_lic where empreendimento = :empreendimento", EsquemaBanco);

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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.tid, c.empreendimento, c.atividade, c.tipo_barragem, lt.texto tipo_barragem_texto, c.finalidade_atividade, c.curso_hidrico, 
				c.vazao_enchente, c.area_bacia_contribuicao, c.precipitacao, c.periodo_retorno, c.coeficiente_escoamento, c.tempo_concentracao, c.equacao_calculo, c.area_alagada, c.volume_armazenado, 
				c.fase, c.possui_monge, c.tipo_monge, c.especificacao_monge, c.possui_vertedouro, c.tipo_vertedouro, c.especificacao_vertedouro, c.possui_estrutura_hidrau, c.adequacoes_realizada, 
				c.data_inicio_obra, c.data_previsao_obra, c.easting, c.northing, c.formacao_resp_tec, c.especificacao_rt, c.autorizacao, c.numero_art_elaboracao, c.numero_art_execucao 
                from crt_barragem_dispensa_lic c, lov_crt_bdla_barragem_tipo lt where lt.id = c.tipo_barragem and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = reader.GetValue<int>("id");
						caracterizacao.Tid = reader.GetValue<string>("tid");
						caracterizacao.EmpreendimentoID = reader.GetValue<int>("empreendimento");
						caracterizacao.AtividadeID = reader.GetValue<int>("atividade");
						//caracterizacao.BarragemTipo = reader.GetValue<int>("tipo_barragem");
						//caracterizacao.BarragemTipoTexto = reader.GetValue<string>("tipo_barragem_texto");
						//caracterizacao.FinalidadeAtividade = reader.GetValue<int>("finalidade_atividade");
						//caracterizacao.CursoHidrico = reader.GetValue<string>("curso_hidrico");
						//caracterizacao.VazaoEnchente = reader.GetValue<decimal?>("vazao_enchente");
						//caracterizacao.AreaBaciaContribuicao = reader.GetValue<decimal?>("area_bacia_contribuicao");
						//caracterizacao.Precipitacao = reader.GetValue<decimal?>("precipitacao");
						//caracterizacao.PeriodoRetorno = reader.GetValue<int?>("periodo_retorno");
						//caracterizacao.CoeficienteEscoamento = reader.GetValue<string>("coeficiente_escoamento");
						//caracterizacao.TempoConcentracao = reader.GetValue<string>("tempo_concentracao");
						//caracterizacao.EquacaoCalculo = reader.GetValue<string>("equacao_calculo");
						//caracterizacao.AreaAlagada = reader.GetValue<decimal?>("area_alagada");
						//caracterizacao.VolumeArmazanado = reader.GetValue<decimal?>("volume_armazenado");
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

		internal CaracterizacaoPDF ObterDadosPdfTitulo(int empreendimento, int atividade, BancoDeDados banco)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}