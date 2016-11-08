using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{

	partial class Mensagem
	{
		private static PTVOutroMsg _emissaoPTVOutroMsg = new PTVOutroMsg();

		public static PTVOutroMsg PTVOutro
		{
			get { return _emissaoPTVOutroMsg; }
			set { _emissaoPTVOutroMsg = value; }
		}
	}

	public class PTVOutroMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Texto = "PTV de outro estado salvo com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem CanceladoSucesso { get { return new Mensagem() { Texto = "PTV de outro estado cancelado com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }

		#region Campos Obrigatóroios

		public Mensagem NumeroPTVObrigatorio { get { return new Mensagem() { Campo = "Numero", Texto = "Número do PTV de outro estado é Obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem NumeroPTVInvalido { get { return new Mensagem() { Campo = "Numero", Texto = "Número do PTV de outro estado deverá ser composto por 10 digitos.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem SituacaoObrigatorio { get { return new Mensagem() { Campo = "Situacao", Texto = "Situação é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InteressadoObrigatorio { get { return new Mensagem() { Campo = "Interessado", Texto = "Interessado é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InteressadoCnpjCpfObrigatorio { get { return new Mensagem() { Campo = "InteressadoCnpjCpf", Texto = "CNPJ/CPF do interessado é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InteressadoCnpjCpfInvalido { get { return new Mensagem() { Campo = "InteressadoCnpjCpf", Texto = "CNPJ/CPF do interessado é inválido.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InteressadoEnderecoObrigatorio { get { return new Mensagem() { Campo = "InteressadoEndereco", Texto = "Endereço do interessado é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InteressadoEstadoObrigatorio { get { return new Mensagem() { Campo = "InteressadoEstadoId", Texto = "Selecione o Estado do interessado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InteressadoMunicipioObrigatorio { get { return new Mensagem() { Campo = "InteressadoMunicipioId", Texto = "Selecione o Município do interessado.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem Identificacao_Produto_Obrigatorio { get { return new Mensagem() { Texto = "Pelo menos uma identificação do produto deve ser adicionado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem TipoDocumentoObrigatorio { get { return new Mensagem() { Campo = "Tipo", Texto = "Tipo de Pessoa é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem DestinatarioObrigatorio { get { return new Mensagem() { Campo = "DestinararioCPFCNPJ", Texto = "Destinatário é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem EstadoObrigatorio { get { return new Mensagem() { Campo = "EstadoId", Texto = "Selecione o Estado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem MunicipioObrigatorio { get { return new Mensagem() { Campo = "MunicipioId", Texto = "Selecione o Município.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem TipoOrigemObrigatorio { get { return new Mensagem() { Campo = "OrigemTipo", Texto = "Origem é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem QuantidadeMaximoCFOCFOC(string documentoOrigem)
		{
			return new Mensagem() { Texto = string.Format("{0} deve conter 10 números.", documentoOrigem), Tipo = eTipoMensagem.Advertencia };
		}

		public Mensagem QuantidadeMaximoCFCFRTF(string documentoOrigem)
		{
			return new Mensagem() { Texto = string.Format("{0} deve conter 20 números.", documentoOrigem), Tipo = eTipoMensagem.Advertencia };
		}

		public Mensagem OrigemObrigatorio { get { return new Mensagem() { Campo = "NumeroOrigem", Texto = "Número Origem é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CulturaObrigatorio { get { return new Mensagem() { Campo = "ProdutoCultura", Texto = "Cultura é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CultivarObrigatorio { get { return new Mensagem() { Campo = "ProdutoCultivar", Texto = "Cultivar é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem UnidadeMedidaObrigatorio { get { return new Mensagem() { Campo = "ProdutoUnidadeMedida", Texto = "Unidade Medida é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem QuantidadeObrigatorio { get { return new Mensagem() { Campo = "ProdutoQuantidade", Texto = "Quantidade é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ResponsavelTecnicoObrigatorio { get { return new Mensagem() { Campo = "ResponsavelTecnico", Texto = "Responsável técnico é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem RespTecnicoNumHabObrigatorio { get { return new Mensagem() { Campo = "RespTecnicoNumHab", Texto = "Nº da habilitação é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		#endregion

		#region Validação

		public Mensagem PtvJaExistente { get { return new Mensagem() { Campo = "Numero", Texto = "O número do PTV de outro estado já existe no sistema.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem ITemProdutoJaAdicionado(string documentoOrigem)
		{
			return new Mensagem() { Texto = String.Format("O cultivar já está adicionado para o documento de origem {0}.", documentoOrigem), Tipo = eTipoMensagem.Advertencia };
		}
		public Mensagem QauntidadeItensUltrapassado { get { return new Mensagem() { Texto = "Não é permitido adicionar mais que 5 itens.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CpfCnpjInvalido { get { return new Mensagem() { Campo = "DestinararioID", Texto = "CPF/CNPJ deve estar válido.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CancelarSituacaoInvalida { get { return new Mensagem() { Texto = "A situação do PTV de outro estado deve ser \"Válida\".", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CancelarAssociadoALote { get { return new Mensagem() { Texto = "O PTV de outro estado não pode ser cancelado, pois está associado a um Lote.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem NaoPodeEditarPTV { get { return new Mensagem() { Texto = "O PTV de outro estado deve estar na situação 'Em elaboração'.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem DataValidadeInvalida { get { return new Mensagem() { Campo = "DataValidade", Texto = "Data de validade deve ser menor ou igual a 30 dias.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem DataValidadeMenorDataEmissao { get { return new Mensagem() { Campo = "DataValidade", Texto = "Data de validade deve ser maior que a data de emissão.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem DestinatarioNaoCadastrado { get { return new Mensagem() { Campo = "txtCodigoUC", Texto = "O destinatário não está cadastrado.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem CodigoUCObrigatorio { get { return new Mensagem() { Campo = "txtCodigoUC", Texto = "Código da UC é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }

		#endregion
	}
}