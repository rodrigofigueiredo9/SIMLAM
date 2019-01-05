using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using System.Web;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloDUA.Business
{
	public class DuaValidar
	{
		#region Propriedades

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public DuaValidar()
		{

		}

		#endregion

	}
}