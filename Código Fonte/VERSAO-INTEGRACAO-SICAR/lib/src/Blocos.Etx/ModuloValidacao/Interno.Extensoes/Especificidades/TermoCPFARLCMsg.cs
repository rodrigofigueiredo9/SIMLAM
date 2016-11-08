using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static TermoCPFARLCMsg _termoCPFARLCMsg = new TermoCPFARLCMsg();
		public static TermoCPFARLCMsg TermoCPFARLC
		{
			get { return _termoCPFARLCMsg; }
			set { _termoCPFARLCMsg = value; }
		}
	}

	public class TermoCPFARLCMsg
	{
		public Mensagem CedenteEmpreendimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É necessário que o protocolo informado possua um empreendimento associado." }; } }
		public Mensagem CedenteDominioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CedenteDominio", Texto = "Matrícula/Posse do cedente é obrigatória." }; } }
		public Mensagem CedenteARLCompensacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CedenteARLCompensacao", Texto = "ARL para compensação cedente é obrigatória." }; } }
		public Mensagem CedenteARLCompensacaoDuplicada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CedenteARLCompensacao", Texto = "ARL para compensação cedente já adicionada." }; } }
		public Mensagem ReceptorEmpreendimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ReceptorEmpreendimento", Texto = "O empreendimento receptor é obrigatório." }; } }
		public Mensagem ReceptorDominioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ReceptorDominio", Texto = "A Matrícula/Posse receptora é obrigatória." }; } }
		public Mensagem ARLEmpreendmentoReceptorDiferente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CedenteARLCompensacao", Texto = "ARL para compensação cedente possui empreendimeto receptor diferente de outra já adicionado." }; } }


		public Mensagem ARLCedenteTipoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este modelo de título só poderá ser emitido para reserva legal compensada." }; } }
		public Mensagem ARLCedenteSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este modelo de título só poderá ser emitido para reserva legal na situação de \"Proposta\"." }; } }
		public Mensagem ARLCedenteSituacaoVegetalInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Para cadastrar este modelo de título é necessário que a reserva legal na Caracterização de Dominialidade com situação vegetal igual a \"Preservada\" ou \"Em recuperação\"." }; } }


		public Mensagem ResponsavelEmpreendimentoObrigatorio(string campo, string tipo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo + "ResponsaveisEmpreendimento", Texto = String.Format("Responsável do empreendimento {0} é obrigatório.", tipo) };
		}
		public Mensagem ResponsavelEmpreendimentoDuplicado(string campo, string tipo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo + "ResponsaveisEmpreendimento", Texto = String.Format("Responsável do empreendimento {0} já adicionado.", tipo) };
		}

		public Mensagem AtividadeInvalida(String atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Este modelo de título não está configurado para a atividade {0}.", atividade) };
		}

		public Mensagem DominialidadeInexistente(string empTipo, string caracterizacaoTipo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A caracterização de {0} do empreendimento {1} deve estar cadastrada.", caracterizacaoTipo, empTipo) };
		}

		public Mensagem CaracterizacaoDeveEstarValida(string empTipo, string caracterizacaoTipo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Para cadastrar este modelo de título é necessário ter os dados da caracterização {0} válidos do empreendimento {1}.", caracterizacaoTipo, empTipo) };
		}

		public Mensagem DominioDessassociado(string tipo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A Matrícula/Posse do empreendimento {0} não está mais associada na Caracterização de Dominialidade do empreendimento.", tipo) };
		}

		public Mensagem ReservaLegalDessassociadoCedente(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A reserva legal {0} não está mais associada à Matrícula/Posse do empreendimento cedente. Verifique o projeto geográfico da Caracterização de Dominialidade do empreendimento.", identificacao) };
		}

		public Mensagem ReservaLegalDessassociadoReceptor(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A reserva legal cedente {0} não possui vinculo de compensação com o empreendimento receptor. É necessário que a reserva legal cedente esteja associada ao empreendimento receptor selecionado.", identificacao) };
		}

		public Mensagem ReservaLegalDessassociadoReceptorDominio(string identificacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A reserva legal cedente {0} não possui vinculo de compensação com a matrícula ou posse do empreendimento receptor. É necessário que a reserva legal cedente esteja associada á matrícula ou posse receptora informada.", identificacao) };
		}

		public Mensagem ResponsavelDessassociado(string empTipo, string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O responsável do empreendimento {0} \"{1}\" não está mais associado ao empreendimento.", empTipo, nome) };
		}

		public Mensagem DominioPossuiTituloCadastrado(string situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CedenteDominio, #ReceptorDominio", Texto = String.Format("A Matrícula/Posse do empreendimento receptor já está associada a outro Título de \"Termo de Compromisso de Preservação e/ou Formação de Área de Reserva Legal por Compensação\" na situação \"{0}\" para a mesma matrícula ou posse do empreendimento cedente.", situacao) };
		}

		public Mensagem DominioPossuiTituloConcluido(string sigla, string numero, string situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CedenteDominio, #ReceptorDominio", Texto = String.Format("A Matrícula/Posse do empreendimento receptor já está associada ao Título de {0} - {1} na situação {\"2}\" para o mesmo empreendimento cedente.", sigla, numero, situacao) };
		}
	}
}