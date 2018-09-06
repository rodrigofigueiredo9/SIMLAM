

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CARSolicitacaoMsg _CARSolicitacaoMsg = new CARSolicitacaoMsg();
		public static CARSolicitacaoMsg CARSolicitacao
		{
			get { return _CARSolicitacaoMsg; }
		}
	}

	public class CARSolicitacaoMsg
	{
		public Mensagem SolicitacaoSalvarTopico1(string strNumero) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("A solicitação de Inscrição no CAR/ES Nº {0} salvo com sucesso.", strNumero) }; }
		public Mensagem SolicitacaoSalvarTopico2 { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "A comunicação com o sistema SICAR foi iniciada." }; } }
		public Mensagem SolicitacaoSalvarTopico3 { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Localize a solicitação na listagem e acompanhe o andamento da comunicação com o SICAR para continuar o cadastro estadual." }; } }
		
		public Mensagem SolicitacaoEditar(string strNumero) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("Solicitacao de Inscrição no CAR/ES Nº {0} editado com sucesso.", strNumero) }; }
		public Mensagem SolicitacaoAlterarSituacao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Situação da Solicitação de Inscrição no CAR/ES alterada com sucesso." }; } }

		public Mensagem SolicitacaoSituacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Solicitacao_Situacao", Texto = "Situação é obrigatória." }; } }
		public Mensagem SolicitacaoSituacaoNaoPodeEditar (string situacaoAtual) 
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Solicitacao_Situacao", Texto = String.Format("Não é possível editar a Solicitação de inscrição na situação \"{0}\".", situacaoAtual) }; 
		} 
		public Mensagem SolicitacaoProtocoloObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Solicitacao_Protocolo_Numero", Texto = "Protocolo é obrigatório." }; } }
		public Mensagem SolicitacaoRequerimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Solicitacao_RequerimentoLst", Texto = "Requerimento Padrão é obrigatório." }; } }
		public Mensagem SituacaoDeveSerAguardandoImportacao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O projeto digital tem que estar em uma das seguintes situações: \"Aguardando Importação\", \"Aguardando Protocolo\", \"Aguardando Análise\", \"Deferido\" ou \"Importado\" para ser associado a uma solicitação de inscrição" }; } }

		public Mensagem SolicitacaoRequerimentoDesassociadoProtocolo(String numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Solicitacao_RequerimentoLst", Texto = String.Format("O requerimento Nº{0} não está mais associado ao processo/documento", numero) };
		}

		public Mensagem SolicitacaoProtocoloApensadoEmOutroProcesso(String protocoloNumero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O processo/documento não pode ser associado, pois está apensado/juntado ao processo {0}", protocoloNumero) };
		}

		public Mensagem SolicitacaoAtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Solicitacao_Requerimento_Atividade", Texto = "Atividade é obrigatória." }; } }
		public Mensagem SolicitacaoAtividadeInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Solicitacao_Requerimento_Atividade", Texto = "É preciso que o requerimento possua a atividade \"Cadastro Ambiental Rural - CAR.\"" }; } }

		public Mensagem SolicitacaoEmpreendimentoCodigoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Solicitacao_Empreendimento_Codigo", Texto = "Código do empreendimento é obrigatório." }; } }
		public Mensagem SolicitacaoEmpreendimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Solicitacao_Empreendimento_NomeRazao", Texto = "Empreendimento é obrigatório." }; } }
		public Mensagem SolicitacaoDeclaranteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Solicitacao_Declarante_Id", Texto = "Declarante é obrigatório." }; } }
		public Mensagem ProjetoDigitalSemEmpreendimento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso que o requerimento digital possua um empreendimento associado." }; } }

		public Mensagem SolicitacaExcluirSituacaoInvalida(string situacao) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Não é possível excluir a Solicitação de inscrição na situação \"{0}\". Para excluir, é necessário que a Solicitação esteja na situação \"Em cadastro\".", situacao) }; }

        public Mensagem SolicitacaExcluirSituacaoArquivoSICARInvalida(string situacao) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Não é possível excluir a Solicitação de inscrição com o envio de arquivo para SICAR na situação \"{0}\". Para excluir, é necessário que a Solicitação esteja na situação \"Arquivo reprovado\" ou não tenha sido enviado.", situacao) }; }
		public Mensagem SolicitacaoMensagemExcluir(string numero) { return new Mensagem() { Texto = string.Format("Esta ação irá excluir a solicitação de inscrição. Tem certeza que deseja excluir a Solicitação de Inscrição Nº \"{0}\"?", numero) }; }
		public Mensagem SolicitacaoExcluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Solicitação de Inscrição excluída com sucesso." }; } }

		public Mensagem EmpreendimentoAssociadoProjetoDigitalJaPossuiSolicitacao
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O empreendimento associado ao Projeto Digital já possui Solicitação de Inscrição no Cadastro Ambiental Rural - CAR." }; }
		}

		public Mensagem EmpreendimentoJaPossuiSolicitacao(String situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O empreendimento informado já possui uma Solicitação de Inscrição no CAR/ES na situação \"{0}\"", situacao) };
		}

		public Mensagem EmpreendimentoProjetoGeograficoDominialidadeNaoFinalizado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para o cadastro da Solicitação de Inscrição no CAR/ES é necessário ter o projeto geográfico da caracterização de Dominialidade na situação \"Finalizado\"." }; } }
		public Mensagem EmpreendimentoProjetoGeograficoDominialidadeEmRascunho { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O projeto geográfico de dominialidade não deve estar em rascunho." }; } }
		public Mensagem EmpreendimentoDominialidadeInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Os dados da caracterização de dominialidade devem estar válidos." }; } }
		public Mensagem EmpreendimentoDominialidadeInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterizacao Dominialidade deve estar cadastrada." }; } }
		public Mensagem SolicitacaoEmpreendimentoNomeRazaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterizacao Dominialidade deve estar cadastrada." }; } }


		public Mensagem SolicitacaoAlterarSituacaoNovaSituacaoNaoPermitida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A nova situação é não é permitida." }; } }

		public Mensagem SolicitacaoAlterarSituacaoNovaSituacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Situacao_Nova", Texto = "A nova situação é obrigatória" }; } }
		public Mensagem SolicitacaoAlterarSituacaoMotivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AlterarSituacao_Motivo", Texto = "Motivo é obrigatório" }; } }

		public Mensagem EmpreendimentoPossuiTitulo(String tituloSituacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O Empreendimento informado possui um Título \"Cadastro Ambiental Rural\" na situação \"{0}\".", tituloSituacao) };
		}

		public Mensagem ProtocoloPosse { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O processo/documento não está na posse do funcionário logado." }; } }
		public Mensagem ProtocoloPosseAlterarSituacao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do processo/documento ao qual está associado à Solicitação de Inscrição." }; } }

		public Mensagem ProtocoloPosseExcluir(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível excluir a Solicitação de Inscrição, pois o processo/documento {0} não está na sua posse.", numero) };
		}

		public Mensagem RequerimentoNaoPossuiAtividadeCAR { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso que o requerimento digital possua a atividade “Cadastro Ambiental Rural - CAR” como atividade solicitada." }; } }
		public Mensagem ProjetoGeograficoNaoEstaFinalizado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para o cadastro da Solicitação de Inscrição no CAR/ES é necessário ter o projeto geográfico da caracterização de Dominialidade na situação 'Finalizado'." }; } }

		public Mensagem ProjetoDigitalDominialidadeAssociada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterização Dominialidade deve estar associada ao Projeto Digital." }; } }
		public Mensagem ProjetoDigitalNaoPossuiCaracterizacaoDominialidade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Projeto digital não possui caracterização de dominialidade." }; } }

		public Mensagem CaracterizacaoDominialidadeInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Projeto digital não possui caracterização de dominialidade válida." }; } }

		public Mensagem ProjetoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O projeto digital é obrigatório." }; } }

		public Mensagem AcessarAlterarSituacao(string situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível alterar a situação da Solicitação de Inscrição na situação \"{0}\".", situacao) };
		}
        public Mensagem AcessarAlterarSituacaoSolicitacaoEnviadaSICAR { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível alterar a situação da Solicitação de Inscrição já enviada para o SICAR." }; } }
		public Mensagem AcessarAlterarSituacaoSolicitacaoEmProcessamentoSICAR { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível alterar a situação da Solicitação de Inscrição. Aguarde o processamento atual." }; } }

        public Mensagem EditarSolicitacaoTopico1(string situacaoSolicitacao, string situacaoArquivo) 
		{ 
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível editar a Solicitação de Inscrição na situação \"{0}\" e situação do arquivo SICAR como \"{1}\".", situacaoSolicitacao, situacaoArquivo) };
		}
		public Mensagem EditarSolicitacaoTopico2 { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para editar, é necessário que a Solicitação de Inscrição esteja na situação \"Em Cadastro\" e arquivo SICAR esteja \"vazio\"." }; } }
		public Mensagem EditarSolicitacaoTopico3 { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Ou a Solicitação de Inscrição esteja na situação \"Pendente\" e arquivo SICAR esteja na situação \"Arquivo Reprovado\"." }; } }



		public Mensagem ExcluirSolicitacaoTopico1(string situacaoSolicitacao, string situacaoArquivo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível excluir a Solicitação de Inscrição na situação \"{0}\" e situação do arquivo SICAR como \"{1}\".", situacaoSolicitacao, situacaoArquivo) };
		}
		public Mensagem ExcluirSolicitacaoTopico2 { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para excluir, é necessário que a Solicitação de Inscrição esteja na situação \"Em Cadastro\" e arquivo SICAR esteja \"vazio\"." }; } }
		public Mensagem ExcluirSolicitacaoTopico3 { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Ou a Solicitação de Inscrição esteja na situação \"Pendente\" e arquivo SICAR esteja na situação \"Arquivo Reprovado\"." }; } }



        public Mensagem SolicitacaoNumeroObrigatorio { get { return new Mensagem() { Campo = "Filtros.SolicitacaoTituloNumero", Tipo = eTipoMensagem.Advertencia, Texto = "Número de controle da solicitação é obrigatório." }; } }
        public Mensagem TituloNumeroObrigatorio { get { return new Mensagem() {Campo = "Filtros.SolicitacaoTituloNumero", Tipo = eTipoMensagem.Advertencia, Texto = "Número do CAR é obrigatório." }; } }
        public Mensagem CPFObrigatorio { get { return new Mensagem() { Campo = "Filtros.DeclaranteCpfCnpj", Tipo = eTipoMensagem.Advertencia, Texto = "CPF é obrigatório." }; } }
        public Mensagem CNPJObrigatorio { get { return new Mensagem() { Campo = "Filtros.DeclaranteCpfCnpj", Tipo = eTipoMensagem.Advertencia, Texto = "CNPJ é obrigatório." }; } }

        public Mensagem GerarPdfSICARUrlNaoEncontrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não foi encontrado o endereço para o PDF de recibo da inscrição no SICAR." }; } }
        public Mensagem ReenviarMsgConfirmacao { get { return new Mensagem() { Tipo = eTipoMensagem.Confirmacao, Texto = "Os dados do envio atual serão perdidos e um novo arquivo será gerado. Deseja continuar?." }; } }
        public Mensagem ErroEnviarArquivoSICAR(bool isEnviar, string solicitacaoSituacao, string arquivoSituacao)
        {
            return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Você não pode {0} uma solicitação \"{1}\" quando o arquivo está na situação \"{2}\". Aguarde o processamento atual.", ((isEnviar) ? "enviar" : "reenviar"), solicitacaoSituacao, arquivoSituacao) };
        }
        public Mensagem SucessoEnviarReenviarArquivoSICAR(bool isEnviar)
        {
            return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Sua solicitação de {0} para o SICAR foi realizada com sucesso.", ((isEnviar) ? "enviar" : "reenviar")) };
        }



		public Mensagem NaoPodeExcluirCredenciado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível excluir a Solicitação de Inscrição do Módulo Credenciado." }; } }
		public Mensagem NaoPodeEnviarCredenciado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível enviar a Solicitação de Inscrição do Módulo Credenciado. É necessário aguardar o protocolo." }; } }
		
		public Mensagem SolicitacaEnviarSituacaoSICARInvalida(string situacao)
		{
			//"Não é possível alterar a situação da Solicitação de Inscrição na situação \"{0}\"."
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível enviar a Solicitação de Inscrição na situação \"{0}\". Crie uma nova Solicitiação de Inscrição no CAR.", situacao) };
		}

		public Mensagem SolicitacaEnviarSituacaoArquivoSICARInvalida(string situacaoArquivo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível enviar a Solicitação de Inscrição com o arquivo SICAR na situação \"{0}\". Aguarde o processamento atual.", situacaoArquivo) };
		}

		public Mensagem SolicitacaoJaEnviada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Arquivo já enviado ao SICAR." }; } }
		public Mensagem ErroPdfDemonstrativo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Houve um problema ao baixar o PDF do demonstrativo CAR. Por favor comunique o administrador do sistema." }; } }
	}
}