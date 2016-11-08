using System;
namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static AtividadeEspecificidadeMsg _atividadeEspecificidadeMsg = new AtividadeEspecificidadeMsg();
		public static AtividadeEspecificidadeMsg AtividadeEspecificidade
		{
			get { return _atividadeEspecificidadeMsg; }
			set { _atividadeEspecificidadeMsg = value; }
		}
	}

	public class AtividadeEspecificidadeMsg
	{
		public Mensagem AtividadeJaAdicionada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AtividadeSolicitada", Texto = "Atividade já adicionada." }; } }
		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AtividadeSolicitada", Texto = "Atividade é obrigatória." }; } }

		public Mensagem AtividadeDesativada(string atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A atividade solicitada \"{0}\" está desativada.", atividade) };
		}
	}
}