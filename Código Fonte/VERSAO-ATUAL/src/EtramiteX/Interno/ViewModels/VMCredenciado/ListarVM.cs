using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado
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

        private List<SelectListItem> _listaSituacaoHab = new List<SelectListItem>();
        public List<SelectListItem> ListaSituacaoHab
        {
            get { return _listaSituacaoHab; }
            set { _listaSituacaoHab = value; }
        }

        private List<SelectListItem> _listaMotivoHab = new List<SelectListItem>();
        public List<SelectListItem> ListaMotivoHab
        {
            get { return _listaMotivoHab; }
            set { _listaMotivoHab = value; }
        }

		public String UltimaBusca { get; set; }

		public ListarVM()
		{
		}

		public ListarVM(List<QuantPaginacao> quantPaginacao, List<Situacao> tipoCredenciado, List<Situacao> situacaoCredenciado)
		{
			SetListItens(quantPaginacao, tipoCredenciado, situacaoCredenciado);
		}

        public ListarVM(List<QuantPaginacao> quantPaginacao, List<Situacao> tipoCredenciado, List<Situacao> situacaoCredenciado, List<Situacao> situacaoHabilitacao, List<Lista> motivosHabilitacao)
        {
            SetListItens(quantPaginacao, tipoCredenciado, situacaoCredenciado, situacaoHabilitacao, motivosHabilitacao);
        }

		public void SetListItens(List<QuantPaginacao> quantPaginacao, List<Situacao> tipoCredenciado, List<Situacao> situacaoCredenciado, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
			ListaSituacaoCred = ViewModelHelper.CriarSelectList(situacaoCredenciado, true, true);
			ListaTipoCred = ViewModelHelper.CriarSelectList(tipoCredenciado, true, true);
		}

        public void SetListItens(List<QuantPaginacao> quantPaginacao, List<Situacao> tipoCredenciado, List<Situacao> situacaoCredenciado, List<Situacao> situacaoHabilitacao, List<Lista> motivosHabilitacao, int quantidadePagina = 5)
        {
            Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
            ListaSituacaoCred = ViewModelHelper.CriarSelectList(situacaoCredenciado, true, true);
            ListaTipoCred = ViewModelHelper.CriarSelectList(tipoCredenciado, true, true);
            ListaSituacaoHab = ViewModelHelper.CriarSelectList(situacaoHabilitacao, true, true);
            ListaMotivoHab = ViewModelHelper.CriarSelectList(motivosHabilitacao, true, true);
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

		public bool PodeHabilitar { get; set; }

		public bool PodeVisualizar { get; set; }

		public bool PodeEditar { get; set; }

		public bool PodeAlterarSituacao { get; set; }

		public bool PodeRegerarChave { get; set; }

		public bool IsAssociar { get; set; }
	}
}