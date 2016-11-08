using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
        private static CulturaMsg _CulturaMsg = new CulturaMsg();
		private static CultivarConfiguracaoMsg _CultivarConfiguracaoMsg = new CultivarConfiguracaoMsg();
        public static CulturaMsg Cultura
		{
            get { return _CulturaMsg; }
		}

		public static CultivarConfiguracaoMsg CultivarConfiguracaoMsg
		{
			get { return _CultivarConfiguracaoMsg; }
		}
	}

    public class CulturaMsg
	{
        public Mensagem CultivarJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Cultivar já adicionado.", Campo="Cultura_Cultivar" }; } }
        public Mensagem CultivarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Cultivar é obrigatório.", Campo = "Cultura_Cultivar" }; } }
        
        public Mensagem ObjetoNulo { get { return new Mensagem() { Tipo = eTipoMensagem.Erro, Texto = "Objeto está nulo." }; } }
		public Mensagem CulturaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Cultura é obrigatório.", Campo = "Cultura_Cultura" }; } }
        public Mensagem CulturaJaExiste { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Cultura já existente.", Campo = "Cultura_Cultura" }; } }
        public Mensagem CulturaSalvaSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Cultura salva com sucesso."}; } }
		
		public Mensagem CultivarMesmoNome(string cultivar)
		{
			return new Mensagem() {Tipo=eTipoMensagem.Advertencia, Texto = String.Format("Existem mais de uma ocorrência para o cultivar \"{0}\"", cultivar ) };
		}
	}

	public class CultivarConfiguracaoMsg
	{
		public Mensagem PragaObrigatorio { get { return new Mensagem() { Campo = "Pragas", Texto = "Praga é obrigatório", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem TipoProducaoObrigatorio { get { return new Mensagem() { Campo = "TipoProducao", Texto = "Tipo de produção é obrigatório", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem DeclaracaoAdicionalObrigatorio { get { return new Mensagem() { Campo = "DeclaracaoAdicional", Texto = "Declaração Adicional é obrigatório", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem DeclaracaoJaAdicionado { get { return new Mensagem() { Texto = "A declaração adicional já está adicionada para a praga e tipo de produção", Tipo = eTipoMensagem.Advertencia }; } }
	}
}