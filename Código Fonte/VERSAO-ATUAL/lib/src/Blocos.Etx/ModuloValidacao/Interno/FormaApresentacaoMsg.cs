using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static FormaApresentacaoMsg _formaApresentacaoMsg = new FormaApresentacaoMsg();
		public static FormaApresentacaoMsg FormaApresentacao
		{
			get { return _formaApresentacaoMsg; }
		}
	}

	public class FormaApresentacaoMsg
	{
		public Mensagem Existente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Forma de apresentação já existente." }; } }
		public Mensagem FormaApresentacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Valor", Texto = "Forma de apresentação é obrigatório." }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Forma de apresentação salva com sucesso." }; } }
	}
}
