using System;
using System.Collections.Generic;
using System.Linq;
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

		private List<FinalidadeExploracao> _finalidades = new List<FinalidadeExploracao>();
		public List<FinalidadeExploracao> Finalidades
		{
			get { return _finalidades; }
			set { _finalidades = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@FinalidadeExploracaoEspecificarObrigatorio = Mensagem.ExploracaoFlorestal.FinalidadeExploracaoEspecificarObrigatorio
				});
			}
		}

		public ExploracaoFlorestalVM(ExploracaoFlorestal caracterizacao, List<FinalidadeExploracao> finalidades, List<Lista> classificacoesVegetais, List<Lista> exploracaoTipos, List<Lista> produtos, bool isVisualizar = false)
		{
			// passa o item "Outros" para a ultiam posição
			FinalidadeExploracao finalidade = finalidades.SingleOrDefault(x => x.Texto == "Outros");
			if (finalidade != null)
			{
				finalidades.Remove(finalidade);
				finalidades.Add(finalidade);
			}

			Finalidades = finalidades;
			Caracterizacao = caracterizacao;
			IsVisualizar = isVisualizar;

			foreach (ExploracaoFlorestalExploracao exploracao in caracterizacao.Exploracoes)
			{
				ExploracaoFlorestalExploracaoVM exploracaoVM = new ExploracaoFlorestalExploracaoVM(exploracaoTipos, classificacoesVegetais, produtos, exploracao, isVisualizar);
				ExploracaoFlorestalExploracaoVM.Add(exploracaoVM);
			}
		}
	}
}