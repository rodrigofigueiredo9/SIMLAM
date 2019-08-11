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
		public eStatusImovelSicar status { get; set; } = eStatusImovelSicar.Cancelado;
		public eCARCancelamentoMotivo condicao { get; set; } = eCARCancelamentoMotivo.DecisaoAdmnistrativa;
		public bool retificavel { get; set; } = true;
		public string codigoImovel { get; set; } = "ES-3203205-DD960C5D228C469FB8DC7C7F8F0F4434";
		public bool completo { get; set; } = false;
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
