using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.Security
{
	public class ControleAcessoBus
	{
		#region Propriedades

		ControleAcessoDa _da = new ControleAcessoDa();

		private static EtramiteIdentity _user
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Acoes DML

		public bool Gerar(Controle controle) 
		{
			try
			{
				_da.Gerar(controle);

				return true;
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return false;
			
		}

		#endregion
	}
}
