namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{

	public partial class Mensagem
	{
		private static SilviculturaAtvMsg _silviculturaAtvMsg = new SilviculturaAtvMsg();
		public static SilviculturaAtvMsg SilviculturaAtvMsg
		{
			get { return _silviculturaAtvMsg; }
			set { _silviculturaAtvMsg = value; }
		}
	}

	public class SilviculturaAtvMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = @"Caracterização Silvicultura - Implantação da Atividade de Silvicultura (Fomento) salva com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = @"Caracterização Silvicultura - Implantação da Atividade de Silvicultura (Fomento) excluída com sucesso." }; } }

		public Mensagem TipoCulturaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TipoCultura", Texto = @"Tipo de cobertura é obrigatório." }; } }
		public Mensagem TipoCulturaJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "TipoCultura", Texto = @"Tipo de cobertura já adicionado." }; } }

		public Mensagem AreaCulturaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AreaCultura", Texto = @"Área da cobertura é obrigatória." }; } }
		public Mensagem AreaCulturaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AreaCultura", Texto = @"Área da cobertura é inválida." }; } }
		public Mensagem AreaCulturaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AreaCultura", Texto = @"Área da cobertura deve ser maior do que zero." }; } }

		public Mensagem CulturaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Lista de Coberturas não pode ser vazia." }; } }

		public Mensagem SemARLConfirm { get { return new Mensagem() { Texto = "O empreendimento está localizado em zona rural e na caracterização de Dominialidade não possui Reserva Legal. Deseja realmente continuar?" }; } }
		public Mensagem ARLDesconhecidaConfirm { get { return new Mensagem() { Texto = "O empreendimento está localizado em zona rural e na caracterização de Dominialidade possui Reserva Legal sem situação vegetal definida. Deseja realmente continuar?" }; } }

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização de Silvicultura - Implantação da Atividade de Silvicultura (Fomento) deste empreendimento?" }; } }

		public Mensagem SelecioneFomento { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#spanRblFomento", Texto = @"Selecione o tipo do fomento." }; } }

		public Mensagem AreaDeclividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtAreaDeclividade", Texto = @"Declividade predominante é obrigatória." }; } }
		public Mensagem AreaFomentoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtAreaFomento", Texto = @"Área de outro fomento é obrigatória." }; } }
		public Mensagem AreaPlantioObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtAreaPlantio", Texto = @"Área de plantio próprio é obrigatória." }; } }
				
		public Mensagem DeclividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#Caracterizacao_Declividade", Texto = @"Declividade predominante é obrigatória." }; } }
		public Mensagem TotalRequeridaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#Caracterizacao_TotalRequerida", Texto = @"Total requerida é obrigatória." }; } }
		public Mensagem TotalPlantadaComEucaliptoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#Caracterizacao_TotalPlantadaComEucalipto", Texto = @"Área total plantada com eucalipto em relação a ATP é obrigatória." }; } }
	}
}

