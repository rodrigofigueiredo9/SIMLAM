using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloAquicultura
{
	public class Aquicultura
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 EmpreendimentoId { get; set; }

		private List<AquiculturaAquicult> _aquiculturaAquicult = new List<AquiculturaAquicult>();
		public List<AquiculturaAquicult> AquiculturasAquicult
		{
			get { return _aquiculturaAquicult; }
			set { _aquiculturaAquicult = value; }
		}

		public List<Dependencia> Dependencias { get; set; }

		public Aquicultura(){}
	}
}
