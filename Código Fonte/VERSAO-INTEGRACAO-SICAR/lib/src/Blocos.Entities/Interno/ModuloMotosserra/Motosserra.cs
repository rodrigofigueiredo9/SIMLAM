using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloMotosserra
{
	public class Motosserra
	{
		public int Id { get; set; }
		public bool PossuiRegistro { get; set; }
		public int RegistroNumero { get; set; }
		public string SerieNumero { get; set; }
		public string Modelo { get; set; }
		public string NotaFiscalNumero { get; set; }
		public Pessoa Proprietario { get; set; }
		public string Tid { get; set; }

		public int SituacaoId { get; set; }
		public String SituacaoTexto
		{
			get
			{
				return ((eMotosserraSituacao)SituacaoId).ToString();
			}
		}


		public Motosserra()
		{
			Proprietario = new Pessoa();
		}
	}
}