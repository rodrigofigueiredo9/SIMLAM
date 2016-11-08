using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPapel;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPapel.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPapel.Business
{
	public class PapelBus
	{
		PapelDa _daPapel = new PapelDa();
		PapelValidar _validar = new PapelValidar();

		public List<PermissaoGrupo> PermissaoGrupoColecao
		{
			get
			{
				if (GerenciadorCache.PermissaoGrupo == null)
				{
					GerenciadorCache.PermissaoGrupo = _daPapel.ObterPermissaoGrupoColecao();
				}

				return GerenciadorCache.PermissaoGrupo as List<PermissaoGrupo>;
			}

			set { GerenciadorCache.PermissaoGrupo = value; }
		}

		public List<PermissaoGrupo> ObterPermissaoGrupoColecao(int id)
		{
			try
			{
				return _daPapel.ObterPermissaoGrupoColecao(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public bool Salvar(Papel papel)
		{
			Mensagem msgSucesso = (papel.Id > 0) ? Mensagem.Papel.Editar : Mensagem.Papel.Salvar;

			try
			{
				if (_validar.Salvar(papel))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_daPapel.Salvar(papel, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(msgSucesso);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public Resultados<Papel> Filtrar(PapelListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<PapelListarFiltro> filtro = new Filtro<PapelListarFiltro>(filtrosListar, paginacao);
				Resultados<Papel> resultados = _daPapel.Filtrar(filtro);

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

		public string ObterNome(int id)
		{
			try
			{
				return _daPapel.ObterNome(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Boolean Excluir(int id)
		{
			if (_validar.VerificarExcluir(id))
			{
				_daPapel.Excluir(id);
				Validacao.Add(Mensagem.Papel.Excluir);
			}

			return Validacao.EhValido;
		}
	}
}