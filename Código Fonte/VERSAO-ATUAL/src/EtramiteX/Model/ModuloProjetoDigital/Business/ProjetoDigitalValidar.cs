using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProjetoDigital.Business
{
	public class ProjetoDigitalValidar
	{
		#region Propriedades

		RequerimentoCredenciadoDa _da;
		PessoaBus _busPessoa;

		#endregion

		public ProjetoDigitalValidar()
		{
			_da = new RequerimentoCredenciadoDa();
			_busPessoa = new PessoaBus();
		}

		public bool ObjetivoPedido(Requerimento requerimento)
		{
			if (requerimento.SetorId <= 0)
			{
				Validacao.Add(Mensagem.Requerimento.SetorObrigatorio);
			}

			requerimento.Atividades.ForEach(atividade =>
			{
				if (atividade.SituacaoId == (int)eAtividadeSituacao.Desativada)
				{
					Validacao.Add(Mensagem.ProjetoDigital.AtividadeDesativadaInformacao(atividade.NomeAtividade));
				}
			});

			return Validacao.EhValido;
		}

		public bool Importar(Requerimento requerimento, List<Pessoa> pessoasRelacionadas)
		{
			ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
			ProjetoDigital projeto = projetoDigitalCredenciadoBus.Obter(idRequerimento: requerimento.Id);

			if (projeto.Situacao != (int)eProjetoDigitalSituacao.AguardandoImportacao)
			{
				Validacao.Add(Mensagem.ProjetoDigital.SituacaoImportar);
				return false;
			}

			if (!_da.ValidarVersaoAtual(requerimento.Id, requerimento.Tid))
			{
				Validacao.Add(Mensagem.ProjetoDigital.DadosDesatualizadoImportacao);
				return false;
			}

			if (requerimento.Interessado.Id <= 0)
			{
				Validacao.Add(Mensagem.ProjetoDigital.InteressadoObrigatorio);
				return false;
			}

			if (requerimento.Pessoas == null || requerimento.Pessoas.Count <= 0)
			{
				Validacao.Add(Mensagem.ProjetoDigital.InteressadoObrigatorio);
			}

			foreach (var item in requerimento.Pessoas.Where(x => x.IsFisica))
			{
				if (item.SelecaoTipo == (int)eExecutorTipo.Credenciado && pessoasRelacionadas.Exists(x => x.Fisica.CPF == item.Fisica.CPF))
				{
					var pessoaRel = _busPessoa.Obter(item.Fisica.CPF);

					if (pessoaRel != null && pessoaRel.Id > 0 &&
						pessoaRel.Fisica.ConjugeCPF != item.Fisica.ConjugeCPF &&
						pessoasRelacionadas.Count(x => x.Fisica.ConjugeCPF == item.Fisica.CPF) > 1)
					{
						Validacao.Add(Mensagem.ProjetoDigital.PessoaCredenciadoConflito);
						break;
					}
				}
			}

			if (requerimento.SetorId <= 0)
			{
				Validacao.Add(Mensagem.Requerimento.SetorObrigatorio);
			}

			requerimento.Atividades.ForEach(atividade =>
			{
				if (atividade.SituacaoId == (int)eAtividadeSituacao.Desativada)
				{
					Validacao.Add(Mensagem.ProjetoDigital.AtividadeDesativada(atividade.NomeAtividade));
				}
			});

			return Validacao.EhValido;
		}

		internal bool Recusar(ProjetoDigital projeto)
		{
			if (projeto.Situacao != (int)eProjetoDigitalSituacao.AguardandoImportacao)
			{
				Validacao.Add(Mensagem.ProjetoDigital.SituacaoRecusar);
				return false;
			}

			if (string.IsNullOrEmpty(projeto.MotivoRecusa))
			{
				Validacao.Add(Mensagem.ProjetoDigital.MotivoObrigatorio);
			}

			return Validacao.EhValido;
		}
	}
}