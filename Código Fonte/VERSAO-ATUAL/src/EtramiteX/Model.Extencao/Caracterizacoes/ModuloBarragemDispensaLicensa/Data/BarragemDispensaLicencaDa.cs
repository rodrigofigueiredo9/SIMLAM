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
using System.Collections.Generic;
using System.Linq;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Data
{
	public class BarragemDispensaLicencaDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		CaracterizacaoDa _caracterizacaoDa = new CaracterizacaoDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public String EsquemaBancoGeo { get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); } }

		private String EsquemaCredenciadoBanco { get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); } }

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

		public void Excluir(int id, BancoDeDados banco = null, bool excluirCred = false)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				//bancoDeDados.IniciarTransacao();

				#region Histórico

				//Atualizar o tid para a nova ação
				Comando comando = bancoDeDados.CriarComando(@"update crt_barragem_dispensa_lic c set c.tid = :tid where c.id = :id");
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.barragemdispensalicenca, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComandoPlSql(
				@"begin
					delete from tab_requerimento_barragem where barragem = :id;
					delete from CRT_BARRAGEM_FINALIDADE_ATIV where barragem = :id;
					delete from crt_barragem_coordenada where barragem = :id;
					delete from crt_barragem_responsavel where barragem = :id;
					delete from crt_barragem_construida_con where barragem = :id;
					delete from crt_barragem_dispensa_lic where id = :id;
				end;");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();

				#endregion

				
			}
			if (excluirCred)
				using (BancoDeDados bd = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
				{
					int interno = 0;
					Comando comando = bd.CriarComando(@"select id from crt_barragem_dispensa_lic where interno_id = :interno");
					comando.AdicionarParametroEntrada("interno", id, DbType.Int32);
					interno = bd.ExecutarScalar<int>(comando);
					Excluir(interno, bd);
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


		public List<BarragemDispensaLicenca> ObterLista(int empreendimentoId, int projetoDigitalId, bool simplificado = false, BancoDeDados banco = null)
		{
			List<BarragemDispensaLicenca> ListaDeBarragens = new List<BarragemDispensaLicenca>();
			BarragemDispensaLicenca caracterizacao;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select  r.numero requerimento, 
							n.numero || '/' || n.ano numero_titulo,
							t.id titulo_id,
							b.id, 
							b.empreendimento,
							b.area_alagada, 
							b.volume_armazenado
					from tab_titulo t 
					inner join tab_titulo_numero n on t.id = n.titulo
					inner join idafcredenciado.tab_requerimento r on r.id = t.requerimento
					inner join tab_requerimento ri on ri.numero = r.id
					inner join tab_requerimento_barragem rb on rb.requerimento = ri.id
					inner join crt_barragem_dispensa_lic b on b.id = rb.barragem

					where t.empreendimento in ( select id from idafcredenciado.tab_empreendimento e
									where interno = :empreendimento ) 
						and t.modelo = 72 and t.credenciado is not null ");

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						caracterizacao = new BarragemDispensaLicenca();

						caracterizacao.Id = reader.GetValue<int>("id");
						caracterizacao.RequerimentoId = reader.GetValue<int>("requerimento");
						caracterizacao.TituloNumero = reader.GetValue<string>("numero_titulo"); ;
						caracterizacao.TituloId = reader.GetValue<int>("titulo_id"); ;
						caracterizacao.EmpreendimentoID = empreendimentoId;
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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select  b.id, b.tid, b.empreendimento, b.atividade, b.tipo_barragem, b.curso_hidrico, 
							b.vazao_enchente, b.area_bacia_contribuicao, b.precipitacao, b.periodo_retorno, 
							b.coeficiente_escoamento, b.tempo_concentracao, b.equacao_calculo, b.area_alagada, 
							b.volume_armazenado, b.fase, b.possui_barragem_contigua, 
							b.altura_barramento, b.comprimento_barramento, b.largura_base_barramento, b.largura_crista_barramento,
							b.fonte_precipitacao, b.fonte_coeficiente_escoamento, b.fonte_vazao_enchente,
							c.id contruida_construir, c.supressao_app, c.largura_demarcada, c.largura_demarcada_legislacao,
							c.faixa_cercada, c.descricao_desen_app, c.demarcacao_app, c.barramento_normas, 
							c.barramento_adequacoes, c.vazao_min_tipo, c.vazao_min_diametro, c.vazao_min_instalado, 
							c.vazao_min_normas, c.vazao_min_adequacoes, c.vazao_max_tipo, c.vazao_max_diametro,
							c.vazao_max_instalado, c.vazao_max_normas, c.vazao_max_adequacoes, c.periodo_inicio_obra, c.periodo_termino_obra,
							r.requerimento
					from crt_barragem_dispensa_lic b
					inner join crt_barragem_construida_con c on b.id = c.barragem
					inner join tab_requerimento_barragem r on r.barragem = b.id
					where b.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = reader.GetValue<int>("id");
						caracterizacao.CredenciadoID = id;
						caracterizacao.Tid = reader.GetValue<string>("tid");

						caracterizacao.EmpreendimentoID = reader.GetValue<int>("empreendimento");
						caracterizacao.AtividadeID = reader.GetValue<int>("atividade");
						caracterizacao.BarragemTipo = (eBarragemTipo)reader.GetValue<int>("tipo_barragem");
						caracterizacao.cursoHidrico = reader.GetValue<string>("curso_hidrico");
						caracterizacao.vazaoEnchente = reader.GetValue<decimal>("vazao_enchente");
						caracterizacao.areaBaciaContribuicao = reader.GetValue<decimal>("area_bacia_contribuicao");
						caracterizacao.precipitacao = reader.GetValue<decimal>("precipitacao");
						caracterizacao.periodoRetorno = reader.GetValue<decimal>("periodo_retorno");
						caracterizacao.coeficienteEscoamento = reader.GetValue<decimal>("coeficiente_escoamento");
						caracterizacao.tempoConcentracao = reader.GetValue<decimal>("tempo_concentracao");
						caracterizacao.tempoConcentracaoEquacaoUtilizada = reader.GetValue<string>("equacao_calculo");
						caracterizacao.areaAlagada = reader.GetValue<decimal>("area_alagada");
						caracterizacao.volumeArmazanado = reader.GetValue<decimal>("volume_armazenado");
						caracterizacao.Fase = reader.GetValue<int>("fase");
						caracterizacao.RequerimentoId = reader.GetValue<int>("requerimento");

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
						caracterizacao.construidaConstruir.descricaoDesenvolvimentoAPP = reader.GetValue<string>("descricao_desen_app");
						caracterizacao.construidaConstruir.isDemarcacaoAPP = reader.GetValue<int>("demarcacao_app");
						caracterizacao.construidaConstruir.barramentoNormas = reader.GetValue<bool>("barramento_normas");
						caracterizacao.construidaConstruir.barramentoAdequacoes = reader.GetValue<string>("barramento_adequacoes");
						caracterizacao.construidaConstruir.vazaoMinTipo = reader.GetValue<int>("vazao_min_tipo");
						caracterizacao.construidaConstruir.vazaoMinDiametro = reader.GetValue<decimal>("vazao_min_diametro");
						caracterizacao.construidaConstruir.vazaoMinInstalado = reader.GetValue<bool>("vazao_min_instalado");
						caracterizacao.construidaConstruir.vazaoMinNormas = reader.GetValue<bool>("vazao_min_normas");
						caracterizacao.construidaConstruir.vazaoMinAdequacoes = reader.GetValue<string>("vazao_min_adequacoes");
						caracterizacao.construidaConstruir.vazaoMaxTipo = reader.GetValue<int>("vazao_max_tipo");
						caracterizacao.construidaConstruir.vazaoMaxDiametro = reader.GetValue<string>("vazao_max_diametro");
						caracterizacao.construidaConstruir.vazaoMaxInstalado = reader.GetValue<bool>("vazao_max_instalado");
						caracterizacao.construidaConstruir.vazaoMaxNormas = reader.GetValue<bool>("vazao_max_normas");
						caracterizacao.construidaConstruir.vazaoMaxAdequacoes = reader.GetValue<string>("vazao_max_adequacoes");
						caracterizacao.construidaConstruir.periodoInicioObra = reader.GetValue<string>("periodo_inicio_obra");
						caracterizacao.construidaConstruir.periodoTerminoObra = reader.GetValue<string>("periodo_termino_obra");
					}

					reader.Close();
				}
				
				#region Coordenadas
				comando = bancoDeDados.CriarComando(@"
					select c.id, c.tipo, c.northing, c.easting from crt_barragem_coordenada c
						where c.barragem = :barragem order by tipo");

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
						from crt_barragem_responsavel r where r.barragem = :barragem");

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
						if (obj.tipo == eTipoRT.ElaboracaoProjeto)
							obj.autorizacaoCREA.Id = reader.GetValue<int>("autorizacao_crea");
						obj.proprioDeclarante = reader.GetValue<bool>("proprio_declarante");
					}
				}
				#endregion

				#region Finalidade Atividade
				comando = bancoDeDados.CriarComando(@"
					select  f.atividade from CRT_BARRAGEM_FINALIDADE_ATIV f where f.barragem = :barragem");

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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				int retornoComando = 0;

				Comando comando = bancoDeDados.CriarComando(@"select dependencia_id from tab_proj_digital_dependencias WHERE PROJETO_DIGITAL_ID = :projetoDigitalId");

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
				where lt.id = c.tipo_barragem and c.id = :retornoComando");

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

		internal CaracterizacaoPDF ObterDadosPdfTitulo(int empreendimento, int atividade, BancoDeDados banco)
		{
			throw new NotImplementedException();
		}

		internal List<string> ObterListaFinalidadeAtividade(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select a.texto from CRT_BARRAGEM_FINALIDADE_ATIV f
						inner join lov_crt_bdla_finalidade_atv a on f.atividade = a.id
					where f.barragem = :barragem");

				comando.AdicionarParametroEntrada("barragem", id, DbType.Int32);

				return bancoDeDados.ExecutarList<string>(comando);
			}
		}

		internal BarragemRT ObterResponsavelTecnicoRequerimento(int projetoDigital, BancoDeDados banco = null)
		{

			BarragemRT rt = new BarragemRT();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select pe.nome, pp.profissao, pp.registro, rr.numero_art 
					from tab_requerimento_responsavel rr
						inner join tab_pessoa pe on pe.id = rr.responsavel
						inner join tab_pessoa_profissao pp on pe.id = pp.pessoa
					where rr.requerimento = :requerimento");

				comando.AdicionarParametroEntrada("requerimento", projetoDigital, DbType.Int32);

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
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select possui_barragem_contigua from tab_requerimento_barragem r
						where r.requerimento in (
							select p.requerimento from tab_projeto_digital p where p.id = :projetoDigital )");

				comando.AdicionarParametroEntrada("projetoDigital", projetoDigital, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		#endregion

		#region Auxiliares
		internal bool PossuiAssociacaoExterna(int empreendimento, int projetoDigitalId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
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
				)");

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("projeto_digital", projetoDigitalId, DbType.Int32);
				comando.AdicionarParametroEntrada("dependencia_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependencia_caracterizacao", (int)eCaracterizacao.BarragemDispensaLicenca, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool PossuiTituloValido(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					SELECT COUNT(1) FROM TAB_TITULO T
					/*INNER JOIN TAB_REQUERIMENTO_BARRAGEM B ON B.REQUERIMENTO = T.REQUERIMENTO*/
					WHERE T.ID = :titulo AND T.SITUACAO = 8
				");

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool VerificarElaboracaoRT(int projetoDigital, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select count(1) from tab_projeto_digital pd
						inner join tab_requerimento_barragem rb on rb.requerimento = pd.requerimento
					where pd.id = :projetoDigital and rb.rt_elaboracao in (1,3)");

				comando.AdicionarParametroEntrada("projetoDigital", projetoDigital, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool PossuiTituloBarragemValido(int id, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select count(1) from tab_titulo t 
						inner join idafcredenciado.tab_requerimento r on r.id = t.requerimento
						inner join tab_requerimento ri on ri.numero = r.id
						inner join tab_requerimento_barragem rb on rb.requerimento = ri.id
						inner join crt_barragem_dispensa_lic b on b.id = rb.barragem 
					where t.modelo = 72 and situacao = 8 and b.id = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}
		#endregion
	}
}