using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPTVOutro.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPTVOutro.Business
{
	public class PTVOutroBus
	{
		#region Propriedades

		PTVOutroDa _da = new PTVOutroDa();
		PTVOutroValidar _validar = new PTVOutroValidar();

		#endregion

		#region DML

		public bool Salvar(PTVOutro ptv)
		{
			try
			{
				if (_validar.Salvar(ptv))
				{
					ptv.Situacao = (int)ePTVOutroSituacao.Valido;
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(ptv, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.PTVOutro.Salvar);
					}
				}
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return Validacao.EhValido;
		}

		public bool PTVCancelar(PTVOutro ptv)
		{
			try
			{
				if (_validar.PTVCancelar(ptv))
				{
					GerenciadorTransacao.ObterIDAtual();
					ptv.DataCancelamento.DataTexto = DateTime.Today.ToShortDateString();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.PTVCancelar(ptv, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.PTVOutro.CanceladoSucesso);
					}
				}
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}
			return Validacao.EhValido;
		}

		public string VerificarNumeroPTV(string numero)
		{
			try
			{
				_validar.VerificarNumeroPTV(numero);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return numero;
		}

		#endregion

		#region Obter

		public PTVOutro Obter(int id, bool simplificado = false)
		{
			try
			{
				return _da.Obter(id, simplificado);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return null;
		}

		public List<Lista> ObterCultivar(eDocumentoFitossanitarioTipo origemTipo, int culturaID)
		{
			try
			{
				return _da.ObterCultivar(origemTipo, culturaID);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return new List<Lista>();
		}

		public DestinatarioPTV ObterDestinatario(int id)
		{
			try
			{
				DestinatarioPTVBus destinatarioBus = new DestinatarioPTVBus();
				return destinatarioBus.Obter(id);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return null;
		}

		public List<ListaValor> ObterResponsavelTecnico(int id)
		{
			try
			{
				return _da.ObterResponsavelTecnico(id);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return new List<ListaValor>();
		}

		public Resultados<PTVOutroListarResultado> Filtrar(PTVOutroListarFiltro ptvListarFiltro, Paginacao paginacao)
		{
			try
			{
				Filtro<PTVOutroListarFiltro> filtro = new Filtro<PTVOutroListarFiltro>(ptvListarFiltro, paginacao);
				Resultados<PTVOutroListarResultado> resultados = _da.Filtrar(filtro);

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

		#endregion
	}
}