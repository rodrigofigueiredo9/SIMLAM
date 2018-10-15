using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class EmpreendimentoPDF
	{
		public Int32? Id { get; set; }
		public String Codigo { get; set; }
		public String CodigoImovel { get; set; }
		public String Nome { get; set; }
		public String CNPJ { get; set; }
		public String Denominador { get; set; }
		public String Segmento { get; set; }

		public String Telefone { get; set; }
		public String TelFax { get; set; }
		public String Email { get; set; }

		//Endereço
		public String EndLogradouro { get; set; }
		public String EndCEP { get; set; }
		public String EndNumero { get; set; }
		public String EndBairro { get; set; }
		public String EndMunicipio { get; set; }
		public String EndEstado { get; set; }
		public String EndDistrito { get; set; }
		public String EndComplemento { get; set; }
		public String EndCorrego { get; set; }
		public String EndZona { get; set; }
		public Int32 EndZonaId { get; set; }
		public String EndCaixaPostal { get; set; }
		public String EndLocalColeta { get; set; }
		public String EndFormaColeta { get; set; }
		public String EndDatum { get; set; }
		public String EndSisCoordenada { get; set; }
		public String EndNorthingLatitude { get; set; }
		public String EndEastingLongitude { get; set; }
		public String EndFuso { get; set; }
		public String EndUF { get; set; }

		public String EndLocalizacao
		{
			get
			{
				if (EndZonaId == 2/*Rural*/)
				{
					return EndLogradouro;
				}

				return EndLogradouro + " " + EndNumero + " " + EndBairro;

			}
		}

		/*Endereco Correspondencia*/
		public String EndCorrespLogradouro { get; set; }
		public String EndCorrespCEP { get; set; }
		public String EndCorrespNumero { get; set; }
		public String EndCorrespBairro { get; set; }
		public String EndCorrespMunicipio { get; set; }
		public String EndCorrespEstado { get; set; }
		public String EndCorrespDistrito { get; set; }
		public String EndCorrespComplemento { get; set; }
		public String EndCorrespCorrego { get; set; }
		public String EndCorrespUF { get; set; }
		public String EndCorrespCaixaPostal { get; set; }
		public Int32 EndCorrespZonaId { get; set; }

		public String ATPCroquiHa { get { return ATPCroquiDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(); } }
		public Decimal ATPCroquiDecimal { get; set; }
		public String ARLRecebidaHa { get { return ARLRecebidaDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(); } }
		public Decimal ARLRecebidaDecimal { get; set; }

		public string PorcentagemARLSobreCroqui
		{
			get
			{
				return ((ARLRecebidaDecimal / ATPCroquiDecimal) * 100).ToStringTrunc();
			}
		}
	}
}