

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao
{
	public class MateriaPrima
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 MateriaPrimaConsumida { get; set; }
		public String MateriaPrimaConsumidaTexto { get; set; }
		public Int32 Unidade { get; set; }
		public String UnidadeTexto { get; set; }
		public String Quantidade { get; set; }
		public String EspecificarTexto { get; set; }

	}
}
