

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
    public partial class Mensagem
    {
        private static OrgaoParceiroConveniadoMsg _orgaoParceiroConveniado = new OrgaoParceiroConveniadoMsg();
        public static OrgaoParceiroConveniadoMsg OrgaoParceiroConveniado
        {
            get { return _orgaoParceiroConveniado; }
        }
    }

	public class OrgaoParceiroConveniadoMsg
	{
		public Mensagem UnidadeSiglaNomeLocalObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso informar pelo menos um dos campos: ‘Sigla’ ou ‘Nome / Local da Unidade’" }; } }
		public Mensagem UnidadeJaAdicionada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Unidade já foi adicionada." }; } }
		public Mensagem NomeOrgaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NomeOrgao", Texto = "Nome do órgão é obrigatório." }; } }
		public Mensagem SiglaOrgaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Sigla", Texto = "Sigla é obrigatório." }; } }
		public Mensagem UnidadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fsUnidade", Texto = "Unidade é obrigatório." }; } }
		public Mensagem NovaSituacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Situacao_Nova", Texto = "Nova situação é obrigatória." }; } }
		public Mensagem SituacaoMotivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Situacao_Motivo", Texto = "Motivo é obrigatório." }; } }
		public Mensagem OrgaoParceiroJaExiste { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Órgão Parceiro/ Conveniado já foi cadastrado." }; } }
		public Mensagem SelecioneUmCredenciado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso que ao menos um credenciado seja selecionado." }; } }
		public Mensagem ChaveGeradaSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Chave gerada com sucesso. Um e-mail com o número da chave de acesso foi enviado ao(s) credenciado(s) selecionado(s)." }; } }
		public Mensagem UnidadeExcluidaSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Unidade excluída com sucesso." }; } }
		public Mensagem SituacaoAlteradaSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Situação alterada com sucesso." }; } }
		public Mensagem CredenciadosBloqueadosSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Situação alterada para ‘Bloqueado’ com sucesso." }; } }
		public Mensagem TituloConfirmBloquear { get { return new Mensagem() { Texto = "Deseja bloquear?" }; } }
		public Mensagem TituloConfirmAtivar { get { return new Mensagem() { Texto = "Deseja ativar?" }; } }
		public Mensagem TituloConfirmGerarChave { get { return new Mensagem() { Texto = "Gerar chave?" }; } }
		public Mensagem TituloConfirmBloquearCredenciado { get { return new Mensagem() { Texto = "Bloquear Credenciados?" }; } }
		public Mensagem TituloConfirmDesbloquearCredenciado { get { return new Mensagem() { Texto = "Desbloquear Credenciados?" }; } }
		public Mensagem ConfirmExcluirUnidade { get { return new Mensagem() { Tipo = eTipoMensagem.Confirmacao, Texto = "Tem certeza que deseja excluir a unidade #siglaUnidade#?" }; } }
		public Mensagem ConfirmBloquearCredenciado { get { return new Mensagem() { Tipo = eTipoMensagem.Confirmacao, Texto = "Essa ação irá bloquear o cadastro e o acesso ao Módulo Credenciado do(s) credenciado(s) selecionado(s). Tem certeza que deseja bloquear?" }; } }
		public Mensagem ConfirmDesbloquearCredenciado { get { return new Mensagem() { Tipo = eTipoMensagem.Confirmacao, Texto = "Essa ação irá ativar o cadastro do(s) credenciado(s) selecionado(s) e enviar uma nova chave de acesso no e-mail deles. Tem certeza que deseja desbloquear?" }; } }
		public Mensagem ConfirmGerarChave { get { return new Mensagem() { Tipo = eTipoMensagem.Confirmacao, Texto = "Essa ação irá gerar uma chave de acesso e enviá-la no e-mail do(s) credenciado(s) selecionado(s). Tem certeza que deseja gerar a chave de acesso?" }; } }
		public Mensagem OrgaoBloqueado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É preciso que a situação do órgão parceiro/ conveniado #nome# seja igual a 'Ativo' ." }; } }
		
		public Mensagem ConfirmBloquear
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.Confirmacao, Texto = "Esta ação irá bloquear todos os credenciados cadastrados para o órgão Parceiro/ Conveniado #siglaNome#, incluindo as pessoas do órgão que estão aguardando ativação e credenciados ativos. Tem certeza que deseja bloquear o órgão parceiro/ conveniado?" }; }
		}

		public Mensagem ConfirmAtivar
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.Confirmacao, Texto = "Está ação irá ativar somente o cadastro do Órgão Parceiro/ Conveniado #siglaNome#, mantendo a situação do cadastro dos credenciados deste órgão bloqueados. Tem certeza que deseja ativar o Órgão Parceiro/ Conveniado?" }; }
		}
		
		public Mensagem SalvarOrgaoParceiroConveniado(string Id)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Órgão Parceiro/ Conveniado {0} salvo com sucesso", Id) };
		}

		public Mensagem UnidadeAssociadaCredenciado(string sigla)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A Unidade {0} apresenta credenciados associados", sigla) };
		}


		public Mensagem SituacaoJaAlterada(string situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível alterar para a situação {0}, pois a mesma já foi alterada", situacao) };
		}

		public Mensagem CredenciadoNaoMaisAssociadoOrgao(string credenciadoNome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = String.Format("O credenciado {0}  não está mais associado ao órgão parceiro/ conveniado", credenciadoNome) };
		}

		public Mensagem OrgaoParceiroBloqueado(string orgaoParceiroNome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("É preciso que a situação de cadastro do órgão parceiro/ conveniado {0} seja igual a 'Ativo'. ", orgaoParceiroNome) };

		}
		
	}
}
