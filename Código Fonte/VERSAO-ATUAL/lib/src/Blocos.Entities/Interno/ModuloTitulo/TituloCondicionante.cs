using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class TituloCondicionante
	{
		public int Id { get; set; }

		public string Tid { get; set; }

		public string Descricao { get; set; }
		public bool PossuiPrazo { get; set; }
		public int? Prazo { get; set; }	// dias
		public int? DiasProrrogados { get; set; }	// dias
		public bool PossuiPeriodicidade { get; set; }
		public int? PeriodicidadeValor { get; set; }
		public int Ordem { get; set; }
		public int tituloId { get; set; }
		public string tituloNumero { get; set; }

		private DateTecno _dataCriacao = new DateTecno();
		public DateTecno DataCriacao { get { return _dataCriacao; } set { _dataCriacao = value; } }

		private DateTecno _dataInicioPrazo = new DateTecno();
		public DateTecno DataInicioPrazo { get { return _dataInicioPrazo; } set { _dataInicioPrazo = value; } }

		private DateTecno _dataVencimento = new DateTecno();
		public DateTecno DataVencimento { get { return _dataVencimento; } set { _dataVencimento = value; } }

		private TituloCondicionanteSituacao _situacao = new TituloCondicionanteSituacao();
		public TituloCondicionanteSituacao Situacao { get { return _situacao; } set { _situacao = value; } }

		private List<TituloCondicionantePeriodicidade> _periodicidades = new List<TituloCondicionantePeriodicidade>();
		public List<TituloCondicionantePeriodicidade> Periodicidades
		{
			get { return _periodicidades; }
			set { _periodicidades = value; }
		}

		private TituloCondicionantePeriodTipo _periodicidadeTipo = new TituloCondicionantePeriodTipo();
		public TituloCondicionantePeriodTipo PeriodicidadeTipo { get { return _periodicidadeTipo; } set { _periodicidadeTipo = value; } }
	}
}