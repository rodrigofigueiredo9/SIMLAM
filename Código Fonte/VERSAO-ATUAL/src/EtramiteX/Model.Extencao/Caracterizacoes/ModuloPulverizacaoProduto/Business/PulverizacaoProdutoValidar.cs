using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloPulverizacaoProduto;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloPulverizacaoProduto.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloPulverizacaoProduto.Business
{
	public class PulverizacaoProdutoValidar
	{
		#region Propriedades

		PulverizacaoProdutoDa _da = new PulverizacaoProdutoDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		CoordenadaAtividadeValidar _coordenadaValidar = new CoordenadaAtividadeValidar();

		#endregion

		internal bool Salvar(PulverizacaoProduto caracterizacao)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new PulverizacaoProduto()).Id > 0)
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
				Validacao.Add(Mensagem.PulverizacaoProduto.AtividadeObrigatoria);
			}

			if (String.IsNullOrWhiteSpace(caracterizacao.EmpresaPrestadora))
			{
				Validacao.Add(Mensagem.PulverizacaoProduto.EmpresaPrestadoraObrigatoria);
			}

			if (String.IsNullOrWhiteSpace(caracterizacao.CNPJ))
			{
				Validacao.Add(Mensagem.PulverizacaoProduto.CnpjObrigatorio);
			}
			else 
			{
				if (!ValidacoesGenericasBus.Cnpj(caracterizacao.CNPJ))
				{
					Validacao.Add(Mensagem.PulverizacaoProduto.CnpjInvalido);
				}
			}

			if (caracterizacao.Culturas.Count <= 0) 
			{
				Validacao.Add(Mensagem.PulverizacaoProduto.CulturaObrigatoria);
			}

			_coordenadaValidar.Salvar(caracterizacao.CoordenadaAtividade);

			return Validacao.EhValido;

		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.PulverizacaoProduto))
			{
				return false;
			}

			return Validacao.EhValido;
		}
	}
}