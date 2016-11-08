

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static PapelMsg _papelMsg = new PapelMsg();

		public static PapelMsg Papel
		{
			get { return _papelMsg; }
		}
	}

	public class PapelMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Papel salvo com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Papel editado com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Papel excluído com sucesso." }; } }
		public Mensagem NomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Nome", Texto = "Nome é obrigatório." }; } }
		public Mensagem PermissaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Permissão é obrigatória." }; } }

		public Mensagem MensagemExcluir(string nome)
		{
			return new Mensagem() { Texto = String.Format("Tem certeza que deseja excluir o papel {0}?", nome) };
		}

		public Mensagem PossuiFuncionario(string funcionarios)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Papel não pode ser excluído pois esta associado ao(s) funcionario(s): {0}.", funcionarios) };
		}
	}
}
