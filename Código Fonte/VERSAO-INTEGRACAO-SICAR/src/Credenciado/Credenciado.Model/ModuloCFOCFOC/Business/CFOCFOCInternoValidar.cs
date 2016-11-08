using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business
{
	public class CFOCFOCInternoValidar
	{
		#region Propriedades

		CFOCFOCInternoDa _da = new CFOCFOCInternoDa();

		#endregion

		internal void ValidarCPFConsulta(string cpf)
		{
			if (string.IsNullOrEmpty(cpf))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.CPFObrigatorio);
				return;
			}

			if (!ValidacoesGenericasBus.Cpf(cpf))
			{
				Validacao.Add(Mensagem.Pessoa.CpfInvalido);
				return;
			}

			if (!_da.VerificarCPFAssociadoALiberacao(cpf))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.CPFNaoAssociadoAUmaLiberacao);
			}
		}

		internal bool Filtrar(ConsultaFiltro filtro)
		{
			if (!string.IsNullOrEmpty(filtro.DataInicialEmissao))
			{
				if (!ValidacoesGenericasBus.ValidarData(filtro.DataInicialEmissao))
				{
					Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.DataInvalida("Data inicial da emissão", "DataInicial" + ((eDocumentoFitossanitarioTipoNumero)filtro.TipoNumero).ToString()));
				}
			}

			if (!string.IsNullOrEmpty(filtro.DataFinalEmissao))
			{
				if (!ValidacoesGenericasBus.ValidarData(filtro.DataFinalEmissao))
				{
					Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.DataInvalida("Data final da emissão", "DataFinal" + ((eDocumentoFitossanitarioTipoNumero)filtro.TipoNumero).ToString()));
				}
			}

			return Validacao.EhValido;
		}
	}
}