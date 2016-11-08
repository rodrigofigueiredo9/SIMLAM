namespace Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado
{
	public class ListarFiltro
	{
		public string Id { get; set; }
		public string Tid { get; set; }
		public string NomeRazaoSocial { get; set; }
		public string NumeroHabilitacao { get; set; }
		public string NomeComumPraga { get; set; }
		public string NomeCultura { get; set; }
		public string CpfCnpj { get; set; }
		public int Tipo { get; set; }
		public string TipoTexto { get; set; }
		public string DataAtivacao { get; set; }
		public int Situacao { get; set; }
		public string SituacaoTexto { get; set; }
		public int QuantPaginacao { get; set; }
		public bool PodeBloquear
		{
			get { return Situacao == 2 || Situacao == 4; /*Ativo e Senha vencida*/ }
		}

		public ListarFiltro()
		{
		}
	}
}
