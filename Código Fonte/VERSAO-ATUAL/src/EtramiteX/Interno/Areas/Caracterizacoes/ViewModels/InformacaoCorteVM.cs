using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class InformacaoCorteVM
	{
		public bool IsVisualizar { get; set; }
		public String TextoAbrirModal { get; set; }
		public String TextoMerge { get; set; }
		public String AtualizarDependenciasModalTitulo { get; set; }

		private InformacaoCorte _caracterizacao = new InformacaoCorte();
		public InformacaoCorte Caracterizacao
		{
			get { return _caracterizacao; }
			set { _caracterizacao = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@EspecieTipoObrigatorio = Mensagem.InformacaoCorte.EspecieTipoObrigatorio,
					@EspecieDuplicada = Mensagem.InformacaoCorte.EspecieDuplicada,

					@EspecieEspecificarDuplicada = Mensagem.InformacaoCorte.EspecieEspecificarDuplicada,
					@EspecieEspecificarObrigatorio = Mensagem.InformacaoCorte.EspecieEspecificarObrigatorio,

					@EspecieArvoresOuAreaObrigatorio = Mensagem.InformacaoCorte.EspecieArvoresOuAreaObrigatorio,
					@EspecieArvoresIsoladasInvalido = Mensagem.InformacaoCorte.EspecieArvoresIsoladasInvalido,
					@EspecieArvoresIsoladasZero = Mensagem.InformacaoCorte.EspecieArvoresIsoladasZero,

					@EspecieAreaCorteInvalido = Mensagem.InformacaoCorte.EspecieAreaCorteInvalido,
					@EspecieAreaCorteZero = Mensagem.InformacaoCorte.EspecieAreaCorteZero,

					@EspecieIdadePlantioInvalido = Mensagem.InformacaoCorte.EspecieIdadePlantioInvalido,
					@EspecieIdadePlantioMaiorZero = Mensagem.InformacaoCorte.EspecieIdadePlantioMaiorZero,

					@ProdutoTipoObrigatorio = Mensagem.InformacaoCorte.ProdutoTipoObrigatorio,
					@ProdutoDuplicado = Mensagem.InformacaoCorte.ProdutoDuplicado,
					@ProdutoDestinacaoObrigatorio = Mensagem.InformacaoCorte.ProdutoDestinacaoObrigatorio,

					@ProdutoQuantidadeObrigatorio = Mensagem.InformacaoCorte.ProdutoQuantidadeObrigatorio,
					@ProdutoQuantidadeInvalido = Mensagem.InformacaoCorte.ProdutoQuantidadeInvalido,
					@ProdutoQuantidadeMaiorZero = Mensagem.InformacaoCorte.ProdutoQuantidadeMaiorZero
				});
			}
		}

		public InformacaoCorteVM(InformacaoCorte caracterizacao, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Caracterizacao = caracterizacao;
		}
		
	}
}