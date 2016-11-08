using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital
{
	public class ProjetoDigitalListarFiltro
	{
		public int? Requerimento { get; set; }
		public int Credenciado { get; set; }
		public int Situacao { get; set; }
		public bool IsCpf { get; set; }
		public string InteressadoCpfCnpj { get; set; }
		public string InteressadoNomeRazaoSocial { get; set; }
		public int EmpreendimentoID { get; set; }
		public string EmpreendimentoCnpj { get; set; }
		public string EmpreendimentoNomeRazaoSocial { get; set; }
		public int QuantPaginacao { get; set; }
		public DateTecno DataEnvio { get; set; }

		public ProjetoDigitalListarFiltro()
		{
			DataEnvio = new DateTecno();
		}
	}
}