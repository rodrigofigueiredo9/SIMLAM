

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class PerguntaInfracaoListarResultado
	{
		public Int32 Id { get; set; }
		public String Texto { get; set; }

		public Int32 SituacaoTipoId { get; set; }
		public String SituacaoTipoTexto { get; set; }

		private Resposta _resposta = new Resposta();
		public Resposta Resposta
		{
			get { return _resposta; }
			set { _resposta = value; }
		}
	}
}
