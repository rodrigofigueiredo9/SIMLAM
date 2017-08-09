

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static FiscalizacaoConfiguracaoMsg _fiscalizacaoConfiguracao = new FiscalizacaoConfiguracaoMsg();
		public static FiscalizacaoConfiguracaoMsg FiscalizacaoConfiguracao
		{
			get { return _fiscalizacaoConfiguracao; }
		}
	}

	public class FiscalizacaoConfiguracaoMsg
	{
		public Mensagem Cadastrar(string strNumero) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("Configuração nº {0} cadastrada com sucesso.", strNumero) }; }
		public Mensagem Editar(string strNumero) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("Configuração nº {0} editada com sucesso.", strNumero) }; } 

		#region Tipo Infracao

		public Mensagem SalvarTipoInfracao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Configuração do tipo da infração salva com sucesso." }; } }
		public Mensagem ExcluirTipoInfracao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Tipo de infração excluído com sucesso." }; } }
		public Mensagem ExcluirTipoInfracaoMensagem(String strTipo) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Tem certeza que deseja excluir o tipo de infração {0}?", strTipo) }; }
		public Mensagem ExcluirTipoInfracaoDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Tipo de infração não pode ser excluído, pois está na situação desativado." }; } }
		public Mensagem ExcluirTipoInfracaoJaAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Tipo de infração não pode ser excluído, pois já foi associado a uma fiscalização." }; } }
		public Mensagem EditarTipoInfracaoJaAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Tipo de infração não pode ser editado, pois já foi associado a uma fiscalização." }; } }

		public Mensagem TipoInfracaoNomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_NomeCampo", Texto = "Nome do tipo de infração é obrigatório." }; } }
		public Mensagem TipoInfracaoNaoPodeDesativar(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O tipo de infração está sendo utilizado na(s) configuração (ões) {0}. Acesse a tela de configurar fiscalização e desassocie o tipo de infração para poder desativa-lo.", strIdsAssociados) }; }
		public Mensagem TipoInfracaoNaoPodeExcluir(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não foi possível excluir o tipo de infração, pois o mesmo está associado a configuração {0}.", strIdsAssociados) }; }
		public Mensagem TipoInfracaoNaoPodeEditar(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não foi possível editar o tipo de infração, pois o mesmo está associado a configuração {0}.", strIdsAssociados) }; }
		public Mensagem TipoInfracaoJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_NomeCampo", Texto = "Tipo de infração já adicionado." }; } }
		public Mensagem EditarTipoInfracaoDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Tipo de infração não pode ser editado, pois está na situação desativado." }; } }

		public Mensagem TipoInfracaoSituacaoAlterada { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Situação do tipo de infração alterada com sucesso." }; } }

		#endregion

		#region Item

		public Mensagem SalvarItemInfracao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Configuração do item da infração salva com sucesso." }; } }
		public Mensagem ExcluirItemInfracao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Item de infração excluído com sucesso." }; } }
		public Mensagem ExcluirItemInfracaoMensagem(String strItem) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Tem certeza que deseja excluir o item de infração {0}?", strItem) }; }
		public Mensagem ExcluirItemInfracaoDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Item não pode ser excluído, pois está na situação desativado." }; } }
		public Mensagem ExcluirItemInfracaoJaAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Item não pode ser excluído, pois já foi associado a uma fiscalização." }; } }
		public Mensagem EditarItemInfracaoJaAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Item não pode ser editado, pois já foi associado a uma fiscalização." }; } }

		public Mensagem ItemNomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_NomeCampo", Texto = "Nome do item é obrigatório." }; } }
		public Mensagem ItemInfracaoNaoPodeDesativar(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O item de infração está sendo utilizado na(s) configuração (ões) {0}. Acesse a tela de configurar fiscalização e desassocie o item de infração para poder desativa-lo.", strIdsAssociados) }; }
		public Mensagem ItemInfracaoNaoPodeExcluir(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não foi possível excluir o item de infração, pois o mesmo está associado a configuração {0}.", strIdsAssociados) }; }
		public Mensagem ItemInfracaoNaoPodeEditar(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não foi possível editar o item, pois o mesmo está associado a configuração {0}.", strIdsAssociados) }; }
		public Mensagem ItemJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_NomeCampo", Texto = "Item já adicionado." }; } }
		public Mensagem EditarItemInfracaoDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Item não pode ser editado, pois está na situação desativado." }; } }


		public Mensagem ItemSituacaoAlterada { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Situação do item alterada com sucesso." }; } }

		#endregion

		#region SubItem

		public Mensagem SalvarSubItemInfracao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Configuração do subitem da infração salva com sucesso." }; } }
		public Mensagem ExcluirSubItemInfracao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Subitem de infração excluído com sucesso." }; } }
		public Mensagem ExcluirSubItemInfracaoMensagem(String strSubItem) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Tem certeza que deseja excluir o subitem de infração {0}?", strSubItem) }; }
		public Mensagem ExcluirSubItemInfracaoDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Subitem não pode ser excluído, pois está na situação desativado." }; } }
		public Mensagem ExcluirSubItemInfracaoJaAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Subitem não pode ser excluído, pois já foi associado a uma fiscalização." }; } }
		public Mensagem EditarSubItemInfracaoJaAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Subitem não pode ser editado, pois já foi associado a uma fiscalização." }; } }

		public Mensagem SubItemNomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_NomeCampo", Texto = "Nome do subitem é obrigatório." }; } }
		public Mensagem SubItemInfracaoNaoPodeDesativar(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O Subitem de infração está sendo utilizado na(s) configuração (ões) {0}. Acesse a tela de configurar fiscalização e desassocie o subitem de infração para poder desativa-lo.", strIdsAssociados) }; }
		public Mensagem SubItemInfracaoNaoPodeExcluir(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não foi possível excluir o subitem de infração, pois o mesmo está associado a configuração {0}.", strIdsAssociados) }; }
		public Mensagem SubItemInfracaoNaoPodeEditar(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não foi possível editar o subitem, pois o mesmo está associado a configuração {0}.", strIdsAssociados) }; }
		public Mensagem SubItemJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_NomeCampo", Texto = "Subitem já adicionado." }; } }
		public Mensagem EditarSubItemInfracaoDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Subitem não pode ser editado, pois está na situação desativado." }; } }

		public Mensagem SubItemSituacaoAlterada { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Situação do subitem alterada com sucesso." }; } }

		#endregion

		#region Campo

		public Mensagem SalvarCampoInfracao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Configuração do campo da infração salva com sucesso." }; } }
		public Mensagem ExcluirCampoInfracao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Campo de infração excluído com sucesso." }; } }
		public Mensagem ExcluirCampoInfracaoMensagem(String strCampo) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Tem certeza que deseja excluir o campo {0}?", strCampo) }; }
		public Mensagem ExcluirCampoInfracaoDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Campo não pode ser excluído, pois está na situação desativado." }; } }
		public Mensagem ExcluirCampoInfracaoJaAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Campo não pode ser excluído, pois já foi associado a uma fiscalização." }; } }
		public Mensagem EditarCampoInfracaoJaAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Campo não pode ser editado, pois já foi associado a uma fiscalização." }; } }

		public Mensagem CampoNomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_NomeCampo", Texto = "Nome do Campo é obrigatório." }; } }
		public Mensagem CampoTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_TipoCampo", Texto = "Tipo do Campo é obrigatório." }; } }
		public Mensagem CampoUnidadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_UnidadeMedida", Texto = "Unidade de medida do Campo é obrigatório." }; } }
		public Mensagem CampoInfracaoNaoPodeDesativar(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O campo de infração está sendo utilizado na(s) configuração (ões) {0}. Acesse a tela de configurar fiscalização e desassocie o campo de infração para poder desativa-lo.", strIdsAssociados) }; }
		public Mensagem CampoInfracaoNaoPodeExcluir(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não foi possível excluir o campo de infração, pois o mesmo está associado a configuração {0}.", strIdsAssociados) }; }
		public Mensagem CampoInfracaoNaoPodeEditar(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não foi possível editar o campo, pois o mesmo está associado a configuração {0}.", strIdsAssociados) }; }
		public Mensagem CampoJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_NomeCampo", Texto = "Campo já adicionado." }; } }
		public Mensagem EditarCampoInfracaoDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Campo não pode ser editado, pois está na situação desativado." }; } }

		public Mensagem CampoSituacaoAlterada { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Situação do campo alterada com sucesso." }; } }

		#endregion

		#region Pergunta

		public Mensagem SalvarPerguntaInfracao(string strNumero) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("Pergunta Nº {0} salva com sucesso.", strNumero) }; }

		public Mensagem ExcluirPerguntaInfracao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Pergunta da infração excluída com sucesso." }; } }
		public Mensagem ExcluirPerguntaInfracaoMensagem(String strPergunta) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Tem certeza que deseja excluir a pergunta da infração {0}?", strPergunta) }; }
		public Mensagem ExcluirPerguntaInfracaoDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Pergunta não pode ser excluída, pois está na situação desativado." }; } }
		public Mensagem ExcluirPerguntaInfracaoJaAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Pergunta não pode ser excluída, pois já foi associada à uma fiscalização." }; } }

		public Mensagem PerguntaNomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pergunta_Pergunta", Texto = "Nome da pergunta é obrigatório." }; } }
		public Mensagem PerguntaInfracaoNaoPodeDesativar(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A pergunta da infração está sendo utilizado na(s) configuração (ões) {0}. Acesse a tela de configurar fiscalização e desassocie a pergunta da infração para poder desativa-la.", strIdsAssociados) }; }
		public Mensagem PerguntaInfracaoNaoPodeExcluir(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não foi possível excluir a pergunta de infração, pois a mesma está associada a configuração {0}.", strIdsAssociados) }; }
		public Mensagem PerguntaJaAdicionada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_NomeCampo", Texto = "Pergunta já adicionada." }; } }
		public Mensagem EditarPerguntaInfracaoDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Pergunta não pode ser editada, pois está na situação desativado." }; } }

		public Mensagem PerguntaSituacaoAlterada { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Situação da pergunta alterada com sucesso." }; } }
		public Mensagem PerguntaNumSituacaoAlterada(String strNumero) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Situação da pergunta Nº {0} alterada com sucesso.", strNumero) }; }

		public Mensagem RespostaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pergunta_Resposta", Texto = "Resposta é obrigatória." }; } }
		public Mensagem RespostaEspecificarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pergunta_EspecificarResposta", Texto = "Especificar resposta é obrigatório." }; } }
		public Mensagem RespostaDuplicada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Pergunta_Resposta", Texto = "Resposta já adicionada." }; } }
		public Mensagem RespostaListaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Pelo menos uma resposta deve ser adicionada." }; } }

		#endregion

		#region Resposta

		public Mensagem SalvarRespostaInfracao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Configuração da resposta da infração salva com sucesso." }; } }
		public Mensagem ExcluirRespostaInfracao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Resposta da infração excluída com sucesso." }; } }
		public Mensagem ExcluirRespostaInfracaoMensagem(String strResposta) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Tem certeza que deseja excluir a resposta da infração {0}?", strResposta) }; }
		public Mensagem ExcluirRespostaInfracaoDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Resposta não pode ser excluída, pois está na situação desativado." }; } }
		public Mensagem ExcluirRespostaInfracaoJaAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Resposta não pode ser excluída, pois já foi associada à uma fiscalização." }; } }
		public Mensagem EditarRespostaInfracaoJaAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Resposta não pode ser editada, pois já foi associada a uma fiscalização." }; } }

		public Mensagem RespostaNomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_NomeCampo", Texto = "Nome da resposta é obrigatório." }; } }
		public Mensagem RespostaInfracaoNaoPodeDesativar(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A resposta da infração está sendo utilizado na(s) pergunta (s) {0}. Acesse a tela de pergunta e desassocie a resposta da infração para poder desativa-la.", strIdsAssociados) }; }
		public Mensagem RespostaInfracaoNaoPodeExcluir(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não foi possível excluir a resposta de infração, pois a mesma está associada a pergunta {0}.", strIdsAssociados) }; }
		public Mensagem RespostaInfracaoNaoPodeEditar(String strIdsAssociados) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não foi possível editar a resposta de infração, pois a mesma está associada a pergunta {0}.", strIdsAssociados) }; }
		public Mensagem RespostaJaAdicionada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_NomeCampo", Texto = "Resposta já adicionada." }; } }
		public Mensagem EditarRespostaInfracaoDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Resposta não pode ser editada, pois está na situação desativado." }; } }

		public Mensagem RespostaSituacaoAlterada { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Situação da resposta alterada com sucesso." }; } }

		#endregion

        #region Produtos Apreendidos/Destinação

        public Mensagem ItemProdutoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_NomeProduto, .erroItem", Texto = "Nome do Item é obrigatório." }; } }
        public Mensagem UnidadeProdutoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_UnidadeProduto, .erroUnidade", Texto = "Unidade é obrigatória." }; } }
        public Mensagem ProdutoDuplicado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_NomeProduto, .erroItem", Texto = "Produto já adicionado." }; } }
        public Mensagem DestinoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DestinoProduto, .erroDestino", Texto = "Destino é obrigatório." }; } }
        public Mensagem DestinoDuplicado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "DestinoProduto, .erroDestino", Texto = "Destinação já adicionada." }; } }

        public Mensagem SalvarProdutosDestinos { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Produtos Apreendidos/Destinação salvo com sucesso." }; } }

        #endregion Produtos Apreendidos/Destinação

        public Mensagem ClassificacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Configuracao_Classificacao", Texto = "Classificação é obrigatório." }; } }
		public Mensagem TipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Configuracao_Tipo", Texto = "Tipo de infração é obrigatório." }; } }
		public Mensagem ItemObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Configuracao_Item", Texto = "Item é obrigatório." }; } }
		public Mensagem ItemDesativado(string strCampo, string strNome, bool isPlural = false) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format((isPlural ? "Os (as)" : "O (a)") + " {0} \"{1}\" " + (isPlural ? "estão" : "está") + " na situação desativado.", strCampo, strNome) }; }

		public Mensagem SubitemObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Subitens", Texto = "Subitem é obrigatório." }; } }
		public Mensagem PerguntaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Perguntas", Texto = "Pergunta é obrigatório." }; } }
		public Mensagem CampoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Campos", Texto = "Campo é obrigatório." }; } }

		public Mensagem SubitemJaAdd { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Subitens", Texto = "Subitem já foi adicionado." }; } }
		public Mensagem PerguntaJaAdd { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Perguntas", Texto = "Pergunta já foi adicionada." }; } }
		public Mensagem CampoJaAdd { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Campos", Texto = "Campo já foi adicionado." }; } }

		public Mensagem ConfiguracaoCadastrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Campos", Texto = "Configuração já cadastrada." }; } }

		public Mensagem ExcluirInvalidoSingular(string strFiscalizacao) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("A configuração não pode ser excluída, pois está associada a fiscalização \"{0}\".", strFiscalizacao) }; }
		public Mensagem ExcluirInvalidoPlural(string strFiscalizacoes) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("A configuração não pode ser excluída, pois está associada as fiscalizações \"{0}\".", strFiscalizacoes) }; }

		public Mensagem Excluir(int numero) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Configuração da fiscalização número {0} excluído com sucesso.", numero) }; }
		public Mensagem ExcluirConfirmacao(int numero) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Tem certeza que deseja excluir a configuração número \"{0}\"?", numero) }; }
	}
}
