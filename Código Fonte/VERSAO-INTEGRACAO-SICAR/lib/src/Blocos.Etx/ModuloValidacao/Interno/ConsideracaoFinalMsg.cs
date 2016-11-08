namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ConsideracaoFinalMsg _consideracaoFinalMsg = new ConsideracaoFinalMsg();
		public static ConsideracaoFinalMsg ConsideracaoFinalMsg
		{
			get { return _consideracaoFinalMsg; }
		}
	}

	public class ConsideracaoFinalMsg
	{
		public Mensagem JustificarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ConsideracaoFinal_Justificar", Texto = "Justificar o valor da penalidade pecuniária atribuída, levando-se em consideração os parâmetros legais é obrigatório." }; } }
		public Mensagem DescreverObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ConsideracaoFinal_Descrever", Texto = "Descrever outras informações que julgar relevante para um maior detalhamento e esclarecimento do processo é obrigatório." }; } }
		public Mensagem OpinarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rblOpinar", Texto = "Há necessidade de reparação do dano ambiental é obrigatório." }; } }
		public Mensagem OpinarReparacaoOpinarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ConsideracaoFinal_Reparacao", Texto = "Opinar quanto à forma de reparação é obrigatório." }; } }
		public Mensagem OpinarReparacaoJustificarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ConsideracaoFinal_Reparacao", Texto = "Justificar é obrigatório." }; } }

		public Mensagem SelecionarTipoTestemunhaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#Span{0}", Texto = "Funcionário do IDAF é obrigatório." }; } }
		public Mensagem TestemunhaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#Testemunha_TestemunhaId{0}", Texto = "Testemunha é obrigatório." }; } }
		public Mensagem TestemunhaNomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#Testemunha_TestemunhaNome{0}", Texto = "Nome Testemunha é obrigatório." }; } }
		public Mensagem TestemunhaEnderecoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#Testemunha_TestemunhaEndereco{0}", Texto = "Endereço é obrigatório." }; } }
		public Mensagem SetorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "#Testemunha_TestemunhaSetorId{0}", Texto = "Setor é obrigatório." }; } }

		public Mensagem AssinanteSetorObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Setor é obrigatorio." }; } }
		public Mensagem AssinanteFuncionarioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Funcionário é obrigatorio." }; } }
		public Mensagem AssinanteCargoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Cargo é obrigatorio." }; } }
		public Mensagem AssinanteObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fdsAssinante", Texto = "Assinante é obrigatório." }; } }
		public Mensagem AssinanteFuncionarioLogado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fdsAssinante", Texto = "O autor do cadastro deverá ser associado como assinante." }; } }
		public Mensagem AssinanteFuncionarioUnico { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fdsAssinante", Texto = "O autor do cadastro deverá ser associado como assinante." }; } }
		public Mensagem AssinanteJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Assinante ja adicionado." }; } }

		public Mensagem FirmarCompromissoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "spanRblTermo", Texto = "Firmou termo de compromisso para reparação do dano de acordo com a forma sugerida é obrigatório." }; } }
		public Mensagem JustificarFirmarCompromissoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ConsideracaoFinal_TermoCompromissoJustificar", Texto = "Justificar é obrigatório." }; } }

		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fileTermo", Texto = "Arquivo é obrigatório." }; } }
		public Mensagem ArquivoNaoEhPdf { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fileTermo", Texto = "Arquivo não é do tipo pdf" }; } }
	}
}
