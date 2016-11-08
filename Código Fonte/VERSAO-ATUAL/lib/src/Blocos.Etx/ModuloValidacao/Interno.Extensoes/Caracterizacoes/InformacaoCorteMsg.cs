namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static InformacaoCorteMsg _informacaoCorteMsg = new InformacaoCorteMsg();
		public static InformacaoCorteMsg InformacaoCorte
		{
			get { return _informacaoCorteMsg; }
			set { _informacaoCorteMsg = value; }
		}
	}


	public class InformacaoCorteMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização informação de corte excluída com sucesso." }; } }
		public Mensagem ItemExcluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Informação de corte excluída com sucesso." }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização informação de corte salva com sucesso." }; } }
		public Mensagem InformacaoCorteListaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Nenhuma Informação de corte adicionada. Pelo menos uma informação de corte deve ser adicionado." }; } }
		public Mensagem InformacaoCorteUltimoItemListaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Não é possivel excluir todas as informações de corte da lista." }; } }

		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_Atividade", Texto = @"Atividade da informação de corte é obrigatório." }; } }

		#region Especie

		public Mensagem EspecieListaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,Campo="#fsInformacaoCorte", Texto = @"Nenhuma espécie adicionada." }; } }
		public Mensagem EspecieTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_EspecieTipo, #fsInformacaoCorte", Texto = @"Espécie é obrigatório." }; } }
		public Mensagem EspecieDuplicada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_EspecieTipo, #fsInformacaoCorte", Texto = @"Espécie ja adicionada." }; } }
		public Mensagem EspecieEspecificarDuplicada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_EspecieEspecificarTexto, #fsInformacaoCorte", Texto = @"Espécie ja adicionada." }; } }
		public Mensagem EspecieEspecificarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_EspecieEspecificarTexto, #fsInformacaoCorte", Texto = @"Espécie especificar é obrigatório." }; } }
		public Mensagem EspecieArvoresOuAreaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_ArvoresIsoladas, #fsInformacaoCorte, #InformacaoCorteInformacao_AreaCorte", Texto = @"Deve preencher árvores isoladas ou área de corte. Pelo menos um é obrigatório." }; } }

		public Mensagem EspecieArvoresIsoladasInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_ArvoresIsoladas, #fsInformacaoCorte", Texto = @"Árvores isoladas é inválida." }; } }
		public Mensagem EspecieArvoresIsoladasZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_ArvoresIsoladas, #fsInformacaoCorte", Texto = @"Árvores isoladas deve ser maior do que zero." }; } }

		public Mensagem EspecieAreaCorteInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_AreaCorte, #fsInformacaoCorte", Texto = @"Área de corte é inválida." }; } }
		public Mensagem EspecieAreaCorteZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_AreaCorte, #fsInformacaoCorte", Texto = @"Área de corte deve ser maior do que zero." }; } }

		public Mensagem EspecieIdadePlantioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_IdadePlantio, #fsInformacaoCorte", Texto = @"Idade do plantio é obrigatório." }; } }
		public Mensagem EspecieIdadePlantioInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_IdadePlantio, #fsInformacaoCorte", Texto = @"Idade do plantio é inválido." }; } }
		public Mensagem EspecieIdadePlantioMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_IdadePlantio, #fsInformacaoCorte", Texto = @"Idade do plantio deve ser maior do que zero." }; } }

		#endregion

		#region Produto

		public Mensagem ProdutoListaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo="#fsInformacaoCorte", Texto = "Nenhum produto adicionado." }; } }
		public Mensagem ProdutoTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_ProdutoTipo, #fsInformacaoCorte", Texto = @"Produto é obrigatório." }; } }
		public Mensagem ProdutoDuplicado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_ProdutoTipo, #fsInformacaoCorte", Texto = @"Produto com a mesma destinação ja adicionado." }; } }
		public Mensagem ProdutoDestinacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_DestinacaoTipo, #fsInformacaoCorte", Texto = @"Destinção do material é obrigatório." }; } }

		public Mensagem ProdutoQuantidadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_ProdutoQuantidade, #fsInformacaoCorte", Texto = @"Quantidade do produto é obrigatória." }; } }
		public Mensagem ProdutoQuantidadeInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_ProdutoQuantidade, #fsInformacaoCorte", Texto = @"Quantidade do produto é inválida." }; } }
		public Mensagem ProdutoQuantidadeMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_ProdutoQuantidade, #fsInformacaoCorte", Texto = @"Quantidade do produto deve ser maior do que zero." }; } }

		#endregion

		public Mensagem ArvoresIsoladasRestantesObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_ArvoresIsoladasRestantes, #fsInformacaoCorte", Texto = @"Árvores isoladas restantes é obrigatória." }; } }
		public Mensagem ArvoresIsoladasRestantesInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_ArvoresIsoladasRestantes, #fsInformacaoCorte", Texto = @"Árvores isoladas restantes é inválida." }; } }
		public Mensagem ArvoresIsoladasRestantesMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_ArvoresIsoladasRestantes, #fsInformacaoCorte", Texto = @"Árvores isoladas restantes deve ser maior do que zero." }; } }

		public Mensagem AreaCorteRestantesObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_AreaCorteRestante, #fsInformacaoCorte", Texto = @"Área de corte restante é obrigatória." }; } }
		public Mensagem AreaCorteRestantesInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_AreaCorteRestante, #fsInformacaoCorte", Texto = @"Área de corte restante é inválida." }; } }
		public Mensagem AreaCorteRestantesMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorteInformacao_AreaCorteRestante, #fsInformacaoCorte", Texto = @"Área de corte restante deve ser maior do que zero." }; } }

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização informação de corte deste empreendimento? " }; } }
		public Mensagem ItemExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a informação de corte dessa caracterização? " }; } }

	}

}
