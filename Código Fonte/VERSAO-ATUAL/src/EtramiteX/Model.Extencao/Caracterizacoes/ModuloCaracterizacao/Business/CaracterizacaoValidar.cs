using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business
{
	public class CaracterizacaoValidar
	{
		#region Propriedades

		CaracterizacaoDa _da;
		ProjetoGeograficoBus _projetoGeoBus;
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig;

		#endregion

		public CaracterizacaoValidar()
		{
			_da = new CaracterizacaoDa();
			_projetoGeoBus = new ProjetoGeograficoBus();
			_caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
		}

		public bool Basicas(int empreendimento, bool isVisualizar = false)
		{
			if (!_da.ExisteEmpreendimento(empreendimento))
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoNaoEncontrado);
				return false;
			}

			if (!isVisualizar && !_da.EmPosse(empreendimento))
			{
				Validacao.Add(Mensagem.Caracterizacao.Posse);
				return false;
			}

			return Validacao.EhValido;
		}

		public bool Dependencias(int empreendimentoId, int caracterizacaoTipo, List<Caracterizacao> caracterizacoes = null, bool isDependencia = false, bool validarProjetoGeoProprio = true, bool validarDscLicAtividadeProprio = true)
		{
			List<DependenciaLst> dependencias = _caracterizacaoConfig.Obter<List<DependenciaLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoesDependencias);

			if (dependencias == null || dependencias.Count <= 0)
			{
				return true;
			}

			if (caracterizacoes == null)
			{
				caracterizacoes = _da.ObterCaracterizacoes(empreendimentoId);
			}

			List<CaracterizacaoLst> caracterizacoesCache = _caracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
			string caracterizacaoTexto = caracterizacoesCache.SingleOrDefault(x => x.Id == caracterizacaoTipo).Texto;
			dependencias = dependencias.Where(x => x.DependenteTipo == caracterizacaoTipo && x.TipoDetentorId == (int)eCaracterizacaoDependenciaTipo.Caracterizacao).ToList();

			Caracterizacao caracterizacao = null;

			foreach (DependenciaLst dependencia in dependencias)
			{
				caracterizacao = caracterizacoes.SingleOrDefault(y => (int)y.Tipo == dependencia.DependenciaTipo) ?? new Caracterizacao();

				switch (dependencia.TipoId)
				{
					case (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico:
						if (dependencia.DependenciaTipo == caracterizacaoTipo)
						{
							if (!validarProjetoGeoProprio)
							{
								continue;
							}
							else if (caracterizacao.ProjetoId > 0)
							{
								if (!string.IsNullOrEmpty(DependenciasAlteradas(empreendimentoId, caracterizacaoTipo, eCaracterizacaoDependenciaTipo.ProjetoGeografico,
									_da.ObterDependencias(caracterizacao.ProjetoId, (eCaracterizacao)caracterizacaoTipo, eCaracterizacaoDependenciaTipo.ProjetoGeografico))))
								{
									Validacao.Add(Mensagem.Caracterizacao.DependenciaDesatualizada(true, caracterizacaoTexto));
									continue;
								}
							}
						}

						if (caracterizacao.ProjetoId <= 0)
						{
							Validacao.Add(Mensagem.Caracterizacao.DependenciasProjetoGeoSalvar(caracterizacaoTexto, false, caracterizacoesCache.SingleOrDefault(x => x.Id == dependencia.DependenciaTipo).Texto));
							continue;
						}
						break;

					case (int)eCaracterizacaoDependenciaTipo.Caracterizacao:
						if (caracterizacao.Id <= 0)
						{
							Validacao.Add(Mensagem.Caracterizacao.DependenciasCaracterizacaoSalvar(caracterizacaoTexto, false, caracterizacoesCache.SingleOrDefault(x => x.Id == dependencia.DependenciaTipo).Texto));
							continue;
						}
						else if (validarProjetoGeoProprio || validarDscLicAtividadeProprio)
						{
							if (!string.IsNullOrEmpty(DependenciasAlteradas(empreendimentoId, dependencia.DependenciaTipo, eCaracterizacaoDependenciaTipo.Caracterizacao,
									_da.ObterDependencias(caracterizacao.Id, (eCaracterizacao)dependencia.DependenciaTipo, eCaracterizacaoDependenciaTipo.Caracterizacao))))
							{
								Validacao.Add(Mensagem.Caracterizacao.DependenciaDesatualizada(false, caracterizacoesCache.SingleOrDefault(x => x.Id == dependencia.DependenciaTipo).Texto));
								continue;
							}
						}

						Dependencias(empreendimentoId, (int)caracterizacao.Tipo, caracterizacoes, true);
						break;

					case (int)eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade:

						if (!validarDscLicAtividadeProprio)
						{
							if (!string.IsNullOrEmpty(DependenciasAlteradas(empreendimentoId, dependencia.DependenciaTipo, eCaracterizacaoDependenciaTipo.Caracterizacao,
									_da.ObterDependencias(caracterizacao.Id, (eCaracterizacao)dependencia.DependenciaTipo, eCaracterizacaoDependenciaTipo.Caracterizacao))))
							{
								Validacao.Add(Mensagem.Caracterizacao.DependenciaDesatualizada(false, caracterizacoesCache.SingleOrDefault(x => x.Id == dependencia.DependenciaTipo).Texto));
								continue;
							}
						}

						break;
				}
			}

			return Validacao.EhValido;
		}

		public string DependenciasAlteradas(int empreendimentoId, int caracterizacaoTipo, eCaracterizacaoDependenciaTipo dependenciaTipo, List<Dependencia> dependencias, bool isVisualizar=false)
		{
			List<Dependencia> dependenciasBanco = _da.ObterDependenciasAtual(empreendimentoId, (eCaracterizacao)caracterizacaoTipo, dependenciaTipo);
			List<CaracterizacaoLst> caracterizacoesLst = _caracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
			List<CaracterizacaoLst> caracterizacoes = new List<CaracterizacaoLst>();

			if (dependenciasBanco.Count > 0 && dependencias.Count <= 0)
			{
				foreach (var item in dependenciasBanco)
				{
					caracterizacoes.Add(new CaracterizacaoLst()
					{
						IsProjeto = item.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico,
						IsDescricao = item.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade,	
						Texto = caracterizacoesLst.SingleOrDefault(x => x.Id == item.DependenciaCaracterizacao).Texto
					});

					if (caracterizacoes.Count > 0)
					{
						return Mensagem.Caracterizacao.AtualizacaoDadosGeografico(caracterizacoes, 
							dependenciaTipo == eCaracterizacaoDependenciaTipo.ProjetoGeografico, 
							dependenciaTipo == eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade,
							isVisualizar).Texto;
					}
				}
				return String.Empty;
			}

			Dependencia dependencia = null;
			foreach (Dependencia item in dependencias)
			{
				dependencia = dependenciasBanco.SingleOrDefault(x => x.DependenciaId == item.DependenciaId && x.DependenciaCaracterizacao == item.DependenciaCaracterizacao
					&& x.DependenciaTipo == item.DependenciaTipo
					
					) ?? new Dependencia();

				if (item.DependenciaTid != dependencia.DependenciaTid)
				{
					caracterizacoes.Add(new CaracterizacaoLst()
					{
						IsProjeto = item.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico,
						IsDescricao = item.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade,
						Texto = caracterizacoesLst.SingleOrDefault(x => x.Id == item.DependenciaCaracterizacao).Texto
					});
				}
			}

			if (caracterizacoes.Count > 0)
			{
				return Mensagem.Caracterizacao.AtualizacaoDadosGeografico(caracterizacoes, 
					dependenciaTipo == eCaracterizacaoDependenciaTipo.ProjetoGeografico,
					dependenciaTipo == eCaracterizacaoDependenciaTipo.DescricaoLicenciamentoAtividade,
					isVisualizar).Texto;
			}

			return string.Empty;
		}

		internal bool DependenciasExcluir(int empreendimentoId, eCaracterizacao caracterizacaoTipo, eCaracterizacaoDependenciaTipo? dependenciaTipo = null)
		{
			List<Dependencia> dependentes = _da.ObterDependentes(empreendimentoId, caracterizacaoTipo, dependenciaTipo);
			List<CaracterizacaoLst> caracterizacoes = _caracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);

			foreach (Dependencia item in dependentes)
			{
				Validacao.Add(Mensagem.Caracterizacao.DependenciasExcluir(caracterizacoes.SingleOrDefault(x => x.Id == item.DependenciaCaracterizacao).Texto, item.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico));
			}

			return Validacao.EhValido;
		}

		public bool Existe(eCaracterizacao caracterizacaoTipo, int empreendimentoId)
		{
			List<Caracterizacao> caracterizacoes = _da.ObterCaracterizacoes(empreendimentoId);

			Caracterizacao caracterizacao = caracterizacoes.SingleOrDefault(x => x.Tipo == caracterizacaoTipo);

			if (caracterizacao.Id <= 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.CaracterizacaoInexistente(caracterizacao.Nome));

				return false;
			}

			return true;
		}

		public bool AtividadeLicencaObrigatoria(int atividade)
		{
			return _da.AtividadeLicencaObrigatoria(atividade);
		}
	}
}