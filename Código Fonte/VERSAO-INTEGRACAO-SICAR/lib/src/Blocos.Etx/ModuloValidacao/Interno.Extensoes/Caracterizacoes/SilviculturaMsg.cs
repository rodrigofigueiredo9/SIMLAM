namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{

	public partial class Mensagem
	{
		private static SilviculturaMsg _silviculturaMsg = new SilviculturaMsg();
		public static SilviculturaMsg Silvicultura
		{
			get { return _silviculturaMsg; }
			set { _silviculturaMsg = value; }
		}
	}

	public class SilviculturaMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = @"Caracterização silvicultura salva com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = @"Caracterização silvicultura excluída com sucesso." }; } }

		public Mensagem TipoCulturaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Silvicultura_TipoCultura", Texto = @"Tipo da cultura da silvicultura é obrigatório." }; } }
		public Mensagem TipoCulturaJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Silvicultura_TipoCultura", Texto = @"Tipo da cultura da silvicultura já adicionado." }; } }

		public Mensagem AreaCulturaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Silvicultura_AreaCultura", Texto = @"Área de cultura é obrigatória." }; } }
		public Mensagem AreaCulturaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Silvicultura_AreaCultura", Texto = @"Área de cultura é inválida." }; } }
		public Mensagem AreaCulturaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Silvicultura_AreaCultura", Texto = @"Área de cultura deve ser maior do que zero." }; } }

		public Mensagem CulturaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Lista de Culturas não pode ser vazia." }; } }

		public Mensagem EspecificarTipoCulturaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Silvicultura_Especificar", Texto = @"Especificar tipo de cultura é obrigatório." }; } }
		public Mensagem EspecificarTipoCulturaJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Silvicultura_Especificar", Texto = @"Especificar tipo de cultura já adicionado." }; } }

		public Mensagem SemARLConfirm { get { return new Mensagem() { Texto = "O empreendimento está localizado em zona rural e na caracterização de Dominialidade não possui Reserva Legal. Deseja realmente continuar?" }; } }
		public Mensagem ARLDesconhecidaConfirm { get { return new Mensagem() { Texto = "O empreendimento está localizado em zona rural e na caracterização de Dominialidade possui Reserva Legal sem situação vegetal definida. Deseja realmente continuar?" }; } }

		public Mensagem EmpreendimentoRuralReservaIndefinida { get { return new Mensagem() { Texto = "O empreendimento está localizado em zona rural e na caracterização de Dominialidade possui Reserva Legal sem situação vegetal definida. Deseja realmente continuar?" }; } }
		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização de silvicultura deste empreendimento?" }; } }

	}
}

