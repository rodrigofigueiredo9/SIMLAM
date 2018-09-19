using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo
{
	public class LaudoVistoriaFlorestal : Especificidade
	{
		public Int32? Id { get; set; }
		public String Tid { get; set; }
		public Int32 Destinatario { get; set; }
		public String DestinatarioNomeRazao { get; set; }
		public Int32 Responsavel { get; set; }
		public Int32 Caracterizacao { get; set; }
		public Int32 Conclusao { get; set; }
		public String Objetivo { get; set; }
		public String Consideracao { get; set; }
		public String Restricao { get; set; }
		public String ParecerDescricao { get; set; }
		public String ParecerDescricaoDesfavoravel { get; set; }

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