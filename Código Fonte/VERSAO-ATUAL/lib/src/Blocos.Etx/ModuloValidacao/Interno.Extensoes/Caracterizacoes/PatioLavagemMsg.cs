namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static PatioLavagemMsg _patioLavagem = new PatioLavagemMsg();
		public static PatioLavagemMsg PatioLavagem
		{
			get { return _patioLavagem; }
			set { _patioLavagem = value; }
		}
	}

	public class PatioLavagemMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de pátio de lavagem, abastecimento e descontaminação de aeronave agrícola excluída com sucesso" }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de pátio de lavagem, abastecimento e descontaminação de aeronave agrícola salva com sucesso" }; } }

		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PatioLavagem_Atividade", Texto = @"Atividade da pulverização aérea de produtos agrotóxicos é obrigatório." }; } }

		public Mensagem AreaTotalObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PatioLavagem_Area", Texto = @"Área total é obrigatória." }; } }
		public Mensagem AreaTotalInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PatioLavagem_Area", Texto = @"Área total é inválida." }; } }
		public Mensagem AreaTotalMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "PatioLavagem_Area", Texto = @"Área total deve ser maior do que zero." }; } }

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização de pulverização aérea de produtos agrotóxicos deste empreendimento?" }; } }
	}
}
