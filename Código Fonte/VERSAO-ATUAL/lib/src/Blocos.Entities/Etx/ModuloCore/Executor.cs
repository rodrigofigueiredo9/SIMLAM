using System;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloCore
{
	public class Executor
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String Nome { get; set; }
		public String Login { get; set; }
		public eExecutorTipo Tipo { get; set; }

		private static Executor _current = null;
		public static Executor Current
		{
			get
			{
				if (HttpContext.Current != null)
				{
					EtramitePrincipal etramitePrincipal = (HttpContext.Current.User as EtramitePrincipal);

					if (etramitePrincipal == null)
					{
						return null;
					}

					EtramiteIdentity etramiteIdentity = etramitePrincipal.EtramiteIdentity;

					if (etramitePrincipal == null)
					{
						return null;
					}

					Executor executor = new Executor();
					executor.Id = etramiteIdentity.FuncionarioId;
					executor.Tid = etramiteIdentity.FuncionarioTid;
					executor.Nome = etramiteIdentity.Name;
					executor.Login = etramiteIdentity.Login;
					executor.Tipo = (eExecutorTipo)etramiteIdentity.ExecutorTipo;

					return executor;
				}
				else
				{
					return _current;
				}
			}
			set
			{
				_current = value;
			}
		}
	}
}
