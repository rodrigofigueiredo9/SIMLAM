using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ClasseUsoMsg _classeUsoMsg = new ClasseUsoMsg();
		public static ClasseUsoMsg ClasseUso
		{
			get { return _classeUsoMsg; }
		}
	}

	public class ClasseUsoMsg
	{
		public Mensagem Existente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Classe de uso já existente." }; } }
		public Mensagem ClasseUsoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Valor", Texto = "Classe de uso é obrigatório." }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Classe de uso salva com sucesso." }; } }
	}
}
