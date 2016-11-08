

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static AnaliseItemMsg _analiseItemMsg = new AnaliseItemMsg();

		public static AnaliseItemMsg AnaliseItem
		{
			get { return _analiseItemMsg; }
		}
	}

	public class AnaliseItemMsg
	{

		#region Peça Tecnica

		public Mensagem PecaTecnicaNumeroRegistroInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProcDoc", Texto = "Número de registro do processo/documento inválido. O formato correto é número/ano." }; } }

		public Mensagem PecaTecnicaRegistroNaoCadastrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProcDoc", Texto = "O processo/documento não está cadastrado no sistema." }; } }

		public Mensagem PecaTecnicaNecessarioPosse { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProcDoc", Texto = "É preciso ter a posse do processo/documento." }; } }

		public Mensagem PecaTecnicaNaoPossuiRequerimento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProcDoc", Texto = "O processo/documento não possui um requerimento." }; } }

		public Mensagem PecaTecnicaRequerimentoNaoAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProcDoc", Texto = "O requerimento selecionado não está mais associado ao processo." }; } }

		public Mensagem PecaTecnicaElaboradorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PecaTecnica_Elaborador", Texto = "Elaborador é obrigatório." }; } }

		public Mensagem PecaTecnicaAtividadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fsAtividadeSolicitada", Texto = "Atividade Solicitada é obrigatório." }; } }

		public Mensagem PecaTecnicaRespEmpreendimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PecaTecnica_RespEmpreendimento", Texto = "Responsável do empreendimento é obrigatório." }; } }

		public Mensagem PecaTecnicaRespEmpreendimentoJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PecaTecnica_RespEmpreendimento", Texto = "Responsável do empreendimento já está adicionado." }; } }

		public Mensagem PecaTecnicaSetorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Setores", Texto = "Setor de cadastro é obrigatório." }; } }

		public Mensagem PecaTecnicaSalvaComSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,  Texto = "Peça Tecnica Salva com Sucesso." }; } }

		public Mensagem PecaTecnicaNaoPossuiEmpreendimento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O requerimento selecionado não possui empreendimento." }; } }

		public Mensagem PecaTecnicaProjetoDeveSerFinalizado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para gerar a peça técnica é necessário que a dominialidade possua projeto geográfico finalizado." }; } }


		public Mensagem PecaTecnicaProcessoApensado(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProcDoc", Texto = String.Format("O processo está apensado ao processo registrado sob nº {0}.", numero) }; 
		}

		public Mensagem PecaTecnicaDocumentoJuntado(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProcDoc", Texto = String.Format("O documento está juntado ao processo registrado sob nº {0}.",numero) }; 
		}

		#endregion

		public Mensagem NumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProcDoc", Texto = "Número de registro do processo/documento é obrigatório." }; } }
		
		public Mensagem NumeroInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProcDoc", Texto = "Número de registro do processo/documento inválido. O formato correto é número/ano." }; } }
		
		public Mensagem NumeroInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProcDoc", Texto = "O processo/documento não está cadastrado no sistema." }; } }
		
		public Mensagem ProtocoloSemPosse { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProcDoc", Texto = "É preciso ter a posse do processo/documento para analisá-lo." }; } }

		public Mensagem ExisteRequerimento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O processo/documento não pode ser analisado, pois não possui um requerimento." }; } }

		public Mensagem ExistePendencia(bool isProcesso)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O " + ((isProcesso) ? "processo" : "documento") + " não pode ser analisado, pois existe(m) pendência(s) aberta(s)." };
		}

		public Mensagem DocumentoJuntado(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProcDoc", Texto = String.Format("O documento está juntado ao processo registrado sob nº {0} portanto deve ser analisado no processo.", numero) }; 
		}

		public Mensagem ProcessoApensado(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NumeroProcDoc", Texto = String.Format("O processo está apensado ao processo registrado sob nº {0} portanto deve ser analisado no processo principal.", numero) }; 
		}

		public Mensagem AnaliseItemTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Tipo", Texto = "O tipo é obrigatório." }; } }

		public Mensagem AnaliseItemNomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Nome", Texto = "O nome é obrigatório." }; } }

		public Mensagem AnaliseItemProcedimentoAnaliseObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProcedimentoAnalise", Texto = "O procedimento de análise é obrigatório." }; } }

		public Mensagem ItemAnalisadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Deve existir pelo menos um item analisado." }; } }

		public Mensagem AnaliseItemDataValida(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataAnalise", Texto = String.Format("A data de análise do item \"{0}\" é inválida.", nome) };
		}

		public Mensagem AnaliseItemDataAnaliseObrigatorio(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "tr", Texto = String.Format("O data de análise do item \"{0}\" é obrigatório.", nome) };
		}

		public Mensagem AnaliseItemDataAnaliseMenorAtual(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "tr", Texto = String.Format("O data de análise do item \"{0}\" não pode ser menor que a data atual.", nome) };
		}

		public Mensagem AnaliseItemSituacaoObrigatorio(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "tr11", Texto = String.Format("A situação do item \"{0}\" é obrigatório.", nome) }; 
		}

		public Mensagem AnaliseItemMotivoObrigatorio(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Motivo", Texto = String.Format("O motivo do item \"{0}\" é obrigatório.", nome) };
		}

		public Mensagem RoteiroDesativado(int numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = String.Format("O roteiro {0} está desativado.", numero) };
		}

		public Mensagem AnaliseSituacaoComPendencia { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Requerimento não pode ser analisado, pois possui título de pendência." }; } }

		public Mensagem AnaliseSituacaoFinalizada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Requerimento não pode ser analisado, pois sua Análise se encontra finalizada." }; } }

		public Mensagem AnaliseProtocoloSemChecagem(bool isProcesso)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não existe uma checagem associada ao " + ((isProcesso) ? "processo" : "documento") + " selecionado." };
		}

		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Análise realizada com sucesso." }; } }

		public Mensagem VersaoAtualizada(int numero, int? versaoCadastrada, int? versaoAtual, string dataAtualizacao)
		{
			return new Mensagem { Tipo = eTipoMensagem.Informacao, Texto = String.Format(@"O roteiro nº {0} foi atualizado da versão nº {1} para a versão nº {2} em {3}.", numero, versaoCadastrada, versaoAtual, dataAtualizacao) };
		}

		public Mensagem Atualizar { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Deseja atualizar a versão para esta análise de itens ?" }; } }

		public Mensagem ItensAtualizados { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Existe(m) iten(s) que foram atualizados,  por favor salve a análise para continuar." }; } }

		public Mensagem AtividadeRequerimentoEncerrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "'Requerimento não pode ser analisado, pois a(s) atividade(s) esta(ão) encerrada(s)." }; } }

		public Mensagem SituacaoObrigatorio { get { return new Mensagem() { Campo="Situacao", Tipo = eTipoMensagem.Advertencia, Texto = "Situação é obrigatório." }; } }

		public Mensagem MotivoObrigatorio { get { return new Mensagem() { Campo = "Motivo", Tipo = eTipoMensagem.Advertencia, Texto = "Motivo é obrigatório." }; } }

		public Mensagem DescricaoMaximoCaracter(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Descrição do item \"{0}\" deve ter no máximo 500 caracteres.", nome) };
		}

		public Mensagem MotivoMaximoCaracter(string nome, int tamMaxCarac)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Motivo do item \"{0}\" deve ter no máximo {1} caracteres.", nome, tamMaxCarac) };
		}

		public Mensagem ImportarProjetoDigital
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Esta ação irá finalizar a análise deste projeto digital, não permitindo nova análise dos itens das Caracterizações. O sistema fará a importação das Caracterizações do Módulo Credenciado para o Módulo Institucional. Deseja realmente importar o projeto digital?" }; }
		}

		public Mensagem SairSemSalvarDados
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "A análise técnica do processo possui itens que foram analisados, porém não foram salvos. Deseja realmente sair sem salvar os dados da análise dos itens?" }; }
		}

		public Mensagem SalvarDadosObrigatorio
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Salvar os dados da análise é obrigatório." }; }
		}
		
	}
}