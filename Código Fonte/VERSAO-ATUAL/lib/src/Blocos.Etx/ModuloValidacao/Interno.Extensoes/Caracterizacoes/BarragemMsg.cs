namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static BarragemMsg _barragemMsg = new BarragemMsg();
		public static BarragemMsg BarragemMsg
		{
			get { return _barragemMsg; }
			set { _barragemMsg = value; }
		}
	}

	public class BarragemMsg
	{
		public Mensagem SelecioneAtividade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ddlAtividade", Texto = "Selecione a atividade." }; } }
		public Mensagem AddBarragem { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Adicione ao menos uma barragem." }; } }
		public Mensagem AddBarragemDadosItem { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Adicione ao menos um tipo de dado para a barragem." }; } }
		public Mensagem InformeQuantidade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtQuantidade", Texto = "Informe a quantidade de barragens." }; } }
		public Mensagem InformeQuantidadeZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtQuantidade", Texto = "Informe a quantidade de barragens maior que zero." }; } }
		public Mensagem QuantidadeInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A quantidade de barragens adicionadas tem que ser menor ou igual a quantidade informada." }; } }
		public Mensagem SelecioneFinalidade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#ddlFinalidade", Texto = "Selecione a finalidade." }; } }
		public Mensagem InformeOutroFinalidade { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtEspecificar", Texto = "Informe o outro tipo de finalidade." }; } }
		public Mensagem InformeLamina { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtLaminaAgua", Texto = "Informe a lâmina d'água por barragem." }; } }
		public Mensagem InformeArmazenado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtArmazenado", Texto = "Informe o volume armazenado por barragem." }; } }
		public Mensagem SelecioneOutorga { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#ddlOutorga", Texto = "Selecione a outorga de uso de água." }; } }
		public Mensagem InformeNumero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtNumero", Texto = "Informe o numero." }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de Barragem salva com sucesso" }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de Barragem excluída com sucesso" }; } }
		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização de Barragem deste empreendimento?" }; } }
		public Mensagem ExcluirBarragemItemConfirm { get { return new Mensagem() { Texto = "Deseja realmente excluir esta barragem?" }; } }
		public Mensagem ExcluirBarragemItemSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Barragem excluída com sucesso." }; } }

		public Mensagem SemARLConfirm { get { return new Mensagem() { Texto = "O empreendimento está localizado em zona rural e na caracterização de Dominialidade não possui Reserva Legal. Deseja realmente continuar?" }; } }
		public Mensagem ARLDesconhecidaConfirm { get { return new Mensagem() { Texto = "O empreendimento está localizado em zona rural e na caracterização de Dominialidade possui Reserva Legal sem situação vegetal definida. Deseja realmente continuar?" }; } }
		public Mensagem SelecioneEspBarragem { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#ddlBarragens", Texto = "Selecione a barragem da licença." }; } }
		public Mensagem BarragemExluida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A barragem da licença não existe mais na caracterização de Barragem." }; } }

		public Mensagem BarragemUltimoItemListaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Não é possivel excluir todas as barragens da lista." }; } }
	}
}