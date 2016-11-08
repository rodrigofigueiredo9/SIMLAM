
namespace Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico
{
	public class AgrotoxicoFiltro
	{
		public int Id { get; set; }
		public int ArquivoId { get; set; }
		public string NomeComercial { get; set; }
		public string NumeroCadastro { get; set; }
		public string NumeroRegistroMinisterio { get; set; }
		public bool CadastroAtivo { get; set; }
		public bool PossuiCadastro { get; set; }
		public int MotivoId { get; set; }
		public string Situacao { get; set; }
		public string TitularRegistro { get; set; }
		public string NumeroProcessoSep { get; set; }
		public int IngredienteAtivoId { get; set; }
		public string IngredienteAtivo { get; set; }
		public string Cultura { get; set; }
		public string Praga { get; set; }
		public int ClasseUso { get; set; }
		public int ModalidadeAplicacao { get; set; }
		public int GrupoQuimico { get; set; }
		public int ClassificacaoToxicologica { get; set; }
	}
}