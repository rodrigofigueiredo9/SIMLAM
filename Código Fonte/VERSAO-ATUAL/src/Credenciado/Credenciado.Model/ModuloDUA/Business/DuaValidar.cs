﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloDUA.Business
{
	public class DuaValidar
	{
		#region Propriedades

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public DuaValidar() {	}

		#endregion

	}
}