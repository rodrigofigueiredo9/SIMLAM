namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static DespolpamentoCafeMsg _despolpamentoCafeMsg = new DespolpamentoCafeMsg();
		public static DespolpamentoCafeMsg DespolpamentoCafe
		{
			get { return _despolpamentoCafeMsg; }
			set { _despolpamentoCafeMsg = value; }
		}
	}

	public class DespolpamentoCafeMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização despolpamento/descascamento de café excluída com sucesso" }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização despolpamento/descascamento de café salva com sucesso" }; } }

		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DespolpamentoCafe_Atividade", Texto = @"Atividade do despolpamento/descascamento de café é obrigatória." }; } }
		
		public Mensagem CapacidadeInstaladaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DespolpamentoCafe_CapacidadeTotalInstalada", Texto = @"Capacidade instalada total é obrigatória." }; } }
		public Mensagem CapacidadeInstaladaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DespolpamentoCafe_CapacidadeTotalInstalada", Texto = @"Capacidade instalada total é inválida." }; } }
		public Mensagem CapacidadeInstaladaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DespolpamentoCafe_CapacidadeTotalInstalada", Texto = @"Capacidade instalada total deve ser maior do que zero." }; } }

		public Mensagem AguaResiduariaCafeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DespolpamentoCafe_AguaResiduariaCafe", Texto = @"Água residuária é obrigatória." }; } }
		public Mensagem AguaResiduariaCafeInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DespolpamentoCafe_AguaResiduariaCafe", Texto = @"Água residuária é inválida." }; } }
		public Mensagem AguaResiduariaCafeMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DespolpamentoCafe_AguaResiduariaCafe", Texto = @"Água residuária deve ser maior do que zero." }; } }

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização despolpamento/descascamento de café deste empreendimento?" }; } }
	}
}
