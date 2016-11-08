using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.EtramiteX.WindowsService.SVCEmail.Entities
{
	class ConfiguracaoEmail
	{
		public String SmtpServer { get; set; }
		public String SmtpUser { get; set; }
		public String SmtpSenha { get; set; }
		public String Remetente { get; set; }
		public int NumeroTentativaEnvio { get; set; }
		public int Porta { get; set; }
		public bool EnableSsl { get; set; }

		public ConfiguracaoEmail()
		{
			Porta = 25;
			EnableSsl = false;
		}
	}
}
