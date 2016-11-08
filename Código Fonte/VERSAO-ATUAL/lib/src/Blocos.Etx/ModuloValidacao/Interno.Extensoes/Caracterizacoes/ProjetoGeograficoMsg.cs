using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ProjetoGeograficoMsg _projetoGeografico = new ProjetoGeograficoMsg();
		public static ProjetoGeograficoMsg ProjetoGeografico
		{
			get { return _projetoGeografico; }
			set { _projetoGeografico = value; }
		}
	}

	public class ProjetoGeograficoMsg
	{
		public Mensagem SalvoSucesso { get { return new Mensagem() { Texto = "Projeto Geográfico salvo com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem RefeitoSucesso { get { return new Mensagem() { Texto = "Projeto Geográfico refeito com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem RecarregadoSucesso { get { return new Mensagem() { Texto = "Projeto Geográfico recarregado com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem RascunhoExcluidoSucesso { get { return new Mensagem() { Texto = "Rascunho do Projeto Geográfico excluído com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem FinalizadoSucesso { get { return new Mensagem() { Texto = "Projeto Geográfico finalizado com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }

		public Mensagem Finalizado { get { return new Mensagem() { Texto = "Projeto Geográfico deve estar finalizado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem Obrigatorio { get { return new Mensagem() { Texto = "Projeto Geográfico é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem Inexistente { get { return new Mensagem() { Texto = "Projeto Geográfico inexistente.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CaracterizacaoTipo { get { return new Mensagem() { Texto = "Projeto Geográfico não é do mesmo tipo da caracterização.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem CoordenadaObrigatoria { get { return new Mensagem() { Texto = "Coordenada é obrigatória, favor editar o empreendimento.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem SituacaoDeveSerProcessado { get { return new Mensagem() { Texto = "Para finalizar o projeto geográfico é necessário que a situação do processamento seja igual a processado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem SituacaoProjetoDeveSerFinalizar { get { return new Mensagem() { Texto = "O projeto geográfico deve estar finalizado para ser refeito.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem SituacaoProjetoFinalizado { get { return new Mensagem() { Texto = "O projeto geográfico já está finalizado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem SituacaoProjetoNaoFinalizado { get { return new Mensagem() { Texto = "O projeto geográfico não está finalizado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem JaExisteCadastro { get { return new Mensagem() { Texto = "Já existe um projeto geográfico para esse empreendimento.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem EmpreendimentoNaoCadastrao { get { return new Mensagem() { Texto = "O empreendimento não cadastrado no sistema.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem EmpreendimentoInexistente { get { return new Mensagem() { Texto = "O empreendimento não cadastrado no sistema.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem NivelPrecisaoObrigatorio { get { return new Mensagem() { Campo = "Projeto_NivelPrecisaoId", Texto = "Nível de precisão é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ArquivoNaoEncontrado { get { return new Mensagem() { Texto = "Não foi encontrado o arquivo enviado na lista de arquivos do Projeto Geográfico.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Texto = "O Arquivo à ser importado deve ser enviado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem AreaDeAbrangenciaObrigatorio { get { return new Mensagem() { Texto = "Área de abrangência obrigatória. Clique no botão “Selecione” para escolher a área de trabalho.", Campo = "AreaAbrangencia_Container", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem MecanismoObrigatorio { get { return new Mensagem() { Texto = "Mecanismo de elaboração é obrigatório.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem AlterarMecanismo { get { return new Mensagem() { Texto = "Ao trocar de mecanismo, deverá ser reprocessado o arquivo, desta forma o sistema irá  habilitar os recursos de verificações de dependência e sobreposição da área total da propriedade.", Tipo = eTipoMensagem.Informacao }; } }

		#region Importador

		public Mensagem ArquivoAnexoNaoEhZip { get { return new Mensagem() { Texto = "O arquivo deve ser do formato zip.", Tipo = eTipoMensagem.Advertencia }; } }

		#endregion

		#region Sobreposicoes

		public Mensagem ATPNaoEncontrada { get { return new Mensagem() { Texto = "ATP não encontrada.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem VerificarSobreposicao { get { return new Mensagem() { Texto = "Para finalizar o projeto geográfico é necessário ter verificação a sobreposição nos últimos 30 minutos. Clique no botão Verificar no grupo Sobreposições da área total da propriedade - ATP.", Tipo = eTipoMensagem.Advertencia }; } }

		#endregion

		public Mensagem ConfirmarExcluir { get { return new Mensagem() { Texto = "Deseja realmente excluir o rascunho do projeto geográfico?" }; } }

		public Mensagem ConfirmarAreaAbrangencia { get { return new Mensagem() { Texto = "Essa ação irá apagar todos os arquivos processados. Tem certeza que deseja redefinir a área de abrangência?" }; } }

		public Mensagem ConfirmacaoReenviar { get { return new Mensagem() { Texto = "As informações processadas serão apagadas. Tem certeza que deseja reenviar?", Tipo = eTipoMensagem.Informacao }; } }

		public Mensagem ConfirmacaoFinalizar(List<string> lista, bool elaboracao = true)
		{
			string listaConcatenada = Mensagem.Concatenar(lista);
			return new Mensagem() { Texto = string.Format((elaboracao ? "Deseja realmente finalizá-lo?" : " Ao finalizar um projeto geográfico revertido irá invalidar os projetos geográficos e as caracterizações dependentes: {0}. Deseja realmente finalizá-lo?"), listaConcatenada), Tipo = eTipoMensagem.Informacao };
		}

		public Mensagem ConfirmacaoRefazer()
		{
			return new Mensagem() { Texto = "Ao refazer o projeto geográfico os arquivos processados serão apagados e o sistema irá carregar os arquivos utilizados na finalização. Deseja realmente refazer o projeto?", Tipo = eTipoMensagem.Informacao };
		}

		public Mensagem ConfirmacaoRecarregar()
		{
			return new Mensagem() { Texto = "Ao recarregar o projeto geográfico irá perder os arquivos enviados, Deseja realmente recarregar o projeto?", Tipo = eTipoMensagem.Informacao };
		}

		public Mensagem DominialidadeInexistente(string caracterizacao)
		{
			return new Mensagem() { Texto = string.Format("Para cadastrar o projeto geográfico - {0} é necessário que a caracterização de Dominialidade esteja cadastrada.", caracterizacao), Tipo = eTipoMensagem.Advertencia };
		}

		public Mensagem ProjetoGraficoInvalido(string caracterizacao)
		{
			return new Mensagem() { Texto = string.Format("O projeto geográfico da caracterização {0} foi alterado, alterando assim os dados de referencia deste projeto. Confira os dados lançado.", caracterizacao), Tipo = eTipoMensagem.Informacao };
		}

		public Mensagem EmpreendimentoForaAbrangencia { get { return new Mensagem() { Texto = "Empreendimento está fora da Área de abrangência. Altere a Área de abrangência ou altere a coordenada do empreendimento.", Tipo = eTipoMensagem.Advertencia }; } }

		public Mensagem NaoPodeVisualizarComDependenciasAlteradas(String caracterizacao)
		{
			return new Mensagem()
			{
				Tipo = eTipoMensagem.Advertencia,
				Texto = String.Format("Para visualizar os dados é necessário que o projeto geográfico da caracterização de {0} esteja válido.", caracterizacao)
			};
		}

		#region Credenciado

		public Mensagem CopiarProjetoGeoFinalizado { get { return new Mensagem() { Texto = "IDAF não possui projeto geográfico finalizado.", Tipo = eTipoMensagem.Advertencia }; } }
		public Mensagem EmpreendimentoForaAbrangenciaCopiar { get { return new Mensagem() { Texto = "Empreendimento está fora da Área de abrangência. Altere a coordenada do empreendimento.", Tipo = eTipoMensagem.Advertencia }; } }

		#endregion
	}
}