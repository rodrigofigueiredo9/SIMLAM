using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class AreasVM
	{
		public Boolean IsVisualizar { get; set; }

		private List<Area> _areas = new List<Area>();
		public List<Area> Areas
		{
			get { return _areas; }
			set { _areas = value; }
		}
	}
}