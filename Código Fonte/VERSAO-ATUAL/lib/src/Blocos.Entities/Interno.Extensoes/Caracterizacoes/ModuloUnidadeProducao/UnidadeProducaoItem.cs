using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao
{
	public class UnidadeProducaoItem
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 IdRelacionamento { get; set; }
		public bool PossuiCodigoUP { get; set; }
		public Int64 CodigoUP { get; set; }
        public string CodigoUPStr
       {
           get { return CodigoUP.ToString(); }
        }
		public int TipoProducao { get; set; }
		public string RenasemNumero { get; set; }
		public string DataValidadeRenasem { get; set; }
		public decimal AreaHA { get; set; }
		public decimal EstimativaProducaoQuantidadeAno { get; set; }
		public string DataPlantioAnoProducao { get; set; }
		public string EstimativaProducaoUnidadeMedida
		{
			get
			{
				switch ((eUnidadeProducaoTipoProducao)TipoProducao)
				{
					case eUnidadeProducaoTipoProducao.Frutos:
						return "T";
					case eUnidadeProducaoTipoProducao.Madeira:
						return "m³";
					case eUnidadeProducaoTipoProducao.MaterialPropagacao:
						return "Und";
					default:
						break;
				}
				return "T";
			}
		}
		public string AnoAbertura { get; set; }
		public int CodigoSequencial { get; set; }
		public int CulturaId { get; set; }
		public string CulturaTexto { get; set; }
		public int? CultivarId { get; set; }
		public string CultivarTexto { get; set; }

		public Municipio Municipio { get; set; }
		public Coordenada Coordenada { get; set; }
		public List<ResponsavelTecnico> ResponsaveisTecnicos { get; set; }
		public ResponsavelTecnico ResponsavelTecnico { get; set; }
		public List<Responsavel> Produtores { get; set; }

		public int EstimativaProducaoUnidadeMedidaId
		{
			get
			{
				switch ((eUnidadeProducaoTipoProducao)TipoProducao)
				{
					case eUnidadeProducaoTipoProducao.Frutos:
						return 2;
					case eUnidadeProducaoTipoProducao.Madeira:
						return 1;
					case eUnidadeProducaoTipoProducao.MaterialPropagacao:
						return 3;
				}
				return 0;
			}
		}

		public UnidadeProducaoItem()
		{
			Municipio = new Municipio();
			Produtores = new List<Responsavel>();
			ResponsavelTecnico = new ResponsavelTecnico();
			ResponsaveisTecnicos = new List<ResponsavelTecnico>();
			Coordenada = new Coordenada();
		}
	}
}