using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal.Business
{
	public class RegistroAtividadeFlorestalBus : ICaracterizacaoBus
	{
		#region Propriedades

		RegistroAtividadeFlorestalValidar _validar = null;
		RegistroAtividadeFlorestalDa _da = new RegistroAtividadeFlorestalDa();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		#endregion

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.RegistroAtividadeFlorestal
				};
			}
		}

		public RegistroAtividadeFlorestalBus()
		{
			_validar = new RegistroAtividadeFlorestalValidar();
		}

		public RegistroAtividadeFlorestalBus(RegistroAtividadeFlorestalValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public bool Salvar(RegistroAtividadeFlorestal caracterizacao)
		{
			try
			{
				#region Configurar

				caracterizacao.Consumos.ForEach(consumo =>
				{
					if (consumo.PossuiLicencaAmbiental.GetValueOrDefault() == ConfiguracaoSistema.NAO)
					{
						consumo.Licenca = null;
					}
				});

				#endregion

				if (_validar.Salvar(caracterizacao))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(caracterizacao, bancoDeDados);

						Validacao.Add(Mensagem.RegistroAtividadeFlorestal.Salvar);

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

		public bool Excluir(int empreendimento, BancoDeDados banco = null, bool validarDependencias = true)
		{
			try
			{
				if (!_caracterizacaoValidar.Basicas(empreendimento))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, bancoDeDados);

					Validacao.Add(Mensagem.RegistroAtividadeFlorestal.Excluir);

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter

		public RegistroAtividadeFlorestal ObterPorEmpreendimento(int EmpreendimentoId, bool simplificado = false, BancoDeDados banco = null, bool historico = false)
		{

			RegistroAtividadeFlorestal caracterizacao = null;
			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(EmpreendimentoId, simplificado: simplificado, historico: historico);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{
			return _da.ObterDadosPdfTitulo(empreendimento, atividade, especificidade, banco);
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			//Caso a coluna das atividades NÃO esteja na tabela Principal
			return _da.ObterAtividadesCaracterizacao(empreendimento);
		}

		public List<TituloModeloLst> ObterModelosLista()
		{
			try
			{
				List<TituloModeloLst> modelos = _da.ObterModelosLista();
				return modelos;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion

		public bool CopiarDadosCredenciado(Dependencia caracterizacao, int empreendimentoInternoId, BancoDeDados bancoDeDados, BancoDeDados bancoCredenciado = null)
		{
			throw new NotImplementedException();
		}
	}
}