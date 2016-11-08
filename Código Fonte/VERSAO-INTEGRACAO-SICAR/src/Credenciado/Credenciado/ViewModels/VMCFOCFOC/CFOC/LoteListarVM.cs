using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC.Lote;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCFOCFOC.CFOC
{
	public class LoteListarVM
	{
		public Paginacao Paginacao { get; set; }
		public List<Lote> Resultados { get; set; }
		public Lote Filtros { get; set; }
		public List<SelectListItem> SituacaoLista { get; set; }

		public String UltimaBusca { get; set; }
		public bool PodeVisualizar { get; set; }
		public bool PodeEditar { get; set; }
		public bool PodeExcluir { get; set; }
		public bool PodeAssociar { get; set; }

		public LoteListarVM() : this(new List<QuantPaginacao>(), new List<Lista>()) { }

		public LoteListarVM(List<QuantPaginacao> quantPaginacao, List<Lista> situacoes)
		{
			Paginacao = new Paginacao();
			Resultados = new List<Lote>();
			Filtros = new Lote();

			SituacaoLista = ViewModelHelper.CriarSelectList(situacoes, true, true);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}