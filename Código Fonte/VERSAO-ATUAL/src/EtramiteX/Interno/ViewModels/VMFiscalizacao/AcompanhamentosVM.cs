using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class AcompanhamentosVM
	{
		public Int32 fiscalizacaoId { get; set; }
		public Boolean PodeCriar { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeExcluir { get; set; }
		public Boolean PodeAlterarSituacao { get; set; }
		public List<Acompanhamento> Resultados { get; set; }

		public AcompanhamentosVM()
		{
			Resultados = new List<Acompanhamento>();
		}
	}
}