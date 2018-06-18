

using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class FiscalizacaoDocumento
	{
		public int FiscalizacaoId { get; set; }
		public int HistoricoId { get; set; }
		public DateTecno SituacaoData { get; set; }

		public Arquivo.Arquivo PdfGeradoAutoTermo { get; set; }
		public Arquivo.Arquivo PdfGeradoLaudo { get; set; }
        public Arquivo.Arquivo PdfGeradoIUF { get; set; }

		public Arquivo.Arquivo Croqui { get; set; }

		//Arquivo Uploaded pela Tela de Infracao
		public Arquivo.Arquivo PdfAutoInfracao { get; set; }

		//Arquivo Uploaded pela Tela de Objeto de Infracao
		public Arquivo.Arquivo PdfTermoEmbargoInter { get; set; }

		//Arquivo Uploaded pela Tela de Material apreendido
		public Arquivo.Arquivo PdfTermoApreensaoDep { get; set; }

		//Arquivo Uploaded pela Tela de Consideracoes Finais
		public Arquivo.Arquivo PdfTermoCompromisso { get; set; }

		public FiscalizacaoDocumento()
		{
			PdfGeradoAutoTermo = new Arquivo.Arquivo();
			PdfGeradoLaudo = new Arquivo.Arquivo();
            PdfGeradoIUF = new Arquivo.Arquivo();

			SituacaoData = new DateTecno();
			Croqui = new Arquivo.Arquivo();

			PdfAutoInfracao = new Arquivo.Arquivo();
			PdfTermoEmbargoInter = new Arquivo.Arquivo();
			PdfTermoApreensaoDep = new Arquivo.Arquivo();
			PdfTermoCompromisso = new Arquivo.Arquivo();
		}
	}
}
