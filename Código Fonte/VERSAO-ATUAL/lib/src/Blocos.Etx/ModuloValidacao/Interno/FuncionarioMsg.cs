

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static FuncionarioMsg _funcionarioMsg = new FuncionarioMsg();

		public static FuncionarioMsg Funcionario
		{
			get { return _funcionarioMsg; }
		}
	}

	public class FuncionarioMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "Funcionário salvo com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "Funcionário editado com sucesso." }; } }
		public Mensagem AlterarSituacao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "Situação alterada com sucesso." }; } }

		public Mensagem SemPermissao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Você não tem permissão para acessar essa funcionalidade." }; } }

        public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Emissao_Arquivo_Nome", Texto = "Selecione o arquivo." }; } }

		public Mensagem AssinaturaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O usuário deve possuir Assinatura Digital." }; } }

        public Mensagem ArquivoNaoImagem { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Arquivo_Nome", Texto = "O arquivo selecionado não é uma imagem." }; } }

		public Mensagem NomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Nome", Texto = "Nome é obrigatório." }; } }
		public Mensagem EmailInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Email", Texto = "Email é inválido." }; } }
		public Mensagem LoginObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Login", Texto = "Login é obrigatório." }; } }
		public Mensagem LoginExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Login", Texto = "O login já está sendo utilizado por um funcionário." }; } }
		public Mensagem SenhaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Senha", Texto = "Senha é obrigatório." }; } }
		public Mensagem ConfirmarSenhaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ConfirmarSenha", Texto = "Confirmar senha é obrigatório." }; } }
		public Mensagem SenhaDiferente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ConfirmarSenha", Texto = "A confirmação de senha está diferente da senha." }; } }
		public Mensagem CpfObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Cpf", Texto = "CPF é obrigatório." }; } }
		public Mensagem CpfInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Cpf", Texto = "CPF é inválido." }; } }
		public Mensagem NaoEncontrouRegistros { get { return Mensagem.Padrao.NaoEncontrouRegistros; } }
		public Mensagem MotivoAusente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Motivo", Texto = "O motivo da ausência é obrigatório." }; } }
		public Mensagem Inexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Funcionário inexistente." }; } }
		public Mensagem CpfEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao,   Texto = "O CPF já está associado a um funcionário." }; } }
		public Mensagem CargoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ddlCargos", Texto = "Função é obrigatória." }; } }
		public Mensagem CargoDuplicado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ddlCargos", Texto = "Função já adicionada." }; } }
		public Mensagem SetoresObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ddlSetores", Texto = "Setor é obrigatório." }; } }
		public Mensagem SetorDuplicado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ddlSetores", Texto = "Setor já adicionado." }; } }
		public Mensagem NovaSitucaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NovaSituacao", Texto = "Nova situação é obrigatória." }; } }
		public Mensagem FormatoLogin { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Login", Texto = "Formato do login é inválido. Use apenas letras e números, iniciando sempre com uma letra." }; } }

		public Mensagem AlterarSenhaFuncionario { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "A senha foi alterada com sucesso." }; } }

		public Mensagem SenhaTamanho(int configuracao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Senha", Texto = String.Format("A senha precisa ter no mínimo {0} caracteres.", configuracao) };
		}

		public Mensagem LoginTamanho(int configuracao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Login", Texto = String.Format("O login precisa ter no mínimo {0} caracteres.", configuracao) };
		}

		public Mensagem SetorComResponsavel(string setor)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Setor", Texto = String.Format("O Setor \"{0}\" já possui responsável.", setor) };
		}

		public Mensagem SetorComPosse(string setor)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Setor", Texto = String.Format("O setor \"{0}\" não pode ser removido, pois o funcionário possui processos/documentos em sua posse neste setor.", setor) };
		}

		public Mensagem SetorComRegistrador(string setor)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Setor", Texto = String.Format("O setor \"{0}\" não pode ser removido, pois o funcionário está configurado como executor de tramitação por registro neste setor.", setor) };
		}

		public Mensagem AlterarFuncionarioOutro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Não é permitido alterar outro funcionário." }; } }
	}
}


