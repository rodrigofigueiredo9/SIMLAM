using System;
using System.Collections.Generic;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Business
{
	public class LicencaPreviaBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		LicencaPreviaDa _da = new LicencaPreviaDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Licenca; }
		}

		public override List<DependenciaLst> ObterDependencias(IEspecificidade especificidade)
		{
			/* Caso for titulo 'gererico'*/
			List<DependenciaLst> retorno = ObterDependenciasAtividadesCaract(especificidade);

			if (retorno != null && retorno.Count > 0 && !retorno.Exists(x => x.DependenciaTipo == (int)eCaracterizacao.ExploracaoFlorestal))
			{
				retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.ExploracaoFlorestal });
			}

			return retorno;
		}

		public IEspecificiadeValidar Validar
		{
			get { return new LicencaPreviaValidar(); }
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
			LicencaPrevia Licenca = especificidade as LicencaPrevia;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(Licenca, bancoDeDados);

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
			return Deserialize(input, typeof(LicencaPrevia));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				Licenca licenca = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);
				licenca.Caracterizacao = ObterDadosCaracterizacoes(especificidade);
				DataEmissaoPorExtenso(licenca.Titulo);

				return licenca;
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
				Licenca Licenca = dataSource as Licenca;
				List<Table> itenRemover = new List<Table>();
				conf.CabecalhoRodape = CabecalhoRodapeFactory.Criar(especificidade.Titulo.SetorId);

				if (!Licenca.Caracterizacao.ExibirCampo)
				{
					itenRemover.Add(doc.Last<Table>("«CARACTERIZACAO.CAMPO.NOME»"));
				}

				if (!Licenca.Caracterizacao.ExibirCampos1 || !Licenca.Caracterizacao.ExibirCampos2)
				{
					doc.Last<Row>("«TableStart:Caracterizacao.Campos1»").Remove();
				}

				if (especificidade.Atividades.Exists(x => x.Id == AtividadeIdSilvicultura || x.Id == AtividadeIdPulverizacao))
				{
					itenRemover.Add(doc.FindTable("COORDENADA DA ATIVIDADE"));

					if (doc.FindTable("«TableStart:Caracterizacao.Campos1»") != null)
					{
						itenRemover.Add(doc.FindTable("«TableStart:Caracterizacao.Campos1»"));
					}

					if (doc.FindTable("«CARACTERIZACAO.CAMPO.NOME»") != null)
					{
						itenRemover.Add(doc.FindTable("«CARACTERIZACAO.CAMPO.NOME»"));
					}
				}
				else
				{
					itenRemover.Add(doc.FindTable("«Caracterizacao.Cultura.AreaTotalHa»"));
				}

				AsposeExtensoes.RemoveTables(itenRemover);
			});

			return conf;
		}

		#region Atividade Caracterização



		#endregion
	}
}