

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static TermoCPFARLMsg _termoCPFARLMsg = new TermoCPFARLMsg();
		public static TermoCPFARLMsg TermoCPFARLMsg
		{
			get { return _termoCPFARLMsg; }
			set { _termoCPFARLMsg = value; }
		}
	}

	public class TermoCPFARLMsg
	{
		public Mensagem DestinatarioNaoPermitido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_Destinatario", Texto = "Os tipos de destinatário, Arrendatário e Representante legal, não podem ser adicionados para este modelo de título." }; } }
		public Mensagem DestinatarioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Destinatário é obrigatorio." }; } }
		public Mensagem DestinatarioJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Destinatário já foi adicionado." }; } }
		public Mensagem DominialidadeInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterizacao Dominialidade deve estar cadastrada." }; } }
		public Mensagem ArlInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este modelo de título só poderá ser emitido se pelo menos uma reserva legal da dominialidade estiver com situação vegetal caracterizada." }; } }
		public Mensagem ARLSituacaoVegetalInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para cadastrar este modelo de título é necessário que reserva legal na dominialidade esteja preenchida e com situação vegetal igual a Preservada e/ou Em recuperação." }; } }
		public Mensagem DominioSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = " Este modelo de título só poderá ser emitido para reserva legal da dominialidade na situação de \"Proposta\"." }; } }
		public Mensagem DominioSituacaoNaoCaracterizada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este modelo de título só poderá ser emitido se pelo menos uma reserva legal da dominialidade estiver com situação vegetal caracterizada." }; } }
		public Mensagem LocalizacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este modelo de título não poderá ser emitido quando o campo da localização da reserva legal na dominialidade está informando \"compensação em outro empreendimento (cedente)\" ou \"compensação em outro empreendimento (receptora)\"." }; } }

		public Mensagem CaracterizacaoDeveEstarValida(String caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Para cadastrar este modelo de título é necessário que a caracterização {0} esteja válida.", caracterizacao) };
		}

		public Mensagem AtividadeInvalida(String atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Este modelo de título não está configurado para a atividade {0}.", atividade) };
		}
	}
}