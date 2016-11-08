using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data;
using Tecnomapas.EtramiteX.WindowsService.SVCAtividade.Data;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;

namespace Tecnomapas.EtramiteX.WindowsService.SVCAtividade.Business
{
	public class AtividadeIrregularBus : ServicoTimerBase
	{
		AtividadeIrregulgarDa _da = new AtividadeIrregulgarDa();
		TituloDa _tituloDa = new TituloDa();

		protected override void Executar()
		{
			try 
			{
				List<Atividade> lstAtividadesTitulosVencidos = _da.ObterTitulosVencidos();

				if (lstAtividadesTitulosVencidos == null || lstAtividadesTitulosVencidos.Count == 0)
				{
					return;
				}

				lstAtividadesTitulosVencidos.RemoveAll(x =>
					(x.SituacaoId == (int)eAtividadeSituacao.Encerrada || x.SituacaoId == (int)eAtividadeSituacao.Irregular));

				Executor.Current = new Executor()
				{
					Id = (int)eExecutorId.AtividadeServico,
					Login = "SVCAtividade",
					Nome = GetType().Assembly.ManifestModule.Name,
					Tid = GetType().Assembly.ManifestModule.ModuleVersionId.ToString(),
					Tipo = eExecutorTipo.Interno
				};

				AtividadeBus atividadeBus = new AtividadeBus();
				atividadeBus.AlterarSituacao(lstAtividadesTitulosVencidos, eAtividadeSituacao.Irregular);
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}
	}
}
