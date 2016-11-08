using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data
{
    public class HistoricoCaracterizacaoDa
    {
        private ConfiguracaoSistema _configSis = new ConfiguracaoSistema();

        public string EsquemaBanco { get { return _configSis.UsuarioInterno; } }

        public HistoricoCaracterizacaoDa() { }

		public void Gerar(int id, eHistoricoArtefatoCaracterizacao artefato, eHistoricoAcao acao, Executor executor = null)
		{
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComando(@"begin {0}historico_caracterizacao." + artefato.ToString() + "(:id, :acao, :executor_id, :executor_nome, :executor_login, :executor_tipo_id, :executor_tid); end;", EsquemaBanco);

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

		public void GerarGeo(int id, eHistoricoArtefatoCaracterizacao artefato, eHistoricoAcao acao, Executor executor = null)
		{
			GerarGeo(id, ((int) artefato), acao, executor);
		}

		public void GerarGeo(int id, int artefato, eHistoricoAcao acao, Executor executor = null)
		{
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComando(@"begin {0}geo_operacoesprocessamentogeo.GerarHistoricoGEO(:id, :acao, :artefato, :executor_id, :executor_nome, :executor_login, :executor_tipo_id, :executor_tid); end;", EsquemaBanco);

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
