

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static TituloMsg _tituloMsg = new TituloMsg();
		public static TituloMsg Titulo
		{
			get { return _tituloMsg; }
			set { _tituloMsg = value; }
		}
	}

	public class TituloMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "Título salvo com sucesso." }; } }
		public Mensagem SalvarCorte { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "Após a emissão do DUA não será mais possível alterar informações cadastradas no Título de Informação de Corte. \n Confira o PDF gerado antes da emissão do DUA." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "Título editado com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "Título excluído com sucesso." }; } }

		public Mensagem NaoEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Título não encontrado." }; } }

		public Mensagem AutorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AutorNome", Texto = "Autor do título é obrigatório." }; } }
		public Mensagem DataCriacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataCriacao", Texto = "Data de criação do título é obrigatória." }; } }
		public Mensagem SetorCadastroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SetorCadastro", Texto = "Setor de cadastro do título é obrigatório." }; } }
		public Mensagem SituacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SituacaoTexto", Texto = "Situação do título é obrigatória." }; } }
		public Mensagem LocalEmissaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "LocalEmissao", Texto = "Local da emissão do título é obrigatório." }; } }
		public Mensagem ModeloObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Modelos", Texto = "Modelo do título é obrigatório." }; } }
		public Mensagem NumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Numero", Texto = "Número do título é obrigatório." }; } }

		public Mensagem ProcessoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloNumero", Texto = "Processo é obrigatório." }; } }
		public Mensagem DocumentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloNumero", Texto = "Documento é obrigatório." }; } }		
		public Mensagem EmpreendimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Numero", Texto = "Empreendimento do título é obrigatório." }; } }
		public Mensagem ProcessoOuEmpreendimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloNumero", Texto = "Processo ou empreendimento obrigatório." }; } }
		public Mensagem DocumentoOuEmpreendimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloNumero", Texto = "Documento ou empreendimento obrigatório." }; } }		

		public Mensagem NumeroInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Numero", Texto = "Número do título inválido. O formato correto é número/ano." }; } }
		public Mensagem NumeroCadastrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Numero", Texto = "Número do título já ultilizado para este modelo." }; } }		
		
		public Mensagem ProcDocSemEmpAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao,   Texto = "Não existe empreendimento associado ao processo/documento." }; } }

		public Mensagem ProcessoPosse { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloNumero", Texto = "O Processo não está na posse do funcionário logado." }; } }
		public Mensagem DocumentoPosse { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloNumero", Texto = "O Documento não está na posse do funcionário logado." }; } }
		
		public Mensagem DataCriacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Data de criação do título é inválida." }; } }

		public Mensagem ProcessoPosseEditar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloNumero", Texto = "É preciso ter a posse do processo ao qual o processo do título está apensado para editá-lo." }; } }
		public Mensagem DocumentoPosseEditar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloNumero", Texto = "É preciso ter a posse do processo ao qual o documento do título está juntado para editá-lo." }; } }

		public Mensagem ModeloDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O modelo de título selecionado está desativado." }; } }
		public Mensagem ModeloSetor { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O setor de cadastro não emite mais este modelo de título." }; } }
		public Mensagem ModeloNaoPossuiPdf { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Modelo de título não possui PDF." }; } }
		public Mensagem ModeloCodigoNaoEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Código do modelo de título não encotrado. Modelo pode estar desativado." }; } }

		public Mensagem AutorSetor { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Funcionário não pertence ao setor do título." }; } }

		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "spanInputFile", Texto = "Arquivo é obrigatório." }; } }
		public Mensagem ArquivoTipoPdf { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "spanInputFile", Texto = "O Arquivo deve ser um PDF." }; } }

		public Mensagem AssinanteSetorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Setor é obrigatorio." }; } }
		public Mensagem AssinanteFuncionarioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Funcionário é obrigatorio." }; } }
		public Mensagem AssinanteCargoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Cargo é obrigatorio." }; } }
		public Mensagem AssinanteJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Assinante ja adicionado." }; } }
		public Mensagem AssinanteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "blocoAssinante", Texto = "Assinante é obrigatório." }; } }
		public Mensagem AssinanteDesmarcar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este modelo de título não possui mais assinante. Favor desmarcá-los." }; } }
		public Mensagem AssinanteSetorSemResponsavel { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não foi encontrado responsável no setor assinante do modelo de título." }; } }

		public Mensagem AssinanteInvalidoDesmarcar(string nome) 
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O \"{0}\" não pode mais ser assinante deste modelo de título. Favor desmarcá-lo", nome) }; 
		}

		public Mensagem ProcessoNaoPossuiSetorModelo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O processo deve estar no mesmo setor de cadastro do título." }; } }
		public Mensagem DocumentoNaoPossuiSetorModelo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O documento deve estar no mesmo setor de cadastro do título." }; } }
		public Mensagem RequerimentoNaoPossuiSetorModelo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O requerimento deve estar no mesmo setor de cadastro do título." }; } }

		public Mensagem ProcessoNaoPossuiModelo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O processo não pode ser associado, pois não possui solicitação do modelo selecionado." }; } }
		public Mensagem DocumentoNaoPossuiModelo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O documento não pode ser associado, pois não possui solicitação do modelo selecionado." }; } }
		public Mensagem RequerimentoNaoPossuiModelo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este modelo de título declaratório não foi solicitado neste requerimento." }; } }

		public Mensagem AtividadeNaoPossuiModelo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A atividade selecionada não possui solicitação do modelo selecionado." }; } }

		public Mensagem TituloSemCondicionante { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este título não possui condicionantes." }; } }
		public Mensagem CondicionanteExcluirPoisModeloDeTituloNaoPossuiMaisCondicionantes { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este título não possui mais condicionantes. Favor removê-las." }; } }
		public Mensagem DestinatarioEmailsDesmarcarPoisModeloDeTituloNaoMaisEnviaEmails { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O modelo de título não permite mais envio de e-mail. Favor remover os destinatários." }; } }
		public Mensagem DestinatarioEmailsInexistentes { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Não existem destinatários com e-mail cadastrado." }; } }

		public Mensagem IniciarEmInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Erro,   Texto = "Configuração de \"iniciar em\" deve estar no formato número/ano." }; } }

		public Mensagem ModeloEmitido(string modelo, bool isProcesso)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloNumero", Texto = String.Format("Já existe um título de {0} cadastrado para o {1}. Não é possível cadastrar outro para o mesmo {1}.", modelo, (isProcesso) ? "processo" : "documento") };
		}

		public Mensagem ModeloEmitidoRequerimento(string modelo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = ".ddlProcessosDocumentos", Texto = String.Format("Já existe um título de \"{0}\" cadastrado para o requerimento.", modelo) };
		}

		public Mensagem ProcessoJuntado(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloNumero", Texto = String.Format("O processo não pode ser associado, pois não está apensado ao processo \"{0}\".", numero) };			
		}

		public Mensagem ObrigatorioProcDocEmp(string protocolo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloNumero", Texto = String.Format("É necessário informar {0} ou empreendimento.", protocolo) };
		}

		public Mensagem EmpreendimentoAlterado(bool isProcesso)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Titulo_Empreendimento_Denominador", Texto = String.Format("O empreendimento do {0} foi alterado. Favor salvar essa alteração no título.", isProcesso?"processo":"documento") };
		}

		public Mensagem DocumentoApensado(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProtocoloNumero", Texto = String.Format("O documento não pode ser associado, pois está Apensado ao processo \"{0}\".", numero) };
		}

		public Mensagem SituacaoEditar(string sitacaoAtual)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível editar o título na situação \"{0}\"", sitacaoAtual) };
		}

		public Mensagem SituacaoExcluir(string sitacaoAtual)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível excluir o título na situação \"{0}\"", sitacaoAtual) };
		}

		public Mensagem MensagemExcluir(string numero, string modelo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Tem certeza que deseja remover o título {0} - {1}?", numero, modelo) };
		}

		public Mensagem AtividadeNaoPossuiSolicitacaoTituloDoModelo(string modelo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A atividade não pode ser selecionada, pois não possui solicitação de título do modelo \"{0}\".", modelo) };
		}

		public Mensagem ResponsavelNaoPodeSerDestinatarioEmail(string responsavel)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O \"{0}\" não pode mais ser destinatário de e-mail deste título.", responsavel) };
		}

		public Mensagem TitulosProtocolosDiferentes { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possivel emitir título de pendência para este tipo de protocolo." }; } }

		public Mensagem ProtocoloNaoEstaEmPosse(bool isProcesso, string numero)
		{
			return new Mensagem() {  Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possivel excluir o título pois o "+ (isProcesso?"processo":"documento")+" número: {0} não está em sua posse.", numero) };
		}

		public Mensagem CroquiNaoGerado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O croqui das caracterizações relacionadas a este título ainda não foi gerado. Por favor aguarde alguns minutos e tente novamente." }; } }


		public Mensagem RequerimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Requerimento", Texto = "Requerimento do título é obrigatório." }; } }
		public Mensagem RequerimentoSemEmpreendimento { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Não existe empreendimento associado ao requerimento." }; } }
		public Mensagem RequerimentoSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O requerimento deve estar na situação \"Finalizado\" para associá-lo." }; } }
		public Mensagem SituacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NovaSituacao", Texto = "Nova situação é obrigatória." }; } }
		public Mensagem MotivoEncerramentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "MotivoEncerramento", Texto = "Motivo de encerramento é obrigatório." }; } }
		public Mensagem MotivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Motivo", Texto = "Motivo é obrigatório." }; } }
		public Mensagem AutorDiferenteExcluir { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O funcionário logado deve ser o autor do título declaratório." }; } }
		public Mensagem ProjetoDigitalSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O projeto digital deve estar na situação \"Aguardando importação\" para associá-lo." }; } }

		public Mensagem PeriodoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Por favor preencha o período de início e de fim." }; } }
		public Mensagem PeriodoFormato { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A data não está no formato correto." }; } }
		public Mensagem PeriodoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A data não é uma data válida." }; } }
	}
}