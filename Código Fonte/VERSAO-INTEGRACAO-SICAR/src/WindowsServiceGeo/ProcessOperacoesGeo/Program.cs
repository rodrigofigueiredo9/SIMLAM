using System;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Entities;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessValidacaoGeo
{
	static class Program
	{
		/// <summary>
		/// projectID: ID do projeto geofráfico
		/// 
		/// projectType: Tipo do projeto em execução, sendo:
		/// 1 - Base de Referência Interna, 2 - Base de Referência GEOBASES, 3 - Dominialidade, 4 - Atividade
		/// 5 - Fiscalização, 6 - Base de Referência Fiscalização, 7 - CAR
		/// 
		/// projectStep: Etapa do precessamento
		/// 1 - Validação, 2 - Processamento, 3 - Geração de PDF
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			try
			{
				if (args == null || args.Length <= 0)
				{
					throw new Exception("Nenhum argumento informado.");
				}

				string[] parametros = args[0].Split(',');

				//projectID,projectType,projectStep 
				//string[] parametros = "1490,3,2,mutexServ1,mutexProc1".Split(',');

				if (parametros == null || parametros.Length != 5)
				{
					throw new Exception("Argumentos insuficientes.");
				}

				if (Convert.ToInt32(parametros[0]) <= 0)
				{
					throw new Exception("PROJETO do TICKET inválido.");
				}

				if (Convert.ToInt32(parametros[1]) <= 0)
				{
					throw new Exception("TIPO do TICKET inválido.");
				}

				if (Convert.ToInt32(parametros[2]) <= 0)
				{
					throw new Exception("ETAPA do TICKET inválida.");
				}

				if (parametros[3] == string.Empty)
				{
					throw new Exception("Nome do mutex do serviço não encontrado.");
				}

				if (parametros[4] == string.Empty)
				{
					throw new Exception("Nome do mutex do processo não encontrado.");
				}

				Projeto projeto = new Projeto();
				projeto.Id = Convert.ToInt32(parametros[0]);
				projeto.Type = Convert.ToInt32(parametros[1]);
				projeto.Step = Convert.ToInt32(parametros[2]);

				ProcessoOperacoesGeoBus operacoes = new ProcessoOperacoesGeoBus(projeto, parametros[3], parametros[4]);

				operacoes.Executar();
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}
	}
}