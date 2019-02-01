

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ProjetoDigitalMsg _projetoDigitalMsg = new ProjetoDigitalMsg();
		public static ProjetoDigitalMsg ProjetoDigital
		{
			get { return _projetoDigitalMsg; }
		}
	}

	public class ProjetoDigitalMsg
	{
		public Mensagem Enviar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Projeto Digital concluído com sucesso. Continue com o Passo 4 – Imprimir Documentos." }; } }
		public Mensagem EnviarBarragem { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Projeto Digital concluído com sucesso. Continue com o cadastro do Título Declaratório." }; } }
		public Mensagem CancelarEnvio { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "O envio do projeto digital foi cancelado com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Projeto Digital foi excluído com sucesso." }; } }

		public Mensagem AtividadeSemCaracterizacao(string atividades)
		{
			return new Mensagem()
			{
				Tipo = eTipoMensagem.Informacao,
				Texto = String.Format("A" + (atividades.Split(',').Length > 1 ? "s" : "") + " atividade" + (atividades.Split(',').Length > 1 ? "s" : "") + " {0} do requerimento digital não possui caracterizações para preenchimento. Continue o cadastro com o Passo 3 – Enviar Projeto Digital.", atividades)
			};
		}

		public Mensagem ProjetoDigitalNaoEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O Projeto Digital não foi encontrado." }; } }
		public Mensagem SituacaoImportar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O Projeto Digital não pode ser importado pois não está na situação de \"Aguardando importação\"." }; } }
		
		public Mensagem SituacaoRecusar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O Projeto Digital não pode ser recusado pois não está na situação de \"Aguardando importação\"." }; } }
		public Mensagem DadosDesatualizadoImportacao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O Projeto Digital não pode ser importado pois seus dados foram modificados." }; } }
		public Mensagem RequerimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para cadastrar o Projeto Digital o requerimento deve ser informado." }; } }
		public Mensagem PassoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para cadastrar o Projeto Digital o passo deve ser informado." }; } }
		public Mensagem PosseCredenciado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Você não tem permissão para acessar os dados desse Projeto Digital." }; } }
		public Mensagem EditarRequerimentoValidar { get { return new Mensagem() { Texto = "Ao editar o passo 1, o projeto digital já enviado, sairá da fila de projetos que estão aguardando importação no IDAF e voltará para \"Em elaboração\". Todos  os dados informados no passo 2 serão apagados. Tem certeza que deseja continuar com a edição?" }; } }
		public Mensagem EditarCaracterizacaoValidar { get { return new Mensagem() { Texto = "Ao editar o passo 2, o projeto digital já enviado, sairá da fila de projetos que estão aguardando importação no IDAF e voltará para \"Em elaboração\". Tem certeza que deseja continuar com a edição?" }; } }

		public Mensagem FinalizarSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A situação do requerimento digital deve estar na situação \"Finalizado\"." }; } }

		public Mensagem EditarRequerimentoSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Projeto Digital na situação ." }; } }
		public Mensagem ExcluirSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O Projeto Digital não pode ser excluído, pois não está na situação \"Em elaboração\"." }; } }
		
		public Mensagem CancelarEnvioSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O envio do projeto digital não pode ser cancelado pois não está \"Aguardando importação\"." }; } }
		public Mensagem CancelarEnvioSolicitacaoEmProcessamento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O envio do projeto digital não pode ser cancelado, pois a solicitação de Inscrição está em processamento. Aguarde o processamento atual." }; } }
		public Mensagem CancelarEnvioSolicitacaoEnviadaSicar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O envio do projeto digital não pode ser cancelado, pois a solicitação de Inscrição já foi enviada para o SICAR." }; } }
		
		public Mensagem EnviarSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O projeto digital não pode ser enviado pois não está \"Em elaboração\"." }; } }
		public Mensagem PossuiPendenciasCorrecao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O projeto digital possui notificação para correção. Verifique o requerimento digital." }; } }

		public Mensagem EditarSituacaoInvalida(string situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O projeto digital não pode ser editado na situação \"{0}\".", situacao) };
		}

		public Mensagem RoteiroAssociado(string projeto)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Este Roteiro não está associado ao Projeto Digital {0}.", projeto) };
		}

		public Mensagem RequerimentoAssociado(string projeto)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Este Requerimento Padrão não está associado ao Projeto Digital {0}.", projeto) };
		}

		public Mensagem InteressadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Requerimento_Pessoa_NomeRazaoSocial, #Requerimento_Pessoa_CPFCNPJ", Texto = "Um interessado deve ser associado ao requerimento." }; } }

		public Mensagem PreenchimentoCaracterizacaoObrigatorio(string caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O Preenchimento das caracterização \"{0}\" é obrigatória.", caracterizacao) };
		}

		public Mensagem SelecaoDadosPessoaObrigatoria(string pessoa)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Os dados da pessoa \"{0}\" devem ser selecionados.", pessoa) };
		}

		public Mensagem SelecaoDadosEmpreendimentoObrigatoria(string empreendimento)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Os dados do empreendimento \"{0}\" devem ser selecionados.", empreendimento) };
		}

		public Mensagem MensagemExcluir(string numero)
		{
			return new Mensagem() { Texto = String.Format("O projeto digital será excluído juntamente com todas as informações contidas nele. Tem certeza que deseja excluir o projeto digital do requerimento Nº {0}?", numero) };
		}

		public Mensagem MensagemCancelarEnvio(string numero)
		{
			return new Mensagem() { Texto = String.Format("Essa ação irá alterar a situação do projeto digital para \"Em elaboração\", não permitindo a importação do projeto pelo funcionário do IDAF. Caso este projeto digital estiver associado a uma Solicitação de Inscrição no CAR ou a um Título Declaratório, ao confirmar o cancelamento, este cadastro será invalidado pelo sistema. Tem certeza que deseja cancelar o envio do projeto digital do requerimento Nº {0}?", numero) };
		}

		public Mensagem MensagemCancelarEnvioCAR()
		{
			return new Mensagem() { Texto = String.Format("O envio do projeto digital não pode ser cancelado, pois a Solicitação de inscrição no CAR já foi enviada para o SICAR. Se você deseja fazer uma retificação é necessário cadastrar um novo Projeto Digital e cadastrar/enviar uma nova Solicitação de inscrição no CAR") };
		}

		public Mensagem RequerimentoImportado(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Requerimento número {0} importado com sucesso.", numero) };
		}

		public Mensagem CaracterizacaoObrigatoria(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A caracterização {0} deve estar preenchida e associada ao projeto digital.", nome) };
		}

		public Mensagem PessoaCredenciadoConflito { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Estão ocorrendo conflitos de informações das pessoas e seus cônjuges. Por favor verifique a situação e selecione qual pessoa deverá manter o cônjuge associado." }; } }

		public Mensagem AssociadaProjetoDigitalCaracterizacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Caracterização possui dados inválidos." }; } }
		public Mensagem AssociadaProjetoDigital { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização associada ao projeto digital com sucesso." }; } }

		public Mensagem MotivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo="MotivoRecusa", Texto = "Campo Motivo obrigatório." }; } }
		public Mensagem RecusadoSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "A importação do requerimento foi recusada com sucesso." }; } }
		public Mensagem AssoiadaProjetoDigitalCaracterizacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Caracterização possui dados inválidos." }; } }
		public Mensagem AssoiadaProjetoDigital { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização associada ao projeto digital com sucesso." }; } }
		public Mensagem DesassociadaProjetoDigital { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização desassociada ao projeto digital com sucesso." }; } }
		public Mensagem CopiarCaracterizacao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização copiada com sucesso." }; } }

		public Mensagem ExcluirPossuiCARSolicitacao(string situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto =  String.Format("O projeto digital não poderá ser excluído, pois está associado a uma Solicitação de Inscrição na situação {0}.", situacao) };
		}

		public Mensagem AtividadeDesativadaInformacao(object atividadeNome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = String.Format("A atividade {0} não pode mais ser solicitada, pois está desativada. Favor recusar importação.", atividadeNome) };
		}

		public Mensagem AtividadeDesativada(object atividadeNome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A atividade {0} não pode mais ser solicitada, pois está desativada. Favor recusar importação.", atividadeNome) };
		}

		public Mensagem ImprimirDocumentosDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível Imprimir Documentos." }; } }
		public Mensagem ImprimirDocumentosConcluido { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Passo 4 – Imprimir Documentos concluído com sucesso." }; } }

		public Mensagem BarragemAssociada { get { return new Mensagem() { Texto = "Já existe uma barragem associada ao projeto digital!", Tipo = eTipoMensagem.Advertencia }; } }
	}
}