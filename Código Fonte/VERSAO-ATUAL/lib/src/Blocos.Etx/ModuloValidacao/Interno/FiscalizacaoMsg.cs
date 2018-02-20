

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static FiscalizacaoMsg _fiscalizacaoMsg = new FiscalizacaoMsg();
		public static FiscalizacaoMsg Fiscalizacao
		{
			get { return _fiscalizacaoMsg; }
		}
	}

	public class FiscalizacaoMsg
	{
		public Mensagem Salvar(string strNumero) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("Fiscalização nº {0} salva com sucesso.", strNumero) }; }
		public Mensagem CadastroObrigatorio(string strNome) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Preencha as informações de {0}.", strNome) }; }
		public Mensagem Concluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Cadastro de Fiscalização concluído com sucesso." }; } }
		public Mensagem JaConcluido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Fiscalização já está concluído." }; } }
		public Mensagem SituacaoNaoPodeEditar(string strSituacao) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Fiscalização não pode ser editada pois está na situação {0}.", strSituacao.ToLower()) }; }
		public Mensagem Excluir(int numero) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Fiscalização número {0} excluído com sucesso.", numero) }; }
		public Mensagem ExcluirConfirmacao(int numero) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Tem certeza que deseja excluir a fiscalização número \"{0}\"?", numero) }; }
		public Mensagem ExcluirInvalido(string strSituacao) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível excluir a fiscalização na situação \"{0}\".", strSituacao) }; }
        public Mensagem ExcluirIUFGerado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível excluir uma fiscalização que tenha número de IUF digital gerado." }; } }
		public Mensagem ExcluirCertidaoDebidoFiscalizacao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A fiscalização não poderá ser excluída pois possui certidão de débito." }; } }
		public Mensagem AgenteFiscalInvalido(string strAcao) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("É preciso ser o agente fiscal para {0} esta fiscalização.", strAcao) }; }
		public Mensagem ConfiguracaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Configuração não encontrada." }; } }
		public Mensagem ProjetoGeoProcessado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Situação do projeto geográfico deve estar processado." }; } }

		public Mensagem SituacaoSalvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Situação da fiscalização alterada com sucesso." }; } }
		public Mensagem SituacaoNovaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Fiscalizacao_SituacaoNovaTipo", Texto = "Nova situação é obrigatório." }; } }
		public Mensagem SituacaoMotivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Fiscalizacao_SituacaoNovaMotivoTexto", Texto = "Motivo é obrigatório." }; } }

		public Mensagem SituacaoEmAndamentoNaoPodeAlterar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Situação em andamento não pode ser alterada." }; } }
		public Mensagem SituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A fiscalização deve estar na situação de cadastro concluído." }; } }
		public Mensagem NaoPodeDesassociar(string strNumero, string strSituacao) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Não é possível limpar a fiscalização nº {0}, pois a mesma está na situação \"{1}\".", strNumero, strSituacao) }; }
		public Mensagem SituacaoJaAlterada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A situação já foi alterada. Por favor, atualize a pagina." }; } }
		public Mensagem FiscalizacaoJaAssociada(string strNumero) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("A fiscalização já esta associada ao protocolo {0}.", strNumero.ToLower()) }; }
		public Mensagem PosseProcessoNecessaria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do processo." }; } }

		public Mensagem ArquivoNaoEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Erro, Texto = "Não foi encontrado o arquivo concluído." }; } }

		public Mensagem SetorNaoPertenceFuncionario { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Funcionário logado não pertence ao setor de cadastro dessa fiscalização. Altere o setor na aba de Local da Infração." }; } }
	}
}
