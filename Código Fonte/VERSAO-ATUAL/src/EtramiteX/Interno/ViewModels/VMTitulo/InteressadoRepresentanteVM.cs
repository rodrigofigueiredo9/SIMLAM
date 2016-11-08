using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class InteressadoRepresentanteVM
	{
		public Boolean IsVisualizar { get; set; }
		public Int32 InteressadoId { get; set; }
		public Int32 IdRelacionamento { get; set; }

		private List<SelectListItem> _representantes = new List<SelectListItem>();
		public List<SelectListItem> Representantes
		{
			get { return _representantes; }
			set { _representantes = value; }
		}

		public InteressadoRepresentanteVM() { }

		public InteressadoRepresentanteVM(List<PessoaLst> representantes, bool isVisualizar, Pessoa representante)
		{
			representante = representante ?? new Pessoa();
			if (representantes != null && representantes.Count > 0)
			{
				this.Representantes = ViewModelHelper.CriarSelectList(representantes, true, true, representante.Id.ToString());
			}
			else
			{
				this.InteressadoId = representante.Id;
			}

			this.IdRelacionamento = representante.IdRelacionamento.GetValueOrDefault();
			this.IsVisualizar = isVisualizar;
		}
	}
}