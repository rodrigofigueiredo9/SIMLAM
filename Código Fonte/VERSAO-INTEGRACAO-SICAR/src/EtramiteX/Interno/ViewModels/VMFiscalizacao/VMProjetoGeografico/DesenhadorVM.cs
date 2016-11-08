using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMProjetoGeografico
{
	public class DesenhadorVM
	{
		private ArquivoProcessamentoVM _arquivoEnviado = new ArquivoProcessamentoVM();
		public ArquivoProcessamentoVM ArquivoEnviado
		{
			get { return _arquivoEnviado; }
			set { _arquivoEnviado = value; }
		}

		private List<ArquivoProcessamentoVM> _arquivosProcessados = new List<ArquivoProcessamentoVM>();
		public List<ArquivoProcessamentoVM> ArquivosProcessados
		{
			get { return _arquivosProcessados; }
			set { _arquivosProcessados = value; }
		}

		public int SituacaoProjeto { get; set; }
		public bool IsVisualizar { get; set; }
		public bool IsFinalizado
		{
			get
			{
				return SituacaoProjeto == (int)eProjetoGeograficoSituacao.Finalizado;
			}
		}
	}
}
