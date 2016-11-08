using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Business
{
	public class LaudoVistoriaLicenciamentoBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		#region Propriedades

		LaudoVistoriaLicenciamentoDa _da = new LaudoVistoriaLicenciamentoDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());

		#endregion

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Laudo; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new LaudoVistoriaLicenciamentoValidar(); }
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
			LaudoVistoriaLicenciamento laudo = especificidade as LaudoVistoriaLicenciamento;

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
			return Deserialize(input, typeof(LaudoVistoriaLicenciamento));
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
				}
				else
				{
					itenRemover.Add(doc.LastTable("Não possui responsável técnico"));
				}

				AsposeExtensoes.RemoveTables(itenRemover);
			});

			return conf;
		}

		public override int? ObterSituacaoAtividade(int? titulo)
		{
			LaudoVistoriaLicenciamento laudo = Obter(titulo) as LaudoVistoriaLicenciamento;

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