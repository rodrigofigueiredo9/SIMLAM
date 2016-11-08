using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Certificado
{
	public class CadComercProdutosAgrotoxicosVM
	{
		public bool IsVisualizar { get; set; }

		private CadComercProdutosAgrotoxicos _cadastroComerciante = new CadComercProdutosAgrotoxicos();
		public CadComercProdutosAgrotoxicos CadastroComerciante
		{
			get { return _cadastroComerciante; }
			set { _cadastroComerciante = value; }
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

		public CadComercProdutosAgrotoxicosVM(CadComercProdutosAgrotoxicos cadastroComerciante,List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, 
			string processoDocumentoSelecionado = null, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			CadastroComerciante = cadastroComerciante;
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true, true, CadastroComerciante.Destinatario.ToString());
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, 0, isVisualizar);
		}
	}
}