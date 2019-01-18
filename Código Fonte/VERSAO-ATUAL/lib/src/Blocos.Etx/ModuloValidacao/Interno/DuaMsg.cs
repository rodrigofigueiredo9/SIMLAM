

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static DuaMsg _DuaMsg = new DuaMsg();
		public static DuaMsg Dua
		{
			get { return _DuaMsg; }
		}
	}

	public class DuaMsg
	{
		public Mensagem Sucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "DUAs emitidos com sucesso." }; } }
		public Mensagem Falha { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Falha ao emitir DUAs." }; } }
		public Mensagem ExisteDuaTitulo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não é possível editar esse título, porque todos os DUAS para esse título já foram emitidos." }; } }
		public Mensagem SolicitacaoSituacaoNaoPodeEditar (string situacaoAtual) 
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Solicitacao_Situacao", Texto = String.Format("Não é possível editar a Solicitação de inscrição na situação \"{0}\".", situacaoAtual) }; 
		} 
	
	}
}