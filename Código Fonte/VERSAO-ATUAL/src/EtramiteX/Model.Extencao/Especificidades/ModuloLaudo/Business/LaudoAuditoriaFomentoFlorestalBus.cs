using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilviculturaATV.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Business
{
	public class LaudoAuditoriaFomentoFlorestalBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		#region Propriedades

		LaudoAuditoriaFomentoFlorestalDa _da = new LaudoAuditoriaFomentoFlorestalDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());

		#endregion

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Laudo; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new LaudoAuditoriaFomentoFlorestalValidar(); }
		}

		public override List<DependenciaLst> ObterDependencias(IEspecificidade especificidade)
		{
			List<DependenciaLst> retorno = new List<DependenciaLst>();
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.Dominialidade });
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.SilviculturaATV });

			return retorno;
		}

		public object Obter(int? tituloId)
		{
			try
			{
				return _da.Obter(tituloId ?? 0);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public ProtocoloEsp ObterProtocolo(int? tituloId)
		{
			return new ProtocoloEsp();
		}

		public void Salvar(IEspecificidade especificidade, BancoDeDados banco)
		{
			LaudoAuditoriaFomentoFlorestal laudo = especificidade as LaudoAuditoriaFomentoFlorestal;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(laudo, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		public void Excluir(int titulo, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Excluir(titulo, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		public object Deserialize(string input)
		{
			return Deserialize(input, typeof(LaudoAuditoriaFomentoFlorestal));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				Laudo laudo = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);
				DataEmissaoPorExtenso(laudo.Titulo);

				#region Anexos

				if (laudo.Anexos != null && laudo.Anexos.Count > 0)
				{
					foreach (AnexoPDF anexo in laudo.Anexos)
					{
						anexo.Arquivo.Conteudo = AsposeImage.RedimensionarImagem(
							File.ReadAllBytes(anexo.Arquivo.Caminho),
							11, eAsposeImageDimensao.Ambos);
					}
				}

				#endregion

				laudo.Dominialidade = new DominialidadePDF(new DominialidadeBus().ObterPorEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault()));

				laudo.SilviculturaATV = new SilviculturaATVPDF(new SilviculturaATVBus().ObterPorEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault()));

				return laudo;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public override IConfiguradorPdf ObterConfiguradorPdf(IEspecificidade especificidade)
		{
			ConfiguracaoDefault conf = new ConfiguracaoDefault();
			conf.AddLoadAcao((doc, dataSource) =>
			{

				Laudo laudo = dataSource as Laudo;
				List<Table> itenRemover = new List<Table>();
				conf.CabecalhoRodape = CabecalhoRodapeFactory.Criar(especificidade.Titulo.SetorId);

				if (String.IsNullOrEmpty(laudo.ResultadoQuais))
				{
					itenRemover.Add(doc.LastTable("«ResultadoQuais»"));
				}

				if (laudo.Anexos.Count <= 0)
				{
					doc.FindTable("«TableStart:Anexos»").RemovePageBreakAnterior();
					itenRemover.Add(doc.FindTable("«TableStart:Anexos»"));
				}

				if (laudo.AnaliseItens.Count <= 0)
				{
					itenRemover.Add(doc.LastTable("RESULTADO DA ANÁLISE"));
				}
				else
				{
					itenRemover.Add(doc.LastTable("Não realizada"));
				}



				if (String.IsNullOrWhiteSpace(laudo.Responsavel.NomeRazaoSocial))
				{
					itenRemover.Add(doc.LastTable("«Responsavel.NomeRazaoSocial»"));
				}
				else
				{
					itenRemover.Add(doc.LastTable("Não possui responsável técnico"));
				}

				AsposeExtensoes.RemoveTables(itenRemover);
			
			});

			return conf;
		}
	}
}