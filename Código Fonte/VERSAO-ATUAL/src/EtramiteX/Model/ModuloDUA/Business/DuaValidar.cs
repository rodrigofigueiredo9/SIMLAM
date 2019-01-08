using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using System.Web;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloDUA.Business
{
	public class DuaValidar
	{
		#region Propriedades
		DuaDa _da = new DuaDa();
		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public DuaValidar()
		{

		}

		public void ExisteDuaTituloCorte(int titulo)
		{
			if (_da.ExisteDuaTitulo(titulo))
				Validacao.Add(Mensagem.Dua.ExisteDuaTitulo);
		}


		#endregion

	}
}