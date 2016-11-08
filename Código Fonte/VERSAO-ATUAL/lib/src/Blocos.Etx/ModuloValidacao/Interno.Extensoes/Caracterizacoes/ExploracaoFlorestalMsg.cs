

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ExploracaoFlorestalMsg _exploracaoFlorestalMsg = new ExploracaoFlorestalMsg();
		public static ExploracaoFlorestalMsg ExploracaoFlorestal
		{
			get { return _exploracaoFlorestalMsg; }
			set { _exploracaoFlorestalMsg = value; }
		}
	}

	public class ExploracaoFlorestalMsg
	{
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização exploração florestal excluida com sucesso" }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Caracterização exploração florestal salva com sucesso" }; } }
		public Mensagem FinalidadeExploracaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "exploracaoFlorestalFinalidade", Texto = @"Finalidade da exploração florestal é obrigatório." }; } }
		public Mensagem FinalidadeExploracaoEspecificarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "FinalidadeEspecificar", Texto = @"Especificar da finalidade da exploração florestal é obrigatório." }; } }
		public Mensagem ProdutoTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Produto", Texto = @"Tipo do Produto é obrigatório." }; } }
		public Mensagem ProdutoDuplicado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Produto", Texto = @"Produto ja adicionado." }; } }
		public Mensagem QuantidadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Quantidade", Texto = @"Quantidade do Produto é obrigatória." }; } }
		public Mensagem QuantidadeMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Quantidade", Texto = @"Quantidade deve ser maior do que zero." }; } }
		public Mensagem QuantidadeInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Quantidade", Texto = @"Quantidade inválida." }; } }

		public Mensagem NaoPossuiAVNOuAA { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"Área da atividade deve sobrepor Área de Vegetação Nativa - AVN ou Área Alterada - AA." }; } }
		public Mensagem PossuiAVNNaoCaracterizada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = @"No projeto geográfico da Dominialidade possui Área de Floresta-Nativa com estágio não caracterizado." }; } }
		
		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização exploração florestal deste empreendimento?" }; } }
		public Mensagem EmpreendimentoRuralReservaIndefinida { get { return new Mensagem() { Texto = "O empreendimento está localizado em zona rural e na caracterização de Dominialidade possui Reserva Legal sem situação vegetal definida. Deseja realmente continuar?" }; } }

		public Mensagem AreaRequiridaObrigatoria(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("ExploracaoFlorestal_Exploracoes_AreaRequerida{0}, #exploracao{0}", identificacao), Texto = @"Área requerida da exploração florestal é obrigatória" };
		}

		public Mensagem AreaRequiridaInvalida(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("ExploracaoFlorestal_Exploracoes_AreaRequerida{0}, #exploracao{0}", identificacao), Texto = @"Área requerida da exploração florestal é inválida" };
		}

		public Mensagem AreaRequiridaMaiorZero(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("ExploracaoFlorestal_Exploracoes_AreaRequerida{0}, #exploracao{0}", identificacao), Texto = @"Área requerida da exploração florestal deve ser maior do que zero" };
		}

		public Mensagem ExploracaoTipoObrigatorio(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("ExploracaoFlorestal_ExploracaoTipo{0}, #exploracao{0}", identificacao), Texto = @"Tipo de exploração da exploração florestal é obrigatório" };
		}

		public Mensagem ClassificacaoVegetacaoObrigatoria(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("ExploracaoFlorestal_ClassificacaoVegetal{0}, #exploracao{0}", identificacao), Texto = @"Classificação da vegetação é obrigatória." };
		}

		public Mensagem ProdutoObrigatorio(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "exploracao" + identificacao, Texto = @"Produto é obrigatório" };
		}

		public Mensagem ArvoresRequeridasObrigatoria(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("ExploracaoFlorestal_Exploracoes_ArvoresRequeridas{0}, #exploracao{0}", identificacao), Texto = @"O número de árvores requeridas da exploração florestal é obrigatória" };
		}

		public Mensagem ArvoresRequeridasMaiorZero(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("ExploracaoFlorestal_Exploracoes_ArvoresRequeridas{0}, #exploracao{0}", identificacao), Texto = @"O número de árvores requeridas da exploração florestal deve ser maior do que zero" };
		}

		public Mensagem QdeArvoresRequeridasObrigatoria(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("ExploracaoFlorestal_Exploracoes_QuantidadeArvores{0}, #exploracao{0}", identificacao), Texto = @"Quantidade de arvores é obrigatória" };
		}

		public Mensagem QdeArvoresRequeridasMaiorZero(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("ExploracaoFlorestal_Exploracoes_QuantidadeArvores{0}, #exploracao{0}", identificacao), Texto = @"Quantidade de arvores deve ser maior do que zero" };
		}
	}
}