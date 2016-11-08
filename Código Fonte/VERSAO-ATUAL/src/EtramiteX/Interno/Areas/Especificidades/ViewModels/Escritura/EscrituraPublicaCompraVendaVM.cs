using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEscritura;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Escritura
{
	public class EscrituraPublicaCompraVendaVM
	{
		public bool IsVisualizar { get; set; }

		private EscrituraPublicaCompraVenda _escritura = new EscrituraPublicaCompraVenda();
		public EscrituraPublicaCompraVenda Escritura
		{
			get { return _escritura; }
			set 
			{ 
				_escritura = value;
			}
		}

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private List<SelectListItem> _destinatarios = new List<SelectListItem>();
		public List<SelectListItem> Destinatarios
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		public EscrituraPublicaCompraVendaVM(EscrituraPublicaCompraVenda escritura, List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, 
			string processoDocumentoSelecionado = null, bool isVisualizar = false)
		{
			Escritura = escritura;
			IsVisualizar = isVisualizar;
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, 0, isVisualizar);
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true, true, Escritura.Destinatario.ToString());
		}
	}
}