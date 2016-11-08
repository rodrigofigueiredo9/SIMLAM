using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static HabilitarEmissaoCFOCFOCMsg _habilitarEmissaoCFOCFOCMsg = new HabilitarEmissaoCFOCFOCMsg();
		public static HabilitarEmissaoCFOCFOCMsg HabilitarEmissaoCFOCFOC
		{
			get { return _habilitarEmissaoCFOCFOCMsg; }
		}
	}

	public class HabilitarEmissaoCFOCFOCMsg
	{
		public string CampoPrefixo { get; set; }

		public Mensagem CpfInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_Responsavel_Pessoa_CPFCNPJ, #Responsavel_Cpf", Texto = "CPF inválido." }; } }
		public Mensagem CpfNaoCadastrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_Responsavel_Pessoa_CPFCNPJ", Texto = "O CPF não está associado a nenhum credenciado." }; } }
		public Mensagem CpfObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_Responsavel_Pessoa_CPFCNPJ", Texto = "CPF é obrigatório." }; } }
		public Mensagem SemProfissaoRegistro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_Responsavel_Pessoa_CPFCNPJ", Texto = "A pessoa deve possuir profissão e registro no órgão de classe informados." }; } }
		public Mensagem ResponsavelHabilitado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_Responsavel_Pessoa_CPFCNPJ", Texto = "O Responsável já está habilitado." }; } }
		public Mensagem ResponsavelObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_Responsavel_Pessoa_CPFCNPJ, #Responsavel_Cpf", Texto = "Responsável é obrigatório." }; } }
		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_Arquivo_Nome", Texto = "Selecione o arquivo." }; } }
		public Mensagem ArquivoNaoImagem { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_Arquivo_Nome", Texto = "O arquivo selecionado não é uma imagem." }; } }
		//
		public Mensagem NumeroHabilitacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_NumeroHabilitacao", Texto = "Informe o número da habilitação." }; } }
		public Mensagem NumeroHabilitacaoTamanhoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_NumeroHabilitacao", Texto = "O número de habilitação deve possuir o tamanho máximo de 8 caracteres." }; } }
		public Mensagem NumeroHabilitacaoJaExiste { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_NumeroHabilitacao", Texto = "O número de habilitação já existe no sistema." }; } }
		public Mensagem NumeroDuaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_NumeroDua", Texto = "Informe o número do DUA." }; } }
		//
		public Mensagem SituacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_Situacao_Novo, #Situacao_Novo", Texto = "Selecione a situação da habilitação." }; } }
		public Mensagem MotivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissa_Motivo_Novo, #Motivo_Novo", Texto = "Selecione o motivo da alteração." }; } }	//
		public Mensagem ValidadeRegistroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_ValidadeRegistro, #ValidadeRegistro_Novo", Texto = "Informe a validade da taxa de registro." }; } }
		public Mensagem ValidadeRegistroInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_ValidadeRegistro, #ValidadeRegistro_Novo", Texto = "Data de validade da taxa de registro inválida." }; } }

		public Mensagem SituacaoDataObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_SituacaoData", Texto = "Informe a data da situação." }; } }
		public Mensagem SituacaoDataInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_SituacaoData", Texto = "Data de situação inválida." }; } }

		//
		public Mensagem NumeroHabilitacaoOrigemObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_NumeroHabilitacaoOrigem", Texto = "Informe o número da habilitação de origem." }; } }
		public Mensagem NumeroHabilitacaoOrigemTamanhoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_NumeroHabilitacaoOrigem", Texto = "O número de habilitação de origem deve possuir o  tamanho máximo de 8 caracteres." }; } }
		public Mensagem NumeroHabilitacaoOrigemInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_NumeroHabilitacaoOrigem", Texto = "O número de habilitação deve ser igual ao número de habilitação de origem." }; } }

		public Mensagem NumeroVistoCreaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_NumeroVistoCrea", Texto = "Informe o número do Visto no CREA." }; } }
		//
		public Mensagem UFObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_UF", Texto = "Selecione a UF da habilitação." }; } }
		public Mensagem RegistroCreaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_RegistroCrea", Texto = "Informe o número do visto no CREA/ES" }; } }

		public Mensagem PragaJaAdicionada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_Pragas_Nome", Texto = "A praga selecionada ja foi adicionada." }; } }
		public Mensagem PragaNomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("HabilitarEmissao_Pragas_Nome", CampoPrefixo), Texto = "Informe o nome da praga." }; } }
		public Mensagem PragaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("HabilitarEmissao_Pragas_Nome", CampoPrefixo), Texto = "Pelo menos uma praga deve ser adicionada." }; } }

		public Mensagem DataInicialHabilitacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("HabilitarEmissao_Pragas_DataInicialHabilitacao, RenovarDataHabilitarEmissao_DataInicial", CampoPrefixo), Texto = "Informe a data inicial da habilitação." }; } }
		public Mensagem DataInicialHabilitacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("HabilitarEmissao_Pragas_DataInicialHabilitacao, RenovarDataHabilitarEmissao_DataInicial", CampoPrefixo), Texto = "Data inicial da habilitação inválida." }; } }
		public Mensagem DataInicialHabilitacaoMaiorAtual { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("HabilitarEmissao_Pragas_DataInicialHabilitacao, RenovarDataHabilitarEmissao_DataInicial", CampoPrefixo), Texto = "A data inicial de habilitação deve ser menor ou igual à data atual." }; } }

		public Mensagem DataFinalHabilitacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("HabilitarEmissao_Pragas_DataFinalHabilitacao, RenovarDataHabilitarEmissao_DataFinal", CampoPrefixo), Texto = "Informe a data final da habilitação." }; } }
		public Mensagem DataFinalHabilitacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("HabilitarEmissao_Pragas_DataFinalHabilitacao, RenovarDataHabilitarEmissao_DataFinal", CampoPrefixo), Texto = "Data final da habilitação inválida." }; } }

		public Mensagem DataInicialRenovarObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("RenovarDataHabilitarEmissao_DataInicial", CampoPrefixo), Texto = "Informe a data inicial da habilitação." }; } }
		public Mensagem DataInicialRenovarInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("RenovarDataHabilitarEmissao_DataInicial", CampoPrefixo), Texto = "Data inicial da habilitação inválida." }; } }
		public Mensagem DataInicialRenovarMaiorAtual { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("RenovarDataHabilitarEmissao_DataInicial", CampoPrefixo), Texto = "A data inicial de habilitação deve ser menor ou igual à data atual." }; } }

		public Mensagem DataFinalRenovarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("RenovarDataHabilitarEmissao_DataFinal", CampoPrefixo), Texto = "Informe a data final da habilitação." }; } }
		public Mensagem DataFinalRenovarInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("RenovarDataHabilitarEmissao_DataFinal", CampoPrefixo), Texto = "Data final da habilitação inválida." }; } }

		public Mensagem SituacaoObservacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "HabilitarEmissao_Observacao", Texto = "O campo 'Observação' deve ser informado." }; } }

		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Habilitar salvo com sucesso." }; } }
		public Mensagem Alterado { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Situação alterada com sucesso." }; } }

		public Mensagem NaoExisteHabilitacaoParaEsteCredenciado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não existe habilitação de CFO/CFOC para este credenciado." }; } }
	}
}