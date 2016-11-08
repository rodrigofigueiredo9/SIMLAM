using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx.CabecalhoRodape
{
	public class CabecalhoRodapeBus
	{
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoFuncionario> _configFunc = new GerenciadorConfiguracao<ConfiguracaoFuncionario>(new ConfiguracaoFuncionario());

		private ICabecalhoRodape FormatarEndereco(ICabecalhoRodape cabecalhoRodape, SetorEndereco end)
		{
			if (!String.IsNullOrEmpty(end.Logradouro) || !String.IsNullOrEmpty(end.Numero) || !String.IsNullOrEmpty(end.Bairro))
			{
				cabecalhoRodape.OrgaoEndereco = String.Empty;
				if (!String.IsNullOrEmpty(end.Logradouro))
				{
					cabecalhoRodape.OrgaoEndereco = end.Logradouro;
				}

				if (!String.IsNullOrEmpty(end.Numero))
				{
					cabecalhoRodape.OrgaoEndereco = String.Format("{0}, Nº {1}", cabecalhoRodape.OrgaoEndereco, end.Numero);
				}

				if (!String.IsNullOrEmpty(end.Bairro))
				{
					cabecalhoRodape.OrgaoEndereco = String.Format("{0} - {1}", cabecalhoRodape.OrgaoEndereco, end.Bairro);
				}

				if (!String.IsNullOrEmpty(cabecalhoRodape.OrgaoEndereco))
				{
					cabecalhoRodape.OrgaoEndereco += ", ";
				}
			}
			else
			{
				cabecalhoRodape.OrgaoEndereco = AsposeData.Empty;
			}

			if (!String.IsNullOrEmpty(end.MunicipioTexto) && !String.IsNullOrEmpty(end.EstadoTexto))
			{
				cabecalhoRodape.OrgaoMunicipio = String.Format("{0}/", end.MunicipioTexto);
				cabecalhoRodape.OrgaoUF = String.Format("{0}, ", end.EstadoTexto);
			}
			else
			{
				cabecalhoRodape.OrgaoMunicipio = AsposeData.Empty;
				cabecalhoRodape.OrgaoUF = AsposeData.Empty;
			}

			if (!String.IsNullOrEmpty(end.CEP))
			{
				cabecalhoRodape.OrgaoCep = String.Format("CEP: {0}.", end.CEP);
			}
			else
			{
				cabecalhoRodape.OrgaoCep = AsposeData.Empty;
			}

			if (!String.IsNullOrEmpty(end.Fone) || !String.IsNullOrEmpty(end.FoneFax))
			{
				if (!String.IsNullOrEmpty(end.Fone))
				{
					cabecalhoRodape.OrgaoContato = String.Format("Fone: {0}", end.Fone);
				}

				if (!String.IsNullOrEmpty(end.FoneFax))
				{
					cabecalhoRodape.OrgaoContato = String.Format("{0}{1}", cabecalhoRodape.OrgaoContato, (!String.IsNullOrEmpty(end.Fone) ? " - " : string.Empty));
					cabecalhoRodape.OrgaoContato += String.Format("Fax: {0}", end.FoneFax);
				}
			}
			else
			{
				cabecalhoRodape.OrgaoContato = AsposeData.Empty;
			}

			return cabecalhoRodape;
		}

		public ICabecalhoRodape ObterEnderecoDefault(ICabecalhoRodape cabecalhoRodape)
		{
			cabecalhoRodape.OrgaoCep = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoCep);
			cabecalhoRodape.OrgaoContato = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoTelefone);
			cabecalhoRodape.OrgaoEndereco = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoEndereco);
			cabecalhoRodape.OrgaoMunicipio = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoMunicipio);
			cabecalhoRodape.OrgaoUF = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoUf);

			ObterNomes(cabecalhoRodape);

			return cabecalhoRodape;
		}

		public ICabecalhoRodape ObterEnderecoSetor(ICabecalhoRodape cabecalhoRodape, int setorId)
		{
			CabecalhoRodapeDa da = new CabecalhoRodapeDa();
			SetorEndereco end = da.ObterEndSetor(setorId);

			if (end != null)
			{
				cabecalhoRodape = FormatarEndereco(cabecalhoRodape, end);
			}
			cabecalhoRodape.SetorNome = _configFunc.Obter<List<Setor>>(ConfiguracaoFuncionario.KeySetores).Single(x => x.Id == setorId).Nome;

			ObterNomes(cabecalhoRodape);

			return cabecalhoRodape;
		}

		public ICabecalhoRodape ObterEnderecoFuncLogado(ICabecalhoRodape cabecalhoRodape)
		{
			CabecalhoRodapeDa da = new CabecalhoRodapeDa();

			int setorId = da.ObterFuncSetor((HttpContext.Current.User.Identity as EtramiteIdentity).FuncionarioId);

			SetorEndereco end = da.ObterEndSetor(setorId);

			cabecalhoRodape = FormatarEndereco(cabecalhoRodape, end);

			cabecalhoRodape.SetorNome = _configFunc.Obter<List<Setor>>(ConfiguracaoFuncionario.KeySetores).Single(x => x.Id == setorId).Nome;

			ObterNomes(cabecalhoRodape);

			return cabecalhoRodape;
		}

		public ICabecalhoRodape ObterNomes(ICabecalhoRodape cabecalhoRodape)
		{
			GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

			cabecalhoRodape.GovernoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyGovernoNome);
			cabecalhoRodape.SecretariaNome = _configSys.Obter<String>(ConfiguracaoSistema.KeySecretariaNome);
			cabecalhoRodape.OrgaoNome = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoNome);
			cabecalhoRodape.OrgaoSigla = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoSigla);

			return cabecalhoRodape;
		}
	}
}
