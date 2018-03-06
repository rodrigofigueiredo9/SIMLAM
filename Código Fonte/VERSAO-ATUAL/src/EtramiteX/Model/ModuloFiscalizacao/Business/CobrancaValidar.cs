using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class CobrancaValidar
	{
		public bool Salvar(Cobranca cobranca)
		{
			if (cobranca.NumeroAutos == 0)
				Validacao.Add(Mensagem.CobrancaMsg.NumeroAutosObrigatorio);

			if (cobranca.FiscalizacaoId == 0)
				Validacao.Add(Mensagem.CobrancaMsg.NumeroFiscalizacaoObrigatorio);

			if (string.IsNullOrWhiteSpace(cobranca.NumeroIUF))
				Validacao.Add(Mensagem.CobrancaMsg.NumeroIUFObrigatorio);

			return Validacao.EhValido;
		}
	}
}
