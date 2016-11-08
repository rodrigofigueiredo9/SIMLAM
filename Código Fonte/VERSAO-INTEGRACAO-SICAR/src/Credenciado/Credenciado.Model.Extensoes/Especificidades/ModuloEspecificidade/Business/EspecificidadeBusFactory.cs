using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloCertidao.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business
{
	public static class EspecificiadadeBusFactory
	{
		public static bool Possui(int modeloCodigo)
		{
			return Enum.IsDefined(typeof(eEspecificidade), modeloCodigo);
		}

		public static IEspecificidadeBus Criar(int modeloCodigo)
		{
			if (!Possui(modeloCodigo))
			{
				return new EspecificidadeBusDefault();
			}

			eEspecificidade modeloEsp = (eEspecificidade)modeloCodigo;

			switch (modeloEsp)
			{
				case eEspecificidade.CertidaoDispensaLicenciamentoAmbiental:
					return new CertidaoDispensaLicenciamentoAmbientalBus();

				default:
					return new EspecificidadeBusDefault();
			}
		}
	}
}