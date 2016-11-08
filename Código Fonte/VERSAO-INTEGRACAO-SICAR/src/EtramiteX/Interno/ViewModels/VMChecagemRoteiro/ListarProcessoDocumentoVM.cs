using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMChecagemRoteiro
{
	public class ListarVM
	{
		public List<SelectListItem> ListaQuantPaginacao { get; set; }
		public List<SelectListItem> ListaAtividadeSolicitadas { get; set; }
		public List<SelectListItem> ListaSituacaoAtividades { get; set; }
		public List<SelectListItem> ListaTiposProcesso { get; set; }
		public Paginacao Paginacao { get; set; }
		public ListarProtocoloFiltro Filtros { get; set; }
		public List<Processo> Resultados { get; set; }
		
		public String UltimaBusca { get; set; }
		public Boolean PodeExcluir { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeAssociar { get; set; }

		public ListarVM()
		{
			ListaQuantPaginacao = new List<SelectListItem>();
			Paginacao = new Paginacao();
			Filtros = new ListarProtocoloFiltro();
			Resultados = new List<Processo>();
		}

		public ListarVM(List<QuantPaginacao> quantPaginacao)
		{
			ListaQuantPaginacao = new List<SelectListItem>();
			Paginacao = new Paginacao();
			Filtros = new ListarProtocoloFiltro();
			Resultados = new List<Processo>();

			ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, List<ProcessoAtividadeItem> atividadeSolicitada, List<Situacao> situacaoAtividadeSolicitada, List<ProtocoloTipo> listaTiposProcesso)
		{
			ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
			ListaAtividadeSolicitadas = ViewModelHelper.CriarSelectList(atividadeSolicitada, true, true);
			ListaSituacaoAtividades = ViewModelHelper.CriarSelectList(situacaoAtividadeSolicitada, true, true);
			ListaTiposProcesso = ViewModelHelper.CriarSelectList(listaTiposProcesso, true, true);
		}

		public void SetResultados(List<Processo> resultados)
		{
			Resultados = resultados;
		}
	}
}