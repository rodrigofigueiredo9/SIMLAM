

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ArquivamentoMsg _arquivamentoMsg = new ArquivamentoMsg();
		public static ArquivamentoMsg Arquivamento
		{
			get { return _arquivamentoMsg; }
			set { _arquivamentoMsg = value; }
		}
	}

	public class ArquivamentoMsg
	{
		public Mensagem DesarquivarProtocoloObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Pelo menos um processo/documento deve estar selecionado para desarquivamento." }; } }
		public Mensagem SetorOrigemObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SetorRemetente_Id", Texto = "Setor de origem é obrigatório." }; } }
		public Mensagem SetorDestinoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SetorDestinatario_Id", Texto = "Setor de destino é obrigatório." }; } }

		public Mensagem SetorSemArquivo { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Não existe arquivo cadastrado para o setor selecionado." }; } }
		public Mensagem SetorOrigemSemArquivo { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Não existe arquivo cadastrado para o setor de origem selecionado." }; } }
		public Mensagem SetorDestinoSemArquivo { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Não existe arquivo cadastrado para o setor de destino selecionado." }; } }

		public Mensagem DesarquivarSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Processo/Documento desarquivado com sucesso." }; } }

		public Mensagem ArquivarSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Processo/Documento arquivado com sucesso." }; } }
		public Mensagem ArquivarArquivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Arquivar_ArquivoId", Texto = "O arquivo é obrigatório." }; } }
		public Mensagem ArquivarSetorOrigemObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Arquivar_SetorId", Texto = "Setor de origem é obrigatório." }; } }

		public Mensagem ArquivarProcessoJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "O processo ja foi adicionado." }; } }
		public Mensagem ArquivarDocumentoJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "O documento ja foi adicionado." }; } }
		public Mensagem ArquivarNumeroProcessoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "Número de registro do processo é obrigatório." }; } }
		public Mensagem ArquivarTipoProcessoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TipoProcesso", Texto = "Tipo de processo é obrigatorio." }; } }
		public Mensagem ArquivarNumeroDocumentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "Número de registro do documento é obrigatório" }; } }
		public Mensagem ArquivarTipoDocumentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TipoDocumento", Texto = "Tipo de documento é obrigatorio." }; } }
		public Mensagem ArquivarObjetivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Arquivar_ObjetivoId", Texto = "O motivo é obrigatório." }; } }
		public Mensagem ArquivarEstanteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Arquivar_EstanteId", Texto = "A estante é obrigatória." }; } }
		public Mensagem ArquivarPrateleiraObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Arquivar_PrateleiraId", Texto = "A prateleira/pasta suspensa é obrigatória." }; } }
		public Mensagem ArquivarIdentificacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Arquivar_Identificacao", Texto = "A identificação é obrigatória." }; } }
		public Mensagem ArquivarProtocoloObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Container_Processos_Documentos", Texto = "Pelo menos um processo/documento deve estar selecionado para arquivamento." }; } }

		public Mensagem ArquivarProcessoApensadoNaoPodeSerArquivado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "O processo não pode ser arquivado pois está apensado a outro processo." }; } }
		public Mensagem ArquivarDocumentoJuntadoNaoPodeSerArquivado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "O documento não pode ser arquivado pois está juntado a um processo." }; } }

		public Mensagem ArquivarNaoPodeExcluirArquivoComArquivos { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Não é possível excluir o arquivo pois este contém processo(s) e/ou documento(s) arquivado(s)." }; } }
		public Mensagem ArquivarSetorNaoContemProtocolo { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Campo = "DocumentoJuntarNumero", Texto = "Não existem processos/documentos em posse do funcionário remetente no setor de origem selecionado." }; } }

		public Mensagem DesarquivarArquivoSemProtocolo { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Campo = "", Texto = "Não existem processos/documentos no arquivo selecionado." }; } }
		public Mensagem DesarquivarArquivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Arquivar_ArquivoId", Texto = "O arquivo é obrigatório." }; } }

		public Mensagem FuncionarioNaoPossuiPosseProtocolo(string numeroProtocolo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O processo/documento {0} não pode ser arquivado, pois não está na posse do funcionário.", numeroProtocolo) };
		}

		public Mensagem ProtocoloJaDesarquivado(string objeto, string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O {0} - {1} já foi desarquivado.", objeto, numero) };
		}

		public Mensagem DesarquivarProtocoloNumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "Número de registro do processo/documento é obrigatório." }; } }
		public Mensagem DesarquivarProtocoloNumeroInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "Número de processo/documento inválido. O formato correto é número/ano." }; } }
		public Mensagem DesarquivarProtocoloNaoCadastrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "O processo/documento não está cadastrado." }; } }
		public Mensagem DesarquivarProtocoloJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "O processo/documento já está adicionado." }; } }
		public Mensagem DesarquivarProtocoloEmOutroSetor { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "Processo/documento pertence a outro setor de origem." }; } }
		public Mensagem DesarquivarProtocoloEmTramitacao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "Processo/documento está em tramitação. Para o desarquivamento, é necessário que o processo/documento esteja arquivado." }; } }
		public Mensagem DesarquivarProtocoloNaoEstaEmArquivoPrateleiraEstanteInformados { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProtocolo", Texto = "O processo/ documento não foi encontrado para os dados de desarquivamento informado." }; } }
		
	}
}