using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao
{
	public class CertidaoDispensaLicenciamentoAmbiental : Especificidade
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 Atividade { get; set; }
		public Int32 VinculoPropriedade { get; set; }
		public string VinculoPropriedadeOutro { get; set; }
		public string Interessado { get; set; }
		public CertidaoDispensaLicenciamentoAmbiental() { }
	}
}