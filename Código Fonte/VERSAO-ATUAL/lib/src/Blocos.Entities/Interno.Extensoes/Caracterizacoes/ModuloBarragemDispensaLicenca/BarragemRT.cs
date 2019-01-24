using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.ModuloProfissao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca
{
	public class BarragemRT
	{
		public int id { get; set; }
		public eTipoRT tipo { get; set; }
		public string nome { get; set; }
		public Profissao profissao { get; set; }
		public string registroCREA { get; set; }
		public string numeroART { get; set; }
		public Arquivo.Arquivo autorizacaoCREA { get; set; }

		public BarragemRT() { }
	}
}