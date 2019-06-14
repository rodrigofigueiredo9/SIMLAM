using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Data;
using Cred = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Data;


namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business
{
	public class ProjetoGeograficoValidar
	{
		ProjetoGeograficoDa _da = new ProjetoGeograficoDa();
		CaracterizacaoDa _caracterizacaoDa = new CaracterizacaoDa();
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());

		internal bool EnviarArquivo(ArquivoProjeto arquivoEnviado)
		{
			if (arquivoEnviado == null)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.ArquivoObrigatorio);
				return false;
			}

			if (arquivoEnviado.Extensao != ".zip")
			{
				Validacao.Add(Mensagem.ProjetoGeografico.ArquivoAnexoNaoEhZip);
				return false;
			}
			return true;
		}

		internal bool Recarregar(ProjetoGeografico projeto)
		{
			CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
			if (!_busCaracterizacao.ExisteEmpreendimento(projeto.EmpreendimentoId))
			{
				Validacao.Add(Mensagem.ProjetoGeografico.EmpreendimentoInexistente);
			}

			if (_da.ExisteProjetoGeografico(projeto.EmpreendimentoId, projeto.CaracterizacaoId, true) <= 0)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.SituacaoProjetoDeveSerFinalizar);
			}
			return Validacao.EhValido;
		}

		internal bool Refazer(ProjetoGeografico projeto)
		{
			CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
			if (!_busCaracterizacao.ExisteEmpreendimento(projeto.EmpreendimentoId))
			{
				Validacao.Add(Mensagem.ProjetoGeografico.EmpreendimentoInexistente);
			}

			if (_da.ExisteProjetoGeografico(projeto.EmpreendimentoId, projeto.CaracterizacaoId, true) <= 0)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.SituacaoProjetoDeveSerFinalizar);
			}
			return Validacao.EhValido;
		}

		public bool Finalizar(ProjetoGeografico projeto)
		{
			CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
			if (!_busCaracterizacao.ExisteEmpreendimento(projeto.EmpreendimentoId))
			{
				Validacao.Add(Mensagem.ProjetoGeografico.EmpreendimentoInexistente);
				return false;
			}

			if (_da.ObterSituacaoProjetoGeografico(projeto.Id) == (int)eProjetoGeograficoSituacao.Finalizado)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.SituacaoProjetoFinalizado);
				return false;
			}

			if (!_da.VerificarProjetoGeograficoProcessado(projeto.Id, (eCaracterizacao)projeto.CaracterizacaoId))
			{
				Validacao.Add(Mensagem.ProjetoGeografico.SituacaoDeveSerProcessado);
				return false;
			}

			if (projeto.Sobreposicoes == null || projeto.Sobreposicoes.Itens == null || projeto.Sobreposicoes.Itens.Count == 0 ||
				(!ValidarDataVerificacao(projeto.Sobreposicoes) && projeto.CaracterizacaoId != (int)eCaracterizacao.ExploracaoFlorestal))
			{
				Validacao.Add(Mensagem.ProjetoGeografico.VerificarSobreposicao);
			}

			if (projeto.CaracterizacaoId == (int)eCaracterizacao.Dominialidade)
			{
				List<string> auxiliar = _da.VerificarExcluirDominios(projeto.EmpreendimentoId);
				if (auxiliar != null && auxiliar.Count > 0)
				{
					foreach (var item in auxiliar)
					{
						Validacao.Add(Mensagem.Dominialidade.DominioAssossiadoReserva(item));
					}
				}
			}

			return Validacao.EhValido;
		}

		private bool ValidarDataVerificacao(Sobreposicoes sobreposicoes)
		{
			if (string.IsNullOrEmpty(sobreposicoes.DataVerificacao))
				return false;

			DateTime dataVerificacao = DateTime.ParseExact(sobreposicoes.DataVerificacao, "dd/MM/yyyy - HH:mm", CultureInfo.CurrentCulture.DateTimeFormat);

			if (dataVerificacao < DateTime.Now.AddMinutes(-30d))
			{
				return false;
			}
			return true;
		}

		public bool ExisteEmpreendimento(int id)
		{
			CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
			if (!_busCaracterizacao.ExisteEmpreendimento(id))
			{
				Validacao.Add(Mensagem.ProjetoGeografico.EmpreendimentoNaoCadastrao);
			}
			return Validacao.EhValido;
		}

		internal bool Salvar(ProjetoGeografico projeto)
		{
			if (_da.ExisteProjetoGeografico(projeto.EmpreendimentoId, projeto.CaracterizacaoId) != projeto.Id)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.JaExisteCadastro);
				return false;
			}

			if (projeto.NivelPrecisaoId <= 0)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.NivelPrecisaoObrigatorio);
				return false;
			}

			if (projeto.MaiorX <= 0 || projeto.MaiorY <= 0 || projeto.MenorX <= 0 || projeto.MenorY <= 0)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.AreaDeAbrangenciaObrigatorio);
				return false;
			}

			if (projeto.MecanismoElaboracaoId == 0)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.MecanismoObrigatorio);
				return false;
			}

			CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
			if (!_busCaracterizacao.ExisteEmpreendimento(projeto.EmpreendimentoId))
			{
				Validacao.Add(Mensagem.ProjetoGeografico.EmpreendimentoInexistente);
			}

			return Validacao.EhValido;
		}

		public bool Dependencias(int empreendimentoId, int caracterizacaoTipo, List<Caracterizacao> caracterizacoes = null, bool isDependencia = false)
		{
			List<DependenciaLst> dependencias = _caracterizacaoConfig.Obter<List<DependenciaLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoesDependencias);

			if (dependencias == null || dependencias.Count <= 0)
			{
				return true;
			}

			if (caracterizacoes == null)
			{
				caracterizacoes = _caracterizacaoDa.ObterCaracterizacoes(empreendimentoId);
			}

			List<CaracterizacaoLst> caracterizacoesCache = _caracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
			dependencias = dependencias.Where(x => x.DependenteTipo == caracterizacaoTipo && x.TipoDetentorId == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico).ToList();
			string caracterizacaoTexto = caracterizacoesCache.SingleOrDefault(x => x.Id == caracterizacaoTipo).Texto;

			Caracterizacao caracterizacao = null;

			foreach (DependenciaLst dependencia in dependencias)
			{
				caracterizacao = caracterizacoes.SingleOrDefault(y => (int)y.Tipo == dependencia.DependenciaTipo) ?? new Caracterizacao();

				if (caracterizacao.ProjetoId <= 0)
				{
					Validacao.Add(Mensagem.Caracterizacao.DependenciasProjetoGeoSalvar(caracterizacaoTexto, true, caracterizacoesCache.SingleOrDefault(x => x.Id == dependencia.DependenciaTipo).Texto));
					continue;
				}

				if (caracterizacaoTipo != (int)caracterizacao.Tipo && _da.ObterProjetoGeografico(caracterizacao.ProjetoId).Tid != caracterizacao.ProjetoTid)
				{
					Validacao.Add(Mensagem.Caracterizacao.DependenciasProjetoGeoSalvar(caracterizacaoTexto, true, caracterizacoesCache.SingleOrDefault(x => x.Id == dependencia.DependenciaTipo).Texto));
					continue;
				}
			}

			return Validacao.EhValido;
		}

		public bool EhFinalizado(int projetoId)
		{
			if (_da.ObterSituacaoProjetoGeografico(projetoId) != (int)eProjetoGeograficoSituacao.Finalizado)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.SituacaoProjetoNaoFinalizado);
				return false;
			}
			return Validacao.EhValido;
		}

		internal bool CopiarDadosCredenciado(ProjetoGeografico projeto)
		{
			Cred.ProjetoGeograficoDa projetoGeoDa = new Cred.ProjetoGeograficoDa();

			if (projeto.Id <= 0 || projeto.SituacaoId != (int)eProjetoGeograficoSituacao.Finalizado)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.CopiarProjetoGeoFinalizado);
			}

			Empreendimento empreendimento = projetoGeoDa.ObterEmpreendimentoCoordenada(projeto.EmpreendimentoId);
			projeto.EmpreendimentoEasting = empreendimento.Coordenada.EastingUtm.Value;
			projeto.EmpreendimentoNorthing = empreendimento.Coordenada.NorthingUtm.Value;

			if (!projeto.EmpreendimentoEstaDentroAreaAbrangencia)
			{
				Validacao.Add(Mensagem.ProjetoGeografico.EmpreendimentoForaAbrangenciaCopiar);
			}

			return Validacao.EhValido;
		}
	}
}