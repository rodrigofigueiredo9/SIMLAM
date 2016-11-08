

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static AtividadeMsg _atividadeMsg = new AtividadeMsg();

		public static AtividadeMsg Atividade
		{
			get { return _atividadeMsg; }
		}
	}

	public class AtividadeMsg
	{
		public Mensagem Indefirida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A atividade encontra-se indeferida." }; } }
		public Mensagem Encerrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A atividade encontra-se encerrada." }; } }
		public Mensagem EncerradaSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "A atividade foi encerrada com sucesso." }; } }
		public Mensagem ExcluirAssociadaTitulo { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O título não pode ser removido, pois já foi emitido." }; } }

		public Mensagem EncerrarSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Só é permitido encerrar atividade \"Em andamento\" ou \"Com Pendência\"." }; } }

		public Mensagem FinalidadeAssociadaTitulo(string modelo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A finalidade {0} não pode ser excluída pois está associada a um título.", modelo) };
		}
	}
}