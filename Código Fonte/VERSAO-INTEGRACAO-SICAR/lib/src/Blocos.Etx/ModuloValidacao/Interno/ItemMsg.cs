using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{

	public partial class Mensagem
	{
		private static ItemMsg _itemMsg = new ItemMsg();

		public static ItemMsg Item
		{
			get { return _itemMsg; }
		}
	}

	public class ItemMsg
	{
		public Mensagem ItemAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Item #ITEM# associado com sucesso." }; } }
		public Mensagem ItemJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Item #ITEM# já está adicionado." }; } }
		
		public Mensagem ItemExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_Nome", Texto = "O item já está cadastrado." }; } }
		
		public Mensagem NomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_Nome", Texto = "Nome do item é obrigatório." }; } }
		public Mensagem TipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Tipo", Texto = "Tipo do item é obrigatório." }; } }

		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Item salvo com sucesso." }; } }
		public Mensagem CadastrarSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Item cadastrado com sucesso." }; } }
		public Mensagem EditarSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Item editado com sucesso." }; } }
		public Mensagem ItemEditado { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Item editado com sucesso." }; } }
		public Mensagem ItemExcluido { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Item excluído com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A alteração deste Item implicará em todos os Roteiros." }; } }
		public Mensagem NaoPermiteManipular { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O item não pode ser manipulado." }; } }
		public Mensagem NaoPermiteVisualizar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O item não pode ser visualizado." }; } }
		public Mensagem TipoNaoPermitido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O tipo do item não é permitido." }; } }

		public Mensagem ItemObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Container_ItemRoteiro", Texto = "Pelo menos um item de roteiro deve estar adicionado." }; } }

		public Mensagem MensagemExcluir(string nome)
		{
			return new Mensagem() { Texto = String.Format("Tem certeza que deseja excluir o item {0}?", nome) };
		}

		public Mensagem ItemExcluidoSistema(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O item {0} foi excluído do sistema, favor remove-lo.", nome) };
		}

		public Mensagem ItemAssociado(string acao, string roteiros)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("O item não pode ser {0} pois está sendo utilizado no(s) roteiro(s) {1}.", acao, Mensagem.Concatenar(roteiros, ',')) };
		}

		public Mensagem ItemAssociadoAnalise(string acao, List<String> protocolo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("O item não pode ser {0} pois está sendo utilizado na(s) análise(s) do(s) {1}.", acao, Mensagem.Concatenar(protocolo)) };
		}

		public Mensagem ItemAssociadoChecagem(string acao, List<String> checagem)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("O item não pode ser {0} pois está sendo utilizado na(s) checagem(s) {1}.", acao, Mensagem.Concatenar(checagem)) };
		}
	}
}