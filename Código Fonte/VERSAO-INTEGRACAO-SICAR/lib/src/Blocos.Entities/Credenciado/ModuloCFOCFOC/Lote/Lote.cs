using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC.Lote
{
	public class Lote
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public int SituacaoId { get; set; }
		public string SituacaoTexto { get; set; }
		public int EmpreendimentoId { get; set; }
		public long CodigoUC { get; set; }
		public int Ano { get; set; }
		public string Numero { get; set; }
		public DateTecno DataCriacao { get; set; }
		public List<LoteItem> Lotes { get; set; }
		public LoteItem Item { get; set; }
		public string CulturaCultivar { get; set; }

		public string NumeroCompleto
		{
			get
			{
				if (!string.IsNullOrEmpty(Numero))
				{
					return CodigoUC.ToString() + Ano.ToString() + Numero.ToString().PadLeft(4, '0');
				}

				return null;
			}
		}

		public Lote()
		{
			DataCriacao = new DateTecno();
			Lotes = new List<LoteItem>();
			Item = new LoteItem();
		}
	}
}