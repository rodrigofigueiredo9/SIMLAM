using System;
using System.ComponentModel.DataAnnotations;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPessoa
{
	public class Profissao
	{
		public Int32 Id { get; set; }
		public Int32 IdRelacionamento { get; set; }
		public Int32 PessoaId { get; set; }
		[Display(Order = 17, Name = "Profissão")]
		public String ProfissaoTexto { get; set; }
		public Int32? OrgaoClasseId { get; set; }
		[Display(Order = 18, Name = "Órgão de classe")]
		public String OrgaoClasseTexto { get; set; }
		[Display(Order = 19, Name = "Registro")]
		public String Registro { get; set; }
		public String Tid { get; set; }
	}
}