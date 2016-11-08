using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilvicultura
{
	public class Silvicultura
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public List<Dependencia> Dependencias { get; set; }

		private List<SilviculturaArea> _areas = new List<SilviculturaArea>();
		public List<SilviculturaArea> Areas
		{
			get { return _areas; }
			set { _areas = value; }
		}

		public String AreaAPPNaoCaracterizada
		{
			get
			{
				Decimal APP_TOTAL = 0;
				Decimal APP_AVN = 0;
				Decimal APP_AA_REC = 0;
				Decimal APP_AA_USO = 0;

				if (Areas.Exists(x => x.Tipo == (int)eSilviculturaArea.APP))
				{
					APP_TOTAL = Areas.LastOrDefault(x => x.Tipo == (int)eSilviculturaArea.APP).Valor;
				}

				if (Areas.Exists(x => x.Tipo == (int)eSilviculturaArea.APP_AVN))
				{
					APP_AVN = Areas.LastOrDefault(x => x.Tipo == (int)eSilviculturaArea.APP_AVN).Valor;
				}

				if (Areas.Exists(x => x.Tipo == (int)eSilviculturaArea.APP_AA_REC))
				{
					APP_AA_REC = Areas.LastOrDefault(x => x.Tipo == (int)eSilviculturaArea.APP_AA_REC).Valor;
				}

				if (Areas.Exists(x => x.Tipo == (int)eSilviculturaArea.APP_AA_USO))
				{
					APP_AA_USO = Areas.LastOrDefault(x => x.Tipo == (int)eSilviculturaArea.APP_AA_USO).Valor;
				}

				Decimal total = APP_TOTAL - APP_AVN - APP_AA_REC - APP_AA_USO;

				return total.ToStringTrunc();
			}
		}

		private List<SilviculturaSilvicult> _silviculturas = new List<SilviculturaSilvicult>();
		public List<SilviculturaSilvicult> Silviculturas
		{
			get { return _silviculturas; }
			set { _silviculturas = value; }
		}
	}
}