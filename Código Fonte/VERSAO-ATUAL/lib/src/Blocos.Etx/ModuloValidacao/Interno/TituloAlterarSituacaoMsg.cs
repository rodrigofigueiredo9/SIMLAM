

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static TituloAlterarSituacaoMsg _tituloAlterarSituacao = new TituloAlterarSituacaoMsg();
		public static TituloAlterarSituacaoMsg TituloAlterarSituacao
		{
			get { return _tituloAlterarSituacao; }
			set { _tituloAlterarSituacao = value; }
		}
	}

	public class TituloAlterarSituacaoMsg
	{
		public Mensagem TituloAltSituacaoSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Situação de título alterada com sucesso." }; } }

		public Mensagem TituloEmitidoConcluir { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O início do prazo deste título está configurado para a data de emissão. Favor concluí-lo." }; } }
		public Mensagem TituloAssinadoConcluir { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "O início do prazo deste título está configurado para a data de assinatura. Favor concluí-lo." }; } }
		public Mensagem TituloEmitidoAssinadoSemPrazoConcluir { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Este título não possui mais prazo, portanto a sua conclusão se dá na emissão. Favor concluí-lo." }; } }

		public Mensagem GerarPdfObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Gerar PDF do título é obrigatório para emitir." }; } }
		public Mensagem ModeloNaoPossuiPdf { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Modelo de título não possui PDF." }; } }

		public Mensagem ProcessoPosseAltSituacao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "É preciso ter a posse do processo ao qual o processo do título está apensado para alterar sua situação." }; } }
		public Mensagem DocumentoPosseAltSituacao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "É preciso ter a posse do processo ao qual o documento do título está juntado para alterar sua situação." }; } }

		public Mensagem MotivoEncerramentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Motivo", Texto = "Motivo do encerramento é obrigatório." }; } }
		public Mensagem PrazoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Prazo", Texto = "Prazo é obrigatório." }; } }
		public Mensagem PrazoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Prazo", Texto = "Prazo é inválido." }; } }
		public Mensagem DiasProrrogadosObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DiasProrrogados", Texto = "Dias prorrogados é obrigatório." }; } }
		public Mensagem CondicionantesRemover { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este título não possui mais condicionantes. Favor removê-las." }; } }

		public Mensagem TituloNaoPossuiSolicitacaoDeInscricao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O Empreendimento do Título não possui uma Solicitação de Inscrição do CAR com situação \"Válido\" e situação do Arquivo \"Arquivo Entregue\"." }; } }

		public Mensagem NumeroAnoEmissaoAno { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataEmissao", Texto = "O ano do número do título deve ser igual ao ano da emissão." }; } }

		public Mensagem DataIgualMenorAtual(string campo, string tipoData)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("Data de {0} deve ser menor ou igual a data atual.", tipoData) };
		}

		public Mensagem DataObrigatoria(string campo, string tipoData)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("Data de {0} é obrigatória.", tipoData) };
		}

		public Mensagem DataInvalida(string campo, string tipoData)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("Data de {0} inválida.", tipoData) };
		}

		public Mensagem SituacaoInvalida(string situacaoNova, string situacaoNecessaria)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Situação inválida, so é possivel mudar o titulo para {0} com a situação {1}.", situacaoNova, situacaoNecessaria) };
		}

		public Mensagem DataDeveSerSuperior(string campo, string dataValidar, string dataInferior)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("Data de {0} deve ser maior ou igual à data de {1}.", dataValidar, dataInferior) };
		}
	}
}