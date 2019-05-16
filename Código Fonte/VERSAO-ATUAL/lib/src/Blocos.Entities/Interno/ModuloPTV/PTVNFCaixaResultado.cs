using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV
{
	public class PTVNFCaixaResultado
	{
		public string NumeroPTV { get; set; }
		public string Empreendimento { get; set; }
		public string Origem { get; set; }
		public string Situacao { get; set; }
		public string Interessado { get; set; }
		public int QtdCaixa { get; set; }
		public DateTime DataEmissao { get; set; }
	}
}