﻿using System;
using System.Collections.Generic;
using System.Linq;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloOutros.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloOutros.Business
{
	public class OutrosInformacaoCorteBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		OutrosInformacaoCorteDa _da = new OutrosInformacaoCorteDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public static EtramitePrincipal Usuario
		{
			get { return (System.Web.HttpContext.Current.User as EtramitePrincipal); }
		}

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Outros; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new OutrosInformacaoCorteValidar(); }
		}

		public override List<DependenciaLst> ObterDependencias(IEspecificidade especificidade)
		{
			List<DependenciaLst> retorno = new List<DependenciaLst>();
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.InformacaoCorte });
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
			OutrosInformacaoCorte outros = especificidade as OutrosInformacaoCorte;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(outros, bancoDeDados);

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
			return Deserialize(input, typeof(OutrosInformacaoCorte));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				var user = Usuario.EtramiteIdentity.UsuarioId;
				
				Outros Outros = _da.ObterDadosPDF(especificidade.Titulo.Id, user, banco);
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
				List<Table> itenRemover = new List<Table>();
				conf.CabecalhoRodape = CabecalhoRodapeFactory.Criar(especificidade.Titulo.SetorId, isCredenciado: true);

				AsposeExtensoes.RemoveTables(itenRemover);
			});

			conf.ExibirAssinantes1 = true;
			conf.ExibirAssinantes2 = false;

			return conf;
		}

		public void AlterarSituacao(int? titulo)
		{

		}
	}
}
