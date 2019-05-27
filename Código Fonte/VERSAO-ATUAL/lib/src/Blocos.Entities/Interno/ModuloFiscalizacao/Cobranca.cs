using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public enum eSituacaoCobranca
	{
		EmAberto = 1,
		Atrasado = 2,
		Pago = 3,
		PagoParcial = 4,
	    Cancelado = 5
	}

	public class Cobranca
	{
		#region Constructor
		public Cobranca() { }

		public Cobranca(Fiscalizacao fiscalizacao, IProtocolo protocolo, Notificacao notificacao)
		{
			FiscalizacaoId = fiscalizacao.Id;
			NumeroFiscalizacao = fiscalizacao.Id;
			NumeroIUF = fiscalizacao.Multa.NumeroIUF ?? fiscalizacao.Infracao.NumeroAutoInfracaoBloco ?? fiscalizacao.NumeroAutos.ToString();
			SerieId = fiscalizacao.Multa.SerieId;
			SerieTexto = fiscalizacao.Multa.SerieTexto;
			DataEmissaoIUF = fiscalizacao.Multa.DataLavratura;
			AutuadoPessoa = fiscalizacao.AutuadoPessoa;
			AutuadoPessoaId = fiscalizacao.AutuadoPessoa.Id;
			CodigoReceitaId = fiscalizacao.Multa.CodigoReceitaId ?? 0;
			if (protocolo?.Id > 0)
			{
				ProcessoNumero = protocolo.Numero;
				NumeroAutuacao = protocolo.NumeroAutuacao;
			}

			if (notificacao.Id > 0)
			{
				Notificacao = notificacao;
				DataIUF = notificacao.DataIUF;
				DataJIAPI = notificacao.DataJIAPI;
				DataCORE = notificacao.DataCORE;
			}

			Parcelamentos = new List<CobrancaParcelamento>();
			Parcelamentos.Add(new CobrancaParcelamento(fiscalizacao));
		}
		#endregion

		#region Properties
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32? FiscalizacaoId { get; set; }
		public Int32? NumeroFiscalizacao { get; set; }
		public String ProcessoNumero { get; set; }
		public String NumeroAutuacao { get; set; }
		public String NumeroIUF { get; set; }
		public Int32? SerieId { get; set; }
		public String SerieTexto { get; set; }

		private DateTecno _dataEmissaoIUF = new DateTecno();
		public DateTecno DataEmissaoIUF
		{
			get { return _dataEmissaoIUF; }
			set { _dataEmissaoIUF = value; }
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

		public CobrancaParcelamento UltimoParcelamento { get; set; }
		public List<CobrancaParcelamento> Parcelamentos { get; set; }
		public List<Anexo> Anexos { get; set; }

		public bool ObterFiscalizacao { get; set; } = false;
		#endregion
	}
}