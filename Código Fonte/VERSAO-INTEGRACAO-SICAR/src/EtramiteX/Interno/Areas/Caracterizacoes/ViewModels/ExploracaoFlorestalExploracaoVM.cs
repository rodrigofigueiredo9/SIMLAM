﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class ExploracaoFlorestalExploracaoVM
	{
		public bool IsVisualizar { get; set; }

		private List<SelectListItem> _exploracaoTipos = new List<SelectListItem>();
		public List<SelectListItem> ExploracaoTipos
		{
			get { return _exploracaoTipos; }
			set { _exploracaoTipos = value; }
		}

		private List<SelectListItem> _produtos = new List<SelectListItem>();
		public List<SelectListItem> Produtos
		{
			get { return _produtos; }
			set { _produtos = value; }
		}

		private ExploracaoFlorestalExploracao _exploracaoFlorestal = new ExploracaoFlorestalExploracao();
		public ExploracaoFlorestalExploracao ExploracaoFlorestal
		{
			get { return _exploracaoFlorestal; }
			set { _exploracaoFlorestal = value; }
		}

		private List<SelectListItem> _classificacoesVegetais = new List<SelectListItem>();
		public List<SelectListItem> ClassificacoesVegetais
		{
			get { return _classificacoesVegetais; }
			set { _classificacoesVegetais = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@ProdutoTipoObrigatorio = Mensagem.ExploracaoFlorestal.ProdutoTipoObrigatorio,
					@QuantidadeObrigatoria = Mensagem.ExploracaoFlorestal.QuantidadeObrigatoria,
					@QuantidadeMaiorZero = Mensagem.ExploracaoFlorestal.QuantidadeMaiorZero,
					@QuantidadeInvalida = Mensagem.ExploracaoFlorestal.QuantidadeInvalida,
					@ProdutoDuplicado = Mensagem.ExploracaoFlorestal.ProdutoDuplicado
				});
			}
		}

		public ExploracaoFlorestalExploracaoVM(List<Lista> exploracaoTipos, List<Lista> classificacoesVegetais, List<Lista> produtos, ExploracaoFlorestalExploracao exploracao, bool IsVisualizar = false)
		{
			int classifSelecionada = exploracao.ClassificacaoVegetacaoId;
			if (exploracao.GeometriaTipoId == (int)eExploracaoFlorestalGeometria.Poligono)
			{
				classificacoesVegetais = classificacoesVegetais.Where(x => x.Id != ((int)eExploracaoFlorestalClassificacaoVegetacao.ArvoresIsoladas).ToString()).ToList();
			}
			else
			{
				classificacoesVegetais = classificacoesVegetais.Where(x => x.Id == ((int)eExploracaoFlorestalClassificacaoVegetacao.ArvoresIsoladas).ToString()).ToList();
				classifSelecionada = (int)eExploracaoFlorestalClassificacaoVegetacao.ArvoresIsoladas;
			}

			ClassificacoesVegetais = ViewModelHelper.CriarSelectList(classificacoesVegetais, true, true, classifSelecionada.ToString());
			ExploracaoTipos = ViewModelHelper.CriarSelectList(exploracaoTipos, true, true, selecionado: exploracao.ExploracaoTipoId.ToString());
			Produtos = ViewModelHelper.CriarSelectList(produtos, true, true);
			ExploracaoFlorestal = exploracao;
			this.IsVisualizar = IsVisualizar;
		}
	}
}