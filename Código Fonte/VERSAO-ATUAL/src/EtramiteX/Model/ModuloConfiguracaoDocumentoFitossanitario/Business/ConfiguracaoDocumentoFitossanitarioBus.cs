using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloConfiguracaoDocumentoFitossanitario.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloConfiguracaoDocumentoFitossanitario.Business
{
	public class ConfiguracaoDocumentoFitossanitarioBus
	{
		#region Propriedades

		ConfiguracaoDocumentoFitossanitarioDa _da = new ConfiguracaoDocumentoFitossanitarioDa();
		ConfiguracaoDocumentoFitossanitarioValidar _validar = new ConfiguracaoDocumentoFitossanitarioValidar();

		#endregion

		#region DMLs

		public bool Salvar(ConfiguracaoDocumentoFitossanitario configuracao)
		{
			if (!_validar.Salvar(configuracao))
			{
				return false;
			}

			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					_da.Salvar(configuracao);

					bancoDeDados.Commit();

					Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.Salvar);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

        public bool Editar(ConfiguracaoDocumentoFitossanitario configuracao, int idEditado)
        {
            if (!_validar.Salvar(configuracao))
            {
                return false;
            }

            try
            {
                GerenciadorTransacao.ObterIDAtual();

                using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
                {
                    bancoDeDados.IniciarTransacao();

                    _da.SalvarEdicaoIntervalo(configuracao, idEditado);

                    bancoDeDados.Commit();

                    Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.Salvar);
                }
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return Validacao.EhValido;
        }

        public bool Excluir(ConfiguracaoDocumentoFitossanitario configuracao, int idEditado)
        {
            try
            {
                GerenciadorTransacao.ObterIDAtual();

                using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
                {
                    bancoDeDados.IniciarTransacao();

                    _da.Excluir(configuracao, idEditado);

                    bancoDeDados.Commit();

                    Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.Excluir);
                }
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return Validacao.EhValido;
        }

		#endregion

		#region Obter

		public ConfiguracaoDocumentoFitossanitario Obter(bool simplificado = false)
		{
			try
			{
				return _da.Obter(simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

        public ConfiguracaoDocumentoFitossanitario ObterAnoCorrente(bool simplificado = false)
        {
            try
            {
                return _da.ObterAnoCorrente(simplificado);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return null;
        }

        public List<long> ObterLiberadosIntervalo(int tipo, long inicio, long fim)
        {
            List<long> retorno = new List<long>();
            try
            {
                retorno = _da.LiberadosIntervalo(tipo, inicio, fim);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return retorno;
        }

		#endregion
	}
}