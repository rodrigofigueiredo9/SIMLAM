using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRelatorioEspecifico.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloRelatorioEspecifico;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRelatorioEspecifico.Business
{
	public class RelatorioMapaBus
	{
		RelatorioMapaDa _da = new RelatorioMapaDa();

		public bool ValidarParametroRelatorio(RelatorioMapaFiltroeResultado relatorio)
		{
			if (!ValidacoesGenericasBus.ValidarData(relatorio.DataInicial))
			{
				Validacao.Add(Mensagem.Relatorio.DataInicialObrigatorio);
			}
			if (!ValidacoesGenericasBus.ValidarData(relatorio.DataFinal))
			{
				Validacao.Add(Mensagem.Relatorio.DataFinalObrigatorio);
			}
			if (relatorio.tipoRelatorio <= 0)
			{
				Validacao.Add(Mensagem.Relatorio.TipodoRelatorio);
			}
			return Validacao.EhValido;
		}


	}
}
