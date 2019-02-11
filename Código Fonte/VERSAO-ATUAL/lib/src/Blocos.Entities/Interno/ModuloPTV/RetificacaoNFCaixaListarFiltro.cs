using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV
{
	public class RetificacaoNFCaixaListarFiltro
	{
		public string NumeroNFCaixa { get; set; }
		public string NumeroPTV { get; set; }
		public bool NFCaixaIsCPF { get; set; }
		public string NFCaixaCPFCNPJ { get; set; }
		public eTipoNotaFiscalDeCaixa tipoDeCaixa { get; set; }
    }
}