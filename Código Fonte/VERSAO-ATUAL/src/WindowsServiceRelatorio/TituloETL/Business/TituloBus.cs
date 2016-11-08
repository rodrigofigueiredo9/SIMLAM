using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Data;
using Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Entities;
using Tecnomapas.EtramiteX.WindowsService.TituloETL.Data;

namespace Tecnomapas.EtramiteX.WindowsService.TituloETL.Business
{
	public class TituloBus
	{
		ConfiguracaoSistema _configSys;
		RelatorioDa _daRelatorio;
		TituloDa _da;

		public TituloBus()
		{
			_configSys = new ConfiguracaoSistema();
			_daRelatorio = new RelatorioDa();
			_da = new TituloDa();
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
			ConfiguracaoRelatorio config = _daRelatorio.Configuracao(eFato.Titulo);

			config.Tid = Guid.NewGuid().ToString();
			config.EmExecucao = true;
			_daRelatorio.AtualizarConfiguracao(config);
			config.EmExecucao = false;

			try
			{
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