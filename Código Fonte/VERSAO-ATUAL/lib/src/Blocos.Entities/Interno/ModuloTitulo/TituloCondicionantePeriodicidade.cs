using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class TituloCondicionantePeriodicidade
	{
		public int Id { get; set; }

		public string Tid { get; set; }

		public int? DiasProrrogados { get; set; }	// dias
			
		private TituloCondicionanteSituacao _situacao = new TituloCondicionanteSituacao();
		public TituloCondicionanteSituacao Situacao { get { return _situacao; } set { _situacao = value; } }

		private DateTecno _dataInicioPrazo = new DateTecno();
		public DateTecno DataInicioPrazo { get { return _dataInicioPrazo; } set { _dataInicioPrazo = value; } }

		private DateTecno _dataVencimento = new DateTecno();
		public DateTecno DataVencimento { get { return _dataVencimento; } set { _dataVencimento = value; } }
	}
}