using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo
{
	public class LaudoAuditoriaFomentoFlorestal : Especificidade
	{
		public Int32? Id { get; set; }
		public String Tid { get; set; }
		public Int32 Destinatario { get; set; }
		public String DestinatarioNomeRazao { get; set; }
		public String Objetivo { get; set; }
		public String ParecerDescricao { get; set; }

		public Int32? PlantioAPP { get; set; }
		public String PlantioAPPArea { get; set; }
		public Int32? PlantioMudasEspeciesFlorestNativas { get; set; }
		public String PlantioMudasEspeciesFlorestNativasQtd { get; set; }
		public String PlantioMudasEspeciesFlorestNativasArea { get; set; }
		public Int32? PreparoSolo { get; set; }
		public String PreparoSoloArea { get; set; }
		public Int32 ResultadoTipo { get; set; }
		public String ResultadoTipoTexto { get; set; }
		public String ResultadoQuais { get; set; }

		private DateTecno _dataVistoria = new DateTecno();
		public DateTecno DataVistoria
		{
			get { return _dataVistoria; }
			set { _dataVistoria = value; }
		}

		private List<Anexo> _anexos = new List<Anexo>();
		public List<Anexo> Anexos
		{
			get { return _anexos; }
			set { _anexos = value; }
		}
	}
}