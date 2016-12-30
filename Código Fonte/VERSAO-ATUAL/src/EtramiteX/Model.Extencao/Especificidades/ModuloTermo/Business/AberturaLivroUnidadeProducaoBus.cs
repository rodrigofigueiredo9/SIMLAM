using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Business
{
	public class AberturaLivroUnidadeProducaoBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		AberturaLivroUnidadeProducaoDa _da = new AberturaLivroUnidadeProducaoDa();
		AberturaLivroUnidadeProducaoValidar _validar = new AberturaLivroUnidadeProducaoValidar();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Termo; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new AberturaLivroUnidadeProducaoValidar(); }
		}

		public override List<DependenciaLst> ObterDependencias(IEspecificidade especificidade)
		{
			List<DependenciaLst> retorno = new List<DependenciaLst>();
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.UnidadeProducao });

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
			AberturaLivroUnidadeProducao termo = especificidade as AberturaLivroUnidadeProducao;

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
			return Deserialize(input, typeof(AberturaLivroUnidadeProducao));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				Termo termo = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);
				UnidadeProducao caracterizacao = new UnidadeProducaoBus().ObterPorEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault());

				if (caracterizacao != null)
				{
					caracterizacao.UnidadesProducao.RemoveAll(x => !termo.UnidadeProducao.Unidades.Any(y => y.CodigoUP == x.CodigoUPStr));
					termo.UnidadeProducao = new UnidadeProducaoPDF(caracterizacao);
				}
				else
				{
					Validacao.Add(Mensagem.UnidadeProducao.Inexistente);
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
				termo.UnidadeProducao.Unidades.SelectMany(x => x.ResponsaveisTecnicos).ToList().ForEach(resp =>
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

		public List<Lista> ObterUnidadesProducoes(int protocoloId)
		{
			try
			{
				return _da.ObterUnidadesProducoes(protocoloId);
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