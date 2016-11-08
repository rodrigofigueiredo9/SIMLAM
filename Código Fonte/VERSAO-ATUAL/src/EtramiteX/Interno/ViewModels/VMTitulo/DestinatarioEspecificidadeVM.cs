using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade
{
	public class DestinatarioEspecificidadeVM
	{
		private List<DestinatarioEspecificidade> _destinatarios = new List<DestinatarioEspecificidade>();
		public List<DestinatarioEspecificidade> Destinatarios
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		private List<SelectListItem> _destinatariosLst = new List<SelectListItem>();
		public List<SelectListItem> DestinatariosLst
		{
			get { return _destinatariosLst; }
			set { _destinatariosLst = value; }
		}

		public bool IsVisualizar { get; set; }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					DestinatarioObrigatorio = Mensagem.Especificidade.DestinatarioObrigatorio("ddlDestinatarioEsp"),
					DestinatarioJaAdicionado = Mensagem.Especificidade.DestinatarioJaAdicionado
				});
			}
		}

		public DestinatarioEspecificidadeVM() {}

		internal void CarregarDestinatario(List<PessoaLst> tituloDestinatarios)
		{
			DestinatariosLst = ViewModelHelper.CriarSelectList(tituloDestinatarios);
		}
	}
}