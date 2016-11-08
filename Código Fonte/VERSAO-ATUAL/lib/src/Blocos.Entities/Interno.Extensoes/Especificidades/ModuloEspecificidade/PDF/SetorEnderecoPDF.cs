namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class SetorEnderecoPDF
	{
		public int Id { get; set; }
		public string Setor { get; set; }
		public string CEP { get; set; }
		public string Logradouro { get; set; }
		public string Bairro { get; set; }
		public int EstadoId { get; set; }
		public string EstadoTexto { get; set; }
		public int MunicipioId { get; set; }
		public string MunicipioTexto { get; set; }
		public string Numero { get; set; }
		public string Distrito { get; set; }
		public string Complemento { get; set; }
		public string Fone { get; set; }
		public string FoneFax { get; set; }
		public string Tid { get; set; }
	}
}