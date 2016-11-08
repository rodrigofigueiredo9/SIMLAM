using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAgrotoxico.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloAgrotoxico.Business
{
	public class AgrotoxicoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		AgrotoxicoDa _da;
		
		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public List<QuantPaginacao> LstQuantPaginacao
		{
			get { return _configSys.Obter<List<QuantPaginacao>>(ConfiguracaoSistema.KeyLstQuantPaginacao); }
		}

		#endregion

		public AgrotoxicoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());			
			_da = new AgrotoxicoDa(UsuarioCredenciado);
		}

		#region Obter/Listar

		public Resultados<AgrotoxicoFiltro> Filtrar(AgrotoxicoFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				//Seta o id do usuário logado				
				var filtros = new Filtro<AgrotoxicoFiltro>(filtrosListar, paginacao);
				var resultados = _da.Filtrar(filtros);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}


		public Arquivo ObterAgrotoxicoArquivo(int id)
		{
			Arquivo retorno;
			try
			{
				ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
				
				if (_busArquivo.Existe(id))
				{
					retorno = _busArquivo.Obter(id);
					return retorno;
				}
				else
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Lista> AgrotoxicoSituacao()
		{
			try
			{
				var retorno = new List<Lista>();
				retorno.Add(new Lista()
				{
					Id = "1",
					Texto = "Ativo",
					IsAtivo = true
				});

				retorno.Add(new Lista()
				{
					Id = "2",
					Texto = "Inativo",
					IsAtivo = true
				});

				return retorno;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Lista> AgrotoxicoClasseUso()
		{
			try
			{
				return _da.ObterAgrotoxicoClasseUso(); 				
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Lista> AgrotoxicoModalidadeAplicacao()
		{
			try
			{
				return _da.ObterAgrotoxicoModalidadeAplicacao();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Lista> AgrotoxicoGrupoQuimico()
		{
			try
			{
				return _da.ObterAgrotoxicoGrupoQuimico();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Lista> AgrotoxicoClassificacaoToxicologica()
		{
			try
			{
				return _da.ObterAgrotoxicoClassificacaoToxicologica();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion
	}
}