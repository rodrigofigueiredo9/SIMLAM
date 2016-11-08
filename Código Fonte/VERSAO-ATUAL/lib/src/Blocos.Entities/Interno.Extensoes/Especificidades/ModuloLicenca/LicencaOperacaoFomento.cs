using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca
{
	public class LicencaOperacaoFomento : Especificidade
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }
		public Int32 Requerimento { get; set; }
	}
}