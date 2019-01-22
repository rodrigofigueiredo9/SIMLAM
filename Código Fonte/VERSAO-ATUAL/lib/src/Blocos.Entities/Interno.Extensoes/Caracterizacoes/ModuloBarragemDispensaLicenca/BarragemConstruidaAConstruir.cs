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
	public class BarragemConstruidaAConstruir
	{
		public int id { get; set; }
		public bool isSupressaoAPP { get; set; }
		public int isDemarcacaoAPP { get; set; }
		public decimal larguraDemarcada { get; set; }
		public bool larguraDemarcadaLegislacao { get; set; }
		public int faixaCercada { get; set; }
		public string descricacaoDesenvolvimentoAPP { get; set; }
		public bool barramentoNormas { get; set; }
		public decimal barramentoAdequacoes { get; set; }

		public int vazaoMinTipo { get; set; }
		public decimal vazaoMinDiametro { get; set; }
		public bool vazaoMinInstalado { get; set; }
		public decimal vazaoMinAdequacoes { get; set; }

		public int vazaoMaxTipo { get; set; }
		public decimal vazaoMaxDiametro { get; set; }
		public bool vazaoMaxInstalado { get; set; }
		public decimal vazaoMaxAdequacoes { get; set; }

		public BarragemConstruidaAConstruir() { }
	}
}