

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities
{
	public enum eTipoContato
	{
		TelefoneResidencial = 1,
		TelefoneCelular,
		TelefoneFax,
		TelefoneComercial,
		Email,
		NomeContato
	}

	public class ContatoRelatorio
	{
		public Int32 Id { get; set; }
		public eTipoContato TipoContato { get; set; }
		public String TipoTexto { get; set; }
		public Int32 PessoaId { get; set; }
		public String Valor { get; set; }
		public String Mascara { get; set; }
		public String Tid { get; set; }
	}
}