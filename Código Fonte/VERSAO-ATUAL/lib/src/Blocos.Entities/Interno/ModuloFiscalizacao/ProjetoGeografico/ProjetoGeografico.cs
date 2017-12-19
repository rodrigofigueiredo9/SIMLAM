using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class ProjetoGeografico
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }

        public Boolean? PossuiProjetoGeo { get; set; }
		public Int32 NivelPrecisaoId { get; set; }
		public String NivelPrecisaoTexto { get; set; }
		public Int32 MecanismoElaboracaoId { get; set; }
		public String MecanismoElaboracaoTexto { get; set; }
		public Int32 SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }
		public String SistemaCoordenada { get; set; }
		
		public Decimal MenorX { get; set; }
		public Decimal MenorY { get; set; }
		public Decimal MaiorX { get; set; }
		public Decimal MaiorY { get; set; }

		public Int32 FiscalizacaoId { get; set; }
		public Decimal? FiscalizacaoEasting { get; set; }
		public Decimal? FiscalizacaoNorthing { get; set; }

		public ArquivoProjeto ArquivoEnviadoDesenhador{ get; set; }

		public List<ArquivoProjeto> Arquivos { get; set; }

		public List<ArquivoProjeto> ArquivosOrtofotos { get; set; }

		public List<ArquivoProjeto> ArquivosDominio { get; set; }

		public List<Dependencia> Dependencias { get; set; }

		public Sobreposicoes Sobreposicoes { get; set; }

		public ProjetoGeografico ProjetoDependente { get; set; }
		
		private bool _cruzou = false;
		public bool FiscalizacaoEstaDentroAreaAbrangencia
		{
			get
			{
				var northing = Convert.ToDecimal(this.FiscalizacaoNorthing);
				var easting = Convert.ToDecimal(this.FiscalizacaoEasting);
				this.CorrigirMbr();
				_cruzou = easting <= this.MaiorX && easting >= this.MenorX && northing <= this.MaiorY && northing >= this.MenorY;
				return _cruzou || this.Id<=0;
			}
			//set { _cruzou = value; }
		}

		public ProjetoGeografico()
		{
			SituacaoId = (int)eProjetoGeograficoSituacao.EmElaboracao;
			SituacaoTexto = "Em elaboração";
			Arquivos = new List<ArquivoProjeto>();
			ArquivosOrtofotos = new List<ArquivoProjeto>();
			ArquivosDominio = new List<ArquivoProjeto>();
			ArquivoEnviadoDesenhador = new ArquivoProjeto();
			Dependencias = new List<Dependencia>();
			Sobreposicoes = new Sobreposicoes();
		}

		public void CorrigirMbr()
		{
			if (MenorX > MaiorX)
			{
				decimal aux = MenorX;
				MenorX = MaiorX;
				MaiorX = aux;
			}

			if (MenorY > MaiorY)
			{
				decimal aux = MenorY;
				MenorY = MaiorY;
				MaiorY = aux;
			}
		}
	}
}
