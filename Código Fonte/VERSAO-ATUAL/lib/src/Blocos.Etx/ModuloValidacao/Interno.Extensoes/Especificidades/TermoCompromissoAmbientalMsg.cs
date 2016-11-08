
namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static TermoCompromissoAmbientalMsg _termoCompromissoAmbientalMsg = new TermoCompromissoAmbientalMsg();
		public static TermoCompromissoAmbientalMsg TermoCompromissoAmbientalMsg
		{
			get { return _termoCompromissoAmbientalMsg; }
			set { _termoCompromissoAmbientalMsg = value; }
		}
	}

	public class TermoCompromissoAmbientalMsg
	{
		public Mensagem EmpreendimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É necessário que o protocolo informado possua um empreendimento associado." }; } }

		public Mensagem TituloModeloObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo="Termo_LicencaNumero", Texto = "O Modelo de Título \"Licença Ambiental de Regularização\" é obrigatório." }; } }
		public Mensagem TituloModeloInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O Modelo de Título associado deve ser \"Licença Ambiental de Regularização\"." }; } }
		public Mensagem TituloSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O Título de Licença Ambiental de Regularização deve estar com situação igual a \"Concluído\" ou \"Prorrogado\"." }; } }

		public Mensagem RepresentanteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo="Termo_Representante", Texto = "Representante é obrigatório." }; } }
		public Mensagem RepresentanteDesassociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_Representante", Texto = "O representante legal selecionado não está mais associado ao destinatário." }; } }

		public Mensagem DescricaoCompromissoAmbientalObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_Descricao", Texto = "Descrição do compromisso ambiental é obrigatório." }; } }
		public Mensagem DescricaoCompromissoAmbientalMaxLength { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Termo_Descricao", Texto = "Descrição do compromisso ambiental não pode conter mais de 5000 caracteres." }; } }

		public Mensagem AtividadeJaAssociada(string numeroModelo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = string.Format("A atividade não pode ser selecionada, pois já está associado ao título {0}.", numeroModelo) };
		}
		
	}
}

