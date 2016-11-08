

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static LaudoVistoriaFundiariaMsg _laudoVistoriaFundiariaMsg = new LaudoVistoriaFundiariaMsg();
		public static LaudoVistoriaFundiariaMsg LaudoVistoriaFundiariaMsg
		{
			get { return _laudoVistoriaFundiariaMsg; }
			set { _laudoVistoriaFundiariaMsg = value; }
		}
	}

	public class LaudoVistoriaFundiariaMsg
	{
		public Mensagem DestinatarioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Destinatario", Texto = "O Destinatário é obrigatório." }; } }
		public Mensagem RegularizacaoDominioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_RegularizacaoDominio", Texto = "Pelo menos uma comprovação de Posse é obrigatória." }; } }
		public Mensagem RegularizacaoDominioInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_RegularizacaoDominio", Texto = "A comprovação de posse selecionada não existe mais na caracterização de regularização fundiária." }; } }
		public Mensagem RegularizacaoDominioJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_RegularizacaoDominio", Texto = "Comprovação de posse já adicionada." }; } }

		public Mensagem RegularizacaoComprovacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_RegularizacaoDominio", Texto = "Comprovação de posse obrigatória." }; } }


		public Mensagem CaracterizacaoAlterada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A especificidade do Título deverá ser atualizada" }; } }
		public Mensagem RegularizacaoFundiariaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para cadastrar este modelo de título é necessário que a caracterização  Regularização Fundiária esteja válida." }; } }
		public Mensagem RegularizacaoFundiariaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterizacao Dominialidade deve estar cadastrada." }; } }

		public Mensagem DataVistoriaMaiorAtual { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_DataVistoria_DataTexto", Texto = "Data da Vistoria não poder ser maior que data atual." }; } }
		public Mensagem DataVistoriaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,Campo = "Laudo_DataVistoria_DataTexto", Texto = "Data da Vistoria inválida." }; } }

		public Mensagem PecaTecnicaObrigatorio(String atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Para emitir este modelo de Título é necessário gerar a peça técnica da atividade de {0}",atividade) };
		}
	}
}
