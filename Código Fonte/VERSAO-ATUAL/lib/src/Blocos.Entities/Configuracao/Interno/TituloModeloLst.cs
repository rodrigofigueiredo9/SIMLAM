namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class TituloModeloLst : IListaValor
	{
		public int Id { set; get; }
		public string Texto { set; get; }
		public int IdRelacionamento { get; set; }
		public bool IsAtivo { set; get; }
		public int Codigo { set; get; }
		public string Sigla { set; get; }
		public string Situacao { set; get; }
	}
}