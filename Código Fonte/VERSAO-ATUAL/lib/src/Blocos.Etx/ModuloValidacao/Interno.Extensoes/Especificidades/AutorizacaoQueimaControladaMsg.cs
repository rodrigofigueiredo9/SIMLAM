

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static AutorizacaoQueimaControladaMsg _autorizacaoQueimaControladaMsg = new AutorizacaoQueimaControladaMsg();
		public static AutorizacaoQueimaControladaMsg AutorizacaoQueimaControlada
		{
			get { return _autorizacaoQueimaControladaMsg; }
			set { _autorizacaoQueimaControladaMsg = value; }
		}
	}

	public class AutorizacaoQueimaControladaMsg
	{
		public Mensagem DominialidadeInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterizacao Dominialidade deve estar cadastrada." }; } }
		public Mensagem QueimaInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterizacao Queima Controlada deve estar cadastrada." }; } }

		public Mensagem LaudoVistoriaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Autorizacao_LaudoVistoriaFlorestalTexto", Texto = "O Laudo de Vistoria de Queima Controlada é obrigatório" }; } }
		public Mensagem LaudoVIstoriaDeveEstarConcluiddo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Autorizacao_LaudoVistoriaFlorestalTexto", Texto = "O Laudo de Vistoria deve estar com a situação concluido" }; } }

		public Mensagem CaracterizacaoDeveEstarValida(String caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Para cadastrar este modelo de título é necessário que a caracterização {0} esteja válida.", caracterizacao) };
		}
	}
}
