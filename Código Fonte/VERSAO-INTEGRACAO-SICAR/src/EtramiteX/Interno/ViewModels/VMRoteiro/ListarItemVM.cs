﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMRoteiro
{
	public class ListarItemVM
	{
		private Paginacao _paginacao = new Paginacao();
		public Paginacao Paginacao
		{
			get { return _paginacao; }
			set { _paginacao = value; }
		}

		private List<Item> _resultados = new List<Item>();
		public List<Item> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		private ListarFiltroItem _filtros = new ListarFiltroItem();
		public ListarFiltroItem Filtros
		{
			get { return _filtros; }
			set { _filtros = value; }
		}

		private List<SelectListItem> _listaTipos = new List<SelectListItem>();
		public List<SelectListItem> ListaTipos { get { return _listaTipos; } set { _listaTipos = value; } }

		public String UltimaBusca { get; set; }

		public bool PodeAssociar { get; set; }
		public bool PodeCadastrar { get; set; }
		public bool PodeEditar { get; set; }
		public bool PodeExcluir { get; set; }
		public bool PodeVisualizar { get; set; }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@ItemAdicionado = Mensagem.Item.ItemAdicionado,
					@ItemExistente = Mensagem.Item.ItemExistente,
					@ItemJaAdicionado = Mensagem.Item.ItemJaAdicionado
				});
			}
		}

		public ListarItemVM()
		{
		}

		public ListarItemVM(List<TipoItem> tipos, List<QuantPaginacao> quantPaginacao)
		{
			ListaTipos = ViewModelHelper.CriarSelectList(tipos.Where(x => x.Id != (int)eRoteiroItemTipo.ProjetoDigital).ToList(), true, true);
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false);
		}

		public void SetListItens(List<QuantPaginacao> quantPaginacao, int quantidadePagina = 5)
		{
			Paginacao.ListaQuantPaginacao = ViewModelHelper.CriarSelectList(quantPaginacao, false, false, selecionadoTexto: quantidadePagina.ToString());
		}
	}
}