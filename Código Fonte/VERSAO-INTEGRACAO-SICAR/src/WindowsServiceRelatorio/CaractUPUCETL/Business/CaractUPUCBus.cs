using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Data;
using Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Entities;
using Tecnomapas.EtramiteX.WindowsService.CaractUPUCETL.Data;

namespace Tecnomapas.EtramiteX.WindowsService.CaractUPUCETL.Business
{
	public class CaractUPUCBus
	{
		ConfiguracaoSistema _configSys;
		RelatorioDa _daRelatorio;
		CaractUPUCDa _da;

		public CaractUPUCBus()
		{
			_configSys = new ConfiguracaoSistema();
			_daRelatorio = new RelatorioDa();
			_da = new CaractUPUCDa();
		}

		private void Executar(ConfiguracaoRelatorio config)
		{
			#region Obter Eleitos

			List<Dictionary<string, object>> eleitos;
			List<Dictionary<string, object>> eleitosUC;
			List<Dictionary<string, object>> eleitosUP;
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

					#region UP - Unidade de Produção
					// Código 51 - Modelo: Abertura de livro de unidade de produção
					eleitosUP = eleitos.Where(x => x["cod_modelo"].ToString() == "51").ToList();

					aux = eleitosUP.Where(x => x["Acao"].ToString() == "3").ToList();

					_da.ExcluirUP(aux, bancoDeDados);

					aux = eleitosUP.Where(x => !aux.Exists(y => y["hst_titulo_id"] == x["hst_titulo_id"])).ToList();

					_da.SalvarUP(aux, bancoDeDados);

					#endregion

					#region UC - Unidade de Consolidação

					// Código 52 - Modelo: Abertura de livro de unidade de consolidação
					eleitosUC = eleitos.Where(x => x["cod_modelo"].ToString() == "52").ToList();

					aux = eleitosUC.Where(x => x["Acao"].ToString() == "3").ToList();

					_da.ExcluirUC(aux, bancoDeDados);

					aux = eleitosUC.Where(x => !aux.Exists(y => y["hst_titulo_id"] == x["hst_titulo_id"])).ToList();

					_da.SalvarUC(aux, bancoDeDados);
					#endregion

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
			ConfiguracaoRelatorio config = _daRelatorio.Configuracao(eFato.CaracterizacaoUPUC);

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