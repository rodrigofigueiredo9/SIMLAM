using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico
{
	public class BaseReferenciaVM
	{
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
			get
			{
				_dadosDominio.ForEach(x =>
				{
					if (x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui)
					{
						x.ArquivoEnviadoFilaTipo = (int)eFilaTipoGeo.Dominialidade;
					}
				});

				return _dadosDominio;

			}
			set { _dadosDominio = value; }
		}

		public BaseReferenciaVM() 
		{

			ArquivosVetoriais.Add(new ArquivoProcessamentoVM()
			{
				Texto = "Dados IDAF",
				SituacaoTexto = "Aguardando solicitação ",
				Tipo = (int)eProjetoGeograficoArquivoTipo.DadosIDAF,
				FilaTipo = (int)eFilaTipoGeo.BaseReferenciaInterna,
				MostarGerar = true
			});

			ArquivosVetoriais.Add(new ArquivoProcessamentoVM()
			{
				Texto = "Dados GEOBASES",
				SituacaoTexto = "Aguardando solicitação ",
				Tipo = (int)eProjetoGeograficoArquivoTipo.DadosGEOBASES,
				FilaTipo = (int)eFilaTipoGeo.BaseReferenciaGEOBASES,
				MostarGerar = true
			});
			
			ArquivosDefault.AddRange(ArquivosVetoriais);
		}

		public int SituacaoProjeto { get; set; }

		public bool IsVisualizar { get; set; }
	}
}