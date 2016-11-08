using System;
using System.Collections.Generic;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMProjetoGeografico
{
	public class BaseReferenciaVM
	{
		public Boolean IsVisualizar { get; set; }

		List<ArquivoProcessamentoVM> _arquivoDefault = new List<ArquivoProcessamentoVM>();
		public List<ArquivoProcessamentoVM> ArquivosDefault
		{
			get { return _arquivoDefault; }
			set { _arquivoDefault = value; }
		}

		List<ArquivoProcessamentoVM> _arquivosVetoriais = new List<ArquivoProcessamentoVM>();
		public List<ArquivoProcessamentoVM> ArquivosVetoriais
		{
			get { return _arquivosVetoriais; }
			set { _arquivosVetoriais = value; }
		}

		List<ArquivoProcessamentoVM> _arquivosOrtoFotoMosaico= new List<ArquivoProcessamentoVM>();
		public List<ArquivoProcessamentoVM> ArquivosOrtoFotoMosaico
		{
			get { return _arquivosOrtoFotoMosaico; }
			set { _arquivosOrtoFotoMosaico = value; }
		}

		List<ArquivoProcessamentoVM> _dadosDominio = new List<ArquivoProcessamentoVM>();
		public List<ArquivoProcessamentoVM> DadosDominio
		{
			get { return _dadosDominio; }
			set { _dadosDominio = value; }
		}

		public BaseReferenciaVM() 
		{
			ArquivosVetoriais.Add(new ArquivoProcessamentoVM() { Texto = "Base de Referência Fiscalização", SituacaoTexto = "Aguardando solicitação ", Tipo = 1, MostarGerar = true });
			//ArquivosVetoriais.Add(new ArquivoProcessamentoVM() { Texto = "Dados GEOBASES", SituacaoTexto = "Aguardando solicitação ", Tipo = 2, MostarGerar = true });
			ArquivosDefault.AddRange(ArquivosVetoriais);
		}

		public int SituacaoProjeto { get; set; }
	}
}