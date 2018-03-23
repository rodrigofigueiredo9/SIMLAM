using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class CobrancasResultado
	{
		#region Constructor
		public CobrancasResultado() { }
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

		private DateTecno _dataCancelamento = new DateTecno();
		public DateTecno DataCancelamento
		{
			get { return _dataCancelamento; }
			set { _dataCancelamento = value; }
		}

		public string iufNumero { get; set; }
		public String Fiscalizacao { get; set; }
		public string ProcNumero { get; set; }
		public String Situacao { get; set; }
		public String InformacoesComplementares { get; set; }
		public Int32? ParcelaPaiId { get; set; }
		public CobrancaDUA ParcelaPai { get; set; }
		public Int32 ParcelamentoId { get; set; }
		#endregion
	}
}
