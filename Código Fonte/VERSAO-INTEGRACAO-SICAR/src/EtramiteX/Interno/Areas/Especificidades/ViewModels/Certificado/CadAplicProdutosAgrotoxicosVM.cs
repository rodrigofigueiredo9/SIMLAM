using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certificado
{
	public class CadAplicProdutosAgrotoxicosVM
	{
		public bool IsVisualizar { get; set; }

		private CadAplicProdutosAgrotoxicos _cadastroAplicadora = new CadAplicProdutosAgrotoxicos();
		public CadAplicProdutosAgrotoxicos CadastroAplicadora
		{
			get { return _cadastroAplicadora; }
			set { _cadastroAplicadora = value; }
		}

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

		public CadAplicProdutosAgrotoxicosVM(CadAplicProdutosAgrotoxicos cadastroAplicadora,List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, 
			string processoDocumentoSelecionado = null, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			CadastroAplicadora = cadastroAplicadora;
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true, true, CadastroAplicadora.Destinatario.ToString());
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, 0, isVisualizar);
		}
	}
}