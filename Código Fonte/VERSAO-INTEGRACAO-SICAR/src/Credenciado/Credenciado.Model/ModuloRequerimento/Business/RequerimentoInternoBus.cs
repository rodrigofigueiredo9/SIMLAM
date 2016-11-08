using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business
{
	public class RequerimentoInternoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		RequerimentoInternoDa _da;
		RequerimentoInternoValidar _validar;

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion
		
		public RequerimentoInternoBus() : this(new RequerimentoInternoValidar()) { }

		public RequerimentoInternoBus(RequerimentoInternoValidar validacao)
		{
			_validar = validacao;
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new RequerimentoInternoDa(UsuarioInterno);
		}

		public int ObterNovoID(BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				return _da.ObterNovoID(bancoDeDados);
			}
		}

		public List<TituloModeloLst> ObterNumerosTitulos(string numero, int modeloId)
		{
			TituloModeloInternoBus bus = new TituloModeloInternoBus();
			List<TituloModeloLst> titulos = new List<TituloModeloLst>();

			try
			{
				if (!_validar.ObterNumerosTitulos(numero, modeloId))
				{
					return titulos;
				}

				if (ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(numero))
				{
					titulos = _da.ObterNumerosTitulos(numero, modeloId);

					switch (titulos.Count)
					{
						case 0:
							Validacao.Add(Mensagem.Requerimento.TituloNaoEncontrado);
							break;

						case 1:
							Validacao.Add(Mensagem.Requerimento.TituloEncontrado);
							break;

						default:
							Validacao.Add(Mensagem.Requerimento.TitulosEncontrados);
							break;
					}
				}
				else
				{
					if (_da.ValidarNumeroSemAnoExistente(numero, modeloId))
					{
						Validacao.Add(Mensagem.Requerimento.TituloNumeroSemAnoEncontrado);
					}
					else
					{
						Validacao.Add(Mensagem.Requerimento.TituloNaoEncontrado);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return titulos;
		}

	}
}
