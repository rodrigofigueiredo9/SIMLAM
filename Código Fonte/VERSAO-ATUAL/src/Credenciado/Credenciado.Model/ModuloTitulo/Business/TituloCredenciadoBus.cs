using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business
{
	public class TituloCredenciadoBus
	{
		#region Propriedades

		PessoaInternoBus _busPessoa;
		TituloInternoBus _busInterno;
		CredenciadoBus _busCredenciado;

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public TituloCredenciadoBus()
		{
			_busInterno = new TituloInternoBus();
			_busCredenciado = new CredenciadoBus();
			_busPessoa = new PessoaInternoBus();
		}

		#region Obter/Filtrar

		public Resultados<Titulo> Filtrar(TituloFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				CredenciadoPessoa credenciado = _busCredenciado.Obter(User.FuncionarioId, true);
				filtrosListar.CredenciadoId = User.FuncionarioId;
				filtrosListar.CredenciadoPessoaId = credenciado.Pessoa.InternoId.GetValueOrDefault();

				return _busInterno.Filtrar(filtrosListar, paginacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

        public bool ExistePorEmpreendimento(int empreendimentoId)
        {
            try
            {
                Resultados<Titulo> titulos = _busInterno.ObterPorEmpreendimento(empreendimentoId);
                if (titulos == null)
                    return false;
                if(titulos.Itens.Count() > 1) 
                    return true;
                else 
                    return false;
               

            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return false;
        }

		public List<Situacao> ObterSituacoes()
		{
			//2 - Emitido | 3 - Concluído | 4 - Assinado | 6 - Prorrogado
			GerenciadorConfiguracao<ConfiguracaoTitulo> configTitulo = new GerenciadorConfiguracao<ConfiguracaoTitulo>(new ConfiguracaoTitulo());
			return configTitulo.Obter<List<Situacao>>(ConfiguracaoTitulo.KeySituacoes).Where(x => x.Id == 2 || x.Id == 3 || x.Id == 4 || x.Id == 6).ToList();
		}

		#endregion
	}
}