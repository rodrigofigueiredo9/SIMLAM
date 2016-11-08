

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static TramitacaoMsg _tramitacaoMsg = new TramitacaoMsg();
		public static TramitacaoMsg Tramitacao
		{
			get { return _tramitacaoMsg; }
			set { _tramitacaoMsg = value; }
		}
	}

	public class TramitacaoMsg
	{
		public Mensagem JuntarApensarDocumentoNumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DocumentoJuntarNumero", Texto = "Número de registro é obrigatório." }; } }
		public Mensagem JuntarApensarDocumentoNumeroInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DocumentoJuntarNumero", Texto = "Número de registro inválido. O formato correto é número/ano." }; } }
		public Mensagem JuntarApensarDocumentoTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DocumentoJuntarTipo", Texto = "Tipo de documento é obrigatório." }; } }
		public Mensagem JuntarApensarProcessoNumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProcessoApensarNumero", Texto = "Número  de registro é obrigatório." }; } }
		public Mensagem JuntarApensarProcessoNumeroInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProcessoApensarNumero", Texto = "Número  de registro inválido. O formato correto é número/ano." }; } }
		public Mensagem JuntarApensarProcessoTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProcessoApensarTipo", Texto = "Tipo de processo é obrigatório." }; } }

		public Mensagem FuncionarioJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Funcionário já adicionado neste setor." }; } }
		public Mensagem FuncionarioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Funcionário é obrigatório em tramitação por registro." }; } }
		public Mensagem RemetenteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Enviar_Remetente_Id", Texto = "Funcionário é obrigatório." }; } }

		public Mensagem SetorRemetenteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Enviar_RemetenteSetor_Id", Texto = "Setor de origem é obrigatório." }; } }

		public Mensagem NumeroProtocoloInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "Número de registro do processo/documento inválido. O formato correto é número/ano." }; } }
		public Mensagem NumeroProtocoloObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "Número de registro do processo/documento obrigatório." }; } }
		public Mensagem NumeroProcessoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "Número de registro é obrigatório." }; } }
		public Mensagem NumeroDocumentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "Número de registro é obrigatório." }; } }
		public Mensagem ProtocoloInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "O processo/documento não está cadastrado." }; } }
		public Mensagem ProcessoInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "O processo não está cadastrado." }; } }
		public Mensagem DocumentoInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "O documento não está cadastrado." }; } }
		public Mensagem ProtocoloJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "O processo/documento já está adicionado." }; } }

		public Mensagem ProtocoloNaoEncontradoParaDadosDesarquivamentoInformado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O processo/ documento não foi encontrado para os dados de desarquivamento informado" }; } }

		public Mensagem FuncionarioNaoPossuiPosseDoProtocolo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do processo/documento para enviá-lo." }; } }

		public Mensagem TipoProcessoNecessario { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Enviar_TipoProcessoId", Texto = "O tipo do processo é obrigatório." }; } }
		public Mensagem TipoDocumentoNecessario { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Enviar_TipoDocumentoId", Texto = "O tipo documento é obrigatório." }; } }
		public Mensagem ObjetivoObrigratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Enviar_Objetivo_Id", Texto = "O motivo da tramitação é obrigatorio." }; } }
		public Mensagem OrgaoExternoObrigratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "OrgaoExterno", Texto = "Órgão externo é obrigatório." }; } }
		
		public Mensagem SetorDestinoObrigratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SetorDestinatario_Id", Texto = "Setor de destino é obrigatório." }; } }
		public Mensagem SetorDestinoPorRegistro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SetorDestinatario_Id", Texto = "O setor de destino selecionado está configurado para tramitação por registro." }; } }

		public Mensagem SetorDestinatarioObrigratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Enviar_DestinatarioSetor_Id", Texto = "Setor de destino é obrigatório." }; } }
		public Mensagem DestinatarioObrigratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Enviar_Destinatario_Id", Texto = "O destinatário é obrigatório." }; } }
		public Mensagem ProtocoloObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Pelo menos um processo/documento deve estar selecionado para envio." }; } }
		public Mensagem FuncionarioNaoPossuiPosseNenhuma { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Campo = "", Texto = "Não existem processos/documentos em posse do funcionário remetente no setor de origem selecionado." }; } }
		public Mensagem EnviarEfetuadoComSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Campo = "Enviar_Remetente_Id", Texto = "Processo/Documento enviado com sucesso." }; } }
		public Mensagem SetorRemetenteTipoNormal { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Enviar_RemetenteSetor_Id", Texto = "O setor de origem selecionado não está configurado para tramitação por registro." }; } }
		public Mensagem SetorRemetenteTipoRegistro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Enviar_RemetenteSetor_Id", Texto = "O setor de origem selecionado está configurado como tramitação por registro." }; } }

		public Mensagem SetorInvalido(string numeroProtocolo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O processo/documento {0} já não se encontra no setor informado.", numeroProtocolo) };
		}

		public Mensagem HistoricoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Tramitação inválida." }; } }

		public Mensagem ProtocoloDeOutroSetor { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Enviar_RemetenteSetor_Id", Texto = "O processo/documento pertence a outro setor do remetente selecionado." }; } }
		public Mensagem NaoEncontrouRegistros { get { return Mensagem.Padrao.NaoEncontrouRegistros; } }

		public Mensagem NaoEncontrouRegistrosOrgaoExterno { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Não existem processos/documentos no  órgão externo selecionado." }; } }

		public Mensagem FuncionarioNaoPossuiPosseProtocolo(string numeroProtocolo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O processo/documento {0} não pode ser enviado, pois não está na posse do funcionário.", numeroProtocolo) };
		}

		public Mensagem FuncionarioNaoAssociadoSetor(string funcionarioNome, string setorSigla)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = String.Format("O funcionário {0} não está cadastrado no setor {1}.", funcionarioNome, setorSigla) };
		}

		public Mensagem RemetenteDestinatarioIguais { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Enviar_Destinatario_Id", Texto = "O destinatário deve ser diferente do remetente quando estiver tramitando para o mesmo setor." }; } }

		public Mensagem ProtocoloSetorInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Setor inválido." }; } }

		public Mensagem TramitConfigSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "Tramitação configurada com sucesso." }; } }

		public Mensagem NenhumProcDocPosse { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Não existe nenhum processo/documento na posse do usuário." }; } }

		public Mensagem ReceberPeloMenosUmProcessoOuDocumentoDeveEstarSelecionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Pelo menos um processo/documento deve estar selecionado para recebimento." }; } }

		public Mensagem ReceberSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Processo/Documento recebido com sucesso." }; } }
		public Mensagem RetirarSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Processo/Documento retirado com sucesso." }; } }
		
		public Mensagem UsuarioNaoRegistrador { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "O usuário logado não é registrador deste setor." }; } }

		public Mensagem ReceberDestinatarioObrigratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Destinatario_Id", Texto = "O destinatário é obrigatório." }; } }
		
		public Mensagem Cancelar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Tramitação cancelada com sucesso." }; } }

		public Mensagem CancelarTramitacaoRegistro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#Enviar_Remetente_Id, #Enviar_RemetenteSetor_Id", Texto = "Não é possível cancelar a tramitação do processo/documento, pois o setor do funcionário está configurado para tramitação por registro." }; } }

		public Mensagem TramitacaoRegistro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#Enviar_Remetente_Id, #Enviar_RemetenteSetor_Id", Texto = "Não é possível tramitar o processo/documento, pois o setor do funcionário está configurado para tramitação por registro." }; } }

		public Mensagem TramitacaoSemPermissao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível tramitar o processo/documento, pois o usuário não tem permissão." }; } }

		public Mensagem ArquivoNaoEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Arquivo inválido ou inexistente." }; } }

		public Mensagem ArquivoEditarSetorNaoEhPossivelPorPossuirProtocolo(eTipoMensagem tipoMensagem)
		{
			return new Mensagem() { Tipo = tipoMensagem, Texto = "Não é possível editar o setor do arquivo com processo(s) e(ou) documento(s)." };
		}

		public Mensagem ProtocoloJaTramitado(string objeto, string numero, string acao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O {0} - {1} não pode ser {2}, pois não está em tramitação.", objeto, numero, acao) };
		}

		public Mensagem ProtocoloJaRetiradoExterno(string objeto, string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O {0} - {1} já foi retirado do órgão externo.", objeto, numero) };
		}

		public Mensagem NaoExisteProtocoloAReceber { get { return new Mensagem() { Campo = "", Tipo = eTipoMensagem.Informacao, Texto = "Não existe processo/documento a receber." }; } }
		public Mensagem NaoExisteProtocoloAReceberRegistro { get { return new Mensagem() { Campo = "", Tipo = eTipoMensagem.Informacao, Texto = "Não existem processo/documento enviado para o setor de destino." }; } }

		#region Motivo de tramitação

		public Mensagem MotivoObrigatorio { get { return new Mensagem() { Campo = "#Inexistente, .txtMotivo", Tipo = eTipoMensagem.Advertencia, Texto = "Nome Obrigatório." }; } }
		public Mensagem MotivoTamanhoMaximo { get { return new Mensagem() { Campo = "#Inexistente, .txtMotivo", Tipo = eTipoMensagem.Advertencia, Texto = "Nome deve ter no máximo 100 caracteres." }; } }

		public Mensagem MotivoCadastrado { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Motivo cadastrado com sucesso." }; } }
		public Mensagem MotivoEditado { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Motivo editado com sucesso." }; } }

		public Mensagem MotivoAtivado { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Motivo ativado com sucesso." }; } }
		public Mensagem MotivoDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Motivo desativado com sucesso." }; } }
		public Mensagem NomeJaAdicionado { get { return new Mensagem() { Campo = "#Inexistente, .txtMotivo", Tipo = eTipoMensagem.Advertencia, Texto = "Nome já adicionado." }; } }
		
		#endregion
	}
}