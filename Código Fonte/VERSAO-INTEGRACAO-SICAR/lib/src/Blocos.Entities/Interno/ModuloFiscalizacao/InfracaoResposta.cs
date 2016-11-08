

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class InfracaoResposta
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String Identificacao { get; set; }
		public Boolean IsSelecionado { get; set; }
		public Boolean IsEspecificar { get; set; }
	}
}
