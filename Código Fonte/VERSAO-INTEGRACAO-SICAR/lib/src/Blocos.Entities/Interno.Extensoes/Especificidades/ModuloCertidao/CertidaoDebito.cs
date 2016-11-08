using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao
{
	public class CertidaoDebito : Especificidade
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }
		public Int32 Requerimento { get; set; }
		public Int32 Destinatario { set; get; }
		public String DestinatarioNomeRazao { get; set; }

		private List<Fiscalizacao> _fiscalizacoes = new List<Fiscalizacao>();
		public List<Fiscalizacao> Fiscalizacoes
		{
			get { return _fiscalizacoes; }
			set { _fiscalizacoes = value; }
		}
	}
}