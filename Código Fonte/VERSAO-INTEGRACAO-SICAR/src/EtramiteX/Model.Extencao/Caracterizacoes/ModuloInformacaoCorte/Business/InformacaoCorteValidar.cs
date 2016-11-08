using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business
{
	public class InformacaoCorteValidar
	{
		#region Propriedades

		InformacaoCorteDa _da = new InformacaoCorteDa();
		CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		#endregion

		public bool Salvar(InformacaoCorte caracterizacao) 
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			if (caracterizacao.Id <= 0 && (_da.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId, true) ?? new InformacaoCorte()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!Acessar(caracterizacao.EmpreendimentoId))
			{
				return false;
			}

			InformacaoCorteInformacaoSalvar(caracterizacao.InformacaoCorteInformacao);

			return Validacao.EhValido;
		}

		public bool InformacaoCorteInformacaoSalvar(InformacaoCorteInformacao entidade) {

			if (entidade.Especies.Count <= 0) 
			{
				Validacao.Add(Mensagem.InformacaoCorte.EspecieListaObrigatorio);
			}

			if (entidade.Produtos.Count <= 0)
			{
				Validacao.Add(Mensagem.InformacaoCorte.ProdutoListaObrigatorio);
			}

			ValidacoesGenericasBus.DataMensagem(entidade.DataInformacao, "InformacaoCorteInformacao_DataInformacao_DataTexto", "informação");

			#region ArvoresIsoladasRestantes

			if (!String.IsNullOrWhiteSpace(entidade.ArvoresIsoladasRestantes))
			{
				Decimal aux = 0;
				if (!Decimal.TryParse(entidade.ArvoresIsoladasRestantes, out aux))
				{
					Validacao.Add(Mensagem.InformacaoCorte.ArvoresIsoladasRestantesInvalido);
				}
			}

			#endregion 

			#region AreaCorteRestante

			if (!String.IsNullOrWhiteSpace(entidade.AreaCorteRestante))
			{
				Decimal aux = 0;
				if (!Decimal.TryParse(entidade.AreaCorteRestante, out aux))
				{
					Validacao.Add(Mensagem.InformacaoCorte.AreaCorteRestantesInvalido);
				}
			}

			#endregion

			return Validacao.EhValido;
		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.InformacaoCorte))
			{
				return false;
			}

			return Validacao.EhValido;
		}
	}
}