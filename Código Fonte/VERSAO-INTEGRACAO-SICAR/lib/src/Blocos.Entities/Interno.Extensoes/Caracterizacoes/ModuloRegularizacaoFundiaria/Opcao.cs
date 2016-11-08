

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria
{
	public class Opcao
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 Tipo { get; set; }
		public Int32? Valor { get; set; }
		public String Outro { get; set; }

		public eTipoOpcao TipoEnum
		{
			get { return (eTipoOpcao) Tipo; }
			set { Tipo = (Int32)value; }
		}
	}
}