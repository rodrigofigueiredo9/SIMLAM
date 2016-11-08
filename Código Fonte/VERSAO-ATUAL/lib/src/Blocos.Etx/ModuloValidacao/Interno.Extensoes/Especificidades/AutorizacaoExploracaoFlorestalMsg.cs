

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static AutorizacaoExploracaoFlorestalMsg _autorizacaoExploracaoFlorestalMsg = new AutorizacaoExploracaoFlorestalMsg();
		public static AutorizacaoExploracaoFlorestalMsg AutorizacaoExploracaoFlorestal
		{
			get { return _autorizacaoExploracaoFlorestalMsg; }
			set { _autorizacaoExploracaoFlorestalMsg = value; }
		}
	}

	public class AutorizacaoExploracaoFlorestalMsg
	{		
		public Mensagem ObservacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ExploracaoFlorestal_Observacoes", Texto = "Observação é obrigatório." }; } }
		public Mensagem ObservacaoMuitoGrande { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ExploracaoFlorestal_Observacoes", Texto = "Observação deve ter no máximo 500 caracteres." }; } }

		public Mensagem DominialidadeInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterizacao Dominialidade deve estar cadastrada." }; } }
		public Mensagem ExploracaoInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterizacao Exploração Florestal deve estar cadastrada." }; } }

		public Mensagem LaudoVistoriaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Autorizacao_LaudoVistoriaFlorestalTexto", Texto = "O Laudo de Vistoria Florestal é obrigatório" }; } }
		public Mensagem LaudoVistoriaModelo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Autorizacao_LaudoVistoriaFlorestalTexto", Texto = "O Modelo de título associado deve ser Laudo de Vistoria Florestal" }; } }
		public Mensagem LaudoVIstoriaDeveEstarConcluiddo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Autorizacao_LaudoVistoriaFlorestalTexto", Texto = "O Laudo de Vistoria deve estar com a situação concluido" }; } }

		public Mensagem CaracterizacaoDeveEstarValida (String caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Para cadastrar este modelo de título é necessário que a caracterização {0} esteja válida.", caracterizacao) };
		}

	}
}