﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTVOutro.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Arquivo.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTVOutro.Business
{
	public class PTVOutroBus
	{
		#region Propriedades
        private GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		PTVOutroDa _da = new PTVOutroDa();
		PTVOutroValidar _validar = new PTVOutroValidar();

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

        public String UsuarioCredenciado
        {
            get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
        }



		#endregion

		#region DML

        public List<ListaValor> ObterListaDeclaracao(int pragaId, int[] cultivarId )
        {
            return _da.ObterListaDeclaracao(pragaId, cultivarId);
        }

        public bool PodeEditar(Int64 numeroPTV)
        {
            return _da.VerificaSePTVAssociadoLote(numeroPTV);
        }

        public bool Salvar(PTVOutro ptv, BancoDeDados banco = null)
		{
			try
			{
				ptv.DestinatarioID = ptv.Destinatario.ID;

				if (_validar.Salvar(ptv))
				{

                    #region Arquivos/Diretorio

                    ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

                 
                    if (ptv.Anexos != null && ptv.Anexos.Count > 0)
                    {
                        foreach (Anexo anexo in ptv.Anexos)
                        {
                            if (!String.IsNullOrWhiteSpace(anexo.Arquivo.TemporarioNome) && anexo.Arquivo.Id == 0)
                            {
                                anexo.Arquivo = _busArquivo.Copiar(anexo.Arquivo);
                            }
                        }

                        using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
                        {
                            bancoDeDados.IniciarTransacao();

                            
                            ArquivoDa arquivoDa = new ArquivoDa();

                            if (ptv.Anexos != null && ptv.Anexos.Count > 0)
                            {
                                foreach (Anexo anexo in ptv.Anexos)
                                {
                                    if (!String.IsNullOrWhiteSpace(anexo.Arquivo.TemporarioNome) && anexo.Arquivo.Id == 0)
                                    {
                                        arquivoDa.Salvar(anexo.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Credenciado, User.FuncionarioTid, bancoDeDados);
                                    }
                                }
                            }

                            bancoDeDados.Commit();
                        }
                    }

                    #endregion

                    ptv.Situacao = (int)ePTVOutroSituacao.Valido;
					ptv.CredenciadoId = User.FuncionarioId;

					GerenciadorTransacao.ObterIDAtual();

                    using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
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

        public List<Lista> ObterPragasLista(List<PTVOutroProduto> produtos)
        {
            try
            {
                return _da.ObterPragasLista(produtos);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }
            return null;
        }

		public PTVOutro ObterPorNumero(long numero, bool simplificado = false, bool credenciado = true)
		{
			try
			{
				return _da.ObterPorNumero(numero, simplificado, credenciado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
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

                foreach (PTVOutroListarResultado lis in resultados.Itens)
                {
                    if (_da.VerificaSePTVAssociadoLote(Convert.ToInt64(lis.Numero)))
                        lis.PodeEditar = false;
                    else
                        lis.PodeEditar = true;
                }

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