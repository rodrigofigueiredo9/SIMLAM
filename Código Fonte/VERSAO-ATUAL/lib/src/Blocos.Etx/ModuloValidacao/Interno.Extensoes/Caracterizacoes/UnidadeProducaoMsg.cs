namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static UnidadeProducaoMsg _unidadeProducaoMsg = new UnidadeProducaoMsg();

		public static UnidadeProducaoMsg UnidadeProducao
		{
			get { return _unidadeProducaoMsg; }
		}
	}

	public class UnidadeProducaoMsg
	{

		public Mensagem SalvoSucesso { get { return new Mensagem() { Texto = "Unidade de Produção salva com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem ExcluidoSucesso { get { return new Mensagem() { Texto = "Unidade de Produção excluída com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Excluir caracterização Unidade de Produção?" }; } }
		public Mensagem Inexistente { get { return new Mensagem() { Texto = "Caracterização Unidade de Produção foi excluída.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ProdutorObrigatorio { get { return new Mensagem() { Texto = "Produtor é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducao_LstProdutores" }; } }
		public Mensagem ProdutorJaAdicionado { get { return new Mensagem() { Texto = "Produtor já adicionado.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducao_LstProdutores" }; } }
		public Mensagem ResponsavelTecnicoObrigatorio { get { return new Mensagem() { Texto = "Responsável técnico obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_ResponsavelTecnico_NomeRazao" }; } }
		public Mensagem ResponsavelTecnicoJaAdicionado { get { return new Mensagem() { Texto = "Responsável técnico já adicionado.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_ResponsavelTecnico_NomeRazao" }; } }
		public Mensagem NumeroARTObrigatorio { get { return new Mensagem() { Texto = "Numero da ART é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_ResponsavelTecnico_NumeroArt" }; } }
		public Mensagem DataValidadeARTObrigatorio { get { return new Mensagem() { Texto = "Data de validade da ART é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_ResponsavelTecnico_DataValidadeART" }; } }
		public Mensagem CodigoUPObrigatorio { get { return new Mensagem() { Texto = "Código da UP é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_CodigoUP" }; } }
		public Mensagem CodigoUPInvalido { get { return new Mensagem() { Texto = "Código da UP deve possuir 17 caracteres.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_CodigoUP" }; } }

		public Mensagem NumeroRenasemObrigatorio { get { return new Mensagem() { Texto = "Número do Renasem é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_RenasemNumero" }; } }
		public Mensagem DataValidadeRenasemObrigatorio { get { return new Mensagem() { Texto = "Data de validade do Renasem é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_DataValidadeRenasem" }; } }
		public Mensagem AreaHAObrigatorio { get { return new Mensagem() { Texto = "Área(ha) é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_AreaHA" }; } }
		public Mensagem CoordenadaObrigatorio { get { return new Mensagem() { Texto = "Coordenada é obrigatória.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_Coordenada_EastingUtmTexto, #UnidadeProducaoItem_Coordenada_NorthingUtmTexto" }; } }

		public Mensagem DataPlantioAnoProducaoObrigatorio { get { return new Mensagem() { Texto = "Data de plantio ou ano de produção é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_DataPlantioAnoProducao" }; } }
		public Mensagem DataPlantioAnoProducaoInvalida { get { return new Mensagem() { Texto = "A data de plantio está inválida.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_DataPlantioAnoProducao" }; } }
		public Mensagem DataPlantioAnoProducaoMaiorAtual { get { return new Mensagem() { Texto = "A data de plantio deve ser menor ou igual à data atual.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_DataPlantioAnoProducao" }; } }


		public Mensagem EstimativaProducaoQuantidadeAnoObrigatorio { get { return new Mensagem() { Texto = "Quantidade / Ano é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_EstimativaProducaoQuantidadeAno" }; } }
		public Mensagem EstimativaProducaoUnidadeMedidaObrigatorio { get { return new Mensagem() { Texto = "Unidade de medida é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_EstimativaProducaoUnidadeMedida" }; } }
		public Mensagem CodigoPropriedadeObrigatorio { get { return new Mensagem() { Texto = "Código da propriedade é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducao_CodigoPropriedade" }; } }
		public Mensagem CodigoPropriedadeSuperiorMaximo { get { return new Mensagem() { Texto = "Código da propriedade superior ao máximo permitido.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducao_CodigoPropriedade" }; } }
        public Mensagem CodigoPropriedadeInvalido { get { return new Mensagem() { Texto = "Código da Propriedade deve possuir 11 caracteres.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducao_CodigoPropriedade" }; } }
        public Mensagem LocalLivroDisponivelObrigatorio { get { return new Mensagem() { Texto = "Local do livro disponível é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducao_LocalLivroDisponivel" }; } }
		public Mensagem UnidadeProducaoObrigatorio { get { return new Mensagem() { Texto = "Unidade de Produção é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "Unidades_Producao" }; } }
		public Mensagem UnidadeProducaoItemIncorreto { get { return new Mensagem() { Texto = "Uma Unidade de Produção foi cadastrada de forma incorreta.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ResponsavelTecnicoInativo { get { return new Mensagem() { Texto = "Responsável técnico deve estar com a situação igual a \"Ativo\".", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ResponsavelTecnicoNaoHabilitadoParaCultivar(string cultivarNome)
		{
			return new Mensagem() { Texto = string.Format("O Responsável técnico não está habilitado para o cultivar \"{0}\".", cultivarNome), Tipo = eTipoMensagem.Advertencia };
		}

		public Mensagem PragaCultivarDataFinalVencida { get { return new Mensagem() { Texto = "A data final da praga adicionada na habilitação para emissão de CFO e CFOC com o cultivar associado deve estar válida.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem RenasemDataVencimentoMenorQueDataAtual { get { return new Mensagem() { Campo = "UnidadeProducaoItem_DataValidadeRenasem", Texto = "Data de validade deve ser maior que a data atual.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem RenasemNumeroObrigatorio { get { return new Mensagem() { Campo = "UnidadeProducaoItem_RenasemNumero", Texto = "RENASEM Nº é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		
		public Mensagem CodigoUPJaExiste { get { return new Mensagem() { Campo = "UnidadeProducaoItem_CodigoUPSequencia", Texto = "Código UP já existente.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CodigoUPSuperiorMaximo { get { return new Mensagem() { Campo = "UnidadeProducaoItem_CodigoUPSequencia", Texto = "Código UP superior ao máximo permitido.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem CodigoPropriedadeJaExiste { get { return new Mensagem() { Texto = "O código da propriedade já existe no sistema.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducao_CodigoPropriedade" }; } }

		public Mensagem ProdutorNaoEstaMaisVinculadoNoEmpreendimento(string nomeProdutor)
		{
			return new Mensagem() { Texto = string.Format("O produtor {0} não está mais adicionado no empreendimento.", nomeProdutor), Tipo = eTipoMensagem.Advertencia };
		}

		public Mensagem NorthingUtmObrigatorio { get { return new Mensagem() { Texto = "Northing Utm é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_Coordenada_NorthingUtmTexto" }; } }
		public Mensagem EastingUtmObrigatorio { get { return new Mensagem() { Texto = "Easting Utm é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_Coordenada_EastingUtmTexto" }; } }

		public Mensagem CulturaObrigatorio { get { return new Mensagem() { Texto = "Cultura é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_CulturaTexto" }; } }
		public Mensagem CultivarObrigatorio { get { return new Mensagem() { Texto = "Cultivar é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "UnidadeProducaoItem_CultivarId" }; } }

		public Mensagem ResponsavelTecnicoNaoHabilitadoParaCultura { get { return new Mensagem() { Texto = string.Format("O responsável técnico deve estar habilitado para emissão de CFO e CFOC para a praga com a cultura associada."), Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem PragaCulturaDataFinalVencida { get { return new Mensagem() { Texto = "A data final da praga adicionada na habilitação para emissão de CFO e CFOC com a cultura associada deve estar válida.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NenhumResponsavelTecnicoHabilitadoParaCultura(string culturaNome)
		{
			return new Mensagem() { Texto = string.Format("Não há responsável técnico habilitado para a cultura \"{0}\".", culturaNome), Tipo = eTipoMensagem.Advertencia };
		}

		public Mensagem CopiarCaractizacaoCadastrada { get { return new Mensagem() { Texto = "IDAF não possui dominialidade cadastrada.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem HabilitacaoInativa { get { return new Mensagem() { Texto = "O responsável técnico não deve estar com a habilitação para emissão de CFO e CFOC inativa.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem CodigoPropriedadeExistenteImportar(string empreendimento)
		{
			return new Mensagem() { Texto = string.Format("O código da propriedade já está associado ao empreendimento {0}.", empreendimento), Tipo = eTipoMensagem.Advertencia };
		}

		public Mensagem CodigoUPJaAssociado(int empreendimentoCodigo, string empreendimentoNome)
		{
			return new Mensagem() { Texto = string.Format("O código da UP já está associado ao empreendimento {0} – {1}.", empreendimentoCodigo, empreendimentoNome), Tipo = eTipoMensagem.Advertencia }; 
		}

		public Mensagem CodigoUPNaoContemCodPropriedade(long codigoUP)
		{
			return new Mensagem() { Texto = string.Format("Código da UP {0} deve possuir o código da propriedade.", codigoUP), Tipo = eTipoMensagem.Advertencia };
		}
	}
}