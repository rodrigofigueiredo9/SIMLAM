namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ProducaoCarvaoVegetalMsg _producaoCarvaoVegetalMsg = new ProducaoCarvaoVegetalMsg();
		public static ProducaoCarvaoVegetalMsg ProducaoCarvaoVegetal
		{
			get { return _producaoCarvaoVegetalMsg; }
			set { _producaoCarvaoVegetalMsg = value; }
		}
	}

	public class ProducaoCarvaoVegetalMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização produção de carvão vegetal excluída com sucesso" }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização produção de carvão vegetal salva com sucesso" }; } }

		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProducaoCarvaoVegetal_Atividade", Texto = @"Atividade da produção de carvão vegetal é obrigatória." }; } }

		public Mensagem NumeroFornosObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProducaoCarvaoVegetal_NumeroFornos", Texto = @"Número de Fornos é obrigatório." }; } }
		public Mensagem NumeroFornosInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProducaoCarvaoVegetal_NumeroFornos", Texto = @"Número de Fornos é inválido." }; } }
		public Mensagem NumeroFornosMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProducaoCarvaoVegetal_NumeroFornos", Texto = @"Número de Fornos deve ser maior do que zero." }; } }
		public Mensagem NumeroFornosMenorQueFornosAdicionados { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProducaoCarvaoVegetal_NumeroFornos", Texto = @"Número de Fornos deve ser maior ou igual aos fornos adicionados." }; } }
		public Mensagem FornosJaAdicionados { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Lista de Fornos já possui a quantidade de fornos informados." }; } }
		public Mensagem FornoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Lista de Fornos não pode ser vazia." }; } }

		public Mensagem VolumeFornoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProducaoCarvaoVegetal_VolumeForno", Texto = @"Volume do forno é obrigatório." }; } }
		public Mensagem VolumeFornoInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProducaoCarvaoVegetal_VolumeForno", Texto = @"Volume do forno é inválido." }; } }
		public Mensagem VolumeFornoMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProducaoCarvaoVegetal_VolumeForno", Texto = @"Volume do forno deve ser maior do que zero." }; } }

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização produção de carvão vegetal deste empreendimento?" }; } }
	}
}
