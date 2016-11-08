using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business
{
	public class AtividadeConfiguracaoBus
	{
		#region Propriedades

		AtividadeConfiguracaoValidar _validar = null;
		AtividadeConfiguracaoDa _da = new AtividadeConfiguracaoDa();

		#endregion

		public AtividadeConfiguracaoBus() { }

		public AtividadeConfiguracaoBus(AtividadeConfiguracaoValidar validar)
		{
			_validar = validar;
		}

		#region Ações DML

		public void Salvar(AtividadeConfiguracao atividadeConfiguracao)
		{
			try
			{
				if (_validar.Salvar(atividadeConfiguracao))
				{
					
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(atividadeConfiguracao, bancoDeDados);

						bancoDeDados.Commit();
					}

					Validacao.Add(Mensagem.AtividadeConfiguracao.Salvar);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void Excluir(int id)
		{
			try
			{
				_da.Excluir(id);
				Validacao.Add(Mensagem.AtividadeConfiguracao.Excluir);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		#endregion

		#region Obter/Filtrar

		public AtividadeConfiguracao Obter(int id, bool obterHistorico = false)
		{
			try
			{
				return _da.Obter(id, null, obterHistorico);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<TituloModeloLst> ObterModelosSolicitadoExterno()
		{
			try
			{
				return _da.ObterModelosSolicitadoExterno();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<TituloModeloLst> ObterModelosAtividades(List<AtividadeSolicitada> atividades, bool renovacao = false)
		{
			return _da.ObterModelosAtividades(atividades, renovacao);
		}

		public Resultados<AtividadeConfiguracao> Filtrar(ListarConfigurarcaoFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ListarConfigurarcaoFiltro> filtro = new Filtro<ListarConfigurarcaoFiltro>(filtrosListar, paginacao);
				Resultados<AtividadeConfiguracao> resultados = _da.Filtrar(filtro);

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

		#endregion

		#region Validar

		public void ValidarAtividadeConfigurada(int id)
		{
			try
			{
				_validar.AtividadeConfigurada(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public bool AtividadeConfigurada(int atividadeId, int modeloId)
		{
			return _da.AtividadeConfigurada(atividadeId, modeloId);
		}

		#endregion
	}
}