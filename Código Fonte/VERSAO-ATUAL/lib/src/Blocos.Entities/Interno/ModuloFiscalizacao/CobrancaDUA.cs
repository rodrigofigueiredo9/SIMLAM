using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class CobrancaDUA
	{
		#region Constructor
		public CobrancaDUA() { }
		#endregion

		#region Properties
		public Int32 Id { get; set; }
		public String Tid { get; set; }

		private DateTecno _dataVencimento = new DateTecno();
		public DateTecno DataVencimento
		{
			get { return _dataVencimento; }
			set { _dataVencimento = value; }
		}

		private DateTecno _dataEmissao = new DateTecno();
		public DateTecno DataEmissao
		{
			get { return _dataEmissao; }
			set { _dataEmissao = value; }
		}

		public String Parcela { get; set; }
		public Int32 NumeroDUA { get; set; }
		public Decimal ValorDUA { get; set; }
		public Decimal ValorPago { get; set; }
		public Decimal VRTE { get; set; }

		private DateTecno _dataPagamento = new DateTecno();
		public DateTecno DataPagamento
		{
			get { return _dataPagamento; }
			set { _dataPagamento = value; }
		}

		public String Situacao { get; set; }
		public String InformacoesComplementares { get; set; }
		public Int32? ParcelaPaiId { get; set; }
		public CobrancaDUA ParcelaPai { get; set; }
		public Int32 ParcelamentoId { get; set; }
		#endregion
	}
}