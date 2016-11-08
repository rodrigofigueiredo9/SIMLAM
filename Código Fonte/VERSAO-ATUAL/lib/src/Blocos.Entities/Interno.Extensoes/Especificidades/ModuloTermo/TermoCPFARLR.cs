using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo
{
	public class TermoCPFARLR : Especificidade
	{
		public Int32 Id { set; get; }
		public Int32 Requerimento { get; set; }
		public String Tid { set; get; }
		public Boolean IsDataTituloAnterior { get; set; }

		public DateTecno _dataTituloAnterior = new DateTecno();
		public DateTecno DataTituloAnterior
		{
			get { return _dataTituloAnterior; }
			set { _dataTituloAnterior = value; }
		}

		public string _numeroAverbacao { get; set; }

		public string NumeroAverbacao
		{
			get { return _numeroAverbacao; }
			set { _numeroAverbacao = value; }
		}

		public List<TermoCPFARLRDestinatario> _destinatarios = new List<TermoCPFARLRDestinatario>();
		public List<TermoCPFARLRDestinatario> Destinatarios
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		private List<Anexo> _anexos = new List<Anexo>();
		public List<Anexo> Anexos
		{
			get { return _anexos; }
			set { _anexos = value; }
		}
	}
}