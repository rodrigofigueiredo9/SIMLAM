using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
        private static PragaMsg _pragaMsg = new PragaMsg();
        public static PragaMsg Praga
		{
            get { return _pragaMsg; }
		}
	}

	public class PragaMsg
	{
		public Mensagem JaExiste { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Praga já existente.", Campo = "Praga.NomeCientífico" }; } }
		public Mensagem CulturaJaAdicionada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Cultura já adicionada."}; } }
		public Mensagem NomeCientificoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Nome científico é obrigatório.", Campo = "Praga.NomeCientifico" }; } }
		public Mensagem ObjetoNulo { get { return new Mensagem() { Tipo = eTipoMensagem.Erro, Texto = "Objeto está nulo." }; } }
		public Mensagem PragaSalvaComSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Praga salva com sucesso." }; } }
		public Mensagem CulturasAssociadasComSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Culturas associadas com sucesso." }; } }

		public Mensagem CulturaExistente(string cultura)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Existem mais de uma ocorrência para a cultura \"{0}\"", cultura) };
		}

	}
}