namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CertificadoCadastroProtudoVegetalMsg _certificadoCadastroProtudoVegetalMsg = new CertificadoCadastroProtudoVegetalMsg();
		public static CertificadoCadastroProtudoVegetalMsg CertificadoCadastroProtudoVegetalMsg
		{
			get { return _certificadoCadastroProtudoVegetalMsg; }
			set { _certificadoCadastroProtudoVegetalMsg = value; }
		}
	}

	public class CertificadoCadastroProtudoVegetalMsg
	{
		public Mensagem InformeNomeProduto { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtNome", Texto = "Informe o nome do produto." }; } }
		public Mensagem InformeFabricanteProduto { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtFabricante", Texto = "Informe o fabricante do produto." }; } }
		public Mensagem InformeClasseToxilogica { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtClasseToxicologica", Texto = "Informe a classe toxilógica." }; } }
		public Mensagem InformeClasse { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtClasse", Texto = "Informe a classe." }; } }
		public Mensagem InformeIngredienteAtivo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtIngrediente", Texto = "Informe o ingrediente ativo." }; } }
		public Mensagem InformeClassificacaoQtoPotPericAmbiental { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtCultura", Texto = "Informe a classificação qto. pot. peric. ambiental." }; } }
		public Mensagem InformeCulturaIndicada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#txtClassificacao", Texto = "Informe a(s) cultura(s) indicada(s)." }; } }
	}
}
