using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCaracterizacao.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloProjetoDigital.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloRelatorioTecnico.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloRelatorioTecnico.Pdf
{
	public class PdfRelatorioTecnicoCredenciado : PdfPadraoRelatorio
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public String EsquemaBanco
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public MemoryStream Gerar(int id, int caracterizacaoTipo)
		{
			RelatorioTecnico dataSource = new RelatorioTecnico();
			ProjetoDigitalDa _daProjetoDigital = new ProjetoDigitalDa(EsquemaBanco);
			ProjetoDigital projetoDigital = _daProjetoDigital.Obter(id, simplificado: true);

			if (projetoDigital.Situacao == (int)eProjetoDigitalSituacao.EmElaboracao || projetoDigital.Situacao == (int)eProjetoDigitalSituacao.EmCorrecao)
			{
				dataSource = new RelatorioTecnicoDa().Obter(id);
			}
			else
			{
				dataSource = new RelatorioTecnicoDa().ObterHistorico(id, eProjetoDigitalSituacao.AguardandoImportacao);
			}

			ArquivoDocCaminho = @"~/Content/_pdfAspose/Relatorio_Tecnico_Parcial.doc";

			ObterArquivoTemplate();

			#region Configurar Assinantes

			ConfiguracaoDefault.TextoTagAssinante = "«Assinante.Nome»";
			ConfiguracaoDefault.TextoTagAssinantes1 = "«TableStart:Assinantes1»";
			ConfiguracaoDefault.TextoTagAssinantes2 = "«TableStart:Assinantes2»";

			#endregion

			dataSource.ProjetoDigital.Situacao = projetoDigital.Situacao;
			dataSource.ProjetoDigital.SituacaoTexto = projetoDigital.SituacaoTexto;

			ConfiguracaoDefault.ExibirSimplesConferencia = dataSource.ProjetoDigital.Situacao == (int)eProjetoDigitalSituacao.EmElaboracao;
			ConfigurarCabecarioRodape(0, true);

			dataSource.ProjetoDigital.Dependencias.Where(x =>
				x.DependenciaCaracterizacao == caracterizacaoTipo &&
				x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.Caracterizacao).ToList().ForEach(w =>
			{
				switch ((eCaracterizacao)w.DependenciaCaracterizacao)
				{
					case eCaracterizacao.Dominialidade:
						dataSource.Dominialidade = new DominialidadePDF(
							new DominialidadeRelatorioDa().Obter(id: w.DependenciaId, tid: w.DependenciaTid));
						break;

					case eCaracterizacao.UnidadeProducao:
						dataSource.UnidadeProducao = new UnidadeProducaoDa().Obter(projetoDigital.Id);
						break;

					case eCaracterizacao.UnidadeConsolidacao:
						dataSource.UnidadeConsolidacao = new UnidadeConsolidacaoDa().Obter(projetoDigital.Id);
						break;
				}
			});

			#region Configurar Tabelas

			ConfiguracaoDefault.AddLoadAcao((doc, dataSrc) =>
			{
				List<Table> tabelasRemover = new List<Table>();
				RelatorioTecnico data = (RelatorioTecnico)dataSrc;

				if (data.Dominialidade != null && data.Dominialidade.Id <= 0)
				{
					tabelasRemover.Add(doc.Last<Table>("«Dominialidade.ConfrontacaoLeste»"));
				}

				if (data.UnidadeProducao == null || data.UnidadeProducao.Id <= 0)
				{
					tabelasRemover.Add(doc.Last<Table>("«TableStart:UnidadeProducao.Produtores»"));
					tabelasRemover.Add(doc.Last<Table>("«TableStart:UnidadeProducao.Responsaveis»"));
					tabelasRemover.Add(doc.Last<Table>("«CodigoUP»"));
				}

				if (data.UnidadeConsolidacao == null || data.UnidadeConsolidacao.Id <= 0)
				{
					tabelasRemover.Add(doc.Last<Table>("«CapacidadeMes»"));
				}

				AsposeExtensoes.RemoveTables(tabelasRemover);
			});

			#endregion

			#region Assinantes

			AssinanteDefault assinante = null;

			if (dataSource.RequerimentoDigital.Interessado.Id > 0)
			{
				assinante = new AssinanteDefault();
				assinante.Nome = dataSource.RequerimentoDigital.Interessado.NomeRazaoSocial;
				assinante.TipoTexto = "Interessado";
				ConfiguracaoDefault.Assinantes.Add(assinante);
			}

			foreach (ResponsavelTecnicoRelatorio responsavel in dataSource.RequerimentoDigital.Responsaveis)
			{
				assinante = new AssinanteDefault();
				assinante.Nome = responsavel.NomeRazao;
				assinante.TipoTexto = "Responsável Técnico";
				ConfiguracaoDefault.Assinantes.Add(assinante);
			}

			#endregion

			return GerarPdf(dataSource);
		}
	}
}