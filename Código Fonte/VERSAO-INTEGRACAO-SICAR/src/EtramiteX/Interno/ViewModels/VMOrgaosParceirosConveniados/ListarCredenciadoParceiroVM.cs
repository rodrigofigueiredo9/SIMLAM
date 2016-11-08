using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados
{
	public class ListarCredenciadoParceiroVM
    {
		private List<CredenciadoPessoa> _credenciados;
		public List<CredenciadoPessoa> Credenciados
		{
			get { return _credenciados; }
			set { _credenciados = value; }
		}

		public String ObterJSon(CredenciadoPessoa item)
		{
			
			return HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(new
			{
				Id = item.Id,
				Nome = item.Nome,
				Email = item.Email,
				Chave = item.Chave,
				OrgaoParceiroId = item.OrgaoParceiroId,
			}));
		}
        
    }
}