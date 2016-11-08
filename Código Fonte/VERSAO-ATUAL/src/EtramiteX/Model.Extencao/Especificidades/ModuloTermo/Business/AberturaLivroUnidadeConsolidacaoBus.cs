using Aspose.Words.Tables;
using System;
using System.Linq;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Business
{
	public class AberturaLivroUnidadeConsolidacaoBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		AberturaLivroUnidadeConsolidacaoDa _da = new AberturaLivroUnidadeConsolidacaoDa();
		AberturaLivroUnidadeConsolidacaoValidar _validar = new AberturaLivroUnidadeConsolidacaoValidar();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Termo; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new AberturaLivroUnidadeConsolidacaoValidar(); }
		}

		public override List<DependenciaLst> ObterDependencias(IEspecificidade especificidade)
		{
			List<DependenciaLst> retorno = new List<DependenciaLst>();
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.UnidadeConsolidacao });

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

		public object ObterHistorico(int tituloId, int situacao)
		{
			try
			{
				return _da.ObterHistorico(tituloId, situacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public ProtocoloEsp ObterProtocolo(int? tituloId)
		{
			try
			{
				return _da.Obter(tituloId.Value).ProtocoloReq;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public void Salvar(IEspecificidade especificidade, BancoDeDados banco)
		{
			AberturaLivroUnidadeConsolidacao termo = especificidade as AberturaLivroUnidadeConsolidacao;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(termo, bancoDeDados);

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
			return Deserialize(input, typeof(AberturaLivroUnidadeConsolidacao));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				AberturaLivroUnidadeConsolidacao esp = especificidade as AberturaLivroUnidadeConsolidacao;
				Termo termo = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);

				UnidadeConsolidacao caracterizacao = new UnidadeConsolidacaoBus().ObterPorEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault());
				caracterizacao.Cultivares = _da.ObterCultivares(esp.Culturas.Select(x => x.Id).ToList(), esp.ProtocoloReq.Id);

				if (caracterizacao != null)
				{
					termo.UnidadeConsolidacao = new UnidadeConsolidacaoPDF(caracterizacao, termo.UnidadeConsolidacao.ResponsaveisEmpreendimento);
				}
				else
				{
					Validacao.Add(Mensagem.UnidadeConsolidacao.Inexistente);
				}

				DataEmissaoPorExtenso(termo.Titulo);

				return termo;
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
				Termo termo = dataSource as Termo;
				List<Table> itenRemover = new List<Table>();

				List<ResponsavelPDF> responsaveis = new List<ResponsavelPDF>();
				termo.UnidadeConsolidacao.ResponsaveisTecnicos.ForEach(resp =>
				{
					if (!responsaveis.Exists(x => x.CPFCNPJ == resp.CPFCNPJ))
					{
						responsaveis.Add(new ResponsavelPDF() { CPFCNPJ = resp.CPFCNPJ });
					}
				});

				if (responsaveis.Count < 2)
				{
					itenRemover.Add(doc.LastTable("dos profissionais"));
				}
				else
				{
					itenRemover.Add(doc.LastTable("do profissional"));
				}

				AsposeExtensoes.RemoveTables(itenRemover);
			});

			conf.CabecalhoRodape = CabecalhoRodapeFactory.Criar(especificidade.Titulo.SetorId);

			return conf;
		}

		#region Auxiliares

		public List<Lista> ObterCulturas(int protocoloId)
		{
			try
			{
				return _da.ObterCulturas(protocoloId);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return null;
		}

		#endregion
	}
}