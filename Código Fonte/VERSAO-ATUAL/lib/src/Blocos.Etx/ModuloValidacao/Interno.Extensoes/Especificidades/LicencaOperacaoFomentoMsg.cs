namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static LicencaOperacaoFomentoMsg _licencaOperacaoFomentoMsg = new LicencaOperacaoFomentoMsg();
		public static LicencaOperacaoFomentoMsg LicencaOperacaoFomentoMsg
		{
			get { return _licencaOperacaoFomentoMsg; }
			set { _licencaOperacaoFomentoMsg = value; }
		}
	}

	public class LicencaOperacaoFomentoMsg
	{
		public Mensagem SilviculturaPPFFInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterizacao Silvicultura – Programa Produtor Florestal (Fomento) deve estar cadastrada." }; } }
		public Mensagem CaracterizacaoAtividadeInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A atividade selecionada no título deve ser a mesma selecionada na caracterização de Silvicultura – Programa Produtor Florestal (Fomento)." }; } }

	}
}