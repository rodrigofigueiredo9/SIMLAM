using System;
using System.Collections.Generic;
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
	public class OutrasPenalidadesBus
	{
		#region Propriedades

		OutrasPenalidadesValidar _validar = null;
        OutrasPenalidadesDa _da = new OutrasPenalidadesDa();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public OutrasPenalidadesBus()
		{
            _validar = new OutrasPenalidadesValidar();
		}

        public OutrasPenalidadesBus(OutrasPenalidadesValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

        public bool Salvar(OutrasPenalidades entidade)
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
                            if (!string.IsNullOrWhiteSpace(entidade.Arquivo.Nome))
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
                            else
                            {
                                entidade.Arquivo.Id = null;
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

        public OutrasPenalidades Obter(int fiscalizacaoId, BancoDeDados banco = null)
        {
            OutrasPenalidades entidade = new OutrasPenalidades();

            try
            {
                entidade = _da.Obter(fiscalizacaoId, banco);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return entidade;
        }

		#endregion
	}
}
