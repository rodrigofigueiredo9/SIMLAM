using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class MotosserraPDF
	{
		public String Marca { set; get; }
		public String NotaFiscal { set; get; }
		public String NumeroFabricacao { set; get; }
		public String NumeroRegistro { set; get; }

		public MotosserraPDF() { }

		public MotosserraPDF(Motosserra motosserra)
		{
			Marca = motosserra.Marca;
			NotaFiscal = motosserra.NotaFiscal;
			NumeroFabricacao = motosserra.NumeroFabricacao;
			NumeroRegistro = motosserra.NumeroRegistro;
		}
	}
}