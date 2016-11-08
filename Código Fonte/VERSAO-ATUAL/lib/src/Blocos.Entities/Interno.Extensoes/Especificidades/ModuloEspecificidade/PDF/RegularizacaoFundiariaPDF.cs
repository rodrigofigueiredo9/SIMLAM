using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class RegularizacaoFundiariaPDF
	{
		private List<PossePDF> _posses = new List<PossePDF>();
		public List<PossePDF> Posses
		{
			get { return _posses; }
			set { _posses = value; }
		}

		private PossePDF _posse = new PossePDF();
		public PossePDF Posse
		{
			get { return _posse; }
			set { _posse = value; }
		}

		#region AreaTotalPosse

		public String AreaTotalPosse 
		{
			get 
			{
				if (Posses != null && Posses.Count > 0)
				{
					return Posses.Sum(posse => posse.AreaCroquiDecimal).ToStringTrunc(4);
				}

				return Decimal.Zero.ToStringTrunc(4);
			}
		}

		#endregion

		#region AreaTotalRequeridaPosse

		public Decimal AreaTotalRequeridaPosse 
		{
			get
			{
				Decimal retorno = 0;
				if (Posses != null && Posses.Count > 0)
				{
					Decimal aux = 0;
					foreach (PossePDF posse in Posses)
					{
						if (Decimal.TryParse(posse.AreaRequerida, out aux))
						{
							retorno += aux;
						}
					}
				}

				return retorno;
			}
		}

		#endregion

		public RegularizacaoFundiariaPDF(RegularizacaoFundiaria regularizacao)
		{
			foreach (Posse posse in regularizacao.Posses)
			{
				Posses.Add(new PossePDF(posse, regularizacao.Matriculas));
			}
		}

		public RegularizacaoFundiariaPDF(){}

	}
}
