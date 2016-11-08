using System;
using System.Security.Principal;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloSecurity
{
	public class EtramiteIdentity : IIdentity
	{
		public EtramiteIdentity(string name, string login, string email, DateTime? dataUltimoLogon, 
			string ipUltimoLogon, int funcionarioId, int funcionarioTipo, String funcionarioTipoTexto, 
			String funcionarioTid, int usuarioId, int executorTipo)
		{
			Name = name;
			Login = login;
			Email = email;
			FuncionarioId = funcionarioId;
			FuncionarioTipo = funcionarioTipo;
			FuncionarioTipoTexto = funcionarioTipoTexto;
			DataUltimoLogon = dataUltimoLogon;
			IpUltimoLogon = ipUltimoLogon;
			FuncionarioTid = funcionarioTid;
			ExecutorTipo = executorTipo;
			UsuarioId = usuarioId;
		}

		public string AuthenticationType
		{
			get { return "Forms"; }
		}

		public bool IsAuthenticated { get { return true; } }

		public string Name { get; private set; }

		public string Login { get; private set; }

		public string Email { get; private set; }

		public DateTime? DataUltimoLogon { get; private set; }

		public string IpUltimoLogon { get; private set; }

		public int FuncionarioId { get; private set; }

		public int FuncionarioTipo { get; private set; }

		public string FuncionarioTipoTexto { get; private set; }

		public string FuncionarioTid { get; private set; }

		public int ExecutorTipo { get; private set; }
		
		public int UsuarioId { get; private set; }
	}
}
