using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static GrupoQuimicoMsg _grupoQuimicoMsg = new GrupoQuimicoMsg();
		public static GrupoQuimicoMsg GrupoQuimico
		{
			get { return _grupoQuimicoMsg; }
		}
	}

	public class GrupoQuimicoMsg
	{
		public Mensagem Existente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Grupo químico já existente." }; } }
		public Mensagem GrupoQuimicoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Valor", Texto = "Grupo químico é obrigatório." }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Grupo químico salvo com sucesso." }; } }		
	}
}
