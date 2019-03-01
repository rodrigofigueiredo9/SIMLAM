﻿using System;
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
		public bool ?isSupressaoAPP { get; set; }
		public string isSupressaoAPPTexto { get; set; }
		public int isDemarcacaoAPP { get; set; }
		public string isDemarcacaoAPPTexto { get; set; }
		public decimal larguraDemarcada { get; set; }
		public bool ?larguraDemarcadaLegislacao { get; set; }
		public string larguraDemarcadaLegislacaoTexto { get; set; }
		public int ?faixaCercada { get; set; }
		public string faixaCercadaTexto { get; set; }
		public string descricaoDesenvolvimentoAPP { get; set; }
		public bool ?barramentoNormas { get; set; }
		public string barramentoNormasTexto { get; set; }
		public string barramentoAdequacoes { get; set; }

		public int vazaoMinTipo { get; set; }
		public string vazaoMinTipoTexto { get; set; }
		public decimal vazaoMinDiametro { get; set; }
		public bool ?vazaoMinInstalado { get; set; }
		public string vazaoMinInstaladoTexto { get; set; }
		public bool ?vazaoMinNormas { get; set; }
		public string vazaoMinNormasTexto { get; set; }
		public string vazaoMinAdequacoes { get; set; }

		public int vazaoMaxTipo { get; set; }
		public string vazaoMaxTipoTexto { get; set; }
		public string vazaoMaxDiametro { get; set; }
		public bool ?vazaoMaxInstalado { get; set; }
		public string vazaoMaxInstaladoTexto { get; set; }
		public bool ?vazaoMaxNormas { get; set; }
		public string vazaoMaxNormasTexto { get; set; }
		public string vazaoMaxAdequacoes { get; set; }

		public string periodoInicioObra { get; set; }
		public string periodoTerminoObra { get; set; }

		public BarragemConstruidaAConstruir() { }
	}
}