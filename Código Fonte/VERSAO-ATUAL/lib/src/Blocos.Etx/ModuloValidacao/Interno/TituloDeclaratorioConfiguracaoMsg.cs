namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static TituloDeclaratorioConfiguracaoMsg _tituloDeclaratorioConfiguracaoMsg = new TituloDeclaratorioConfiguracaoMsg();
		public static TituloDeclaratorioConfiguracaoMsg TituloDeclaratorioConfiguracao
		{
			get { return _tituloDeclaratorioConfiguracaoMsg; }
			set { _tituloDeclaratorioConfiguracaoMsg = value; }
		}
	}

	public class TituloDeclaratorioConfiguracaoMsg
	{	
		public Mensagem SalvoSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Configuração de título declaratório salva com sucesso." }; } }
	}
}
