namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class Segmento : IListaValorString
	{
		public string Id { get; set; }
		public string Texto { get; set; }
		public string Denominador { get; set; }
		public bool IsAtivo { get; set; }
	}
}