using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloConfiguracaoDocumentoFitossanitario.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloConfiguracaoDocumentoFitossanitario.Business
{
	public class ConfiguracaoDocumentoFitossanitarioValidar
	{
		#region Propriedades

		ConfiguracaoDocumentoFitossanitarioDa _da = new ConfiguracaoDocumentoFitossanitarioDa();

		#endregion

		public bool Salvar(ConfiguracaoDocumentoFitossanitario configuracao)
		{
			if (configuracao.DocumentoFitossanitarioIntervalos == null || configuracao.DocumentoFitossanitarioIntervalos.Count < 1)
			{
				Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.ValidaBloco);
				Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.ValidaDigital);
			}
			else
			{
				configuracao.DocumentoFitossanitarioIntervalos.ForEach(item =>
				{
					ValidarIntervalo(item, configuracao.DocumentoFitossanitarioIntervalos);
				});
			}

			return Validacao.EhValido;
		}

		public bool ValidarIntervalo(DocumentoFitossanitario intervalo, List<DocumentoFitossanitario> intervalos)
		{
			if (intervalo.TipoDocumentoID <= 0)
			{
				Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.TipoDocumentoObrigatorio(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString()));
			}

			if (intervalo.NumeroInicial <= 0)
			{
				Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.NumeroInicialObrigatorio(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString()));
			}
            else if (intervalo.NumeroInicial.ToString().Substring(2, 2) != DateTime.Now.Year.ToString().Substring(2, 2))
            {
                var temp1 = intervalo.NumeroInicial.ToString().Substring(2, 2);
                var temp2 = DateTime.Now.Year.ToString().Substring(2, 2);
                Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.AnoCorrenteObrigatorio(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString(), "NumeroInicial"));
            }
            else if (intervalo.NumeroFinal.ToString().Substring(2, 2) != DateTime.Now.Year.ToString().Substring(2, 2))
            {
                Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.AnoCorrenteObrigatorio(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString(), "NumeroFinal"));
            }
			else if (intervalo.NumeroInicial.ToString().Length != 10 
                && intervalo.TipoDocumentoTexto == "PTV")
			{
				Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.NumeroInicialInvalido(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString()));
            }
            else if (intervalo.NumeroInicial.ToString().Length != 8
                && (intervalo.TipoDocumentoTexto == "CFO" || intervalo.TipoDocumentoTexto == "CFOC")
                && (intervalo.ID <= 0) )
            {
                Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.NumeroInicialInvalidoCFOeCFOC(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString()));
            }
            else if ((intervalo.NumeroFinal.ToString().Length != 8 && intervalo.NumeroFinal.ToString().Length != 10)
                && (intervalo.TipoDocumentoTexto == "CFO" || intervalo.TipoDocumentoTexto == "CFOC"))
            {
                Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.NumeroInicialInvalidoCFOeCFOC(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString()));
            }

			if (intervalo.NumeroFinal <= 0)
			{
				Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.NumeroFinalObrigatorio(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString()));
			}
            else if (intervalo.NumeroFinal.ToString().Length != 10
                && intervalo.TipoDocumentoTexto == "PTV")
			{
				Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.NumeroFinalInvalido(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString()));
            }
            else if (intervalo.NumeroFinal.ToString().Length != 8
                && (intervalo.TipoDocumentoTexto == "CFO" || intervalo.TipoDocumentoTexto == "CFOC")
                && (intervalo.ID <= 0))
            {
                Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.NumeroFinalInvalidoCFOeCFOC(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString()));
            }
            else if ((intervalo.NumeroFinal.ToString().Length != 8 && intervalo.NumeroFinal.ToString().Length != 10)
                && (intervalo.TipoDocumentoTexto == "CFO" || intervalo.TipoDocumentoTexto == "CFOC"))
            {
                Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.NumeroFinalInvalidoCFOeCFOC(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString()));
            }
			else if (intervalo.NumeroFinal % 25 != 0)
			{
				Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.NumeroFinalMultiplo25(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString()));
			}

			if (intervalo.NumeroFinal <= intervalo.NumeroInicial)
			{
				Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.NumeroFinalMaiorInicial(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString()));
			}

			if (intervalos != null && intervalos.Count > 0)
			{
				intervalos.ForEach(item =>
				{
					if (!intervalo.Equals(item) && intervalo.TipoDocumentoID == item.TipoDocumentoID
						&& ((intervalo.NumeroInicial >= item.NumeroInicial && intervalo.NumeroInicial <= item.NumeroFinal) ||
							(item.NumeroInicial >= intervalo.NumeroInicial && item.NumeroInicial <= intervalo.NumeroFinal)))
					{
						Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.NumeroInicialExiste(intervalo.TipoDocumentoTexto));
					}
				});
			}

			return Validacao.EhValido;
		}

        public bool ValidarBusca(string tipoDocumentoID, string tipoNumeracaoID, string anoStr)
        {
            if (Convert.ToInt32(tipoDocumentoID) <= 0)
            {
                Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.TipoDocumentoObrigatorio(string.Empty));
            }

            if (Convert.ToInt32(tipoNumeracaoID) <= 0)
            {
                Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.TipoNumeracaoObrigatorio());
            }

            if (string.IsNullOrWhiteSpace(anoStr))
            {
                Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.AnoObrigatorio());
            }

            return Validacao.EhValido;
        }

	}
}