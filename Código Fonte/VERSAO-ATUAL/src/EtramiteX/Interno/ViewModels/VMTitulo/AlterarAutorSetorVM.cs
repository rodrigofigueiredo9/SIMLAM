using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class AlterarAutorSetorVM
	{
		public String AutorAtual { get; set; }
		public String AutorNovo { get; set; }
		public String SetorAtual { get; set; }

		public Boolean TrocarAutor { get; set; }
		public Boolean TrocarSetor { get; set; }

		private List<SelectListItem> _setores = new List<SelectListItem>();
		public List<SelectListItem> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

		public AlterarAutorSetorVM(){}

		public AlterarAutorSetorVM(List<Setor> setores, Titulo titulo, String autorNovo) 
		{
			this.AutorAtual = titulo.Autor.Nome;
			this.AutorNovo = autorNovo;
			this.SetorAtual = titulo.Setor.Nome;
			this.Setores = ViewModelHelper.CriarSelectList(setores, true, setores.Count > 1);
		}
	}
}