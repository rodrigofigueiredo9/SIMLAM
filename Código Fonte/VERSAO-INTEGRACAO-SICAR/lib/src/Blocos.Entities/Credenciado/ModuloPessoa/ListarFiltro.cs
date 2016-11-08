namespace Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa
{
	public class ListarFiltro
	{

		public string CpfCnpj { get; set; }
		public string DataAtivacao { get; set; }
		public string Id { get; set; }
		public string NomeRazaoSocial { get; set; }
		public bool PodeBloquear { get; set; }
		public int QuantPaginacao { get; set; }
		public int Situacao { get; set; }
		public string SituacaoTexto { get; set; }
		public string Tid { get; set; }
		public int Tipo { get; set; }
		public string TipoTexto { get; set; }

		public ListarFiltro()
		{
		}
	}
}

