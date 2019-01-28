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

				Comando comando = bancoDeDados.CriarComando(@"
				insert into crt_barragem_dispensa_lic (id, tid, empreendimento, atividade, tipo_barragem, finalidade_atividade, curso_hidrico, vazao_enchente, area_bacia_contribuicao, 
				precipitacao, periodo_retorno, coeficiente_escoamento, tempo_concentracao, equacao_calculo, area_alagada, volume_armazenado, fase, possui_monge, tipo_monge, especificacao_monge, 
				possui_vertedouro, tipo_vertedouro, especificacao_vertedouro, possui_estrutura_hidrau, adequacoes_realizada, data_inicio_obra, data_previsao_obra, easting, northing, formacao_resp_tec, 
				especificacao_rt, autorizacao, numero_art_elaboracao, numero_art_execucao, interno_id, interno_tid)
				values (seq_crt_barragem_dispensa_lic.nextval, :tid, :empreendimento, :atividade, :tipo_barragem, :finalidade_atividade, :curso_hidrico, :vazao_enchente, :area_bacia_contribuicao, 
				:precipitacao, :periodo_retorno, :coeficiente_escoamento, :tempo_concentracao, :equacao_calculo, :area_alagada, :volume_armazenado, :fase, :possui_monge, :tipo_monge, :especificacao_monge, 
				:possui_vertedouro, :tipo_vertedouro, :especificacao_vertedouro, :possui_estrutura_hidrau, :adequacoes_realizada, :data_inicio_obra, :data_previsao_obra, :easting, :northing, :formacao_resp_tec, 
				:especificacao_rt, :autorizacao, :numero_art_elaboracao, :numero_art_execucao, :interno_id, :interno_tid) returning id into :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.AtividadeID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_barragem", caracterizacao.BarragemTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("finalidade_atividade", caracterizacao.FinalidadeAtividade, DbType.Int32);
				comando.AdicionarParametroEntrada("curso_hidrico", caracterizacao.CursoHidrico, DbType.String);
				comando.AdicionarParametroEntrada("vazao_enchente", caracterizacao.VazaoEnchente, DbType.Decimal);
				comando.AdicionarParametroEntrada("area_bacia_contribuicao", caracterizacao.AreaBaciaContribuicao, DbType.Decimal);
				comando.AdicionarParametroEntrada("precipitacao", caracterizacao.Precipitacao, DbType.Decimal);
				comando.AdicionarParametroEntrada("periodo_retorno", caracterizacao.PeriodoRetorno, DbType.Int32);
				comando.AdicionarParametroEntrada("coeficiente_escoamento", caracterizacao.CoeficienteEscoamento, DbType.String);
				comando.AdicionarParametroEntrada("tempo_concentracao", caracterizacao.TempoConcentracao, DbType.String);
				comando.AdicionarParametroEntrada("equacao_calculo", caracterizacao.EquacaoCalculo, DbType.String);
				comando.AdicionarParametroEntrada("area_alagada", caracterizacao.AreaAlagada, DbType.Decimal);
				comando.AdicionarParametroEntrada("volume_armazenado", caracterizacao.VolumeArmazanado, DbType.Decimal);
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
				comando.AdicionarParametroEntrada("finalidade_atividade", caracterizacao.FinalidadeAtividade, DbType.Int32);
				comando.AdicionarParametroEntrada("curso_hidrico", caracterizacao.CursoHidrico, DbType.String);
				comando.AdicionarParametroEntrada("vazao_enchente", caracterizacao.VazaoEnchente, DbType.Decimal);
				comando.AdicionarParametroEntrada("area_bacia_contribuicao", caracterizacao.AreaBaciaContribuicao, DbType.Decimal);
				comando.AdicionarParametroEntrada("precipitacao", caracterizacao.Precipitacao, DbType.Decimal);
				comando.AdicionarParametroEntrada("periodo_retorno", caracterizacao.PeriodoRetorno, DbType.Int32);
				comando.AdicionarParametroEntrada("coeficiente_escoamento", caracterizacao.CoeficienteEscoamento, DbType.String);
				comando.AdicionarParametroEntrada("tempo_concentracao", caracterizacao.TempoConcentracao, DbType.String);
				comando.AdicionarParametroEntrada("equacao_calculo", caracterizacao.EquacaoCalculo, DbType.String);
				comando.AdicionarParametroEntrada("area_alagada", caracterizacao.AreaAlagada, DbType.Decimal);
				comando.AdicionarParametroEntrada("volume_armazenado", caracterizacao.VolumeArmazanado, DbType.Decimal);
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
                comando.AdicionarParametroEntrada("finalidade_atividade", caracterizacao.FinalidadeAtividade, DbType.Int32);
                comando.AdicionarParametroEntrada("curso_hidrico", caracterizacao.CursoHidrico, DbType.String);
                comando.AdicionarParametroEntrada("vazao_enchente", caracterizacao.VazaoEnchente, DbType.Decimal);
                comando.AdicionarParametroEntrada("area_bacia_contribuicao", caracterizacao.AreaBaciaContribuicao, DbType.Decimal);
                comando.AdicionarParametroEntrada("precipitacao", caracterizacao.Precipitacao, DbType.Decimal);
                comando.AdicionarParametroEntrada("periodo_retorno", caracterizacao.PeriodoRetorno, DbType.Int32);
                comando.AdicionarParametroEntrada("coeficiente_escoamento", caracterizacao.CoeficienteEscoamento, DbType.String);
                comando.AdicionarParametroEntrada("tempo_concentracao", caracterizacao.TempoConcentracao, DbType.String);
                comando.AdicionarParametroEntrada("equacao_calculo", caracterizacao.EquacaoCalculo, DbType.String);
                comando.AdicionarParametroEntrada("area_alagada", caracterizacao.AreaAlagada, DbType.Decimal);
                comando.AdicionarParametroEntrada("volume_armazenado", caracterizacao.VolumeArmazanado, DbType.Decimal);
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
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.tid, c.interno_id, c.interno_tid, c.empreendimento, c.atividade, c.tipo_barragem, 
                lt.texto tipo_barragem_texto, c.finalidade_atividade, c.curso_hidrico, c.vazao_enchente, c.area_bacia_contribuicao, c.precipitacao, 
                c.periodo_retorno, c.coeficiente_escoamento, c.tempo_concentracao, c.equacao_calculo, c.area_alagada, c.volume_armazenado, 
				c.fase, c.possui_monge, c.tipo_monge, c.especificacao_monge, c.possui_vertedouro, c.tipo_vertedouro, c.especificacao_vertedouro, 
                c.possui_estrutura_hidrau, c.adequacoes_realizada, c.data_inicio_obra, c.data_previsao_obra, c.easting, c.northing, c.formacao_resp_tec, 
                c.especificacao_rt, c.autorizacao, c.numero_art_elaboracao, c.numero_art_execucao, lf.texto
                from crt_barragem_dispensa_lic c, lov_crt_bdla_barragem_tipo lt, LOV_CRT_BDLA_FINALIDADE_ATV lf
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
					and c.EMPREENDIMENTO = :empreendimentoId and lt.id = c.TIPO_BARRAGEM and lf.id = c.finalidade_atividade", EsquemaCredenciadoBanco);

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
						caracterizacao.AtividadeID = reader.GetValue<int>("atividade");
						caracterizacao.BarragemTipo = reader.GetValue<int>("tipo_barragem");
						caracterizacao.BarragemTipoTexto = reader.GetValue<string>("tipo_barragem_texto");
						caracterizacao.FinalidadeAtividade = reader.GetValue<int>("finalidade_atividade");
						caracterizacao.FinalidadeTexto = reader.GetValue<string>("texto");
						caracterizacao.CursoHidrico = reader.GetValue<string>("curso_hidrico");
						caracterizacao.VazaoEnchente = reader.GetValue<decimal?>("vazao_enchente");
						caracterizacao.AreaBaciaContribuicao = reader.GetValue<decimal?>("area_bacia_contribuicao");
						caracterizacao.Precipitacao = reader.GetValue<decimal?>("precipitacao");
						caracterizacao.PeriodoRetorno = reader.GetValue<int?>("periodo_retorno");
						caracterizacao.CoeficienteEscoamento = reader.GetValue<string>("coeficiente_escoamento");
						caracterizacao.TempoConcentracao = reader.GetValue<string>("tempo_concentracao");
						caracterizacao.EquacaoCalculo = reader.GetValue<string>("equacao_calculo");
						caracterizacao.AreaAlagada = reader.GetValue<decimal?>("area_alagada");
						caracterizacao.VolumeArmazanado = reader.GetValue<decimal?>("volume_armazenado");
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
                Comando comando = bancoDeDados.CriarComando(@"select c.id, c.tid, c.interno_id, c.interno_tid, c.empreendimento, c.atividade, c.tipo_barragem, 
                lt.texto tipo_barragem_texto, c.finalidade_atividade, c.curso_hidrico, c.vazao_enchente, c.area_bacia_contribuicao, c.precipitacao, 
                c.periodo_retorno, c.coeficiente_escoamento, c.tempo_concentracao, c.equacao_calculo, c.area_alagada, c.volume_armazenado, 
				c.fase, c.possui_monge, c.tipo_monge, c.especificacao_monge, c.possui_vertedouro, c.tipo_vertedouro, c.especificacao_vertedouro, 
                c.possui_estrutura_hidrau, c.adequacoes_realizada, c.data_inicio_obra, c.data_previsao_obra, c.easting, c.northing, c.formacao_resp_tec, 
                c.especificacao_rt, c.autorizacao, c.numero_art_elaboracao, c.numero_art_execucao, lf.texto 
                from crt_barragem_dispensa_lic c, lov_crt_bdla_barragem_tipo lt, lov_crt_bdla_finalidade_atv lf where lt.id = c.tipo_barragem and c.id = :id and lf.id = c.finalidade_atividade", EsquemaCredenciadoBanco);

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
						caracterizacao.BarragemTipo = reader.GetValue<int>("tipo_barragem");
						caracterizacao.BarragemTipoTexto = reader.GetValue<string>("tipo_barragem_texto");
						caracterizacao.FinalidadeAtividade = reader.GetValue<int>("finalidade_atividade");
						caracterizacao.FinalidadeTexto = reader.GetValue<string>("texto");
						caracterizacao.CursoHidrico = reader.GetValue<string>("curso_hidrico");
						caracterizacao.VazaoEnchente = reader.GetValue<decimal?>("vazao_enchente");
						caracterizacao.AreaBaciaContribuicao = reader.GetValue<decimal?>("area_bacia_contribuicao");
						caracterizacao.Precipitacao = reader.GetValue<decimal?>("precipitacao");
						caracterizacao.PeriodoRetorno = reader.GetValue<int?>("periodo_retorno");
						caracterizacao.CoeficienteEscoamento = reader.GetValue<string>("coeficiente_escoamento");
						caracterizacao.TempoConcentracao = reader.GetValue<string>("tempo_concentracao");
						caracterizacao.EquacaoCalculo = reader.GetValue<string>("equacao_calculo");
						caracterizacao.AreaAlagada = reader.GetValue<decimal?>("area_alagada");
						caracterizacao.VolumeArmazanado = reader.GetValue<decimal?>("volume_armazenado");
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

				comando = bancoDeDados.CriarComando(@"select c.id, c.tid, c.interno_id, c.interno_tid, c.empreendimento, c.atividade, c.tipo_barragem, 
                lt.texto tipo_barragem_texto, c.finalidade_atividade, c.curso_hidrico, c.vazao_enchente, c.area_bacia_contribuicao, c.precipitacao, 
                c.periodo_retorno, c.coeficiente_escoamento, c.tempo_concentracao, c.equacao_calculo, c.area_alagada, c.volume_armazenado, 
				c.fase, c.possui_monge, c.tipo_monge, c.especificacao_monge, c.possui_vertedouro, c.tipo_vertedouro, c.especificacao_vertedouro, 
                c.possui_estrutura_hidrau, c.adequacoes_realizada, c.data_inicio_obra, c.data_previsao_obra, c.easting, c.northing, c.formacao_resp_tec, 
                c.especificacao_rt, c.autorizacao, c.numero_art_elaboracao, c.numero_art_execucao, lf.texto,
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
                from crt_barragem_dispensa_lic c, lov_crt_bdla_barragem_tipo lt, LOV_CRT_BDLA_FINALIDADE_ATV lf where lt.id = c.tipo_barragem and c.id = :retornoComando and lf.id = c.finalidade_atividade", EsquemaCredenciadoBanco);

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
						caracterizacao.AtividadeID = reader.GetValue<int>("atividade");
						caracterizacao.BarragemTipo = reader.GetValue<int>("tipo_barragem");
						caracterizacao.BarragemTipoTexto = reader.GetValue<string>("tipo_barragem_texto");

						caracterizacao.FinalidadeAtividade = reader.GetValue<int>("finalidade_atividade");
						caracterizacao.FinalidadeTexto = reader.GetValue<string>("texto");
						caracterizacao.BarragemTipoTexto = reader.GetValue<string>("tipo_barragem_texto");
						caracterizacao.CursoHidrico = reader.GetValue<string>("curso_hidrico");
						caracterizacao.VazaoEnchente = reader.GetValue<decimal?>("vazao_enchente");
						caracterizacao.AreaBaciaContribuicao = reader.GetValue<decimal?>("area_bacia_contribuicao");
						caracterizacao.Precipitacao = reader.GetValue<decimal?>("precipitacao");
						caracterizacao.PeriodoRetorno = reader.GetValue<int?>("periodo_retorno");
						caracterizacao.CoeficienteEscoamento = reader.GetValue<string>("coeficiente_escoamento");
						caracterizacao.TempoConcentracao = reader.GetValue<string>("tempo_concentracao");
						caracterizacao.EquacaoCalculo = reader.GetValue<string>("equacao_calculo");
						caracterizacao.AreaAlagada = reader.GetValue<decimal?>("area_alagada");
						caracterizacao.VolumeArmazanado = reader.GetValue<decimal?>("volume_armazenado");
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
						caracterizacao.PossuiAssociacaoExterna = reader.GetValue<int>("possui_associacao_externa") > 1;

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
                        caracterizacao.BarragemTipo = reader.GetValue<int>("tipo_barragem_id");
                        caracterizacao.BarragemTipoTexto = reader.GetValue<string>("tipo_barragem_texto");
                        caracterizacao.FinalidadeAtividade = reader.GetValue<int>("finalidade_atividade");
                        caracterizacao.CursoHidrico = reader.GetValue<string>("curso_hidrico");
                        caracterizacao.VazaoEnchente = reader.GetValue<decimal?>("vazao_enchente");
                        caracterizacao.AreaBaciaContribuicao = reader.GetValue<decimal?>("area_bacia_contribuicao");
                        caracterizacao.Precipitacao = reader.GetValue<decimal?>("precipitacao");
                        caracterizacao.PeriodoRetorno = reader.GetValue<int?>("periodo_retorno");
                        caracterizacao.CoeficienteEscoamento = reader.GetValue<string>("coeficiente_escoamento");
                        caracterizacao.TempoConcentracao = reader.GetValue<string>("tempo_concentracao");
                        caracterizacao.EquacaoCalculo = reader.GetValue<string>("equacao_calculo");
                        caracterizacao.AreaAlagada = reader.GetValue<decimal?>("area_alagada");
                        caracterizacao.VolumeArmazanado = reader.GetValue<decimal?>("volume_armazenado");
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

		#endregion
	}
}