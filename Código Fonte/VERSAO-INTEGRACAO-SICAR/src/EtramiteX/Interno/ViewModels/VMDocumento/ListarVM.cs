using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento
{
	public class ListarVM
	{
		public List<SelectListItem> ListaAtividadeSolicitadas { get; set; }
		public List<SelectListItem> ListaSituacaoAtividades { get; set; }
		public List<SelectListItem> ListaMunicipios{ get; set; }
		public List<SelectListItem> ListaTiposDocumento { get; set; }
		public Paginacao Paginacao { get; set; }
		public ListarProtocoloFiltro Filtros { get; set; }
		public List<Protocolo> Resultados { get; set; }

		public String UltimaBusca { get; set; }
		public Boolean PodeExcluir { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeAssociar { get; set; }

		public ListarVM()
		{
			Paginacao = new Paginacao();
			Filtros = new ListarProtocoloFiltro();
			Resultados = new List<Protocolo>();
		}

		public ListarVM(List<QuantPaginacao> quantPaginacao, List<ProcessoAtividadeItem> atividadeSolicitada, List<Situacao> situacaoAtividadeSolicitada, List<ProtocoloTipo> listaTiposDocumento, List<Municipio> listaMunicipios)
		{
			Paginacao = new Paginacao();
			Filtros = new ListarProtocoloFiltro();
			Resultados = new List<Protocolo>();

			ListaAtividadeSolicitadas = ViewModelHelper.CriarSelectList(atividadeSolicitada, true, true);
			ListaSituacaoAtividades = ViewModelHelper.CriarSelectList(situacaoAtividadeSolicitada, true, true);
			ListaTiposDocumento = ViewModelHelper.CriarSelectList(listaTiposDocumento, true, true);
			ListaMunicipios = ViewModelHelper.CriarSelectList(listaMunicipios, true, true);
			
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}

		public String ObterJSon(Protocolo protocolo)
		{
			object objeto = new
			{
				@Id = protocolo.Id,
				@Numero = protocolo.Numero,
				@SituacaoId = protocolo.SituacaoId,
				@SituacaoTexto = protocolo.SituacaoTexto
			};

			return HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(objeto));
		}
	}
}