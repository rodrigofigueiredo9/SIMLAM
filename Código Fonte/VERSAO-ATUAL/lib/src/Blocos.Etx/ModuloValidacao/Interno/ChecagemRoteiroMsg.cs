using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ChecagemRoteiroMsg _checagemRoteirotMsg = new ChecagemRoteiroMsg();

		public static ChecagemRoteiroMsg ChecagemRoteiro
		{
			get { return _checagemRoteirotMsg; }
		}
	}

	public class ChecagemRoteiroMsg
	{
		public Mensagem NomeInteressadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "vm_CheckListRoteiro_Interessado", Texto = "Nome do interessado é obrigatório." }; } }
		public Mensagem RoteiroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Roteiro_Container", Texto = "Roteiro é obrigatório." }; } }
		public Mensagem RoteiroJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O roteiro já está associado." }; } }
		public Mensagem AssociarChecagemSituacaoNaoFinalizada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A checagem deve estar finalizada para ser associada." }; } }
		public Mensagem AssociarChecagemSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A checagem de itens está inválida, pois roteiro utilizado nela foi alterado." }; } }
		public Mensagem ItemJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Item de roteiro já está associado." }; } }
		public Mensagem RemoverItemAssociadoRoteiro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O item não pode ser removido, pois está associado a um roteiro." }; } }
		public Mensagem EditarItemAssociadoRoteiro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O item não pode ser editado, pois está associado a um roteiro." }; } }
		public Mensagem RemoverItemSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O item não pode ser removido na situação #situacao#." }; } }
		public Mensagem EditarItemSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O item não pode ser editado na situação #situacao#." }; } }
		public Mensagem ConferirItemSituacaoIvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O item não pode ser conferido na situação #situacao#." }; } }
		public Mensagem DispensarItemSituacaoIvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O item não pode ser dispensado na situação #situacao#." }; } }
		public Mensagem CancelarDispRecebItemSituacaoIvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O item não pode ser cancelado na situação #situacao#." }; } }
		public Mensagem ItemNaoConferidoOuDispensado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Existem itens não conferidos ou dispensados." }; } }
		public Mensagem ItemPendente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível salvar checagem com item pendente." }; } }
		public Mensagem RoteiroSetorDiferente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível salvar checagem com roteiros de setores diferentes." }; } }
		public Mensagem RoteiroAssociarSetorDiferente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível associar roteiros de setores diferentes." }; } }
		public Mensagem RoteiroInvalidoNaoEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Roteiro inválido ou não encontrado." }; } }
		public Mensagem ItemNomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Nome", Texto = "Nome é obrigatório." }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Checagem número #n# salva com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Checagem de itens de roteiro editado com sucesso." }; } }
		public Mensagem SituacaoFinalizada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Checagem de itens de roteiro já finalizada." }; } }
		public Mensagem SituacaoItemRoteiro { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "A situação do item de roteiro refere-se à última checagem realizada." }; } }
		public Mensagem ItemMotivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Motivo", Texto = "O motivo do item é obrigatório." }; } }

		public Mensagem RoteiroAtualizado(int numero, string versao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = String.Format("O roteiro {0} foi alterado para a versão {1}.", numero, versao) };
		}

		public Mensagem RoteiroDesativado(int numero, eTipoMensagem tipoMsg)
		{
			return new Mensagem() { Tipo = tipoMsg, Texto = String.Format("O roteiro número {0} está desativado.", numero) };
		}

		public Mensagem MensagemExcluirConfirm(string numero)
		{
			return new Mensagem() { Texto = String.Format("Tem certeza que deseja excluir a checagem {0}?", numero) };
		}

		public Mensagem Excluir(int numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Checagem de itens do roteiro {0} excluída com sucesso.", numero) };
		}

		public Mensagem ErroExcluir(string cadastro)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A checagem de itens de roteiro não pode ser excluída, pois está associada ao {0}.", cadastro) };
		}

		public Mensagem ItemRoteiroNaoAssociado(int numero, string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O Item {0} do roteiro número {1} não está associado a checagem de itens de roteiro.", nome, numero) };
		}

		public Mensagem MotivoObrigatorio(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O motivo do item {0} é obrigatório.", nome) };
		}

		public Mensagem PossuiAtividadeDesativada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O roteiro selecionado possui atividade desativada. Por favor, entre em contato com o seu departamento para que seja feita a remoção da atividade deste roteiro." }; } }

		public Mensagem ChecagemSelecionadaPossuiRoteirosAtividadesDesativadas { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A checagem selecionada possui roteiro com atividade desativada. Por favor, entre em contato com o seu departamento para que seja feita a remoção da atividade deste roteiro." }; } }

		public Mensagem AtividadeDesativada(string atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A atividade {0} do requerimento da checagem não pode mais ser solicitada, pois está desativada. Favor removê-la.", atividade) };
		}
	}
}