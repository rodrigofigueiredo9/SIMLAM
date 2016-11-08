using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAtividade
{
	public class ListarAtividadeVM
	{
		public Paginacao Paginacao { get; set; }
		public AtividadeListarFiltro Filtros { get; set; }
		public List<AtividadeListarFiltro> Resultados { get; set; }
		public String UltimaBusca { get; set; }

		public List<SelectListItem> Setores { get; set; }
		public List<SelectListItem> Agrupadores { get; set; }
		public String AtividadesSolicitadas { get; set; }

		public ListarAtividadeVM()
		{
			Paginacao = new Paginacao();
			Filtros = new AtividadeListarFiltro();
			Resultados = new List<AtividadeListarFiltro>();
		}

		public ListarAtividadeVM(List<QuantPaginacao> quantPaginacao, List<Setor> setores, List<ProcessoAtividadeItem> atividades, List<AtividadeAgrupador> agrupadores)
		{
			Paginacao = new Paginacao();
			Filtros = new AtividadeListarFiltro();
			Resultados = new List<AtividadeListarFiltro>();
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
			Setores = ViewModelHelper.CriarSelectList(setores, true);
			Agrupadores = ViewModelHelper.CriarSelectList(agrupadores, true);
			AtividadesSolicitadas = ViewModelHelper.Json(atividades.Select(x => x.Texto).ToList());
		}

		public void SetResultados(List<AtividadeListarFiltro> resultados)
		{
			Resultados = resultados;
		}

		public String ObterJSon(AtividadeListarFiltro atividade)
		{
			object objeto = new
			{
				@Id = atividade.Id,
				@Nome = atividade.AtividadeNome,
				@SetorId = atividade.SetorId,
				@SetorTexto = atividade.SetorTexto
			};

			return HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(objeto));
		}
	}
}