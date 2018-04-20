using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Pdf
{
	public class PdfFiscalizacao : PdfPadraoRelatorio
	{
		private FiscalizacaoDa _da = new FiscalizacaoDa();
		private AcompanhamentoDa _daAcompanhamento = new AcompanhamentoDa();
		private GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		private GerenciadorConfiguracao<ConfiguracaoFuncionario> _configFunc = new GerenciadorConfiguracao<ConfiguracaoFuncionario>(new ConfiguracaoFuncionario());

		public MemoryStream GerarAutoTermoFiscalizacao(int id, bool gerarTarja = true, BancoDeDados banco = null)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Auto_Termo_Fiscalizacao.docx";
			AutoInfracaoRelatorio dataSource = _da.ObterAutoTermoFiscalizacao(id, banco);

			dataSource.IsAI = dataSource.IsAI ?? AsposeData.Empty;
			dataSource.IsTAD = dataSource.IsTAD ?? AsposeData.Empty;
			dataSource.IsTEI = dataSource.IsTEI ?? AsposeData.Empty;

			ObterArquivoTemplate();

			dataSource.GovernoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyGovernoNome);
			dataSource.SecretariaNome = _configSys.Obter<String>(ConfiguracaoSistema.KeySecretariaNome);
			dataSource.OrgaoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoNome);
			Setor setor = _configFunc.Obter<List<Setor>>(ConfiguracaoFuncionario.KeySetores).Single(x => x.Id == dataSource.SetorId);
			dataSource.SetorNome = setor.Nome;
			dataSource.CodigoUnidadeConvenio = setor.UnidadeConvenio;

			string pathImg = HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logobrasao.jpg");
			dataSource.LogoBrasao = File.ReadAllBytes(pathImg);

			dataSource.LogoBrasao = AsposeImage.RedimensionarImagem(dataSource.LogoBrasao, 1);

			string pathImgLogo = HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logo_novo.png");
			dataSource.LogoNovo = File.ReadAllBytes(pathImgLogo);

			dataSource.LogoNovo = AsposeImage.RedimensionarImagem(dataSource.LogoNovo, 2);

			ConfigurarCabecarioRodape(dataSource.SetorId);

            ConfiguracaoDefault.ExibirSimplesConferencia = gerarTarja;

			return GerarPdf(dataSource);
		}

        public MemoryStream GerarInstrumentoUnicoFiscalizacao(int id, bool gerarTarja = true, BancoDeDados banco = null)
        {
            ArquivoDocCaminho = @"~/Content/_pdfAspose/Instrumento_Unico_Fiscalizacao.docx";
            InstrumentoUnicoFiscalizacaoRelatorio dataSource = _da.ObterInstrumentoUnicoFiscalizacao(id, banco);

            dataSource.IsDDSIA = dataSource.IsDDSIA ?? AsposeData.Empty;
            dataSource.IsDDSIV = dataSource.IsDDSIV ?? AsposeData.Empty;
            dataSource.IsDRNRE = dataSource.IsDRNRE ?? AsposeData.Empty;

            dataSource.InfrLeve = dataSource.InfrLeve ?? AsposeData.Empty;
            dataSource.InfrMedia = dataSource.InfrMedia ?? AsposeData.Empty;
            dataSource.InfrGrave = dataSource.InfrGrave ?? AsposeData.Empty;
            dataSource.InfrGravissima = dataSource.InfrGravissima ?? AsposeData.Empty;

            dataSource.TemAdvertencia = dataSource.TemAdvertencia ?? AsposeData.Empty;
            dataSource.TemMulta = dataSource.TemMulta ?? AsposeData.Empty;
            dataSource.TemApreensao = dataSource.TemApreensao ?? AsposeData.Empty;
            dataSource.TemInterdicao = dataSource.TemInterdicao ?? AsposeData.Empty;
            dataSource.TemOutra01 = dataSource.TemOutra01 ?? AsposeData.Empty;
            dataSource.TemOutra02 = dataSource.TemOutra02 ?? AsposeData.Empty;
            dataSource.TemOutra03 = dataSource.TemOutra03 ?? AsposeData.Empty;
            dataSource.TemOutra04 = dataSource.TemOutra04 ?? AsposeData.Empty;

            dataSource.IsInterditado = dataSource.IsInterditado ?? AsposeData.Empty;
            dataSource.IsEmbargado = dataSource.IsEmbargado ?? AsposeData.Empty;
            dataSource.IsDesinterditado= dataSource.IsDesinterditado?? AsposeData.Empty;
            dataSource.IsDesembargado= dataSource.IsDesembargado ?? AsposeData.Empty;

            ObterArquivoTemplate();

            dataSource.GovernoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyGovernoNome);
            dataSource.SecretariaNome = _configSys.Obter<String>(ConfiguracaoSistema.KeySecretariaNome);
            dataSource.OrgaoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoNome);
            Setor setor = _configFunc.Obter<List<Setor>>(ConfiguracaoFuncionario.KeySetores).Single(x => x.Id == dataSource.SetorId);
            //dataSource.SetorNome = setor.Nome;
            dataSource.DocumentoNome = "INSTRUMENTO ÚNICO DE FISCALIZAÇÃO";
            dataSource.CodigoUnidadeConvenio = setor.UnidadeConvenio;

            string pathImg = HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logobrasao.jpg");
            dataSource.LogoBrasao = File.ReadAllBytes(pathImg);

            dataSource.LogoBrasao = AsposeImage.RedimensionarImagem(dataSource.LogoBrasao, 1);

            string pathImgMarca = HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logomarca.png");
            dataSource.Logomarca = File.ReadAllBytes(pathImgMarca);

            dataSource.Logomarca = AsposeImage.RedimensionarImagem(dataSource.Logomarca, 2);

			string pathImgLogo = HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logo_novo.png");
			dataSource.LogoNovo = File.ReadAllBytes(pathImgLogo);

			dataSource.LogoNovo = AsposeImage.RedimensionarImagem(dataSource.LogoNovo, 2);

			ConfigurarCabecarioRodape(dataSource.SetorId);

            ConfiguracaoDefault.ExibirSimplesConferencia = gerarTarja;

            return GerarPdf(dataSource);
        }

		public MemoryStream GerarLaudoFiscalizacao(int id, bool gerarTarja = true, BancoDeDados banco = null)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Laudo_de_Fiscalizacao.docx";

			FiscalizacaoRelatorio dataSource = _da.Obter(id, banco);

			ConfiguracaoDefault.TextoTagAssinante = "«Assinante.Nome»";
			ConfiguracaoDefault.TextoTagAssinantes1 = "«TableStart:Assinantes1»";
			ConfiguracaoDefault.TextoTagAssinantes2 = "«TableStart:Assinantes2»";

			if (dataSource.ConsideracoesFinais != null &&
				dataSource.ConsideracoesFinais.Assinantes != null &&
				dataSource.ConsideracoesFinais.Assinantes.Count > 0)
			{
				var autor = dataSource.ConsideracoesFinais.Assinantes.First(x => x.Id == dataSource.UsuarioCadastro.Id);
				if (autor != null)
				{
					dataSource.ConsideracoesFinais.Assinantes.Remove(autor);
					dataSource.ConsideracoesFinais.Assinantes.Insert(0, autor);
				}

				
				ConfiguracaoDefault.Assinantes = dataSource.ConsideracoesFinais.Assinantes.Cast<IAssinante>().ToList();
			}

			ConfigurarCabecarioRodape(dataSource.LocalInfracao.SetorId);

			if (dataSource.ConsideracoesFinais.Anexos != null && dataSource.ConsideracoesFinais.Anexos.Count > 0)
			{
				foreach (ConsideracoesFinaisAnexoRelatorio anexo in dataSource.ConsideracoesFinais.Anexos)
				{
					anexo.Arquivo.Conteudo = AsposeImage.RedimensionarImagem(File.ReadAllBytes(anexo.Arquivo.Caminho), 11, eAsposeImageDimensao.Ambos);
				}
			}

			ObterArquivoTemplate();

			object objeto = dataSource;

			#region Remover

			this.ConfiguracaoDefault.AddLoadAcao((doc, a) =>
			{
				List<Table> itenRemover = new List<Table>();
				FiscalizacaoRelatorio fiscalizacao = (FiscalizacaoRelatorio)dataSource;

				fiscalizacao.OrgaoMunicipio = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoMunicipio);
				fiscalizacao.OrgaoUF = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoUf);

				if (fiscalizacao.Infracao.Campos.Count == 0)
				{					
					doc.Find<Row>("«TableStart:Infracao.Campos»").Remove();
				}

				if (fiscalizacao.Infracao.Perguntas.Count == 0)
				{
					doc.Find<Row>("«TableStart:Infracao.Perguntas»").Remove();
				}

				if (fiscalizacao.ConsideracoesFinais.Anexos.Count == 0)
				{
					itenRemover.Add(doc.Last<Table>("«TableStart:ConsideracoesFinais.Anexos»"));
					doc.RemovePageBreak();
				}

				#region Anexo Croqui da fiscalizacao

				/*List<ArquivoProjeto> arquivosProj = new ProjetoGeograficoBus().ObterArquivos(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(0), true).Where(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui).ToList();

				autorizacao.AnexosPdfs = arquivosProj.Cast<Arquivo>().ToList();

				//Obtendo Arquivos
				ArquivoBus _busArquivo = new ArquivoBus();

				for (int i = 0; i < autorizacao.AnexosPdfs.Count; i++)
				{
					autorizacao.AnexosPdfs[i] = _busArquivo.ObterDados(autorizacao.AnexosPdfs[i].Id.GetValueOrDefault(0));
				}*/

				#endregion

				AsposeExtensoes.RemoveTables(itenRemover);
			});

			#endregion Remover

            ConfiguracaoDefault.ExibirSimplesConferencia = gerarTarja; 

			return GerarPdf(dataSource);
		}

        public MemoryStream GerarLaudoFiscalizacaoNovo(int id, bool gerarTarja = true, BancoDeDados banco = null)
        {
            ArquivoDocCaminho = @"~/Content/_pdfAspose/Laudo_de_Fiscalizacao_Novo.docx";

            FiscalizacaoRelatorioNovo dataSource = _da.ObterNovo(id, banco);

            dataSource.Sessao = new Sessoes()
            {
                Empreendimento = AsposeData.Empty,
                Multa = AsposeData.Empty,
                InterdicaoEmbargo = AsposeData.Empty,
                Apreensao = AsposeData.Empty,
                OutrasPenalidades = AsposeData.Empty
            };

            ConfiguracaoDefault.TextoTagAssinante = "«Assinante.Nome»";
            ConfiguracaoDefault.TextoTagAssinantes1 = "«TableStart:Assinantes1»";
            ConfiguracaoDefault.TextoTagAssinantes2 = "«TableStart:Assinantes2»";

            if (dataSource.ConsideracoesFinais != null &&
                dataSource.ConsideracoesFinais.Assinantes != null &&
                dataSource.ConsideracoesFinais.Assinantes.Count > 0)
            {
                var autor = dataSource.ConsideracoesFinais.Assinantes.First(x => x.Id == dataSource.UsuarioCadastro.Id);
                if (autor != null)
                {
                    dataSource.ConsideracoesFinais.Assinantes.Remove(autor);
                    dataSource.ConsideracoesFinais.Assinantes.Insert(0, autor);
                }


                ConfiguracaoDefault.Assinantes = dataSource.ConsideracoesFinais.Assinantes.Cast<IAssinante>().ToList();
            }

            ConfigurarCabecarioRodape(dataSource.LocalInfracao.SetorId);

            if (dataSource.ConsideracoesFinais.Anexos != null && dataSource.ConsideracoesFinais.Anexos.Count > 0)
            {
                foreach (ConsideracoesFinaisAnexoRelatorio anexo in dataSource.ConsideracoesFinais.Anexos)
                {
                    anexo.Arquivo.Conteudo = AsposeImage.RedimensionarImagem(File.ReadAllBytes(anexo.Arquivo.Caminho), 11, eAsposeImageDimensao.Ambos);
                }
            }

            ObterArquivoTemplate();

            object objeto = dataSource;

            #region Remover

            this.ConfiguracaoDefault.AddLoadAcao((doc, a) =>
            {
                List<Table> itenRemover = new List<Table>();
                FiscalizacaoRelatorioNovo fiscalizacao = (FiscalizacaoRelatorioNovo)dataSource;


				CabecalhoRodapeDa _daCabecalho = new CabecalhoRodapeDa();
				SetorEndereco endereco = _daCabecalho.ObterEndSetor(fiscalizacao.LocalInfracao.SetorId);
				fiscalizacao.OrgaoMunicipio = endereco.MunicipioTexto;
				fiscalizacao.OrgaoUF = endereco.EstadoTexto;

				//fiscalizacao.OrgaoMunicipio = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoMunicipio);
				//fiscalizacao.OrgaoUF = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoUf);

				if (fiscalizacao.Infracao.Campos.Count == 0)
                {
                    doc.Find<Row>("«TableStart:Infracao.Campos»").Remove();
                }

                if (fiscalizacao.Infracao.Perguntas.Count == 0)
                {
                    doc.Find<Row>("«TableStart:Infracao.Perguntas»").Remove();
                }

                if (fiscalizacao.ConsideracoesFinais.Anexos.Count == 0)
                {
                    itenRemover.Add(doc.Last<Table>("«TableStart:ConsideracoesFinais.Anexos»"));
                    doc.RemovePageBreak();
                }

                //Remove as seções que não foram preenchidas
                if (fiscalizacao.Multa == null)
                {
                    doc.Find<Row>("«Secao.Multa»").Remove();
                }
                else
                {
                    
                }
                if (fiscalizacao.ObjetoInfracao == null)
                {
                    doc.Find<Row>("«Secao.InterdicaoEmbargo»").Remove();
                }
                if (fiscalizacao.MaterialApreendido == null)
                {
                    doc.Find<Row>("«Secao.Apreensao»").Remove();
                }
                else
                {
                    if (fiscalizacao.MaterialApreendido.ProdutosDestinacoes == null || fiscalizacao.MaterialApreendido.ProdutosDestinacoes.Count == 0)
                    {
                        doc.Find<Row>("«TableStart:MaterialApreendido.ProdutosDestinacoes»").Remove();
                    }
                }
                if (fiscalizacao.OutrasPenalidades == null)
                {
                    doc.Find<Row>("«Secao.OutrasPenalidades»").Remove();
                }
                if (fiscalizacao.LocalInfracao.EmpreendimentoId == 0)
                {
                    doc.Find<Row>("«Secao.Empreendimento»").Remove();
                }
                
                AsposeExtensoes.RemoveTables(itenRemover);
            });

            #endregion Remover

            ConfiguracaoDefault.ExibirSimplesConferencia = gerarTarja;

            return GerarPdf(dataSource);
        }

		public MemoryStream GerarLaudoAcompanhamentoFiscalizacao(int id, bool gerarTarja = true, BancoDeDados banco = null)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Laudo_de_Fiscalizacao_Acompanhamento.docx";

			AcompanhamentoRelatorio acompanhamento = _daAcompanhamento.Obter(id, banco: banco);
			int historicoId = _da.ObterHistoricoIdConcluido(acompanhamento.FiscalizacaoId, banco);
			FiscalizacaoRelatorio dataSource = _da.ObterHistorico(historicoId, banco);
			dataSource.Acompanhamento = acompanhamento;

			ConfiguracaoDefault.TextoTagAssinante = "«Assinante.Nome»";
			ConfiguracaoDefault.TextoTagAssinantes1 = "«TableStart:Assinantes1»";
			ConfiguracaoDefault.TextoTagAssinantes2 = "«TableStart:Assinantes2»";

			if (dataSource.Acompanhamento != null &&
				dataSource.Acompanhamento.Assinantes != null &&
				dataSource.Acompanhamento.Assinantes.Count > 0)
			{
				var autor = dataSource.Acompanhamento.Assinantes.First(x => x.Id == dataSource.Acompanhamento.AgenteId);
				if (autor != null)
				{
					dataSource.Acompanhamento.Assinantes.Remove(autor);
					dataSource.Acompanhamento.Assinantes.Insert(0, autor);
				}

				ConfiguracaoDefault.Assinantes = dataSource.Acompanhamento.Assinantes.Cast<IAssinante>().ToList();
			}

			ConfigurarCabecarioRodape(dataSource.Acompanhamento.SetorId);

			if (dataSource.Acompanhamento.Anexos != null && dataSource.Acompanhamento.Anexos.Count > 0)
			{
				foreach (ConsideracoesFinaisAnexoRelatorio anexo in dataSource.Acompanhamento.Anexos)
				{
					anexo.Arquivo.Conteudo = AsposeImage.RedimensionarImagem(File.ReadAllBytes(anexo.Arquivo.Caminho), 11, eAsposeImageDimensao.Ambos);
				}
			}

			ObterArquivoTemplate();

			object objeto = dataSource;

			#region Remover

			this.ConfiguracaoDefault.AddLoadAcao((doc, a) =>
			{
				List<Table> itenRemover = new List<Table>();
				FiscalizacaoRelatorio fiscalizacao = (FiscalizacaoRelatorio)dataSource;

				fiscalizacao.OrgaoMunicipio = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoMunicipio);
				fiscalizacao.OrgaoUF = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoUf);

				if (fiscalizacao.Infracao.Campos.Count == 0)
				{
					doc.Find<Row>("«TableStart:Infracao.Campos»").Remove();
				}

				if (fiscalizacao.Infracao.Perguntas.Count == 0)
				{
					doc.Find<Row>("«TableStart:Infracao.Perguntas»").Remove();
				}

				if (fiscalizacao.Acompanhamento.Anexos.Count == 0)
				{
					itenRemover.Add(doc.Last<Table>("«TableStart:Acompanhamento.Anexos»"));
					doc.RemovePageBreak();
				}

				AsposeExtensoes.RemoveTables(itenRemover);
			});

			#endregion Remover

            ConfiguracaoDefault.ExibirSimplesConferencia = gerarTarja;

			return GerarPdf(dataSource);
		}

		public MemoryStream GerarAutoTermoFiscalizacaoHistorico(int historicoId, bool gerarTarja = true, BancoDeDados banco = null)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Auto_Termo_Fiscalizacao.docx";
			AutoInfracaoRelatorio dataSource = _da.ObterAutoTermoFiscalizacaoHistorico(historicoId, banco);

			if (String.IsNullOrEmpty(dataSource.IsAI) &&
				String.IsNullOrEmpty(dataSource.IsTAD) &&
				String.IsNullOrEmpty(dataSource.IsTEI))
			{
				return null;
			}

			dataSource.IsAI = dataSource.IsAI ?? AsposeData.Empty;
			dataSource.IsTAD = dataSource.IsTAD ?? AsposeData.Empty;
			dataSource.IsTEI = dataSource.IsTEI ?? AsposeData.Empty;

			ObterArquivoTemplate();

			dataSource.GovernoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyGovernoNome);
			dataSource.SecretariaNome = _configSys.Obter<String>(ConfiguracaoSistema.KeySecretariaNome);
			dataSource.OrgaoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoNome);
			Setor setor = _configFunc.Obter<List<Setor>>(ConfiguracaoFuncionario.KeySetores).Single(x => x.Id == dataSource.SetorId);
			dataSource.SetorNome = setor.Nome;
			dataSource.CodigoUnidadeConvenio = setor.UnidadeConvenio;

			string pathImg = HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logobrasao.jpg");
			dataSource.LogoBrasao = File.ReadAllBytes(pathImg);

			dataSource.LogoBrasao = AsposeImage.RedimensionarImagem(dataSource.LogoBrasao, 1);

			ConfigurarCabecarioRodape(dataSource.SetorId);

            ConfiguracaoDefault.ExibirSimplesConferencia = gerarTarja;

			return GerarPdf(dataSource);
		}

		public MemoryStream GerarLaudoFiscalizacaoHistorico(int historicoId, bool gerarTarja = true, BancoDeDados banco = null)
		{
			ArquivoDocCaminho = @"~/Content/_pdfAspose/Laudo_de_Fiscalizacao.docx";

			FiscalizacaoRelatorio dataSource = _da.ObterHistorico(historicoId, banco);

			ConfiguracaoDefault.TextoTagAssinante = "«Assinante.Nome»";
			ConfiguracaoDefault.TextoTagAssinantes1 = "«TableStart:Assinantes1»";
			ConfiguracaoDefault.TextoTagAssinantes2 = "«TableStart:Assinantes2»";

			if (dataSource.ConsideracoesFinais != null &&
				dataSource.ConsideracoesFinais.Assinantes != null &&
				dataSource.ConsideracoesFinais.Assinantes.Count > 0)
			{
				var autor = dataSource.ConsideracoesFinais.Assinantes.SingleOrDefault(x => x.Id == dataSource.UsuarioCadastro.Id);
				if (autor != null)
				{
					dataSource.ConsideracoesFinais.Assinantes.Remove(autor);
					dataSource.ConsideracoesFinais.Assinantes.Insert(0, autor);
				}

				ConfiguracaoDefault.Assinantes = dataSource.ConsideracoesFinais.Assinantes.Cast<IAssinante>().ToList();
			}

			ConfigurarCabecarioRodape(dataSource.LocalInfracao.SetorId);

			if (dataSource.ConsideracoesFinais.Anexos != null && dataSource.ConsideracoesFinais.Anexos.Count > 0)
			{
				foreach (ConsideracoesFinaisAnexoRelatorio anexo in dataSource.ConsideracoesFinais.Anexos)
				{
					try
					{
						anexo.Arquivo.Conteudo = AsposeImage.RedimensionarImagem(File.ReadAllBytes(anexo.Arquivo.Caminho), 11, eAsposeImageDimensao.Ambos);
					}
					catch
					{
						Debug.Write(anexo.Arquivo.Caminho);
					}
				}
				//dataSource.ConsideracoesFinais.Anexos.Clear();
			}

			ObterArquivoTemplate();

			object objeto = dataSource;

			#region Remover

			this.ConfiguracaoDefault.AddLoadAcao((doc, a) =>
			{
				List<Table> itenRemover = new List<Table>();
				FiscalizacaoRelatorio fiscalizacao = (FiscalizacaoRelatorio)dataSource;

				fiscalizacao.OrgaoMunicipio = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoMunicipio);
				fiscalizacao.OrgaoUF = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoUf);

				if (fiscalizacao.Infracao.Campos.Count == 0)
				{
					doc.Find<Row>("«TableStart:Infracao.Campos»").Remove();
				}

				if (fiscalizacao.Infracao.Perguntas.Count == 0)
				{
					doc.Find<Row>("«TableStart:Infracao.Perguntas»").Remove();
				}

				if (fiscalizacao.ConsideracoesFinais.Anexos.Count == 0)
				{
					itenRemover.Add(doc.Last<Table>("«TableStart:ConsideracoesFinais.Anexos»"));
					doc.RemovePageBreak();
				}

				#region Anexo Croqui da fiscalizacao

				/*List<ArquivoProjeto> arquivosProj = new ProjetoGeograficoBus().ObterArquivos(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(0), true).Where(x => x.Tipo == (int)eProjetoGeograficoArquivoTipo.Croqui).ToList();

				autorizacao.AnexosPdfs = arquivosProj.Cast<Arquivo>().ToList();

				//Obtendo Arquivos
				ArquivoBus _busArquivo = new ArquivoBus();

				for (int i = 0; i < autorizacao.AnexosPdfs.Count; i++)
				{
					autorizacao.AnexosPdfs[i] = _busArquivo.ObterDados(autorizacao.AnexosPdfs[i].Id.GetValueOrDefault(0));
				}*/

				#endregion

				AsposeExtensoes.RemoveTables(itenRemover);
			});

			#endregion Remover

            ConfiguracaoDefault.ExibirSimplesConferencia = gerarTarja;

			return GerarPdf(dataSource);
		}

		#region Gerar Pdf do Historico (Passivo)

		public MemoryStream GerarPassivoZip()
		{
			FiscalizacaoDa da = new FiscalizacaoDa();
			List<FiscalizacaoRelatorio> lstFiscalizacao = da.ObterHistoricoConcluidos();

			MemoryStream ms = null;

			string appPath = Path.GetTempPath() + "IDAF_PDF_FISC\\";


			if (!Directory.Exists(appPath))
			{
				Directory.CreateDirectory(appPath);
			}

			foreach (var item in lstFiscalizacao)
			{
				//Fiscalizacao fiscalizacao = fiscBus.ObterHistorico(item.HistoricoId);
				PdfFiscalizacao pdf = new PdfFiscalizacao();

				ms = pdf.GerarAutoTermoFiscalizacaoHistorico(item.HistoricoId, false);
				if (ms != null)
				{
					//ms.Seek(0, SeekOrigin.Begin);
					//ms = PdfMetodosAuxiliares.TarjaEncerrado(ms, "CANCELADO " + fiscalizacao.SituacaoAtualData.DataTexto);
					File.WriteAllBytes(String.Format("{0}{1}_{2}_{3}_auto.pdf", appPath, item.Id, item.HistoricoId, item.DataConclusao), ms.ToArray());
					ms.Close();
					ms.Dispose();
				}

				ms = pdf.GerarLaudoFiscalizacaoHistorico(item.HistoricoId, false);
				//ms.Seek(0, SeekOrigin.Begin);
				//ms = PdfMetodosAuxiliares.TarjaEncerrado(ms, "CANCELADO " + fiscalizacao.SituacaoAtualData.DataTexto);
				File.WriteAllBytes(String.Format("{0}{1}_{2}_{3}_laudo.pdf", appPath, item.Id, item.HistoricoId, item.DataConclusao), ms.ToArray());
				ms.Close();
				ms.Dispose();
			}

			ArquivoZip zipBus = new ArquivoZip();
			MemoryStream msZip = zipBus.Create(appPath);

			Directory.GetFiles(appPath).ToList().ForEach(x => File.Delete(x));
			msZip.Seek(0, SeekOrigin.Begin);

			return msZip;
		}

		public MemoryStream GerarPassivoTarjaZip()
		{
			FiscalizacaoDa da = new FiscalizacaoDa();
			List<FiscalizacaoRelatorio> lstFiscalizacao = da.ObterHistoricoTarja();

			MemoryStream ms = null;

			string appPath = Path.GetTempPath() + "IDAF_PDF_FISC_TARJA\\";


			if (!Directory.Exists(appPath))
			{
				Directory.CreateDirectory(appPath);
			}

			foreach (var item in lstFiscalizacao)
			{
				//Fiscalizacao fiscalizacao = fiscBus.ObterHistorico(item.HistoricoId);
				PdfFiscalizacao pdf = new PdfFiscalizacao();

				ms = pdf.GerarLaudoFiscalizacaoHistorico(item.HistoricoId, false);
				//ms.Seek(0, SeekOrigin.Begin);
				//ms = PdfMetodosAuxiliares.TarjaEncerrado(ms, "CANCELADO " + fiscalizacao.SituacaoAtualData.DataTexto);
				File.WriteAllBytes(String.Format("{0}{1}_{2}_{3}_laudo.pdf", appPath, item.Id, item.HistoricoId, item.DataConclusao), ms.ToArray());
				ms.Close();
				ms.Dispose();
			}

			ArquivoZip zipBus = new ArquivoZip();
			MemoryStream msZip = new MemoryStream();//zipBus.Create(appPath);

			//Directory.GetFiles(appPath).ToList().ForEach(x => System.IO.File.Delete(x));
			msZip.Seek(0, SeekOrigin.Begin);

			return msZip;
		}

		public string GerarPassivo()
		{
			List<FiscalizacaoRelatorio> lstFiscalizacao = _da.ObterHistoricoConcluidos();

			string appPath = string.Empty;
			/*string appPath = System.IO.Path.GetTempPath() + "IDAF_PDF_FISC\\";
			if (!Directory.Exists(appPath))
			{
				System.IO.Directory.CreateDirectory(appPath);
			}*/

			ArquivoBus arqBus = new ArquivoBus(eExecutorTipo.Interno);
			ArquivoDa arquivoDa = new ArquivoDa();
			Arquivo arqAuto = null;
			Arquivo arqLaudo = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				GerenciadorTransacao.ObterIDAtual();
				bancoDeDados.IniciarTransacao();

				foreach (var item in lstFiscalizacao)
				{
					PdfFiscalizacao pdf = new PdfFiscalizacao();
					arqAuto = new Arquivo();
					arqLaudo = new Arquivo();

					MemoryStream ms = pdf.GerarAutoTermoFiscalizacaoHistorico(item.HistoricoId, false);
					if (ms != null)
					{
						arqAuto.Buffer = ms;
						arqAuto.Nome = String.Format("{0}{1}_{2}_{3}_auto", appPath, item.Id, item.HistoricoId, item.DataConclusao);
						arqAuto.Extensao = ".pdf";
						arqAuto.ContentType = "application/pdf";

						arqBus.Salvar(arqAuto);
						arquivoDa.Salvar(arqAuto, null, "Path de atualização fiscalização", "Path", null, GerenciadorTransacao.ObterIDAtual(), bancoDeDados);

						//System.IO.File.WriteAllBytes(String.Format("{0}{1}_{2}_{3}_auto.pdf", appPath, item.Id, item.HistoricoId, item.DataConclusao), ms.ToArray());
						ms.Close();
						ms.Dispose();
					}

					arqLaudo.Buffer = pdf.GerarLaudoFiscalizacaoHistorico(item.HistoricoId, false);
					arqLaudo.Nome = String.Format("{0}{1}_{2}_{3}_laudo", appPath, item.Id, item.HistoricoId, item.DataConclusao);
					arqLaudo.Extensao = ".pdf";
					arqLaudo.ContentType = "application/pdf";
					//System.IO.File.WriteAllBytes(String.Format("{0}{1}_{2}_{3}_laudo.pdf", appPath, item.Id, item.HistoricoId, item.DataConclusao), ms.ToArray());

					arqBus.Salvar(arqLaudo);
					arquivoDa.Salvar(arqLaudo, null, "Path de atualização fiscalização", "Path", null, GerenciadorTransacao.ObterIDAtual(), bancoDeDados);

					arqLaudo.Buffer.Close();
					arqLaudo.Buffer.Dispose();

					_da.AtualizarHistorico(item.HistoricoId, arqLaudo.Id, arqAuto.Id, bancoDeDados);
				}

				_da.CorrigirHistoricoSubSequente(bancoDeDados);

				bancoDeDados.Commit();
			}

			return appPath;
		}

		#endregion
	}
}