using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros
{
	public class OutrosLegitimacaoTerraDevoluta : Especificidade
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }
		public Int32 Requerimento { get; set; }
		public Int32 Dominio { get; set; }
		public Boolean? IsInalienabilidade { get; set; }
		public String ValorTerreno { get; set; }
		public Int32 MunicipioGlebaId { set; get; }
		public String MunicipioGlebaTexto { get; set; }

		private List<OutrosLegitimacaoDestinatario> _destinatarios = new List<OutrosLegitimacaoDestinatario>();
		public List<OutrosLegitimacaoDestinatario> Destinatarios
		{
			get { return this._destinatarios; }
			set { this._destinatarios = value; }
		}
	}
}