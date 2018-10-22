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

				var exploracoes = new ExploracaoFlorestalBus().ObterExploracoes(especificidade.Titulo.Id, Convert.ToInt32(especificidade.Titulo.Modelo));
				laudo.ExploracaoFlorestal = exploracoes.Select(x => new ExploracaoFlorestalPDF(x)).ToList();
				var parecerFavoravel = String.Join(", ", exploracoes.SelectMany(x => x.Exploracoes).Where(w => w.ParecerFavoravel == true).Select(y => y.Identificacao)?.ToList());
				laudo.ParecerFavoravel = string.IsNullOrWhiteSpace(parecerFavoravel) ? "" : String.Concat("(", parecerFavoravel, ")");
				var parecerDesfavoravel = String.Join(", ", exploracoes.SelectMany(x => x.Exploracoes).Where(w => w.ParecerFavoravel == false).Select(y => y.Identificacao)?.ToList());
				laudo.ParecerDesfavoravel = string.IsNullOrWhiteSpace(parecerDesfavoravel) ? "" : String.Concat("(", parecerDesfavoravel, ")");
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

				#region Exploração Florestal

				if (laudo.CaracterizacaoTipo == (int)eCaracterizacao.ExploracaoFlorestal)
				{
					if (laudo.ExploracaoFlorestal.Count <= 0)
					{
						itenRemover.Add(doc.LastTable("«TableStart:ExploracaoFlorestal"));
					}

					itenRemover.Add(doc.LastTable("«TableStart:QueimaControlada.QueimasContr"));
				}

				#endregion

				#region Queima Controlada

				else if (laudo.CaracterizacaoTipo == (int)eCaracterizacao.QueimaControlada)
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