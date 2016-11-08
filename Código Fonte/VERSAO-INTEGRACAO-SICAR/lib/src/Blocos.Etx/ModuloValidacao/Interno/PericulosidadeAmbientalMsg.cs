using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static PericulosidadeAmbientalMsg _periculosidadeAmbientalMsg = new PericulosidadeAmbientalMsg();
		public static PericulosidadeAmbientalMsg PericulosidadeAmbiental
		{
			get { return _periculosidadeAmbientalMsg; }
		}
	}

	public class PericulosidadeAmbientalMsg
	{
		public Mensagem Existente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Periculosidade ambiental já existente." }; } }
		public Mensagem PericulosidadeAmbientalObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Valor", Texto = "Periculosidade ambiental é obrigatório." }; } }
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Periculosidade ambiental salva com sucesso." }; } }
	}
}
