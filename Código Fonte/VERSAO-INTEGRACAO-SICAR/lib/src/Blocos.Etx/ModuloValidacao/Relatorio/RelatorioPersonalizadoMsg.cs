

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static RelatorioPersonalizadoMsg _relatorioPersonalizadoMsg = new RelatorioPersonalizadoMsg();
		public static RelatorioPersonalizadoMsg RelatorioPersonalizado
		{
			get { return _relatorioPersonalizadoMsg; }
		}
	}

	public class RelatorioPersonalizadoMsg 
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Relatório excluído com sucesso." }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Relatório salvo com sucesso." }; } }
		public Mensagem Importar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Relatório importado com sucesso." }; } }

		public Mensagem MensagemExcluir(string nome)
		{
			return new Mensagem() { Texto = String.Format("Tem certeza que deseja excluir o relatório {0}?", nome) };
		}

		public Mensagem SemPermissaoExecutar { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Usuário sem permissão para executar relatório." }; } }
		public Mensagem NomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Nome", Texto = "Nome é obrigatório." }; } }
		public Mensagem DescricaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Descricao", Texto = "Descrição é obrigatória." }; } }
		public Mensagem SomaColunasInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O somatório das colunas deve ser igual a 100." }; } }
		public Mensagem RelatorioTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Tipo do Relatório é obrigatório." }; } }
		public Mensagem CampoSelecionarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Selecione ao menos um campo." }; } }
		public Mensagem OperadorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Operador", Texto = "Operador é obrigatório." }; } }
		public Mensagem CampoFiltroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Campo", Texto = "Campo é obrigatório." }; } }
		public Mensagem FiltroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Filtro", Texto = "Filtro é obrigatório." }; } }
		public Mensagem ExportadorInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Tipo de saída do relatório inválido." }; } }
		public Mensagem ErroAoExecutarRelatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Erro ao executar relatório." }; } }
		public Mensagem ConfiguracaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não foi possível ler a configuração do relatório." }; } }
		public Mensagem FonteDadosNaoEncontrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A fonte de dados associada ao relatório não foi encontrada." }; } }
		public Mensagem FonteDadosDesatualizada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A fonte de dados associada ao relatório mudou recentemente. Atualize a configuração do relatório." }; } }
		public Mensagem SelecioneArquivo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "inputFile", Texto = "Selecione o arquivo para ser importado." }; } }
		public Mensagem SetorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Setor", Texto = "Setor é obrigatório." }; } }
		public Mensagem CamposFiscalizacaoNaoImportado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Os campos referentes a \"Campos, Perguntas e Respostas\" não foram importados." }; } }

		public Mensagem TermoNaoDefinido(string nome)
		{
			return new Mensagem() { Texto = String.Format(@"O valor do campo ""{0}"" não foi definido antes da execução.", nome) };
		}

		public Mensagem CampoObrigatorio(string filtro, int posicao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Filtro" + posicao, Texto = String.Format("Campo {0} é obrigatório.", filtro) };
		}

		public Mensagem FiltroInvalido(string filtro, int posicao = 0)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Filtro" + posicao, Texto = String.Format("Filtro \"{0}\" inválido.", filtro) };
		}
	}
}