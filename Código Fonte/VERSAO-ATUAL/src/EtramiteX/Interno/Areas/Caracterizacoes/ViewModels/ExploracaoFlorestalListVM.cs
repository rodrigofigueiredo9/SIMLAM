using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class ExploracaoFlorestalListVM
	{
		public Boolean IsVisualizar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

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
					@ProdutoSemRendimento = eProduto.SemRendimento,
					@ProdutoMudaPlanta = eProduto.MudaPlanta
				});
			}
		}
		private List<ExploracaoFlorestalVM> _exploracaoFlorestalVM = new List<ExploracaoFlorestalVM>();
		public List<ExploracaoFlorestalVM> ExploracaoFlorestalVM
		{
			get { return _exploracaoFlorestalVM; }
			set { _exploracaoFlorestalVM = value; }
		}

		public List<Dependencia> Dependencias { get; set; }

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

		public ExploracaoFlorestalListVM() { }
	}
}