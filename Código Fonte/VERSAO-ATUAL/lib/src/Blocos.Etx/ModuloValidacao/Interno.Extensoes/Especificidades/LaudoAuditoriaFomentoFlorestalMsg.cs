

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static LaudoAuditoriaFomentoFlorestalMsg _laudoAuditoriaFomentoFlorestalMsg = new LaudoAuditoriaFomentoFlorestalMsg();
		public static LaudoAuditoriaFomentoFlorestalMsg LaudoAuditoriaFomentoFlorestal
		{
			get { return _laudoAuditoriaFomentoFlorestalMsg; }
			set { _laudoAuditoriaFomentoFlorestalMsg = value; }
		}
	}

	public class LaudoAuditoriaFomentoFlorestalMsg
	{
		public Mensagem ObjetivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Objetivo", Texto = "Objetivo é obrigatório." }; } }

		public Mensagem PlantioAPPObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PlantioAPP, #fsConstatacoes", Texto = "Plantio em APP é obrigatório." }; } }
		public Mensagem PlantioAPPAreaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PlantioAPPArea", Texto = "Área do plantio em APP é obrigatória." }; } }
		public Mensagem PlantioAPPAreaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PlantioAPPArea", Texto = "Área do plantio em APP é inválida." }; } }
		public Mensagem PlantioAPPAreaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PlantioAPPArea", Texto = "Área do plantio em APP deve ser maior do que zero." }; } }

		public Mensagem PlantioMudasEspeciesFlorestNativasObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PlantioMudasEspeciesFlorestNativas, #fsConstatacoes", Texto = "Plantio de mudas de espécies nativas é obrigatório." }; } }
		public Mensagem PlantioMudasEspeciesFlorestNativasQtdObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PlantioMudasEspeciesFlorestNativasQtd", Texto = "Quantidade do plantio de mudas de espécies nativas é obrigatória." }; } }
		public Mensagem PlantioMudasEspeciesFlorestNativasQtdInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PlantioMudasEspeciesFlorestNativasQtd", Texto = "Quantidade do plantio de mudas de espécies nativas é inválida." }; } }
		public Mensagem PlantioMudasEspeciesFlorestNativasQtdMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PlantioMudasEspeciesFlorestNativasQtd", Texto = "Quantidade do plantio de mudas de espécies nativas deve ser maior do que zero." }; } }
		public Mensagem PlantioMudasEspeciesFlorestNativasAreaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PlantioMudasEspeciesFlorestNativasArea", Texto = "Área do plantio de mudas de espécies nativas é obrigatória." }; } }
		public Mensagem PlantioMudasEspeciesFlorestNativasAreaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PlantioMudasEspeciesFlorestNativasArea", Texto = "Área do plantio de mudas de espécies nativas é inválida." }; } }
		public Mensagem PlantioMudasEspeciesFlorestNativasAreaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PlantioMudasEspeciesFlorestNativasArea", Texto = "Área do plantio de mudas de espécies nativas deve ser maior do que zero." }; } }

		public Mensagem PreparoSoloObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PreparoSolo, #fsConstatacoes", Texto = "Preparo do solo é obrigatório." }; } }
		public Mensagem PreparoSoloAreaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PreparoSoloArea", Texto = "Área do preparo do solo é obrigatória." }; } }
		public Mensagem PreparoSoloAreaInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PreparoSoloArea", Texto = "Área do preparo do solo é inválida." }; } }
		public Mensagem PreparoSoloAreaMaiorZero { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_PreparoSoloArea", Texto = "Área do preparo do solo deve ser maior do que zero." }; } }

		public Mensagem ResultadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Resultados", Texto = "Resultado é obrigatório." }; } }
		public Mensagem ResultadoQuaisObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_ResultadoQuais", Texto = "Qual(is) é obrigatório." }; } }

		public Mensagem CaracterizacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Caracterizacao", Texto = "A Caracterização é obrigatória." }; } }
		public Mensagem CaracterizacaoCadastrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Caracterizacao", Texto = "A Caracterização de implantação da atividade de silvicultura deve estar cadastrada." }; } }

		public Mensagem CaracterizacaoInvalida(string caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Caracterizacao", Texto = String.Format("Para cadastrar este modelo de título é necessário ter os dados da caracterização {0} válido.", caracterizacao) };
		}

		public Mensagem AtividadeInvalida(string atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O modelo de título Laudo de Auditoria de Fomento Florestal não pode ser utilizado para atividade {0}", atividade) };
		}
	}
}
