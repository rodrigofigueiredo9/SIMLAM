using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloIndicador.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloIndicador.Business
{
	public class RelatorioIndicadorBus
	{
		GerenciadorConfiguracao<ConfiguracaoTitulo> _configTitulo = new GerenciadorConfiguracao<ConfiguracaoTitulo>(new ConfiguracaoTitulo());
		RelatorioIndicadorDa _da = new RelatorioIndicadorDa();

		public List<Situacao> TituloSituacoes
		{
			get { return _configTitulo.Obter<List<Situacao>>(ConfiguracaoTitulo.KeySituacoes); }
		}

		public List<Situacao> TituloCondicionanteSituacoes
		{
			get { return _configTitulo.Obter<List<Situacao>>(ConfiguracaoTitulo.KeyTituloCondicionanteSituacoes); }
		}

		public IndicadorPeriodoRelatorio BuscarTitulosIndicadores()
		{
			return _da.RelatorioTituloIndicadores();
		}

		public IndicadorPeriodoRelatorio BuscarCondicionantesIndicadores()
		{
			return _da.RelatorioTituloCondicionantesIndicadores();
		}

		public static bool ValidarData(String data)
		{
			try
			{
				DateTime.Parse(data).ToString("dd/MM/yyyy");
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool ValidarData(DateTime data)
		{
			return ValidarData(data.ToShortDateString());
		}

		public static DateTime? ParseData(String dataTexto)
		{

			if (String.IsNullOrEmpty(dataTexto))
			{
				return DateTime.MinValue;
			}
			else if (ValidarData(dataTexto) && dataTexto != DateTime.MinValue.ToShortDateString())
			{
				return DateTime.Parse(dataTexto);
			}
			else
			{
				return null;
			}
		}
	}
}