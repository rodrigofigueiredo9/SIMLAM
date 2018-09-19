

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static LaudoVistoriaFlorestalMsg _laudoVistoriaFlorestalMsg = new LaudoVistoriaFlorestalMsg();
		public static LaudoVistoriaFlorestalMsg LaudoVistoriaFlorestalMsg
		{
			get { return _laudoVistoriaFlorestalMsg; }
			set { _laudoVistoriaFlorestalMsg = value; }
		}
	}

	public class LaudoVistoriaFlorestalMsg
	{
		public Mensagem CaracterizacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Caracterizacao", Texto = "A Caracterização é obrigatória." }; } }
		public Mensagem CaracterizacaoCadastrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Caracterizacao", Texto = "A Caracterização selecionada deve estar cadastrada." }; } }
		public Mensagem ObjetivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Objetivo", Texto = "Objetivo é obrigatório." }; } }
		public Mensagem ConsideracoesObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Consideracao", Texto = "Considerações é obrigatório." }; } }
		public Mensagem ParecerTecnicoDescricaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_ParecerDescricao", Texto = "Descrição do Parecer técnico é obrigatório." }; } }
		public Mensagem ConclusaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Conclusao", Texto = "Conclusão é obrigatória." }; } }
		public Mensagem CaracterizacaoDuplicada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Caracterizacao", Texto = "A caraterização selecionada já foi adicionada ao título." }; } }

		public Mensagem CaracterizacaoInvalida(string caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Caracterizacao", Texto = String.Format("Para cadastrar este modelo de título é necessário ter os dados da caracterização {0} válido.", caracterizacao) };
		}
	}
}