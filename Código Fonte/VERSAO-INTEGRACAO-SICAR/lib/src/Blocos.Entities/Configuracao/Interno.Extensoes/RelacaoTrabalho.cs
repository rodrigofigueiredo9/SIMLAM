namespace Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes
{
	public class RelacaoTrabalho : IListaValor
	{
		public int Id {set;get;}
		public string Texto {set;get;}
		public bool IsAtivo {set;get;}
		public int Codigo { set; get; }
	}
}