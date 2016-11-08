using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Entities;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data
{
	internal class ImportarTituloDeclaratorioDa
	{
		#region Propriedades

		private ConfiguracaoSistema _configSis = new ConfiguracaoSistema();

		Historico _historico = new Historico();
		HistoricoCaracterizacaoDa _historicoCaracterizacao = new HistoricoCaracterizacaoDa();

		public Historico Historico { get { return _historico; } }

		public HistoricoCaracterizacaoDa HistoricoCaracterizacao { get { return _historicoCaracterizacao; } }

		public String EsquemaBanco { get; set; }

		public string UsuarioCredenciado
		{
			get { return _configSis.UsuarioCredenciado; }
		}

		public string UsuarioInterno
		{
			get { return _configSis.UsuarioInterno; }
		}

		public ImportarTituloDeclaratorioDa(string esquema = null)
		{
			EsquemaBanco = esquema;
		}

		public string TID { get; set; }

		#endregion Propriedades

		#region Configurações

		internal void EditarConfiguracao(ConfiguracaoServico configuracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando;

				if (configuracao.EmExecucao)
				{
					comando = bancoDeDados.CriarComando(@"update cnf_servicos r set r.data_inicio_execucao = :data_inicio_execucao, r.em_execucao = :em_execucao, r.tid = :tid where r.id = :id");

					configuracao.DataInicioExecucao = configuracao.DataInicioExecucao.Value.AddHours(configuracao.Intervalo.TotalHours);
					comando.AdicionarParametroEntrada("data_inicio_execucao", configuracao.DataInicioExecucao, DbType.DateTime);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update cnf_servicos r set r.data_ultima_execucao = sysdate, r.em_execucao = :em_execucao, r.tid = :tid where r.id = :id");
				}

				comando.AdicionarParametroEntrada("id", configuracao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("em_execucao", configuracao.EmExecucao ? 1 : 0, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, this.TID);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal ConfiguracaoServico Configuracao(eServico servico)
		{
			return Configuracoes(servico).FirstOrDefault() ?? new ConfiguracaoServico();
		}

		internal List<ConfiguracaoServico> Configuracoes(eServico servico)
		{
			List<ConfiguracaoServico> retorno = new List<ConfiguracaoServico>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"select id, servico, intervalo, data_inicio, data_inicio_execucao, 
                    nvl(data_ultima_execucao, data_inicio) data_ultima_execucao, em_execucao, tid from cnf_servicos t");

				comando.DbCommand.CommandText += " where t.id = :servico";
				comando.AdicionarParametroEntrada("servico", (int)servico, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ConfiguracaoServico conf;

					while (reader.Read())
					{
						conf = new ConfiguracaoServico();
						conf.Id = reader.GetValue<Int32>("id");
						conf.Servico = reader.GetValue<String>("servico");
						conf.Intervalo = new TimeSpan(reader.GetValue<Int32>("intervalo"), 0, 0); //Em Horas
						conf.DataInicio = reader.GetValue<DateTime>("data_inicio");
						conf.DataInicioExecucao = reader.GetValue<DateTime>("data_inicio_execucao");
						conf.DataUltimaExecucao = reader.GetValue<DateTime>("data_ultima_execucao");
						conf.EmExecucao = reader.GetValue<Boolean>("em_execucao");
						retorno.Add(conf);
					}

					reader.Close();
				}

				return retorno;
			}
		}

		#endregion Configurações

		internal List<Requerimento> ObterRequerimentosCredenciado()
		{
			List<Requerimento> eleitos = new List<Requerimento>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
                select  tt.requerimento id, 
                        tr.setor
                from    tab_titulo          tt,
                        tab_titulo_modelo   ttm,
                        cre_requerimento    tr,
                        tab_projeto_digital tpd
                where tt.modelo = ttm.id
                and tt.requerimento = tr.id
                and tr.id = tpd.requerimento
                and tpd.situacao = 2
                and ttm.documento = 2
                and tt.situacao = 8
                and not exists (select 1 from tab_requerimento r where r.id = tr.id)");

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Requerimento req;

					while (reader.Read())
					{
						req = new Requerimento();
						req.Id = reader.GetValue<Int32>("id");
						req.SetorId = reader.GetValue<Int32>("setor");
						eleitos.Add(req);
					}

					reader.Close();
				}

				return eleitos;
			}
		}

		internal BarragemDispensaLicenca ImportarCaracterizacaoCdla(int requerimentoId, BancoDeDados bancoCredenciado)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoCredenciado, UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"select crtb.*, te.codigo empreendimento_codigo 
                from crt_barragem_dispensa_lic crtb, tab_requerimento tr, tab_empreendimento te 
                where crtb.empreendimento = tr.empreendimento and te.id(+) = tr.empreendimento and tr.id = :requerimento");

				comando.AdicionarParametroEntrada("requerimento", requerimentoId, DbType.Int32);

				BarragemDispensaLicenca caract = new BarragemDispensaLicenca();

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						caract.Id = reader.GetValue<Int32>("id");
						caract.Tid = reader.GetValue<String>("tid");
						caract.EmpreendimentoID = reader.GetValue<Int32>("empreendimento");
						caract.EmpreendimentoCodigo = reader.GetValue<Int32>("empreendimento_codigo");
						caract.AtividadeID = reader.GetValue<Int32>("atividade");
						caract.BarragemTipo = reader.GetValue<Int32>("tipo_barragem");
						caract.FinalidadeAtividade = reader.GetValue<Int32>("finalidade_atividade");
						caract.CursoHidrico = reader.GetValue<String>("curso_hidrico");
						caract.VazaoEnchente = reader.GetValue<Decimal>("vazao_enchente");
						caract.AreaBaciaContribuicao = reader.GetValue<Decimal>("area_bacia_contribuicao");
						caract.Precipitacao = reader.GetValue<Decimal>("precipitacao");
						caract.PeriodoRetorno = reader.GetValue<Int32>("periodo_retorno");
						caract.CoeficienteEscoamento = reader.GetValue<String>("coeficiente_escoamento");
						caract.TempoConcentracao = reader.GetValue<String>("tempo_concentracao");
						caract.EquacaoCalculo = reader.GetValue<String>("equacao_calculo");
						caract.AreaAlagada = reader.GetValue<Decimal>("area_alagada");
						caract.VolumeArmazanado = reader.GetValue<Decimal>("volume_armazenado");
						caract.Fase = reader.GetValue<Int32>("fase");
						caract.PossuiMonge = reader.GetValue<Int32>("possui_monge");
						caract.MongeTipo = reader.GetValue<Int32>("tipo_monge");
						caract.EspecificacaoMonge = reader.GetValue<String>("especificacao_monge");
						caract.PossuiVertedouro = reader.GetValue<Int32>("possui_vertedouro");
						caract.VertedouroTipo = reader.GetValue<Int32>("tipo_vertedouro");
						caract.EspecificacaoVertedouro = reader.GetValue<String>("especificacao_vertedouro");
						caract.PossuiEstruturaHidraulica = reader.GetValue<Int32>("possui_estrutura_hidrau");
						caract.AdequacoesRealizada = reader.GetValue<String>("adequacoes_realizada");
						caract.DataInicioObra = reader.GetValue<String>("data_inicio_obra");
						caract.DataPrevisaoTerminoObra = reader.GetValue<String>("data_previsao_obra");
						caract.Coordenada.EastingUtmTexto = reader.GetValue<String>("easting");

						caract.Autorizacao.Id = reader.GetValue<Int32>("autorizacao");
						caract.Coordenada.NorthingUtmTexto = reader.GetValue<String>("northing");
						caract.FormacaoRT = reader.GetValue<Int32>("formacao_resp_tec");
						caract.EspecificacaoRT = reader.GetValue<String>("especificacao_rt");
						caract.NumeroARTElaboracao = reader.GetValue<String>("numero_art_elaboracao");
						caract.NumeroARTExecucao = reader.GetValue<String>("numero_art_execucao");
						caract.InternoID = reader.GetValue<Int32>("interno_id");
						caract.InternoTID = reader.GetValue<String>("interno_tid");
					}
					reader.Close();
				}
				return caract;
			}
		}

		internal int ObterCaracterizacaoId(int empreedimentoCodigo, BancoDeDados bancoInterno)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, UsuarioInterno))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"select nvl(crt.id, 0) from tab_empreendimento te, crt_barragem_dispensa_lic crt where te.id = crt.empreendimento and te.codigo = :emp_codigo");

				comando.AdicionarParametroEntrada("emp_codigo", empreedimentoCodigo, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal void CopiarDadosCredenciado(BarragemDispensaLicenca caracterizacao, BancoDeDados bancoInterno)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(bancoInterno, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando;

				caracterizacao.InternoID = ObterCaracterizacaoId(caracterizacao.EmpreendimentoCodigo, bancoInterno);

				if (caracterizacao.InternoID <= 0)
				{
					comando = bancoDeDados.CriarComando(@"
				    insert into crt_barragem_dispensa_lic (id, tid, empreendimento, atividade, tipo_barragem, finalidade_atividade, curso_hidrico, vazao_enchente, area_bacia_contribuicao, 
				    precipitacao, periodo_retorno, coeficiente_escoamento, tempo_concentracao, equacao_calculo, area_alagada, volume_armazenado, fase, possui_monge, tipo_monge, especificacao_monge, 
				    possui_vertedouro, tipo_vertedouro, especificacao_vertedouro, possui_estrutura_hidrau, adequacoes_realizada, data_inicio_obra, data_previsao_obra, easting, northing, formacao_resp_tec, 
				    especificacao_rt, numero_art_elaboracao, numero_art_execucao, autorizacao)
				    values (seq_crt_barragem_dispensa_lic.nextval, :tid, (select te.id from tab_empreendimento te where te.codigo = :emp_codigo), :atividade, :tipo_barragem, :finalidade_atividade, :curso_hidrico, :vazao_enchente, :area_bacia_contribuicao, 
				    :precipitacao, :periodo_retorno, :coeficiente_escoamento, :tempo_concentracao, :equacao_calculo, :area_alagada, :volume_armazenado, :fase, :possui_monge, :tipo_monge, :especificacao_monge, 
				    :possui_vertedouro, :tipo_vertedouro, :especificacao_vertedouro, :possui_estrutura_hidrau, :adequacoes_realizada, :data_inicio_obra, :data_previsao_obra, :easting, :northing, :formacao_resp_tec, 
				    :especificacao_rt, :numero_art_elaboracao, :numero_art_execucao, :autorizacao) returning id into :id", EsquemaBanco);

					comando.AdicionarParametroSaida("id", DbType.Int32);
					comando.AdicionarParametroEntrada("emp_codigo", caracterizacao.EmpreendimentoCodigo, DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update crt_barragem_dispensa_lic c set c.tid = :tid, c.atividade = :atividade, c.tipo_barragem = :tipo_barragem, c.autorizacao = :autorizacao,
				    c.finalidade_atividade = :finalidade_atividade, c.curso_hidrico = :curso_hidrico, c.vazao_enchente = :vazao_enchente, c.area_bacia_contribuicao = :area_bacia_contribuicao, c.precipitacao = :precipitacao, 
				    c.periodo_retorno = :periodo_retorno, c.coeficiente_escoamento = :coeficiente_escoamento, c.tempo_concentracao = :tempo_concentracao, c.equacao_calculo = :equacao_calculo, c.area_alagada = :area_alagada, 
				    c.volume_armazenado = :volume_armazenado, c.fase = :fase, c.possui_monge = :possui_monge, c.tipo_monge = :tipo_monge, c.especificacao_monge = :especificacao_monge, c.possui_vertedouro = :possui_vertedouro, 
				    c.tipo_vertedouro = :tipo_vertedouro, c.especificacao_vertedouro = :especificacao_vertedouro, c.possui_estrutura_hidrau = :possui_estrutura_hidrau, c.adequacoes_realizada = :adequacoes_realizada, 
				    c.data_inicio_obra = :data_inicio_obra, c.data_previsao_obra = :data_previsao_obra, c.easting = :easting, c.northing = :northing, c.formacao_resp_tec = :formacao_resp_tec, c.especificacao_rt = :especificacao_rt, 
				    c.numero_art_elaboracao = :numero_art_elaboracao, c.numero_art_execucao = :numero_art_execucao
                    where c.id = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", caracterizacao.InternoID, DbType.Int32);
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
				comando.AdicionarParametroEntrada("possui_monge", caracterizacao.PossuiMonge, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_monge", caracterizacao.MongeTipo > 0 ? caracterizacao.MongeTipo : (Object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("especificacao_monge", caracterizacao.EspecificacaoMonge, DbType.String);
				comando.AdicionarParametroEntrada("possui_vertedouro", caracterizacao.PossuiVertedouro, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_vertedouro", caracterizacao.VertedouroTipo > 0 ? caracterizacao.VertedouroTipo : (Object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("especificacao_vertedouro", caracterizacao.EspecificacaoVertedouro, DbType.String);
				comando.AdicionarParametroEntrada("possui_estrutura_hidrau", caracterizacao.PossuiEstruturaHidraulica, DbType.Int32);
				comando.AdicionarParametroEntrada("adequacoes_realizada", caracterizacao.AdequacoesRealizada, DbType.String);
				comando.AdicionarParametroEntrada("data_inicio_obra", caracterizacao.DataInicioObra, DbType.String);
				comando.AdicionarParametroEntrada("data_previsao_obra", caracterizacao.DataPrevisaoTerminoObra, DbType.String);
				comando.AdicionarParametroEntrada("easting", caracterizacao.Coordenada.EastingUtmTexto, DbType.Int64);
				comando.AdicionarParametroEntrada("northing", caracterizacao.Coordenada.NorthingUtmTexto, DbType.Int64);
				comando.AdicionarParametroEntrada("formacao_resp_tec", caracterizacao.FormacaoRT, DbType.Int32);
				comando.AdicionarParametroEntrada("autorizacao", caracterizacao.Autorizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("especificacao_rt", caracterizacao.EspecificacaoRT, DbType.String);
				comando.AdicionarParametroEntrada("numero_art_elaboracao", caracterizacao.NumeroARTElaboracao, DbType.String);
				comando.AdicionarParametroEntrada("numero_art_execucao", caracterizacao.NumeroARTExecucao, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (caracterizacao.InternoID <= 0)
				{
					caracterizacao.InternoID = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				HistoricoCaracterizacao.Gerar(caracterizacao.InternoID, eHistoricoArtefatoCaracterizacao.barragemdispensalicenca, eHistoricoAcao.importar, null);

				bancoDeDados.Commit();
			}
		}

		public Dictionary<Int32, String> ObterCredenciadoDiretorioArquivoTemp(BancoDeDados bancoCredenciado)
		{
			Dictionary<Int32, String> lstDiretorios = new Dictionary<int, string>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.raiz from cnf_arquivo c where c.ativo = 1 and tipo = 1", bancoCredenciado);
			foreach (var item in daReader)
			{
				lstDiretorios.Add(Convert.ToInt32(item["id"]), item["raiz"].ToString());
			}

			return lstDiretorios;
		}

		public Dictionary<Int32, String> ObterCredenciadoDiretorioArquivo(BancoDeDados bancoCredenciado)
		{
			Dictionary<Int32, String> lstDiretorios = new Dictionary<int, string>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.raiz from cnf_arquivo c where c.ativo = 1 and tipo = 2", bancoCredenciado);
			foreach (var item in daReader)
			{
				lstDiretorios.Add(Convert.ToInt32(item["id"]), item["raiz"].ToString());
			}

			return lstDiretorios;
		}
	}
}