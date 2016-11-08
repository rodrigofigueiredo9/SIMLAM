using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloGeoProcessamento.Data
{
	public class Historico
	{
		GerenciadorConfiguracao<ConfiguracaoSistema> _config = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		private string EsquemaBanco { get; set; }

		public Historico(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		public void Gerar(int id, eHistoricoArtefato artefato, eHistoricoAcao acao, BancoDeDados banco, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComando(@"begin {0}historico_geoprocessamento." + artefato.ToString() + "(:id, :acao, :executor_id, :executor_nome, :executor_login, :executor_tipo_id, :executor_tid); end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("acao", Convert.ToInt32(acao), DbType.Int32);
				comando.AdicionarParametroEntrada("executor_id", DbType.Int32);
				comando.AdicionarParametroEntrada("executor_tid", DbType.String);
				comando.AdicionarParametroEntrada("executor_nome", DbType.String);
				comando.AdicionarParametroEntrada("executor_login", DbType.String);
				comando.AdicionarParametroEntrada("executor_tipo_id", DbType.Int32);

				executor = executor ?? Executor.Current;

				if (executor == null)
				{
					throw new Exception("Não foi encontrado executar para gerar Historico");
				}

				comando.SetarValorParametro("executor_id", executor.Id);
				comando.SetarValorParametro("executor_tid", executor.Tid);
				comando.SetarValorParametro("executor_nome", executor.Nome);
				comando.SetarValorParametro("executor_login", executor.Login);
				comando.SetarValorParametro("executor_tipo_id", executor.Tipo);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		public bool ValidarHistoricoAcaoId(eHistoricoAcao acao, eHistoricoArtefato artefato, int historicoId, BancoDeDados banco = null)
		{
			int retorno = ObterHistoricoAcaoId(acao, artefato, banco);
			return retorno == historicoId;
		}

		public int ObterHistoricoAcaoId(eHistoricoAcao acao, eHistoricoArtefato artefato, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ct.id from {0}lov_historico_acao c, {0}lov_historico_artefato t, {0}lov_historico_artefatos_acoes ct
					where ct.acao = c.id and ct.artefato = t.id  and lower(c.codigo) = lower(:acao) and lower(t.texto) = lower(:artefato)", EsquemaBanco);

				comando.AdicionarParametroEntrada("acao", acao.ToString(), DbType.String);
				comando.AdicionarParametroEntrada("artefato", artefato.ToString(), DbType.String);

				object resultado = bancoDeDados.ExecutarScalar(comando);

				if (resultado != null && !Convert.IsDBNull(resultado))
				{
					return Convert.ToInt32(resultado);
				}
			}
			return 0;
		}
	}
}
