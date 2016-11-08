using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMProjetoGeografico
{
	public class EnviarProjetoVM
	{
		public Boolean IsVisualizar { get; set; }

		private ArquivoProcessamentoVM _arquivoEnviado = new ArquivoProcessamentoVM();
		public ArquivoProcessamentoVM ArquivoEnviado
		{
			get { return (ArquivosProcessados.FirstOrDefault(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado) ?? new ArquivoProcessamentoVM()); }
		}

		private List<ArquivoProcessamentoVM> _arquivosProcessados = new List<ArquivoProcessamentoVM>();
		public List<ArquivoProcessamentoVM> ArquivosProcessados
		{
			get { return _arquivosProcessados; }
			set { _arquivosProcessados = value; }
		}

		public int SituacaoProjeto { get; set; }
	}
}
