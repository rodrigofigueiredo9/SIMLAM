using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;

namespace Tecnomapas.EtramiteX.Publico.ViewModels.VMAgrotoxico
{
	public class ListarVM
	{
		public String UltimaBusca { get; set; }
		public List<SelectListItem> Situacao { get; set; }
		public List<SelectListItem> ClasseUso { get; set; }
		public List<SelectListItem> ModalidadeAplicacao { get; set; }
		public List<SelectListItem> GrupoQuimico { get; set; }
		public List<SelectListItem> ClassificacaoToxicologica { get; set; }
		public Paginacao Paginacao { get; set; }
		public AgrotoxicoFiltro Filtros { get; set; }
		public List<AgrotoxicoFiltro> Resultados { get; set; }
		public ListarVM(List<QuantPaginacao> quantPaginacao, List<Lista> classes, List<Lista> modalidades, List<Lista> grupos, List<Lista> classificacoes, List<Lista> situacao)
		{
			Paginacao = new Blocos.Entities.Etx.ModuloCore.Paginacao();
			Situacao = ViewModelHelper.CriarSelectList(situacao, true);
			ClasseUso = ViewModelHelper.CriarSelectList(classes, true);
			ModalidadeAplicacao = ViewModelHelper.CriarSelectList(modalidades, true);
			GrupoQuimico = ViewModelHelper.CriarSelectList(grupos, true);
			ClassificacaoToxicologica = ViewModelHelper.CriarSelectList(classificacoes, true);

			Filtros = new AgrotoxicoFiltro();
			UltimaBusca = string.Empty;
			Resultados = new List<AgrotoxicoFiltro>();

			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}
		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}

		public ListarVM()
		{
			Paginacao = new Blocos.Entities.Etx.ModuloCore.Paginacao();
			Resultados = new List<AgrotoxicoFiltro>();
		}
	}
}