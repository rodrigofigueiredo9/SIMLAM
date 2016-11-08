

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class PerguntaInfracaoListarFiltro
	{
		public String CodigoPergunta { get; set; }

		public Int32 PerguntaId { get; set; }
		public String PerguntaTexto { get; set; }

		public Int32 RespostaId { get; set; }
		public String RespostaTexto { get; set; }
	}
}