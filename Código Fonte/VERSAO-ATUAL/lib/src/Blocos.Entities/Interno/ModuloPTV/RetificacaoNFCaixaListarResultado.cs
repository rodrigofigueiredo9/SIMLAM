using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV
{
	public class RetificacaoNFCaixaListarResultado
	{
		public int Id { get; set; }
		public string NumeroNFCaixa { get; set; }
		public int TipoPessoa { get; set; }
		public string CPFCNPJ { get; set; }
		public int TipoCaixa { get; set; }
		public string TipoCaixaTexto { get; set; }
		public int SaldoInicial { get; set; }
		public int SaldoAtual { get; set; }
		public int idPTV { get; set; }
	}
}