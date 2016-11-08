using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado
{
	public class CertificadoCadastroProdutoVegetal : Especificidade
	{
		public int? Id { get; set; }
		public String Tid { get; set; }
		public Int32? Destinatario { get; set; }
		public String DestinatarioNomeRazao { get; set; }
		public String Nome { get; set; }
		public String Fabricante { get; set; }
		public String ClasseToxicologica { get; set; }
		public String Classe { get; set; }
		public String Ingrediente { get; set; }
		public String Classificacao { get; set; }
		public String Cultura { get; set; }

		public CertificadoCadastroProdutoVegetal()
		{
			this.Tid =
			this.Nome =
			this.Fabricante =
			this.ClasseToxicologica =
			this.Classe =
			this.Ingrediente =
			this.Classificacao =
			this.Cultura = "";
		}
	}
}