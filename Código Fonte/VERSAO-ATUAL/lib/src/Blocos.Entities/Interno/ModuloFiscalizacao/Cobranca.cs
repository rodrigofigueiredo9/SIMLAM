using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class Cobranca
	{
		#region Constructor
		public Cobranca() { }

		public Cobranca(Fiscalizacao fiscalizacao)
		{
			NumeroFiscalizacao = fiscalizacao.Id;
			ProcessoNumero = fiscalizacao.ProcessoNumero;
			NumeroAutos = fiscalizacao.NumeroAutos;
			NumeroIUF = fiscalizacao.Multa.NumeroIUF;
			SerieId = fiscalizacao.Multa.SerieId;
			SerieTexto = fiscalizacao.Multa.SerieTexto;
			DataLavratura = fiscalizacao.Multa.DataLavratura;
			AutuadoPessoa = fiscalizacao.AutuadoPessoa;
			ValorMulta = fiscalizacao.Multa.ValorMulta;
		}
		#endregion

		#region Properties
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 NumeroFiscalizacao { get; set; }
		public String ProcessoNumero { get; set; }
		public Int32 NumeroAutos { get; set; }
		public String NumeroIUF { get; set; }
		public Int32? SerieId { get; set; }
		public String SerieTexto { get; set; }

		private DateTecno _dataLavratura = new DateTecno();
		public DateTecno DataLavratura
		{
			get { return _dataLavratura; }
			set { _dataLavratura = value; }
		}
		public Int32 AutuadoPessoaId { get; set; }
		public Pessoa AutuadoPessoa { get; set; }

		public Notificacao Notificacao { get; set; }

		private DateTecno _dataIUF = new DateTecno();
		public DateTecno DataIUF
		{
			get { return _dataIUF; }
			set { _dataIUF = value; }
		}

		private DateTecno _dataJIAPI = new DateTecno();
		public DateTecno DataJIAPI
		{
			get { return _dataJIAPI; }
			set { _dataJIAPI = value; }
		}

		private DateTecno _dataCORE = new DateTecno();
		public DateTecno DataCORE
		{
			get { return _dataCORE; }
			set { _dataCORE = value; }
		}

		public Int32 CodigoReceitaId { get; set; }
		public String CodigoReceitaTexto { get; set; }
		public Decimal ValorMulta { get; set; }
		public Int32 QuantidadeParcelas { get; set; }

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

		public List<CobrancaParcelamento> Parcelamentos { get; set; }
		#endregion
	}
}