namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CadastroAmbientalRuralMsg _cadastroAmbientalRuralMsg = new CadastroAmbientalRuralMsg();

		public static CadastroAmbientalRuralMsg CadastroAmbientalRural
		{
			get { return _cadastroAmbientalRuralMsg; }
		}
	}

	public class CadastroAmbientalRuralMsg
	{
		public Mensagem Finalizar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = @"Caracterização cadastro ambiental rural salva com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = @"Caracterização cadastro ambiental rural excluída com sucesso." }; } }
		public Mensagem CedenteNaoPossuiTCPFRLCConcluido { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = @"É necessário que o empreendimento cedente possua o Título ‘Termo de Compromisso de Preservação e ou Formação de Reserva Legal por Compensação’ na situação ‘Concluído’ e averbado no Cartório." }; } }

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização de cadastro ambiental rural deste empreendimento?" }; } }
		public Mensagem ConfirmReprocessarProjeto { get { return new Mensagem() { Texto = "As informações processadas serão apagadas. Tem certeza que deseja reprocessar?" }; } }
		public Mensagem ConfirmFinalizarProjeto { get { return new Mensagem() { Texto = "Deseja realmente finalizá-lo?" }; } }

		public Mensagem OcorreuAlteracaoApos2008Obrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "OcorreuAlteracaoApos2008", Texto = "Ocorreu alteração no tamanho da área do imóvel após 22/07/2008 é obrigatório." }; } }
		public Mensagem MunicipioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Municipio", Texto = "Município é obrigatório." }; } }
		public Mensagem MunicipioForaES { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Municipio", Texto = "Para cadastrar a caracterização 'Cadastro Ambiental Rural', é necessário que o empreendimento esteja localizado no Estado do Espírito Santo." }; } }
		public Mensagem ATPDocumento2008Obrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ATPDocumento2008", Texto = "ATP Documento em 22/07/2008 é obrigatório." }; } }
		public Mensagem ModuloFiscalObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ModuloFiscalHA", Texto = "1 Modulo Fiscal (ha) é obrigatório." }; } }
		public Mensagem ATPQuantidadeModuloFiscalObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ATPQuantidadeModuloFiscal", Texto = "Qtd. Módulo fiscal da ATP é obrigatório." }; } }

		public Mensagem DispensaARLNaoMarcado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"É necessário que o empreendimento possua ARL do empreendimento maior ou igual a 20%. Verifique a área de reserva legal do empreendimento." }; } }
		public Mensagem VistoriaAprovacaoCARObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Vistoria para aprovação do CAR é obrigatório.", Campo = "VistoriaAprovacaoCAR" }; }  }
		public Mensagem DispensaARL_ReservaLegalEmOutroCAR { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"O empreendimento não pode possuir dispensa de ARL e ‘Reserva Legal em outro CAR’. É necessário que o empreendimento possua somente um dos casos." }; } }
		public Mensagem CedenteNaoPodeCederRLEmUso { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"O empreendimento não pode ceder uma área de reserva legal compensada com situação ‘Em uso’. O empreendimento cedente só poderá doar área de reserva legal com situação igual a ‘Preservada’ ou ‘Em recuperação’. Verifique a área de reserva legal do empreendimento." }; } }
		public Mensagem ReceptorNaoPodeReceberRLEmUso { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"O empreendimento não pode receber uma área de reserva legal compensada com situação ‘Em uso’. O empreendimento receptor só poderá receber área de reserva legal com situação igual a ‘Preservada’ ou ‘Em recuperação’. Verifique a área de reserva legal do empreendimento (cedente)." }; } }
		public Mensagem NaoPodeCederRLNaoCaracterizada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"O empreendimento não pode ceder uma área de reserva legal com situação ‘Não caracterizada’. O empreendimento cedente só poderá doar área de reserva legal com situação igual a ‘Preservada’ ou ‘Em recuperação’. Verifique a área de reserva legal do empreendimento." }; } }
		public Mensagem ReceptorNaoPodeReceberRLNaoCaracterizada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"O empreendimento não pode ceder uma área de reserva legal com situação ‘Não caracterizada’. O empreendimento cedente só poderá doar área de reserva legal com situação igual a ‘Preservada’ ou ‘Em recuperação’. Verifique a área de reserva legal do empreendimento." }; } }
		public Mensagem EmpreendimentoCedenteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Nº do CAR é obrigatório.", Campo = "N_CAR_Cedente" }; } }
		public Mensagem EmpreendimentoReceptorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Nº do CAR é obrigatório.", Campo = "N_CAR_Receptor" }; } }
	}
}