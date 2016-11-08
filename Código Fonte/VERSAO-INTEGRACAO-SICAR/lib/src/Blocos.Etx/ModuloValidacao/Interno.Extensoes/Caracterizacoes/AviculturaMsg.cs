namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static AviculturaMsg _aviculturaMsg = new AviculturaMsg();
		public static AviculturaMsg Avicultura
		{
			get { return _aviculturaMsg; }
			set { _aviculturaMsg = value; }
		}
	}
	public class AviculturaMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de Avicultura excluída com sucesso" }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de Avicultura salva com sucesso" }; } }

		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Avicultura_Atividade", Texto = @"Atividade da avicultura é obrigatória." }; } }
		
		public Mensagem ConfinamentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Lista de confinamentos não pode ser vazia." }; } }
		public Mensagem ConfinamentoFinalidadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Avicultura_ConfinamentoFinalidade", Texto = @"Finalidade do confinamento de aves é obrigatória." }; } }
		public Mensagem ConfinamentoFinalidadeDuplicada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Avicultura_ConfinamentoFinalidade", Texto = @"Finalidade do confinamento de aves já adicionada." }; } }
		
		public Mensagem ConfinamentoAreaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Avicultura_ConfinamentoArea", Texto = @"Área do confinamento de aves é obrigatória." }; } }
		public Mensagem ConfinamentoAreaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Avicultura_ConfinamentoArea", Texto = @"Área do confinamento de aves é inválida." }; } }
		public Mensagem ConfinamentoAreaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Avicultura_ConfinamentoArea", Texto = @"Área do confinamento de aves deve ser maior do que zero." }; } }

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização de avicultura deste empreendimento?" }; } }
	}
}
