using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Antigo;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels
{
	public class InformacaoCorteInformacaoVM
	{
		public bool IsVisualizar { get; set; }

		private InformacaoCorteInformacao _entidade = new InformacaoCorteInformacao();
		public InformacaoCorteInformacao Entidade
		{
			get { return _entidade; }
			set { _entidade = value; }
		}

		private List<SelectListItem> _especieTipo = new List<SelectListItem>();
		public List<SelectListItem> EspecieTipo
		{
			get { return _especieTipo; }
			set { _especieTipo = value; }
		}

		private List<SelectListItem> _produtoTipo = new List<SelectListItem>();
		public List<SelectListItem> ProdutoTipo
		{
			get { return _produtoTipo; }
			set { _produtoTipo = value; }
		}

		private List<SelectListItem> _destinacaoTipo = new List<SelectListItem>();
		public List<SelectListItem> DestinacaoTipo
		{
			get { return _destinacaoTipo; }
			set { _destinacaoTipo = value; }
		}

		public InformacaoCorteInformacaoVM(InformacaoCorteInformacao entidade, List<Lista> especieTipo, List<Lista> produtoTipo, List<Lista> destinacaoTipo, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Entidade = entidade;

			EspecieTipo = ViewModelHelper.CriarSelectList(especieTipo, true, true);
			ProdutoTipo = ViewModelHelper.CriarSelectList(produtoTipo.Where(x => x.Id != ((int)eProduto.SemRendimento).ToString()).ToList(), true, true);
			DestinacaoTipo = ViewModelHelper.CriarSelectList(destinacaoTipo, true, true);
		}
	}
}