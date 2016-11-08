using System.Collections.Generic;

namespace Tecnomapas.EtramiteX.Scheduler.models.misc
{
	public class MensagemRetorno
	{
		//Domínio do campo CODIGORESPOSTA
		public const int CodigoRespostaSucesso = 200;
		public const int CodigoRespostaInconformidade = 400;
		public const int CodigoRespostaErro = 500;

		public int codigoResposta { get; set; }
		public string codigoImovel { get; set; }
		public string codigoImovelComMascara { get; set; }
		public string urlReciboInscricao { get; set; }
		public List<string> mensagensResposta { get; set; }
		public string protocoloImovel { get; set; }
		public int? idtImovel { get; set; }
		public string statusImovel { get; set; }


		public object diretorioTemp { get; set; }
		public object imoveisImpactados { get; set; }
	}
}
