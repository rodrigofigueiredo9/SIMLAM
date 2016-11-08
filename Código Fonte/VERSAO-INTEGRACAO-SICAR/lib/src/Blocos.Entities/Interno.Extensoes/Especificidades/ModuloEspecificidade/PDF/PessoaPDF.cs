using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class PessoaPDF
	{
		public Int32 Id { get; set; }
		public String NomeRazaoSocial { get; set; }
		public String CPFCNPJ { get; set; }
		public String RGIE { get; set; }
		public Int32 Tipo { get; set; }
		public String TipoTexto { get; set; }
		public String TipoTextoDominio { get; set; }
		public String VinculoTipoTexto { get; set; }
		public String DataNasc { get; set; }
		public String EstadoCivil { get; set; }

		public String ConjugeNome { get; set; }
		public String ConjugeCPF { get; set; }
		public String ConjugeRGIE { get; set; }
		public String ConjugeNomeMae { get; set; }
		public String ConjugeNomePai { get; set; }
		public String ConjugeNomePaiMae { get; set; }

		public String NomeMae { get; set; }
		public String NomePai { get; set; }
		public String NomePaiMae { get; set; }
		public String Nacionalidade { get; set; }
		public String Profissao { get; set; }
		public String VinculoTipoTextoUpper { get { return (VinculoTipoTexto ?? string.Empty).ToUpper(); } }

		//Endereço
		public String EndLogradouro { get; set; }
		public String EndNumero { get; set; }
		public String EndBairro { get; set; }
		public String EndMunicipio { get; set; }
		public String EndEstado { get; set; }
		public String EndUF { get; set; }
		public String EndCEP { get; set; }
		public String EndDistrito { get; set; }
	}
}