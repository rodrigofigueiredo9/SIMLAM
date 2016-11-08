using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business
{
	public class OrgaoExternoBus
	{
		TramitacaoValidar _validar = new TramitacaoValidar();
		TramitacaoExternoDa _daExt = new TramitacaoExternoDa();
		public EtramiteIdentity User
		{
			get
			{
				try
				{
					return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity;
				}
				catch (Exception exc)
				{
					Validacao.AddErro(exc);
					return null;
				}
			}
		}

		public bool EnviarExterno(List<Tramitacao> tramitacoes)
		{
			try
			{
				if (_validar.EnviarExterno(tramitacoes))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						foreach (Tramitacao tramitacao in tramitacoes)
						{
							_daExt.EnviarExterno(tramitacao);
						}

						bancoDeDados.Commit();
					}

					Validacao.Add(Mensagem.Tramitacao.EnviarEfetuadoComSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return Validacao.EhValido;
		}

		public bool ReceberExterno(List<Tramitacao> tramitacoes, int destinatarioSetorId, int orgaoExternoId)
		{
			try
			{
				if (_validar.ReceberExterno(tramitacoes, destinatarioSetorId, orgaoExternoId))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						foreach (Tramitacao tramitacao in tramitacoes)
						{
							tramitacao.DestinatarioSetor.Id = destinatarioSetorId;
							tramitacao.Destinatario.Id = User.FuncionarioId;
							_daExt.ReceberExterno(tramitacao);
						}

						bancoDeDados.Commit();
					}

					Validacao.Add(Mensagem.Tramitacao.RetirarSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public List<OrgaoClasse> ObterOrgaosExterno()
		{
			try
			{
				return _daExt.ObterOrgaosExterno();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}
	}
}