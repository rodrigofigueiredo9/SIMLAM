using System;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class ObjetoInfracaoBus
	{
		#region Propriedades

		ObjetoInfracaoValidar _validar = null;
		ObjetoInfracaoDa _da = new ObjetoInfracaoDa();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Comandos DML

		public bool Salvar(ObjetoInfracao entidade)
		{
			try
			{
				if (_validar.Salvar(entidade))
				{
					if (entidade.Id < 1)
					{
						entidade.Id = _da.ObterID(entidade.FiscalizacaoId);
					}

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

                        #region Arquivo

                        if (entidade.Arquivo != null)
                        {
                            if (entidade.Arquivo.Id != null && entidade.Arquivo.Id == 0)
                            {
                                ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
                                entidade.Arquivo = _busArquivo.Copiar(entidade.Arquivo);
                            }

                            if (entidade.Arquivo.Id == 0)
                            {
                                ArquivoDa _arquivoDa = new ArquivoDa();
                                _arquivoDa.Salvar(entidade.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
                            }
                        }

                        #endregion 

						_da.Salvar(entidade, bancoDeDados); 

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter

		public ObjetoInfracao Obter(int fiscalizacaoId, BancoDeDados banco = null) 
		{

			ObjetoInfracao entidade = null;
			try
			{
				entidade = _da.Obter(fiscalizacaoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return entidade;
		}

		#endregion 

		public ObjetoInfracaoBus()
		{
			_validar = new ObjetoInfracaoValidar();
		}

		public ObjetoInfracaoBus(ObjetoInfracaoValidar validar)
		{
			_validar = validar;
		}
	}
}