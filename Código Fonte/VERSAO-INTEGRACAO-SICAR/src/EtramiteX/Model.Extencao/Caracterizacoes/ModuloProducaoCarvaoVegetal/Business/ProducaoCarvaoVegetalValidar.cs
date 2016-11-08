using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProducaoCarvaoVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProducaoCarvaoVegetal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProducaoCarvaoVegetal.Business
{
	public class ProducaoCarvaoVegetalValidar
	{
		#region Propriedades

		ProducaoCarvaoVegetalDa _da = new ProducaoCarvaoVegetalDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CoordenadaAtividadeValidar _coordenadaValidar = new CoordenadaAtividadeValidar();

		#endregion

		internal bool Salvar(ProducaoCarvaoVegetal caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new ProducaoCarvaoVegetal()).Id > 0)
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
				Validacao.Add(Mensagem.ProducaoCarvaoVegetal.AtividadeObrigatoria);
			}

			_coordenadaValidar.Salvar(caracterizacao.CoordenadaAtividade);

			#region Fornos

			if (caracterizacao.Fornos.Count <= 0)
			{
				Validacao.Add(Mensagem.ProducaoCarvaoVegetal.FornoObrigatorio);
			}

			if (!String.IsNullOrWhiteSpace(caracterizacao.NumeroFornos))
			{
				if (Convert.ToDecimal(caracterizacao.NumeroFornos) <= 0) 
				{
					Validacao.Add(Mensagem.ProducaoCarvaoVegetal.NumeroFornosMaiorZero);
				}
			}
			else 
			{
				Validacao.Add(Mensagem.ProducaoCarvaoVegetal.NumeroFornosObrigatorio);
			}

			if (Convert.ToDecimal(caracterizacao.NumeroFornos) < caracterizacao.Fornos.Count)
			{
				Validacao.Add(Mensagem.ProducaoCarvaoVegetal.NumeroFornosMenorQueFornosAdicionados);
			}

			#endregion

			#region Materias

			if (caracterizacao.MateriasPrimasFlorestais.Count <= 0)
			{
				Validacao.Add(Mensagem.MateriaPrimaFlorestalConsumida.MateriaObrigatoria);
			}

			#endregion

			return Validacao.EhValido;

		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.ProducaoCarvaoVegetal))
			{
				return false;
			}

			return Validacao.EhValido;
		}
	}
}
