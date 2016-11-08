using System;
namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CertificadoCadastroProdutoAgrotoxicoMsg _certificadoCadastroProdutoAgrotoxico = new CertificadoCadastroProdutoAgrotoxicoMsg();
		public static CertificadoCadastroProdutoAgrotoxicoMsg CertificadoCadastroProdutoAgrotoxico
		{
			get { return _certificadoCadastroProdutoAgrotoxico; }
			set { _certificadoCadastroProdutoAgrotoxico = value; }
		}
	}

	public class CertificadoCadastroProdutoAgrotoxicoMsg
	{
		public Mensagem AgrotoxicoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Certificado_AgrotoxicoNome", Texto = "Agrotóxico é obrigatório." }; } }
		public Mensagem AgrotoxicoInativo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Certificado_AgrotoxicoNome", Texto = "O agrotóxico não pode ser utilizado, pois está inativo." }; } }
		public Mensagem AgrotoxicoDesatualizado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Certificado_AgrotoxicoNome", Texto = "O agrotóxico foi atualizado." }; } }
		public Mensagem NumeroSEPAlterado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Certificado_AgrotoxicoNome", Texto = "O agrotóxico não possui o numero de SEP do processo selecionado." }; } }
	}
}
