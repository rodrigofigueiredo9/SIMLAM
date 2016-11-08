

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ProcessoMsg _processoMsg = new ProcessoMsg();

		public static ProcessoMsg Processo
		{
			get { return _processoMsg; }
		}
	}

	public class ProcessoMsg
	{
		public Mensagem NumeroInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Número de processo inválido." }; } }
		public Mensagem Inexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O processo não está cadastrado." }; } }

		public Mensagem NumeroAutuacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Processo_NumeroAutuacao", Texto = "Número do SEP é obrigatório." }; } }
		public Mensagem NumeroAutuacaoFormato { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Processo_NumeroAutuacao", Texto = "Formato do número do SEP é inválido." }; } }
		public Mensagem PossuiNumeroSEPObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Possui número do SEP é obrigatório." }; } }
		public Mensagem DataAutuacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Processo_DataAutuacao", Texto = "Data da autuação é obrigatória." }; } }
		public Mensagem DataAutuacaoMaiorAtual { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Processo_DataAutuacao", Texto = "Data de autuação deve ser menor ou igual a data atual." }; } }

		public Mensagem ChecagemObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ChecagemRoteiro_Id", Texto = "Checagem de itens de roteiro é obrigatória." }; } }
		public Mensagem RequerimentoSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O requerimento deve estar finalizado para ser associado." }; } }
		public Mensagem RequerimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Requerimento_Id, #Requerimento_Requerimento_Id", Texto = "Requerimento Padrão é obrigatório." }; } }
		public Mensagem FiscalizacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Fiscalizacao_Id, #Fiscalizacao_Fiscalizacao_Id", Texto = "Fiscalização é obrigatória." }; } }

		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Processo_Arquivo_Nome", Texto = "Arquivo é obrigatório." }; } }
		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Atividade solicitada é obrigatória." }; } }
		public Mensagem FinalidadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Finalidade é obrigatória." }; } }
		public Mensagem TipoTituloObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Tipo de título é obrigatório." }; } }
		public Mensagem Finalizado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Processo finalizado." }; } }

		public Mensagem PosseProcessoNecessaria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do processo." }; } }
		public Mensagem PosseProcessoNecessariaAtividade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do processo para encerrar sua atividade." }; } }
		public Mensagem PosseProcessoNecessariaEditar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do processo para editá-lo." }; } }
		public Mensagem PosseProcessoNecessariaExcluir { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do processo para excluí-lo." }; } }

		public Mensagem InexistenteNumero(string processoNumero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O processo {0} não está cadastrado.", processoNumero) };
		}

		public Mensagem NumeroAutuacaoJaExistente(string processoNumero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Processo_NumeroAutuacao", Texto = String.Format("O nº de autuação no SEP já foi utilizado. Verifique o nº informado ou verifique o processo registrado sob nº {0}.", processoNumero) };
		}

		public Mensagem AtividadeExcluir(string modeloNome, string tituloNumero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A atividade não pode ser removida pois está associada a {0} {1}.", modeloNome, tituloNumero) };
		}

		public Mensagem MensagemExcluir(string numero)
		{
			return new Mensagem() { Texto = String.Format("Tem certeza que deseja excluir o processo {0}?", numero) };
		}

		public Mensagem ExcluirApensadoPai(string numeroProcesso)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível excluir este processo, pois o mesmo está apensado ao processo {0}.", numeroProcesso) };
		}

		public Mensagem ExcluirApensadoFilho(string numeroProcessoFilho)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível excluir este processo, pois o processo {0} está apensado a ele. Retire os processos apensados.", numeroProcessoFilho) };
		}

		public Mensagem ExcluirDocumentosJuntados(string numeroDocumento)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível excluir este processo, pois o documento {0} está juntado a ele. Retire os documentos juntados.", numeroDocumento) };
		}

		public Mensagem ExcluirAcompanhamentoFiscalizacao(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível excluir o processo pois a fiscalização Nº {0} possui acompanhamento(s).", numero) };
		}

		public Mensagem ExcluirCARSolicitacao(string situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O processo não pode ser excluído, pois está associado a uma Solicitação de Inscrição no CAR/ES na situação \"{0}\".", situacao) };
		}

		public Mensagem ExcluirAnalise { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível excluir este processo, pois ele está analisado." }; } }
		public Mensagem MotivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Atividade_Motivo", Texto = "Motivo é obrigatório." }; } }

		public Mensagem NaoEncontrouRegistros { get { return Mensagem.Padrao.NaoEncontrouRegistros; } }

		public Mensagem Salvar(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Processo registrado sob nº {0}", numero) };
		}

		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Processo editado com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Processo excluído com sucesso." }; } }

		public Mensagem ProcessoNumeroNaoProtocolado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Processo_Numero", Texto = "O processo não está protocolado no órgão." }; } }
		public Mensagem ProcessoNumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Processo_Numero", Texto = "Número do processo é obrigatório." }; } }
		public Mensagem ProcessoNumeroInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Processo_Numero", Texto = "Número de processo inválido. O formato correto é número/ano." }; } }
		public Mensagem ProcessoTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Processo_Tipo_Id", Texto = "Tipo de processo é obrigatório." }; } }

		public Mensagem EmTramitacao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Processo em tramitação." }; } }

		public Mensagem AtividadejaAdicionada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Atividade solicitada já adicionada." }; } }
		public Mensagem ResponsaveljaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Responsável tecnico já adicionado." }; } }
		public Mensagem DataCriacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Processo_DataCadastro_DataTexto", Texto = "Data de registro é obrigatória." }; } }
		public Mensagem DataCriacaoMaiorAtual { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Processo_DataCadastro_DataTexto", Texto = "Data de registro deve ser menor ou igual a data atual." }; } }
		public Mensagem QuantVolumesObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Processo_Volume", Texto = "Quantidade de volumes é obrigatória." }; } }

		public Mensagem ChecagemRoteirosDifentesRoteiroAtuais { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Existem diferentes roteiros na checagem de itens e no requerimento padrão. Favor alterar a checagem." }; } }
		public Mensagem ProcessoSemRequerimentos { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este processo não possui requerimentos adicionados." }; } }

		public Mensagem InteressadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Interessado_NomeRazaoSocial, #Interessado_CPFCNPJ", Texto = "Um interessado deve ser associado ao processo." }; } }
		public Mensagem InteressadoDifProtocolo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Interessado_NomeRazaoSocial, #Interessado_CPFCNPJ", Texto = "O interessado protocolado é diferente do interessado adicionado neste processo." }; } }
		public Mensagem InteressadoEndDifProtocolo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O endereço protocolado é diferente do endereço do interessado adicionado neste processo." }; } }
		public Mensagem ResponsavelTecnicoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Responsavel técnico é obrigatório." }; } }
		public Mensagem ResponsavelTecnicoSemPreencher { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Já existe responsável técnico sem preencher." }; } }
		public Mensagem ResponsavelTecnicoRemover { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Tem certeza que deseja remover o responsável técnico #texto do processo?" }; } }

		public Mensagem SetorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Processo_SetorId", Texto = "Setor é obrigatório." }; } }

		public Mensagem ResponsavelNomeRazaoObrigatorio(int index)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("resp_{0}__NomeRazao", index), Texto = String.Format("Nome/Razão  social do responsável {0} é obrigatório.", index + 1) };
		}

		public Mensagem ResponsavelCpfCnpjObrigatorio(int index)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("resp_{0}__cpfCnpj", index), Texto = String.Format("CPF/CNPJ do responsável {0} é obrigatório.", index + 1) };
		}

		public Mensagem ResponsavelFuncaoObrigatorio(int index)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("resp_{0}__funcao", index), Texto = String.Format("Função do responsável {0} é obrigatório.", index + 1) };
		}

		public Mensagem ResponsavelARTObrigatorio(int index)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("resp_{0}__art", index), Texto = String.Format("Número da ART do responsável {0} é obrigatório.", index + 1) };
		}

		public Mensagem ChecagemJaAssociada(string tipo, string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A checagem já está associada ao {0} {1}.", tipo, numero) };
		}

		public Mensagem RequerimentoJaAssociado(string tipo, string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O requerimento já está associado ao {0} {1}.", tipo, numero) };
		}

		public Mensagem ProcessoApensado(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O processo está apensado ao processo {0}.", numero) };
		}

		public Mensagem EditarProcessoApensado(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível editar o processo, pois ele está apensado ao processo registrado sob nº {0}.", numero) };
		}

		public Mensagem DocumentoNaoPodeJuntarPoisNaoPossuiPosse { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do documento para juntá-lo ao processo." }; } }

		public Mensagem DocumentoNaoPodeJuntarProcessoDiferenteAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O documento não pode ser juntado, pois o processo associado a ele é diferente do processo verificado." }; } }
		public Mensagem DocumentoNaoPodeJuntarPoisNaoEstaNoMesmoSetor { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O documento não pode ser juntado, pois não está no mesmo setor do processo verificado." }; } }
		public Mensagem ProcessoNaoPodeApensarPoisNaoEstaNoMesmoSetor { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O processo não pode ser apensado, pois não está no mesmo setor do processo verificado." }; } }

		public Mensagem DocumentoNaoPodeJuntarPoisEstaJuntado(string processoNumero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O documento já está juntado ao processo registrado sob nº {0}.", processoNumero) };
		}

		public Mensagem ProcessoNaoPodeApensarPoisNaoPossuiPosse { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do processo para juntar/apensar." }; } }
		public Mensagem ProcessoNaoPodeSerApensadoPoisNaoPossuiPosse { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do  processo para apensá-lo ao processo." }; } }

		public Mensagem ProcessoNaoPodeApensarPoisTemFilhos { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O processo não pode ser apensado pois tem documentos ajuntados ou processos apensados a ele." }; } }

		public Mensagem ProcessoNaoPodeApensarJuntarPoisEstaApensado(string processoNumero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível apensar/juntar no processo, pois ele está apensado ao processo registrado sob nº {0}.", processoNumero) };
		}

		public Mensagem ProcessoNaoPodeApensarPoisPaiEstaApensado(string processoNumero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O processo {0} não pode ter processos apensados pois este já está apensado a outro processo.", processoNumero) };
		}

		public Mensagem ProcessoNaoPodeApensarPoisEstaApensado(string processoNumero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O processo já está apensado ao processo registrado sob nº {0}.", processoNumero) };
		}

		public Mensagem DocumentoJaEstaNaListaParaSerJuntado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NovoDocumentoNumero", Texto = "O documento já está adicionado." }; } }
		public Mensagem ProcessoJaEstaNaListaParaSerJuntado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NovoProcessoNumero", Texto = "O processo já está adicionado." }; } }

		public Mensagem NaoPodeApensarASiProprio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NovoProcessoNumero", Texto = "O processo não pode ser apensado a ele mesmo." }; } }

		public Mensagem JuntadoApensadoSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Os processos apensados e os documentos juntados do processo foram alterados com sucesso." }; } }

		public Mensagem ChecagemAssociadaTitulo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Checagem não pode ser trocada pois o processo está associado a um título de notificação de pendência." }; } }

		public Mensagem RequerimentoAssociadoTitulo()
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Requerimento associado a título emitido.") };
		}

		public Mensagem ProcessoAssociadoTitulo(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Processo associado ao título número {0}.", numero) };
		}

		public Mensagem ProcessoAssociadoProtocolo(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Processo associado ao documento número {0}.", numero) };
		}

		public Mensagem ProcessoAssociadoDocumento(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Processo associado ao documento número {0}.", numero) };
		}

		public Mensagem ProcessoSemAtividade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este processo não possui atividades." }; } }
		public Mensagem ProtocoloSemAtividade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este protocolo não possui atividades." }; } }

		public Mensagem ChecagemComTituloPendencia { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = " A checagem não pode ser alterada pois existe título de pendência emitida para o processo." }; } }

		public Mensagem DocumentoAtividadesEncerradas { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O processo não pode ser editado, pois suas atividades estão encerradas." }; } }

		public Mensagem MensagemExcluirProcessoComTitulos(string modelo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O processo não pode ser excluído, pois está associado a especificidade do título: {0}.", modelo) };
		}

		public Mensagem ProcessoJaAutuado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O processo já está autuado." }; } }
		public Mensagem ProcessoAutuado { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Processo autuado com sucesso." }; } }

		public Mensagem PosseCredenciado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Você não possui a posse desse processo." }; } }
		public Mensagem FiscalizacaoAssociadaPossuiAcompanhamento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A fiscalização possui acompanhamento." }; } }

		#region Fiscalizacao

		public Mensagem FiscalizacaoAssociadaNaoPodeExluir(string numeroFiscalizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível excluir este processo, pois a fiscalização Nº {0} está associada a ele.", numeroFiscalizacao) };
		}

		public Mensagem FiscalizacaoAssociadaNaoPodeExluirSituacao(string numeroFiscalizacao, string situacaoTexto)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível excluir este processo, pois a fiscalização Nº {0} está na situação \"{1}\".", numeroFiscalizacao, situacaoTexto) };
		}

		#endregion

		public Mensagem AssociarDeclaratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O requerimento não deve possuir solicitação de Título Declaratório." }; } }
	}
}