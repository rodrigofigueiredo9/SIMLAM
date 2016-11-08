

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{

	public partial class Mensagem
	{
		private static QueimaControladaMsg _queimaControlada = new QueimaControladaMsg();
		public static QueimaControladaMsg QueimaControlada
		{
			get { return _queimaControlada; }
			set { _queimaControlada = value; }
		}
	}

	public class QueimaControladaMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = @"Caracterização queima controlada salva com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = @"Caracterização queima controlada excluída com sucesso." }; } }

		public Mensagem TipoCultivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Queima_TipoCultivo", Texto = @"Tipo do cultivo da queima controlada é obrigatório." }; } }
		public Mensagem FinalidadeNomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Queima_FinalidadeNome", Texto = @"O nome da finalidade da queima controlada é obrigatório." }; } }
		public Mensagem AreaQueimaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Queima_AreaQueima", Texto = @"Área de queima é obrigatória." }; } }
		public Mensagem AreaQueimaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Queima_AreaQueima", Texto = @"Área de queima é inválida." }; } }
		public Mensagem AreaQueimaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Queima_AreaQueima", Texto = @"Área de queima deve ser maior do que zero." }; } }
		
		public Mensagem TipoCultivoQueimaDuplicado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Queima_TipoCultivo", Texto = @"Tipo de cultivo da queima já adicionado." }; } }
		public Mensagem FinalidadeNomeQueimaDuplicada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Queima_FinalidadeNome", Texto = @"Já existe uma mesma finalidade adicionada." }; } }

		public Mensagem EmpreendimentoRuralReservaIndefinida { get { return new Mensagem() { Texto = "O empreendimento está localizado em zona rural e na caracterização de Dominialidade possui Reserva Legal sem situação vegetal definida. Deseja realmente continuar?" }; } }
		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Deseja realmente excluir a caracterização de queima controlada deste empreendimento?" }; } }
		
		#region Area Requerida

		public Mensagem AreaRequeridaObrigatoria(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Queima_AreaRequerida{0}, #queima{0}", identificacao), Texto = @"Área requerida da queima controlada é obrigatória." };
		}

		public Mensagem AreaRequeridaInvalida(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Queima_AreaRequerida{0}, #queima{0}", identificacao), Texto = @"Área requerida da queima controlada está inválida." };
		}

		public Mensagem AreaRequiridaMaiorZero(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Queima_AreaRequerida{0}, #queima{0}", identificacao), Texto = @"Área requerida da queima controlada deve ser maior do que zero." };
		}

		#endregion

		public Mensagem TipoQueimaObrigatorio(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("Queima_QueimaTipo{0}, #queima{0}", identificacao), Texto = @"O tipo de queima é obrigatório." };
		}

		public Mensagem QueimaControladaCultivoObrigatoria(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("#queima{0}", identificacao), Texto = @"Pelo menos um cultivo da queima controlada deve ser adicionado." };
		}

	}
}
