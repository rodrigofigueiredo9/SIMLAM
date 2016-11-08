using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	partial class Mensagem
	{
		private static EmissaoCFOMsg _emissaoCFOMsg = new EmissaoCFOMsg();

		public static EmissaoCFOMsg EmissaoCFO
		{
			get { return _emissaoCFOMsg; }
			set { _emissaoCFOMsg = value; }
		}
	}

	public class EmissaoCFOMsg
	{
		public Mensagem Salvar(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("CFO Nº {0} salvo com sucesso.", numero) };
		}

		public Mensagem AtivadoSucesso(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("CFO Nº {0} ativado com sucesso.", numero) };
		}

		public Mensagem ExcluidoSucesso { get { return new Mensagem() { Texto = "CFO excluída com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Excluir o Certificado Fitossanitário de Origem ?" }; } }
		public Mensagem ExcluirSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A situação do CFO deve ser \"Em elaboração\"." }; } }
		public Mensagem AtivarSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A situação do CFO deve ser \"Em elaboração\"." }; } }
		public Mensagem EditarSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A situação do CFO deve ser \"Em elaboração\"." }; } }

		public Mensagem TipoNumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Nº Bloco ou Nº Digital é obrigatório." }; } }
		public Mensagem NumeroExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O número do CFO já foi utilizado." }; } }
		public Mensagem NumeroNaoLiberado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O número do CFO não está liberado para o responsável técnico." }; } }
		public Mensagem NumeroObrigatorio { get { return new Mensagem() { Campo = "CFO_Numero", Tipo = eTipoMensagem.Advertencia, Texto = "O número do CFO é obrigatório." }; } }
		public Mensagem NumeroCancelado { get { return new Mensagem() { Campo = "CFO_Numero", Tipo = eTipoMensagem.Advertencia, Texto = "O número do CFO está cancelado." }; } }
		public Mensagem NumeroInvalido { get { return new Mensagem() { Campo = "CFO_Numero", Tipo = eTipoMensagem.Advertencia, Texto = "O número do CFO está inválido." }; } }
		public Mensagem ProdutorObrigatorio { get { return new Mensagem() { Campo = "CFO_ProdutorId", Tipo = eTipoMensagem.Advertencia, Texto = "Produtor é obrigatório." }; } }
		public Mensagem EmpreendimentoObrigatorio { get { return new Mensagem() { Campo = "CFO_EmpreendimentoId", Tipo = eTipoMensagem.Advertencia, Texto = "Empreendimento obrigatório." }; } }

		public Mensagem ProdutoObrigatorio { get { return new Mensagem() { Texto = "Pelo menos uma UP é obrigatória.", Tipo = eTipoMensagem.Advertencia, Campo = "Container_Produto" }; } }
		public Mensagem UPTituloConcluido { get { return new Mensagem() { Texto = "A UP selecionada deve possuir título de Abertura de Livro de Unidade de Produção concluído.", Campo = "CFO_Produto_UnidadeProducao", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ProdutoUnidadeProducaoObrigatorio { get { return new Mensagem() { Campo = "CFO_Produto_UnidadeProducao", Texto = "Unidade de produção é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ProdutoQuantidadeObrigatorio { get { return new Mensagem() { Campo = "CFO_Produto_Quantidade", Texto = "Quantidade é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem PragaAssociadaCulturaObrigatorio { get { return new Mensagem() { Texto = "Pragas associadas a cultura é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "Container_Praga" }; } }

		public Mensagem PragaObrigatorio { get { return new Mensagem() { Texto = "Praga é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem PragaJaAdicionada { get { return new Mensagem() { Texto = "Praga já adicionada.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem PossuiLaudoLaboratorialObrigatorio { get { return new Mensagem() { Texto = "Possui laudo laboratorial é obrigatório.", Campo = "PossuiLaudoLaboratorial", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem PossuiTratamentoFinsQuarentenarioObrigatorio { get { return new Mensagem() { Texto = "Possui tratamento fitossanitário com fins quarentenários é obrigatório.", Campo = "PossuiTratamentoFinsQuarentenario", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NomeLaboratorioObrigatorio { get { return new Mensagem() { Campo = "CFO_NomeLaboratorio", Texto = "Nome do laboratório é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroLaudoResultadoAnaliseObrigatorio { get { return new Mensagem() { Campo = "CFO_NumeroLaudoResultadoAnalise", Texto = "Número do laudo com resultado da análise é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem EstadoObrigatorio { get { return new Mensagem() { Campo = "CFO_EstadoId", Texto = "Estado é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem MunicipioObrigatorio { get { return new Mensagem() { Campo = "CFO_MunicipioId", Texto = "Município é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ProdutoEspecificacaoObrigatorio { get { return new Mensagem() { Campo = "ProdutoEspecificacao", Texto = "Certifico que, mediante acompanhamento técnico, o(s) produto(s) acima especificado(s) se apresenta(m) é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem TratamentoNomeProdutoComercialObrigatorio { get { return new Mensagem() { Campo = "CFO_TratamentoFitossanitario_NomeProduto", Texto = "Nome do produto comercial é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem TratamentoIngredienteAtivoObrigatorio { get { return new Mensagem() { Campo = "CFO_TratamentoFitossanitario_IngredienteAtivo", Texto = "Ingrediente ativo é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem TratamentoDoseObrigatorio { get { return new Mensagem() { Campo = "CFO_TratamentoFitossanitario_Dose", Texto = "Dose é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem TratamentoPragaProdutoObrigatorio { get { return new Mensagem() { Campo = "CFO_TratamentoFitossanitario_PragaProduto", Texto = "Praga/Produto é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem TratamentoModoAplicacao { get { return new Mensagem() { Campo = "CFO_TratamentoFitossanitario_ModoAplicacao", Texto = "Modo de aplicação é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ValidadeCertificadoObrigatorio { get { return new Mensagem() { Campo = "CFO_ValidadeCertificado", Texto = "Validade do certificado é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem MunicipioEmissaoObrigatorio { get { return new Mensagem() { Campo = "CFO_MunicipioEmissaoId", Texto = "Município da emissão é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ResponsavelTecnicoNaoHabilitado { get { return new Mensagem() { Texto = "O responsável técnico deve estar habilitado para emissão de CFO.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ResponsavelTecnicoInativo { get { return new Mensagem() { Texto = "A habilitação do responsável técnico está na situação inativo.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroDigitalIndisponivel { get { return new Mensagem() { Texto = "O responsável técnico não possui mais nº digital disponível.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ProdutorNaoEstaMaisAssociado { get { return new Mensagem() { Campo = "CFO_ProdutorId", Texto = "O produtor não está mais associado na caracterização de UP.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ProdutorNaoEstaMaisAssociadoEmpreendimento { get { return new Mensagem() { Campo = "CFO_ProdutorId", Texto = "O empreendimento não possui mais este produtor adicionado na caracterização de UP.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem UnidadeProducaoJaAdicionado { get { return new Mensagem() { Texto = "Unidade de produção já adicionada.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NumeroValido { get { return new Mensagem() { Texto = "Número válido.", Tipo = eTipoMensagem.Sucesso }; } }

		public Mensagem LimiteMaximo { get { return new Mensagem() { Texto = "É permitido adicionar até 5 itens.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem DataFimColheitaNaoPodeSerMenorQueDataInicial { get { return new Mensagem() { Texto = "Data final da colheita não pode ser menor do que a Data inicial.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem UPDessassociada(string codigo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A UP {0} não está mais associada na caracterização de UP do empreendimento.", codigo) };
		}

		public Mensagem LacrePoraoConteinerObrigatorio { get { return new Mensagem() { Texto = "Pelo menos um dos números (lacre/porão/contêiner) é obrigatório.", Campo = "NumeroLacre, #NumeroPorao, #NumeroContainer", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ValidadeCertificadoMaxima { get { return new Mensagem() { Campo = "CFO_ValidadeCertificado", Texto = "A data de validade deve ser menor ou igual a 30 dias.", Tipo = eTipoMensagem.Advertencia }; } }

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

		public Mensagem DataValidadeARTMenorAtual { get { return new Mensagem() { Texto = string.Format("A data de validade da ART do responsável técnico na caracterização UP do empreendimento deve ser maior ou igual a data atual."), Tipo = eTipoMensagem.Advertencia }; } }
		
		public Mensagem DataValidadeRENASEMMenorAtual(string codigo)
		{
			return new Mensagem()
			{
				Tipo = eTipoMensagem.Advertencia,
				Texto = String.Format("A UP {0} não possui data do RENASEM dentro do prazo de validade.", codigo)
			//String.Format("A data de validade do RENASEM na UP {0} deve ser maior ou igual à data atual.", codigo)
			};
		}

		public Mensagem QuantidadeMensalInvalida(string up)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A soma da quantidade do cultivar e unidade de medida em todos os CFO da Unidade de Produção Nº {0} ultrapassa a quantidade prevista na caracterização de UP para o ano.", up) };
		}

		public Mensagem DocumentoOrigemDeveSerDeMesmaUC { get { return new Mensagem() { Campo = "", Texto = "O saldo do documento deve ser utilizado somente para a formação de lotes na UC em que já foi utilizado.", Tipo = eTipoMensagem.Advertencia }; } }
	}
}