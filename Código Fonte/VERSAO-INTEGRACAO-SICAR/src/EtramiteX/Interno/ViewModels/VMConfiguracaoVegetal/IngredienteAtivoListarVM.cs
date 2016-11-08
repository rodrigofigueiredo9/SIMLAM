using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMConfiguracaoVegetal
{
	public class IngredienteAtivoListarVM
	{
		public String UltimaBusca { get; set; }
		public Boolean PodeAssociar { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeAlterarSituacao { get; set; }

		private List<SelectListItem> _situacoesLista = new List<SelectListItem>();
		public List<SelectListItem> SituacoesLista
		{
			get { return _situacoesLista; }
			set { _situacoesLista = value; }
		}

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private ConfiguracaoVegetalItem _filtros = new ConfiguracaoVegetalItem();
		public ConfiguracaoVegetalItem Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<ConfiguracaoVegetalItem> _resultados = new List<ConfiguracaoVegetalItem>();
		public List<ConfiguracaoVegetalItem> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		public IngredienteAtivoListarVM() { }

		public IngredienteAtivoListarVM(List<QuantPaginacao> quantPaginacao, List<Situacao> situacoes)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
			SituacoesLista = ViewModelHelper.CriarSelectList(situacoes, true);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}

		public String ObterJSon(ConfiguracaoVegetalItem item)
		{
			object objeto = new
			{
				Id = item.Id,
				Texto = item.Texto
			};

			return HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(objeto));
		}
	}
}