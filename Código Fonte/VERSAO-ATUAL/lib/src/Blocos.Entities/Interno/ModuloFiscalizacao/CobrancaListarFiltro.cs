using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class CobrancaListarFiltro
	{
		public string NumeroRegistroProcesso { get; set; }
		public string NumeroAutuacao { get; set; }
		public string NumeroFiscalizacao { get; set; }
		public string NumeroAIIUF { get; set; }
		public string SituacaoFiscalizacao { get; set; }
		public string SituacaoDUA { get; set; }
		public string NumeroDUA { get; set; }
		public DateTime? DataVencimentoDe { get; set; }
		public DateTime? DataVencimentoAte { get; set; }
		public DateTime? DataPagamentoDe { get; set; }
		public DateTime? DataPagamentoAte { get; set; }
		public string NumeroRazaoSocial { get; set; }
		
	}
}
