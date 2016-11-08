namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static SetorMsg _setorMsg = new SetorMsg();

		public static SetorMsg Setor
		{
			get { return _setorMsg; }
		}
	}

	public class SetorMsg
	{
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Setor editado com sucesso." }; } }
		public Mensagem LogradouroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Setor_Endereco_Logradouro", Texto = "Logradouro do endereço do setor é obrigatório." }; } }
		public Mensagem NumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Setor_Endereco_Numero", Texto = "Número do endereço do setor é obrigatório." }; } }
		public Mensagem SiglaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Setor_Sigla", Texto = "Sigla do setor é obrigatória." }; } }
		public Mensagem SiglaDuplicada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Setor_Sigla", Texto = "A sigla utilizada já esta em uso por outro setor. Escolha outra sigla e tente salvar novamente." }; } }
	}
}
