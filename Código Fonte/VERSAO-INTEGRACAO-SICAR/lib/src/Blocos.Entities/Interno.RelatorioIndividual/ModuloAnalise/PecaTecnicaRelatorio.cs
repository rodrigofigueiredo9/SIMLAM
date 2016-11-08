using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloAnalise
{
	public class PecaTecnicaRelatorio
	{
		public Int32? Id { get; set; }
		public String Protocolo { get; set; }
		public String Atividade { get; set; }
		public List<String> Destinatarios { get; set; }

		public Int32 SetorId { set; get; }
		public String SetorCadastro { get; set; }

		public String Bairro { get; set; }
		public String Distrito { get; set; }
		public String Municipio { get; set; }
		public String Uf { get; set; }

		public String Elaborador { set; get; }
		public String ElaboradorTipo { get; set; }
		public String ElaboradorArt { set; get; }
		public String ElaboradorProfissao { set; get; }
		public String ElaboradorOrgaoClasse { set; get; }
		public String ElaboradorRegistro { set; get; }

		public Int32 DominioQtd { set; get; }

		public String ElaboradorOrgaoClasseRegistro
		{
			get
			{
				var separador = (String.IsNullOrWhiteSpace(ElaboradorOrgaoClasse) || String.IsNullOrWhiteSpace(ElaboradorRegistro)) ? "" :" - " ;

				return ElaboradorOrgaoClasse + separador + ElaboradorRegistro;
			}
		}

		public PecaTecnicaRelatorio()
		{
			Destinatarios = new List<string>();
		}
	}
}
