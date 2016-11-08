

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static LaudoVistoriaFomentoFlorestalMsg _laudoVistoriaFomentoFlorestalMsg = new LaudoVistoriaFomentoFlorestalMsg();
		public static LaudoVistoriaFomentoFlorestalMsg LaudoVistoriaFomentoFlorestal
		{
			get { return _laudoVistoriaFomentoFlorestalMsg; }
			set { _laudoVistoriaFomentoFlorestalMsg = value; }
		}
	}

	public class LaudoVistoriaFomentoFlorestalMsg
	{
		public Mensagem ObjetivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Objetivo", Texto = "Objetivo é obrigatório." }; } }
		public Mensagem ConsideracoesObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Consideracoes", Texto = "Considerações é obrigatório." }; } }
		public Mensagem ParecerTecnicoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_DescricaoParecer", Texto = "Descrição do parecer é obrigatório." }; } }
		public Mensagem ConclusaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Conclusao", Texto = "Conclusão é obrigatória." }; } }
		public Mensagem RestricoesObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Restricoes", Texto = "Restrições é obrigatório." }; } }
		public Mensagem ObservacoesObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Observacoes", Texto = "Observações é obrigatório." }; } }

		public Mensagem CaracterizacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Caracterizacao", Texto = "A Caracterização é obrigatória." }; } }
		public Mensagem CaracterizacaoCadastrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Caracterizacao", Texto = "A Caracterização de implantação da atividade de silvicultura deve estar cadastrada." }; } }

		public Mensagem CaracterizacaoInvalida(string caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Caracterizacao", Texto = String.Format("Para cadastrar este modelo de título é necessário ter os dados da caracterização {0} válido.", caracterizacao) };
		}

		public Mensagem AtividadeInvalida(string atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O modelo de título Laudo de Vistoria de Fomento Florestal não pode ser utilizado para atividade {0}", atividade) };
		}
	}
}

