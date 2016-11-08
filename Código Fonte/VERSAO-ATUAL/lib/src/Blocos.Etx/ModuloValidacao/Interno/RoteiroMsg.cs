using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static RoteiroMsg _roteiroMsg = new RoteiroMsg();

		public static RoteiroMsg Roteiro
		{
			get { return _roteiroMsg; }
		}
	}

	public class RoteiroMsg
	{
		public Mensagem NaoEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Roteiro não encontrado." }; } }
		public Mensagem NomeExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Roteiro_Nome", Texto = "O nome já está sendo utilizado por um roteiro." }; } }
		public Mensagem ObrigatorioNumero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Roteiro_Numero", Texto = "Número do roteiro é obrigatório." }; } }
		public Mensagem ObrigatorioVersao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Roteiro_Versao", Texto = "Versão do roteiro é obrigatório." }; } }
		public Mensagem ObrigatorioNome { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Roteiro_Nome", Texto = "Nome do roteiro é obrigatório." }; } }
		public Mensagem ObrigatorioSetor { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Roteiro_Setor", Texto = "Setor do roteiro é obrigatório." }; } }
		public Mensagem FuncionarioSetorDiferente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Roteiro_Setor", Texto = "Funcionário logado não pertence a este setor." }; } }

		public Mensagem ItemExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_Nome", Texto = "O item #nome já está adicionado." }; } }
		public Mensagem NenhumTituloEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Não existe título configurado para a atividade associada." }; } }
		public Mensagem RoteiroDesativo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este roteiro esta desativado." }; } }
		public Mensagem ItemObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Item_Nome", Texto = "Nome do item é obrigatório." }; } }
		public Mensagem ArquivoAnexoExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ArquivoTexto", Texto = "Já existe um item com este nome." }; } }

		public Mensagem ArquivoAnexoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ArquivoId_", Texto = "Arquivo é obrigatório." }; } }
		public Mensagem DescricaoAnexoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Descricao", Texto = "Descrição é obrigatória." }; } }
		public Mensagem ArquivoAnexoNaoEhPDF { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ArquivoTexto", Texto = "Arquivo não é um PDF." }; } }
		public Mensagem PalavraChaveExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Roteiro_PalavraChave", Texto = "Já existe uma palavra-chave com este nome." }; } }
		public Mensagem PalavraChaveObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Roteiro_PalavraChave", Texto = "Palavra-chave é obrigatória." }; } }

		public Mensagem ModeloObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Titulo é Obrigatório." }; } }
		public Mensagem FinalidadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Finalidade é Obrigatória." }; } }
		public Mensagem AtividadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Atividade é Obrigatória." }; } }
		public Mensagem AtividadeDesativada(string atividade) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A atividade {0} está desativada. Favor removê-la.", atividade) };  }
		public Mensagem SituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O roteiro não pode ser editado pois está desativado." }; } }
		public Mensagem RoteiroPadrao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O roteiro padrão não pode ser desativado." }; } }
		public Mensagem AtividadeSetorDiferenteRoteiro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível associar atividade solicitada de setor diferente do selecionado no roteiro. Verifique a atividade solicitada ou faça novo roteiro." }; } }
		public Mensagem NaoEditarRoteiroDeOutroSetor { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O roteiro não poderá ser editado, pois não faz parte do setor do usuário logado." }; } }

		public Mensagem NaoPodeAssociarDesativado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O roteiro não pode ser associado, pois está desativado." }; } }

		public Mensagem NaoEncontrouRegistros { get { return Mensagem.Padrao.NaoEncontrouRegistros; } }

		public Mensagem AtualizarRoteiro(string roteiros)
		{
			return new Mensagem() { Texto = String.Format(@"Ao editar este item a versão do(s) roteiro(s) {0} será alterada. Deseja confirmar a edição?", Mensagem.Concatenar(roteiros)) };
		}

		public Mensagem Salvar(int numero, int versaoAtual)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Roteiro {0} na versão {1} salvo com sucesso.", numero, versaoAtual) };
		}

		public Mensagem Editar(int numero, int? versaoAnterior, int? versaoAtual)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Roteiro {0} na versão {1} editado com sucesso.", numero, versaoAnterior) }; 
		}

		public Mensagem Ativar(int numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = String.Format("Roteiro {0} Ativado com sucesso.", numero) };
		}

		public Mensagem Desativar(int numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = String.Format("Roteiro {0} desativado com sucesso.", numero) };
		}

		public Mensagem RoteiroAtividadeConfigurada(List<string> atividades, List<string> finalidades, List<string> titulos)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = String.Format("Já existe roteiro cadastrado para a(s) atividade(s) {0}, com a(s) finalidade(s) {1}, e para o(s) título(s) {2}.", Mensagem.Concatenar(atividades), Mensagem.Concatenar(finalidades), Mensagem.Concatenar(titulos)) };
		}

		public Mensagem TituloNaoEncontradoAtividade(string modelo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O título {0} não pode ser selecionado, não está mais configurado para a atividade associada.", modelo) };
		}

		public Mensagem DesativarConfirm(string nome)
		{
			return new Mensagem() { Texto = String.Format("Tem certeza que deseja desativar o roteiro {0}?", nome) };
		}

		public Mensagem SetorSemRoteiroPadrao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "​Não existe roteiro padrão para o setor." }; } }

		public Mensagem TituloNaoAdicionadoRoteiroInterno(string modelos) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O(s) Título(s) {0} não está(ão) adicionado(s) no Roteiro Orientativo da atividade solicitada. Verifique o Roteiro da atividade solicitada ou faça um novo Roteiro Orientativo.", modelos) }; }
		public Mensagem TituloNaoAdicionadoRoteiroCredenciado(string modelos) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O(s) Título(s) {0} não está(ão) adicionado(s) no Roteiro Orientativo da atividade solicitada. Solicite ao IDAF que verifique o Roteiro Orientativo da atividade solicitada.", modelos) }; }
	}
}