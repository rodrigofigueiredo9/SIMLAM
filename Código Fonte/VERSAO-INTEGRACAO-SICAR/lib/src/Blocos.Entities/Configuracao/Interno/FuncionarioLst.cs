namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class FuncionarioLst : IListaValor
	{
		public int Id { set; get; }
		public string Texto { set; get; }
		public bool IsAtivo { set; get; }
	}
}