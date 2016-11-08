

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CondicionanteMsg _condicionanteMsg = new CondicionanteMsg();
		public static CondicionanteMsg Condicionante
		{
			get { return _condicionanteMsg; }
			set { _condicionanteMsg = value; }
		}
	}

	public class CondicionanteMsg
	{
		public Mensagem DataCriacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Data de criação da condicionante é obrigatória." }; } }
		public Mensagem DataCriacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Data de criação da condicionante inválida." }; } }

		public Mensagem DataVencimentoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Data de vencimento da condicionante é obrigatória." }; } }
		public Mensagem DataVencimentoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Data de vencimento da condicionante inválida." }; } }

		public Mensagem PrazoValorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Condicionante_Prazo", Texto = "Valor do prazo da condicionante é obrigatório." }; } }
		public Mensagem PeriodicidadeValorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Condicionante_PeriodicidadeValor", Texto = "Valor da periodicidade da condicionante é obrigatório." }; } }
		public Mensagem PeriodicidadeTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Condicionante_PeriodicidadeTipo_Id", Texto = "Tipo de periodicidade da condicionante é obrigatório." }; } }

		public Mensagem DescricaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Condicionante_Descricao, #Texto", Texto = "Descrição da condicionante é obrigatória." }; } }
		public Mensagem DescricaoInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Condicionante_Descricao", Texto = "Descrição da condicionante não existe." }; } }

		public Mensagem DescricaoMuitoGrande(int tamanho)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Condicionante_Descricao", Texto = String.Format("Descrição da condicionante não deve exceder {0} caracteres.", tamanho) };
		}

		public Mensagem AtenderNaoAtiva(string situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Não é possível atender condicionante na situação {0}.", situacao) };
		}

		public Mensagem ProrrogarDiasObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Dias", Texto = "Dias de prorrogação da condicionante é obrigatório." }; } }
		public Mensagem ProrrogarCondicionanteInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Condicionante inválida." }; } }
		public Mensagem ProrrogarAtentidaEncerrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Condicionante atentida ou encerrada não pode ser prorrogada." }; } }
		public Mensagem ProrrogarNaoPossuiPrazo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Condicionante não pode ser prorrogada pois não possui prazo." }; } }

		public Mensagem AtenderEncerrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Condicionante encerrada não pode ser atentida." }; } }

		public Mensagem Inexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Condicionante não existe." }; } }
		public Mensagem NaoAtiva { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Condicionante não ativada." }; } }
		public Mensagem Invalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Condicionante inválida." }; } }
		public Mensagem JaAtendida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A condicionante já está atendida." }; } }
	}
}