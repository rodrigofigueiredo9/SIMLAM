using System;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMRequerimento
{
	public class ListarVME
	{
		public Int32 Id { get; set; }
		public Int32 Numero { get; set; }
		public String NomeRazaoInteressado { get; set; }
		public String DataCriacaoTexto { get; set; }
		public String Denominacao { get; set; }
		public Int32 Situacao { get; set; }
		public String SituacaoTexto { get; set; }
	}
}