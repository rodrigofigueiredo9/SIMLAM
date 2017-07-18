using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.WebService
{
	public class DUA
	{
		public string OrgaoCNPJ { get; set; }
		public string OrgaoSigla { get; set; }
		public string ServicoCodigo { get; set; }

		public string ReferenciaData { get; set; }
		public float ReceitaValor { get; set; }
		public string PagamentoCodigo { get; set; }
        public string CodigoServicoRef { get; set; }

        public string CPF { get; set; }
        public string CNPJ { get; set; }

        public float ValorTotal { get; set; }
	}
}