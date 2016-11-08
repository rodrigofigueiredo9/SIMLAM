using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness
{
	public class CaracterizacaoBusFactory
	{
		public static ICaracterizacaoBus Criar(eCaracterizacao caracterizacao)
		{
			switch (caracterizacao)
			{
				case eCaracterizacao.Nulo:
					return null;

				case eCaracterizacao.Dominialidade:
					return new DominialidadeBus();

				case eCaracterizacao.UnidadeProducao:
					return new UnidadeProducaoBus();

				case eCaracterizacao.UnidadeConsolidacao:
					return new UnidadeConsolidacaoBus();

                case eCaracterizacao.BarragemDispensaLicenca:
                    return new BarragemDispensaLicencaBus();
				default:
					return null;
			}
		}
	}
}