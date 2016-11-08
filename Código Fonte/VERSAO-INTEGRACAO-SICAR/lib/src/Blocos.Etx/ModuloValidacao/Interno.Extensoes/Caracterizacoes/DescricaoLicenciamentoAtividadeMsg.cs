

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static DescricaoLicenciamentoAtividadeMsg _descricaoLicenciamentoAtividadeMsg = new DescricaoLicenciamentoAtividadeMsg();
		public static DescricaoLicenciamentoAtividadeMsg DescricaoLicenciamentoAtividadeMsg
		{
			get { return _descricaoLicenciamentoAtividadeMsg; }
			set { _descricaoLicenciamentoAtividadeMsg = value; }
		}
	}

	public class DescricaoLicenciamentoAtividadeMsg
	{
		public Mensagem SelecioneResponsavel { get { return new Mensagem() { Texto = "Selecione o reponsável pela atividade.", Campo = "#ddlRespAtiv", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformePatrimonio { get { return new Mensagem() { Texto = "Infome se há patrimônio histórico cultural na área útil.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeResidencia { get { return new Mensagem() { Texto = "Infome se há residência(s) no entorno.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeDistancia { get { return new Mensagem() { Texto = "Infome a Distância.", Campo = "#txtResidentesEnternoDistancia", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeAreaUtil { get { return new Mensagem() { Texto = "Infome a Área útil.", Campo = "#txtAreaUtil", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem SelecioneTipoFonteAbastecimento { get { return new Mensagem() { Texto = "Selecione o tipo de Fonte de abastecimento de água.", Campo = "#ddlFontesAbastecimentoAguaTipo", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem FonteAguaExistente { get { return new Mensagem() { Texto = "Fonte de Abastecimento já foi adicionada.", Campo = "#ddlFontesAbastecimentoAguaTipo", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeFonteUso { get { return new Mensagem() { Texto = "Informe a(o) {0}.", Campo = "#txtFonteUso", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem AddFonteAbastecimento { get { return new Mensagem() { Texto = "Adicione ao menos uma fonte de abastecimento de água.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeConsumo { get { return new Mensagem() { Texto = "Informe ao menos um tipo de Consumo de água L/s, m³/h, m³/dia ou m³/mês.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem AddPontoLancamento { get { return new Mensagem() { Texto = "Informe informe ao menos um ponto de lançamento.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem SelecionePontoLancamento { get { return new Mensagem() { Texto = "Selecione o tipo de Ponto de Lançamento de Efluente.", Campo = "#ddlPontosLancamentoEfluenteTipo", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformePontoLancamento { get { return new Mensagem() { Texto = "Informe a(o) {0}.", Campo = "#txtPontoLancamento", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem PontoLancamentoExistente { get { return new Mensagem() { Texto = "Ponto de Lançamento já foi adicionado.", Campo = "#ddlPontosLancamentoEfluenteTipo", Tipo = eTipoMensagem.Advertencia }; } }
		
		public Mensagem SelecioneTipoFonteGeracao { get { return new Mensagem() { Texto = "Selecione o tipo de Fonte de Geração.", Campo = "#ddlFontesGeracaoTipo", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem TipoFonteGeracaoExistente { get { return new Mensagem() { Texto = "Fonte de Geração já foi adicionado.", Campo = "#ddlFontesGeracaoTipo", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeVazao { get { return new Mensagem() { Texto = "Informe a Vazão.", Campo = "#txtVazao", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeVazaoValida { get { return new Mensagem() { Texto = "Informe uma Vazão válida.", Campo = "#txtVazao", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeUnidade { get { return new Mensagem() { Texto = "Informe a Unidade.", Campo = "#ddlUnidadeTipo", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeSisTratamento { get { return new Mensagem() { Texto = "Informe o Sistema de Tratamento.", Campo = "#txtSisTratamento", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeOutraFonteGeracao { get { return new Mensagem() { Texto = "Informe a Outra Fonte de Geração.", Campo = "#txtEflLiquidoEspecificar", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem AddFonteGeracao { get { return new Mensagem() { Texto = "Adicione ao menos uma Fonte de Geração.", Tipo = eTipoMensagem.Advertencia }; } }
		
		public Mensagem InformeClasseResiduo { get { return new Mensagem() { Texto = "Informe a Classe do Resíduo.", Campo = "#txtClasseRediduo", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeTipoResiduo { get { return new Mensagem() { Texto = "Informe o Tipo de resíduo.", Campo = "#txtTipoRediduo", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeAcondicionamento { get { return new Mensagem() { Texto = "Informe o Acondicionamento.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeEstocagem { get { return new Mensagem() { Texto = "Informe a Estocagem.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeTratamento { get { return new Mensagem() { Texto = "Informe o Tratamento.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeTratamentoOutro { get { return new Mensagem() { Texto = "Informe o outro Tratamento.", Campo = "#txtTratamentoEspecificar", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeDestinoFinal { get { return new Mensagem() { Texto = "Informe o  Destino Final.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeDestinoFinalOutro { get { return new Mensagem() { Texto = "Informe o outro Destino Final.", Campo = "#txtDestinoFinalEspecificar", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem AddResiduoSolidoNaoInerte { get { return new Mensagem() { Texto = "Adicione ao menos um resíduo sólido não inerte.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem SelecioneCombustivel { get { return new Mensagem() { Texto = "Selecione o tipo de combustível.", Campo = "#ddlCombustivel", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CombustivelExistente { get { return new Mensagem() { Texto = "Tipo de combustível já foi adicionado.", Campo = "#ddlCombustivel", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeSubstancia { get { return new Mensagem() { Texto = "Informe a Substância emitida.", Campo = "#txtSubstanciaEmitida", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem InformeEquipamento { get { return new Mensagem() { Texto = "Informe o Equipamento de controle.", Campo = "#txtEquipamentoControle", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem AddEmissoesAtm { get { return new Mensagem() { Texto = "Adicione ao menos um tipo de Emissão atmosferica.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem DscLicAtvSalvoSucesso { get { return new Mensagem() { Texto = "Descrição de Atividade salva com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem DscLicAtvExcluidoSucesso { get { return new Mensagem() { Texto = "Descrição de Atividade excluída com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }

		public Mensagem InformeZonaUC { get { return new Mensagem() { Texto = "Inforne se está em zona de amortecimento de UC.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NaoPodeVisualizarComDependenciasAlteradas(String caracterizacao)
		{
			return new Mensagem()
			{
				Tipo = eTipoMensagem.Advertencia,
				Texto = String.Format("Para visualizar os dados é necessário que a descrição de atividade da caracterização de {0} esteja válida.", caracterizacao)
			};
		}
	}
}
