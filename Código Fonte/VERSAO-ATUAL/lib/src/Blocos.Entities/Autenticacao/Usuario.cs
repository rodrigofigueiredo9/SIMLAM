

using System;

namespace Tecnomapas.Blocos.Entities.Autenticacao
{
	public class Usuario
	{
		public int Id { get; set; }
		public String Login { get; set; }
		public String Ip { get; set; }
		public DateTime? DataUltimoLogon { get; set; }
		public String IpUltimoLogon { get; set; }
		public String TID { get; set; }
		public DateTime? DataUltimaAlteracaoSenha { get; set; }
		public int TentativasErro { get; set; }
		public DateTime? DataUltimaAcao { get; set; }
		public String senhaHash { get; set; }
	}
}