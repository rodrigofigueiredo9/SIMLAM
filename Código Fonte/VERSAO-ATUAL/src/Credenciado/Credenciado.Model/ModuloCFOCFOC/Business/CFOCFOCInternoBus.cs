using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business
{
	public class CFOCFOCInternoBus
	{
		#region Propriedades

		CFOCFOCInternoDa _da = new CFOCFOCInternoDa();
		CFOCFOCInternoValidar _validar = new CFOCFOCInternoValidar();

		#endregion

		public void SetarNumeroUtilizado(string numero, int tipoNumero, eDocumentoFitossanitarioTipo tipoDocumento, string serieNumero)
		{
			GerenciadorTransacao.ObterIDAtual();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				bancoDeDados.IniciarTransacao();

                if (_da.SetarNumeroUtilizado(numero, tipoNumero, tipoDocumento, serieNumero) <= 0)
				{
					Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.NumeroNaoEncontrado);
				}

				bancoDeDados.Commit();
			}
		}

		public void VerificarCPFConsulta(string cpf)
		{
			_validar.ValidarCPFConsulta(cpf);
		}

		public List<NumeroCFOCFOC> FiltrarConsulta(ConsultaFiltro filtro)
		{
			List<NumeroCFOCFOC> retorno = new List<NumeroCFOCFOC>();

			try
			{
				if (!_validar.Filtrar(filtro))
				{
					return retorno;
				}

				retorno = _da.FiltrarConsulta(filtro);

				if (retorno == null || retorno.Count <= 0)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return retorno;
		}
	}
}