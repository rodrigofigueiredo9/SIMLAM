﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro;
using Tecnomapas.Blocos.Entities.WebService;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.WebService.ModuloWSDUA;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business
{
	public class PTVBus
	{
		#region Propriedades

		PTVDa _da = new PTVDa();
		PTVValidar _validar = new PTVValidar();
		CredenciadoBus _busCredenciado = new CredenciadoBus();

		private GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}


		#endregion

		#region DML

		public bool Salvar(PTV ptv)
		{
			try
			{
				if (ptv.Id <= 0)
				{
					ptv.Situacao = (int)eSolicitarPTVSituacao.Cadastrado;
				}

				if (_validar.Salvar(ptv))
				{
					#region Arquivos/Diretorio

					ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Credenciado);

					if (ptv.Anexos != null && ptv.Anexos.Count > 0)
					{
						foreach (Anexo anexo in ptv.Anexos)
						{
							if (!String.IsNullOrWhiteSpace(anexo.Arquivo.TemporarioNome) && anexo.Arquivo.Id == 0)
							{
								anexo.Arquivo = _busArquivo.Copiar(anexo.Arquivo);
							}
						}
					}

					#endregion

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						#region Arquivos/Banco
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
						#endregion

						_da.Salvar(ptv, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.PTV.Salvar);
					}
				}
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return Validacao.EhValido;
		}

		public bool Enviar(int id)
		{
			try
			{
				PTV ptv = Obter(id, true);

				if (!_validar.Enviar(ptv))
				{
					return false;
				}

				ptv.Situacao = (int)eSolicitarPTVSituacao.AguardandoAnalise;

				if (ptv.NumeroTipo == (int)eDocumentoFitossanitarioTipoNumero.Digital && ptv.Numero <= 0)
				{
					string numero = VerificarNumeroPTV(string.Empty, ptv.NumeroTipo.GetValueOrDefault());
					if (!string.IsNullOrEmpty(numero))
					{
						ptv.Numero = Convert.ToInt64(numero);
					}

					if (!Validacao.EhValido)
					{
						return Validacao.EhValido;
					}
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Enviar(ptv, bancoDeDados);

					bancoDeDados.Commit();

					Validacao.Add(Mensagem.PTV.EnviadoSucesso);
				}
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return Validacao.EhValido;
		}

		private string ObterNumeroDigital()
		{
			string numeroDigital = string.Empty;
			try
			{
				numeroDigital = _da.ObterNumeroDigital();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return numeroDigital;
		}

		public string VerificarNumeroPTV(string numero, int tipoNumero)
		{
			try
			{
				if (tipoNumero != (int)eDocumentoFitossanitarioTipoNumero.Digital && tipoNumero != (int)eDocumentoFitossanitarioTipoNumero.Bloco)
				{
					Validacao.Add(Mensagem.PTV.TipoNumeroObrigatorio);
				}
				else
				{
					if (tipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Bloco)
					{
						_validar.VerificarNumeroPTV(tipoNumero, numero);
					}

					if (tipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Digital)
					{
						numero = _validar.VerificarNumeroPTV(tipoNumero, ObterNumeroDigital());
					}

					if (Validacao.EhValido && tipoNumero == (int)eDocumentoFitossanitarioTipoNumero.Bloco)
					{
						Validacao.Add(Mensagem.PTV.NumeroValido);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return numero;
		}

		public void VerificarDua(string numero, string cpfCnpj, string tipo, int ptvId)
		{
			try
			{
				if (!_validar.ValidarDuaInicio(numero, cpfCnpj, tipo))
				{
					return;
				}

				WSDUA wsDua = new WSDUA();
				DUA dua = null;

				if (tipo == "1")
				{
					dua = wsDua.ObterDUAPF(numero, cpfCnpj);
				}
				else
				{
					dua = wsDua.ObterDUAPJ(numero, cpfCnpj);
				}

				_validar.ValidarDadosWebServiceDua(dua, numero, cpfCnpj, tipo, ptvId);

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public Dictionary<string, object> VerificarDocumentoOrigem(eDocumentoFitossanitarioTipo origemTipo, string origemTipoTexto, long numero)
		{
			List<Lista> listaCulturas = new List<Lista>();
			Dictionary<string, object> retorno = new Dictionary<string, object>();
			Dictionary<string, object> documentoOrigem = null;

			try
			{
				if (numero <= 0)
				{
					Validacao.Add(Mensagem.PTV.NumeroDocumentoOrigemObrigatorio);
				}
				else
				{
					documentoOrigem = _da.VerificarDocumentoOrigem(origemTipo, numero);
					if (documentoOrigem != null)
					{
						switch (origemTipo)
						{
							case eDocumentoFitossanitarioTipo.CFO:
								if ((int)documentoOrigem["situacao"] != (int)eDocumentoFitossanitarioSituacao.Valido)
								{
									Validacao.Add(Mensagem.PTV.CfoSituacaoInvalida);
								}
								break;
							case eDocumentoFitossanitarioTipo.CFOC:
								if ((int)documentoOrigem["situacao"] != (int)eDocumentoFitossanitarioSituacao.Valido)
								{
									Validacao.Add(Mensagem.PTV.CfocSituacaoInvalida);
								}
								break;
							case eDocumentoFitossanitarioTipo.PTV:
								if ((int)documentoOrigem["situacao"] != (int)ePTVSituacao.Valido)
								{
									Validacao.Add(Mensagem.PTV.PTVSituacaoInvalida);
								}
								break;
							case eDocumentoFitossanitarioTipo.PTVOutroEstado:
								if ((int)documentoOrigem["situacao"] != (int)ePTVOutroSituacao.Valido)
								{
									Validacao.Add(Mensagem.PTV.PTVOutroEstadoSituacaoInvalida);
								}
								break;
						}
					}
					else
					{
						Validacao.Add(Mensagem.PTV.NumeroDocumentoOrigemNaoExistente(origemTipoTexto));
					}
				}

				if (documentoOrigem == null)
				{
					retorno.Add("id", 0);
					retorno.Add("empreendimento_id", 0);
					retorno.Add("empreendimento_denominador", string.Empty);
					retorno.Add("listaCulturas", new List<Lista>());
				}
				else
				{
					retorno.Add("id", documentoOrigem["id"]);
					retorno.Add("empreendimento_id", documentoOrigem["empreendimento_id"]);
					retorno.Add("empreendimento_denominador", documentoOrigem["empreendimento_denominador"]);
					retorno.Add("listaCulturas", _da.ObterCultura((int)origemTipo, (int)documentoOrigem["id"]));
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return retorno;
		}

		public bool Excluir(int id)
		{
			try
			{
				if (!_validar.Excluir(id))
				{
					return false;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(id, bancoDeDados);

					Validacao.Add(Mensagem.PTV.Excluido);

					bancoDeDados.Commit();
				}
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter

		public PTV Obter(int id, bool simplificado = false)
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

		internal PTV ObterPorNumero(long numero, bool simplificado = false)
		{
			try
			{
				return _da.ObterPorNumero(numero, simplificado);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return null;
		}

		/// <summary>
		///	Buscar Empreendimento	
		/// </summary>
		/// <param name="empreendimentoID"></param>
		/// <returns>Retorna o ID e a Descrição do Empreendimento</returns>
		public List<ListaValor> ObterResponsaveisEmpreendimento(int empreendimentoID, List<PTVProduto> produtos)
		{
			try
			{
				int produtorOrigem = 0;
				if (produtos != null && produtos.Count > 0)
				{
					PTVProduto primeiroItem = produtos.FirstOrDefault(x => x.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO || x.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTV);

					if (primeiroItem != null)
					{
						switch ((eDocumentoFitossanitarioTipo)primeiroItem.OrigemTipo)
						{
							case eDocumentoFitossanitarioTipo.CFO:
								EmissaoCFOBus emissaoCFOBus = new EmissaoCFOBus();
								EmissaoCFO cfo = emissaoCFOBus.Obter(primeiroItem.Origem, true);
								produtorOrigem = cfo.ProdutorId;
								break;

							case eDocumentoFitossanitarioTipo.PTV:
								PTV ptv = Obter(primeiroItem.Origem, true);
								produtorOrigem = ptv.ResponsavelEmpreendimento;
								break;
						}
					}
				}

				List<ListaValor> lista = _da.ObterResponsaveisEmpreendimento(empreendimentoID);
				if (produtorOrigem > 0)
				{
					return lista.Where(x => x.Id == produtorOrigem).ToList();
				}

				return lista;
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return new List<ListaValor>();
		}

		public List<Lista> ObterNumeroOrigem(eDocumentoFitossanitarioTipo origemTipo, int empreendimentoID)
		{
			try
			{
				return _da.ObterNumeroOrigem(origemTipo, empreendimentoID);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return new List<Lista>();
		}

		public List<Lista> ObterCultura(int origemTipo, Int64 numeroOrigem)
		{
			try
			{
				return _da.ObterCultura(origemTipo, numeroOrigem);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return new List<Lista>();
		}

		public List<Lista> ObterCultura()
		{
			try
			{
				return _da.ObterCultura();
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return new List<Lista>();
		}

		public List<Lista> ObterCultivar(int culturaID)
		{
			try
			{
				return _da.ObterCultivar(culturaID);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return new List<Lista>();
		}

		public List<Lista> ObterCultivar(eDocumentoFitossanitarioTipo origemTipo, int origemID, int culturaID)
		{
			try
			{
				return _da.ObterCultivar(origemTipo, origemID, culturaID);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return new List<Lista>();
		}

		public List<Lista> ObterUnidadeMedida(eDocumentoFitossanitarioTipo origemTipo, int origemID, int culturaID, int cultivarID)
		{
			try
			{
				return _da.ObterUnidadeMedida(origemTipo, origemID, culturaID, cultivarID);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return new List<Lista>();
		}

		public List<LaudoLaboratorial> ObterLaudoLaboratorial(List<PTVProduto> origens)
		{
			try
			{
				return _da.ObterLaudoLaboratorial(origens);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return new List<LaudoLaboratorial>();
		}

		public List<TratamentoFitossanitario> TratamentoFitossanitário(List<PTVProduto> origens)
		{
			try
			{
				return _da.TratamentoFitossanitario(origens);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return new List<TratamentoFitossanitario>();
		}

		public int ObterDestinatarioID(string CpfCnpj)
		{
			try
			{
				DestinatarioPTVBus _destinatarioBus = new DestinatarioPTVBus();
				return _destinatarioBus.ObterId(CpfCnpj);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return 0;
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

		public CredenciadoPessoa ObterPorCPF_CNPJ(String cpf_cnpj)
		{
			try
			{
				return _busCredenciado.Obter(cpf_cnpj);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<PTVListarResultado> Filtrar(PTVListarFiltro ptvListarFiltro, Paginacao paginacao)
		{
			try
			{
				Filtro<PTVListarFiltro> filtro = new Filtro<PTVListarFiltro>(ptvListarFiltro, paginacao);
				Resultados<PTVListarResultado> resultados = _da.Filtrar(filtro);

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

		public List<ListaValor> DiasHorasVistoria(int setor)
		{
			try
			{
				return _da.DiasHorasVistoria(setor);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Setor> SetoresLocalVistoria()
		{
			try
			{
				return _da.SetoresLocalVistoria();
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return new List<Setor>();
		}

		#endregion

		public PTVHistorico ObterHistoricoAnalise(int ptvID)
		{
			try
			{
				return _da.ObterHistoricoAnalise(ptvID);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}
			return null;
		}

		#region Comunicador

		public PTVComunicador ObterComunicador(int IdPTV)
		{
			try
			{
				PTVComunicador comunicador;
				comunicador = _da.ObterComunicador(IdPTV);


				if (comunicador.ArquivoInternoId > 0)
				{
					ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
					comunicador.ArquivoInterno = _busArquivo.ObterDados(comunicador.ArquivoInternoId);
				}

				if (comunicador.ArquivoCredenciadoId > 0)
				{
					ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Credenciado);
					comunicador.ArquivoCredenciado = _busArquivo.ObterDados(comunicador.ArquivoCredenciadoId);
				}

				return comunicador;

			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return null;
		}

		public bool SalvarConversa(PTVComunicador comunicador)
		{
			try
			{
				#region Configuração

				Blocos.Arquivo.Arquivo arq;

				//if (comunicador.ArquivoCredenciado == null)
				//{
				//    comunicador.ArquivoCredenciado = new Blocos.Arquivo.Arquivo();
				//}

				arq = comunicador.ArquivoCredenciado;



				PTVConversa conversa;
				conversa = comunicador.Conversas[0];

				conversa.DataConversa.Data = DateTime.Now;
				conversa.CredenciadoId = Executor.Current.Id;
				conversa.NomeComunicador = Executor.Current.Nome;
				conversa.TipoComunicador = (int)eExecutorTipo.Credenciado;

				#endregion Configuração

				if (_validar.ValidarConversa(comunicador))
				{
					comunicador.liberadoCredenciado = false;

					#region Arquivos/Diretorio

					ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Credenciado);

					if (arq != null && !String.IsNullOrWhiteSpace(arq.TemporarioNome))
					{
						if (arq.Id == 0)
						{
							arq = _busArquivo.Copiar(arq);
						}
					}

					#endregion

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						#region Arquivo
						if (arq != null && !String.IsNullOrWhiteSpace(arq.TemporarioNome))
						{
							ArquivoDa arquivoDa = new ArquivoDa();
							if (arq.Id == 0)
							{
								arquivoDa.Salvar(arq, Executor.Current.Id, Executor.Current.Nome, Executor.Current.Login, (int)eExecutorTipo.Credenciado, Executor.Current.Tid, bancoDeDados);
								conversa.ArquivoId = arq.Id.GetValueOrDefault();
								conversa.ArquivoNome = arq.Nome;
								comunicador.ArquivoCredenciadoId = arq.Id.GetValueOrDefault();
							}
						}

						#endregion

						_da.EnviarConversa(comunicador, bancoDeDados);

						bancoDeDados.Commit();
						Validacao.Add(Mensagem.PTV.SalvarConversa);

					}
				}
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region DUA

		public bool VerificarSeDUAConsultada(int filaID)
		{
			bool duaConsultada = false;
			try
			{
				duaConsultada = _da.VerificarSeDUAConsultada(filaID);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return duaConsultada;
		}

		public void VerificarDUA(int filaID, string numero, string cpfCnpj, string tipo, int ptvId)
		{
			try
			{
				DUA dua = new DUA();

				var duaRequisicao = _da.BuscarRespostaConsultaDUA(filaID);

				if (duaRequisicao == null)
					return;

				if (!duaRequisicao.Sucesso)
				{
					Validacao.Add(Mensagem.PTV.ErroAoConsultarDua);
					return;
				}

				var xser = new XmlSerializer(typeof(RespostaConsultaDua));

				RespostaConsultaDua xml = null;

				try
				{
					xml = (RespostaConsultaDua)xser.Deserialize(new StringReader(duaRequisicao.Resultado));
				}
				catch
				{
					Validacao.Add(Mensagem.PTV.ErroAoConsultarDua);
					return;
				}

				if (xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua == null)
				{
					Validacao.Add(Mensagem.PTV.ErroSefaz(xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.XMotivo));
					return;
				}

				dua.OrgaoSigla = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Orgao.XSigla;
				dua.ServicoCodigo = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Area.CArea;

				dua.ReferenciaData = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Data.DRef;
				dua.CPF = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Contri.Cpf;
				dua.CNPJ = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Contri.Cnpj;

				dua.ReceitaValor = (float)xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Rece.VRece;
				dua.PagamentoCodigo = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Pgto.CPgto;

				_validar.ValidarDadosWebServiceDua(dua, numero, cpfCnpj, tipo, ptvId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public int GravarConsultaDUA(string numeroDUA, string cpfCnpj, string tipo)
		{
			int filaID = 0;
			try
			{
				if (!_validar.ValidarDuaInicio(numeroDUA, cpfCnpj, tipo))
				{
					return 0;
				}

				cpfCnpj = cpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "");

				var dua = String.Format("{{ \"dua\": \"{0}\", \"{1}\": \"{2}\" }}", numeroDUA, tipo == "1" ? "cpf" : "cnpj", cpfCnpj);//'{"dua":"1995908182","cnpj":"27473669000580"}',

				filaID = _da.VerificarConsultaDUAFila(Executor.Current.Id, dua);

				if (filaID == 0)
					filaID = _da.GravarFilaConsultaDUA(Executor.Current.Id, dua);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return filaID;
		}

		#endregion
	}
}