using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloEmail
{
	public class Email
	{
		public int Id { get; set; }
		public int Codigo { get; set; }
		public eEmailTipo Tipo { get; set; }
		public String Assunto { get; set; }
		public String Destinatario { get; set; }
		public String Texto { get; set; }

		private List<Arquivo.Arquivo> _anexos = new List<Arquivo.Arquivo>();
		public List<Arquivo.Arquivo> Anexos
		{
			get { return _anexos; }
			set { _anexos = value; }
		}
	}
}