using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaATV
{
	public class SilviculturaATV
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public List<Dependencia> Dependencias { get; set; }
		public List<SilviculturaAreaATV> Areas { get; set; }

		public List<SilviculturaCaracteristicaATV> Caracteristicas { get; set; }

		public SilviculturaATV()
		{
			this.Dependencias = new List<Dependencia>();
			this.Caracteristicas = new List<SilviculturaCaracteristicaATV>();
			this.Areas = new List<SilviculturaAreaATV>();
			this.Tid = String.Empty;
		}
	}
}