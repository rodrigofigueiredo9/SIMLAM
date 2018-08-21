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
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFOC.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmpreendimento.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTVOutro.Business;
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

		public void CancelarEnvio(int ptvId)
		{
			try
			{
				PTV ptv = Obter(ptvId);

				if (_validar.CancelarEnvio(ptv))
				{
					ptv.Situacao = (int)eSolicitarPTVSituacao.Cadastrado;

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						_da.CancelarEnvio(ptv, bancoDeDados);

						Validacao.Add(Mensagem.PTV.CanceladoSucesso);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
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

        public string ObterDeclaracaoAdicionaisPTVOutro(int numero)        
        {
            string numeroDigital = string.Empty;
            try
            {
                numeroDigital = _da.ObterDeclaracaoAdicional(numero);
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

		public Dictionary<string, object> VerificarDocumentoOrigem(eDocumentoFitossanitarioTipo origemTipo, string origemTipoTexto, long numero, string serieNumero)
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
					documentoOrigem = _da.VerificarDocumentoOrigem(origemTipo, numero, serieNumero);
					if (documentoOrigem != null)
					{
						CredenciadoDa _credenciadoDa = new CredenciadoDa();
						var credenciado = _credenciadoDa.Obter(User.FuncionarioId);

						switch (origemTipo)
						{
							case eDocumentoFitossanitarioTipo.CFO:
								if ((int)documentoOrigem["situacao"] != (int)eDocumentoFitossanitarioSituacao.Valido)
								{
									Validacao.Add(Mensagem.PTV.CfoSituacaoInvalida);
								}

								PessoaInternoDa _pessoaInternoDa = new PessoaInternoDa();
								var produtor = _pessoaInternoDa.Obter((int)documentoOrigem["produtor"]);

								if ((int)documentoOrigem["credenciado"] != User.FuncionarioId && produtor.CPFCNPJ != credenciado.Pessoa.CPFCNPJ)
								{
									Validacao.Add(Mensagem.PTV.UsuarioSemPermissaoDocOrigem);
								}
								break;
							case eDocumentoFitossanitarioTipo.CFOC:
								if ((int)documentoOrigem["situacao"] != (int)eDocumentoFitossanitarioSituacao.Valido)
								{
									Validacao.Add(Mensagem.PTV.CfocSituacaoInvalida);
								}

								EmpreendimentoInternoDa _empreendimentoInternoDa = new EmpreendimentoInternoDa();
								var empreendimento = _empreendimentoInternoDa.Obter((int)documentoOrigem["empreendimento_id"]);

								if ((int)documentoOrigem["credenciado"] != User.FuncionarioId && empreendimento.CNPJ != credenciado.Pessoa.CPFCNPJ)
								{
									Validacao.Add(Mensagem.PTV.UsuarioSemPermissaoDocOrigem);
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
								if ((int)documentoOrigem["credenciado"] != User.FuncionarioId)
								{
									Validacao.Add(Mensagem.PTV.UsuarioSemPermissaoDocOrigem);
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
                    retorno.Add("declaracao_adicional", " ");
				}
				else
				{
					retorno.Add("id", documentoOrigem["id"]);
					retorno.Add("empreendimento_id", documentoOrigem["empreendimento_id"]);
					retorno.Add("empreendimento_denominador", documentoOrigem["empreendimento_denominador"]);
					retorno.Add("listaCulturas", _da.ObterCultura((int)origemTipo, (int)documentoOrigem["id"]));
                    retorno.Add("declaracao_adicional", _da.ObterDeclaracaoAdicional((int)documentoOrigem["id"]));

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
				PTV ptv = new PTV();

				ptv = _da.Obter(id, simplificado);
				ptv.NotaFiscalDeCaixas = ObterNotasFiscalDeCaixas(id);
				ptv.NFCaixa.notaFiscalCaixaApresentacao = (ptv.NotaFiscalDeCaixas.Count() > 0) ? 0 : 1;

				return ptv;
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

		public List<ListaValor> DiasHorasVistoria(int setor, DateTime? dataVistoria = null)
		{
			try
			{
				return _da.DiasHorasVistoria(setor, dataVistoria);
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

		public NotaFiscalCaixa VerificarNumeroNFCaixa(NotaFiscalCaixa notaFiscal)
		{
			try
			{
				return _da.VerificarNumeroNFCaixa(notaFiscal);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<NotaFiscalCaixa> ObterNotasFiscalDeCaixas(int idPTV)
		{
			try
			{
				return _da.ObterNFCaixas(idPTV);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public decimal ObterSaldoDocOrigem(PTVProduto prod)
		{
			decimal saldo = 0;
			switch ((eDocumentoFitossanitarioTipo)prod.OrigemTipo)
			{
				case eDocumentoFitossanitarioTipo.CFO:
					EmissaoCFOBus emissaoCFOBus = new EmissaoCFOBus();
					EmissaoCFO cfo = emissaoCFOBus.Obter(prod.Origem);
					saldo = cfo.Produtos.Where(x => x.CultivarId == prod.Cultivar && x.UnidadeMedidaId == prod.UnidadeMedida).Sum(x => x.Quantidade);
					break;

				case eDocumentoFitossanitarioTipo.CFOC:
					EmissaoCFOCBus emissaoCFOCBus = new EmissaoCFOCBus();
					EmissaoCFOC cfoc = emissaoCFOCBus.Obter(prod.Origem);
					saldo = cfoc.Produtos.Where(x => x.CultivarId == prod.Cultivar && x.UnidadeMedidaId == prod.UnidadeMedida).Sum(x => x.Quantidade);
					break;

				case eDocumentoFitossanitarioTipo.PTVOutroEstado:
					PTVOutroBus ptvOutroBus = new PTVOutroBus();
					PTVOutro ptvOutro = ptvOutroBus.Obter(prod.Origem);
					saldo = ptvOutro.Produtos.Where(x => x.Cultivar == prod.Cultivar && x.UnidadeMedida == prod.UnidadeMedida).Sum(x => x.Quantidade);
					break;

				case eDocumentoFitossanitarioTipo.PTV:
					PTVBus ptvBus = new PTVBus();
					PTV ptv = ptvBus.Obter(prod.Origem);
					saldo = ptv.Produtos.Where(x => x.Cultivar == prod.Cultivar && x.UnidadeMedida == prod.UnidadeMedida).Sum(x => x.Quantidade);
					break;
			}

			decimal saldoOutrosDoc = _da.ObterOrigemQuantidade((eDocumentoFitossanitarioTipo)prod.OrigemTipo, prod.Origem, prod.OrigemNumero, prod.Cultivar, prod.UnidadeMedida, 0);    //o último parâmetro, idPTV, nesse caso não importa, porque o PTV atual não deve ser desconsiderado do cálculo

			saldo = saldo - saldoOutrosDoc;

			return saldo;
		}

		public PTV ObterNumeroPTVExibirMensagemCredenciado(int idCredenciado, BancoDeDados banco = null)
		{
			var ptv = new PTV();

			try
			{
				ptv = _da.ObterNumeroPTVExibirMensagemCredenciado(idCredenciado, banco);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return ptv;
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

		public void VerificarDUA(string numero, string cpfCnpj, string tipo, int ptvId)
		{
			try
			{
				var wSDUA = new WSDUA();
				var dua = wSDUA.ObterDUA(numero, cpfCnpj, tipo == "1" ? eTipoPessoa.Fisica : eTipoPessoa.Juridica);

				if(Validacao.EhValido)
					_validar.ValidarDadosWebServiceDua(dua, numero, cpfCnpj, tipo, ptvId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}
		
		#endregion

		#region Alerta de E-PTV

		public bool VerificarAlertaChegadaMensagemEPTV()
		{
			bool exibirMensagem = false;

			int credenciadoId = HttpContext.Current.User != null ? (HttpContext.Current.User.Identity as EtramiteIdentity).FuncionarioId : 0;

			var ptv = ObterNumeroPTVExibirMensagemCredenciado(credenciadoId);

			if (ptv?.Id > 0)
			{
				switch (ptv.Situacao)
				{
					case (int)eSolicitarPTVSituacao.Valido:
						Validacao.Add(Mensagem.PTV.ChegadaMensagemEPTVAprovada(ptv.Numero, ptv.Id));
						break;
					case (int)eSolicitarPTVSituacao.Bloqueado:
						Validacao.Add(Mensagem.PTV.ChegadaMensagemEPTVBloqueada(ptv.Numero, ptv.Id));
						break;
					case (int)eSolicitarPTVSituacao.Rejeitado:
						Validacao.Add(Mensagem.PTV.ChegadaMensagemEPTVRejeitada(ptv.Numero, ptv.SituacaoMotivo, ptv.Id));
						break;
					case (int)eSolicitarPTVSituacao.AgendarFiscalizacao:
						Validacao.Add(Mensagem.PTV.ChegadaMensagemEPTVFiscalizacaoAgendada(ptv.Numero, ptv.LocalVistoriaTexto,
							ptv.DataHoraVistoriaTexto, ptv.InformacoesAdicionais));
						break;
				}

				exibirMensagem = true;
			}

			return exibirMensagem;
		}

		#endregion Alerta de E-PTV
	}
}