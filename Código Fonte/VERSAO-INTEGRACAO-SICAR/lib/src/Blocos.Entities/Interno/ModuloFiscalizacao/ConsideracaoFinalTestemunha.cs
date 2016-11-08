namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class ConsideracaoFinalTestemunha
	{
		public int Id { get; set; }
		public int ConsideracaoFinalId { get; set; }
		public bool? TestemunhaIDAF { get; set; }
		public int? TestemunhaId { get; set; }
		public string TestemunhaNome { get; set; }
		public string TestemunhaEndereco { get; set; }
		public string Tid { get; set; }
		public int Colocacao { get; set; }
		public int? TestemunhaSetorId { get; set; }

		public ConsideracaoFinalTestemunha()
		{
			this.TestemunhaNome =
			this.TestemunhaEndereco =
			this.Tid = string.Empty;
		}
	}
}
