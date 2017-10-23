namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
        private static OutrasPenalidadesMsg _outrasPenalidadesMsg = new OutrasPenalidadesMsg();
		public static OutrasPenalidadesMsg OutrasPenalidadesMsg
		{
            get { return _outrasPenalidadesMsg; }
            set { _outrasPenalidadesMsg = value; }
		}
	}

	public class OutrasPenalidadesMsg
	{
		public Mensagem  Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Outras Penalidades salvo com sucesso." }; } }
		
		public Mensagem DigitalOuBlocoObrigatorio { get { return new Mensagem() { Campo = "OutrasPenalidades_IsDigital", Tipo = eTipoMensagem.Advertencia, Texto = "É obrigatório selecionar Digital ou Bloco." }; } }
		public Mensagem SerieObrigatorio { get { return new Mensagem() { Campo = "OutrasPenalidades_Serie", Tipo = eTipoMensagem.Advertencia, Texto = "Série é obrigatório." }; } }
		public Mensagem NumeroIUFObrigatorio { get { return new Mensagem() { Campo = "OutrasPenalidades_NumeroIUF", Tipo = eTipoMensagem.Advertencia, Texto = "Número do IUF é obrigatório." }; } }
		public Mensagem DescricaoObrigatorio { get { return new Mensagem() { Campo = "OutrasPenalidades_Descricao", Tipo = eTipoMensagem.Advertencia, Texto = "Descrição de outras penalidades é obrigatório." }; } }
		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Campo = "file", Tipo = eTipoMensagem.Advertencia, Texto = "Arquivo é obrigatório." }; } }
        public Mensagem ArquivoNaoEhPdf { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Arquivo_Nome", Texto = "Arquivo não é do tipo pdf" }; } }
	}
}
