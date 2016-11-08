using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class CondicionanteSituacaoAlterarVM
	{
		private Titulo _titulo = new Titulo();
		public Titulo Titulo { get { return _titulo; } set { _titulo = value; } }

		private List<SelectListItem> _tituloModelos = new List<SelectListItem>();
		public List<SelectListItem> TituloModelos { get { return _tituloModelos; } set { _tituloModelos = value; } }
		
		public CondicionanteSituacaoAlterarVM(List<TituloModeloLst> modelos) 
		{
			TituloModelos = ViewModelHelper.CriarSelectList(modelos, true);
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
				});
			}
		}
	}
}