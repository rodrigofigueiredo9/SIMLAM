using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloUnidadeConsolidacao;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloUnidadeProducao;


namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloProjetoDigital
{
	public class RelatorioTecnico : IAssinanteDataSource
	{
		ProjetoDigitalRelatorio _projetoDigital = new ProjetoDigitalRelatorio();
		RequerimentoRelatorio _requerimentoDigital = new RequerimentoRelatorio();
		DominialidadePDF _dominialidade = new DominialidadePDF();

		private CredenciadoRelatorio _usuarioCredenciado = new CredenciadoRelatorio();
		public CredenciadoRelatorio UsuarioCredenciado
		{
			get { return _usuarioCredenciado; }
			set { _usuarioCredenciado = value; }
		}

		public DominialidadePDF Dominialidade
		{
			get { return _dominialidade; }
			set { _dominialidade = value; }
		}

		public RequerimentoRelatorio RequerimentoDigital
		{
			get { return _requerimentoDigital; }
			set { _requerimentoDigital = value; }
		}

		public ProjetoDigitalRelatorio ProjetoDigital
		{
			get { return _projetoDigital; }
			set { _projetoDigital = value; }
		}

		private List<AssinanteDefault> _assinantes = new List<AssinanteDefault>();
		public List<AssinanteDefault> Assinantes
		{
			get { return _assinantes; }
			set { _assinantes = value; }
		}

		public IAssinante Assinante { get; set; }
		public List<IAssinante> AssinanteSource { get; set; }
		public List<IAssinante> Assinantes1 { get; set; }
		public List<IAssinante> Assinantes2 { get; set; }

		public UnidadeProducaoRelatorio UnidadeProducao { get; set; }

		public UnidadeConsolidacaoRelatorio UnidadeConsolidacao { get; set; }
	}
}
