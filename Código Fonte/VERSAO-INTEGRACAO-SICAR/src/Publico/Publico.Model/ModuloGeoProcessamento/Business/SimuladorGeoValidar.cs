using System;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Data;
using Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Entities;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Business
{
	public class SimuladorGeoValidar
	{
		internal bool EnviarArquivo(SimuladorGeo projeto)
		{
			if (String.IsNullOrWhiteSpace(projeto.Easting))
			{
				Validacao.Add(Mensagem.SimuladorGeo.EastingObrigatorio);
			} 
			else if (!ValidacoesGenericasBus.ValidarDecimal(projeto.Easting, 7, 4))
			{
				Validacao.Add(Mensagem.SimuladorGeo.EastingInvalido);
			}

			if (String.IsNullOrWhiteSpace(projeto.Northing))
			{
				Validacao.Add(Mensagem.SimuladorGeo.NorthingObrigatorio);
			}
			else if (!ValidacoesGenericasBus.ValidarDecimal(projeto.Northing, 7, 4))
			{
				Validacao.Add(Mensagem.SimuladorGeo.NorthingInvalido);
			}

			if (!Validacao.EhValido)
			{
				return false;
			}

			SimuladorGeoDa da = new SimuladorGeoDa();
			if (da.PontoForaMBR(projeto.Easting, projeto.Northing))
			{
				Validacao.Add(Mensagem.Sistema.CoordenadaForaMBR);
				return false;
			}

			if (projeto.ArquivoEnviado.Extensao != ".zip")
			{
				Validacao.Add(Mensagem.SimuladorGeo.ArquivoAnexoNaoEhZip);
				return false;
			}

			return Validacao.EhValido;
		}

		internal bool VerificarCpf(string cpf)
		{
			if (String.IsNullOrEmpty(cpf))
			{
				Validacao.Add(Mensagem.SimuladorGeo.CpfObrigatorio);
			}
			else if (!ValidacoesGenericasBus.Cpf(cpf))
			{
				Validacao.Add(Mensagem.SimuladorGeo.CpfInvalido);
			}
			return Validacao.EhValido;
		}
	}
}