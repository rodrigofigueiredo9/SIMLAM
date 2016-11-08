using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMLiberacaoCFOCFOC
{
	public class ListarVM
	{
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private List<ListarResultados> _resultados = new List<ListarResultados>();
		public List<ListarResultados> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		private ListarFiltro _filtros = new ListarFiltro();
		public ListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		public List<SelectListItem> LstTipoNumero { get; set; }
		public List<SelectListItem> LstTipoDocumento { get; set; }
		public String UltimaBusca { get; set; }

		public ListarVM()
		{
			List<Lista> valores = new List<Lista>();
			valores.Add(new Lista() { Id = "1", Texto = "Bloco" });
			valores.Add(new Lista() { Id = "2", Texto = "Digital" });

			LstTipoNumero = ViewModelHelper.CriarSelectList(valores);

			valores = new List<Lista>();
			valores.Add(new Lista() { Id = "1", Texto = "CFO" });
			valores.Add(new Lista() { Id = "2", Texto = "CFOC" });

			LstTipoDocumento = ViewModelHelper.CriarSelectList(valores);
		}

		internal void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());

		}
	}
}