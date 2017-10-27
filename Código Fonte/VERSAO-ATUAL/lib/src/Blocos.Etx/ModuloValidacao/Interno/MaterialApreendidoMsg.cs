namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static MaterialApreendidoMsg _materialApreendidoMsg = new MaterialApreendidoMsg();
		public static MaterialApreendidoMsg MaterialApreendidoMsg
		{
			get { return _materialApreendidoMsg; }
			set { _materialApreendidoMsg = value; }
		}
	}

	public class MaterialApreendidoMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Material apreendido salvo com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Material apreendido editado com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Material apreendido excluído com sucesso." }; } }

		public Mensagem IsApreendidoObritatorio { get { return new Mensagem() { Campo = "MaterialApreendido_IsApreendido", Tipo = eTipoMensagem.Advertencia, Texto = "Houve a apreensão de algum material é obrigatório." }; } }
		public Mensagem IsTadGeradoSistemaObrigatorio { get { return new Mensagem() { Campo = "MaterialApreendido_IsTadGeradoSistema", Tipo = eTipoMensagem.Advertencia, Texto = "Gerar TAD é obrigatório." }; } }
		public Mensagem SerieObrigatorio { get { return new Mensagem() { Campo = "MaterialApreendido_Serie", Tipo = eTipoMensagem.Advertencia, Texto = "Série é obrigatório." }; } }
		public Mensagem TadNumeroObrigatorio { get { return new Mensagem() { Campo = "MaterialApreendido_NumeroTad", Tipo = eTipoMensagem.Advertencia, Texto = "Nº do TAD é obrigatório." }; } }
		public Mensagem TadNumeroBlocoObrigatorio { get { return new Mensagem() { Campo = "MaterialApreendido_NumeroTad", Tipo = eTipoMensagem.Advertencia, Texto = "Nº do TAD - bloco é obrigatório." }; } }
		public Mensagem DataLavraturaObrigatorio { get { return new Mensagem() { Campo = "MaterialApreendido_DataLavratura", Tipo = eTipoMensagem.Advertencia, Texto = "Data da lavratura do termo é obrigatório." }; } }
		public Mensagem DescricaoObrigatorio { get { return new Mensagem() { Campo = "MaterialApreendido_Descricao", Tipo = eTipoMensagem.Advertencia, Texto = "Descrever a apreensão é obrigatório." }; } }
        public Mensagem ValorProdutosObrigatorio { get { return new Mensagem() { Campo = "MaterialApreendido_ValorProdutosReais", Tipo = eTipoMensagem.Advertencia, Texto = "Valor dos bens apreendidos (R$ e por extenso) é obrigatório." }; } }
        public Mensagem ValorProdutosInvalido { get { return new Mensagem() { Campo = "MaterialApreendido_ValorProdutos", Tipo = eTipoMensagem.Advertencia, Texto = "Valor dos bens apreendidos (R$ e por extenso) inválido." }; } }
        public Mensagem NumeroLacreInvalido { get { return new Mensagem() { Campo = "MaterialApreendido_NumeroLacre", Tipo = eTipoMensagem.Advertencia, Texto = "Número(s) do(s) Lacre(s) da Interdição/Embargo inválido." }; } }
		public Mensagem DepositarioObrigatorio { get { return new Mensagem() { Campo = "Depositario_NomeRazaoSocial", Tipo = eTipoMensagem.Advertencia, Texto = "Depositário é obrigatório." }; } }
		public Mensagem DepositarioLogradouroObrigatorio { get { return new Mensagem() { Campo = "Depositario_Logradouro", Tipo = eTipoMensagem.Advertencia, Texto = "Logradouro / Rua / Rodovia é obrigatório." }; } }
		public Mensagem DepositarioBairroObrigatorio { get { return new Mensagem() { Campo = "Depositario_Bairro", Tipo = eTipoMensagem.Advertencia, Texto = "Bairro / Gleba / Comunidade é obrigatório." }; } }
		public Mensagem DepositarioDistritoObrigatorio { get { return new Mensagem() { Campo = "Depositario_Distrito", Tipo = eTipoMensagem.Advertencia, Texto = "Distrito / Localidade é obrigatório." }; } }
		public Mensagem DepositarioEstadoObrigatorio { get { return new Mensagem() { Campo = "Depositario_Estado", Tipo = eTipoMensagem.Advertencia, Texto = "UF é obrigatório." }; } }
		public Mensagem DepositarioMunicipioObrigatorio { get { return new Mensagem() { Campo = "Depositario_Municipio", Tipo = eTipoMensagem.Advertencia, Texto = "Município é obrigatório." }; } }
        public Mensagem ProdutoApreendidoObrigatorio { get { return new Mensagem() { Campo = "MaterialApreendido_ProdutosApreendidos", Tipo = eTipoMensagem.Advertencia, Texto = "Produto Apreendido é obrigatório." }; } }
        public Mensagem OpiniaoObrigatoria { get { return new Mensagem() { Campo = "MaterialApreendido_Opiniao", Tipo = eTipoMensagem.Advertencia, Texto = "Descrever ou opinar quanto à destinação do material apreendido é obrigatório." }; } }
		public Mensagem TipoObrigatorio { get { return new Mensagem() { Campo = "MaterialApreendido_Tipo", Tipo = eTipoMensagem.Advertencia, Texto = "Tipo do material é obrigatório." }; } }
		public Mensagem EspecificacaoObrigatorio { get { return new Mensagem() { Campo = "MaterialApreendido_Especificacao", Tipo = eTipoMensagem.Advertencia, Texto = "Especificação do material é obrigatório." }; } }
		public Mensagem MaterialJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Material ja adicionado." }; } }
        public Mensagem ProdutoObrigatorio { get { return new Mensagem() { Campo = "MaterialApreendido_ProdutosApreendidos", Tipo = eTipoMensagem.Advertencia, Texto = "Produto é obrigatório." }; } }
        public Mensagem QuantidadeObrigatoria { get { return new Mensagem() { Campo = "MaterialApreendido_Quantidade", Tipo = eTipoMensagem.Advertencia, Texto = "Quantidade é obrigatória." }; } }
        public Mensagem DestinoObrigatorio { get { return new Mensagem() { Campo = "MaterialApreendido_Destinos", Tipo = eTipoMensagem.Advertencia, Texto = "Destino é obrigatório." }; } }
        public Mensagem ProdutoJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Produto ja adicionado." }; } }
        public Mensagem NumeroIUFObrigatorio { get { return new Mensagem() { Campo = "MaterialApreendido_NumeroIUF", Tipo = eTipoMensagem.Advertencia, Texto = "Nº do IUF é obrigatório." }; } }
        public Mensagem DigitalOuBlocoObrigatorio { get { return new Mensagem() { Campo = "MaterialApreendido_IsDigital", Tipo = eTipoMensagem.Advertencia, Texto = "É obrigatório selecionar Digital ou Bloco." }; } }

		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Arquivo_Nome", Texto = "Arquivo é obrigatório." }; } }
		public Mensagem ArquivoNaoEhPdf { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Arquivo_Nome", Texto = "Arquivo não é do tipo pdf" }; } }
		
	}
}
