using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV
{
	public class ProdutoSemDocOrigem
	{
		public int id { get; set; }
		public string Produtor { get; set; }
		public string cpfCnpjProdutor { get; set; }
		public string enderecoEmpreendimento { get; set; }
		public string responsavel { get; set; }
		public string empreendimentoDenominador { get; set; }
		public int ufEndereco { get; set; }
		public int municipioEndereco { get; set; }
	}
}
