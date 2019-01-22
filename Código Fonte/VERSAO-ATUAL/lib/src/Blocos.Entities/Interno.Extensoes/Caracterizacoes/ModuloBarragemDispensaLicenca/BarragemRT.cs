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
	public class BarragemRT
	{
		public int id { get; set; }
		public eTipoRT tipo { get; set; }
		public string nome { get; set; }
		public string profissao { get; set; }
		public string registroCREA { get; set; }
		public Arquivo.Arquivo AutorizacaoCREA { get; set; }

		public BarragemRT() { }
	}
}