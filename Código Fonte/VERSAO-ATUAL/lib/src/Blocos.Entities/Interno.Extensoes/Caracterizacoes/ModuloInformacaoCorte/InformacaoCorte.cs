using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte
{
	public class InformacaoCorte
	{
		public Int32 Id { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public EmpreendimentoCaracterizacao Empreendimento { get; set; }

		private DateTecno _dataInformacao = new DateTecno();
		public DateTecno DataInformacao { get { return _dataInformacao; } set { _dataInformacao = value; } }

		public Decimal AreaFlorestaPlantada { get; set; }

		private List<InformacaoCorteLicenca> _informacaoCorteLicenca = new List<InformacaoCorteLicenca>();
		public List<InformacaoCorteLicenca> InformacaoCorteLicenca
		{
			get { return _informacaoCorteLicenca; }
			set { _informacaoCorteLicenca = value; }
		}

		private List<InformacaoCorteTipo> _informacaoCorteTipo = new List<InformacaoCorteTipo>();
		public List<InformacaoCorteTipo> InformacaoCorteTipo
		{
			get { return _informacaoCorteTipo; }
			set { _informacaoCorteTipo = value; }
		}

		public List<Dependencia> Dependencias { get; set; }

		public int? InternoID { get; set; }
		public string InternoTID { get; set; }
		public String Tid { get; set; }

		public Decimal AreaCorteCalculada { get; set; }
	}
}
