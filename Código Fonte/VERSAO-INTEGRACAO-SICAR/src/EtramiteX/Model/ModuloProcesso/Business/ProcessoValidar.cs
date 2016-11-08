using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemRoteiro.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProcesso.Business
{
	public class ProcessoValidar
	{
		#region Propriedades

		ProtocoloDa _da;
		PessoaBus _busPessoa;
		ChecagemRoteiroBus _busCheckListRoteiro;
		RequerimentoValidar _requerimentoValidar;
		GerenciadorConfiguracao<ConfiguracaoTituloModelo> _configTituloModelo;
		AtividadeDa _atividadeDa = new AtividadeDa();

		internal List<int> ModeloCodigosPendencia
		{
			get { return _configTituloModelo.Obter<List<int>>(ConfiguracaoTituloModelo.KeyModeloCodigoPendencia); }
		}

		#endregion

		public ProcessoValidar()
		{
			_da = new ProtocoloDa();
			_busPessoa = new PessoaBus(new PessoaValidar());
			_busCheckListRoteiro = new ChecagemRoteiroBus();
			_requerimentoValidar = new RequerimentoValidar();
			_configTituloModelo = new GerenciadorConfiguracao<ConfiguracaoTituloModelo>(new ConfiguracaoTituloModelo());
		}

		public bool Salvar(Processo processo, bool isConversao = false)
		{
			RequerimentoValidar _requerimentoValidar = new RequerimentoValidar();

			#region Validações Genérica

			if (processo.SetorId <= 0 && processo.Id.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Mensagem.Processo.SetorObrigatorio);
			}

			if (!processo.DataCadastro.Data.HasValue)
			{
				Validacao.Add(Mensagem.Processo.DataCriacaoObrigatoria);
			}
			else
			{
				if (processo.DataCadastro.Data > DateTime.Today)
				{
					Validacao.Add(Mensagem.Processo.DataCriacaoMaiorAtual);
				}
			}

			if (processo.Tipo.Id <= 0)
			{
				Validacao.Add(Mensagem.Processo.ProcessoTipoObrigatorio);
			}

			if (processo.Volume.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Mensagem.Processo.QuantVolumesObrigatoria);
			}

			if (!processo.PossuiSEP.HasValue)
			{
				Validacao.Add(Mensagem.Processo.PossuiNumeroSEPObrigatorio);
			}
			else
			{
				if (processo.PossuiSEP.Value)
				{
					int numeroAutuacao = 0;

					if (String.IsNullOrWhiteSpace(processo.NumeroAutuacao))
					{
						Validacao.Add(Mensagem.Processo.NumeroAutuacaoObrigatorio);
					}
					else if (!Int32.TryParse(processo.NumeroAutuacao, out numeroAutuacao))
					{
						Validacao.Add(Mensagem.Processo.NumeroAutuacaoFormato);
					}
					else
					{
						String numProcesso;
						if (ExisteNumeroSEP(processo, out numProcesso))
						{
							Validacao.Add(Mensagem.Processo.NumeroAutuacaoJaExistente(numProcesso));
						}
					}

					if (!processo.DataAutuacao.Data.HasValue)
					{
						Validacao.Add(Mensagem.Processo.DataAutuacaoObrigatoria);
					}
					else
					{
						if (processo.DataAutuacao.Data > DateTime.Today)
						{
							Validacao.Add(Mensagem.Processo.DataAutuacaoMaiorAtual);
						}
					}
				}

			}

			if (processo.Tipo.Id <= 0)
			{
				return false;
			}

			ValidarProcessoTodosCampos(processo, isConversao);

			if (processo.Tipo.Id == 2/*Fiscalização*/ && processo.Fiscalizacao.Id == 0)
			{
				Validacao.Add(Mensagem.Processo.FiscalizacaoObrigatoria);
			}

			if (processo.Requerimento.Id > 0)
			{
				ResponsavelTecnico(processo.Responsaveis);

				if (_requerimentoValidar.RequerimentoDeclaratorio(processo.Requerimento.Id))
				{
					Validacao.Add(Mensagem.Processo.AssociarDeclaratorio);
				}
			}

			#endregion

			if (processo.Fiscalizacao.Id > 0)
			{
				String numeroProcessoAssociado = ValidarFiscalizacaoAssociadaOutroProtocolo(processo.Id.GetValueOrDefault(0), processo.Fiscalizacao.Id);
				if (!String.IsNullOrWhiteSpace(numeroProcessoAssociado)) 
				{
					Validacao.Add(Mensagem.Fiscalizacao.FiscalizacaoJaAssociada(numeroProcessoAssociado));
				}
			}

			if (processo.Id > 0)
			{
				#region Validações ao Editar

				if (!EmPosse(processo.Id.Value))
				{
					Validacao.Add(Mensagem.Processo.PosseProcessoNecessariaEditar);
					return Validacao.EhValido;
				}

				Processo processoOriginal = _da.ObterSimplificado(processo.Id.Value) as Processo;

				if ((processo.ChecagemRoteiro.Id > 0 && processo.Requerimento.Id > 0) &&
					(processoOriginal.ChecagemRoteiro.Id != processo.ChecagemRoteiro.Id || processoOriginal.Requerimento.Id != processo.Requerimento.Id))
				{
					_requerimentoValidar.RoteirosChecagemRequerimento(processo.ChecagemRoteiro.Id, processo.Requerimento.Id, processo.Requerimento.SituacaoId);
				}

				if (processoOriginal.ChecagemRoteiro.Id != processo.ChecagemRoteiro.Id)
				{
					if (_da.VerificarChecagemTemTituloPendencia(ModeloCodigosPendencia, processo.Id.Value).Count > 0)
					{
						Validacao.Add(Mensagem.Documento.ChecagemAssociadaTitulo);
					}
					else
					{
						_busCheckListRoteiro.ValidarAssociarCheckList(processo.ChecagemRoteiro.Id, processo.Id.GetValueOrDefault(), true);
					}
				}

				if (processoOriginal.Requerimento.Id != processo.Requerimento.Id)
				{
					List<String> titulos = _da.VerificarAtividadeAssociadaTitulo(processo.Id.GetValueOrDefault());

					if (titulos.Count > 0)
					{
						Validacao.Add(Mensagem.Processo.RequerimentoAssociadoTitulo());
					}
				}

				if (processoOriginal.Fiscalizacao.Id != processo.Fiscalizacao.Id)
				{
					AcompanhamentoBus acompanhamentoBus = new AcompanhamentoBus();

					if (acompanhamentoBus.ObterAcompanhamentos(processoOriginal.Fiscalizacao.Id).Itens.Count > 0)
					{
						Validacao.Add(Mensagem.Processo.FiscalizacaoAssociadaPossuiAcompanhamento);
					}
				}

				#endregion
			}
			else
			{
				#region Validações ao cadastrar

				if (processo.ChecagemRoteiro != null && processo.ChecagemRoteiro.Id > 0)
				{
					_busCheckListRoteiro.ValidarAssociarCheckList(processo.ChecagemRoteiro.Id, processo.Id.GetValueOrDefault(), true, isConversao);
				}

				if ((processo.ChecagemRoteiro != null && processo.ChecagemRoteiro.Id > 0) && (processo.Requerimento != null && processo.Requerimento.Id > 0))
				{
					_requerimentoValidar.RoteirosChecagemRequerimento(processo.ChecagemRoteiro.Id, processo.Requerimento.Id, processo.Requerimento.SituacaoId);
				}

				#endregion
			}

			return Validacao.EhValido;
		}

		public void ValidarProcessoTodosCampos(Processo processo, bool isConversao = false)
		{
			ListaBus listaBus = new ListaBus();
			ProtocoloTipo configuracaoProtocoloTipo = listaBus.TiposProcesso.FirstOrDefault(x => x.Id == processo.Tipo.Id);

			if (processo.ChecagemRoteiro.Id <= 0 && processo.Requerimento.Id <= 0)
			{
				if (processo.Interessado.Id <= 0)
				{
					Validacao.Add(Mensagem.Processo.InteressadoObrigatorio);
				}
			}
			else
			{
				if ((configuracaoProtocoloTipo.ChecagemRoteiroObrigatorio && processo.ChecagemRoteiro.Id <= 0) ||
					processo.Requerimento.Id > 0 && processo.ChecagemRoteiro.Id <= 0)
				{
					Validacao.Add(Mensagem.Processo.ChecagemObrigatoria);
					return;
				}

				if (configuracaoProtocoloTipo.FiscalizacaoObrigatorio && processo.Fiscalizacao.Id <= 0)
				{
					Validacao.Add(Mensagem.Processo.FiscalizacaoObrigatoria);
				}

				if ((configuracaoProtocoloTipo.RequerimentoObrigatorio && processo.Requerimento.Id <= 0) ||
					processo.ChecagemRoteiro.Id > 0 && processo.Requerimento.Id <= 0)
				{
					Validacao.Add(Mensagem.Processo.RequerimentoObrigatorio);
				}
				else
				{
					if (!isConversao)
					{
						RequerimentoFinalizado(processo.Requerimento.Id, processo.Id.GetValueOrDefault());						
					}

					if (processo.Atividades.Count <= 0)
					{
						Validacao.Add(Mensagem.Processo.AtividadeObrigatoria);
					}
					else
					{
						Processo processoBanco = processo.Id.GetValueOrDefault() > 0 ? _da.Obter(processo.Id.Value) as Processo : new Processo();
						ValidarAtividades(processo.Atividades, (processoBanco.Requerimento.Id != processo.Requerimento.Id));

						if (processoBanco.Requerimento.Id == processo.Requerimento.Id)
						{
							foreach (Atividade atividade in processoBanco.Atividades)
							{
								Atividade atividadeAux = processo.Atividades.SingleOrDefault(x => x.Id == atividade.Id);
								foreach (var finalidade in atividade.Finalidades)
								{
									if (!atividadeAux.Finalidades.Exists(x => x.Id == finalidade.Id) && _atividadeDa.VerificarAtividadeAssociadaTitulo(processoBanco.Id.Value, true, atividade.Id, finalidade.TituloModelo))
									{
										Validacao.Add(Mensagem.Atividade.FinalidadeAssociadaTitulo(finalidade.TituloModeloTexto));
									}
								}
							}
						}
					}
				}
			}
		}

		public bool Numero(string numero)
		{
			try
			{
				if (String.IsNullOrWhiteSpace(numero))
				{
					Validacao.Add(Mensagem.Processo.ProcessoNumeroObrigatorio);
				}
				else
				{
					if (!ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(numero))
					{
						Validacao.Add(Mensagem.Processo.ProcessoNumeroInvalido);
					}

					return Validacao.EhValido;
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return Validacao.EhValido;
		}

		public int ExisteProtocolo(string numero, int excetoId = 0)
		{
			try
			{
				return _da.ExisteProtocolo(numero, excetoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

		public bool RequerimentoFinalizado(int requerimentoId, int excetoId = 0, bool isProcesso = true, string local = null)
		{
			try
			{
				if (excetoId > 0)
				{
					if (isProcesso)
					{
						Processo processo = _da.ObterSimplificado(excetoId) as Processo;

						if (requerimentoId == processo.Requerimento.Id)
						{
							return true;
						}
					}
					else
					{
						DocumentoBus busDoc = new DocumentoBus();
						Documento documento = busDoc.ObterSimplificado(excetoId);

						if (requerimentoId == documento.Requerimento.Id)
						{
							return true;
						}
					}
				}

				RequerimentoBus busRequerimento = new RequerimentoBus();

				Requerimento requerimento = busRequerimento.Obter(requerimentoId);

				if (requerimento.SituacaoId != (int)eRequerimentoSituacao.Finalizado)
				{
					Validacao.Add(Mensagem.Processo.RequerimentoSituacaoInvalida);
				}

				requerimento.Atividades.ForEach(atividade =>
				{
					if (atividade.SituacaoId == (int)eAtividadeSituacao.Desativada)
					{
						Validacao.Add(Mensagem.Requerimento.AtividadeDesativada(atividade.NomeAtividade, local ?? string.Empty));
					}
				});
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool ResponsavelTecnico(List<ResponsavelTecnico> responsaveisTecnicos)
		{
			int i = 0;
			foreach (ResponsavelTecnico responsavel in responsaveisTecnicos)
			{
				if (string.IsNullOrEmpty(responsavel.NomeRazao))
				{
					Validacao.Add(Mensagem.Processo.ResponsavelNomeRazaoObrigatorio(i));
				}

				if (string.IsNullOrEmpty(responsavel.CpfCnpj))
				{
					Validacao.Add(Mensagem.Processo.ResponsavelCpfCnpjObrigatorio(i));
				}

				if (responsavel.Funcao <= 0)
				{
					Validacao.Add(Mensagem.Processo.ResponsavelFuncaoObrigatorio(i));
				}

				if (string.IsNullOrEmpty(responsavel.NumeroArt))
				{
					Validacao.Add(Mensagem.Processo.ResponsavelARTObrigatorio(i));
				}

				_busPessoa.ValidarAssociarResponsavelTecnico(responsavel.Id);

				i++;
			}

			return Validacao.EhValido;
		}

		public bool ValidarAtividades(List<Atividade> atividades, bool isCadastrar)
		{
			return _requerimentoValidar.ValidarAtividade(atividades, (isCadastrar ? " do requerimento" : null));
		}

		public bool EmPosse(int processo, int msg = 0)
		{
			try
			{
				if (!_da.EmPosse(processo))
				{
					switch (msg)
					{
						case 1:
							Validacao.Add(Mensagem.Processo.PosseProcessoNecessariaEditar);
							break;

						case 2:
							Validacao.Add(Mensagem.Processo.PosseProcessoNecessaria);
							break;

						default:
							break;
					}

					return false;
				}

				return true;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return false;
		}

		public bool VerificarPossuiAtividadesNaoEncerrada(int processoId)
		{
			try
			{
				if (_da.ExisteAtividade(processoId) && !_da.VerificarPossuiAtividadesNaoEncerrada(processoId))
				{
					Validacao.Add(Mensagem.Processo.DocumentoAtividadesEncerradas);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool TemProcessoAtividadeEncerrada(List<Atividade> atividades)
		{
			foreach (Atividade atividade in atividades)
			{
				if (atividade.SituacaoId != 7)
				{
					return false;
				}
			}

			return true;
		}

		internal bool VerificarPossuiCARSolicitacao(int id)
		{
			List<Situacao> lista = _da.VerificarPossuiCARSolicitacao(id);
			if (lista.Count > 0)
			{
				Situacao situacao = lista.FirstOrDefault(x => x.Id == (int)eCARSolicitacaoSituacao.Invalido);
				if (situacao == null)
				{
					situacao = lista.FirstOrDefault(x => x.Id != (int)eCARSolicitacaoSituacao.Invalido);
				}

				Validacao.Add(Mensagem.Processo.ExcluirCARSolicitacao(situacao.Texto));
			}

			return Validacao.EhValido;
		}

		internal bool VerificarPossuiAcompanhamentoFiscalizacao(int id)
		{
			AcompanhamentoBus acompanhamentoBus = new AcompanhamentoBus();
			if (acompanhamentoBus.ExisteAcompanhamento(id))
			{
				Validacao.Add(Mensagem.Processo.ExcluirAcompanhamentoFiscalizacao(id.ToString()));
			}

			return Validacao.EhValido;
		}

		internal bool Excluir(int id)
		{
			try
			{
				if (!EmPosse(id))
				{
					Validacao.Add(Mensagem.Processo.PosseProcessoNecessariaExcluir);
					return Validacao.EhValido;
				}

				String processoPaiNumero = _da.ObterNumeroProcessoPai(id);
				if (!String.IsNullOrEmpty(processoPaiNumero))
				{
					Validacao.Add(Mensagem.Processo.ExcluirApensadoPai(processoPaiNumero));
					return Validacao.EhValido;
				}

				List<string> processosFilhos = _da.ObterNumeroProcessoFilhos(id);
				foreach (var item in processosFilhos)
				{
					Validacao.Add(Mensagem.Processo.ExcluirApensadoFilho(item));
				}

				List<string> documentosFilhos = _da.ObterNumeroDocumentosFilhos(id);
				foreach (var item in documentosFilhos)
				{
					Validacao.Add(Mensagem.Processo.ExcluirDocumentosJuntados(item));
				}

				List<string> documentosAssociados = _da.VerificarProtocoloAssociadoOutroProtocolo(id);
				foreach (var item in documentosAssociados)
				{
					Validacao.Add(Mensagem.Processo.ProcessoAssociadoProtocolo(item));
				}

				TramitacaoBus busTramitacao = new TramitacaoBus();
				if (busTramitacao.ExisteTramitacao(id))
				{
					Validacao.Add(Mensagem.Processo.EmTramitacao);
				}

				if (_da.VerificarPossuiAnalise(id))
				{
					Validacao.Add(Mensagem.Processo.ExcluirAnalise);
				}

				Processo processo = _da.Obter(id, true) as Processo;
				if (processo.Fiscalizacao.Id > 0)
				{
					if (processo.Fiscalizacao.SituacaoId >= 5)
					{
						Validacao.Add(Mensagem.Processo.FiscalizacaoAssociadaNaoPodeExluirSituacao(processo.Fiscalizacao.Id.ToString(), processo.Fiscalizacao.SituacaoTexto));
					}
					else
					{
						VerificarPossuiAcompanhamentoFiscalizacao(processo.Fiscalizacao.Id);
					}
				}

				VerificarAssociadoTitulo(id);

				VerificarPossuiCARSolicitacao(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		internal bool ExisteProcessoAtividade(int processo)
		{
			if (!_da.ExisteAtividade(processo))
			{
				Validacao.Add(Mensagem.Processo.ProcessoSemAtividade);
			}

			return Validacao.EhValido;
		}

		internal bool ExisteNumeroSEP(Processo processo, out String numProcesso) 
		{
			return _da.VerificarNumeroSEP(processo, out numProcesso);
		}

		internal void VerificarAssociadoTitulo(int processo)
		{
			List<String> titulos = _da.VerificarAssociadoTitulo(processo);

			foreach (var item in titulos)
			{
				Validacao.Add(Mensagem.Processo.MensagemExcluirProcessoComTitulos(item));
			}
		}

		public bool VerificarChecagemTemTituloPendencia(int id)
		{
			if (_da.VerificarChecagemTemTituloPendencia(ModeloCodigosPendencia, id).Count > 0)
			{
				Validacao.Add(Mensagem.Processo.ChecagemComTituloPendencia);
			}

			return Validacao.EhValido;
		}

		internal String ValidarFiscalizacaoAssociadaOutroProtocolo(int processoId, int fiscalizacaoId) 
		{
			return _da.ValidarFiscalizacaoAssociadaOutroProtocolo(processoId, fiscalizacaoId);
		}
	}
}