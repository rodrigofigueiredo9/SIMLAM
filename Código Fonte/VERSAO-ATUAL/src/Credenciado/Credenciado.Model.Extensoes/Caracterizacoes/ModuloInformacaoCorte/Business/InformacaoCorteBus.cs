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
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Antigo;

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

				//TODO: Ao realizar importação automática para Institucional, preencher o InternoID e InternoTID
				//InformacaoCorteInternoBus informacaoCorteInternoBus = new InformacaoCorteInternoBus();
				//InformacaoCorte caracterizacaoInterno = informacaoCorteInternoBus.ObterPorEmpreendimento(empreendimento.InternoID, true);

				//caracterizacao.InternoID = caracterizacaoInterno.Id;
				//caracterizacao.InternoTID = caracterizacaoInterno.Tid;

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

		public bool Excluir(int id, BancoDeDados banco = null, bool validarDependencias = true)
		{
			try
			{
				var caracterizacao = this.Obter(id, simplificado: true);

				if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
					return Validacao.EhValido;

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					var caracterizacoes = this.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, simplificado: true);
					if (caracterizacoes?.Count == 1)
					{
						CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
						caracterizacaoBus.ConfigurarEtapaExcluirCaracterizacao(caracterizacao.EmpreendimentoId, bancoDeDados);
					}

					_da.Excluir(id, bancoDeDados);

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

		public InformacaoCorte Obter(int id, bool simplificado = false)
		{
			InformacaoCorte caracterizacao = null;
			try
			{
				caracterizacao = _da.Obter(id, simplificado: simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public List<InformacaoCorte> ObterPorEmpreendimento(int empreendimentoInternoId, bool simplificado = false)
		{
			List<InformacaoCorte> caracterizacao = null;
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

		public List<InformacaoCorte> FiltrarPorEmpreendimento(int empreendimentoInternoId)
		{
			List<InformacaoCorte> caracterizacao = null;
			try
			{
				caracterizacao = _da.FiltrarPorEmpreendimento(empreendimentoInternoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public List<Lista> ObterListaInfCorteEmpreendimento (int empreendimento)
		{
			List<Lista> retorno = new List<Lista>();
			try
			{
				retorno = _da.ObterListaInfCorteEmpreendimento(empreendimento);
			}catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}
			return retorno;
		}

		public List<Lista> ObterListaInfCorteTitulo (int titulo)
		{
			List<Lista> retorno = new List<Lista>();
			try
			{
				retorno = _da.ObterListaInfCorteTitulo(titulo);
			}catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}
			return retorno;
		}

		public List<Lista> ObterProdutos(int destinacaoId)
		{
			List<Lista> retorno = new List<Lista>();
			try
			{
				retorno = _da.ObterProdutos(destinacaoId);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}
			return retorno;
		}

		public InformacaoCorteAntigo ObterAntigo(int id, bool simplificado = false, BancoDeDados banco = null)
		{

			InformacaoCorteAntigo caracterizacao = null;
			try
			{
				caracterizacao = _da.ObterAntigo(id, simplificado, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public InformacaoCorteInformacao ObterInformacaoItem(int id, BancoDeDados banco = null)
		{

			InformacaoCorteInformacao item = null;
			try
			{
				item = _da.ObterInformacaoItem(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return item;
		}

		public List<InformacaoCorteLicenca> ObterLicencas(int empreendimento, BancoDeDados banco = null) => _da.ObterLicencas(empreendimento, banco);

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

		public bool ValidarCriar(int empreendimentoId)
		{
			try
			{
				if (_da.PossuiCaracterizacaoEmAberto(empreendimentoId))
					Validacao.Add(Mensagem.InformacaoCorte.ProibidoCriar);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool ValidarEditar(int id)
		{
			try
			{
				if (_da.CaracterizacaoEmAberto(id))
					Validacao.Add(Mensagem.InformacaoCorte.ProibidoEditar);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}
	}
}