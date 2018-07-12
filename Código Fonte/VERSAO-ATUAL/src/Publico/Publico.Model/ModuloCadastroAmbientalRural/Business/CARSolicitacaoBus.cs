using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;

using Tecnomapas.EtramiteX.Publico.Model.ModuloCadastroAmbientalRural.Data;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloCadastroAmbientalRural.Business
{
	public class CARSolicitacaoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		CARSolicitacaoDa _da = null;
		CARSolicitacaoValidar _validar = null;
		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public CARSolicitacaoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new CARSolicitacaoDa();
			_validar = new CARSolicitacaoValidar();
		}

		String UrlSICAR
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUrlSICAR); }
		}

		#region Obter

		public CARSolicitacao ObterInterno(int id, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterInterno(id, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public CARSolicitacao ObterCredenciado(int id, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterCredenciado(id, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<PessoaLst> ObterDeclarantesLst(int requerimentoId)
		{
			List<PessoaLst> pessoas = new List<PessoaLst>();
			try
			{
				pessoas = _da.ObterDeclarantes(requerimentoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return pessoas;
		}

		public Resultados<SolicitacaoListarResultados> Filtrar(SolicitacaoListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				if (!_validar.Buscar(filtrosListar))
				{
					return null;
				}

				if (filtrosListar.IsSolicitacaoNumero)
				{
					filtrosListar.SolicitacaoNumero = int.Parse(filtrosListar.SolicitacaoTituloNumero);
				}
				else
				{
					filtrosListar.EmpreendimentoCodigo = Convert.ToInt32(filtrosListar.SolicitacaoTituloNumero);
					filtrosListar.ResponsavelEmpreendimentoCPFCNPJ = filtrosListar.DeclaranteCPFCNPJ;
					filtrosListar.DeclaranteCPFCNPJ = null;
				}

				Filtro<SolicitacaoListarFiltro> filtro = new Filtro<SolicitacaoListarFiltro>(filtrosListar, paginacao);
				Resultados<SolicitacaoListarResultados> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public string ObterUrlDemonstrativo(int solicitacaoId, int schemaSolicitacao, bool isTitulo)
		{
			var urlGerar = _da.ObterUrlGeracaoDemonstrativo(solicitacaoId, schemaSolicitacao, isTitulo) ?? "";
			if (String.IsNullOrWhiteSpace(urlGerar)) return null;

			RequestJson requestJson = new RequestJson();

			//urlGerar = "http://www.car.gov.br/pdf/demonstrativo/" + urlGerar + "/gerar";
			urlGerar = "http://homolog-car.mma.gov.br/pdf/demonstrativo/" + urlGerar + "/gerar";

			var strResposta = requestJson.Executar(urlGerar);

			var resposta = requestJson.Deserializar<dynamic>(strResposta);

			if (resposta["status"] != "s")
			{
				return string.Empty;
			}

			//return UrlSICAR + resposta["dados"];  // PRODUCAO 
			return "http://homolog-car.mma.gov.br" + resposta["dados"]; // HOMOLOG 
		}

		#endregion

		#region Validacao

		public bool ExisteCredenciado(int solicitacaoId)
		{
			try
			{
				return _da.ExisteCredenciado(solicitacaoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}


		#endregion
	}
}