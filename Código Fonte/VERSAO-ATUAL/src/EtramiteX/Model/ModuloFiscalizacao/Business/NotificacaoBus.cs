using System;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class NotificacaoBus
	{
		#region Propriedades

		NotificacaoValidar _validar = null;
        NotificacaoDa _da = new NotificacaoDa();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Construtores
		public NotificacaoBus()
		{
			_validar = new NotificacaoValidar();
		}

		public NotificacaoBus(NotificacaoValidar validar)
		{
			_validar = validar;
		} 
		#endregion

		#region Comandos DML

		public bool Salvar(Notificacao entidade)
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

        public Notificacao Obter(int fiscalizacaoId, BancoDeDados banco = null)
        {
            Notificacao entidade = new Notificacao();

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
