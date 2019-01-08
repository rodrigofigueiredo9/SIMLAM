using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloDUA
{

	public class Dua
	{
		public String Codigo { get; set; }
		public Decimal Valor { get; set; }
		public eSituacaoDua Situacao { get; set; }
		public String SituacaoTexto { get; set; }
		public String Numero { get; set; }
		public DateTecno Validade { get; set; }
		public String CpfCnpj { get; set; }


		public Dua(){ }
	}
}