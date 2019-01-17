

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static OutrosInformacaoCorteMsg _outrosInformacaoCorte = new OutrosInformacaoCorteMsg();
		public static OutrosInformacaoCorteMsg OutrosInformacaoCorte
		{
			get { return _outrosInformacaoCorte; }
			set { _outrosInformacaoCorte = value; }
		}
	}

	public class OutrosInformacaoCorteMsg
	{
		public Mensagem InformacaoCorteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Outros_InformacaoCortes", Texto = "Informação de corte é obrigatória." }; } }
		public Mensagem InformacaoCorteInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Outros_InformacaoCortes", Texto = "A informação de corte deve estar cadastrada." }; } }
		public Mensagem InformacaoCorteAssociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Outros_InformacaoCorte", Texto = "Já existe um título para a informação de corte selecionada." }; } }
		public Mensagem CaracterizacaoCadastrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterização de informação de corte deve estar cadastrada." }; } }

		public Mensagem CaracterizacaoValida(String caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Para cadastrar este modelo de título é necessário ter os dados da caracterização {0} válidos.", caracterizacao) };
		}

		public Mensagem AtividadeInvalida(String atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O modelo de título informação de corte não pode ser utilizado para atividade {0}.", atividade) };
		}

		public Mensagem ValidadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Validade obrigatória." }; } }
		public Mensagem ValidadeIntervalo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A validade precisa ser de 20 dias ou até 180 dias." }; } }
		public Mensagem ExisteDuaTitulo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível editar esse título, porque todos os DUAS para esse título já foram emitidos." }; } }
	}
}