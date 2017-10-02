using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class ObjetoInfracao
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 FiscalizacaoId { get; set; }
		public Int32 FiscalizacaoSituacaoId { get; set; }
        public Boolean? IsDigital { get; set; }
        public String NumeroIUF { get; set; }
        public Int32? SerieId { get; set; }
        public Boolean? Interditado { get; set; }
		public Int32? AreaEmbargadaAtvIntermed { get; set; }
		public Int32? TeiGeradoPeloSistema { get; set; }
		public Int32 TeiGeradoPeloSistemaSerieTipo { get; set; }
		public String TeiGeradoPeloSistemaSerieTipoTexto { get; set; }
		public String NumTeiBloco { get; set; }
        public String NumeroLacre { get; set; }

		private DateTecno _dataLavraturaTermo = new DateTecno();
		public DateTecno DataLavraturaTermo
		{
			get { return _dataLavraturaTermo; }
			set { _dataLavraturaTermo = value; }
		}

		public String OpniaoAreaDanificada { get; set; }
		public String DescricaoTermoEmbargo { get; set; }
		public Int32? ExisteAtvAreaDegrad { get; set; }
		public String ExisteAtvAreaDegradEspecificarTexto { get; set; }
		public String FundamentoInfracao { get; set; }
		public String UsoSoloAreaDanificada { get; set; }

		public Int32? CaracteristicaSoloAreaDanificada { get; set; }
		public String CaracteristicaSoloAreaDanificadaTipoTexto { get; set; }

		public String AreaDeclividadeMedia { get; set; }
		public Int32 InfracaoResultouErosaoTipo { get; set; }
		public String InfracaoResultouErosaoTipoTexto { get; set; }

		public Arquivo.Arquivo _arquivo = new Arquivo.Arquivo();
		public Arquivo.Arquivo Arquivo
		{
			get { return _arquivo; }
			set { _arquivo = value; }
		}
	}
}