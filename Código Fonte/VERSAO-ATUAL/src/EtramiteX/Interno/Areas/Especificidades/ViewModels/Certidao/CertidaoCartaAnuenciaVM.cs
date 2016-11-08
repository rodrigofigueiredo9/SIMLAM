using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certidao
{
	public class CertidaoCartaAnuenciaVM
	{
		public bool IsVisualizar { get; set; }

		private CertidaoCartaAnuencia _certidao = new CertidaoCartaAnuencia();
		public CertidaoCartaAnuencia Certidao
		{
			get { return _certidao; }
			set { _certidao = value; }
		}

		private DestinatarioEspecificidadeVM _destinatariosVM = new DestinatarioEspecificidadeVM();

		public DestinatarioEspecificidadeVM DestinatariosVM
		{
			get { return _destinatariosVM; }
			set { _destinatariosVM = value; }
		}

		private List<SelectListItem> _dominios = new List<SelectListItem>();
		public List<SelectListItem> Dominios
		{
			get { return _dominios; }
			set { _dominios = value; }
		}

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		public CertidaoCartaAnuenciaVM()
		{

		}

		public CertidaoCartaAnuenciaVM(CertidaoCartaAnuencia certidaoCartaAnuencia, List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, List<ListaValor> dominios, string processoDocumentoSelecionado = null, int atividadeId=0, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Certidao = certidaoCartaAnuencia;
			DestinatariosVM.IsVisualizar = isVisualizar;
			DestinatariosVM.Destinatarios = certidaoCartaAnuencia.Destinatarios ?? new List<DestinatarioEspecificidade>();
			DestinatariosVM.DestinatariosLst = ViewModelHelper.CriarSelectList(destinatarios, true, true);
			Dominios = ViewModelHelper.CriarSelectList(dominios, true, true, certidaoCartaAnuencia.Dominio.ToString());
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeId, isVisualizar);
			Atividades.MostrarBotoes = false;
		}
	}
}