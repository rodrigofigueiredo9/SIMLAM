

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento
{
	public class ResponsavelRelatorio
	{
		public Int32? Id { get; set; }
		public Int32? IdRelacionamento { get; set; }
		public String NomeRazao { get; set; }
		public String CpfCnpj { get; set; }
		public Int32? Tipo { get; set; }
		public String TipoTexto { get; set; }
		public String DataVencimentoTexto { get; set; }
		public DateTime? DataVencimento { get; set; }
		public string Habilitacao { get; set; }
	}
}
