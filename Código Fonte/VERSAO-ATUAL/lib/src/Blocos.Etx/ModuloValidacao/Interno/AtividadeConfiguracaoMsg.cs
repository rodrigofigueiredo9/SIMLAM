

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static AtividadeConfiguracaoMsg _atividadeConfiguracaoMsg = new AtividadeConfiguracaoMsg();

		public static AtividadeConfiguracaoMsg AtividadeConfiguracao
		{
			get { return _atividadeConfiguracaoMsg; }
		}
	}

	public class AtividadeConfiguracaoMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Atividade configurada com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Configuração da atividade excluida com sucesso." }; } }
		public Mensagem AtividadeAssociada { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Atividade #ATIVIDADE# associada com sucesso" }; } }

		public Mensagem AtividadeDesabilitada(string atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não será possível editar esta configuração, pois a atividade \"{0}\" está desabilitada.", atividade) };
		}

		public Mensagem MensagemExcluir(string nome)
		{
			return new Mensagem() { Texto = String.Format("Tem certeza que deseja excluir a configuração {0}?", nome) };
		}

		#region Validações

		public Mensagem NomeGrupoObrigatorio { get { return new Mensagem() { Campo = "Configuracao_NomeGrupo", Tipo = eTipoMensagem.Advertencia, Texto = "Nome do grupo obrigatório." }; } }
		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Campo = "Container_Atividade", Tipo = eTipoMensagem.Advertencia, Texto = "Atividade solicitada é obrigatória." }; } }
		public Mensagem AtividadeJaAssociada { get { return new Mensagem() { Campo = "Container_Atividade", Tipo = eTipoMensagem.Advertencia, Texto = "Atividade já adicionada." }; } }
		public Mensagem ModeloObrigatorio { get { return new Mensagem() { Campo = "Modelos", Tipo = eTipoMensagem.Advertencia, Texto = "Modelo é obrigatório." }; } }
		public Mensagem ModeloJaAssociado { get { return new Mensagem() { Campo = "Modelos", Tipo = eTipoMensagem.Advertencia, Texto = "Modelo já adicionado." }; } }
		public Mensagem TituloEmitidoObrigatorio { get { return new Mensagem() { Campo = "Container_Titulo", Tipo = eTipoMensagem.Advertencia, Texto = "Título emitido é obrigatório." }; } }

		public Mensagem AtividadeSetorDiferentes { get { return new Mensagem() { Campo = "Container_Titulo", Tipo = eTipoMensagem.Advertencia, Texto = "As atividades associadas devem ser do mesmo setor." }; } }

		public Mensagem AtividadeJaConfigurada(string nomeGrupo)
		{
			return new Mensagem() { Campo = "Container_Atividade", Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A atividade já foi configurada no grupo {0}.", nomeGrupo) };
		}
		#endregion
	}
}