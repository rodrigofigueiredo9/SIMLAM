

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloMotosserra
{
	public class MotosserraListarFiltros
	{
		public int? RegistroNumero { get; set; }
		public string SerieNumero { get; set; }
		public string Modelo { get; set; }
		public string NotaFiscalNumero { get; set; }
		public string PessoaNomeRazao { get; set; }
		public string PessoaCpfCnpj { get; set; }
		public bool PessoaIsCnpj { get; set; }
		public Int32 Situacao { get; set; }
	}
}