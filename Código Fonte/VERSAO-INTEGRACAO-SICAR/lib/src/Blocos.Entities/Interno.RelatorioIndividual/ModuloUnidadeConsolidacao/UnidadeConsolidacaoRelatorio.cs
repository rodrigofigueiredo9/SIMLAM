using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloUnidadeConsolidacao
{
	public class UnidadeConsolidacaoRelatorio : IAssinanteDataSource
	{
		public long CodigoUc { get; set; }
		public int Id { get; set; }
		public string LocalLivro { get; set; }
		public string TipoApresentacao { get; set; }

		private EmpreendimentoRelatorio _empreendimento = new EmpreendimentoRelatorio();
		public EmpreendimentoRelatorio Empreendimento
		{
			get { return _empreendimento; }
			set { _empreendimento = value; }
		}

		public List<ResponsavelRelatorio> _responsaveisTecnicos = new List<ResponsavelRelatorio>();
		public List<ResponsavelRelatorio> ResponsaveisTecnicos
		{
			get { return _responsaveisTecnicos; }
			set { _responsaveisTecnicos = value; }
		}

		public List<CultivarRelatorio> _cultivar = new List<CultivarRelatorio>();
		public List<CultivarRelatorio> Cultivar
		{
			get { return _cultivar; }
			set { _cultivar = value; }
		}	 
		public int Situacao { get; set; }

		public IAssinante Assinante { get; set; }
		public List<IAssinante> Assinantes1 { get; set; }
		public List<IAssinante> Assinantes2 { get; set; }
		public List<IAssinante> AssinanteSource { get; set; }
	}
}