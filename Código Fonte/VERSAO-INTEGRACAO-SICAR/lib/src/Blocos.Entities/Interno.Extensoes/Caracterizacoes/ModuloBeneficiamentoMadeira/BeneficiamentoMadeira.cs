using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBeneficiamentoMadeira
{
	public class BeneficiamentoMadeira
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 EmpreendimentoId { get; set; }

		private List<BeneficiamentoMadeiraBeneficiamento> _beneficiamentos = new List<BeneficiamentoMadeiraBeneficiamento>();
		public List<BeneficiamentoMadeiraBeneficiamento> Beneficiamentos
		{
			get { return _beneficiamentos; }
			set { _beneficiamentos = value; }
		}

		public List<Dependencia> Dependencias { get; set; }

		public BeneficiamentoMadeira()
		{

		}
	}
}
