using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao
{
	public class MaterialApreendidoRelatorio
	{
		public Int32 Id { get; set; }
		public Int32 HistoricoId { get; set; }
		public int? IsGeradoSistema { get; set; }
		public String IsApreendido { get; set; }
		public String NumeroTAD { get; set; }
		public String DataLavraturaTAD { get; set; }
		public String DescreverApreensao { get; set; }
		public String OpinarDestino { get; set; }
		public String SerieTexto { get; set; }

		private List<MaterialApreendidoMaterialRelatorio> _materiais = new List<MaterialApreendidoMaterialRelatorio>();
		public List<MaterialApreendidoMaterialRelatorio> Materiais
		{
			get { return _materiais; }
			set { _materiais = value; }
		}

	}
}
