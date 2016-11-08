namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static UnidadeConsolidacaoMsg _unidadeConsolidacaoMsg = new UnidadeConsolidacaoMsg();

		public static UnidadeConsolidacaoMsg UnidadeConsolidacao
		{
			get { return _unidadeConsolidacaoMsg; }
		}
	}

	public class UnidadeConsolidacaoMsg
	{
		public Mensagem UnidadeConsolidacaoSalvaSucesso { get { return new Mensagem() { Texto = "Caracterização Unidade de Consolidação salva com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem ExcluidoSucesso { get { return new Mensagem() { Texto = "Caracterização Unidade de Consolidação excluída com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Excluir a caracterização Unidade de Consolidação ?" }; } }
		public Mensagem Inexistente { get { return new Mensagem() { Texto = "Caracterização Unidade de Consolidação foi excluída.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem CodigoUCObrigatorio { get { return new Mensagem() { Campo = "CodigoPropriedade", Texto = "O código da UC é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CodigoUCInvalido { get { return new Mensagem() { Campo = "CodigoPropriedade", Texto = "O código da UC deve possuir 11 caracteres.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CodigoUCSuperiorMaximo { get { return new Mensagem() { Texto = "Código da UC superior ao máximo permitido.", Tipo = eTipoMensagem.Advertencia, Campo = "CodigoPropriedade" }; } }
		public Mensagem CodigoUCJaExistente { get { return new Mensagem() { Campo = "CodigoPropriedade", Texto = "O código da UC já existe no sistema.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem LocalLivroDisponivelObrigatorio { get { return new Mensagem() { Campo = "LocalLivroDisponivel", Texto = "Local em que o livro estará disponível é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem CulturaObrigatorio { get { return new Mensagem() { Campo = "Cultura_Nome", Texto = "Cultura é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CultivarObrigatorio { get { return new Mensagem() { Campo = "Cultivar_Nome", Texto = "Cultivar é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CultivarJaAdicionado { get { return new Mensagem() { Campo = "Cultivar_Nome", Texto = "Cultivar já adicionado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CapacidadeMesObrigatorio { get { return new Mensagem() { Campo = "CapacidadeMes", Texto = "Capacidade/Mês é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem UnidadeMedidaObrigatorio { get { return new Mensagem() { Campo = "UnidadeMedida", Texto = "Unidade de Medida é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem PossuiCultivarInvalido { get { return new Mensagem() { Texto = "Um dos itens da lista de cultivar foi informado de forma incorreta.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ResponsavelTecnicoObrigatorio { get { return new Mensagem() { Campo = "ResponsavelTecnico_NomeRazao", Texto = "Responsável Técnico é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem NumeroHabilitacaoCFOCFOCObrigatorio { get { return new Mensagem() { Campo = "CFONumero", Texto = "Nº da habilitação CFO/CFOC é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem NumeroARTObrigatorio { get { return new Mensagem() { Campo = "NumeroArt", Texto = "Nº da ART é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ResponsavelTecnicoInativo { get { return new Mensagem() { Texto = "O responsável técnico deve estar ativo.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ResponsavelTecnicoHabilitacaoSemCultivarAdicionado { get { return new Mensagem() { Texto = "O responsável técnico deve estar habilitado para emissão de CFO e CFOC para a praga com pelo menos um cultivar adicionado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ResponsavelTecnicoHabilitacaoPragaVencida { get { return new Mensagem() { Texto = "A data final da praga adicionada na habilitação para emissão de CFO e CFOC da cultura adicionada deve estar válida.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ResponsavelTecnicoJaAdicionado { get { return new Mensagem() { Texto = "Responsável técnico já adicionado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem PossuiResponsavelTecnicoInvalido { get { return new Mensagem() { Texto = "Um dos itens da lista de responsáveis técnicos foi informado de forma incorreta.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem CapacidadeProcessamentoObrigatorioAdicionar { get { return new Mensagem() { Campo = "CapacidadeProcessamento", Texto = "Capacidade de processamento/armazenamento é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ResponsavelTecnicoObrigatorioAdicionar { get { return new Mensagem() { Campo = "ResponsavelTecnico", Texto = "Responsável Técnico é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ResponsavelTecnicoNaoHabilitadoParaCultura { get { return new Mensagem() { Texto = string.Format("O responsável técnico deve estar habilitado para emissão de CFO e CFOC para a praga com a cultura associada."), Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem CulturaJaAdicionada { get { return new Mensagem() { Campo = "Cultura_Nome", Texto = "Cultura ja adicionada.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem HabilitacaoInativa { get { return new Mensagem() { Texto = "O responsável técnico deve estar com a habilitação para emissão de CFO e CFOC ativa.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem TipoApresentacaoProducaoFormaIdentificacaoObrigatorio { get { return new Mensagem() { Texto = "Tipo de apresentação do produto e forma de identificação é obrigatório.", Campo = "TipoApresentacaoProducaoFormaIdentificacao", Tipo = eTipoMensagem.Advertencia }; } }

		#region Credenciado

		public Mensagem CopiarCaractizacaoCadastrada { get { return new Mensagem() { Texto = "IDAF não possui Unidade de Consolidação cadastrada.", Tipo = eTipoMensagem.Advertencia }; } }

		#endregion

		public Mensagem CodigoUCJaExistenteInterno(string empreendimento)
		{
			return new Mensagem() { Texto = string.Format("O código da UC já está associado ao empreendimento {0}.", empreendimento), Tipo = eTipoMensagem.Advertencia };
		}
	}
}