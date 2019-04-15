using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class TituloRelatorioFiltro
	{
		public int modelo{ get; set; }
		public string inicioPeriodo { get; set; }
		public string fimPeriodo { get; set; }
		public string nomeRazaoSocial { get; set; }
		public string cpfCnpj { get; set; }
		public bool isCpf { get; set; }
		public int municipio { get; set; }
		
		public TituloRelatorioFiltro()
		{
		}
	}
}