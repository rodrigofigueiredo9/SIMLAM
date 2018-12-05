using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloQueimaControlada.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilvicultura.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using System.Collections;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Business
{
	public class LaudoVistoriaFlorestalBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		#region Propriedades

		LaudoVistoriaFlorestalDa _da = new LaudoVistoriaFlorestalDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());

		#endregion

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Laudo; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new LaudoVistoriaFlorestalValidar(); }
		}

		public override List<DependenciaLst> ObterDependencias(IEspecificidade especificidade)
		{
			LaudoVistoriaFlorestal laudo = especificidade as LaudoVistoriaFlorestal;

			List<DependenciaLst> retorno = new List<DependenciaLst>();
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.Dominialidade });
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = laudo.Caracterizacao });

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
			LaudoVistoriaFlorestal laudo = especificidade as LaudoVistoriaFlorestal;

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
			return Deserialize(input, typeof(LaudoVistoriaFlorestal));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				Laudo laudo = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);
				DataEmissaoPorExtenso(laudo.Titulo);

				#region Anexos

				laudo.AnexosPdfs = laudo.Anexos
					.Select(x => x.Arquivo)
					.Where(x => (!String.IsNullOrEmpty(x.Nome) && new FileInfo(x.Nome).Extension.ToLower().IndexOf("pdf") > -1)).ToList();

				laudo.Anexos.RemoveAll(anexo =>
					String.IsNullOrEmpty(anexo.Arquivo.Extensao) ||
					!((new[] { ".jpg", ".gif", ".png", ".bmp" }).Any(x => anexo.Arquivo.Extensao.ToLower() == x)));

				if (laudo.Anexos != null && laudo.Anexos.Count>0)
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

				if (laudo.CaracterizacaoTipo == (int)eCaracterizacao.ExploracaoFlorestal)
				{
					var exploracoes = new ExploracaoFlorestalBus().ObterExploracoes(especificidade.Titulo.Id, (int)eTituloModeloCodigo.LaudoVistoriaFlorestal);
					laudo.ExploracaoFlorestalList = exploracoes.Select(x => new ExploracaoFlorestalPDF(x)).ToList();
					var parecerFavoravel = new ArrayList();
					var parecerDesfavoravel = new ArrayList();
					foreach (var exploracao in exploracoes)
					{
						if (exploracao.Exploracoes.Where(x => x.ParecerFavoravel == true)?.ToList().Count > 0)
							parecerFavoravel.Add(String.Concat(exploracao.CodigoExploracaoTexto, " (", String.Join(", ", exploracao.Exploracoes.Where(x => x.ParecerFavoravel == true).Select(x => x.Identificacao)?.ToList()), ")"));
						if (exploracao.Exploracoes.Where(x => x.ParecerFavoravel == false)?.ToList().Count > 0)
							parecerDesfavoravel.Add(String.Concat(exploracao.CodigoExploracaoTexto, " (", String.Join(", ", exploracao.Exploracoes.Where(x => x.ParecerFavoravel == false).Select(x => x.Identificacao)?.ToList()), ")"));
					}
					laudo.ParecerFavoravel = parecerFavoravel.Count > 0 ? String.Join(", ", parecerFavoravel?.ToArray()) : "";
					laudo.ParecerDesfavoravel = parecerDesfavoravel.Count > 0 ? String.Join(", ", parecerDesfavoravel?.ToArray()) : "";
				}
				else
					laudo.ExploracaoFlorestal = new ExploracaoFlorestalPDF(new ExploracaoFlorestalBus().ObterPorEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault()));
				laudo.QueimaControlada = new QueimaControladaPDF(new QueimaControladaBus().ObterPorEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault()));

				laudo.Silvicultura = new SilviculturaPDF(new SilviculturaBus().ObterPorEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault()));
				
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

				switch ((eCaracterizacao)laudo.CaracterizacaoTipo)
				{
					case eCaracterizacao.ExploracaoFlorestal:
					case eCaracterizacao.QueimaControlada:
						itenRemover.Add(doc.FindTable("«Silvicultura.AreaTotalHa»"));
						itenRemover.Add(doc.FindTable("«TableStart:QueimaControlada.TipoQueima»"));
						break;
					case eCaracterizacao.Silvicultura:
						//Removendo Exploracao
						itenRemover.Add(doc.FindTable("FINALIDADE DA EXPLORAÇÃO"));
						
						//Removendo Queima Controlada
						itenRemover.Add(doc.LastTable("«TableStart:QueimaControlada.QueimasContr"));
						break;
				}

				#region Queima Controlada

				if (laudo.CaracterizacaoTipo == (int)eCaracterizacao.QueimaControlada ||
				(laudo.Titulo.ModeloSigla == "LVQC" && laudo.CaracterizacaoTipo == (int)eCaracterizacao.ExploracaoFlorestal))
				{
					if (laudo.QueimaControlada.QueimasControladas.Count <= 0)
					{
						itenRemover.Add(doc.LastTable("«TableStart:QueimaControlada.QueimasContr"));
					}

					itenRemover.Add(doc.LastTable("«TableStart:ExploracaoFlorestal.CorteRaso"));
					itenRemover.Add(doc.LastTable("«TableStart:ExploracaoFlorestal.CorteSele"));
					itenRemover.Add(doc.FindTable("FINALIDADE DA EXPLORAÇÃO"));
				}
				#endregion

				#region Exploração Florestal

				else if (laudo.CaracterizacaoTipo == (int)eCaracterizacao.ExploracaoFlorestal)
				{
					if (laudo.ExploracaoFlorestalList.Count <= 0)
						itenRemover.Add(doc.LastTable("«TableStart:ExploracaoFlorestalList"));

					if (string.IsNullOrWhiteSpace(laudo.ParecerFavoravel))
					{
						laudo.ParecerFavoravel = "«remover»";
						laudo.DescricaoParecer = "«remover»";
					}

					if (string.IsNullOrWhiteSpace(laudo.ParecerDesfavoravel))
					{
						laudo.ParecerDesfavoravel = "«remover»";
						laudo.DescricaoParecerDesfavoravel = "«remover»";
					}
				}

				#endregion

				#region Silvicultura

				else if (laudo.CaracterizacaoTipo == (int)eCaracterizacao.Silvicultura)
				{
					if (laudo.Silvicultura.Culturas.Count <= 0)
					{
						//itenRemover.Add(doc.LastTable("«TableStart:QueimaControlada.QueimasContr"));
					}
				}

				#endregion

				if (string.IsNullOrEmpty(laudo.Restricao))
				{
					itenRemover.Add(doc.LastTable("«Restricao»"));
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
				}else
				{
					itenRemover.Add(doc.LastTable("Não possui responsável técnico"));
				}

				AsposeExtensoes.RemoveTables(itenRemover);
			});

			return conf;
		}

		public override int? ObterSituacaoAtividade(int? titulo)
		{
			LaudoVistoriaFlorestal laudo = Obter(titulo) as LaudoVistoriaFlorestal;

			if (laudo.Conclusao == (int)eEspecificidadeConclusao.Desfavoravel)
			{
				return (int)eAtividadeSituacao.Indeferida;
			}

			return null;
		}

		public List<CaracterizacaoLst> ObterCaracterizacoes(int empreendimento)
		{
			List<CaracterizacaoLst> caracterizacoesRetorno = new List<CaracterizacaoLst>();
			List<CaracterizacaoLst> caracterizacoes = _caracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
			caracterizacoes = caracterizacoes.Where(x => x.Id == (int)eCaracterizacao.ExploracaoFlorestal || x.Id == (int)eCaracterizacao.QueimaControlada || x.Id == (int)eCaracterizacao.Silvicultura).ToList();
			int caracterizacao = 0;

			CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			CaracterizacaoValidar caracterizacaoValidar = new CaracterizacaoValidar();

			foreach (CaracterizacaoLst item in caracterizacoes)
			{
				caracterizacao = caracterizacaoBus.Existe(empreendimento, (eCaracterizacao)item.Id);

				if (caracterizacao <= 0)
				{
					continue;
				}

				List<Dependencia> dependencias = caracterizacaoBus.ObterDependencias(caracterizacao, (eCaracterizacao)item.Id, eCaracterizacaoDependenciaTipo.Caracterizacao);
				string retorno = caracterizacaoValidar.DependenciasAlteradas(empreendimento, item.Id, eCaracterizacaoDependenciaTipo.Caracterizacao, dependencias);

				if (!string.IsNullOrEmpty(retorno))
				{
					continue;
				}

				caracterizacoesRetorno.Add(item);
			}

			return caracterizacoesRetorno;
		}

		public static List<string> ObterFinalidades(int? finalidades)
		{
			List<string> finalidadesSelecionadas = new List<string>();
			GerenciadorConfiguracao _configSys = new GerenciadorConfiguracao(new ConfiguracaoSistema());

			foreach (Finalidade finalidade in _configSys.Obter<List<Finalidade>>(ConfiguracaoSistema.KeyFinalidadesExploracao))
			{
				if ((finalidades & finalidade.Codigo) > 0)
				{
					finalidadesSelecionadas.Add(finalidade.Texto);
				}
			}

			return finalidadesSelecionadas;
		}
	}
}