namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class FuncionarioPDF
	{
		public int Id { get; set; }
		public int UsuarioId { get; set; }
		public string UsuarioLogin { get; set; }
		public string Nome { get; set; }
		public string Cpf { get; set; }
		public string Email { get; set; }
		public int Tipo { get; set; }
		public int Situacao { get; set; }
		public string SituacaoMotivo { get; set; }
		public string Tid { get; set; }

		public FuncionarioPDF()
		{
			this.Id =
			this.UsuarioId =
			this.Tipo =
			this.Situacao = 0;
			this.UsuarioLogin =
			this.Nome =
			this.Cpf =
			this.Email =
			this.SituacaoMotivo =
			this.Tid = "";
		}
	}
}