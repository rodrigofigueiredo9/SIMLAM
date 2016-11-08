using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Business
{
	public class LicencaPorteUsoMotosserraBus : EspecificidadeBusBase, IEspecificidadeBus
	{
		LicencaPorteUsoMotosserraDa _da = new LicencaPorteUsoMotosserraDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public eEspecificidadeTipo Tipo
		{
			get { return eEspecificidadeTipo.Licenca; }
		}

		public IEspecificiadeValidar Validar
		{
			get { return new LicencaPorteUsoMotosserraValidar(); }
		}

		private LicencaPorteUsoMotosserraValidar _licencaValidar = new LicencaPorteUsoMotosserraValidar();
		public LicencaPorteUsoMotosserraValidar LicencaValidar
		{
			get { return _licencaValidar; }
			set { _licencaValidar = value; }
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

		public Motosserra ObterMotosserra(int motosserraId)
		{
			try
			{
				return _da.ObterMotosserra(motosserraId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Motosserra ObterMotosserraPorHistorico(int motosserraId, string motosserraTid)
		{
			try
			{
				return _da.ObterMotosserraPorHistorico(motosserraId, motosserraTid);
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
			LicencaPorteUsoMotosserra licenca = especificidade as LicencaPorteUsoMotosserra;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				_da.Salvar(licenca, bancoDeDados);

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
			return Deserialize(input, typeof(LicencaPorteUsoMotosserra));
		}

		public object ObterDadosPdf(IEspecificidade especificidade, BancoDeDados banco)
		{
			try
			{
				Licenca licenca = _da.ObterDadosPDF(especificidade.Titulo.Id, banco);
				licenca.Motosserra = new MotosserraPDF(_da.Obter(especificidade.Titulo.Id).Motosserra);

				String pathImg = HttpContext.Current.Request.MapPath("~/Content/_imgLogo/logomarca.png");
				licenca.LogoOrgao = File.ReadAllBytes(pathImg);
				licenca.LogoOrgao = AsposeImage.RedimensionarImagem(licenca.LogoOrgao, 2.0f);

				GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
				GerenciadorConfiguracao<ConfiguracaoFuncionario> _configFunc = new GerenciadorConfiguracao<ConfiguracaoFuncionario>(new ConfiguracaoFuncionario());

				licenca.GovernoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyGovernoNome);
				licenca.SecretariaNome = _configSys.Obter<String>(ConfiguracaoSistema.KeySecretariaNome);
				licenca.OrgaoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoNome);
				licenca.SetorNome = _configFunc.Obter<List<Setor>>(ConfiguracaoFuncionario.KeySetores).Single(x => x.Id == especificidade.Titulo.SetorId).Nome;

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
			conf.PrimeiraPaginaDiferente = true;
			return conf;
		}

		public override List<DependenciaLst> ObterDependencias(IEspecificidade especificidade)
		{
			List<DependenciaLst> retorno = new List<DependenciaLst>();
			retorno.Add(new DependenciaLst() { TipoId = (int)eTituloDependenciaTipo.Caracterizacao, DependenciaTipo = (int)eCaracterizacao.ExploracaoFlorestal });

			return retorno;
		}
	}
}