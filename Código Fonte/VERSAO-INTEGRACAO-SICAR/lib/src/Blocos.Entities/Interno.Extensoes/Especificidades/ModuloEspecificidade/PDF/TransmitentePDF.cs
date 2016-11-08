using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class TransmitentePDF : PessoaPDF
	{
		public String TempoOcupacao { set; get; }

		public TransmitentePDF(){}

		public TransmitentePDF(TransmitentePosse trasmitente )
		{
			NomeRazaoSocial = trasmitente.Transmitente.NomeRazaoSocial;
			CPFCNPJ = trasmitente.Transmitente.CPFCNPJ;
			TempoOcupacao = trasmitente.TempoOcupacao.ToString();
		}
	}
}