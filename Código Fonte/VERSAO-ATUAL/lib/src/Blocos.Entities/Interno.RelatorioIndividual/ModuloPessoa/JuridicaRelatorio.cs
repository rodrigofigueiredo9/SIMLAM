using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa
{
	public class JuridicaRelatorio
	{
		public String CNPJ { get; set; }
		public String RazaoSocial { get; set; }
		public String NomeFantasia { get; set; }
		public String IE { get; set; }
		public Int32? Porte { get; set; }
		public DateTime? DataFundacao { get; set; }

		public int RepresentanteId { get; set; }
		public String RepresentanteTexto { get; set; }

		private List<PessoaRelatorio> _representantes = new List<PessoaRelatorio>();
		public List<PessoaRelatorio> Representantes
		{
			get { return _representantes; }
			set { _representantes = value; }

		}
	}
}
