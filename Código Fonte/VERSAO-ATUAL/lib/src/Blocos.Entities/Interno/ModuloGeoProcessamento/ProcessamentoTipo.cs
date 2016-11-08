

using Tecnomapas.Blocos.Entities.Configuracao;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloGeoProcessamento
{
	public class ProcessamentoTipo : IListaValorString
	{
		public string Id { get; set; }
		public bool IsAtivo { get; set; }
		public string Texto { get; set; }
	}
}
