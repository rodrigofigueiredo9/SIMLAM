using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	partial class Mensagem
	{
		private static EmissaoCFOCMsg _emissaoCFOCMsg = new EmissaoCFOCMsg();

		public static EmissaoCFOCMsg EmissaoCFOC
		{
			get { return _emissaoCFOCMsg; }
			set { _emissaoCFOCMsg = value; }
		}
	}

	public class EmissaoCFOCMsg
	{
		public Mensagem Salvar(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("CFOC Nº {0} salvo com sucesso.", numero) };
		}

		public Mensagem AtivadoSucesso(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("CFOC Nº {0} ativado com sucesso.", numero) };
		}

		public Mensagem ExcluidoSucesso { get { return new Mensagem() { Texto = "CFOC excluída com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Excluir o Certificado Fitossanitário de Origem Consolidado?" }; } }
		public Mensagem ExcluirSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A situação do CFOC deve ser \"Em elaboração\"." }; } }
		public Mensagem AtivarSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A situação do CFOC deve ser \"Em elaboração\"." }; } }
		public Mensagem EditarSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A situação do CFOC deve ser \"Em elaboração\"." }; } }

		public Mensagem TipoNumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Nº Bloco ou Nº Digital é obrigatório." }; } }
        public Mensagem AnoCFOCInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Número de Bloco Inválido. O código informado não é do ano atual." }; } }
        public Mensagem NumeroExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O número do CFOC já existe no sistema." }; } }
		public Mensagem NumeroNaoLiberado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O número do CFOC não está liberado para o responsável técnico." }; } }
		public Mensagem NumeroObrigatorio { get { return new Mensagem() { Campo = "CFOC_Numero", Tipo = eTipoMensagem.Advertencia, Texto = "O número do CFOC é obrigatório." }; } }
		public Mensagem NumeroInvalido { get { return new Mensagem() { Campo = "CFOC_Numero", Tipo = eTipoMensagem.Advertencia, Texto = "O número do CFOC está inválido." }; } }
		public Mensagem NumeroCancelado { get { return new Mensagem() { Campo = "CFOC_Numero", Tipo = eTipoMensagem.Advertencia, Texto = "O número do CFO está cancelado." }; } }
		public Mensagem EmpreendimentoObrigatorio { get { return new Mensagem() { Campo = "CFOC_EmpreendimentoId", Tipo = eTipoMensagem.Advertencia, Texto = "Empreendimento obrigatório." }; } }
        public Mensagem QtdProdutoObrigatorio { get { return new Mensagem() { Texto = "É necessário informar a quantidade.", Tipo = eTipoMensagem.Advertencia, Campo = "Container_Produto" }; } }
		public Mensagem ProdutoObrigatorio { get { return new Mensagem() { Texto = "Pelo menos um lote é obrigatório informar.", Tipo = eTipoMensagem.Advertencia, Campo = "Container_Produto" }; } }
		public Mensagem UCTituloConcluido { get { return new Mensagem() { Texto = "A Cultura selecionada deve possuir título de Abertura de Livro de Unidade de Consolidação concluído.", Campo = "CFOC_Produto_UnidadeProducao", Tipo = eTipoMensagem.Advertencia }; } }
        public Mensagem ProdutoUnico { get { return new Mensagem() { Texto = "Produto já informado.", Tipo = eTipoMensagem.Advertencia, Campo = "Container_Produto" }; } }
		public Mensagem LoteObrigatorio { get { return new Mensagem() { Campo = "CFOC_Produto_LoteId", Texto = "Lote é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem LotePossuiOrigemCancelada { get { return new Mensagem() { Campo = "CFOC_Produto_LoteId", Texto = "Lote possui Documento de origem na situação cancelado.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem CultivarAssociadoEmpreendimento(string cultivar)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O cultivar {0} não está adicionado na caracterização de UC do empreendimento.", cultivar) };
		}

		public Mensagem QuantidadeMensalInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A soma da quantidade do cultivar em todos os CFOC deste empreendimento ultrapassa a quantidade prevista na caracterização de UC para o mês." }; } }

        public Mensagem LoteSaldoInsuficiente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Lote com saldo insuficiente." }; } }


		public Mensagem PragaAssociadaCulturaObrigatorio { get { return new Mensagem() { Texto = "Pragas associadas a cultura é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "Container_Praga" }; } }

		public Mensagem PragaObrigatorio { get { return new Mensagem() { Texto = "Praga é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem PragaJaAdicionada { get { return new Mensagem() { Texto = "Praga já adicionada.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem PossuiLaudoLaboratorialObrigatorio { get { return new Mensagem() { Texto = "Possui laudo laboratorial é obrigatório.", Campo = "PossuiLaudoLaboratorial", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem PossuiTratamentoFinsQuarentenarioObrigatorio { get { return new Mensagem() { Texto = "Possui tratamento fitossanitário com fins quarentenários é obrigatório.", Campo = "PossuiTratamentoFinsQuarentenario", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NomeLaboratorioObrigatorio { get { return new Mensagem() { Campo = "CFOC_NomeLaboratorio", Texto = "Nome do laboratório é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroLaudoResultadoAnaliseObrigatorio { get { return new Mensagem() { Campo = "CFOC_NumeroLaudoResultadoAnalise", Texto = "Número do laudo com resultado da análise é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem EstadoObrigatorio { get { return new Mensagem() { Campo = "CFOC_EstadoId", Texto = "Estado é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem MunicipioObrigatorio { get { return new Mensagem() { Campo = "CFOC_MunicipioId", Texto = "Município é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ProdutoEspecificacaoObrigatorio { get { return new Mensagem() { Campo = "ProdutoEspecificacao", Texto = "Certifico que, mediante reinspeção, acompanhamento do recebimento e conferência do CFO e CFOC, PTV ou CFR das cargas que compuseram o(s) lote(s) acima especificado(s), este(s) se apresenta(m) é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem TratamentoNomeProdutoComercialObrigatorio { get { return new Mensagem() { Campo = "CFOC_TratamentoFitossanitario_NomeProduto", Texto = "Nome do produto comercial é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem TratamentoIngredienteAtivoObrigatorio { get { return new Mensagem() { Campo = "CFOC_TratamentoFitossanitario_IngredienteAtivo", Texto = "Ingrediente ativo é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem TratamentoDoseObrigatorio { get { return new Mensagem() { Campo = "CFOC_TratamentoFitossanitario_Dose", Texto = "Dose é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem TratamentoPragaProdutoObrigatorio { get { return new Mensagem() { Campo = "CFOC_TratamentoFitossanitario_PragaProduto", Texto = "Praga/Produto é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem TratamentoModoAplicacao { get { return new Mensagem() { Campo = "CFOC_TratamentoFitossanitario_ModoAplicacao", Texto = "Modo de aplicação é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ValidadeCertificadoObrigatorio { get { return new Mensagem() { Campo = "CFOC_ValidadeCertificado", Texto = "Validade do certificado é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ValidadeCertificadoMaxima { get { return new Mensagem() { Campo = "CFOC_ValidadeCertificado", Texto = "A data de validade deve ser menor ou igual a 30 dias.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem MunicipioEmissaoObrigatorio { get { return new Mensagem() { Campo = "CFOC_MunicipioEmissaoId", Texto = "Município da emissão é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ResponsavelTecnicoNaoHabilitado { get { return new Mensagem() { Texto = "O responsável técnico deve estar habilitado para emissão de CFOC.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroDigitalIndisponivel { get { return new Mensagem() { Texto = "O responsável técnico não possui mais nº digital disponível.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem LoteJaAdicionado { get { return new Mensagem() { Texto = "O lote já está adicionado.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroValido { get { return new Mensagem() { Texto = "Número válido.", Tipo = eTipoMensagem.Sucesso }; } }

		public Mensagem LimiteMaximo { get { return new Mensagem() { Texto = "É permitido adicionar até 5 itens.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem LoteUtilizado(string lote, string cfoc)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O lote {0} já está sendo utilizado pelo CFOC {1}.", lote, cfoc) };
		}

		public Mensagem LacrePoraoConteinerObrigatorio { get { return new Mensagem() { Texto = "Pelo menos um dos números (lacre/porão/contêiner) é obrigatório.", Campo = "NumeroLacre, #NumeroPorao, #NumeroContainer", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ResponsavelTecnicoNaoHabilitadoParaCultura { get { return new Mensagem() { Texto = string.Format("O responsável técnico deve estar habilitado para emissão de CFO e CFOC para a praga com a cultura associada."), Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ResponsavelTecnicoHabilitacaoPragaVencidaBloco(string cultivares)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = String.Format("A data da habilitação do responsável técnico para a praga/cultura do cultivar {0} está vencida. Deseja continuar?", cultivares) };
		}

		public Mensagem ResponsavelTecnicoHabilitacaoPragaVencidaDigital(string cultivares)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A data da habilitação do responsável técnico para a praga/cultura do cultivar {0} está vencida.", cultivares) };
		}

		public Mensagem PragaNaoAssociadaHabilitacao(string nomeCientifico, string nomeComum)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A praga {0} - {1} não está mais associada a habilitação do responsável técnico.", nomeCientifico, nomeComum) };
		}

		public Mensagem ValidadeRegistroMenorAtual { get { return new Mensagem() { Texto = string.Format("A validade da taxa de registro da habilitação para emissão de CFO e CFOC do responsável técnico deve ser maior ou igual à data atual."), Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ResponsavelDessassociadoUC { get { return new Mensagem() { Texto = string.Format("O empreendimento não possui mais este responsável técnico adicionado na caracterização de UC."), Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem DataValidadeARTMenorAtual { get { return new Mensagem() { Texto = string.Format("A data de validade da ART do responsável técnico na caracterização UC do empreendimento deve ser maior ou igual a data atual."), Tipo = eTipoMensagem.Advertencia }; } }
	}
}