

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao
{
	public class ObjetoInfracaoRelatorioNovo
	{
		public int? IsGeradoSistema { get; set; }
		public String AreaEmbargadaInterditada { get; set; }
		public String NumeroTEI { get; set; }
		public String DataLavraturaTEI { get; set; }
        public String NumeroIUF { get; set; }
        public String DataLavraturaIUF { get; set; }
		public String OpinarEmbargo { get; set; }
		public String AtividadeAreaDegradado { get; set; }
		public String AtividadeAreaDegradadoEspecif { get; set; }
		public String FundamentoCaracterizacaoInfra { get; set; }
		public String UsoOcupacaoSolo { get; set; }
		public String CaracteristicaoAreaSoloDanifi { get; set; }
		public String DeclividadeMedia { get; set; }
		public String IsInfracaoErosaoSolo { get; set; }
		public String EspecificarIsInfracaoErosaoSolo { get; set; }		
		public String SerieTexto { get; set; }
	}
}
