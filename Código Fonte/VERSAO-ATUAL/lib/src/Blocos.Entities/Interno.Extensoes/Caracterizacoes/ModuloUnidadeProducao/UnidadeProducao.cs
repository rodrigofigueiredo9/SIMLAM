using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao
{
	public class UnidadeProducao
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String LocalLivroDisponivel { get; set; }
		public Boolean PossuiCodigoPropriedade { get; set; }
		public Int64 CodigoPropriedade { get; set; }
		public int InternoID { get; set; }
		public string InternoTID { get; set; }
		public int CredenciadoID { get; set; }
		public EmpreendimentoCaracterizacao Empreendimento { get; set; }
		public List<UnidadeProducaoItem> UnidadesProducao { get; set; }

		public UnidadeProducao()
		{
			Empreendimento = new EmpreendimentoCaracterizacao();
			UnidadesProducao = new List<UnidadeProducaoItem>();
		}
	}
}