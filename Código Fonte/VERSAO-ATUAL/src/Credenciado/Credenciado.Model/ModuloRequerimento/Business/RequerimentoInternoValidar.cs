using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business
{
	public class RequerimentoInternoValidar
	{
		internal bool ObterNumerosTitulos(string numero, int modeloId)
		{
			if (modeloId < 1)
			{
				Validacao.Add(Mensagem.Requerimento.TituloAnteriorObrigatorioModal);
			}

			if (string.IsNullOrWhiteSpace(numero))
			{
				Validacao.Add(Mensagem.Requerimento.NumeroAnteriorObrigatorioModal);
			}

			if (!(ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(numero) || ValidacoesGenericasBus.ValidarNumero(numero, 12)))
			{
				Validacao.Add(Mensagem.Requerimento.TituloAnteriorNumeroInvalido);
			}

			return Validacao.EhValido;
		}

	}
}
