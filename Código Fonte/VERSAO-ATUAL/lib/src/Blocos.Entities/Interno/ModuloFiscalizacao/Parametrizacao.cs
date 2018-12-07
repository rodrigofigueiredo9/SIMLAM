using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public enum eDecorrencia
	{
		Dia = 1,
		Mes = 2,
		Ano = 3
	}

	public class Parametrizacao
	{
		#region Constructor
		public Parametrizacao()
		{
			ParametrizacaoDetalhes = new List<ParametrizacaoDetalhe>();
		}

		public Parametrizacao(Fiscalizacao fiscalizacao)
		{
			CodigoReceitaId = fiscalizacao.Multa.CodigoReceitaId ?? 0;
			ParametrizacaoDetalhes = new List<ParametrizacaoDetalhe>();
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
		public Int32 ValorMinimoPF { get; set; }
		public Int32 ValorMinimoPJ { get; set; }
		public Int32 MultaPercentual { get; set; }
		public Int32 JurosPercentual { get; set; }
		public Int32 DescontoPercentual { get; set; }
		public Int32 PrazoDescontoUnidade { get; set; }
		public Int32 PrazoDescontoDecorrencia { get; set; }
		public List<ParametrizacaoDetalhe> ParametrizacaoDetalhes { get; set; }
		#endregion
	}
}