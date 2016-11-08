using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo
{
	public class LaudoVistoriaFundiaria : Especificidade
	{
		public Int32? Id { get; set; }
		public String Tid { get; set; }
		public Int32 Destinatario { get; set; }
		public String DestinatarioNomeRazao { get; set; }
		public Int32 RegularizacaoId { get; set; }
		public String RegularizacaoTid { get; set; }

		private List<RegularizacaoDominio> _regularizacaoDominios = new List<RegularizacaoDominio>();
		public List<RegularizacaoDominio> RegularizacaoDominios
		{
			get { return _regularizacaoDominios; }
			set { _regularizacaoDominios = value; }
		}

		private DateTecno _dataVistoria = new DateTecno();
		public DateTecno DataVistoria
		{
			get { return _dataVistoria; }
			set { _dataVistoria = value; }
		}
	}
}