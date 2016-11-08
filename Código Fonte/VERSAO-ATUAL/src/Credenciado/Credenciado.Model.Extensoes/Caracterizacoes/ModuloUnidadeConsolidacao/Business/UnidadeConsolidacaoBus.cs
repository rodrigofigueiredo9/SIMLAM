using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using System.Web;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Business
{
	public class UnidadeConsolidacaoBus : ICaracterizacaoBus
	{
		#region Propriedades

		UnidadeConsolidacaoValidar _validar = null;
		UnidadeConsolidacaoDa _da = new UnidadeConsolidacaoDa();
		CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		Configuracao.ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

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
					Tipo = eCaracterizacao.UnidadeConsolidacao
				};
			}
		}

		public UnidadeConsolidacaoBus()
		{
			_validar = new UnidadeConsolidacaoValidar();
		}

		#region Comandos DML

		public bool Salvar(UnidadeConsolidacao unidade)
		{
			try
			{
				if (!_validar.Salvar(unidade))
				{
					return Validacao.EhValido;
				}

				EmpreendimentoCaracterizacao empreendimento = _busCaracterizacao.ObterEmpreendimentoSimplificado(unidade.Empreendimento.Id);

				UnidadeConsolidacaoInternoBus internoBus = new UnidadeConsolidacaoInternoBus();
				UnidadeConsolidacao caracterizacaoInterno = internoBus.ObterPorEmpreendimento(empreendimento.InternoID, true);

				if (caracterizacaoInterno.Id > 0)
				{
					unidade.PossuiCodigoUC = caracterizacaoInterno.PossuiCodigoUC;
					unidade.CodigoUC = caracterizacaoInterno.CodigoUC;
				}
				else
				{
					if (!unidade.PossuiCodigoUC)
					{
						if (unidade.Id < 1)
						{
							int codigo = _da.ObterSequenciaCodigoUC();

							unidade.CodigoUC = Convert.ToInt64(empreendimento.MunicipioIBGE.ToString() + codigo.ToString("D4"));
						}
						else
						{
							unidade.CodigoUC = ObterPorEmpreendimento(unidade.Empreendimento.Id, simplificado: true).CodigoUC;
						}
					}
					else if (_da.FoiCopiada(unidade.Id))
					{
						unidade.CodigoUC = ObterPorEmpreendimento(unidade.Empreendimento.Id, simplificado: true).CodigoUC;
					}
				}

				foreach (var aux in unidade.ResponsaveisTecnicos)
				{
					aux.CFONumero = aux.CFONumero.Split('-').GetValue(0).ToString();
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Salvar(unidade, bancoDeDados);

					Validacao.Add(Mensagem.UnidadeConsolidacao.UnidadeConsolidacaoSalvaSucesso);

					bancoDeDados.Commit();
				}

			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool CopiarDadosInstitucional(int empreendimentoID, int empreendimentoInternoID, BancoDeDados banco)
		{
			if (banco == null)
			{
				return false;
			}

			if (_validar == null)
			{
				_validar = new UnidadeConsolidacaoValidar();
			}

			#region Configurar Caracterização

			UnidadeConsolidacaoInternoBus unidadeConsolidacaoInternoBus = new UnidadeConsolidacaoInternoBus();
			UnidadeConsolidacao caracterizacao = unidadeConsolidacaoInternoBus.ObterPorEmpreendimento(empreendimentoInternoID);

			caracterizacao.Empreendimento.Id = empreendimentoID;
			caracterizacao.InternoId = caracterizacao.Id;
			caracterizacao.InternoTid = caracterizacao.Tid;
			caracterizacao.Cultivares.ForEach(r => { r.IdRelacionamento = 0; });
			caracterizacao.ResponsaveisTecnicos.ForEach(r => { r.IdRelacionamento = 0; });

			#endregion

			if (_validar.CopiarDadosInstitucional(caracterizacao))
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					//Setar ID do credenciado
					caracterizacao.Id = ObterPorEmpreendimento(empreendimentoID, simplificado: true, banco: bancoDeDados).Id;

					_da.CopiarDadosInstitucional(caracterizacao, bancoDeDados);

					bancoDeDados.Commit();
				}
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

					Validacao.Add(Mensagem.UnidadeConsolidacao.ExcluidoSucesso);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public void AtualizarInternoIdTid(int caracterizacaoId, int internoId, string tid, BancoDeDados banco)
		{
			_da.AtualizarInternoIdTid(caracterizacaoId, internoId, tid, banco);
		}

		#endregion

		#region Obter

		public UnidadeConsolidacao ObterPorEmpreendimento(int empreendimentoId, int projetoDigitalId = 0, bool simplificado = false, BancoDeDados banco = null)
		{
			UnidadeConsolidacao caracterizacao = null;

			try
			{
				List<Caracterizacao> caracterizacoesAssociadas = _busCaracterizacao.ObterCaracterizacoesAssociadasProjetoDigital(projetoDigitalId).Where(x => x.Tipo == eCaracterizacao.UnidadeConsolidacao).ToList();

				if (caracterizacoesAssociadas != null && caracterizacoesAssociadas.Count > 0)
				{
					caracterizacao = ObterHistorico(caracterizacoesAssociadas.FirstOrDefault().Id, caracterizacoesAssociadas.FirstOrDefault().Tid);
				}
				else
				{
					caracterizacao = _da.ObterPorEmpreendimento(empreendimentoId, simplificado);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public UnidadeConsolidacao ObterHistorico(int caracterizacaoID, string caracterizacaoTID, bool simplificado = false)
		{
			UnidadeConsolidacao caracterizacao = null;

			try
			{
				caracterizacao = _da.ObterHistorico(caracterizacaoID, caracterizacaoTID, simplificado: simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public List<ListaValor> ObterListaUnidadeMedida()
		{
			List<ListaValor> retorno = null;
			try
			{
				return _da.ObterListaUnidadeMedida();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return retorno;
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			//Para o caso da coluna da atividade estar na tabela principal
			return _busCaracterizacao.ObterAtividades(empreendimento, Caracterizacao.Tipo);
		}

		#endregion

		#region Auxilizares

		public bool FoiCopiada(int caracterizacao)
		{
			try
			{
				return _da.FoiCopiada(caracterizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public bool PodeCopiar(int empInternoID, BancoDeDados banco = null)
		{
			CredenciadoPessoa credenciado = _busCaracterizacao.ObterCredenciado(User.FuncionarioId, true);

			UnidadeConsolidacaoInternoBus unidadeConsolidacaoInternoBus = new UnidadeConsolidacaoInternoBus();
			UnidadeConsolidacao caracterizacao = unidadeConsolidacaoInternoBus.ObterPorEmpreendimento(empInternoID);

			return caracterizacao.ResponsaveisTecnicos.Any(x => x.CpfCnpj == credenciado.Pessoa.CPFCNPJ);
		}

		public bool ValidarAssociar(int id, int projetoDigitalID = 0)
		{
			return true;
		}

		public bool PodeEnviar(int caracterizacaoID)
		{
			return true;
		}

		#endregion
	}
}