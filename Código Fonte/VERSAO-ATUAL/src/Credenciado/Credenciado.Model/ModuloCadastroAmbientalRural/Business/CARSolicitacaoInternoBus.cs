using System;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business
{
	public class CARSolicitacaoInternoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		CARSolicitacaoInternoDa _da;

		String UrlSICAR
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUrlSICAR); }
		}

		#endregion

		public CARSolicitacaoInternoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new CARSolicitacaoInternoDa();
		}

		public CARSolicitacao Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			try
			{
				return _da.Obter(id, simplificado, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public int ObterNovoID(BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterNovoID(banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

        public void EnviarReenviarArquivoSICAR(int solicitacaoId, bool isEnviar, BancoDeDados banco = null)
        {
            try
            {
                GerenciadorTransacao.ObterIDAtual();

                using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
                {
                    bancoDeDados.IniciarTransacao();

                    _da.InserirFilaArquivoCarSicar(solicitacaoId, eCARSolicitacaoOrigem.Institucional, banco);

                    bancoDeDados.Commit();

                    Validacao.Add(Mensagem.CARSolicitacao.SucessoEnviarReenviarArquivoSICAR(isEnviar));
                }
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }
        }

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

		public string ObterUrlRecibo(int solicitacaoId, int schemaSolicitacao)
		{
			var urlGerar = _da.ObterUrlGeracaoRecibo(solicitacaoId, schemaSolicitacao);

			RequestJson requestJson = new RequestJson();

			var strResposta = requestJson.Executar(urlGerar);

			var resposta = requestJson.Deserializar<dynamic>(strResposta);

			if (resposta["status"] != "s")
			{
				return string.Empty;
			}

			//return UrlSICAR + resposta["dados"];  // PRODUCAO
            return "http://homolog-car.mma.gov.br/" + resposta["dados"]; // HOMOLOG
		}


		public int ObterIdAquivoSICAR(int solicitacaoId, int schemaSolicitacao)
		{
			return _da.ObterIdAquivoSICAR(solicitacaoId, schemaSolicitacao);
		}
	}
}