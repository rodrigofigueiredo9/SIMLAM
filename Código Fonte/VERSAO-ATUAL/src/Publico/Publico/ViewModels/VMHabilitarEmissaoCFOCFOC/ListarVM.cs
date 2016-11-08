using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Publico.ViewModels.VMHabilitarEmissaoCFOCFOC
{
	public class ListarVM
	{
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private List<ListarFiltro> _resultados = new List<ListarFiltro>();
		public List<ListarFiltro> Resultados
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

		public String ProfissaoTexto { get; set; }

		private List<SelectListItem> _listaSituacaoCred = new List<SelectListItem>();
		public List<SelectListItem> ListaSituacaoCred
		{
			get { return _listaSituacaoCred; }
			set { _listaSituacaoCred = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					RegerarChave = Mensagem.Credenciado.RegerarChave
				});
			}
		}

		private List<SelectListItem> _listaTipoCred = new List<SelectListItem>();
		public List<SelectListItem> ListaTipoCred
		{
			get { return _listaTipoCred; }
			set { _listaTipoCred = value; }
		}

		public String UltimaBusca { get; set; }

		public ListarVM()
		{
		}

		public ListarVM(List<QuantPaginacao> quantPaginacao, List<Situacao> tipoCredenciado, List<Situacao> situacaoCredenciado)
		{
			SetListItens(quantPaginacao, tipoCredenciado, situacaoCredenciado);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, List<Situacao> tipoCredenciado, List<Situacao> situacaoCredenciado, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
			ListaSituacaoCred = ViewModelHelper.CriarSelectList(situacaoCredenciado, true, true);
			ListaTipoCred = ViewModelHelper.CriarSelectList(tipoCredenciado, true, true);
		}

		public void SetResultados(List<ListarFiltro> resultados)
		{
			if (resultados != null && resultados.Count > 0)
			{
				for (int i = 0; i < resultados.Count; i++)
				{
					if (resultados[i].Situacao == 4)
					{
						resultados[i].SituacaoTexto = ListaSituacaoCred.Find(x => x.Value == "2").Text;
					}
				}
				Resultados = resultados;
			}
		}

		

		public bool PodeVisualizar { get; set; }
	}
}