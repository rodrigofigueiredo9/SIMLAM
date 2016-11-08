using System;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloSetor;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloSetor.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloSetor.Business
{
	public class SetorLocalizacaoBus
	{

		#region Propriedades

		private SetorLocalizacaoValidar _validar = null;
		SetorLocalizacaoDa _da = new SetorLocalizacaoDa();
		ListaBus _listaBus = new ListaBus();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public void Salvar(SetorLocalizacao setor)
		{
			try
			{
				if (_validar.Salvar(setor))
				{
					Mensagem msgSucesso;

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(setor, bancoDeDados);

						bancoDeDados.Commit();
					}

					msgSucesso = Mensagem.Setor.Editar;

					Validacao.Add(msgSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public SetorLocalizacaoBus(SetorLocalizacaoValidar validar)
		{
			_validar = validar;
		}

		public SetorLocalizacao Obter(int id, string tid = null)
		{
			SetorLocalizacao setor = null;

			try
			{
				if (tid == null || _da.VerificarTidAtual(id, tid))
				{
					setor = _da.Obter(id);
				}
				else
				{
					//setor = _da.ObterHistorico(id, tid);
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return setor;
		}

		public Resultados<SetorLocalizacao> Filtrar(ListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ListarFiltro> filtro = new Filtro<ListarFiltro>(filtrosListar, paginacao);
				Resultados<SetorLocalizacao> resultados = _da.Filtrar(filtro);

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
	}
}
