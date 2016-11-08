

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa
{
	public class FisicaRelatorio
	{
		public String CPF { get; set; }
		public String Nome { get; set; }
		public String RG { get; set; }
		public Int32? EstadoCivil { get; set; }
		public Int32? Sexo { get; set; }
		public String Nacionalidade { get; set; }
		public String Naturalidade { get; set; }
		public DateTime? DataNascimento { get; set; }
		public String Profissao { get; set; }
		public String Apelido { get; set; }

		public FisicaRelatorio()
		{
		}
	}
}
