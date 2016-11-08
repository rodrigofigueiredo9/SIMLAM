using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo
{
	public class TermoCPFARL : Especificidade
	{
		public Int32 Id { set; get; }
		public Int32 Requerimento { get; set; }
		public String Tid { set; get; }

		public List<TermoCPFARLDestinatario> _destinatarios = new List<TermoCPFARLDestinatario>();
		public List<TermoCPFARLDestinatario> Destinatarios
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