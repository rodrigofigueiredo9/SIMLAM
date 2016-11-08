

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ResponsavelTecnicoMsg _responsavelTecnicoMsg = new ResponsavelTecnicoMsg();
		public static ResponsavelTecnicoMsg ResponsavelTecnico
		{
			get { return _responsavelTecnicoMsg; }
		}
	}

	public class ResponsavelTecnicoMsg
	{
		public Mensagem ResponsavelObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Já existe responsável técnico sem preencher." }; } }
		public Mensagem ResponsavelExcluir { get { return new Mensagem() { Texto = "Tem certeza que deseja remover o responsável #texto do requerimento?" }; } }
		public Mensagem ResponsaveljaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Responsável Tecnico já adicionado." }; } }

		public Mensagem ResponsavelNaoPossuiProfissao(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Profissão é obrigatório para o responsável técnico {0}.", nome) };
		}
	}
}
