using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business
{
	public class TituloModeloInternoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		TituloModeloInternoDa _da;

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion

		public TituloModeloInternoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new TituloModeloInternoDa(UsuarioInterno);
		}

		public TituloModelo Obter(int id)
		{
			try
			{
				TituloModelo tituloModelo = _da.Obter(id);

				if (tituloModelo.Regra(eRegra.FaseAnterior))
				{
					foreach (var item in tituloModelo.Respostas(eRegra.FaseAnterior, eResposta.Modelo))
					{
						if (tituloModelo.Id != Convert.ToInt32(item.Valor))
						{
							TituloModelo titulo = _da.ObterSimplificado(Convert.ToInt32(item.Valor));

							titulo.IdRelacionamento = item.Id;

							tituloModelo.Modelos.Add(titulo);
						}
					}
				}

				return tituloModelo;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public TituloModelo ObterSimplificado(int id)
		{
			try
			{
				return _da.ObterSimplificado(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<TituloModeloLst> ObterModelos(int exceto = 0, bool todos = false)
		{
			try
			{
				List<TituloModeloLst> modelos = _da.ObterModelos(exceto, todos);

				return modelos;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<TituloModeloLst> ObterModelosAnteriores(int modelo)
		{
			try
			{
				return _da.ObterModelosAnteriores(modelo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<TituloModeloLst> ObterModelosRenovacao(int modelo)
		{
			return _da.ObterModelosRenovacao(modelo);
		}

		public List<TituloModeloLst> ObterModelosDeclaratorios()
		{
			try
			{
				return _da.ObterModelosDeclaratorios();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}
	}
}