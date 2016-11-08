namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static SecagemMecanicaGraosMsg _secagemMecanicaGraosMsg = new SecagemMecanicaGraosMsg();
		public static SecagemMecanicaGraosMsg SecagemMecanicaGraos
		{
			get { return _secagemMecanicaGraosMsg; }
			set { _secagemMecanicaGraosMsg = value; }
		}
	}


	public class SecagemMecanicaGraosMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização secagem mecânica de grãos excluída com sucesso" }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização secagem mecânica de grãos salva com sucesso" }; } }

		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SecagemMecanicaGraos_Atividade", Texto = @"Atividade da secagem mecânica de grãos é obrigatório." }; } }

		public Mensagem NumeroSecadorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SecagemMecanicaGraos_numSecadores", Texto = @"Número de Secadores da secagem mecânica de grãos é obrigatório." }; } }
		public Mensagem NumeroSecadorInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SecagemMecanicaGraos_numSecadores", Texto = @"Número de Secadores da secagem mecânica de grãos é inválido." }; } }
		public Mensagem NumeroSecadorMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SecagemMecanicaGraos_numSecadores", Texto = @"Número de Secadores da secagem mecânica de grãos deve ser maior do que zero." }; } }
		public Mensagem NumeroSecadorMenorQueSecadoresAdicionados { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SecagemMecanicaGraos_numSecadores", Texto = @"Número de Secadores da secagem mecânica de grãos deve ser maior ou igual aos secadores adicionados." }; } }
		public Mensagem SecadoresJaAdicionados { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Lista de Secadores da secagem mecânica de grãos já possui a quantidade de secadores informados." }; } }
		public Mensagem SecadorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Lista de Secadores não pode ser vazia." }; } }
		
		public Mensagem CapacidadeSecadorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SecagemMecanicaGraos_Capacidade", Texto = @"Capacidade do secador da secagem mecânica de grãos é obrigatória." }; } }
		public Mensagem CapacidadeSecadorInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SecagemMecanicaGraos_Capacidade", Texto = @"Capacidade do secador da secagem mecânica de grãos é inválida." }; } }
		public Mensagem CapacidadeSecadorMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SecagemMecanicaGraos_Capacidade", Texto = @"Capacidade do secador da secagem mecânica de grãos deve ser maior do que zero." }; } }

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização secagem mecânica de grãos deste empreendimento?" }; } }
	}
}
