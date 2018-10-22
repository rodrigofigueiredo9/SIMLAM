using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloAutorizacao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloAutorizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloAutorizacao.Business
{
	public class AutorizacaoExploracaoFlorestalBus: EspecificidadeBusBase, IEspecificidadeBus
	{
		AutorizacaoExploracaoFlorestalDa _da = new AutorizacaoExploracaoFlorestalDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Autorizacao; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new AutorizacaoExploracaoFlorestalValidar(); }
		}

		public override List<DependenciaLst> ObterDependencias(IEspecificidade especificidade)
		{
			List<DependenciaLst> retorno = new List<DependenciaLst>();
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.Dominialidade });
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.ExploracaoFlorestal });
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
			AutorizacaoExploracaoFlorestal autorizacao = especificidade as AutorizacaoExploracaoFlorestal;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(autorizacao, bancoDeDados);

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
			return Deserialize(input, typeof(AutorizacaoExploracaoFlorestal));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				Autorizacao autorizacao = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);
				DataEmissaoPorExtenso(autorizacao.Titulo);

				#region Anexos

				autorizacao.AnexosPdfs = autorizacao.Anexos
					.Select(x => x.Arquivo)
					.Where(x => (!String.IsNullOrEmpty(x.Extensao) && x.Extensao.ToLower().IndexOf("pdf") > -1)).ToList();

				autorizacao.Anexos.RemoveAll(anexo =>
					String.IsNullOrEmpty(anexo.Arquivo.Extensao) ||
					!((new[] { ".jpg", ".gif", ".png", ".bmp" }).Any(x => anexo.Arquivo.Extensao.ToLower() == x)));

				if (autorizacao.Anexos != null && autorizacao.Anexos.Count > 0)
				{
					foreach (AnexoPDF anexo in autorizacao.Anexos)
					{
						anexo.Arquivo.Conteudo = AsposeImage.RedimensionarImagem(
								File.ReadAllBytes(anexo.Arquivo.Caminho),
								11, eAsposeImageDimensao.Ambos);
					}
				}

				#endregion

				autorizacao.Dominialidade = new DominialidadePDF(new DominialidadeBus().ObterPorEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault()));
				var exploracoes = new ExploracaoFlorestalBus().ObterExploracoes(especificidade.Titulo.Id, Convert.ToInt32(especificidade.Titulo.Modelo));
				autorizacao.ExploracaoFlorestal = exploracoes.Where(x => x.TipoExploracao != (int)eTipoExploracao.CAI).Select(x => new ExploracaoFlorestalAutorizacaoPDF(x)).ToList();
				autorizacao.ExploracaoFlorestalPonto = exploracoes.Where(x => x.TipoExploracao == (int)eTipoExploracao.CAI).SelectMany(y => y.Exploracoes).Select(x => new ExploracaoFlorestalAutorizacaoDetalhePDF(x)).ToList();
				autorizacao.TotalPonto = autorizacao.ExploracaoFlorestalPonto.Sum(x => Convert.ToDouble(string.IsNullOrWhiteSpace(x.QuantidadeArvores) ? "0" : x.QuantidadeArvores)).ToString("N2");
				var produtos = exploracoes.SelectMany(x => x.Exploracoes).SelectMany(x => x.Produtos).Select(x => new ExploracaoFlorestalExploracaoProdutoPDF(x)).ToList();
				autorizacao.Produtos = produtos.GroupBy(x => new { x.Nome, x.Especie }, x => x.Quantidade, (key, g) => new ExploracaoFlorestalAutorizacaoProdutoPDF() {
					Nome = key.Nome,
					Especie = key.Especie,
					Quantidade = g.Sum(x => x)
				}).ToList();

				return autorizacao;
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
				Autorizacao autorizacao = dataSource as Autorizacao;
				List<Table> itenRemover = new List<Table>();
				conf.CabecalhoRodape = CabecalhoRodapeFactory.Criar(especificidade.Titulo.SetorId);

				if (autorizacao.ExploracaoFlorestal.Count <= 0)
				{
					itenRemover.Add(doc.LastTable("«TableStart:ExploracaoFlorestal»"));
				}

				if (autorizacao.ExploracaoFlorestalPonto.Count <= 0)
				{
					itenRemover.Add(doc.LastTable("«TableStart:ExploracaoFlorestalPonto»"));
				}

				AsposeExtensoes.RemoveTables(itenRemover);
			});

			return conf;
		}
	}
}