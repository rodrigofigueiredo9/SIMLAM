using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Model.Business;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragem
{
	public class Barragem
	{
		public int Id { set; get; }
		public string Tid { set; get; }
		public int EmpreendimentoId { set; get; }

		private int _atividadeId = 0;
		public int AtividadeId
		{
			get { return _atividadeId; }
		}

		public List<BarragemItem> Barragens { set; get; }
		public List<Dependencia> Dependencias { get; set; }

		public decimal? TotalLamina { get { return this.Barragens.Sum(x => x.TotalLamina); } }
		public decimal? TotalArmazenado { get { return this.Barragens.Sum(x => x.TotalArmazenado); } }

		public string ToStringTotalLamina { get { return TotalLamina.ToStringTrunc(4); } }
		public string ToStringTotalArmazenado { get { return TotalArmazenado.ToStringTrunc(4); } }

		public Barragem()
		{
			_atividadeId = EntitiesBus.ObterAtividadeId((int)eAtividadeCodigo.Barragem);
			this.Barragens = new List<BarragemItem>();
			this.Dependencias = new List<Dependencia>();
		}
	}
}