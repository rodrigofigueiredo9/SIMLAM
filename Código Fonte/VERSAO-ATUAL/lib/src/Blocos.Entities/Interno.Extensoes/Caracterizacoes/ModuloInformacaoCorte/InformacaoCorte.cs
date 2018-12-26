using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte
{
	public class InformacaoCorte
	{
		public Int32 Id { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public EmpreendimentoCaracterizacao Emprendimento { get; set; }
		public String Tid { get; set; }

		public List<Dependencia> Dependencias { get; set; }

		private List<InformacaoCorteLicenca> _informacaoCorteLicenca = new List<InformacaoCorteLicenca>();
		public List<InformacaoCorteLicenca> InformacaoCorteLicenca
		{
			get { return _informacaoCorteLicenca; }
			set { _informacaoCorteLicenca = value; }
		}

		private InformacaoCorteInformacao _informacaoCorteInformacao = new InformacaoCorteInformacao();
		public InformacaoCorteInformacao InformacaoCorteInformacao
		{
			get { return _informacaoCorteInformacao; }
			set { _informacaoCorteInformacao = value; }
		}
	}
}
