using System;
namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static AberturaLivroUnidadeConsolidacaoMsg _aberturaLivroUnidadeConsolidacao = new AberturaLivroUnidadeConsolidacaoMsg();
		public static AberturaLivroUnidadeConsolidacaoMsg AberturaLivroUnidadeConsolidacao
		{
			get { return _aberturaLivroUnidadeConsolidacao; }
			set { _aberturaLivroUnidadeConsolidacao = value; }
		}
	}

	public class AberturaLivroUnidadeConsolidacaoMsg
	{
		public Mensagem TotalPaginasLivroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_TotalPaginasLivro", Texto = "Total de páginas do livro é obrigatório." }; } }
		public Mensagem TotalPaginasLivroInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_TotalPaginasLivro", Texto = "Total de páginas do livro é inválido." }; } }
		public Mensagem TotalPaginasLivroMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_TotalPaginasLivro", Texto = "Total de páginas do livro deve ser maior que zero." }; } }

		public Mensagem PaginaInicialObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_PaginaInicial", Texto = "Nº da página inicial é obrigatório." }; } }
		public Mensagem PaginaInicialInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_PaginaInicial", Texto = "Nº da página inicial é inválido." }; } }
		public Mensagem PaginaInicialMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_PaginaInicial", Texto = "Nº da página inicial deve ser maior que zero." }; } }

		public Mensagem PaginaFinalObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_PaginaFinal", Texto = "Nº da página final é obrigatório." }; } }
		public Mensagem PaginaFinalInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_PaginaFinal", Texto = "Nº da página final é inválido." }; } }
		public Mensagem PaginaFinalMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_PaginaFinal", Texto = "Nº da página final deve ser maior que zero." }; } }

		public Mensagem CaracterizacaoNaoCadastrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterização de UC do empreendimento deve estar preenchida" }; } }
		public Mensagem CulturaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_CulturaCultId", Texto = "Cultura é obrigatória." }; } }
		public Mensagem CulturaJaAdicionada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_CulturaCultId", Texto = "Cultura já foi adicionada." }; } }
		public Mensagem CulturaCultivarDesatualizada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_CulturaCultId", Texto = "A cultura não está mais adicionada na caracterização de UC." }; } }

		public Mensagem CulturaAdicionadaOutroTitulo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_CulturaCultId", Texto = "A cultura já está selecionada em outro título do mesmo modelo." }; } }
	}
}