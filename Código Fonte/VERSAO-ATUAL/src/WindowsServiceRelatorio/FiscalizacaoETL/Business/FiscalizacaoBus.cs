using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.FiscalizacaoETL.Data;
using Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Data;
using Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Entities;

namespace Tecnomapas.EtramiteX.WindowsService.FiscalizacaoETL.Business
{
	public class FiscalizacaoBus
	{
		RelatorioDa _daRelatorio;
		ConfiguracaoSistema _configSys;
		FiscalizacaoDa _da;

		public FiscalizacaoBus()
		{
			_configSys = new ConfiguracaoSistema();
			_da = new FiscalizacaoDa();
			_daRelatorio = new RelatorioDa();
		}

		private void GerarColunasDinamicas()
		{
			#region Colunas Perguntas
			List<Dictionary<string, object>> colunasIds = _da.ObterPerguntaIds();

			if (colunasIds != null && colunasIds.Count > 0)
			{
				SqlBuilderDa sqlBuilder = new SqlBuilderDa("dim_fisc_pergunta", "seq_dim_fisc_pergunta");
				var lstColunas = sqlBuilder.ObterColunas();

				colunasIds.Where(x => lstColunas.Exists(c => c.ColumnName == string.Format("P_TXT_{0}", x["PERGUNTA_ID"]))).ToList()
				.ForEach(x =>
				{
					RelatorioCampo campoRelatorio = new RelatorioCampo();
					campoRelatorio.TabelaFato = "fat_fiscalizacao";
					campoRelatorio.TabelaDimensao = "dim_fisc_pergunta";
					campoRelatorio.Alias = x["PERGUNTA_TEXTO"].ToString();
					campoRelatorio.Nome = string.Format("P_RES_TXT_{0}", x["PERGUNTA_ID"]);
					campoRelatorio.TipoDado = eCampoTipoDado.String;

					_daRelatorio.SalvarCampo(campoRelatorio);
				});

				colunasIds.Where(x => !lstColunas.Exists(c => c.ColumnName == string.Format("P_TXT_{0}", x["PERGUNTA_ID"]))).ToList()
				.ForEach(x =>
				{
					sqlBuilder.AddColuna("dim_fisc_pergunta", string.Format("P_TXT_{0}", x["PERGUNTA_ID"]), "VARCHAR2(100)");
					sqlBuilder.AddColuna("dim_fisc_pergunta", string.Format("P_TID_{0}", x["PERGUNTA_ID"]), "VARCHAR2(36)");
					sqlBuilder.AddColuna("dim_fisc_pergunta", string.Format("P_RES_ID_{0}", x["PERGUNTA_ID"]), "NUMBER(38)");
					sqlBuilder.AddColuna("dim_fisc_pergunta", string.Format("P_RES_TXT_{0}", x["PERGUNTA_ID"]), "VARCHAR2(100)");
					sqlBuilder.AddColuna("dim_fisc_pergunta", string.Format("P_RES_TID_{0}", x["PERGUNTA_ID"]), "VARCHAR2(36)");

					sqlBuilder.AddIndiceTxt("dim_fisc_pergunta", string.Format("P_TXT_{0}", x["PERGUNTA_ID"]), string.Format("idx_fisc_p{0}", x["PERGUNTA_ID"]));
					sqlBuilder.AddIndiceTxt("dim_fisc_pergunta", string.Format("P_RES_TXT_{0}", x["PERGUNTA_ID"]), string.Format("idx_fisc_pr{0}", x["PERGUNTA_ID"]));

					RelatorioCampo campoRelatorio = new RelatorioCampo();
					campoRelatorio.TabelaFato = "fat_fiscalizacao";
					campoRelatorio.TabelaDimensao = "dim_fisc_pergunta";
					campoRelatorio.Alias = x["PERGUNTA_TEXTO"].ToString();
					campoRelatorio.Nome = string.Format("P_RES_TXT_{0}", x["PERGUNTA_ID"]);
					campoRelatorio.TipoDado = eCampoTipoDado.String;

					campoRelatorio.Exibicao = true;
					campoRelatorio.Filtro = true;
					campoRelatorio.Ordenacao = true;

					campoRelatorio.ConsultaSistema = eSistemaConsulta.Relatorio;
					campoRelatorio.ConsultaSql = string.Format(" ( select distinct p.p_res_id_{0} id, p.P_RES_TXT_{0} texto from dim_fisc_pergunta p where p.P_RES_TXT_{0} is not null ) ", x["PERGUNTA_ID"]);
					campoRelatorio.ConsultaCampo = string.Format("P_RES_ID_{0}", x["PERGUNTA_ID"]);

					_daRelatorio.SalvarCampo(campoRelatorio);

				});
			}
			#endregion

			#region Colunas Campos
			colunasIds = _da.ObterFiscCamposIds();

			if (colunasIds != null && colunasIds.Count > 0)
			{
				SqlBuilderDa sqlBuilder = new SqlBuilderDa("dim_fisc_campo", "seq_dim_fisc_campo");
				var lstColunas = sqlBuilder.ObterColunas();

				colunasIds.Where(x => lstColunas.Exists(c => c.ColumnName == string.Format("C_VAL_{0}", x["INFRACAO_CAMPO_ID"]))).ToList()
				.ForEach(x =>
				{
					RelatorioCampo campoRelatorio = new RelatorioCampo();
					campoRelatorio.TabelaFato = "fat_fiscalizacao";
					campoRelatorio.TabelaDimensao = "dim_fisc_campo";
					campoRelatorio.Alias = string.Format("{0}{1}", x["TEXTO"], (String.IsNullOrEmpty(x["UNIDADE_TEXTO"].ToString()) ? String.Empty : " (" + x["UNIDADE_TEXTO"] + ")"));
					campoRelatorio.Nome = string.Format("C_VAL_{0}", x["INFRACAO_CAMPO_ID"]);
					campoRelatorio.TipoDado = ((Convert.ToInt32(x["TIPO"]) == 1) ? eCampoTipoDado.String : eCampoTipoDado.Real);

					_daRelatorio.SalvarCampo(campoRelatorio);
				});

				colunasIds.Where(x => !lstColunas.Exists(c => c.ColumnName == string.Format("C_VAL_{0}", x["INFRACAO_CAMPO_ID"]))).ToList()
				.ForEach(x =>
				{
					sqlBuilder.AddColuna("dim_fisc_campo", string.Format("C_VAL_{0}", x["INFRACAO_CAMPO_ID"]), ((Convert.ToInt32(x["TIPO"]) == 1) ? "VARCHAR2(100)" : "NUMBER"));

					if (Convert.ToInt32(x["TIPO"]) == 1)
					{
						sqlBuilder.AddIndiceTxt("dim_fisc_campo", string.Format("C_VAL_{0}", x["INFRACAO_CAMPO_ID"]), string.Format("idx_fisc_c{0}", x["INFRACAO_CAMPO_ID"]));
					}

					RelatorioCampo campoRelatorio = new RelatorioCampo();
					campoRelatorio.TabelaFato = "fat_fiscalizacao";
					campoRelatorio.TabelaDimensao = "dim_fisc_campo";
					campoRelatorio.Alias = string.Format("{0}{1}", x["TEXTO"], (String.IsNullOrEmpty(x["UNIDADE_TEXTO"].ToString()) ? String.Empty : " (" + x["UNIDADE_TEXTO"] + ")"));
					campoRelatorio.Nome = string.Format("C_VAL_{0}", x["INFRACAO_CAMPO_ID"]);
					campoRelatorio.TipoDado = ((Convert.ToInt32(x["TIPO"]) == 1) ? eCampoTipoDado.String : eCampoTipoDado.Real);

					campoRelatorio.Exibicao = true;
					campoRelatorio.Filtro = true;
					campoRelatorio.Ordenacao = true;

					_daRelatorio.SalvarCampo(campoRelatorio);

				});
			}
			#endregion
		}

		private void Executar(ConfiguracaoRelatorio config)
		{
			#region Obter Eleitos
			List<Dictionary<string, object>> eleitos;
			List<Dictionary<string, object>> aux;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				eleitos = _da.Eleitos(config.DadosAte, bancoDeDados);
			}

			if (eleitos == null || eleitos.Count == 0)
			{
				return;
			}
			#endregion

			#region Atualizar Fato / Dimensao
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(_configSys.UsuarioRelatorio))
			{
				try
				{
					bancoDeDados.IniciarTransacao();

					aux = eleitos.Where(x => x["Acao"].ToString() == "3").ToList();

					_da.Excluir(aux, bancoDeDados);

					aux = eleitos.Where(x => !aux.Exists(y => y["Id"] == x["Id"])).ToList();

					_da.Salvar(aux, bancoDeDados);

					bancoDeDados.Commit();
				}
				catch
				{
					bancoDeDados.Rollback();
					throw;
				}
			}
			#endregion
		}

		internal void AtualizarETL()
		{
			ConfiguracaoRelatorio config = _daRelatorio.Configuracao(eFato.Fiscalizacao);

			config.Tid = Guid.NewGuid().ToString();
			config.EmExecucao = true;
			_daRelatorio.AtualizarConfiguracao(config);
			config.EmExecucao = false;

			try
			{
				GerarColunasDinamicas();
				Executar(config);

				config.Erro = false;
				config.DadosAte = config.DadosAte.AddDays((config.ExecucaoInicio - config.DadosAte).Days);
				_daRelatorio.AtualizarConfiguracao(config);
			}
			catch
			{
				config.Erro = true;
				_daRelatorio.AtualizarConfiguracao(config);
				throw;
			}
		}
	}
}