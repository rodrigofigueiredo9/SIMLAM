using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.RelatorioPersonalizado.Data;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Business
{
	public class FatoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		FatoDa _da;

		public String UsuarioRelatorio
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioRelatorio); }
		}

		#endregion

		public FatoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new FatoDa(UsuarioRelatorio);
		}

		public Fato Obter(int id)
		{
            try
            {
				Fato resultado;
				string cacheKey = string.Format("Fato{0}", id);

				if (!GerenciadorCache.Get(cacheKey, out resultado))
				{   
					resultado = _da.Obter(id);
					ObterCamposListas(resultado);
					GerenciadorCache.SetCache(cacheKey, resultado);
				}
				else
				{   
					ObterCamposDinamicos(resultado);
					ObterCamposListas(resultado);
				}

				return resultado;
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

			return null;
		}

		private void ObterCamposDinamicos(Fato resultado)
		{
			resultado.Campos.RemoveAll(x => x.Codigo == 0);

			resultado.Dimensoes.ForEach(x => {
				x.Campos.RemoveAll(c => c.Codigo == 0);
			});

			resultado = _da.ObterCamposDinamicos(resultado);
		}

		public void ObterCamposListas(Fato fato)
		{
			List<Campo> camposComListas = fato.Campos.Where(c => !string.IsNullOrEmpty(c.Consulta)).ToList();

			ObterCamposListas(camposComListas.Where(x => x.SistemaConsultaEnum == eSistemaConsulta.Interno).ToList(), null);

			ObterCamposListas(camposComListas.Where(x => x.SistemaConsultaEnum == eSistemaConsulta.Relatorio).ToList(), UsuarioRelatorio);
		}

		public void ObterCamposListas(List<Campo> campos, string esquema)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(esquema))
			{
				foreach (Campo campo in campos)
				{
					_da.ObterCamposListas(campo, bancoDeDados);
				}
			}
		}

		public List<Lista> ObterFonteDados()
		{
			try
			{
				List<Lista> resultado;
				string cacheKey = "FatosDoSistema";

				if (!GerenciadorCache.Get(cacheKey, out resultado))
				{
					resultado = _da.ObterFonteDados();
					GerenciadorCache.SetCache(cacheKey, resultado);
				}

				return resultado;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}
	}
}