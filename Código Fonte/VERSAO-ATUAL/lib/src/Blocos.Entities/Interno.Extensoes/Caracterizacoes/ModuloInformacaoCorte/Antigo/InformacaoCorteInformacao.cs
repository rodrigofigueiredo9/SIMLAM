using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Antigo
{
	public class InformacaoCorteInformacao
	{
		public Int32 Id { get; set; }
		public Int32 CaracterizacaoId { get; set; }
		public String Tid { get; set; }

		public String ArvoresIsoladasRestantes { get; set; }
		public String AreaCorteRestante { get; set; }

		private DateTecno _dataInformacao = new DateTecno();
		public DateTecno DataInformacao
		{
			get { return _dataInformacao; }
			set { _dataInformacao = value; }
		}

		public String ArvoresIsoladas 
		{
			get 
			{
				Decimal retorno = 0;
				if (Especies != null && Especies.Count > 0) 
				{
					foreach (Especie item in Especies)
					{
						Decimal aux = 0;
						if(Decimal.TryParse(item.ArvoresIsoladas, out aux))
						{
							retorno += aux;
						}
					}
				}

				return retorno.ToString("N0");
			}
		}

		public String AreaCorte
		{
			get
			{
				Decimal retorno = 0;
				if (Especies != null && Especies.Count > 0)
				{
					foreach (Especie item in Especies)
					{
						Decimal aux = 0;
						if (Decimal.TryParse(item.AreaCorte, out aux))
						{
							retorno += aux;
						}
					}
				}

				return retorno.ToString("N4");
			}
		}

		private List<Especie> _especies = new List<Especie>();
		public List<Especie> Especies
		{
			get { return _especies; }
			set { _especies = value; }
		}

		private List<Produto> _produtos = new List<Produto>();
		public List<Produto> Produtos
		{
			get { return _produtos; }
			set { _produtos = value; }
		}
	}
}
