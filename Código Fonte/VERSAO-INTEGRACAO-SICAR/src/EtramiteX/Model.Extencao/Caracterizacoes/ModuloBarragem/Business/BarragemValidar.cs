using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragem;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragem.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragem.Business
{
	public class BarragemValidar
	{
		BarragemDa _da = new BarragemDa();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		internal bool Salvar(Barragem caracterizacao)
		{
			if (caracterizacao.Barragens.Where(x => !x.Quantidade.HasValue).Count() > 0)
			{
				Validacao.Add(Mensagem.BarragemMsg.InformeQuantidade);
			}

			if (caracterizacao.Barragens.Where(x => x.Quantidade.HasValue && x.Quantidade == 0).Count() > 0)
			{
				Validacao.Add(Mensagem.BarragemMsg.InformeQuantidadeZero);
			}

			if (caracterizacao.Barragens.Where(x => x.Quantidade.GetValueOrDefault() > 0 && x.BarragensDados.Count > x.Quantidade.GetValueOrDefault()).Count() > 0)
			{
				Validacao.Add(Mensagem.BarragemMsg.QuantidadeInvalida);
			}

			if (caracterizacao.Barragens.Where(x => x.FinalidadeId == 0).Count() > 0)
			{
				Validacao.Add(Mensagem.BarragemMsg.SelecioneFinalidade);
			}

			if (caracterizacao.Barragens.Where(x => x.FinalidadeId == (int)eFinalidade.Outros && string.IsNullOrEmpty(x.Especificar)).Count() > 0)
			{
				Validacao.Add(Mensagem.BarragemMsg.InformeOutroFinalidade);
			}

			if (caracterizacao.Barragens.Where(x => x.BarragensDados.Count == 0).Count() > 0)
			{
				Validacao.Add(Mensagem.BarragemMsg.AddBarragemDadosItem);
			}

			caracterizacao.Barragens.ForEach(x =>
			{
				if (x.CoordenadaAtividade.Id == 0)
				{
					Validacao.Add(Mensagem.CoordenadaAtividade.CoordenadaAtividadeObrigatoria);
				}

				if (x.CoordenadaAtividade.Tipo == 0)
				{
					Validacao.Add(Mensagem.CoordenadaAtividade.GeometriaTipoObrigatorio);
				}
			});

			return Validacao.EhValido;
		}

		public bool Acessar(int empreendimentoId)
		{
			if (!_caracterizacaoValidar.Dependencias(empreendimentoId, (int)eCaracterizacao.Barragem))
			{
				return false;
			}

			return Validacao.EhValido;
		}
	}
}