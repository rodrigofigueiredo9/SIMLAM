

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static EnquadramentoMsg _enquadramentoMsg = new EnquadramentoMsg();
		public static EnquadramentoMsg Enquadramento
		{
			get { return _enquadramentoMsg; }
			set { _enquadramentoMsg = value; }
		}
	}

	public class EnquadramentoMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Enquadramento salvo com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Enquadramento editado com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Enquadramento excluído com sucesso." }; } }

		public Mensagem ListaArtigosObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Obrigatório pelo menos um artigo." }; } }
		public Mensagem ListaArtigosCheia { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Já foi preenchido o número máximo de artigos que podem ser adicionados. (máximo 3)" }; } }

		public Mensagem ArtigoObrigatorio(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Enquadramento_ArtigoTexto{0}, #fsArtigo{0}", identificacao), Texto = @"Artigo é obrigatório." };
		}

		public Mensagem DaDoObrigatorio(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Enquadramento_Enquadramento_DaDo{0}, #fsArtigo{0}", identificacao), Texto = @"Da/Do (Citar norma legal: lei, decreto, resolução, portaria, etc) é obrigatório." };
		}
	}

}

