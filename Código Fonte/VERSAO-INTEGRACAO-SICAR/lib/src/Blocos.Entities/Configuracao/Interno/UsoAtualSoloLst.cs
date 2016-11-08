namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class UsoAtualSoloLst : IListaValor
	{
		public int Id { get; set; }
		public bool IsAtivo { get; set; }
		public string Texto { get; set; }
		public string TipoGeo { get; set; }
	}
}