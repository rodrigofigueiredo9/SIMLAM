using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static HabilitacaoEmissaoPTVMsg _habilitacaoEmissaoPTVMsg = new HabilitacaoEmissaoPTVMsg();
		public static HabilitacaoEmissaoPTVMsg HabilitacaoEmissaoPTV
		{
			get { return _habilitacaoEmissaoPTVMsg; }
		}
	}

	public class HabilitacaoEmissaoPTVMsg
	{
		public Mensagem SalvoSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Habilitação salva com sucesso." }; } }
		public Mensagem Desativada { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Habilitação desativada com sucesso." }; } }
		public Mensagem Ativada { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Habilitação ativada com sucesso." }; } }

		public Mensagem VerificarCPFObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_CPF", Texto = "Verificar CPF é obrigatório." }; } }
		public Mensagem CPFNaoAssociadoAFuncionario { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_CPF", Texto = "O CPF não está associado a um funcionário." }; } }
		public Mensagem FuncionarioHabilitado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pessoa_Fisica_CPF", Texto = "O funcionário já está habilitado." }; } }
		public Mensagem FuncionarioJaOperador { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O funcionário não pode ser operador, pois já está associado ao responsável técnico #TEXTO#" }; } }
		public Mensagem NumeroHabilitacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_NumeroRegistro", Texto = "Número da habilitação é obrigatório." }; } }
		public Mensagem NumeroHabilitacaoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_NumeroRegistro", Texto = "O Nº da habilitação deve possuir o tamanho mínimo de 8 caracteres." }; } }
		public Mensagem NumeroHabilitacaoJaExiste { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_NumeroRegistro", Texto = "O número de habilitação já existe no sistema" }; } }
		public Mensagem RGObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_RG", Texto = "RG é obrigatório." }; } }
		public Mensagem NumberoMatriculaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_NumeroMatricula", Texto = "Nº da matrícula no IDAF é obrigatório." }; } }
		public Mensagem NumberoMatriculaInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_NumeroMatricula", Texto = "O Nº da matrícula no IDAF deve possuir o tamanho mínimo de 7 caracteres." }; } }

		public Mensagem UFHabilitacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_EstadoRegistro", Texto = "UF do registro no CREA é obrigatório." }; } }
		public Mensagem NumeroCREAObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_NumeroCREA", Texto = "Número CREA é obrigatório." }; } }
		public Mensagem NumeroVistoCREAObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_NumeroVistoCrea", Texto = "Número do visto CREA é obrigatório." }; } }

		public Mensagem ArquivoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Arquivo inválido." }; } }
		public Mensagem OperadorAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Operador ja adicionado." }; } }
		public Mensagem SituacaoOperadorDeveEstarAtivo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A situação do operador deve ser igual a 'Ativo'." }; } }

		public Mensagem MeioContatoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_TelefoneResidencial, #Habilitacao_TelefoneCelular, #Habilitacao_TelefoneComercial", Texto = "Informe ao menos um número de telefone." }; } }

		public Mensagem LogradouroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_Endereco_Logradouro", Texto = "Logradouro/Rua/Rodovia é obrigatório." }; } }
		public Mensagem BairroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_Endereco_Bairro", Texto = "Bairro/Gleba é obrigatório." }; } }
		public Mensagem UFObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_Endereco_EstadoId", Texto = "UF é obrigatório." }; } }
		public Mensagem MunicipioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_Endereco_MunicipioId", Texto = "Município é obrigatório." }; } }
		public Mensagem NumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Habilitacao_Endereco_Numero", Texto = "Número é obrigatório." }; } }
	}
}