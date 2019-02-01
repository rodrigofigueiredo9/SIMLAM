using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business
{
	public class ProjetoDigitalCredenciadoValidar
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		RequerimentoCredenciadoValidar _requerimentoCredenciadoValidar;
		ProjetoDigitalCredenciadoDa _da;
		CaracterizacaoBus _busCaracterizacao;
		CaracterizacaoValidar _validarCaracterizacao;
		CaracterizacaoInternoBus _internoBus;

		private static EtramitePrincipal User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal); }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public ProjetoDigitalCredenciadoValidar(string esquema = null)
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new ProjetoDigitalCredenciadoDa(UsuarioCredenciado);
			_requerimentoCredenciadoValidar = new RequerimentoCredenciadoValidar();
			_busCaracterizacao = new CaracterizacaoBus();
			_validarCaracterizacao = new CaracterizacaoValidar();
			_internoBus = new CaracterizacaoInternoBus();
		}

		public bool Salvar(ProjetoDigital projeto)
		{
			if (projeto.RequerimentoId <= 0)
			{
				Validacao.Add(Mensagem.ProjetoDigital.RequerimentoObrigatorio);
				return false;
			}
			
			return Validacao.EhValido;
		}

		public bool Excluir(ProjetoDigital projeto)
		{
			if (projeto == null)
			{
				Validacao.Add(Mensagem.ProjetoDigital.ProjetoDigitalNaoEncontrado);
			}

			if (projeto.Situacao != (int)eProjetoDigitalSituacao.EmElaboracao)
			{
				Validacao.Add(Mensagem.ProjetoDigital.ExcluirSituacaoInvalida);
			}

			CARSolicitacaoBus carSolicitacaoBus = new CARSolicitacaoBus();
			CARSolicitacao carSolicitacao = carSolicitacaoBus.ObterPorRequerimento(projeto.RequerimentoId);
			if (carSolicitacao.Id > 0)
			{
				Validacao.Add(Mensagem.ProjetoDigital.ExcluirPossuiCARSolicitacao(carSolicitacao.SituacaoTexto));
			}

			return Validacao.EhValido;
		}

		public bool EmPosseCredenciado(int idProjeto)
		{
			if (idProjeto == 0)
			{
				return true;
			}

			ProjetoDigital processo = _da.Obter(idProjeto, null, null);

			return EmPosseCredenciado(processo);
		}

		public bool EmPosseCredenciado(ProjetoDigital projeto)
		{
			if (projeto == null)
			{
				return true;
			}

			return projeto.CredenciadoId == User.EtramiteIdentity.FuncionarioId;
		}

		public bool PodeEditarRequerimento(int idProjeto)
		{
			return true;
		}

		public bool PodeEditarCaracterizacao(int idProjeto)
		{
			return true;
		}

		public bool PossuiCaracterizacaoAtividade(List<CaracterizacaoLst> caracterizacoes, int projetoDigitalId)
		{
			if (caracterizacoes == null || caracterizacoes.Count <= 0)
			{
				List<Atividade> atividades = _da.ObterAtividades(projetoDigitalId);
				Validacao.Add(Mensagem.ProjetoDigital.AtividadeSemCaracterizacao(Mensagem.Concatenar(atividades.Select(x => x.NomeAtividade).ToList())));
				return false;
			}

			return true;
		}

		public bool EnviarBasicas(ProjetoDigital projeto, bool validardesatualizadas = false)
		{
			if (projeto.Situacao == (int)eProjetoDigitalSituacao.AguardandoCorrecao)
			{
				Validacao.Add(Mensagem.ProjetoDigital.PossuiPendenciasCorrecao);
				return false;
			}

			if (projeto.Situacao != (int)eProjetoDigitalSituacao.EmElaboracao)
			{
				Validacao.Add(Mensagem.ProjetoDigital.EnviarSituacaoInvalida);
				return false;
			}

			if (validardesatualizadas)
			{
				EmpreendimentoCaracterizacao empreendimento = _busCaracterizacao.ObterEmpreendimentoSimplificado(projeto.EmpreendimentoId.GetValueOrDefault(0));
				List<CaracterizacaoLst> caracterizacoes = _busCaracterizacao.ObterCaracterizacoesPorProjetoDigital(projeto.Id);

				List<Caracterizacao> cadastradas = _busCaracterizacao.ObterCaracterizacoesEmpreendimento(projeto.EmpreendimentoId.GetValueOrDefault(0), projeto.Id) ?? new List<Caracterizacao>();
				List<Caracterizacao> cadastradasInterno = _internoBus.ObterCaracterizacoesAtuais(empreendimento.InternoID, caracterizacoes);

				if (cadastradas != null && cadastradas.Count > 0)
				{
					List<int> desatualizadas = new List<int>();
					if (!string.IsNullOrEmpty(_validarCaracterizacao.CaracterizacoesCadastradasDesatualizadas(projeto.EmpreendimentoId.GetValueOrDefault(), cadastradas, cadastradasInterno, out desatualizadas)))
					{
						GerenciadorConfiguracao<ConfiguracaoCaracterizacao> configCaracterizacao = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
						Validacao.Add(Mensagem.Caracterizacao.CaracterizacaoDesatualizadaEnviar(
							configCaracterizacao.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes)
							.Where(x => desatualizadas.Exists(y => y == x.Id))
							.Select(x => x.Texto).ToList()
						));
					}
				}
			}

			return Validacao.EhValido;
		}

		internal bool Enviar(Requerimento requerimento, ProjetoDigital projeto)
		{
			if (!EnviarBasicas(projeto))
			{
				return false;
			}

			if (requerimento.Id <= 0)
			{
				Validacao.Add(Mensagem.ProjetoDigital.RequerimentoObrigatorio);
			}
			else if (requerimento.SituacaoId != (int)eRequerimentoSituacao.Finalizado)
			{
				Validacao.Add(Mensagem.ProjetoDigital.FinalizarSituacaoInvalida);
			}

			if (requerimento.Interessado.Id <= 0)
			{
				Validacao.Add(Mensagem.ProjetoDigital.InteressadoObrigatorio);
			}

			if (!Validacao.EhValido)
			{
				return Validacao.EhValido;
			}

			_requerimentoCredenciadoValidar.Finalizar(requerimento);

			FinalizarCaracterizacoes(projeto);

			return Validacao.EhValido;
		}

		internal bool CancelarEnvio(ProjetoDigital projeto)
		{
			if (projeto.Situacao != (int)eProjetoDigitalSituacao.AguardandoImportacao)
			{
				Validacao.Add(Mensagem.ProjetoDigital.CancelarEnvioSituacaoInvalida);
			}

			var requerimentoDA =new RequerimentoCredenciadoDa();
			if (requerimentoDA.PossuiSolicitacaoCAREmProcessamento(projeto.RequerimentoId))
			{
				Validacao.Add(Mensagem.ProjetoDigital.CancelarEnvioSolicitacaoEmProcessamento);
			}

			if (requerimentoDA.PossuiSolicitacaoCAREnviadaSicar(projeto.RequerimentoId))
			{
				Validacao.Add(Mensagem.ProjetoDigital.CancelarEnvioSolicitacaoEnviadaSicar);
			}

			return Validacao.EhValido;
		}

		internal bool AssociarDependencias(ProjetoDigital projetoDigital)
		{
			Dependencia caracterizacao = projetoDigital.Dependencias.FirstOrDefault(x => x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.Caracterizacao) ?? new Dependencia();

			List<Dependencia> dependencias = _busCaracterizacao.ObterDependencias(
				caracterizacao.DependenciaId,
				(eCaracterizacao)caracterizacao.DependenciaCaracterizacao,
				eCaracterizacaoDependenciaTipo.Caracterizacao);

			string retorno = _validarCaracterizacao.DependenciasAlteradas(
				projetoDigital.EmpreendimentoId.GetValueOrDefault(),
				caracterizacao.DependenciaCaracterizacao,
				eCaracterizacaoDependenciaTipo.Caracterizacao,
				dependencias);

			if (!string.IsNullOrEmpty(retorno))
			{
				Validacao.Add(Mensagem.ProjetoDigital.AssociadaProjetoDigitalCaracterizacaoInvalida);
			}
			else
			{
				if ((eCaracterizacao)caracterizacao.DependenciaCaracterizacao == eCaracterizacao.UnidadeProducao)
				{
					ICaracterizacaoBus caracterizacaoBus = CaracterizacaoBusFactory.Criar(eCaracterizacao.UnidadeProducao);
					caracterizacaoBus.ValidarAssociar(caracterizacao.DependenciaId, projetoDigital.Id);
				}
			}
			
			return Validacao.EhValido;
		}

		public bool FinalizarCaracterizacoes(ProjetoDigital projetoDigital)
		{
			#region Configurar

			List<CaracterizacaoLst> caracterizacoes = _busCaracterizacao.ObterCaracterizacoesPorProjetoDigital(projetoDigital.Id);
			if (caracterizacoes.Count <= 0)
			{
				return true;
			}

			List<Caracterizacao> lista = new List<Caracterizacao>();
			caracterizacoes.ForEach(r =>
			{
				lista.Add(new Caracterizacao() { Tipo = (eCaracterizacao)r.Id });
			});

			lista = _busCaracterizacao.ObterCaracterizacoesAtuais(projetoDigital.EmpreendimentoId.GetValueOrDefault(), lista);

			lista.ForEach(r =>
			{
				if (r.Tipo == eCaracterizacao.UnidadeProducao)
				{
					ICaracterizacaoBus caracterizacaoBus = CaracterizacaoBusFactory.Criar(eCaracterizacao.UnidadeProducao);
					if (!caracterizacaoBus.PodeEnviar(r.Id))
					{
						return;
					}
				}

				projetoDigital.Dependencias.Add(new Dependencia()
				{
					DependenciaTipo = (int)eCaracterizacaoDependenciaTipo.Caracterizacao,
					DependenciaCaracterizacao = (int)r.Tipo,
					DependenciaId = r.Id,
					DependenciaTid = r.Tid
				});

				if (r.ProjetoId > 0)
				{
					projetoDigital.Dependencias.Add(new Dependencia()
					{
						DependenciaTipo = (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico,
						DependenciaCaracterizacao = (int)r.Tipo,
						DependenciaId = r.ProjetoId,
						DependenciaTid = r.ProjetoTid
					});
				}
			});

			#endregion

			if (!Validacao.EhValido)
			{
				return false;
			}

			AssociarDependencias(projetoDigital);

			List<CaracterizacaoLst> caracterizacoesObrigatorias = _busCaracterizacao.ObterCaracterizacoesPorProjetoDigital(projetoDigital.Id)
																  .Where(x => x.Permissao == ePermissaoTipo.Obrigatorio).ToList();

			List<Dependencia> dependenciasBanco = _da.ObterDependencias(projetoDigital.Id);
			foreach (var item in caracterizacoesObrigatorias)
			{
				if (!dependenciasBanco.Exists(x => x.DependenciaCaracterizacao == item.Id))
				{
					Validacao.Add(Mensagem.ProjetoDigital.CaracterizacaoObrigatoria(item.Texto));
				}
			}

			return Validacao.EhValido;
		}

		public bool EnviarProjeto(ProjetoDigital projetoDigital)
		{
			FinalizarCaracterizacoes(projetoDigital);

			return Validacao.EhValido;
		}
	}
}