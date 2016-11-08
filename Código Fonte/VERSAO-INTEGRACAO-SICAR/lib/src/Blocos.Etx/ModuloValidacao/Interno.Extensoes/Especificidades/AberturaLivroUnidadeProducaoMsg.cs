using System;
namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static AberturaLivroUnidadeProducaoMsg _aberturaLivroUnidadeProducao = new AberturaLivroUnidadeProducaoMsg();
		public static AberturaLivroUnidadeProducaoMsg AberturaLivroUnidadeProducao
		{
			get { return _aberturaLivroUnidadeProducao; }
			set { _aberturaLivroUnidadeProducao = value; }
		}
	}

	public class AberturaLivroUnidadeProducaoMsg
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

		public Mensagem UnidadeProducaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_UnidadeProducaoUnidadeId", Texto = "Unidade de produção é obrigatório." }; } }
		public Mensagem UnidadeProducaoJaAdicionada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_UnidadeProducaoUnidadeId", Texto = "Unidade de produção já foi adicionado." }; } }
		public Mensagem UnidadeProducaoUnidadeNaoAssociadaCaracterizacaoEmpreendimento(String unidadeCodigo) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_UnidadeProducaoUnidadeId", Texto = String.Format("A unidade de produção {0} não está mais adicionada na caracterização do empreendimento do título.", unidadeCodigo) }; }
		public Mensagem UnidadeProducaoUnidadePossuiTituloAssociado(String unidadeCodigo, String tituloNumero) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_UnidadeProducaoUnidadeId", Texto = String.Format("A unidade de produção {0} já está adicionada em outro título de nº {1}.", unidadeCodigo, tituloNumero) }; }
		public Mensagem UnidadeProducaoUnidadeResponsavelTecnicoNaoHabilitadoCFO { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_UnidadeProducaoUnidadeId", Texto = "O responsável técnico da UP deve estar habilitado para emissão de CFO." }; } }

	}
}
