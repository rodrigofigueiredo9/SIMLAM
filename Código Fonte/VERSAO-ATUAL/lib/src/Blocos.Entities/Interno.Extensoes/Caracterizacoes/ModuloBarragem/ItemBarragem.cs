using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragem
{
	public class BarragemItem
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }
		public int? Quantidade { get; set; }
		public int FinalidadeId { get; set; }
		public string FinalidadeTexto { get; set; }
		public string Especificar { get; set; }
		public CoordenadaAtividade CoordenadaAtividade { get; set; }
		public List<BarragemDadosItem> BarragensDados { get; set; }
		public decimal? TotalLamina { get { return BarragensDados.Sum(x => x.LaminaAguaToDecimal); } }
		public decimal? TotalArmazenado { get { return BarragensDados.Sum(x => x.VolumeArmazenamentoToDecimal); } }

		public string ToStringTotalLamina { get { return TotalLamina.ToStringTrunc(4); } }
		public string ToStringTotalArmazenado { get { return TotalArmazenado.ToStringTrunc(4); } }

		public string Tid { get; set; }

		public BarragemItem()
		{
			Especificar =
			Tid = string.Empty;
			CoordenadaAtividade = new CoordenadaAtividade();
			BarragensDados = new List<BarragemDadosItem>();
		}
	}
}