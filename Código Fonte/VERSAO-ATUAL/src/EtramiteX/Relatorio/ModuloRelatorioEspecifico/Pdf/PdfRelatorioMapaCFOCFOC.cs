using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRelatorioEspecifico.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Entities.Interno.ModuloRelatorioEspecifico;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloRelatorioEspecifico.Pdf
{
	public class PdfRelatorioMapaCFOCFOC : PdfPadraoRelatorio
	{
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		
		RelatorioMapaDa _da = new RelatorioMapaDa();

		public MemoryStream Gerar(RelatorioMapaFiltroeResultado filtro)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Relatorio_Mapa_CFOCFOC.doc";

			RelatorioMapaFiltroeResultado dataSource = new RelatorioMapaFiltroeResultado();

			dataSource = _da.RelatorioCFOCFOC(filtro);

			dataSource.DataInicial = filtro.DataInicial;
			dataSource.DataFinal = filtro.DataFinal;
			dataSource.tipoRelatorio = filtro.tipoRelatorio;
			dataSource.LocalRelatorio = filtro.LocalRelatorio;
			dataSource.DataRelatorio = filtro.DataRelatorio;
			dataSource.NomeFuncionario = filtro.NomeFuncionario;

			SetorEndereco endereco = new CabecalhoRodapeDa().ObterEndSetor(filtro.IdSetor);
			dataSource.LocalRelatorio = endereco.MunicipioTexto;

			ObterArquivoTemplate();

			ConfigurarCabecarioRodape(0);

			return GerarPdf(dataSource);
		}
	}
}
