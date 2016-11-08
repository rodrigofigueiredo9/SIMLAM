using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloUnidadeProducao
{
	public class UnidadeProducaoRelatorio : IAssinanteDataSource
	{
		public string CodigoPropriedade { get; set; }
		public int Id { get; set; }
		public eProjetoDigitalSituacao Situacao { get; set; }
		public string LocalLivro { get; set; }

		private EmpreendimentoRelatorio _empreendimento = new EmpreendimentoRelatorio();
		public EmpreendimentoRelatorio Empreendimento
		{
			get { return _empreendimento; }
			set { _empreendimento = value; }
		}

		private List<PessoaRelatorio> _produtores = new List<PessoaRelatorio>();
		public List<PessoaRelatorio> Produtores
		{
			get { return _produtores; }
			set { _produtores = value; }
		}

		private List<UnidadeProducaoItemRelatorio> _up = new List<UnidadeProducaoItemRelatorio>();
		public List<UnidadeProducaoItemRelatorio> UP
		{
			get { return _up; }
			set { _up = value; }
		}

		public List<ResponsavelTecnicoRelatorio> Responsaveis
		{
			get
			{
				List<ResponsavelTecnicoRelatorio> responsaveis = new List<ResponsavelTecnicoRelatorio>();

				UP.SelectMany(x => x.Responsaveis).ToList().ForEach(item =>
				{
					if (!responsaveis.Any(x => x.NomeRazao == item.NomeRazao))
					{
						responsaveis.Add(item);
					}
				});

				return responsaveis;
			}
		}

		public IAssinante Assinante { get; set; }
		public List<IAssinante> Assinantes1 { get; set; }
		public List<IAssinante> Assinantes2 { get; set; }
		public List<IAssinante> AssinanteSource { get; set; }
	}
}