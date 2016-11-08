using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMProtocolo
{
	public class ListarVM
	{
		public List<SelectListItem> ListaAtividadeSolicitadas { get; set; }
		public List<SelectListItem> ListaSituacaoAtividades { get; set; }
		public List<SelectListItem> ListaTiposProcesso { get; set; }
		public List<SelectListItem> ListaMunicipios { get; set; }

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private ListarProtocoloFiltro _filtros = new ListarProtocoloFiltro();
		public ListarProtocoloFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<Protocolo> _resultados = new List<Protocolo>();
		public List<Protocolo> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}
		
		public String UltimaBusca { get; set; }
		public Boolean PodeExcluir { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeAssociar { get; set; }

		public ListarVM()
		{
		}

		public ListarVM(List<QuantPaginacao> quantPaginacao, List<ProcessoAtividadeItem> atividadeSolicitada, List<Situacao> situacaoAtividadeSolicitada, List<ProtocoloTipo> listaTiposProcesso, List<Municipio> listaMunicipios)
		{
			ListaAtividadeSolicitadas = ViewModelHelper.CriarSelectList(atividadeSolicitada, true, true);
			ListaSituacaoAtividades = ViewModelHelper.CriarSelectList(situacaoAtividadeSolicitada, true, true);
			ListaTiposProcesso = ViewModelHelper.CriarSelectList(listaTiposProcesso, true, true);
			ListaMunicipios = ViewModelHelper.CriarSelectList(listaMunicipios, true, true);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}