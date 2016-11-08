namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class Assinante
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public int? IdRelacionamento { get; set; }

		public int SetorId { get; set; }
		public string SetorTexto { get; set; }
		public int TipoId { get; set; }
		public string TipoTexto { get; set; }
	}
}