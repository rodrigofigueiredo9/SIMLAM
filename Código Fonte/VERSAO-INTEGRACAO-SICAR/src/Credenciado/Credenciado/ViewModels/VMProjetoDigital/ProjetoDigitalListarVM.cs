using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProjetoDigital
{
	public class ProjetoDigitalListarVM
	{
		public Paginacao Paginacao { get; set; }
		public List<ProjetoDigital> Resultados { get; set; }
		public ProjetoDigitalListarFiltro Filtros { get; set; }

		public bool PodeVisualizar { get; set; }
		public bool PodeEditar { get; set; }
		public bool PodeExcluir { get; set; }
		public bool PodeAssociar { get; set; }
		public String UltimaBusca { get; set; }

		public ProjetoDigitalListarVM() : this(new List<QuantPaginacao>()) { }

		public ProjetoDigitalListarVM(List<QuantPaginacao> quantPaginacao)
		{
			Paginacao = new Paginacao();
			Resultados = new List<ProjetoDigital>();
			Filtros = new ProjetoDigitalListarFiltro();

			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}

		public String ObterJSon(ProjetoDigital item)
		{
			return HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(new { Id = item.Id, RequerimentoId = item.RequerimentoId, EmpreendimentoId = item.EmpreendimentoId, EmpreendimentoDenominador = item.EmpreendimentoTexto }));
		}
	}
}