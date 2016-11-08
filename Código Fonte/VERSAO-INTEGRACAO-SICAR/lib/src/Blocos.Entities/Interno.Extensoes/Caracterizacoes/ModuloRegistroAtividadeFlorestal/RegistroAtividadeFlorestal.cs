using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal
{
	public class RegistroAtividadeFlorestal
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public Boolean? PossuiNumero { get; set; }
		public String NumeroRegistro { get; set; }
		public List<Consumo> Consumos { get; set; }

		public RegistroAtividadeFlorestal()
		{
			Consumos = new List<Consumo>();
		}
	}
}