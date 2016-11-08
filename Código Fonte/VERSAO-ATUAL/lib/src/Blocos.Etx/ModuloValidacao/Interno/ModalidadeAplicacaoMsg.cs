using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static ModalidadeAplicacaoMsg _modalidadeAplicacaoMsg = new ModalidadeAplicacaoMsg();
		public static ModalidadeAplicacaoMsg ModalidadeAplicacao
		{
			get { return _modalidadeAplicacaoMsg; }
		}
	}

	public class ModalidadeAplicacaoMsg
	{
		public Mensagem Existente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Modalidade de aplicação já existente." }; } }
		public Mensagem ModalidadeAplicacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Valor", Texto = "Modalidade de aplicação é obrigatório." }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Modalidade de aplicação salva com sucesso." }; } }
	}
}
