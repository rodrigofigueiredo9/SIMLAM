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
		public bool ?isSupressaoAPP { get; set; }
		public string isSupressaoAPPTexto
		{
			get
			{
				if (isSupressaoAPP == true) return "Sim";
				else if (isSupressaoAPP == false) return "Não";
				else return string.Empty;
			}
		}
		public int? isDemarcacaoAPP { get; set; }
		public string isDemarcacaoAPPTexto
		{
			get
			{
				if (isDemarcacaoAPP == 0) return "Não";
				else if (isDemarcacaoAPP == 1) return "Sim";
				else if (isDemarcacaoAPP == 2) return "Não se aplica";
				else return string.Empty;
			}
		}
		public decimal? larguraDemarcada { get; set; }
		public bool ?larguraDemarcadaLegislacao { get; set; }
		public string larguraDemarcadaLegislacaoTexto
		{
			get
			{
				if (larguraDemarcadaLegislacao == false) return "Não";
				else if (larguraDemarcadaLegislacao == true) return "Sim";
				else return string.Empty;
			}
		}
		public int ?faixaCercada { get; set; }
		public string faixaCercadaTexto {
			get
			{
				if (faixaCercada == 0) return "Não";
				else if (faixaCercada == 1) return "Sim";
				else if (faixaCercada == 2) return "Parcialmente";
				else return string.Empty;
			}
		}
		public string descricaoDesenvolvimentoAPP { get; set; }
		public bool ?barramentoNormas { get; set; }
		public string barramentoNormasTexto {
			get
			{
				if (barramentoNormas == true) return "Sim";
				else if (barramentoNormas == false) return "Não";
				else return string.Empty;
			}
		}
		public string barramentoAdequacoes { get; set; }

		public int vazaoMinTipo { get; set; }
		public string vazaoMinTipoTexto { get; set; }
		public decimal vazaoMinDiametro { get; set; }
		public bool ?vazaoMinInstalado { get; set; }
		public string vazaoMinInstaladoTexto
		{
			get
			{
				if (vazaoMinInstalado == true) return "Sim";
				else if (vazaoMinInstalado == false) return "Não";
				else return string.Empty;
			}
		}
		public bool ?vazaoMinNormas { get; set; }
		public string vazaoMinNormasTexto
		{
			get
			{
				if (vazaoMinNormas == true) return "Sim";
				else if (vazaoMinNormas == false) return "Não";
				else return string.Empty;
			}
		}
		public string vazaoMinAdequacoes { get; set; }

		public int vazaoMaxTipo { get; set; }
		public string vazaoMaxTipoTexto { get; set; }
		public string vazaoMaxDiametro { get; set; }
		public bool ?vazaoMaxInstalado { get; set; }
		public string vazaoMaxInstaladoTexto
		{
			get
			{
				if (vazaoMaxInstalado == true) return "Sim";
				else if (vazaoMaxInstalado == false) return "Não";
				return string.Empty;
			}
		}
		public bool ?vazaoMaxNormas { get; set; }
		public string vazaoMaxNormasTexto
		{
			get
			{
				if (vazaoMaxNormas == true) return "Sim";
				else if (vazaoMaxNormas == false) return "Não";
				return string.Empty;
			}
		}
		public string vazaoMaxAdequacoes { get; set; }

		public string periodoInicioObra { get; set; }
		public string periodoTerminoObra { get; set; }

		public BarragemConstruidaAConstruir() { }
	}
}