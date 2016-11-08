namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static TerraplanagemMsg _terraplanagemMsg = new TerraplanagemMsg();
		public static TerraplanagemMsg Terraplanagem
		{
			get { return _terraplanagemMsg; }
			set { _terraplanagemMsg = value; }
		}
	}


	public class TerraplanagemMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de terraplanagem excluída com sucesso" }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de terraplanagem salva com sucesso" }; } }

		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Terraplanagem_Atividade", Texto = @"Atividade da terraplanagem é obrigatória." }; } }

		public Mensagem AreaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Terraplanagem_Area", Texto = @"Área é obrigatória." }; } }
		public Mensagem AreaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Terraplanagem_Area", Texto = @"Área é inválida." }; } }
		public Mensagem AreaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Terraplanagem_Area", Texto = @"Área deve ser maior do que zero." }; } }

		public Mensagem VolumeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Terraplanagem_VolumeMovimentado", Texto = @"Volume de terra movimentado é obrigatório." }; } }
		public Mensagem VolumeInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Terraplanagem_VolumeMovimentado", Texto = @"Volume de terra movimentado é inválido." }; } }
		public Mensagem VolumeMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Terraplanagem_VolumeMovimentado", Texto = @"Volume de terra movimentado deve ser maior do que zero." }; } }

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização de terraplanagem deste empreendimento?" }; } }

	}
}
