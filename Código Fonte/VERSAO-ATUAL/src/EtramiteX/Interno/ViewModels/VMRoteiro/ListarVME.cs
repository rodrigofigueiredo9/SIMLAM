

using System;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro
{
	public class ListarVME
	{
		public Int32 Id { get; set; }
		public Int32? Versao { get; set; }
		public String Nome { get; set; }
		public Int32? Numero { get; set; }
		public String SituacaoTexto { get; set; }
		public Int32 SituacaoId { get; set; }
		public String Tid { get; set; }
	}
}