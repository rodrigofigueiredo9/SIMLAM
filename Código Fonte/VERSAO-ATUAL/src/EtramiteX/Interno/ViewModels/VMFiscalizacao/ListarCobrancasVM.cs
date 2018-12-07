using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class ListarCobrancasVM
	{
		#region Properties

		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private CobrancaListarFiltro _filtros = new CobrancaListarFiltro();
		public CobrancaListarFiltro Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<SelectListItem> _ParametrizacaoLst = new List<SelectListItem>();
		public List<SelectListItem> ParametrizacaoLst
		{
			get { return _ParametrizacaoLst; }
			set { _ParametrizacaoLst = value; }
		}

		private List<CobrancasResultado> _resultados = new List<CobrancasResultado>();
		public List<CobrancasResultado> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		private List<SelectListItem> _situacaoFiscalizacao = new List<SelectListItem>();
		public List<SelectListItem> SituacaoFiscalizacao
		{
			get { return _situacaoFiscalizacao; }
			set { _situacaoFiscalizacao = value; }
		}

		private List<SelectListItem> _situacaoCobranca = new List<SelectListItem>();
		public List<SelectListItem> SituacaoCobranca
		{
			get { return _situacaoCobranca; }
			set { _situacaoCobranca = value; }
		}

		public String UltimaBusca { get; set; }
		public Boolean PodeEditar { get; set; }
		public Boolean PodeVisualizar { get; set; }

		#endregion Properties

		public ListarCobrancasVM() { }

		public ListarCobrancasVM(List<QuantPaginacao> quantPaginacao, List<Lista> parametrizacaoLst, List<Lista> situacao)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
			ParametrizacaoLst = ViewModelHelper.CriarSelectList(parametrizacaoLst, true, true);
			SituacaoFiscalizacao = ViewModelHelper.CriarSelectList(situacao, true, true);
			SituacaoCobranca = GetListSituacao();
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5) =>
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());

		private List<SelectListItem> GetListSituacao()
		{
			var list = new List<SelectListItem>();
			list.Add(new SelectListItem() { Text = "Todos", Value = "0" });
			list.Add(new SelectListItem() { Text = "Em Aberto", Value = ((int)eSituacaoCobranca.EmAberto).ToString() });
			list.Add(new SelectListItem() { Text = "Atrasado", Value = ((int)eSituacaoCobranca.Atrasado).ToString() });
			list.Add(new SelectListItem() { Text = "Pago", Value = ((int)eSituacaoCobranca.Pago).ToString() });
			list.Add(new SelectListItem() { Text = "Pago Parcial", Value = ((int)eSituacaoCobranca.PagoParcial).ToString() });

			return list;
		}
	}
}