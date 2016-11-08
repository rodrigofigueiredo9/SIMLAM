namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static TermoAprovacaoMedicaoMsg _termoAprovacaoMedicaoMsg = new TermoAprovacaoMedicaoMsg();
		public static TermoAprovacaoMedicaoMsg TermoAprovacaoMedicaoMsg
		{
			get { return _termoAprovacaoMedicaoMsg; }
			set { _termoAprovacaoMedicaoMsg = value; }
		}
	}

	public class TermoAprovacaoMedicaoMsg
	{
		public Mensagem SelecioneDestinatario { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#ddlDestinatarios", Texto = "Selecione o destinatário." }; } }
		public Mensagem InformeData { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtDataMedicao", Texto = "Informe a data da medição." }; } }
		public Mensagem SelecioneResponsavel { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Selecione o responsável pela medição." }; } }
		public Mensagem SelecioneTecnico { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Selecione o técnico responsável pela medição." }; } }
		public Mensagem DestinatarioNaoAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O destinatário selecionado não está mais associado ao empreendimento ou processo ou documento" }; } }
		public Mensagem ResponsavelNaoContido (string strNumeroProcesso) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("O responsável pela medição não está mais associado ao processo nº {0}.", strNumeroProcesso) }; }
		public Mensagem SelecioneSetorCadastro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#ddlSetoresUsuario", Texto = "Selecione o Setor de Cadastro." }; } }
		public Mensagem DataInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtDataMedicao", Texto = "Data de medição deve ser válida e menor ou igual a data atual." }; } }
	}
}
