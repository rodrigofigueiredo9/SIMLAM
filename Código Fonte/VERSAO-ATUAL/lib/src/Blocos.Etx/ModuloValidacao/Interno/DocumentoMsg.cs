

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static DocumentoMsg _documentoMsg = new DocumentoMsg();

		public static DocumentoMsg Documento
		{
			get { return _documentoMsg; }
		}
	}

	public class DocumentoMsg
	{
		public Mensagem NumeroInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo="#txtNumero", Texto = "Número de documento inválido." }; } }
		public Mensagem Inexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O documento não está cadastrado." }; } }
		public Mensagem ChecagemPendenciaSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Só é permitido associar checagem de pendências com situação finalizada." }; } }
		public Mensagem ProcessoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Protocolo_Id", Texto = "Processo/Documento é obrigatório." }; } }
		public Mensagem ChecagemPendenciaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ChecagemPendencia_Id", Texto = "Número da checagem de pendência é obrigatório." }; } }
		public Mensagem ChecagemObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ChecagemRoteiro_Id", Texto = "Checagem de itens de roteiro é obrigatória." }; } }
		public Mensagem RequerimentoSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O requerimento deve estar finalizado para ser associado." }; } }
		public Mensagem RequerimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Requerimento_Id, #Requerimento_Requerimento_Id", Texto = "Requerimento é obrigatório." }; } }
		public Mensagem ChecagemPendenciaAlterada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A checagem de pendências nao pode ser alterada." }; } }

		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Arquivo_Nome", Texto = "Arquivo é obrigatório." }; } }
		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Atividade solicitada é obrigatória." }; } }
		public Mensagem FinalidadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Finalidade é obrigatória." }; } }
		public Mensagem TipoTituloObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Tipo de título é obrigatório." }; } }

		public Mensagem PosseDocumentoNecessaria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do documento." }; } }
		public Mensagem PosseDocumentoNecessariaAtividade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do documento para encerrar sua atividade." }; } }
		public Mensagem PosseDocumentoNecessariaEditar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do documento para editá-lo." }; } }
		public Mensagem PosseDocumentoNecessariaExcluir { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do documento para excluí-lo." }; } }

		public Mensagem InexistenteNumero(string documentoNumero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O documento {0} não está cadastrado.", documentoNumero) };
		}

		public Mensagem SemRequerimentoAtividades { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este documento não possui atividades." }; } }

		public Mensagem NaoPossivelExcluirDocumentoFilho(string numeroProcesso)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O documento não pode ser excluído, pois está juntado ao processo {0}.", numeroProcesso) };
		}

		public Mensagem MotivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProcessoAtividade_Motivo", Texto = "Motivo é obrigatório." }; } }

		public Mensagem NaoEncontrouRegistros { get { return Mensagem.Padrao.NaoEncontrouRegistros; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Documento editado com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Documento excluído com sucesso." }; } }

		public Mensagem Salvar(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Documento registrado sob nº {0}", numero) };
		}

		public Mensagem DocumentoNumeroNaoProtocolado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Numero", Texto = "O documento não está protocolado no órgão." }; } }
		public Mensagem DocumentoNumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Numero, #txtNumero", Texto = "Número do documento é obrigatório." }; } }
		public Mensagem DocumentoNumeroInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Numero", Texto = "Número de documento inválido. O formato correto é número/ano." }; } }
		public Mensagem DocumentoTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Tipo_Id", Texto = "Tipo de documento é obrigatório." }; } }
		public Mensagem DocumentoJaFinalizado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O documento já está finalizado." }; } }
		public Mensagem DocumentoAtividadesEncerradas { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O documento não pode ser editado, pois suas atividades estão encerradas." }; } }

		public Mensagem FiscalizacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Fiscalizacao_Id, #Fiscalizacao_Fiscalizacao_Id", Texto = "Fiscalização é obrigatória." }; } }

		public Mensagem EmTramitacao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Documento em tramitação." }; } }

		public Mensagem AtividadejaAdicionada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Atividade solicitada já adicionada." }; } }
		public Mensagem ResponsaveljaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Responsável tecnico já adicionado." }; } }
		public Mensagem DataCriacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_DataCadastro_DataTexto", Texto = "Data de registro é obrigatória." }; } }
		public Mensagem DataCriacaoMaiorAtual { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_DataCadastro_DataTexto", Texto = "Data de registro deve ser menor ou igual a data atual." }; } }
		public Mensagem QuantidadeDocumentoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Volume", Texto = "Quantidade de documento é obrigatório." }; } }
		public Mensagem NomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Nome", Texto = "Nome é obrigatório." }; } }
		public Mensagem AssuntoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Assunto", Texto = "Assunto é obrigatório." }; } }
		public Mensagem DescricaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Descricao", Texto = "Descrição da Comunicação Interna é obrigatório." }; } }

		public Mensagem RoteirosCheckDifRequerimento { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Existem diferentes roteiros na checagem de itens e no requerimento padrão." }; } }

		public Mensagem ExcluirAcompanhamentoFiscalizacao(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível excluir o documento pois a fiscalização Nº {0} possui acompanhamento(s).", numero) };
		}

		public Mensagem ExcluirCARSolicitacao(string situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O documento não pode ser excluído, pois está associado a uma Solicitação de Inscrição no CAR/ES na situação \"{0}\".", situacao) };
		}

		public Mensagem InteressadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Interessado_NomeRazaoSocial, #Interessado_CPFCNPJ", Texto = "Um interessado deve ser associado ao documento." }; } }
		public Mensagem InteressadoDifProtocolo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Interessado_NomeRazaoSocial, #Interessado_CPFCNPJ", Texto = "O interessado protocolado é diferente do interessado adicionado neste documento." }; } }
		public Mensagem InteressadoEndDifProtocolo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O endereço protocolado é diferente do endereço do interessado adicionado neste documento." }; } }
		public Mensagem ExcluirAnalise { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível excluir este documento, pois ele está analisado." }; } }
		public Mensagem ResponsavelTecnicoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Responsavel técnico é obrigatório." }; } }
		public Mensagem ResponsavelTecnicoSemPreencher { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Já existe responsável técnico sem preencher." }; } }
		public Mensagem ResponsavelTecnicoRemover { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Tem certeza que deseja remover o responsável técnico #texto do documento?" }; } }

		public Mensagem Posse { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter posse do documento." }; } }

		public Mensagem SetorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_SetorId", Texto = "Setor é obrigatório." }; } }

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

		public Mensagem ChecagemPendenciaJaAssociada(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Checagem de pendências já associada ao documento número {0}.", numero) };
		}

		public Mensagem ChecagemJaAssociada(string tipo, string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Checagem de itens de roteiro já associada ao {0} com o número {1}.", tipo, numero) };
		}

		public Mensagem RequerimentoJaAssociado(string tipo, string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Requerimento já associado ao {0} com o número {1}.", tipo, numero) };
		}

		public Mensagem DocumentoJuntado(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O documento está juntado ao processo {0}.", numero) };
		}

		public Mensagem EditarDocumentoJuntado(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível editar o documento, pois ele está juntado ao processo registrado no nº {0}.", numero) };
		}

		public Mensagem ChecagemAssociadaTitulo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Checagem não pode ser trocada pois o documento está associado a um título de notificação de pendência." }; } }

		public Mensagem RequerimentoAssociadoTitulo()
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Requerimento associado a título emitido.") };
		}

		public Mensagem ChecagemComTituloPendencia { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A checagem não pode ser alterada, pois existe título de pendência emitido para o documento." }; } }

		public Mensagem MensagemExcluir(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Tem certeza que deseja excluir o documento {0}?", numero) };
		}

		public Mensagem MensagemExcluirDocumentoComTitulos(string modelo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O documento não pode ser excluído, pois está associado a especificidade do título: {0}.", modelo) };
		}

		public Mensagem DocumentoJaAutuado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O documento já está autuado." }; } }
		public Mensagem DocumentoAutuado { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Documento autuado com sucesso." }; } }
		public Mensagem DocNaoConverter(string strTipo) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("O documento do tipo {0} não pode ser convertido em processo.", strTipo) }; }
		public Mensagem InformeNDocumento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Informe o número de um documento.", Campo = "#txtNumero" }; } }
		public Mensagem DocConvertidoSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Documento convertido em processo com sucesso." }; } }

		public Mensagem PosseCredenciado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Você não possui a posse desse documento." }; } }

		public Mensagem PossuiAI_TEI_TAD { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para fiscalização que possuir AI, TEI e/ou TAD deverá ser cadastrado um processo e não um documento." }; } }

		public Mensagem AssociarDeclaratorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O requerimento não deve possuir solicitação de Título Declaratório." }; } }
	}
}