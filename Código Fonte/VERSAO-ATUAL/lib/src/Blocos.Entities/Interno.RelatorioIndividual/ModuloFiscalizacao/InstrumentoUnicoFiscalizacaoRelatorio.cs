

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao
{
	public class InstrumentoUnicoFiscalizacaoRelatorio
	{
		#region Cabeçalho

		public Int32 SetorId { get; set; }
		public Int32 SituacaoId { get; set; }
        //public String NumeroAutoTermo { get; set; }
        public string NumeroIUF { get; set; }
		public String Serie { get; set; }
		public String CodigoUnidadeConvenio { get; set; }
		public String DataVencimento { get; set; }
        //public String IsAI { get; set; }
        //public String IsTAD { get; set; }
        //public String IsTEI { get; set; }

        public String IsDDSIA { get; set; }
        public String IsDDSIV { get; set; }
        public String IsDRNRE { get; set; }
		public String GovernoNome { get; set; }
		public String SecretariaNome { get; set; }
		public String OrgaoNome { get; set; }
        //public String SetorNome { get; set; }
        public String DocumentoNome { get; set; }
		public Byte[] LogoBrasao { get; set; }
        public Byte[] Logomarca { get; set; }

		#endregion

		#region Identificação do autuado

		public String AutuadoCPFCNPJ { get; set; }
		public String AutuadoNomeRazaoSocial { get; set; }
		public String AutuadoEstadoCivil { get; set; }
		public String AutuadoNaturalidade { get; set; }
		public String AutuadoRG { get; set; }
		public String AutuadoEndLogradouro { get; set; }
		public String AutuadoEndNumero { get; set; }
		public String AutuadoEndMunicipio { get; set; }
		public String AutuadoEndUF { get; set; }
		public String AutuadoEndBairro { get; set; }
		public String AutuadoEndDistrito { get; set; }
		public String AutuadoEndCEP { get; set; }
		public String AutuadoEndComplemento { get; set; }

		#endregion

		#region Enquadramento

		public String EnquadramentoArtigo1 { get; set; }
		public String EnquadramentoArtigoItemParagrafo1 { get; set; }
		public String EnquadramentoCombinadoArtigo1 { get; set; }
		public String EnquadramentoCombinadoArtigoItemParagrafo1 { get; set; }
		public String EnquadramentoCitarNormaLegal1 { get; set; }

		public String EnquadramentoArtigo2 { get; set; }
		public String EnquadramentoArtigoItemParagrafo2 { get; set; }
		public String EnquadramentoCombinadoArtigo2 { get; set; }
		public String EnquadramentoCombinadoArtigoItemParagrafo2 { get; set; }
		public String EnquadramentoCitarNormaLegal2 { get; set; }

		public String EnquadramentoArtigo3 { get; set; }
		public String EnquadramentoArtigoItemParagrafo3 { get; set; }
		public String EnquadramentoCombinadoArtigo3 { get; set; }
		public String EnquadramentoCombinadoArtigoItemParagrafo3 { get; set; }
		public String EnquadramentoCitarNormaLegal3 { get; set; }

		public String DiaAutuacao { get; set; }
		public String MesAutuacao { get; set; }
		public String AnoAtuacao { get; set; }
		public String Local { get; set; }
		public String CoordenadaEasting { get; set; }
		public String CoordenadaNorthing { get; set; }

		#endregion

		#region Descrição da infração

		public String DescricaoInfracao { get; set; }

		#endregion

		#region Valor

		public String ValorMulta { get; set; }
		public String CodigoReceita { get; set; }
		public String ValorMultaPorExtenso { get; set; }

		#endregion

		#region Descrição da apreensão

		public String DescreverApreensao { get; set; }
		public String ValorBemProdutoArbitrado { get; set; }
		public String ValorBemPorExtenso { get; set; }
		public String DepositarioLogradouro { get; set; }
		public String DepositarioBairro { get; set; }
		public String DepositarioDistrito { get; set; }
		public String DepositarioMunicipio { get; set; }
		public String DepositarioUF { get; set; }
		public String DepositarioNome { get; set; }
		public String DepositarioEstadoCivil { get; set; }
		public String DepositarioNaturalidade { get; set; }
		public String DepositarioRG { get; set; }
		public String DepositarioEndLogradouro { get; set; }
		public String DepositarioEndBairro { get; set; }
		public String DepositarioEndDistrito { get; set; }
		public String DepositarioEndMunicipio { get; set; }

		#endregion

		#region Descrição do embargo/interdição

		public String DescricaoTermoEmbargo { get; set; }

		#endregion

		#region Firmas

		public String NomeUsuarioCadastro { get; set; }
		public String ResponsavelEmpNomeRazaoSocial { get; set; }
		public String ResponsavelEmpCPFCNPJ { get; set; }

		#endregion

		#region Testemunhas

		public String TestemunhaNome1 { get; set; }
		public String TestemunhaEnd1 { get; set; }
		public String TestemunhaNome2 { get; set; }
		public String TestemunhaEnd2 { get; set; }

		#endregion
	}
}