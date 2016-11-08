

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static AcompanhamentoMsg _acompanhamentoMsg = new AcompanhamentoMsg();
		public static AcompanhamentoMsg Acompanhamento
		{
			get { return _acompanhamentoMsg; }
		}
	}

	public class AcompanhamentoMsg
	{
		public Mensagem Salvar(string strNumero) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("Acompanhamento Nº {0} salvo com sucesso.", strNumero) }; }
		public Mensagem Excluir(string numero) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Acompanhamento {0} excluído com sucesso.", numero) }; }
		public Mensagem ExcluirConfirmacao(string numero) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Tem certeza que deseja excluir o Acompanhamento {0}?", numero) }; }
		public Mensagem ExcluirInvalido(string numero, string situacao) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não foi possível excluir o Acompanhamento {0}, pois o mesmo está com a situação \"{1}\".", numero, situacao) }; }

		public Mensagem SetorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_Setor", Texto = "Setor é obrigatório." }; } }
		public Mensagem ReservalegalObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_ReservalegalTipo, .ReservalegalTipo", Texto = "Área de reserva legal é obrigatória." }; } }

		public Mensagem AtividadeAreaEmbargadaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_AtividadeAreaEmbargada, #SpnAtividadeAreaEmbargada", Texto = "Está sendo desenvolvida alguma atividade na área degradada é obrigatório." }; } }
		public Mensagem AtividadeAreaEmbargadaEspecificarTextoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_AtividadeAreaEmbargadaEspecificarTexto", Texto = "Especificar da atividade desenvolvida na área é obrigatório." }; } }

		public Mensagem InfracaoResultouErosaoEspecificarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_InfracaoResultouErosaoEspecificar", Texto = "Especificar da infração que resultou em erosão do solo é obrigatório." }; } }		

		public Mensagem AreaTotalObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_AreaTotal", Texto = "Área total informada é obrigatória." }; } }
		public Mensagem AreaTotalInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_AreaTotal", Texto = "Área total informada é inválida." }; } }
		public Mensagem AreaTotalMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_AreaTotal", Texto = "Área total informada é deve ser maior do que zero." }; } }

		public Mensagem AreaNativaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_AreaNativa", Texto = "Área com cobertura florestal nativa informada/estimada é obrigatória." }; } }
		public Mensagem AreaNativaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_AreaNativa", Texto = "Área com cobertura florestal nativa informada/estimada é inválida." }; } }
		public Mensagem AreaNativaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_AreaNativa", Texto = "Área com cobertura florestal nativa informada/estimada deve ser maior do que zero." }; } }

		public Mensagem HouveDesrespeitoTADObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_HouveDesrespeitoTAD, #SpnHouveDesrespeitoTAD", Texto = "Houve desrespeito ao TAD é obrigatório." }; } }
		public Mensagem HouveDesrespeitoTADEspecificarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_HouveDesrespeitoTADEspecificar", Texto = "Especificar do houve desrespeito ao TAD é obrigatório." }; } }

		public Mensagem RepararDanoAmbientalObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_RepararDanoAmbiental, #SpnRepararDanoAmbiental", Texto = "Há necessidade de reparo de dano ambiental é obrigatório." }; } }
		public Mensagem RepararDanoAmbientalEspecificarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_RepararDanoAmbientalEspecificar", Texto = "Especificar da necessidade de reparo de dano ambiental é obrigatório." }; } }

		public Mensagem FirmouTermoRepararDanoAmbientalObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_FirmouTermoRepararDanoAmbiental, #SpnFirmouTermoRepararDanoAmbiental", Texto = "Firmou termo de compromisso para reparação do dano de acordo com a forma sugerida é obrigatório." }; } }
		public Mensagem FirmouTermoRepararDanoAmbientalEspecificarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Acompanhamento_FirmouTermoRepararDanoAmbientalEspecificar", Texto = "Especificar do firmou termo de compromisso para reparação do dano de acordo com a forma sugerida é obrigatório." }; } }

		public Mensagem AssinanteSetorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Setor do assinante é obrigatorio." }; } }
		public Mensagem AssinanteFuncionarioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Nome do assinante é obrigatorio." }; } }
		public Mensagem AssinanteCargoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Cargo é obrigatorio." }; } }
		public Mensagem AssinanteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fdsAssinante", Texto = "Assinante é obrigatório." }; } }
		public Mensagem AssinanteFuncionarioLogado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fdsAssinante", Texto = "O autor do cadastro deverá ser associado como assinante." }; } }
		public Mensagem AssinanteFuncionarioUnico { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fdsAssinante", Texto = "O autor do cadastro deverá ser associado como assinante." }; } }
		public Mensagem AssinanteJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Assinante ja adicionado." }; } }

		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fileTermo", Texto = "Arquivo é obrigatório." }; } }
		public Mensagem ArquivoNaoEhPdf { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fileTermo", Texto = "Arquivo não é do tipo pdf" }; } }

		public Mensagem SituacaoSalvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Situação do acompanhamento alterada com sucesso." }; } }
		public Mensagem SituacaoNovaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "SituacaoNova", Texto = "Nova situação é obrigatório." }; } }
		public Mensagem SituacaoMotivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Motivo", Texto = "Motivo é obrigatório." }; } }
		public Mensagem SituacaoDataAntigaMaiorNova { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DataSituacaoNova", Texto = "Data da nova situação deve ser maior ou igual que a data da situação atual." }; } }

		public Mensagem Acompanhamentos { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para cadastrar acompanhamento é preciso que a fiscalização tenha sido protocolada." }; } }
		public Mensagem PosseProcessoAlterarSituacao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso ter a posse do processo para alterar a situação do acompanhamento de fiscalização." }; } }
		public Mensagem SituacaoInvalidaAlterarSituacao(string situacao) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Não é possível alterar situação com acompanhamento na situação {0}.", situacao) }; }
		public Mensagem SituacaoInvalidaEditar(string situacao) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Não é possível editar acompanhamento na situação {0}.", situacao) }; }

		public Mensagem ArquivoNaoEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Erro, Texto = "Não foi encontrado o arquivo concluído." }; } }
	}
}