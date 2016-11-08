using System;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business
{
	public class DestinatarioPTVBus
	{
		#region Propriedades

		DestinatarioPTVDa _da = new DestinatarioPTVDa();
		DestinatarioPTVValidar _validar = new DestinatarioPTVValidar();

		#endregion

		#region DMLs

		public bool Salvar(DestinatarioPTV destinatario)
		{
			try
			{
				if (_validar.Salvar(destinatario))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(destinatario, bancoDeDados);

						bancoDeDados.Commit();
					}
				}

				return Validacao.EhValido;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
				return false;
			}
		}

		#endregion

		#region Obter

		public DestinatarioPTV Obter(int id)
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

        public DestinatarioPTV ObterDestinatarioPorCodigoUC(decimal? codigoUC)
        {
            try
            {
                return _da.ObterDestinatarioPorCodigoUC(codigoUC);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return null;
        }

		public int ObterId(String CpfCnpj, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterId(CpfCnpj, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

		#endregion

        public Resultados<DestinatarioListarResultado> Filtrar(DestinatarioListarFiltro destinatarioListarFiltro, Paginacao paginacao)
        {
            try
            {
                Filtro<DestinatarioListarFiltro> filtro = new Filtro<DestinatarioListarFiltro>(destinatarioListarFiltro, paginacao);
                Resultados<DestinatarioListarResultado> resultados = _da.Filtrar(filtro);

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

        public bool Excluir(int id)
        {
            if (_validar.Excluir(id))
            {
                GerenciadorTransacao.ObterIDAtual();

                using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
                {
                    bancoDeDados.IniciarTransacao();

                    _da.Excluir(id, bancoDeDados);

                    bancoDeDados.Commit();

                    Validacao.Add(Mensagem.DestinatarioPTV.DestinatarioExcluido);
                }
            }

            return Validacao.EhValido;
        }
	}
}