using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Arquivo;

namespace Tecnomapas.EtramiteX.WindowsService.SVCEmail.Entities
{
	class Email
	{
		public int Id { get; set; }
		public int Codigo { get; set; }
		public int Situacao { get; set; }
		public int Tipo { get; set; }
		public String Assunto { get; set; }
		public String Destinatario { get; set; }
		public String Texto { get; set; }
		public int NumTentativas { get; set; }

		private List<Arquivo> _anexos = new List<Arquivo>();
		public List<Arquivo> Anexos
		{
			get { return _anexos; }
			set { _anexos = value; }
		}
	}
}
