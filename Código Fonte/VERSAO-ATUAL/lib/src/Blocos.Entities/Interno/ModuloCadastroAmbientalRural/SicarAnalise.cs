using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural
{
	public class SicarAnalise
	{
		public int id { get; set; }
		public int status { get; set; }
		public int condicao { get; set; }
		public bool retificavel { get; set; }
		public string codigoImovel { get; set; }
		public bool completo { get; set; }
		public string motivoMudancaStatus { get; set; }
		public string cpfResponsavel { get; set; }
		public bool vistoriaRealizada { get; set; }
		public int situacaoRL { get; set; }
		public DateTecno dataValidacaoAnalise { get; set; }
		public Arquivo.Arquivo relatorioAnaliseTecnica { get; set; }
		public string quadroAreas { get; set; }
		public string geoRL { get; set; }
		public Arquivo.Arquivo documentoCancelamento { get; set; }
	}
}
