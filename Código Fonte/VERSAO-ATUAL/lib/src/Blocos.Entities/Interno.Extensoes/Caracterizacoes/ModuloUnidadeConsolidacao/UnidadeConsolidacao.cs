using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao
{
	public class UnidadeConsolidacao
	{
		public Int32 Id { get; set; }
		public Boolean PossuiCodigoUC { get; set; }
		public Int64 CodigoUC { get; set; }
		public String LocalLivroDisponivel { get; set; }
		public String TipoApresentacaoProducaoFormaIdentificacao { get; set; }
		public String Tid { get; set; }
		public List<Cultivar> Cultivares { get; set; }
		public List<ResponsavelTecnico> ResponsaveisTecnicos { get; set; }
		public EmpreendimentoCaracterizacao Empreendimento { get; set; }
		public Int32 InternoId { get; set; }
		public String InternoTid { get; set; }
		public Int32 CredenciadoID { get; set; }

		public UnidadeConsolidacao()
		{
			ResponsaveisTecnicos = new List<ResponsavelTecnico>();
			Cultivares = new List<Cultivar>();
			Empreendimento = new EmpreendimentoCaracterizacao();
		}
	}
}