namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
        private static MultaMsg _multaMsg = new MultaMsg();
		public static MultaMsg MultaMsg
		{
			get { return _multaMsg; }
			set { _multaMsg = value; }
		}
	}

	public class MultaMsg
	{
		public Mensagem  Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Multa salva com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Multa editada com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Multa excluída com sucesso." }; } }
		
		public Mensagem ClassificacaoObrigatorio { get { return new Mensagem() { Campo = "Infracao_Classificacao", Tipo = eTipoMensagem.Advertencia, Texto = "Classificação é obrigatório." }; } }		
		public Mensagem TipoInfracaoObrigatorio { get { return new Mensagem() { Campo = "Infracao_Tipo", Tipo = eTipoMensagem.Advertencia, Texto = "Tipo de infração é obrigatório." }; } }
		public Mensagem ItemObrigatorio { get { return new Mensagem() { Campo = "Infracao_Item", Tipo = eTipoMensagem.Advertencia, Texto = "Item é obrigatório." }; } }
		public Mensagem CamposObrigatorioo { get { return new Mensagem() { Campo = "divCampos input:text[value='']", Tipo = eTipoMensagem.Advertencia, Texto = "É necessario preencher todos os campos." }; } }
		public Mensagem QuestionariosObrigatorio { get { return new Mensagem() { Campo = "divQuestionarios", Tipo = eTipoMensagem.Advertencia, Texto = "É necessário responder todas as perguntas." }; } }
		public Mensagem InfracaoAutuadaObrigatorio { get { return new Mensagem() { Campo = "Infracao_IsAutuada", Tipo = eTipoMensagem.Advertencia, Texto = "Auto de infração é obrigatório." }; } }
		public Mensagem DigitalOuBlocoObrigatorio { get { return new Mensagem() { Campo = "Infracao_IsDigital", Tipo = eTipoMensagem.Advertencia, Texto = "É obrigatório selecionar Digital ou Bloco." }; } }
		public Mensagem SerieObrigatorio { get { return new Mensagem() { Campo = "Multa_Serie", Tipo = eTipoMensagem.Advertencia, Texto = "Série é obrigatório." }; } }
		public Mensagem NumeroIUFObrigatorio { get { return new Mensagem() { Campo = "Multa_NumeroIUF", Tipo = eTipoMensagem.Advertencia, Texto = "Número do IUF é obrigatório." }; } }
		public Mensagem DataLavraturaAutoObrigatorio { get { return new Mensagem() { Campo = "Infracao_DataLavraturaAuto", Tipo = eTipoMensagem.Advertencia, Texto = "Data da lavratura do auto obrigatório." }; } }
		public Mensagem JustificativaObrigatorio { get { return new Mensagem() { Campo = "Multa_Justificativa", Tipo = eTipoMensagem.Advertencia, Texto = "Justificar o valor da penalidade é obrigatório." }; } }
		public Mensagem CodigoReceitaObrigatorio { get { return new Mensagem() { Campo = "Multa_CodigoReceita", Tipo = eTipoMensagem.Advertencia, Texto = "Código da receita obrigatório." }; } }
		public Mensagem ValorMultaObrigatorio { get { return new Mensagem() { Campo = "Multa_ValorMulta", Tipo = eTipoMensagem.Advertencia, Texto = "Valor da multa obrigatório." }; } }
		public Mensagem ValorMultaInvalido { get { return new Mensagem() { Campo = "Multa_ValorMulta", Tipo = eTipoMensagem.Advertencia, Texto = "Valor da multa inválido." }; } }
		public Mensagem TipoArquivoDoc { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Arquivo no formato inválido." }; } }
		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Campo = "file", Tipo = eTipoMensagem.Advertencia, Texto = "Arquivo é obrigatório." }; } }
        public Mensagem ArquivoNaoEhPdf { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Arquivo_Nome", Texto = "Arquivo não é do tipo pdf" }; } }
		public Mensagem ConfigAlteradaSemAtualizar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A configuração da fiscalização utilizada para este cadastro foi alterada. Cancele a edição." }; } }
		public Mensagem ConfigAlterada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A configuração da fiscalização utilizada para este cadastro foi alterada. Por consequência algumas informações deverão ser atualizadas." }; } }
		public Mensagem ConfigAlteradaConfirme { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A configuração da fiscalização utilizada para este cadastro foi alterada. Por consequência algumas informações deverão ser atualizadas. Deseja continuar?" }; } }
		public Mensagem ConfigAlteradaConcluir { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A configuração da fiscalização utilizada para este cadastro foi alterada. Por consequência algumas informações deverão ser atualizadas na tela de Infração." }; } }
	}
}
