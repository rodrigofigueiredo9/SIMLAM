using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

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
		public string NomeRazaoSocial { get; set; }
		public string CPFCNPJ { get; set; }

		private DateTecno _dataVencimentoDe = new DateTecno();
		public DateTecno DataVencimentoDe
		{
			get { return _dataVencimentoDe; }
			set { _dataVencimentoDe = value; }
		}

		private DateTecno _dataVencimentoAte = new DateTecno();
		public DateTecno DataVencimentoAte
		{
			get { return _dataVencimentoAte; }
			set { _dataVencimentoAte = value; }
		}

		private DateTecno _dataPagamentoDe = new DateTecno();
		public DateTecno DataPagamentoDe
		{
			get { return _dataPagamentoDe; }
			set { _dataPagamentoDe = value; }
		}

		private DateTecno _dataPagamentoAte = new DateTecno();
		public DateTecno DataPagamentoAte
		{
			get { return _dataPagamentoAte; }
			set { _dataPagamentoAte = value; }
		}
	}
}
