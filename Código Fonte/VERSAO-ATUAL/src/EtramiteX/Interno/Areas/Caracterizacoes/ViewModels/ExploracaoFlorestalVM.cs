using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class ExploracaoFlorestalVM
	{
		public Boolean IsVisualizar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }
		public Int32? FinalidadeExploracao { get; set; }

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@GeometriaTipoPonto = eExploracaoFlorestalGeometria.Ponto,
					@ProdutoLenha = eProduto.Lenha,
					@ProdutoToras = eProduto.Toras,
					@ProdutoToretes = eProduto.Toretes,
					@ProdutoMouroesEstacas = eProduto.MouroesEstacas,
					@ProdutoEscoras = eProduto.Escoras,
					@ProdutoPalmito = eProduto.Palmito,
					@ProdutoSemRendimento = eProduto.SemRendimento
				});
			}
		}

		private ExploracaoFlorestal _caracterizacao = new ExploracaoFlorestal();
		public ExploracaoFlorestal Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		private List<ExploracaoFlorestalExploracaoVM> _exploracaoFlorestalExploracaoVM = new List<ExploracaoFlorestalExploracaoVM>();
		public List<ExploracaoFlorestalExploracaoVM> ExploracaoFlorestalExploracaoVM
		{
			get { return _exploracaoFlorestalExploracaoVM; }
			set { _exploracaoFlorestalExploracaoVM = value; }
		}

		private List<SelectListItem> _tipoExploracao = new List<SelectListItem>();
		public List<SelectListItem> TipoExploracao
		{
			get { return _tipoExploracao; }
			set { _tipoExploracao = value; }
		}

		private List<SelectListItem> _codigoExploracao = new List<SelectListItem>();
		public List<SelectListItem> CodigoExploracao
		{
			get { return _codigoExploracao; }
			set { _codigoExploracao = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@FinalidadeExploracaoEspecificarObrigatorio = Mensagem.ExploracaoFlorestal.FinalidadeExploracaoEspecificarObrigatorio("")
				});
			}
		}

		public ExploracaoFlorestalVM(ExploracaoFlorestal caracterizacao, List<FinalidadeExploracao> finalidades, List<Lista> classificacoesVegetais,
			List<Lista> exploracaoTipos, List<Lista> produtos, List<Lista> tipoExploracao, bool isVisualizar = false)
		{
			Caracterizacao = caracterizacao;
			IsVisualizar = isVisualizar;
			TipoExploracao = ViewModelHelper.CriarSelectList(tipoExploracao, selecionado: caracterizacao.TipoAtividade.ToString());

			var codigoExploracao = new List<Lista>();
			if (caracterizacao.CodigoExploracao > 0)
			{
				codigoExploracao = new List<Lista>() {
				new Lista(){
					Id = caracterizacao.CodigoExploracao.ToString(),
					Texto = tipoExploracao.FirstOrDefault(x => x.Id == caracterizacao.TipoAtividade.ToString()).Texto.Substring(0, 3) + caracterizacao.CodigoExploracao.ToString().PadLeft(3, '0') }
				};
			}
			CodigoExploracao = ViewModelHelper.CriarSelectList(codigoExploracao, selecionado: caracterizacao.CodigoExploracao.ToString());

			foreach (ExploracaoFlorestalExploracao exploracao in caracterizacao.Exploracoes)
			{
				ExploracaoFlorestalExploracaoVM exploracaoVM = new ExploracaoFlorestalExploracaoVM(finalidades, exploracaoTipos, classificacoesVegetais, produtos, exploracao, isVisualizar);
				ExploracaoFlorestalExploracaoVM.Add(exploracaoVM);
			}
		}
	}
}