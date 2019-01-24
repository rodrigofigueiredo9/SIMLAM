using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca
{
	public class BarragemCoordenada
	{
		public int id { get; set; }
		public eTipoCoordenadaBarragem tipo { get; set; }
		public int northing { get; set; }
		public int easting { get; set; }

		public BarragemCoordenada() { }
	}
}