using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Data
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

		public void Gerar(int id, eHistoricoArtefatoCaracterizacao artefato, eHistoricoAcao acao, BancoDeDados banco, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComando(String.Format(@"begin {0}historico_caracterizacao." + artefato.ToString() + "(:id, :acao, :executor_id, :executor_nome, :executor_login, :executor_tipo_id, :executor_tid); end;", EsquemaBanco));
				
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

		public void GerarGeo(int id, eHistoricoArtefatoCaracterizacao artefato, eHistoricoAcao acao, BancoDeDados banco, Executor executor = null)
		{
			GerarGeo(id, ((int)artefato), acao, banco, executor);
		}

		public void GerarGeo(int id, int artefato, eHistoricoAcao acao, BancoDeDados banco, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComando(String.Format(@"begin {0}operacoesprocessamentogeo.GerarHistoricoGEO(:id, :acao, :artefato, :executor_id, :executor_nome, :executor_login, :executor_tipo_id, :executor_tid); end;", EsquemaBanco));

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("acao", Convert.ToInt32(acao), DbType.Int32);
				comando.AdicionarParametroEntrada("artefato", artefato, DbType.Int32);
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

	}
}
