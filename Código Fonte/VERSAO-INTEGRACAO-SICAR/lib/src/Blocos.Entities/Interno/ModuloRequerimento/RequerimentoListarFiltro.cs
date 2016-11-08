using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento
{
	public class RequerimentoListarFiltro
	{
		public int? Numero { get; set; }
		public int? Credenciado { get; set; }
		public int? Situacao { get; set; }
		public string InteressadoNomeRazao { get; set; }
		public string InteressadoCpfCnpj { get; set; }
		public string EmpreendimentoDenominador { get; set; }
		public string EmpreendimentoCnpj { get; set; }
		public bool IsCpf { get; set; }
        public bool IsRemoverTituloDeclaratorio { get; set; }

		private List<eProjetoDigitalSituacao> _projetoDigitalSituacoes;

		public List<eProjetoDigitalSituacao> ProjetoDigitalSituacoes
		{
			get { return _projetoDigitalSituacoes; }
			set { _projetoDigitalSituacoes = value; }
		}
	}
}