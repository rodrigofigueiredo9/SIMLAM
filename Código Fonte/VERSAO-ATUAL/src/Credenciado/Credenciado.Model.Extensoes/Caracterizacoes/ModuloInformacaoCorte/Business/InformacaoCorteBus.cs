using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pentago.Utilities;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business
{
	public class InformacaoCorteBus : ICaracterizacaoBus
	{
		#region Propriedades

		InformacaoCorteValidar _validar = new InformacaoCorteValidar();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		InformacaoCorteDa _da = new InformacaoCorteDa();
		Configuracao.ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.UnidadeProducao
				};
			}
		}

		public InformacaoCorteBus()
		{
			_validar = new InformacaoCorteValidar();
		}

		#region Comandos DML

		public bool Salvar(InformacaoCorte caracterizacao, int projetoDigitalId)
		{
			try
			{
				CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
				EmpreendimentoCaracterizacao empreendimento = caracterizacaoBus.ObterEmpreendimentoSimplificado(caracterizacao.Empreendimento.Id);

				InformacaoCorteInternoBus informacaoCorteInternoBus = new InformacaoCorteInternoBus();
				InformacaoCorte caracterizacaoInterno = informacaoCorteInternoBus.ObterPorEmpreendimento(empreendimento.InternoID, true);

				caracterizacao.InternoID = caracterizacaoInterno.Id;
				caracterizacao.InternoTID = caracterizacaoInterno.Tid;

				if (!_validar.Salvar(caracterizacao, projetoDigitalId))
					return Validacao.EhValido;

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Salvar(caracterizacao, bancoDeDados);

					Validacao.Add(Mensagem.InformacaoCorte.Salvar);

					bancoDeDados.Commit();
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

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
					caracterizacaoBus.ConfigurarEtapaExcluirCaracterizacao(empreendimento, bancoDeDados);

					_da.Excluir(empreendimento, bancoDeDados);

					Validacao.Add(Mensagem.InformacaoCorte.Excluir);

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

		#region Obter/Filtrar

		public InformacaoCorte ObterPorEmpreendimento(int empreendimentoInternoId, bool simplificado = false)
		{
			InformacaoCorte caracterizacao = null;
			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(empreendimentoInternoId, simplificado: simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		#endregion

		#region Caracterizacao

		public bool CopiarDadosInstitucional(int empreendimentoID, int empreendimentoInternoID, BancoDeDados banco)
		{
			throw new NotImplementedException();
		}

		public bool PodeCopiar(int empInternoID, BancoDeDados banco = null)
		{
			throw new NotImplementedException();
		}

		public bool ValidarAssociar(int id, int projetoDigitalID = 0)
		{
			throw new NotImplementedException();
		}

		public bool PodeEnviar(int caracterizacaoID)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}