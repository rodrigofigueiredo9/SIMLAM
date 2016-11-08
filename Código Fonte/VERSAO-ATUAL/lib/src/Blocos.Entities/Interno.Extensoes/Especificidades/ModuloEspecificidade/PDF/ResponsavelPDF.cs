using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class ResponsavelPDF : PessoaPDF
	{
		public String OrgaoClasseSigla { get; set; }
		public String NumeroRegistro { get; set; }
		public String NumeroART { get; set; }
		public String Funcao { get; set; }
		public String CFONumero { get; set; }
		public String DadosCompletos { get; set; }
	}
}