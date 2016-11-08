using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDespolpamentoCafe;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDespolpamentoCafe.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDespolpamentoCafe.Business
{
	public class DespolpamentoCafeValidar
	{

		#region Propriedades

		DespolpamentoCafeDa _da = new DespolpamentoCafeDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CoordenadaAtividadeValidar _coordenadaValidar = new CoordenadaAtividadeValidar();

		#endregion

		internal bool Salvar(DespolpamentoCafe caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new DespolpamentoCafe()).Id > 0)
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
				Validacao.Add(Mensagem.DespolpamentoCafe.AtividadeObrigatoria);
			}

			_coordenadaValidar.Salvar(caracterizacao.CoordenadaAtividade);

			if (!String.IsNullOrWhiteSpace(caracterizacao.CapacidadeTotalInstalada))
			{
				Decimal aux = 0;
				if (Decimal.TryParse(caracterizacao.CapacidadeTotalInstalada, out aux))
				{
					if (aux <= 0)
					{
						Validacao.Add(Mensagem.DespolpamentoCafe.CapacidadeInstaladaMaiorZero);
					}
				}
				else 
				{
					Validacao.Add(Mensagem.DespolpamentoCafe.CapacidadeInstaladaInvalida);
				}
			}
			else 
			{
				Validacao.Add(Mensagem.DespolpamentoCafe.CapacidadeInstaladaObrigatoria);
			}


			if (!String.IsNullOrWhiteSpace(caracterizacao.AguaResiduariaCafe))
			{
				Decimal aux = 0;
				if (Decimal.TryParse(caracterizacao.AguaResiduariaCafe, out aux))
				{
					if (aux <= 0)
					{
						Validacao.Add(Mensagem.DespolpamentoCafe.AguaResiduariaCafeMaiorZero);
					}
				}
				else
				{
					Validacao.Add(Mensagem.DespolpamentoCafe.AguaResiduariaCafeInvalida);
				}
			}

			return Validacao.EhValido;

		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.DespolpamentoCafe))
			{
				return false;
			}

			return Validacao.EhValido;
		}
	}
}
