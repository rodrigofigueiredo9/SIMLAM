namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static SuinoculturaMsg _suinoculturaMsg = new SuinoculturaMsg();
		public static SuinoculturaMsg Suinocultura
		{
			get { return _suinoculturaMsg; }
			set { _suinoculturaMsg = value; }
		}
	}

	public class SuinoculturaMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de suinocultura excluída com sucesso" }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização de suinocultura salva com sucesso" }; } }

		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Suinocultura_Atividade", Texto = @"Atividade da suinocultura é obrigatória." }; } }

		public Mensagem NumeroCabecaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Suinocultura_NumeroCabecas", Texto = @"Número máximo de cabeças é obrigatório." }; } }
		public Mensagem NumeroCabecaInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Suinocultura_NumeroCabecas", Texto = @"Número máximo de cabeças é inválido." }; } }
		public Mensagem NumeroCabecaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Suinocultura_NumeroCabecas", Texto = @"Número máximo de cabeças deve ser maior do que zero." }; } }

		public Mensagem NumeroMatrizesObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Suinocultura_NumeroMatrizes", Texto = @"Número máximo de matrizes é obrigatório." }; } }
		public Mensagem NumeroMatrizesInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Suinocultura_NumeroMatrizes", Texto = @"Número máximo de matrizes é inválido." }; } }
		public Mensagem NumeroMatrizesMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Suinocultura_NumeroMatrizes", Texto = @"Número máximo de matrizes deve ser maior do que zero." }; } }

		public Mensagem FaseObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo="Suinocultura_Fase", Texto = @"Fase é obrigatória." }; } }

		public Mensagem ExisteBiodigestorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Existe biodigestor é obrigatório." }; } }
		public Mensagem PossuiFabricaRacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Possui fábrica de ração é obrigatório." }; } }

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização suinocultura deste empreendimento?" }; } }
	
	}
}