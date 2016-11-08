using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemRoteiro.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProcesso.Business
{
	public class ProcessoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		ProcessoValidar _validar;
		ProtocoloDa _da;
		PessoaBus _busPessoa;
		ProjetoDigitalBus _busProjetoDigital;
		ProcessoBusWebService _webService;
		RequerimentoBus _busRequerimento;
		ChecagemRoteiroBus _busCheckList;
		FiscalizacaoBus _busFiscalizacao;

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public ProcessoBus()
		{
			_validar = new ProcessoValidar();
			_da = new ProtocoloDa();
			_busPessoa = new PessoaBus(new PessoaValidar());
			_webService = new ProcessoBusWebService();
			_busRequerimento = new RequerimentoBus(new RequerimentoValidar());
			_busProjetoDigital = new ProjetoDigitalBus();
			_busCheckList = new ChecagemRoteiroBus();
			_busFiscalizacao = new FiscalizacaoBus();
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		}

		#region Ações DML

		public bool Salvar(Processo processo)
		{
			try
			{
				processo.Emposse.Id = User.FuncionarioId;
				processo.Requerimento = _busRequerimento.ObterSimplificado(processo.Requerimento.Id);

				if (processo.Arquivo == null)
				{
					processo.Arquivo = new Arquivo();
				}

				bool isEdicao = (processo.Id > 0);

				if (processo.Fiscalizacao.Id > 0)
				{
					processo.Requerimento = new Requerimento();
					processo.Atividades = new List<Atividade>();
					processo.ChecagemRoteiro = new ChecagemRoteiro();
				}

				if (_validar.Salvar(processo))
				{
					#region Arquivos/Diretorio

					if (processo.Arquivo.Id != null && processo.Arquivo.Id == 0)
					{
						ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
						processo.Arquivo = _busArquivo.Copiar(processo.Arquivo);
					}

					#endregion

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						#region Arquivos/Banco

						if (processo.Arquivo.Id == 0)
						{
							ArquivoDa _arquivoDa = new ArquivoDa();
							_arquivoDa.Salvar(processo.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
						}

						#endregion

						#region Atividade

						List<Atividade> lstAtividadesAtual = null;
						if (processo.Id.GetValueOrDefault() > 0)
						{
							lstAtividadesAtual = _da.ObterAtividadesSolicitadas(processo.Id.Value, bancoDeDados);
						}

						if (processo.Atividades != null && processo.Atividades.Count > 0)
						{
							processo.Atividades.ForEach(x =>
							{
								x.Protocolo.Id = processo.Id.GetValueOrDefault();
								x.Protocolo.IsProcesso = true;
							});

							AtividadeBus atividadeBus = new AtividadeBus();
							atividadeBus.AlterarSituacaoProcDoc(processo.Atividades, lstAtividadesAtual, bancoDeDados);
							atividadeBus.TituloAnteriores(processo.Atividades, lstAtividadesAtual, bancoDeDados);
						}

						#endregion

						Processo processoOriginal = new Processo();
						if (processo.Id != null && processo.Id.HasValue && processo.Id.Value > 0)
						{
							processoOriginal = ObterSimplificado(processo.Id.Value);
						}

						//Salvar
						_da.Salvar(processo, bancoDeDados);

						using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(UsuarioCredenciado))
						{
							bancoDeDadosCredenciado.IniciarTransacao();

							// editando processo
							if (processoOriginal.Id > 0)
							{
								// se checagem de itens de roteiro foi alterada, setar o status da antiga como finalizada e setar o status da nova como protocolada
								if (processo.ChecagemRoteiro.Id != processoOriginal.ChecagemRoteiro.Id)
								{
									_busCheckList.AlterarSituacao(processoOriginal.ChecagemRoteiro.Id, 1, bancoDeDados);
								}

								if (processoOriginal.Requerimento.Id != processo.Requerimento.Id)
								{
									processoOriginal.Requerimento = _busRequerimento.ObterSimplificado(processoOriginal.Requerimento.Id);

									if (processoOriginal.Requerimento.IsRequerimentoDigital)
									{
										CARSolicitacaoBus carSolicitacaoCredenciadoBus = new CARSolicitacaoBus();
										carSolicitacaoCredenciadoBus.DesassociarProtocolo(new CARSolicitacao() { Requerimento = processoOriginal.Requerimento }, bancoDeDadosCredenciado);

										if (!_busProjetoDigital.AlterarSituacao(processoOriginal.Requerimento.Id, eProjetoDigitalSituacao.AguardandoProtocolo, bancoDeDadosCredenciado))
										{
											bancoDeDados.Rollback();
											return false;
										}
									}

									AlterarRequerimentoSituacao(processoOriginal, banco: bancoDeDados);
								}

								if (processo.Fiscalizacao.Id != processoOriginal.Fiscalizacao.Id && processoOriginal.Fiscalizacao.Id > 0)
								{
									processoOriginal.Fiscalizacao.SituacaoNovaTipo = (int)eFiscalizacaoSituacao.CadastroConcluido;
									processoOriginal.Fiscalizacao.SituacaoNovaData.Data = DateTime.Now;
									processoOriginal.Fiscalizacao.SituacaoAtualTipo = processo.Fiscalizacao.SituacaoId;

									_busFiscalizacao.AlterarSituacaoProcDoc(processoOriginal.Fiscalizacao, bancoDeDados);
								}
							}

							if (processo.ChecagemRoteiro.Id > 0)
							{
								_busCheckList.AlterarSituacao(processo.ChecagemRoteiro.Id, 2, bancoDeDados);//Checagem Protocolada
								AlterarRequerimentoSituacao(processo, 3, bancoDeDados);// Requerimento Protocolado 
							}

							if (processo.Requerimento.IsRequerimentoDigital)
							{
								CARSolicitacaoBus carSolicitacaoCredenciadoBus = new CARSolicitacaoBus();
								carSolicitacaoCredenciadoBus.AssociarProtocolo(new CARSolicitacao() { Requerimento = processo.Requerimento }, bancoDeDadosCredenciado);

								if (processoOriginal.Requerimento.Id != processo.Requerimento.Id)
								{
									if (!_busProjetoDigital.AlterarSituacao(processo.Requerimento.Id, eProjetoDigitalSituacao.AguardandoAnalise, bancoDeDadosCredenciado))
									{
										bancoDeDados.Rollback();
										return false;
									}
								}
							}

							#region Fiscalizacao

							if (processo.Fiscalizacao.Id != processoOriginal.Fiscalizacao.Id)
							{
								processo.Fiscalizacao.SituacaoNovaTipo = (int)eFiscalizacaoSituacao.Protocolado;
								processo.Fiscalizacao.SituacaoNovaData.Data = DateTime.Now;
								processo.Fiscalizacao.SituacaoAtualTipo = processo.Fiscalizacao.SituacaoId;

								_busFiscalizacao.AlterarSituacaoProcDoc(processo.Fiscalizacao, bancoDeDados);
							}

							#endregion Fiscalizacao

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

					Mensagem msgSucesso = Mensagem.Processo.Salvar(processo.Numero);
					if (isEdicao)
					{
						msgSucesso = Mensagem.Processo.Editar;
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

		public Processo Autuar(Processo processo)
		{
			try
			{
				//Processo processo = _da.ObterSimplificado(id) as Processo;

				if (!string.IsNullOrEmpty(processo.NumeroAutuacao))
				{
					Validacao.Add(Mensagem.Processo.ProcessoJaAutuado);
					return processo;
				}

				processo = _webService.ObterProcesso(processo.Numero, processo.Tipo.Id);
				processo.Id = processo.Id.GetValueOrDefault(0);

				_da.Autuar(processo);

				Validacao.Add(Mensagem.Processo.ProcessoAutuado);
				return processo;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
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
						Processo proc = _da.ObterAtividades(id) as Processo;

						#region Atividades

						AtividadeBus atividadeBus = new AtividadeBus();
						atividadeBus.TituloAnteriores(new List<Atividade>(), proc.Atividades, bancoDeDados);

						#endregion

						_da.Excluir(id, bancoDeDados);

						AlterarRequerimentoSituacao(proc, banco: bancoDeDados);
						if (proc.ChecagemRoteiro.Id > 0)
						{
							_busCheckList.AlterarSituacao(proc.ChecagemRoteiro.Id, 1, bancoDeDados);
						}

						if (proc.Fiscalizacao.Id > 0)
						{
							proc.Fiscalizacao.SituacaoNovaTipo = (int)eFiscalizacaoSituacao.CadastroConcluido;
							proc.Fiscalizacao.SituacaoNovaData.Data = DateTime.Now;

							_busFiscalizacao.AlterarSituacaoProcDoc(proc.Fiscalizacao, bancoDeDados);
						}

						using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(UsuarioCredenciado))
						{
							bancoDeDadosCredenciado.IniciarTransacao();

							CARSolicitacaoBus carSolicitacaoCredenciadoBus = new CARSolicitacaoBus();
							carSolicitacaoCredenciadoBus.DesassociarProtocolo(new CARSolicitacao() { Requerimento = proc.Requerimento }, bancoDeDadosCredenciado);

							_busProjetoDigital.AlterarSituacao(proc.Requerimento.Id, eProjetoDigitalSituacao.AguardandoProtocolo, bancoDeDadosCredenciado);

							if (!Validacao.EhValido)
							{
								bancoDeDadosCredenciado.Rollback();
								bancoDeDados.Rollback();
								return false;
							}

							bancoDeDadosCredenciado.Commit();
						}

						Validacao.Add(Mensagem.Processo.Excluir);
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

		public void AlterarRequerimentoSituacao(Processo processo, int situacao = 2, BancoDeDados banco = null)// 2 Finalizado
		{
			if (processo.Requerimento.Id > 0)
			{
				_busRequerimento.AlterarSituacao(new Requerimento() { Id = processo.Requerimento.Id, SituacaoId = situacao }, banco);
			}
		}

		public void AlterarAtividades(Processo processo, BancoDeDados banco = null)
		{
			_da.AlterarAtividades(processo, banco);
		}

		#endregion

		#region Obter / Filtrar

		public Processo ObterProcessosDocumentos(int processo)
		{
			try
			{
				return _da.ObterProcessosDocumentos(processo) as Processo;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<Protocolo> Filtrar(ListarProtocoloFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				filtrosListar.ProtocoloId = 1;
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

		public Processo Obter(int id)
		{
			Processo processo = null;

			try
			{
				processo = _da.Obter(id) as Processo;

				if ((processo.Id ?? 0) <= 0)
				{
					Validacao.Add(Mensagem.Processo.Inexistente);
				}
				else
				{
					if (processo.Arquivo != null && processo.Arquivo.Id > 0)
					{
						ArquivoDa _arquivoDa = new ArquivoDa();
						processo.Arquivo = _arquivoDa.Obter(processo.Arquivo.Id.Value);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return processo;
		}

		public Processo Obter(string numero)
		{
			Processo proc = null;

			try
			{
				proc = _da.Obter(_da.ExisteProtocolo(numero, protocoloTipo: (int)eTipoProtocolo.Processo)) as Processo;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return proc;
		}

		public Processo ObterAtividades(int id)
		{
			Processo processo = null;
			try
			{
				processo = _da.ObterAtividades(id) as Processo;
				if ((processo.Id ?? 0) <= 0)
				{
					Validacao.Add(Mensagem.Processo.Inexistente);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return processo;
		}

		public Processo ObterSimplificado(int id, bool suprimirMensagemInexistente = false)
		{
			Processo processo = null;

			try
			{
				processo = _da.ObterSimplificado(id) as Processo;

				if (!suprimirMensagemInexistente && (processo.Id ?? 0) <= 0)
				{
					Validacao.Add(Mensagem.Processo.Inexistente);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return processo;
		}

		public Finalidade ObterTituloAnteriorAtividade(int atividade, int processo, int modelo)
		{
			try
			{
				return _da.ObterTituloAnteriorAtividade(atividade, processo, modelo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		// orgao externo
		// OU setor x
		// OU arquivo y
		// OU tramitando para setor j
		// OU tramitando para funcionario k no setor i
		// OU em posse do funcionario w no setor z
		// OU apensado/juntado a um processo
		public ProtocoloLocalizacao ObterLocalizacao(int processoId, ProtocoloLocalizacao localizacao = null)
		{
			ProtocoloLocalizacao loc = localizacao == null ? new ProtocoloLocalizacao() : localizacao;
			Processo proc = ObterSimplificado(processoId);

			if (loc.ProcessoPaiId <= 0)
			{
				int ApensadoEmProcessoId = ProcessoApensado(processoId);
				if (ApensadoEmProcessoId > 0)
				{
					loc.ProcessoPaiId = ApensadoEmProcessoId;
					loc.ProcessoPaiNumero = ObterSimplificado(loc.ProcessoPaiId).Numero;
					return ObterLocalizacao(loc.ProcessoPaiId, loc);
				}
			}

			TramitacaoBus _busTramitacao = new TramitacaoBus();

			loc.Tramitacao.Id = _busTramitacao.ObterTramitacaoProtocolo(processoId);

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
				TramitacaoPosse posse = _busTramitacao.ObterProtocoloPosse(processoId);
				loc.FuncionarioDestinatarioNome = posse.FuncionarioNome;
				loc.SetorDestinatarioNome = posse.SetorNome;
			}
			return loc;
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

		public Processo ObterJuntadosApensados(int id, BancoDeDados banco = null)
		{
			return _da.ObterJuntadosApensados(id, banco) as Processo;
		}

		internal string ObterNumeroProcessoPai(int? id)
		{
			return _da.ObterNumeroProcessoPai(id);
		}

		internal int ObterSetor(int? protocolo)
		{
			return _da.ObterSetor(protocolo ?? 0);
		}

		#endregion

		#region Verificar / Validar

		public bool ValidarAssociarResponsavelTecnico(int id)
		{
			return _busPessoa.ValidarAssociarResponsavelTecnico(id);
		}

		public bool ValidarCheckList(int checkListId, int processoId)
		{
			return _busCheckList.ValidarAssociarCheckList(checkListId, processoId, true);
		}

		public bool ValidarAssociarFiscalizacao(int fiscalizacaoId)
		{
			return _busFiscalizacao.ValidarAssociar(fiscalizacaoId);
		}

		public bool ValidarDesassociarFiscalizacao(int fiscalizacaoId)
		{
			_busFiscalizacao.ValidarDesassociar(fiscalizacaoId);

			AcompanhamentoBus acompanhamentoBus = new AcompanhamentoBus();

			if (acompanhamentoBus.ObterAcompanhamentos(fiscalizacaoId).Itens.Count > 0)
			{
				Validacao.Add(Mensagem.Processo.FiscalizacaoAssociadaPossuiAcompanhamento);
			}

			return Validacao.EhValido;
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

		public ProtocoloNumero ObterProtocolo(string numero)
		{
			return _da.ObterProtocolo(numero);
		}

		public int ProcessoApensado(int protocolo)
		{
			ProtocoloNumero retorno = _da.VerificarProtocoloAssociado(protocolo);
			if (retorno != null)
			{
				return retorno.Id;
			}
			return 0;
		}

		public bool ExisteProcessoAtividade(int processo)
		{
			return _validar.ExisteProcessoAtividade(processo);
		}

		public bool VerificarChecagemTemTituloPendencia(int id)
		{
			return _validar.VerificarChecagemTemTituloPendencia(id);
		}

		public string VerificarProcessoApensadoNumero(int protocolo)
		{
			ProtocoloNumero retorno = _da.VerificarProtocoloAssociado(protocolo);
			if (retorno != null)
			{
				return retorno.NumeroTexto;
			}
			return string.Empty;
		}

		public bool EmPosse(int processoId)
		{
			return _da.EmPosse(processoId);
		}

		public bool ExisteAtividade(int id)
		{
			return _da.ExisteAtividade(id);
		}

		#endregion
	}
}