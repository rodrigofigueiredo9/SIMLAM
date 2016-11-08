using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business
{
	public class GrupoQuimicoBus
	{
		#region Propriedades

		GrupoQuimicoDa _da = new GrupoQuimicoDa();
		GrupoQuimicoValidar _validar = new GrupoQuimicoValidar();

		#endregion

		#region Ações DML

		public bool Salvar(ConfiguracaoVegetalItem grupoQuimico)
		{
			try
			{
				
				if (_validar.Salvar(grupoQuimico))
				{
					grupoQuimico.Texto = grupoQuimico.Texto.DeixarApenasUmEspaco();

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(grupoQuimico, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.GrupoQuimico.Salvar);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter/Listar

		public ConfiguracaoVegetalItem Obter(int id)
		{
			try
			{
				return _da.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<ConfiguracaoVegetalItem> Listar()
		{
			try
			{
				return _da.Listar();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Lista> ObterLista()
        {
			List<Lista> retorno = new List<Lista>();
            try
            {
                var lista = _da.Listar();

                lista.ForEach(itemLista =>
                {
                    retorno.Add(new Lista() { Id = itemLista.Id.ToString(), Texto = itemLista.Texto, IsAtivo = true });
                });

                return retorno;
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return null;
        }

		#endregion
	}
}
