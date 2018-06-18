using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class ParametrizacaoListarResultado
	{
		public Int32 Id { get; set; }

		public Int32 CodigoReceitaId { get; set; }
		public String CodigoReceitaTexto { get; set; }
		public DateTime InicioVigencia { get; set; }
		public DateTime? FimVigencia { get; set; }
		public Int32 MaximoParcelas { get; set; }
		public Decimal ValorMinimoPF { get; set; }
		public Decimal ValorMinimoPJ { get; set; }
		public Int32? MultaPercentual { get; set; }
		public Int32? JurosPercentual { get; set; }
		public Int32? DescontoPercentual { get; set; }
		public Int32? PrazoDescontoUnidade { get; set; }
		public Int32? PrazoDescontoDecorrencia { get; set; }
	}
}
