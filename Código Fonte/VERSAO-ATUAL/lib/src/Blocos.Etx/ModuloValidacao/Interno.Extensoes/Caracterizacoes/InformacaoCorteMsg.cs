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
		public Mensagem ProibidoExcluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização não pode ser excluída pois está vinculada a um título declaratório." }; } }
		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Excluir caracterização Informação de Corte?" }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização informação de corte salva com sucesso." }; } }
		public Mensagem ProibidoCriar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível incluir Informação de Corte para o empreendimento, pois possui informação(s) de corte em aberto." }; } }
		public Mensagem ProibidoEditar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível editar a Informação de Corte, pois a mesma se encontra vinculada a um Título." }; } }

		public Mensagem AreaPlantadaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_AreaPlantada", Texto = @"Área de Floresta Plantada é obrigatória." }; } }
		public Mensagem Declaracao1Obrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_DeclaracaoVerdadeira", Texto = "Para salvar a caracterização informação de corte é necessário aceitar as declarações." }; } }
		public Mensagem Declaracao2Obrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_ResponsavelPelasDeclaracoes", Texto = "Para salvar a caracterização informação de corte é necessário aceitar as declarações." }; } }

		#region Licenca

		public Mensagem LicencaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Nenhuma licença adicionada. \n Para Área de Floresta Plantada maior que 100 ha é necessário informar a Licença Ambiental de Silvicultura." }; } }
		public Mensagem NumeroLicencaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_NumeroLicenca", Texto = @"Nº Licença é obrigatória." }; } }
		public Mensagem TipoLicencaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_TipoLicenca", Texto = @"Tipo de Licença é obrigatória." }; } }
		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_Atividade", Texto = @"Atividade é obrigatória." }; } }
		public Mensagem AreaLicencaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_AreaLicenciada", Texto = @"Área Licenciada / Plantada (ha) é obrigatória." }; } }
		public Mensagem DataVencimentoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_DataVencimento", Texto = @"Data de Vencimento é obrigatória." }; } }
		public Mensagem DataVencimentoInvalida(string licenca) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = $"Data de Vencimento da licença de N° {licenca} inválida." };  }

		#endregion

		#region TipoCorte
		public Mensagem TipoCorteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_TipoCorte", Texto = @"Tipo de Corte é obrigatório." }; } }
		public Mensagem EspecieObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_EspecieInformada", Texto = @"Espécie Informada é obrigatória." }; } }
		public Mensagem AreaCorteObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_AreaCorte", Texto = @"Área de Corte(ha) / N° Árvores é obrigatório." }; } }
		public Mensagem IdadePlantioObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_IdadePlantio", Texto = @"Idade Plantio (anos) é obrigatório." }; } }
		#endregion

		#region Destinacao
		public Mensagem DestinacaoMaterialObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_DestinacaoMaterial", Texto = @"Destinação Material é obrigatório	." }; } }
		public Mensagem ProdutoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_Produto", Texto = @"Produto é obrigatório." }; } }
		public Mensagem QuantidadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "InformacaoCorte_Quantidade", Texto = @"Quantidade é obrigatória." }; } } 
		#endregion

		#region Remover

		public Mensagem ItemExcluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Informação de corte excluída com sucesso." }; } }
		public Mensagem InformacaoCorteListaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Nenhuma Informação de corte adicionada. Pelo menos uma informação de corte deve ser adicionado." }; } }
		public Mensagem InformacaoCorteUltimoItemListaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Não é possivel excluir todas as informações de corte da lista." }; } }

		#endregion
	}
}
