using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloPatioLavagem;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloPatioLavagem.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloPatioLavagem.Business
{
	public class PatioLavagemValidar
	{
		#region Propriedades

		PatioLavagemDa _da = new PatioLavagemDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CoordenadaAtividadeValidar _coordenadaValidar = new CoordenadaAtividadeValidar();

		#endregion

		internal bool Salvar(PatioLavagem caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new PatioLavagem()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!Acessar(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Atividade <= 0)
			{
				Validacao.Add(Mensagem.PatioLavagem.AtividadeObrigatoria);
			}

			if (!String.IsNullOrWhiteSpace(caracterizacao.AreaTotal))
			{
				Decimal aux = 0;
				if (Decimal.TryParse(caracterizacao.AreaTotal, out aux))
				{
					if (aux <= 0)
					{
						Validacao.Add(Mensagem.PatioLavagem.AreaTotalMaiorZero);
					}
				}
				else
				{
					Validacao.Add(Mensagem.PatioLavagem.AreaTotalInvalida);
				}
			}
			else
			{
				Validacao.Add(Mensagem.PatioLavagem.AreaTotalObrigatoria);
			}

			_coordenadaValidar.Salvar(caracterizacao.CoordenadaAtividade);

			return Validacao.EhValido;

		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.PatioLavagem))
			{
				return false;
			}

			return Validacao.EhValido;
		}
	}
}
