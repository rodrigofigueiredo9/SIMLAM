using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class CobrancaParcelamentoValidar
	{
		public bool Salvar(CobrancaParcelamento parcelamento)
		{
			if (parcelamento.QuantidadeParcelas == 0)
				Validacao.Add(Mensagem.CobrancaMsg.QuantidadeParcelasObrigatorio);

			if (!parcelamento.Data1Vencimento.IsValido)
				Validacao.Add(Mensagem.CobrancaMsg.DataVencimentoObrigatoria);

			if (!parcelamento.DataEmissao.IsValido)
				Validacao.Add(Mensagem.CobrancaMsg.DataEmissaoObrigatoria);

			return Validacao.EhValido;
		}
	}
}
