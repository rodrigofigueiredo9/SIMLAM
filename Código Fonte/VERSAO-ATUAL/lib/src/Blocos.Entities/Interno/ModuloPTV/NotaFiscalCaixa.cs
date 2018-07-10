using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV
{
	public class NotaFiscalCaixa
	{
		public int id { get; set; }

		public int? notaFiscalCaixaApresentacao { get; set; }
		public string notaFiscalCaixaNumero { get; set; }

		public int saldoAtual { get; set; }
		public int numeroCaixas { get; set; }
		public int tipoCaixaId { get; set; }
		public string tipoCaixaTexto { get; set; }

		public NotaFiscalCaixa()
		{
			notaFiscalCaixaApresentacao = 1;
		}
	}
}
