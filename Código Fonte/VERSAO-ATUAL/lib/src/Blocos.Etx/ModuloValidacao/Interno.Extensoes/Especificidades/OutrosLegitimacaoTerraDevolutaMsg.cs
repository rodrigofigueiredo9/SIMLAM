

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static OutrosLegitimacaoTerraDevolutaMsg _outrosLegitimacaoTerraDevolutaMsg = new OutrosLegitimacaoTerraDevolutaMsg();
		public static OutrosLegitimacaoTerraDevolutaMsg OutrosLegitimacaoTerraDevolutaMsg
		{
			get { return _outrosLegitimacaoTerraDevolutaMsg; }
			set { _outrosLegitimacaoTerraDevolutaMsg = value; }
		}
	}

	public class OutrosLegitimacaoTerraDevolutaMsg
	{
		public Mensagem IsInalienabilidadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Outros_IsInalienabilidade", Texto = "Inalienabilidade é obrigatorio." }; } }
		public Mensagem ValorTerrenoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Outros_ValorTerreno", Texto = "Valor do terreno é obrigatorio." }; } }
		public Mensagem MunicipioGlebaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Outros_MunicipioGlebaId", Texto = "Município da Gleba é obrigatorio." }; } }
		public Mensagem DestinatarioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Outros_Destinatario", Texto = "Destinatário é obrigatorio." }; } }
		public Mensagem DestinatarioJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Outros_Destinatario", Texto = "Destinatário já foi adicionado." }; } }
		public Mensagem DominioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Outros_Dominio", Texto = "Posse é obrigatoria." }; } }
		public Mensagem DominioInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Outros_Dominio", Texto = "Atualize a especificidade, pois a posse selecionada não existe mais na caracterização de Regularização Fundiária." }; } }
		public Mensagem CaracterizacaoCadastrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterização de Regularização Fundiária deve estar cadastrada." }; } }

		public Mensagem CaracterizacaoValida(String caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Para cadastrar este modelo de título é necessário ter os dados da caracterização {0} válidos.", caracterizacao) };
		}

		public Mensagem AtividadeInvalida(String atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O modelo de título Legitimação de Terra Devoluta não pode ser utilizado para atividade {0}.", atividade) };
		}

		public Mensagem DestinatarioInvalido(String destinatario)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O destinatário selecionado não está mais associado ao {0}.", destinatario) };
		}

		public Mensagem PosseAssociadaNumero(String titulo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A posse selecionada não pode ser utilizada para este título, pois já esta associada ao título LTD - {0}.", titulo) };
		}

		public Mensagem PosseAssociadaSituacao(String situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A posse selecionada não pode ser utilizada, pois está associada a outro Título de Legitimação de Terra Devoluta na situação \"{0}\".", situacao) };
		}
	}
}