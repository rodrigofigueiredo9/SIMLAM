using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
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
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business
{
	public class DocumentoValidar
	{
		#region Propriedades

		ProtocoloDa _da = new ProtocoloDa();
		PessoaBus _busPessoa = new PessoaBus(new PessoaValidar());
		ChecagemRoteiroBus _busCheckListRoteiro = new ChecagemRoteiroBus();
		RequerimentoBus _busRequerimento = new RequerimentoBus();
		RequerimentoValidar _requerimentoValidar = new RequerimentoValidar();
		GerenciadorConfiguracao<ConfiguracaoTituloModelo> _configTituloModelo = new GerenciadorConfiguracao<ConfiguracaoTituloModelo>(new ConfiguracaoTituloModelo());
		AtividadeDa _atividadeDa = new AtividadeDa();

		public List<int> ModeloCodigosPendencia
		{
			get { return _configTituloModelo.Obter<List<int>>(ConfiguracaoTituloModelo.KeyModeloCodigoPendencia); }
		}

		#endregion

		public bool Salvar(Documento documento)
		{
			#region Validações Genéricas

			if (documento.SetorId <= 0 && documento.Id.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Mensagem.Documento.SetorObrigatorio);
			}

			if (!documento.DataCadastro.Data.HasValue)
			{
				Validacao.Add(Mensagem.Documento.DataCriacaoObrigatoria);
			}
			else
			{
				if (documento.DataCadastro.Data > DateTime.Today)
				{
					Validacao.Add(Mensagem.Documento.DataCriacaoMaiorAtual);
				}
			}

			if (documento.Tipo.Id <= 0)
			{
				Validacao.Add(Mensagem.Documento.DocumentoTipoObrigatorio);
			}

			if (documento.Volume.GetValueOrDefault() <= 0 && documento.Tipo.Id != 13) //Documento Avulso
			{
				Validacao.Add(Mensagem.Documento.QuantidadeDocumentoObrigatoria);
			}

			if (string.IsNullOrEmpty(documento.Nome))
			{
				Validacao.Add(Mensagem.Documento.NomeObrigatorio);
			}

			if (documento.Tipo.Id == (int)eProtocoloTipo.FiscalizacaoSemAI_TEI_TAD)
			{
				if (documento.Fiscalizacao.Id == 0)
				{
					Validacao.Add(Mensagem.Documento.FiscalizacaoObrigatoria);
				}
				else
				{
					FiscalizacaoValidar fiscValidar = new FiscalizacaoValidar();

					if (fiscValidar.PossuiAI_TED_TAD(documento.Fiscalizacao.Id))
					{
						Validacao.Add(Mensagem.Documento.PossuiAI_TEI_TAD);
					}

					String numeroProtocoloAssociado = ValidarFiscalizacaoAssociadaOutroProtocolo(documento.Id.GetValueOrDefault(0), documento.Fiscalizacao.Id);
					if (!String.IsNullOrWhiteSpace(numeroProtocoloAssociado))
					{
						Validacao.Add(Mensagem.Fiscalizacao.FiscalizacaoJaAssociada(numeroProtocoloAssociado));
					}
				}
			}

			if (documento.Tipo.Id <= 0)
			{
				return false;
			}

			ListaBus listaBus = new ListaBus();
			ProtocoloTipo configuracaoDocumentoTipo = listaBus.TiposDocumento.FirstOrDefault(x => x.Id == documento.Tipo.Id);
			Documento documentoOriginal = documento.Id.GetValueOrDefault() > 0 ? _da.Obter(documento.Id.Value) as Documento : new Documento();

			if (configuracaoDocumentoTipo.ProcessoObrigatorio && documento.ProtocoloAssociado.Id.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Mensagem.Documento.ProcessoObrigatorio);
			}

			if (configuracaoDocumentoTipo.ChecagemPendenciaObrigatorio)
			{
				if (documento.ChecagemPendencia.Id <= 0)
				{
					Validacao.Add(Mensagem.Documento.ChecagemPendenciaObrigatoria);
				}
				else
				{
					ChecagemPendenciaJaAssociada(documento.ChecagemPendencia.Id, documento.Id.GetValueOrDefault());
				}
			}

			if (configuracaoDocumentoTipo.InteressadoObrigatorio && documento.Interessado.Id == 0)
			{
				Validacao.Add(Mensagem.Documento.InteressadoObrigatorio);
			}

			if (configuracaoDocumentoTipo.RequerimentoObrigatorio)
			{
				if (documento.ChecagemRoteiro.Id <= 0)
				{
					Validacao.Add(Mensagem.Documento.ChecagemObrigatoria);
				}

				if (documento.Requerimento.Id <= 0)
				{
					Validacao.Add(Mensagem.Documento.RequerimentoObrigatorio);
				}
				else
				{
					if (_requerimentoValidar.RequerimentoDeclaratorio(documento.Requerimento.Id))
					{
						Validacao.Add(Mensagem.Documento.AssociarDeclaratorio);
					}

					RequerimentoFinalizado(documento.Requerimento.Id, documento.Id.GetValueOrDefault());

					ResponsavelTecnico(documento.Responsaveis);

					if (documento.Atividades.Count <= 0)
					{
						Validacao.Add(Mensagem.Documento.AtividadeObrigatoria);
					}
					else
					{
						Atividades(documento.Atividades, (documentoOriginal.Requerimento.Id != documento.Requerimento.Id));

						if (documentoOriginal.Requerimento.Id == documento.Requerimento.Id)
						{
							foreach (Atividade atividade in documentoOriginal.Atividades)
							{
								Atividade atividadeAux = documento.Atividades.SingleOrDefault(x => x.Id == atividade.Id);
								foreach (var finalidade in atividade.Finalidades)
								{
									if (!atividadeAux.Finalidades.Exists(x => x.Id == finalidade.Id) && _atividadeDa.VerificarAtividadeAssociadaTitulo(documentoOriginal.Id.Value, false, atividade.Id, finalidade.TituloModelo))
									{
										Validacao.Add(Mensagem.Atividade.FinalidadeAssociadaTitulo(finalidade.TituloModeloTexto));
									}
								}
							}
						}
					}
				}
			}

			#endregion

			if (documento.Id > 0)
			{
				#region Editar

				if (!_da.EmPosse(documento.Id.Value))
				{
					Validacao.Add(Mensagem.Documento.PosseDocumentoNecessariaEditar);
					return Validacao.EhValido;
				}

				if ((documento.ChecagemRoteiro.Id > 0 && documento.Requerimento.Id > 0) &&
					(documentoOriginal.ChecagemRoteiro.Id != documento.ChecagemRoteiro.Id || documentoOriginal.Requerimento.Id != documento.Requerimento.Id))
				{
					_requerimentoValidar.RoteirosChecagemRequerimento(documento.ChecagemRoteiro.Id, documento.Requerimento.Id, documento.Requerimento.SituacaoId);
				}

				if (documentoOriginal.ChecagemRoteiro.Id != documento.ChecagemRoteiro.Id)
				{
					if (_da.VerificarChecagemTemTituloPendencia(ModeloCodigosPendencia, documento.Id.Value).Count > 0)
					{
						Validacao.Add(Mensagem.Documento.ChecagemAssociadaTitulo);
					}
					else
					{
						_busCheckListRoteiro.ValidarAssociarCheckList(documento.ChecagemRoteiro.Id, documento.Id.Value, false);
					}
				}

				if (documentoOriginal.Requerimento.Id != documento.Requerimento.Id)
				{
					List<String> titulos = _da.VerificarAtividadeAssociadaTitulo(documento.Id.GetValueOrDefault());

					if (titulos.Count < 0)
					{
						Validacao.Add(Mensagem.Documento.RequerimentoAssociadoTitulo());
					}
				}

				if (documento.ChecagemPendencia.Id != documentoOriginal.ChecagemPendencia.Id)
				{
					Validacao.Add(Mensagem.Documento.ChecagemPendenciaAlterada);
				}

				#endregion
			}
			else
			{
				#region Criar

				if (documento.ChecagemRoteiro != null && documento.ChecagemRoteiro.Id > 0)
				{
					_busCheckListRoteiro.ValidarAssociarCheckList(documento.ChecagemRoteiro.Id, documento.Id.GetValueOrDefault(), false);
				}

				if ((documento.ChecagemRoteiro != null && documento.ChecagemRoteiro.Id > 0) && (documento.Requerimento != null && documento.Requerimento.Id > 0))
				{
					_requerimentoValidar.RoteirosChecagemRequerimento(documento.ChecagemRoteiro.Id, documento.Requerimento.Id, documento.Requerimento.SituacaoId);
				}

				#endregion
			}

			return Validacao.EhValido;
		}

		public bool NumeroTipo(string numero, int tipo)
		{
			try
			{
				if (String.IsNullOrWhiteSpace(numero))
				{
					Validacao.Add(Mensagem.Documento.DocumentoNumeroObrigatorio);
				}
				else
				{
					NumeroFormato(numero);
				}

				if (tipo <= 0)
				{
					Validacao.Add(Mensagem.Documento.DocumentoTipoObrigatorio);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return Validacao.EhValido;
		}

		public bool NumeroFormato(string numero)
		{
			if (!ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(numero))
			{
				Validacao.Add(Mensagem.Documento.DocumentoNumeroInvalido);
			}

			return Validacao.EhValido;
		}

		public bool ChecagemPendenciaJaAssociada(int checagem, int excetoId = 0)
		{
			try
			{
				string numero = _da.ChecagemPendenciaJaAssociada(checagem, excetoId);

				if (!string.IsNullOrEmpty(numero))
				{
					Validacao.Add(Mensagem.Documento.ChecagemPendenciaJaAssociada(numero));
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool RequerimentoFinalizado(int requerimento, int excetoId = 0, string local = null)
		{
			try
			{
				if (excetoId > 0)
				{
					Documento documento = _da.ObterSimplificado(excetoId) as Documento;

					if (requerimento == documento.Requerimento.Id)
					{
						return true;
					}
				}

				Requerimento objeto = _busRequerimento.Obter(requerimento);

				if (objeto.SituacaoId != 2)
				{
					Validacao.Add(Mensagem.Documento.RequerimentoSituacaoInvalida);
				}

				objeto.Atividades.ForEach(atividade =>
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
					Validacao.Add(Mensagem.Documento.ResponsavelNomeRazaoObrigatorio(i));
				}

				if (string.IsNullOrEmpty(responsavel.CpfCnpj))
				{
					Validacao.Add(Mensagem.Documento.ResponsavelCpfCnpjObrigatorio(i));
				}

				if (responsavel.Funcao <= 0)
				{
					Validacao.Add(Mensagem.Documento.ResponsavelFuncaoObrigatorio(i));
				}

				if (string.IsNullOrEmpty(responsavel.NumeroArt))
				{
					Validacao.Add(Mensagem.Documento.ResponsavelARTObrigatorio(i));
				}

				_busPessoa.ValidarAssociarResponsavelTecnico(responsavel.Id);

				i++;
			}

			return Validacao.EhValido;
		}

		public bool Atividades(List<Atividade> atividades, bool isCadastrar)
		{
			return _requerimentoValidar.ValidarAtividade(atividades, (isCadastrar ? " do requerimento" : null));
		}

		public void RoteirosChecagemRequerimento(int checagemId, int requerimentoId)
		{
			try
			{
				ChecagemRoteiro checagem = _busCheckListRoteiro.Obter(checagemId);
				Requerimento requerimento = _busRequerimento.Obter(requerimentoId);

				foreach (var roteiro in checagem.Roteiros)
				{
					Boolean isExiste = false;

					foreach (var item in requerimento.Roteiros)
					{
						if (roteiro.Id == item.Id)
						{
							isExiste = true;
							break;
						}
					}

					if (!isExiste)
					{
						Validacao.Add(Mensagem.Documento.RoteirosCheckDifRequerimento);
						return;
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public int ExisteDocumento(string numero)
		{
			return _da.ExisteProtocolo(numero);
		}

		public bool Juntado(int protocolo, bool isEditar = false)
		{
			try
			{
				ProtocoloNumero retorno = _da.VerificarProtocoloAssociado(protocolo);

				string numero = string.Empty;
				
				if (retorno != null)
				{
					numero = retorno.NumeroTexto;
				}
				
				if (!string.IsNullOrEmpty(numero))
				{
					if (isEditar)
					{
						Validacao.Add(Mensagem.Documento.EditarDocumentoJuntado(numero));
					}
					else
					{
						Validacao.Add(Mensagem.Documento.DocumentoJuntado(numero));
					}
					return true;
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		internal bool VerificarPossuiAnalise(int id)
		{
			if (_da.VerificarPossuiAnalise(id))
			{
				Validacao.Add(Mensagem.Documento.ExcluirAnalise);
			}

			return Validacao.EhValido;
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

				Validacao.Add(Mensagem.Documento.ExcluirCARSolicitacao(situacao.Texto));
			}

			return Validacao.EhValido;
		}

		internal bool VerificarPossuiAcompanhamentoFiscalizacao(int id)
		{
			AcompanhamentoBus acompanhamentoBus = new AcompanhamentoBus();
			if (acompanhamentoBus.ExisteAcompanhamento(id))
			{
				Validacao.Add(Mensagem.Documento.ExcluirAcompanhamentoFiscalizacao(id.ToString()));
			}

			return Validacao.EhValido;
		}

		internal bool Excluir(int id)
		{
			if (Juntado(id))
			{
				return false;
			}

			TramitacaoBus busTramitacao = new TramitacaoBus();
			if (busTramitacao.ExisteTramitacao(id))
			{
				Validacao.Add(Mensagem.Documento.EmTramitacao);
				return Validacao.EhValido;
			}

			if (!_da.EmPosse(id))
			{
				Validacao.Add(Mensagem.Documento.PosseDocumentoNecessariaExcluir);
				return Validacao.EhValido;
			}
			
			String processoPaiNumero = _da.ObterNumeroProcessoPai(id);
			if (!String.IsNullOrEmpty(processoPaiNumero))
			{
				Validacao.Add(Mensagem.Documento.NaoPossivelExcluirDocumentoFilho(processoPaiNumero));
			}

			VerificarPossuiAnalise(id);

			Documento documento = _da.Obter(id, true) as Documento;
			if (documento.Fiscalizacao.Id > 0)
			{
				VerificarPossuiAcompanhamentoFiscalizacao(documento.Fiscalizacao.Id);
			}

			VerificarAssociadoTitulo(id);

			VerificarPossuiCARSolicitacao(id);

			return Validacao.EhValido;
		}

		public bool VerificarPossuiPosse(int documento)
		{
			try
			{
				if (!EmPosse(documento))
				{
					Validacao.Add(Mensagem.Documento.PosseDocumentoNecessariaEditar);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool VerificarPossuiAtividadesNaoEncerrada(int documento)
		{
			try
			{
				if (_da.ExisteAtividade(documento) && !_da.VerificarPossuiAtividadesNaoEncerrada(documento))
				{
					Validacao.Add(Mensagem.Documento.DocumentoAtividadesEncerradas);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool EmPosse(int documento, int usuarioId)
		{
			return _da.EmPosse(documento, usuarioId);
		}

		public bool EmPosse(int documento, bool mensagem = false)
		{
			try
			{
				bool possuiPosse = _da.EmPosse(documento);

				if (mensagem && !possuiPosse)
				{
					Validacao.Add(Mensagem.Documento.Posse);
				}
				return possuiPosse;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		internal void VerificarAssociadoTitulo(int documento)
		{
			List<String> titulos = _da.VerificarAssociadoTitulo(documento);

			foreach (var item in titulos)
			{
				Validacao.Add(Mensagem.Documento.MensagemExcluirDocumentoComTitulos(item));
			}
		}

		public bool ValidarPossuiRequerimentoAtividades(Documento doc)
		{
			if (doc.Requerimento == null || doc.Requerimento.Id <= 0)
			{
				Validacao.Add(Mensagem.Documento.SemRequerimentoAtividades);
			}

			return Validacao.EhValido;
		}

		public bool VerificarChecagemTemTituloPendencia(int id)
		{
			if (_da.VerificarChecagemTemTituloPendencia(ModeloCodigosPendencia, id).Count > 0)
			{
				Validacao.Add(Mensagem.Documento.ChecagemComTituloPendencia);
			}

			return Validacao.EhValido;
		}

		public bool ValidarConversao(ProtocoloNumero docNumero, int usuarioId)
		{
			if (docNumero.Id > 0)
			{
				if (docNumero.IsProcesso)
				{
					Validacao.Add(Mensagem.Documento.InformeNDocumento);
					return Validacao.EhValido;
				}

				if (!EmPosse(docNumero.Id, usuarioId))
				{
					Validacao.Add(Mensagem.Documento.Posse);
				}

				if (docNumero.Tipo != 7 && docNumero.Tipo != 10)/*Requerimento (nova solicitação) / Declaração*/
				{
					Validacao.Add(Mensagem.Documento.DocNaoConverter(docNumero.TipoTexto));
				}

				Juntado(docNumero.Id);
			}
			else
			{
				Validacao.Add(Mensagem.Documento.NaoEncontrouRegistros);				
			}

			return Validacao.EhValido;
		}

		internal String ValidarFiscalizacaoAssociadaOutroProtocolo(int protocoloId, int fiscalizacaoId)
		{
			return _da.ValidarFiscalizacaoAssociadaOutroProtocolo(protocoloId, fiscalizacaoId);
		}
	}
}