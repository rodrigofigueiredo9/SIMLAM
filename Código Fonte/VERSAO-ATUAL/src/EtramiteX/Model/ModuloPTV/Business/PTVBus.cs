using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloEmail;
using Tecnomapas.Blocos.Etx.ModuloEmail.Business;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Data;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Business;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using System.Web;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Business
{
	public class PTVBus : HttpApplication
	{
		#region Propriedades

		PTVDa _da = new PTVDa();
		PTVValidar _validar = new PTVValidar();
		CredenciadoBus _busCredenciado = new CredenciadoBus();

		private GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		private String SchemaUsuarioInterno { get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); } }

		private String SchemaUsuarioCredenciado { get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); } }

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region DML

		public bool Salvar(PTV ptv)
		{
			try
			{
				if (ptv.NumeroTipo == (int)eDocumentoFitossanitarioTipoNumero.Bloco && ptv.Situacao != (int)eDocumentoFitossanitarioSituacao.EmElaboracao)
				{
					ptv.DataAtivacao = ptv.DataEmissao;
				}

				if (_validar.Salvar(ptv))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();
						ptv.PossuiLaudoLaboratorial = 0;

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

		public bool Ativar(PTV ptv)
		{
			try
			{
				PTV PTVBanco = Obter(ptv.Id, false);
				ptv.Numero = PTVBanco.Numero;
				ptv.Produtos = PTVBanco.Produtos;

				if (PTVBanco.NumeroTipo == (int)eDocumentoFitossanitarioTipoNumero.Digital && PTVBanco.Situacao == (int)ePTVSituacao.EmElaboracao)
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

				if (Validacao.EhValido && _validar.Ativar(ptv.DataAtivacao, PTVBanco))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Ativar(ptv, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.PTV.AtivadoSucesso(ptv.Numero.ToString()));
					}
				}
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return Validacao.EhValido;
		}

		public bool PTVCancelar(PTV ptv)
		{
			try
			{
				if (_validar.PTVCancelar(ptv))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.PTVCancelar(ptv, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.PTV.CanceladoSucesso);
					}
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

		public Dictionary<string, object> VerificarDocumentoOrigem(eDocumentoFitossanitarioTipo origemTipo, string origemTipoTexto, long numero, string serieNumeral)
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
					documentoOrigem = _da.VerificarDocumentoOrigem(origemTipo, numero, serieNumeral);
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

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
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

		public bool Analisar(PTV eptv)
		{
			try
			{
				Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus ptvCredenciadoBus = new Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus();
				PTV eptvBanco = ptvCredenciadoBus.Obter(eptv.Id);

				if (!_validar.Analisar(eptv, eptvBanco))
					return false;

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDadosInterno = BancoDeDados.ObterInstancia(SchemaUsuarioInterno))
				{
					bancoDeDadosInterno.IniciarTransacao();

					using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(SchemaUsuarioCredenciado))
					{
						bancoDeDadosCredenciado.IniciarTransacao();

						_da.AnalizarEPTV(eptv, bancoDeDadosCredenciado);

						if (eptv.Situacao == (int)eSolicitarPTVSituacao.Valido)
						{
							eptvBanco.ValidoAte = eptv.ValidoAte;
							eptvBanco.ResponsavelTecnicoId = eptv.ResponsavelTecnicoId;
							eptvBanco.ResponsavelTecnicoNome = eptv.ResponsavelTecnicoNome;
							eptvBanco.LocalEmissaoId = eptv.LocalEmissaoId;
							eptvBanco.TemAssinatura = _da.ExisteAssinaturaDigital(eptv.ResponsavelTecnicoId);

							if (!_validar.Importar(eptvBanco))
							{
								return Validacao.EhValido;
							}

							if (eptvBanco.Anexos.Any())
							{
								var arquivoBusCred = new ArquivoBus(eExecutorTipo.Credenciado);
								var arquivoBusInst = new ArquivoBus(eExecutorTipo.Interno);
								foreach (var anexo in eptvBanco.Anexos)
								{
									if (anexo.Arquivo.Id > 0)
									{
										var arquivoCred = arquivoBusCred.Obter(anexo.Arquivo.Id.Value);
										if (arquivoCred?.Id > 0)
										{
											anexo.Arquivo = arquivoBusInst.Salvar(arquivoCred);
											anexo.Arquivo.Id = 0;

											ArquivoDa _arquivoDa = new ArquivoDa();
											_arquivoDa.Salvar(anexo.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDadosInterno);
										}
									}
								}
							}

							try
							{
								_da.Importar(eptvBanco, bancoDeDadosInterno);
							}
							catch (Exception exc)
							{
								if (exc.Message.Contains("UK_TAB_PTV_EPTV"))
								{
									Validacao.Add(new Mensagem { Texto = "O EPTV já foi importado para o institucional.", Tipo = eTipoMensagem.Advertencia });
									return Validacao.EhValido;
								}

								throw exc;
							}
						}

						string textoEmail = string.Empty;
						switch (eptv.Situacao)
						{
							case 3:
								eptvBanco.SituacaoTexto = "Aprovado";
								textoEmail = _configSys.Obter<String>(ConfiguracaoSistema.KeyModeloTextoEmailAprovarSolicitacaoPTV);
								break;

							case 4:
								eptvBanco.SituacaoTexto = "Rejeitado";
								textoEmail = _configSys.Obter<String>(ConfiguracaoSistema.KeyModeloTextoEmailRejeitarSolicitacaoPTV) + eptv.SituacaoMotivo;
								break;

							case 5:
								eptvBanco.SituacaoTexto = "Fiscalização Agendada";
								textoEmail = _configSys.Obter<String>(ConfiguracaoSistema.KeyModeloTextoEmailAgendarFiscalizacaoSolicitacaoPTV);
								break;
						}

						#region [ Enviar E-mail ]

						if (eptv.Situacao == (int)eSolicitarPTVSituacao.Bloqueado ||
							eptv.Situacao == (int)eSolicitarPTVSituacao.AgendarFiscalizacao)
						{
							PTVComunicador comunicador = new PTVComunicador();
							comunicador.Id = _da.ObterIDComunicador(eptv.Id);
							comunicador.PTVId = eptv.Id;
							comunicador.PTVNumero = eptv.Numero;
							comunicador.liberadoCredenciado = true;

							var conversa = new PTVConversa();
							conversa.Texto = eptv.SituacaoMotivo;
							comunicador.Conversas.Add(conversa);

							SalvarConversa(comunicador, bancoDeDadosInterno, bancoDeDadosCredenciado);
						}
						else
						{
							var emailKeys = new Dictionary<string, string>
							{
								{ "[data situacao]", DateTime.Today.ToShortDateString() },
								{ "[hora situacao]", DateTime.Now.ToShortTimeString() },
								{ "[numero]", eptvBanco.Numero.ToString() },
								{ "[local vistoria]", eptvBanco.LocalVistoriaTexto },
								{ "[hora vistoria]", eptv.DataHoraVistoriaTexto }
							};

							foreach (var item in emailKeys)
							{
								textoEmail = textoEmail.Replace(item.Key, item.Value);
							}

							var email = new Email();
							email.Assunto = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoSigla);
							email.Texto = textoEmail;
							email.Tipo = eEmailTipo.AnaliseEPTV;
							email.Codigo = eptv.Id;

							List<String> lstEmail = _da.ObterEmailsCredenciado(eptv.Id, bancoDeDadosCredenciado);

							if (lstEmail != null && lstEmail.Count > 0)
							{
								email.Destinatario = String.Join(", ", lstEmail.ToArray());

								EmailBus emailBus = new EmailBus();
								emailBus.Enviar(email, bancoDeDadosInterno);
							}
						}

						#endregion

						bancoDeDadosInterno.Commit();
						bancoDeDadosCredenciado.Commit();
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

				//return _destinatarioBus.Obter(_destinatarioBus.ObterId(CpfCnpj));
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

		public List<ListaValor> ObterLocalEmissao(int usuarioId)
		{
			try
			{
				return _da.ObterLocalEmissao(usuarioId);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return new List<ListaValor>();
		}

		public Resultados<PTVListarResultado> FiltrarEPTV(PTVListarFiltro ptvListarFiltro, Paginacao paginacao)
		{
			try
			{
				Filtro<PTVListarFiltro> filtro = new Filtro<PTVListarFiltro>(ptvListarFiltro, paginacao);
				Resultados<PTVListarResultado> resultados = _da.FiltrarEPTV(filtro);

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

		public List<ListaValor> DiasHorasVistoria(int setor, bool visualizar)
		{
			try
			{
				return _da.DiasHorasVistoria(setor, visualizar);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public int QuantidadeEPTVAguardandoAnaliseFuncionario(int idFuncionario, BancoDeDados banco = null)
		{
			int quantidade = -1;

			try
			{
				quantidade = _da.QuantidadeEPTVAguardandoAnaliseFuncionario(idFuncionario, banco);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return quantidade;
		}

		public PTV ObterNumeroPTVExibirMensagemFuncionario(int idFuncionario, BancoDeDados banco = null)
		{
			var ptv = new PTV();

			try
			{
				ptv = _da.ObterNumeroPTVExibirMensagemFuncionario(idFuncionario, banco);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return ptv;
		}

		#endregion

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

		public bool SalvarConversa(PTVComunicador comunicador, BancoDeDados bancoInterno = null, BancoDeDados bancoCredenciado = null)
		{
			try
			{
				#region Configuração

				Blocos.Arquivo.Arquivo arq;

				if (comunicador.ArquivoInterno == null)
				{
					comunicador.ArquivoInterno = new Blocos.Arquivo.Arquivo();
				}

				comunicador.liberadoCredenciado = true;
				arq = comunicador.ArquivoInterno;

				PTVConversa conversa;
				conversa = comunicador.Conversas[0];

				conversa.DataConversa.Data = DateTime.Now;
				conversa.FuncionarioId = Executor.Current.Id;
				conversa.NomeComunicador = Executor.Current.Nome;
				conversa.TipoComunicador = (int)eExecutorTipo.Interno;

				#endregion Configuração

				if (_validar.ValidarConversa(comunicador))
				{
					#region Arquivos/Diretorio

					ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

					if (arq != null && !String.IsNullOrWhiteSpace(arq.TemporarioNome))
					{
						if (arq.Id == 0)
						{
							arq = _busArquivo.Copiar(arq);
						}
					}

					#endregion

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(bancoCredenciado, SchemaUsuarioCredenciado))
					{
						bancoDeDadosCredenciado.IniciarTransacao();

						using (BancoDeDados bancoDeDadosInterno = BancoDeDados.ObterInstancia(bancoInterno))
						{
							bancoDeDadosInterno.IniciarTransacao();

							try
							{
								#region Arquivo

								if (arq != null && !String.IsNullOrWhiteSpace(arq.TemporarioNome))
								{
									ArquivoDa arquivoDa = new ArquivoDa();
									if (arq.Id == 0)
									{
										arquivoDa.Salvar(arq, Executor.Current.Id, Executor.Current.Nome, Executor.Current.Login, (int)eExecutorTipo.Interno, Executor.Current.Tid, bancoDeDadosInterno);
										conversa.ArquivoId = arq.Id.GetValueOrDefault();
										conversa.ArquivoNome = arq.Nome;
										comunicador.ArquivoInternoId = arq.Id.GetValueOrDefault();
									}
								}

								#endregion

								if (comunicador.Id <= 0)
								{
									_da.CriarComunicador(comunicador, bancoDeDadosCredenciado);
								}
								else
								{
									_da.EnviarConversa(comunicador, bancoDeDadosCredenciado);
								}

								bancoDeDadosCredenciado.Commit();
								bancoDeDadosInterno.Commit();
								EnviarEmailConversa(comunicador, bancoDeDadosCredenciado);
							}
							catch
							{
								bancoDeDadosCredenciado.Rollback();
								bancoDeDadosInterno.Rollback();
								throw;
							}

							Validacao.Add(Mensagem.PTV.SalvarConversa);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return Validacao.EhValido;
		}

		private bool EnviarEmailConversa(PTVComunicador comunicador, BancoDeDados bancoDeDados)
		{
			try
			{
				Email email = null;
				PTVConversa conversa = comunicador.Conversas[0];
				PTVComunicador comunicadorCompleto = _da.ObterComunicadorSemConversas(comunicador.PTVId, bancoDeDados);
				if (!String.IsNullOrEmpty(conversa.Texto))
				{
					string textoEmail = _configSys.Obter<String>(ConfiguracaoSistema.KeyModeloTextoEmailComunicadorSolicitacaoPTV);

					if (!String.IsNullOrWhiteSpace(textoEmail))
					{
						Dictionary<String, String> emailKeys = new Dictionary<string, string>();

						emailKeys.Add("[data da conclusão]", conversa.DataConversa.DataHoraTexto);
						emailKeys.Add("[número do PTV]", comunicadorCompleto.PTVNumero.ToString());
						emailKeys.Add("[Situação]", _da.BuscarSituacaoPTV(comunicadorCompleto.PTVId, bancoDeDados));

						foreach (string key in emailKeys.Keys)
						{
							textoEmail = textoEmail.Replace(key, emailKeys[key]);
						}

						email = new Email();
						email.Assunto = _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoSigla);
						email.Texto = textoEmail;
						email.Tipo = eEmailTipo.ComunicadorPTV;
						email.Codigo = conversa.Id;
					}
				}

				if (email != null)
				{
					List<String> lstEmail = _da.ObterEmailsCredenciado(comunicador.PTVId, bancoDeDados);

					if (lstEmail != null && lstEmail.Count > 0)
					{
						email.Destinatario = String.Join(", ", lstEmail.ToArray());

						EmailBus emailBus = new EmailBus();
						emailBus.Enviar(email);
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

		#region Alerta de E-PTV

		public bool VerificaAlertaEPTV()
		{
			bool houveAlerta = false;

			int funcionarioId = HttpContext.Current.User != null ? (HttpContext.Current.User.Identity as EtramiteIdentity).FuncionarioId : 0;

			//verifica se o usuário está habilitado para emissão de PTV
			bool habilitado = _validar.FuncionarioHabilitadoValido(funcionarioId);

			if (habilitado)
			{
				//Verifica quantas PTVs estão aguardando análise
				int quantidade = QuantidadeEPTVAguardandoAnaliseFuncionario(funcionarioId);

				if (quantidade > 0)
				{
					Validacao.AddAlertaEPTV(Mensagem.PTV.ExistemEPTVsAguardandoAnalise(quantidade));
					houveAlerta = true;
				}
			}
			return houveAlerta;
		}

		public bool VerificarAlertaChegadaMensagemEPTV()
		{
			bool exibirMensagem = false;

			int funcionarioId = HttpContext.Current.User != null ? (HttpContext.Current.User.Identity as EtramiteIdentity).FuncionarioId : 0;

			//verifica se o usuário está habilitado para emissão de PTV
			bool habilitado = _validar.FuncionarioHabilitadoValido(funcionarioId);

			if (habilitado)
			{
				//Verifica quantas PTVs estão aguardando análise
				var ptv = ObterNumeroPTVExibirMensagemFuncionario(funcionarioId);

				if (ptv?.Id > 0)
				{
					Validacao.Add(Mensagem.PTV.ChegadaMensagemEPTV(ptv.Numero, ptv.Id));
					exibirMensagem = true;
				}
			}
			return exibirMensagem;
		}


		#endregion Alerta de E-PTV
	}
}