using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloUnidadeProducao
{
	public class UnidadeProducaoItemRelatorio
	{
		public int Id { get; set; }
		public string CodigoUP 
        { 
            get; 
            set; 
        }
		public string Latitude { get; set; }
		public string Longitude { get; set; }

		string _cultivarNome = string.Empty;
		public string CultivarNome
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_cultivarNome) && !CulturaNome.ToLower().Equals("citros"))
				{
					return string.Concat(CulturaNome, " ", _cultivarNome);
				}
				else if (CulturaNome.ToLower().Equals("citros"))
				{
					return _cultivarNome;
				}
				return CulturaNome;
			}
			set
			{
				_cultivarNome = value;
			}
		}
		public string CulturaNome { get; set; }
		public string DataPlantio { get; set; }
		public string QuantidadeAno { get; set; }
		public string UnidadeMedida { get; set; }

		private List<ResponsavelTecnicoRelatorio> _responsaveis = new List<ResponsavelTecnicoRelatorio>();
		public List<ResponsavelTecnicoRelatorio> Responsaveis
		{
			get { return _responsaveis; }
			set { _responsaveis = value; }
		}

		private string _areaHa = "";
		public string AreaHa
		{
			get { return Convert.ToDecimal(_areaHa).ToStringTrunc(4); }
			set { _areaHa = value; }
		}
	}
}