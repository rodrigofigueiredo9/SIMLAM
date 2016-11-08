using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico
{
	public class ImportadorShapeVM
	{
		private ArquivoProcessamentoVM _arquivoEnviado = new ArquivoProcessamentoVM();
		public ArquivoProcessamentoVM ArquivoEnviado
		{
			get { return (ArquivosProcessados.FirstOrDefault(x => x.ArquivoEnviadoTipo == (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado) ?? new ArquivoProcessamentoVM()); }
		}

		private List<ArquivoProcessamentoVM> _arquivosProcessados = new List<ArquivoProcessamentoVM>();
		public List<ArquivoProcessamentoVM> ArquivosProcessados
		{
			get { return _arquivosProcessados; }
			set { _arquivosProcessados = value; }
		}

		public int SituacaoProjeto { get; set; }
		public bool IsVisualizar { get; set; }
	}
}