using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ProfissaoMsg _profissaoMsg = new ProfissaoMsg();
		public static ProfissaoMsg Profissao
		{
			get { return _profissaoMsg; }
		}
	}

	public class ProfissaoMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Profissão salva com sucesso." }; } }		
		public Mensagem EditarSomenteAdministrador { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "É permitido editar somente as profissões cadastradas pelo perfil de administrador do sistema." }; } }				
		public Mensagem Existente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Profissão já está cadastrada." }; } }
		public Mensagem ProfissaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Texto", Texto = "Profissão é obrigatório." }; } }
	}
}