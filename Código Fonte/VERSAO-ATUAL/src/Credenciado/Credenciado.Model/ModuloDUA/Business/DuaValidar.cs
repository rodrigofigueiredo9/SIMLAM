using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloDUA.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloDUA.Business
{
	public class DuaValidar
	{
		#region Propriedades
		DuaDa _da = new DuaDa();

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public DuaValidar() {	}

		public void ExisteDuaTituloCorte(int titulo)
		{
			if (_da.ExisteDuaTitulo(titulo))
				Validacao.Add(Mensagem.Dua.ExisteDuaTitulo);
		}
		#endregion

	}
}