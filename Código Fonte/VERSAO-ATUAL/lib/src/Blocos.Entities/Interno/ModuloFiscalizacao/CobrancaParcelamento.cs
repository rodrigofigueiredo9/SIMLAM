using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class CobrancaParcelamento
	{
		#region Constructor
		public CobrancaParcelamento()
		{
			DUAS = new List<CobrancaDUA>();
		}

		public CobrancaParcelamento(Fiscalizacao fiscalizacao, DateTime dataVencimento)
		{
			ValorMulta = fiscalizacao.Multa.ValorMulta;
			ValorMultaAtualizado = fiscalizacao.Multa.ValorMulta;
			DataEmissao = new DateTecno() { Data = DateTime.Now };
			Data1Vencimento = new DateTecno() { Data = dataVencimento };
			DUAS = new List<CobrancaDUA>();
		}
		#endregion

		#region Properties
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 CobrancaId { get; set; }
		public Int32 QuantidadeParcelas { get; set; }
		public Decimal ValorMulta { get; set; }
		public Decimal ValorMultaAtualizado { get; set; }

		private DateTecno _data1Vencimento = new DateTecno();
		public DateTecno Data1Vencimento
		{
			get { return _data1Vencimento; }
			set { _data1Vencimento = value; }
		}

		private DateTecno _dataEmissao = new DateTecno();
		public DateTecno DataEmissao
		{
			get { return _dataEmissao; }
			set { _dataEmissao = value; }
		}

		public List<CobrancaDUA> DUAS { get; set; }
		#endregion
	}
}