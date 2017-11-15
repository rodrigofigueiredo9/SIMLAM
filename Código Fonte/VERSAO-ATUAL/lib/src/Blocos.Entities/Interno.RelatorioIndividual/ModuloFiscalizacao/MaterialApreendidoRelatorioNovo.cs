using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao
{
	public class MaterialApreendidoRelatorioNovo
	{
		public Int32 Id { get; set; }
		public Int32 HistoricoId { get; set; }
		public int? IsGeradoSistema { get; set; }
		public String IsApreendido { get; set; }
		public String NumeroTAD { get; set; }
		public String DataLavraturaTAD { get; set; }
        public String NumeroIUF { get; set; }
        public String DataLavraturaIUF { get; set; }
		public String DescreverApreensao { get; set; }
		public String OpinarDestino { get; set; }
		public String SerieTexto { get; set; }

        private List<ApreensaoProdutoDestinacaoRelatorio> _produtosDestinacoes = new List<ApreensaoProdutoDestinacaoRelatorio>();
        public List<ApreensaoProdutoDestinacaoRelatorio> ProdutosDestinacoes
        {
            get { return _produtosDestinacoes; }
            set { _produtosDestinacoes = value; }
        }

	}
}
