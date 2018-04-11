using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class CobrancasResultado
	{
		#region Constructor
		public CobrancasResultado() { }
		#endregion

		#region Properties
		public String Fiscalizacao { get; set; }
		public string ProcNumero { get; set; }
		public string NomeRazaoSocial { get; set; }
		public string NumeroIUF { get; set; }

		private DateTecno _dataEmissao = new DateTecno();
		public DateTecno DataEmissao
		{
			get { return _dataEmissao; }
			set { _dataEmissao = value; }
		}

		public Decimal ValorMulta { get; set; }
		public Decimal ValorMultaAtualizado { get; set; }
		public Decimal ValorPago { get; set; }
		public String Situacao { get; set; }
		#endregion
	}
}
