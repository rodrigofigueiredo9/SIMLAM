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
	}
}
