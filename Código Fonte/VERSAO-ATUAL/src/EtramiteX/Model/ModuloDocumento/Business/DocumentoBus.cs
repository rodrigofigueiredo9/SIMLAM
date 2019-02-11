using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemPendencia;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemPendencia.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemRoteiro.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProcesso.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business
{
	public class DocumentoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		DocumentoValidar _validar = new DocumentoValidar();
		ProtocoloDa _da = new ProtocoloDa();
		PessoaBus _busPessoa = new PessoaBus(new PessoaValidar());
		ChecagemRoteiroBus _busCheckList = new ChecagemRoteiroBus();
		RequerimentoBus _busRequerimento = new RequerimentoBus(new RequerimentoValidar());
		FiscalizacaoBus _busFiscalizacao;
		ProjetoDigitalBus _busProjetoDigital;
		FuncionarioBus _busFuncionario = new FuncionarioBus();

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public DocumentoBus()
		{
			_busFiscalizacao = new FiscalizacaoBus();
			_busProjetoDigital = new ProjetoDigitalBus();
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		}

		#region Ações DML

		public bool Salvar(Documento documento)
		{
			try
			{
				documento.Emposse.Id = User.FuncionarioId;

				if (documento.Arquivo == null)
				{
					documento.Arquivo = new Arquivo();
				}

				bool isEdicao = (documento.Id > 0);

				if (_validar.Salvar(documento))
				{
					#region Arquivos/Diretorio

					if (documento.Arquivo.Id != null && documento.Arquivo.Id == 0)
					{
						ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
						documento.Arquivo = _busArquivo.Copiar(documento.Arquivo);
					}

					#endregion

					#region Setar Valores

					ListaBus listaBus = new ListaBus();
					ProtocoloTipo configuracao = listaBus.TiposDocumento.FirstOrDefault(x => x.Id == documento.Tipo.Id);

					documento.ProtocoloAssociado = (configuracao.PossuiProcesso || configuracao.ProcessoObrigatorio) ? documento.ProtocoloAssociado : new Protocolo();
					documento.ChecagemPendencia = (configuracao.PossuiChecagemPendencia || configuracao.ChecagemPendenciaObrigatorio) ? documento.ChecagemPendencia : new ChecagemPendencia();
					documento.ChecagemRoteiro = (configuracao.PossuiChecagemRoteiro || configuracao.ChecagemRoteiroObrigatorio) ? documento.ChecagemRoteiro : new ChecagemRoteiro();
					documento.Requerimento = (configuracao.PossuiRequerimento || configuracao.RequerimentoObrigatorio) ? documento.Requerimento : new Requerimento();

					#endregion

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						#region Arquivos/Banco

						if (documento.Arquivo.Id == 0)
						{
							ArquivoDa _arquivoDa = new ArquivoDa();
							_arquivoDa.Salvar(documento.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
						}

						#endregion

						ChecagemPendenciaBus _checagemPendenciaBus = new ChecagemPendenciaBus();

						using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(UsuarioCredenciado))
						{
							bancoDeDadosCredenciado.IniciarTransacao();

							#region Alterar Situacao Requerimento

							if (documento.Id.HasValue)
							{
								Documento docOriginal = _da.ObterSimplificado(documento.Id.Value) as Documento;
								docOriginal.Requerimento = _busRequerimento.ObterSimplificado(docOriginal.Requerimento.Id);

								if (docOriginal.Requerimento.Id != documento.Requerimento.Id)
								{
									AlterarRequerimentoSituacao(docOriginal, banco: bancoDeDados);

									if (docOriginal.Requerimento.IsRequerimentoDigital)
									{
										CARSolicitacaoBus carSolicitacaoCredenciadoBus = new CARSolicitacaoBus();
										carSolicitacaoCredenciadoBus.DesassociarProtocolo(new CARSolicitacao() { Requerimento = docOriginal.Requerimento }, bancoDeDadosCredenciado);

										if (!_busProjetoDigital.AlterarSituacao(docOriginal.Requerimento.Id, eProjetoDigitalSituacao.AguardandoProtocolo, bancoDeDadosCredenciado))
										{
											bancoDeDados.Rollback();
											return false;
										}
									}
								}
							}

							#endregion

							Documento documentoOriginal = new Documento();
							if (documento.Id != null && documento.Id.GetValueOrDefault() > 0)
							{
								documentoOriginal = ObterSimplificado(documento.Id.Value);
							}

							#region Titulo

							if (documento.ChecagemPendencia.Id > 0 && documento.Id.GetValueOrDefault() == 0)
							{
								documento.ChecagemPendencia = _checagemPendenciaBus.Obter(documento.ChecagemPendencia.Id);

								TituloBus tituloBus = new TituloBus();
								Titulo titulo = tituloBus.Obter(documento.ChecagemPendencia.TituloId);

								if (titulo.Situacao.Id != 5)//5 - Encerrado
								{
									titulo.DataEncerramento.Data = DateTime.Now;
									titulo.MotivoEncerramentoId = 7;//Encerrado
									TituloSituacaoBus tituloSituacaoBus = new TituloSituacaoBus();
									tituloSituacaoBus.AlterarSituacao(titulo, (int)eAlterarSituacaoAcao.Encerrar, bancoDeDados);

									if (Validacao.EhValido)
									{
										Validacao.Erros.Clear();
									}
								}

								#region Itens da Analise

								AnaliseItensBus busAnalise = new AnaliseItensBus(new AnaliseItensValidar());

								AnaliseItem analiseItem = busAnalise.ObterAnaliseTitulo(documento.ChecagemPendencia.TituloId);
								analiseItem = busAnalise.Obter(analiseItem.Id, bancoDeDados);
								//int setorId = _busProc.ObterSetor(analiseItem.Protocolo.Id);

								foreach (Item item in analiseItem.Itens)
								{
									if (documento.ChecagemPendencia.Itens.Exists(x => x.Id == item.Id))
									{
										item.Analista = User.Name;
										item.Situacao = (int)eAnaliseItemSituacao.Recebido;
										item.Motivo = String.Empty;
										item.Recebido = true;
										item.Editado = true;
										//item.SetorId = setorId;
										item.DataAnalise = DateTime.Now.ToString();
									}
								}

								AnaliseItensDa _daAnalise = new AnaliseItensDa();
								_daAnalise.Salvar(analiseItem, bancoDeDados);

								#endregion
							}

							#endregion

							#region Atividade

							List<Atividade> lstAtividadesAtual = null;
							if ((documento.Id ?? 0) > 0)
							{
								lstAtividadesAtual = _da.ObterAtividades(documento.Id.GetValueOrDefault(), bancoDeDados).Atividades;
							}

							if (documento.Atividades != null && documento.Atividades.Count > 0)
							{
								documento.Atividades.ForEach(x =>
								{
									x.Protocolo.Id = documento.Id.GetValueOrDefault();
									x.Protocolo.IsProcesso = false;
								});

								AtividadeBus atividadeBus = new AtividadeBus();
								atividadeBus.AlterarSituacaoProcDoc(documento.Atividades, lstAtividadesAtual, bancoDeDados);
								atividadeBus.TituloAnteriores(documento.Atividades, lstAtividadesAtual, bancoDeDados);
							}

							#endregion

							_da.Salvar(documento, bancoDeDados);

							#region Checagens

							// cadastrando, seta situação da checagem de itens de roteiro/pendencia como protocolada
							if (documentoOriginal.Id.GetValueOrDefault() <= 0)
							{
								_busCheckList.AlterarSituacao(documento.ChecagemRoteiro.Id, 2, bancoDeDados);//protocolada

								documento.ChecagemPendencia.SituacaoId = 2;//protocolada
								_checagemPendenciaBus.AlterarSituacao(documento.ChecagemPendencia, bancoDeDados);
							}
							else // editando documento
							{	// se checagem de itens de roteiro foi alterada, setar o status da antiga como finalizada e setar o status da nova como protocolada
								if (documento.ChecagemRoteiro.Id != documentoOriginal.ChecagemRoteiro.Id)
								{
									_busCheckList.AlterarSituacao(documentoOriginal.ChecagemRoteiro.Id, 1, bancoDeDados);//finalizada
									_busCheckList.AlterarSituacao(documento.ChecagemRoteiro.Id, 2, bancoDeDados);//protocolada
								}
							}

							#endregion

							documento.Requerimento = _busRequerimento.ObterSimplificado(documento.Requerimento.Id);

							if (documento.Requerimento.IsRequerimentoDigital)
							{
								CARSolicitacaoBus carSolicitacaoCredenciadoBus = new CARSolicitacaoBus();
								carSolicitacaoCredenciadoBus.AssociarProtocolo(new CARSolicitacao() { Requerimento = documento.Requerimento }, bancoDeDadosCredenciado);

								if (documentoOriginal.Requerimento.Id != documento.Requerimento.Id)
								{
									if (!_busProjetoDigital.AlterarSituacao(documento.Requerimento.Id, eProjetoDigitalSituacao.AguardandoAnalise, bancoDeDadosCredenciado))
									{
										bancoDeDados.Rollback();
										return false;
									}
								}
							}

							AlterarRequerimentoSituacao(documento, 3, bancoDeDados);// Protocolado

							#region Fiscalizacao

							if (isEdicao && documento.Fiscalizacao.Id != documentoOriginal.Fiscalizacao.Id && documentoOriginal.Fiscalizacao.Id > 0)
							{
								documentoOriginal.Fiscalizacao.SituacaoNovaTipo = (int)eFiscalizacaoSituacao.CadastroConcluido;
								documentoOriginal.Fiscalizacao.SituacaoNovaData.Data = DateTime.Now;
								documentoOriginal.Fiscalizacao.SituacaoAtualTipo = documento.Fiscalizacao.SituacaoId;

								_busFiscalizacao.AlterarSituacaoProcDoc(documentoOriginal.Fiscalizacao, bancoDeDados);
							}

							if (documento.Fiscalizacao.Id != documentoOriginal.Fiscalizacao.Id)
							{
								documento.Fiscalizacao.SituacaoNovaTipo = (int)eFiscalizacaoSituacao.Protocolado;
								documento.Fiscalizacao.SituacaoNovaData.Data = DateTime.Now;
								documento.Fiscalizacao.SituacaoAtualTipo = documento.Fiscalizacao.SituacaoId;

								_busFiscalizacao.AlterarSituacaoProcDoc(documento.Fiscalizacao, bancoDeDados);
							}

							#endregion

							//sempre no final esse if
							if (!Validacao.EhValido)
							{
								bancoDeDadosCredenciado.Rollback();
								bancoDeDados.Rollback();
								return false;
							}

							bancoDeDadosCredenciado.Commit();
						}

						bancoDeDados.Commit();
					}

					Mensagem msgSucesso = Mensagem.Documento.Salvar(documento.Numero);
					if (isEdicao)
					{
						msgSucesso = Mensagem.Documento.Editar;
					}

					Validacao.Add(msgSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool Excluir(int id)
		{
			try
			{
				if (_validar.Excluir(id))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						ChecagemPendenciaBus _checagemPendenciaBus = new ChecagemPendenciaBus();
						Documento doc = _da.ObterAtividades(id) as Documento;

						#region Atividades

						AtividadeBus atividadeBus = new AtividadeBus();
						atividadeBus.TituloAnteriores(new List<Atividade>(), doc.Atividades, bancoDeDados);

						#endregion

						_da.Excluir(id, bancoDeDados);

						AlterarRequerimentoSituacao(doc, banco: bancoDeDados);

						if (doc.ChecagemRoteiro.Id > 0)
						{
							_busCheckList.AlterarSituacao(doc.ChecagemRoteiro.Id, 1, bancoDeDados);
						}

						if (doc.ChecagemPendencia.Id > 0)
						{
							doc.ChecagemPendencia.SituacaoId = 1;//finalizada
							_checagemPendenciaBus.AlterarSituacao(doc.ChecagemPendencia, bancoDeDados);
						}

						if (doc.Fiscalizacao.Id > 0)
						{
							doc.Fiscalizacao.SituacaoNovaTipo = (int)eFiscalizacaoSituacao.CadastroConcluido;
							doc.Fiscalizacao.SituacaoNovaData.Data = DateTime.Now;

							_busFiscalizacao.AlterarSituacaoProcDoc(doc.Fiscalizacao, bancoDeDados);
						}

						using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(UsuarioCredenciado))
						{
							bancoDeDadosCredenciado.IniciarTransacao();

							//CARSolicitacaoBus carSolicitacaoCredenciadoBus = new CARSolicitacaoBus();
							//carSolicitacaoCredenciadoBus.DesassociarProtocolo(new CARSolicitacao() { Requerimento = doc.Requerimento });

							_busProjetoDigital.AlterarSituacao(doc.Requerimento.Id, eProjetoDigitalSituacao.AguardandoProtocolo, bancoDeDadosCredenciado);

							if (!Validacao.EhValido)
							{
								bancoDeDadosCredenciado.Rollback();
								bancoDeDados.Rollback();
								return false;
							}

							bancoDeDadosCredenciado.Commit();
						}

						Validacao.Add(Mensagem.Documento.Excluir);
						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public void AlterarRequerimentoSituacao(Documento documento, int situacao = 2, BancoDeDados banco = null)// 2 Finalizado
		{
			if (documento.Requerimento.Id > 0)
			{
				_busRequerimento.AlterarSituacao(new Requerimento() { Id = documento.Requerimento.Id, SituacaoId = situacao }, banco);
			}
		}

		public void ConverterDocumento(ConverterDocumento convertDoc)
		{
			try
			{
				ProtocoloNumero docNumero = new ProtocoloBus().ObterProtocolo(convertDoc.NumeroDocumento);
				Documento doc = Obter(convertDoc.DocumentoId);
				convertDoc.Processo.SetorId = doc.SetorId;
				convertDoc.Processo.Id = null;

				if (_validar.ValidarConversao(docNumero, User.FuncionarioId) && new ProcessoValidar().Salvar(convertDoc.Processo, true))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.ConverterDocumento(convertDoc, bancoDeDados);

						Validacao.Add(Mensagem.Documento.DocConvertidoSucesso);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		#endregion

		#region Obter / Filtrar

		public ProtocoloLocalizacao ObterLocalizacao(Documento doc, ProtocoloLocalizacao localizacao = null)
		{
			ProtocoloLocalizacao loc = localizacao == null ? new ProtocoloLocalizacao() : localizacao;

			if (loc.ProcessoPaiId <= 0)
			{
				ProtocoloNumero processoPai = ProtocoloAssociado(doc.Id.GetValueOrDefault()) ?? new ProtocoloNumero();
				if (processoPai.Id > 0)
				{
					loc.ProcessoPaiId = processoPai.Id;
					loc.ProcessoPaiNumero = processoPai.NumeroTexto;
					ProcessoBus _busProcesso = new ProcessoBus();
					return _busProcesso.ObterLocalizacao(loc.ProcessoPaiId, loc);
				}
			}

			TramitacaoBus _busTramitacao = new TramitacaoBus(new TramitacaoValidar());

			loc.Tramitacao.Id = _busTramitacao.ObterTramitacaoProtocolo(doc.Id.GetValueOrDefault());

			if (loc.Tramitacao.Id > 0)
			{
				loc.Tramitacao = _busTramitacao.Obter(loc.Tramitacao.Id);
				if (loc.Tramitacao.SituacaoId == (int)eTramitacaoSituacao.Arquivado)
				{
					loc.Localizacao = eLocalizacaoProtocolo.Arquivado;
					ArquivarBus _arquivarBus = new ArquivarBus();
					Arquivar arquivamento = _arquivarBus.ObterArquivamento(loc.Tramitacao.Id);
					loc.ArquivoNome = arquivamento.ArquivoNome;
				}
				else if (loc.Tramitacao.SituacaoId == (int)eTramitacaoSituacao.ParaOrgaoExterno)
				{
					loc.Localizacao = eLocalizacaoProtocolo.OrgaoExterno;
					loc.OrgaoExternoNome = loc.Tramitacao.OrgaoExterno.Texto;
				}
				else if (loc.Tramitacao.SituacaoId == (int)eTramitacaoSituacao.Tramitando)
				{
					if (loc.Tramitacao.Destinatario.Id == 0)
					{
						loc.Localizacao = eLocalizacaoProtocolo.EnviadoParaSetor;
					}
					else
					{
						loc.Localizacao = eLocalizacaoProtocolo.EnviadoParaFuncionario;
						loc.FuncionarioDestinatarioNome = loc.Tramitacao.Destinatario.Nome;
					}
				}
			}
			else // se não existir tramitação, ele está na posse de algum funcionário
			{
				loc.Localizacao = eLocalizacaoProtocolo.PosseFuncionario;
				TramitacaoPosse posse = _busTramitacao.ObterProtocoloPosse(doc.Id.GetValueOrDefault());
				loc.FuncionarioDestinatarioNome = posse.FuncionarioNome;
				loc.SetorDestinatarioNome = posse.SetorNome;
			}
			return loc;
		}

		public Resultados<Protocolo> Filtrar(ListarProtocoloFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				filtrosListar.ProtocoloId = 2;
				Filtro<ListarProtocoloFiltro> filtro = new Filtro<ListarProtocoloFiltro>(filtrosListar, paginacao);
				Resultados<Protocolo> resultados = _da.Filtrar(filtro);

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

		public Documento Obter(int id)
		{
			Documento documento = null;

			try
			{
				documento = _da.Obter(id) as Documento;

				if (documento != null && documento.Arquivo != null && documento.Arquivo.Id > 0)
				{
					ArquivoDa _arquivoDa = new ArquivoDa();
					documento.Arquivo = _arquivoDa.Obter(documento.Arquivo.Id.Value);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return documento;
		}

		public Documento ObterSimplificado(int id, bool suprimirMensagem = false)
		{
			Documento documento = null;

			try
			{
				documento = _da.ObterSimplificado(id) as Documento;

				if ((documento.Id ?? 0) <= 0 && !suprimirMensagem)
				{
					Validacao.Add(Mensagem.Documento.Inexistente);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return documento;
		}

		public Documento ObterAtividades(int id)
		{
			Documento documento = null;

			try
			{
				documento = _da.ObterAtividades(id) as Documento;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return documento;
		}

		public Documento Obter(string numero)
		{
			Documento documento = null;

			try
			{
				documento = _da.Obter(_da.ExisteProtocolo(numero)) as Documento;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return documento;
		}

		public Finalidade ObterTituloAnteriorAtividade(int atividade, int processo, int modeloCodigo)
		{
			try
			{
				return _da.ObterTituloAnteriorAtividade(atividade, processo, modeloCodigo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<PessoaLst> ObterInteressadoRepresentantes(int protocolo)
		{
			try
			{
				return _da.ObterInteressadoRepresentantes(protocolo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public string ObterNumeroProcessoPai(int? id)
		{
			return _da.ObterNumeroProcessoPai(id);
		}

		public Documento ObterDocumentoParaConversao(string strDocumentoNumero)
		{
			ProtocoloNumero docNumero = new ProtocoloNumero();
			Documento doc = new Documento();

			if (String.IsNullOrWhiteSpace(strDocumentoNumero))
			{
				Validacao.Add(Mensagem.Documento.DocumentoNumeroObrigatorio);
				return null;
			}

			if (!ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(strDocumentoNumero))
			{
				Validacao.Add(Mensagem.Documento.NumeroInvalido);
				return null;
			}

			docNumero = new ProtocoloBus().ObterProtocolo(strDocumentoNumero);

			if (_validar.ValidarConversao(docNumero, User.FuncionarioId))
			{
				doc = Obter(docNumero.Id);
			}

			return doc;
		}

		public List<ListaValor> ObterAssinanteFuncionarios(int setorId, int cargoId)
		{
			try
			{
				return _da.ObterAssinanteFuncionarios(setorId, cargoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<ListaValor> ObterAssinanteCargos(int setorId)
		{
			try
			{
				return _da.ObterAssinanteCargos(setorId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<TituloAssinante> ObterAssinantesCargos(List<TituloAssinante> lstAssinantes)
		{
			try
			{
				foreach (TituloAssinante assinante in lstAssinantes)
				{
					assinante.Cargos = _busFuncionario.ObterFuncionarioCargos(assinante.FuncionarioId);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return lstAssinantes;
		}

		#endregion

		#region Verificar / Validar

		public bool ValidarCheckList(int checkListId, int documentoId)
		{
			return _busCheckList.ValidarAssociarCheckList(checkListId, documentoId, false);
		}

		public bool ValidarAssociarResponsavelTecnico(int id)
		{
			return _busPessoa.ValidarAssociarResponsavelTecnico(id);
		}

		public ProtocoloNumero ProtocoloAssociado(int juntado)
		{
			return _da.VerificarProtocoloAssociado(juntado);
		}

		public int ExisteProtocolo(string numero, int excetoId = 0)
		{
			if (numero == null)
			{
				numero = string.Empty;
			}

			return _da.ExisteProtocolo(numero, excetoId);
		}

		public bool ExisteProtocolo(int id)
		{
			return _da.ExisteProtocolo(id);
		}

		public bool VerificarChecagemTemTituloPendencia(int id)
		{
			return _validar.VerificarChecagemTemTituloPendencia(id);
		}

		public bool ExisteAtividade(int id)
		{
			return _da.ExisteAtividade(id);
		}

		public string VerificarDocumentoJuntadoNumero(int protocolo)
		{
			ProtocoloNumero retorno = _da.VerificarProtocoloAssociado(protocolo);
			if (retorno != null)
			{
				return retorno.NumeroTexto;
			}
			return string.Empty;
		}

		public bool EmPosse(int documento)
		{
			return _da.EmPosse(documento);
		}

		#endregion
	}
}