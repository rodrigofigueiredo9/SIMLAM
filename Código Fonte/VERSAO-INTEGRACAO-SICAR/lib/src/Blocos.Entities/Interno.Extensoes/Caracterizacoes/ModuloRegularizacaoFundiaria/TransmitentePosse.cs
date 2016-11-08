using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria
{
	public class TransmitentePosse
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Pessoa Transmitente { set; get; }
		public Int32 TempoOcupacao { get; set; }

		public TransmitentePosse()
		{
			Transmitente = new Pessoa();
		}
	}
}
