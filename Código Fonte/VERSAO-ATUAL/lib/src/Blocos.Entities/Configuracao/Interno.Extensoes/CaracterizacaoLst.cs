namespace Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes
{
	public class CaracterizacaoLst : IListaValor
	{
		public int Id { get; set; }
		public string Texto { get; set; }
		public bool IsAtivo { get; set; }
		public string Tabela { get; set; }
		public bool IsProjeto { get; set; }
		public bool IsDescricao { get; set; }
		public bool IsExibirCredenciado { get; set; }
		public bool ParecerFavoravel { get; set; }
		public ePermissaoTipo Permissao { get; set; }
	}
}