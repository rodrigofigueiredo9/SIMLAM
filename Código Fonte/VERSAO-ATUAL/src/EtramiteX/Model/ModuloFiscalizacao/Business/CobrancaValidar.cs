using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class CobrancaValidar
	{
		public bool Salvar(Cobranca cobranca)
		{
			if (cobranca.NumeroFiscalizacao == 0)
				Validacao.Add(Mensagem.CobrancaMsg.NumeroFiscalizacaoObrigatorio);

			if (string.IsNullOrWhiteSpace(cobranca.NumeroIUF))
				Validacao.Add(Mensagem.CobrancaMsg.NumeroIUFObrigatorio);

			if (!cobranca.DataIUF.IsValido)
				Validacao.Add(Mensagem.CobrancaMsg.DataIUFObrigatorio);

			if(cobranca.CodigoReceitaId == 0)
				Validacao.Add(Mensagem.CobrancaMsg.CodigoReceitaObrigatorio);

			if(string.IsNullOrWhiteSpace(cobranca?.AutuadoPessoa?.NomeRazaoSocial))
				Validacao.Add(Mensagem.CobrancaMsg.NomeAutuadoObrigatorio);

			if (string.IsNullOrWhiteSpace(cobranca?.AutuadoPessoa?.CPFCNPJ))
				Validacao.Add(Mensagem.CobrancaMsg.CpfCnpjObrigatorio);

			return Validacao.EhValido;
		}

		public bool Calcular(Cobranca cobranca, CobrancaParcelamento parcelamento)
		{
			if (!cobranca.DataIUF.IsValido)
				Validacao.Add(Mensagem.CobrancaMsg.DataIUFObrigatorio);

			if (!parcelamento.Data1Vencimento.IsValido)
				Validacao.Add(Mensagem.CobrancaMsg.DataVencimentoObrigatoria);

			return Validacao.EhValido;
		}

		public bool CalcularParametrizacao(Parametrizacao parametrizacao, Vrte vrte, Vrte vrteVencimento)
		{
			if (parametrizacao == null)
				Validacao.Add(Mensagem.CobrancaMsg.ParametrizacaoNaoEncontrada);

			if ((vrte?.Id ?? 0) == 0)
				Validacao.Add(Mensagem.CobrancaMsg.VrteNaoEncontrada);

			if ((vrteVencimento?.Id ?? 0) == 0)
				Validacao.Add(Mensagem.CobrancaMsg.VrteNaoEncontrada);

			return Validacao.EhValido;
		}
	}
}
