﻿using System;
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
			else if (intervalo.NumeroInicial.ToString().Length != 10)
			{
				Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.NumeroInicialInvalido(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString()));
			}

			if (intervalo.NumeroFinal <= 0)
			{
				Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.NumeroFinalObrigatorio(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString()));
			}
			else if (intervalo.NumeroFinal.ToString().Length != 10)
			{
				Validacao.Add(Mensagem.ConfiguracaoDocumentoFitossanitario.NumeroFinalInvalido(((eDocumentoFitossanitarioTipoNumero)intervalo.Tipo).ToString()));
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
	}
}