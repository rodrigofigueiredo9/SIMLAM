

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa
{
	public class ProfissaoRelatorio
	{
		public Int32 Id { get; set; }
		public Int32 PessoaId { get; set; }
		public Int32 ProfissaoId { get; set; }
		public String ProfissaoTexto { get; set; }
		public Int32? OrgaoClasseId { get; set; }
		public String OrgaoClasseTexto { get; set; }
		public String Registro { get; set; }
		public String Tid { get; set; }
	}
}