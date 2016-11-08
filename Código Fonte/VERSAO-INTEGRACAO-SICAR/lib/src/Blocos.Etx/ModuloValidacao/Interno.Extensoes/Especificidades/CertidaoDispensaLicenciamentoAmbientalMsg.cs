namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CertidaoDispensaLicenciamentoAmbientalMsg _certidaoDispensaLicenciamentoAmbientalMsg = new CertidaoDispensaLicenciamentoAmbientalMsg();
		public static CertidaoDispensaLicenciamentoAmbientalMsg CertidaoDispensaLicenciamentoAmbiental
		{
			get { return _certidaoDispensaLicenciamentoAmbientalMsg; }
			set { _certidaoDispensaLicenciamentoAmbientalMsg = value; }
		}
	}

	public class CertidaoDispensaLicenciamentoAmbientalMsg
	{
		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Certidao_Atividade", Texto = @"Atividade é obrigatória." }; } }
		public Mensagem VinculoPropriedadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Vinculo", Texto = @"Vinculo com a propriedade é obrigatório." }; } }
		public Mensagem VinculoPropriedadeOutroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "VinculoPropOutro", Texto = @"Especificar é obrigatório." }; } }
	}
}
