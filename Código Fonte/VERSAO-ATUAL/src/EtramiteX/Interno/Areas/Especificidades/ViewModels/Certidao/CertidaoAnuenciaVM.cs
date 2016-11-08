using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certidao
{
	public class CertidaoAnuenciaVM
	{
		public bool IsVisualizar { get; set; }

		private CertidaoAnuencia _certidaoAnuencia = new CertidaoAnuencia();
		public CertidaoAnuencia CertidaoAnuencia
		{
			get { return _certidaoAnuencia; }
			set { _certidaoAnuencia = value; }
		}

		private DestinatarioEspecificidadeVM _destinatariosVM = new DestinatarioEspecificidadeVM();
		public DestinatarioEspecificidadeVM DestinatariosVM
		{
			get { return _destinatariosVM; }
			set { _destinatariosVM = value; }
		}

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		public CertidaoAnuenciaVM()
		{

		}

		public CertidaoAnuenciaVM(CertidaoAnuencia certidaoAnuencia, List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, 
			string processoDocumentoSelecionado = null, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			CertidaoAnuencia = certidaoAnuencia;
			DestinatariosVM.IsVisualizar = isVisualizar;			
			DestinatariosVM.DestinatariosLst = ViewModelHelper.CriarSelectList(destinatarios, true, true);
			DestinatariosVM.Destinatarios = (certidaoAnuencia != null && certidaoAnuencia.Destinatarios != null)? certidaoAnuencia.Destinatarios : new List<DestinatarioEspecificidade>();
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, 0, isVisualizar);
		}
	}
}