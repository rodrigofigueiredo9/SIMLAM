namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static EntregaMsg _entregaMsg = new EntregaMsg();

		public static EntregaMsg Entrega
		{
			get { return _entregaMsg; }
		}
	}

	public class EntregaMsg
	{
		public Mensagem Cadastrar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Entrega cadastrada com sucesso." }; } }
		public Mensagem ItemJaAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este título já esta associado." }; } }
		public Mensagem TituloEntregue { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Este título já foi entregue." }; } }
		public Mensagem TituloSituacao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O título para ser associado deve estar na situação ‘Prorrogado’ ou ‘Concluído’." }; } }
		public Mensagem ItemAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Título associado com sucesso." }; } }

		#region Validações

		public Mensagem ProtocoloNaoExiste { get { return new Mensagem() { Campo = "Entrega_ProcessoNumero", Tipo = eTipoMensagem.Advertencia, Texto = "Nº de registro não está cadastrado no sistema." }; } }
		public Mensagem ProcessoNumeroInvalido { get { return new Mensagem() { Campo = "Entrega_ProcessoNumero", Tipo = eTipoMensagem.Advertencia, Texto = "Nº de registro formato inválido." }; } }
		
		public Mensagem ProcessoNumeroObrigatorio { get { return new Mensagem() { Campo = "Entrega_ProcessoNumero", Tipo = eTipoMensagem.Advertencia, Texto = "Nº de registro é obrigatório." }; } }

		public Mensagem PessoaNaoCadastrada { get { return new Mensagem() { Campo = "Entrega_Nome", Tipo = eTipoMensagem.Informacao, Texto = "Pessoa não cadastrada no sistema. Favor informar o nome." }; } }

		public Mensagem NenhumTituloEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não existem títulos a serem entregues para o processo/documento." }; } }

		public Mensagem ProcessoApenso(string numero)
		{
			return new Mensagem() { Campo = "Entrega_ProcessoNumero", Tipo = eTipoMensagem.Advertencia, Texto = string.Format("O processo não pode ter registro de entrega, pois está apensado ao processo  {0}.", numero) };
		}

		public Mensagem DocumentoJuntado(string numero)
		{
			return new Mensagem() { Campo = "Entrega_ProcessoNumero", Tipo = eTipoMensagem.Advertencia, Texto = string.Format("O documento não pode ter registro de entrega, pois está juntado ao processo {0}.", numero) };
		}

		public Mensagem DataEntregaObrigatria { get { return new Mensagem() { Campo = "Entrega_DataEntrega", Tipo = eTipoMensagem.Advertencia, Texto = "Data de entrega é obrigatória." }; } }
		public Mensagem DataEntregaInvalida{ get { return new Mensagem() { Campo = "Entrega_DataEntrega", Tipo = eTipoMensagem.Advertencia, Texto = "Data de entrega é inválida." }; } }
		public Mensagem DataMaiorDataAtual { get { return new Mensagem() { Campo = "Entrega_DataEntrega", Tipo = eTipoMensagem.Advertencia, Texto = "Data de entrega deve ser menor ou igual a data atual." }; } }
		
		public Mensagem TituloObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Título é obrigatório." }; } }
		public Mensagem PessoaObrigatorio { get { return new Mensagem() { Campo="Entrega_PessoaId", Tipo = eTipoMensagem.Advertencia, Texto = "Pessoa é obrigatória." }; } }

		public Mensagem NomeObrigatorio { get { return new Mensagem() { Campo = "Entrega_Nome", Tipo = eTipoMensagem.Advertencia, Texto = "Nome é obrigatório." }; } }
		public Mensagem CPFObrigatorio { get { return new Mensagem() { Campo = "Entrega_CPF", Tipo = eTipoMensagem.Advertencia, Texto = "CPF é obrigatório." }; } }

		#endregion

		#region Mensagem Modal

		public Mensagem ConfirmModal(string numeros)
		{
			int titulos = numeros.Split(',').Length + numeros.Split('e').Length;

			if (titulos > 2)
			{
				return new Mensagem() { Campo = "Entrega_ProcessoNumero", Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Ao confirmar esta entrega, os títulos {0} serão concluídos. Deseja confirmar a entrega deles?", numeros) };
			}
			else
			{
				return new Mensagem() { Campo = "Entrega_ProcessoNumero", Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Ao confirmar esta entrega, o título {0} será concluído. Deseja confirmar a entrega dele?", numeros) };
			}
		}

		#endregion
	}
}
