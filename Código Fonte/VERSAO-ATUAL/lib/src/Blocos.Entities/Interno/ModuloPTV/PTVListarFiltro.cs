using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV
{
	public class PTVListarFiltro
	{
		public string Numero { get; set; }
		public string DUANumero { get; set; }
		public bool DUAIsCPF { get; set; }
		public string DUACPFCNPJ { get; set; }
		public string Empreendimento { get; set; }
		public string Destinatario { get; set; }
		public Int32 Situacao { get; set; }
		public string CulturaCultivar { get; set; }
		public int FuncionarioId { get; set; }
		public int TipoDocumento { get; set; }
		public string NumeroDocumento { get; set; }
		public string Interessado { get; set; }
	}
}