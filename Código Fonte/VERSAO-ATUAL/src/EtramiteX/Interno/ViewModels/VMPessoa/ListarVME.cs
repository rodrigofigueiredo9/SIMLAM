

using System;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa
{
	public class ListarVME
	{
		public Int32 Id {get; set; }
		public Int32 Tipo { get; set; }
		public String NomeRazaoSocial { get; set; }
		public String CpfCnpj { get; set; }
		public String RgIe { get; set; }
		public String NomeFantasia { get; set; }
	}
}