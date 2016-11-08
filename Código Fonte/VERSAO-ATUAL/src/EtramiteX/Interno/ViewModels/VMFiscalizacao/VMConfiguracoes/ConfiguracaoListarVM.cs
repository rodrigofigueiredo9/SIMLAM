using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
	public class ConfiguracaoListarVM
	{
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private ConfigFiscalizacaoListarFiltro _filtros = new ConfigFiscalizacaoListarFiltro();
		public ConfigFiscalizacaoListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<SelectListItem> _classificacao = new List<SelectListItem>();
		public List<SelectListItem> Classificacao
		{
			get { return _classificacao; }
			set { _classificacao = value; }
		}

		private List<SelectListItem> _tipo = new List<SelectListItem>();
		public List<SelectListItem> Tipo
		{
			get { return _tipo; }
			set { _tipo = value; }
		}

		private List<SelectListItem> _item = new List<SelectListItem>();
		public List<SelectListItem> Item
		{
			get { return _item; }
			set { _item = value; }
		}

		private List<ConfigFiscalizacao> _resultados = new List<ConfigFiscalizacao>();
		public List<ConfigFiscalizacao> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public String UltimaBusca { get; set; }
		public Boolean PodeExcluir { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean PodeAssociar { get; set; }

		public ConfiguracaoListarVM() { }

		public ConfiguracaoListarVM(List<QuantPaginacao> quantPaginacao, List<Lista> classificacao, List<Lista> tipo, List<Lista> item)
		{
			this.Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
			this.Classificacao = ViewModelHelper.CriarSelectList(classificacao);
			this.Tipo = ViewModelHelper.CriarSelectList(tipo);
			this.Item = ViewModelHelper.CriarSelectList(item);			
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}