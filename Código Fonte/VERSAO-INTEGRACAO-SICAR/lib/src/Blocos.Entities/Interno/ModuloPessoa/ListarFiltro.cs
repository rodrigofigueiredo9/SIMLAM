namespace Tecnomapas.Blocos.Entities.Interno.ModuloPessoa
{
	public class ListarFiltro
	{
		public string NomeRazaoSocial { get; set; }
		public string CpfCnpj { get; set; }
		public int Tipo { get; set; }
		public int QuantPaginacao { get; set; }
		public bool IsCpf { get; set; }
		public int Credenciado { get; set; }

		public ListarFiltro()
		{
		}
	}
}
