

using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloChecagemPendencia
{
	public class ListarFiltroChecagemPendencia
	{
		public int? Numero { get; set; }
		public int SituacaoPendencia { get; set; }
		public int Titulo { get; set; }
		public string TituloNumero { get; set; }
		public string InteressadoNome { get; set; }
		public ProtocoloNumero Protocolo { get; set; }
	}
}