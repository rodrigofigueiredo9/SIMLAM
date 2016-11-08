namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ComplementacaoDadosMsg _complementacaoDadosMsg = new ComplementacaoDadosMsg();
		public static ComplementacaoDadosMsg ComplementacaoDados
		{
			get { return _complementacaoDadosMsg; }
			set { _complementacaoDadosMsg = value; }
		}
	}

	public class ComplementacaoDadosMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Complementação de dados do autuado salvo com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Complementação de dados do autuado editado com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Complementação de dados do autuado excluído com sucesso." }; } }

		public Mensagem ResponsavelObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_Responsavel", Texto = "Responsável é obrigatório." }; } }
		public Mensagem ResidePropriedadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_ResidePropriedadeTipo", Texto = "Reside na propriedade é obrigatório." }; } }
		public Mensagem RendaMensalFamiliarObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_RendaMensalFamiliarTipo", Texto = "Renda mensal familiar é obrigatória." }; } }
		public Mensagem NivelEscolaridadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_NivelEscolaridadeTipo", Texto = "Nível de escolaridade é obrigatório." }; } }
		public Mensagem VinculoComPropriedadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_VinculoComPropriedadeTipo", Texto = "Vínculo com a propriedade é obrigatório." }; } }
		public Mensagem VinculoComPropriedadeEspecificarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_VinculoComPropriedadeEspecificarTexto", Texto = "Texto especificar do vínculo com a propriedade é obrigatório." }; } }
		public Mensagem ConhecimentoLegislacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_ConhecimentoLegislacaoTipo", Texto = "Conhecimento quanto a legislação é obrigatório." }; } }
		public Mensagem JustificativaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_Justificativa", Texto = "Justificativa do conhecimento quanto a legislação é obrigatória." }; } }
		public Mensagem ReservalegalObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_ReservalegalTipo, .ReservalegalTipo", Texto = "Área de reserva legal é obrigatória." }; } }

		public Mensagem AreaTotalInformadaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_AreaTotalInformada", Texto = "Área total informada é obrigatória." }; } }
		public Mensagem AreaTotalInformadaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_AreaTotalInformada", Texto = "Área total informada é inválida." }; } }
		public Mensagem AreaTotalInformadaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_AreaTotalInformada", Texto = "Área total informada é deve ser maior do que zero." }; } }

		public Mensagem AreaCoberturaFlorestalNativaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_AreaCoberturaFlorestalNativa", Texto = "Área com cobertura florestal nativa informada/estimada é obrigatória." }; } }
		public Mensagem AreaCoberturaFlorestalNativaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_AreaCoberturaFlorestalNativa", Texto = "Área com cobertura florestal nativa informada/estimada é inválida." }; } }
		public Mensagem AreaCoberturaFlorestalNativaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Complementacao_AreaCoberturaFlorestalNativa", Texto = "Área com cobertura florestal nativa informada/estimada deve ser maior do que zero." }; } }
	
	}
}
