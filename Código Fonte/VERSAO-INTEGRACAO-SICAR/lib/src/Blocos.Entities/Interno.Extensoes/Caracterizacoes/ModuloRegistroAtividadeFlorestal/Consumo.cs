using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal
{
	public class Consumo
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 Atividade { get; set; }
		public String AtividadeNome { get; set; }
		public String AtividadeCategoria { get; set; }
		public Int32? PossuiLicencaAmbiental { get; set; }
		public FinalidadeCaracterizacao Licenca { get; set; }
		public List<FonteEnergia> FontesEnergia { get; set; }

		public String DUANumero { get; set; }
		public String DUAValor { get; set; }
		public DateTecno DUADataPagamento { get; set; }

		public Consumo()
		{
			DUADataPagamento = new DateTecno();
			Licenca = new FinalidadeCaracterizacao();
			FontesEnergia = new List<FonteEnergia>();
		}
	}
}