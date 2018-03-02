using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class Parametrizacao
	{
		#region Constructor
		public Parametrizacao() { }

		public Parametrizacao(Fiscalizacao fiscalizacao)
		{
			CodigoReceitaId = fiscalizacao.Multa.CodigoReceitaId ?? 0;
		}
		#endregion

		#region Properties
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 CodigoReceitaId { get; set; }
		public String CodigoReceitaTexto { get; set; }
		private DateTecno _inicioVigencia = new DateTecno();
		public DateTecno InicioVigencia
		{
			get { return _inicioVigencia; }
			set { _inicioVigencia = value; }
		}
		private DateTecno _fimVigencia = new DateTecno();
		public DateTecno FimVigencia
		{
			get { return _fimVigencia; }
			set { _fimVigencia = value; }
		}
		public Int32 MaximoParcelas { get; set; }
		public Int32 ValorMinimoPF { get; set; }
		public Int32 ValorMinimoPJ { get; set; }
		public Int32 MultaPercentual { get; set; }
		public Int32 JurosPercentual { get; set; }
		public Int32 DescontoPercentual { get; set; }
		public Int32 PrazoDescontoUnidade { get; set; }
		public Int32 PrazoDescontoDecorrencia { get; set; }
		#endregion
	}
}