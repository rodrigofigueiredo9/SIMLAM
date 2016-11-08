using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certificado
{
	public class CertificadoCadastroProdutoAgrotoxicoVM
	{
		public bool IsVisualizar { set; get; }

		private List<SelectListItem> _destinatarios = new List<SelectListItem>();
		public List<SelectListItem> Destinatarios
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private CertificadoCadastroProdutoAgrotoxico _certificado = new CertificadoCadastroProdutoAgrotoxico();
		public CertificadoCadastroProdutoAgrotoxico Certificado
		{
			get { return _certificado; }
			set { _certificado = value; }
		}

		public CertificadoCadastroProdutoAgrotoxicoVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, CertificadoCadastroProdutoAgrotoxico termo, string processoDocumentoSelecionado, bool isVisualizar, int atividadeSelecionada)
		{
			IsVisualizar = isVisualizar;
			Certificado = termo;
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true, true, termo.DestinatarioId.ToString());
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			Atividades.MostrarBotoes = false;

		}

		public CertificadoCadastroProdutoAgrotoxicoVM() { }

	}
}