using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado
{
	public class CertificadoCadastroProdutoAgrotoxico : Especificidade
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }

		public Int32 DestinatarioId { get; set; }
		public String DestinatarioNomeRazao { get; set; }

		public String AgrotoxicoNome { get; set; }
		public Int32 AgrotoxicoId { get; set; }
		public String AgrotoxicoTid { get; set; }

		public CertificadoCadastroProdutoAgrotoxico()
		{

		}

	}
}

