using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Entities.Interno.ModuloSetor;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Business
{
	public class OutrosLegitimacaoTerraDevolutaBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		OutrosLegitimacaoTerraDevolutaDa _da = new OutrosLegitimacaoTerraDevolutaDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Outros; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new OutrosLegitimacaoTerraDevolutaValidar(); }
		}

		public override List<eCargo> CargosOrdenar
		{
			get
			{
				List<eCargo> cargos = new List<eCargo>();
				cargos.Add(eCargo.DiretorTecnico);
				cargos.Add(eCargo.DiretorPresidente);
				return cargos;
			}
		}

		public override List<DependenciaLst> ObterDependencias(IEspecificidade especificidade)
		{
			List<DependenciaLst> retorno = new List<DependenciaLst>();
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.Dominialidade });
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.RegularizacaoFundiaria });

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
			OutrosLegitimacaoTerraDevoluta Outros = especificidade as OutrosLegitimacaoTerraDevoluta;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(Outros, bancoDeDados);

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
			return Deserialize(input, typeof(OutrosLegitimacaoTerraDevoluta));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				Outros Outros = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);
				DataEmissaoPorExtenso(Outros.Titulo);

				return Outros;
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
				Outros outros = dataSource as Outros;
				List<Table> itenRemover = new List<Table>();
				conf.CabecalhoRodape = CabecalhoRodapeFactory.Criar(0, isBrasao: true, isLogo: true);

				(conf.CabecalhoRodape as CabecalhoRodapeDefault).CabecalhoTituloNumero = outros.Titulo.Numero;

				if (!outros.IsInalienabilidade)
				{
					itenRemover.Add(doc.LastTable("Artigo 28, parágrafos 1º e 2º da Lei nº 9.769"));
				}

				foreach (var item in outros.Destinatarios)
				{
					if (!string.IsNullOrEmpty(item.ConjugeNomePai) && !string.IsNullOrEmpty(item.ConjugeNomeMae))
					{
						item.ConjugeNomePaiMae = item.ConjugeNomePai + " e " + item.ConjugeNomeMae;
					}
					else if (!string.IsNullOrEmpty(item.ConjugeNomePai))
					{
						item.ConjugeNomePaiMae = item.ConjugeNomePai;
					}
					else if (!string.IsNullOrEmpty(item.ConjugeNomeMae))
					{
						item.ConjugeNomePaiMae = item.ConjugeNomeMae;
					}

					if (!string.IsNullOrEmpty(item.NomePai) && !string.IsNullOrEmpty(item.NomeMae))
					{
						item.NomePaiMae = item.NomePai + " e " + item.NomeMae;
					}
					else if (!string.IsNullOrEmpty(item.NomePai))
					{
						item.NomePaiMae = item.NomePai;
					}
					else if (!string.IsNullOrEmpty(item.NomeMae))
					{
						item.NomePaiMae = item.NomeMae;
					}
				}

				AsposeExtensoes.RemoveTables(itenRemover);
			});

			return conf;
		}

		public List<ListaValor> ObterDominios(int id)
		{
			try
			{
				return _da.ObterDominios(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}
	}
}