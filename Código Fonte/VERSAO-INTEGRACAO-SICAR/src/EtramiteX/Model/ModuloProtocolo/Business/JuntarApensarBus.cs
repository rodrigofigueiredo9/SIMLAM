using System;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProcesso.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business
{
	public class JuntarApensarBus
	{
		#region Propriedade

		ProcessoValidar _validarProcesso = new ProcessoValidar();
		DocumentoValidar _validarDocumento = new DocumentoValidar();
		JuntarApensarValidar _validar = new JuntarApensarValidar();
		ProtocoloBus _busProtocolo = new ProtocoloBus();
		ProcessoBus _busProcesso = new ProcessoBus();
		DocumentoBus _busDocumento = new DocumentoBus();
		Historico _historico = new Historico();		
		internal Historico Historico { get { return _historico; } }
		
		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Ações

		public bool EditarApensadosJuntados(Processo processo)
		{
			try
			{
				processo.Emposse.Id = User.FuncionarioId;

				if (_validar.EditarApensadosJuntados(processo))
				{
					Mensagem msgSucesso = Mensagem.Processo.Editar;
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						#region Protocolo

						foreach (Processo item in processo.Processos)
						{
							if (item.Id.GetValueOrDefault() > 0)
							{
								Processo processoOriginal = new Processo();
								processoOriginal = _busProcesso.ObterSimplificado(item.Id.Value);

								if (processoOriginal.Requerimento.Id != item.Requerimento.Id)
								{
									_busProcesso.AlterarRequerimentoSituacao(processoOriginal, banco: bancoDeDados);
								}
							}

							_busProcesso.AlterarAtividades(item, bancoDeDados);
							_busProcesso.AlterarRequerimentoSituacao(item, (int)eRequerimentoSituacao.Protocolado, bancoDeDados);
						}

						#endregion

						#region Documento

						if (processo.Documentos.Count > 0)
						{
							ProtocoloDa _busProcessoDoc = new ProtocoloDa();

							foreach (Documento item in processo.Documentos)
							{
								if (item.Id.GetValueOrDefault() > 0)
								{
									Documento documentoOriginal = new Documento();
									documentoOriginal = _busDocumento.ObterSimplificado(item.Id.Value);

									if (documentoOriginal.Requerimento.Id != item.Requerimento.Id)
									{
										_busDocumento.AlterarRequerimentoSituacao(documentoOriginal, banco: bancoDeDados);
									}
								}

								_busProcessoDoc.AlterarAtividades(item, bancoDeDados);
								_busDocumento.AlterarRequerimentoSituacao(item, (int)eRequerimentoSituacao.Protocolado, bancoDeDados);
							}
						}

						#endregion

						bancoDeDados.Commit();
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

		public bool JuntarApensar(Processo processo)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					if (processo.Documentos != null && processo.Documentos.Count > 0)
					{
						foreach (Documento doc in processo.Documentos)
						{
							VerificarJuntarDocumento(doc.Id.Value, doc.Numero, processo.Id.Value);
						}
					}

					if (processo.Processos != null && processo.Processos.Count > 0)
					{
						foreach (Processo proc in processo.Processos)
						{
							VerificarApensarProcesso(proc.Id.Value, proc.Numero, processo.Id.Value);
						}
					}

					if (Validacao.EhValido)
					{
						ProtocoloDa processoDa = new ProtocoloDa();						
						processoDa.JuntarApensar(processo, bancoDeDados);
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

		#endregion

		#region Obter / Filtrar

		public Processo ObterJuntadosApensados(int id)
		{
			Processo processo = null;

			try
			{
				processo = _busProcesso.ObterJuntadosApensados(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return processo;
		}

		public Processo ObterProcessosDocumentos(int id)
		{
			Processo processo = null;

			try
			{
				processo = _busProtocolo.ObterProcessosDocumentos(id) as Processo;
				if (processo == null)
				{
					Validacao.Add(Mensagem.Processo.Inexistente);
					return processo;
				}

				/*for (int i = 0; i < processo.Documentos.Count; i++)
				{
					processo.Documentos[i] = _busDocumento.ObterAtividades(processo.Documentos[i].id.Value);
				}*/

				processo.Processos.RemoveAll(x => x.Requerimento.Id <= 0);
				processo.Documentos.RemoveAll(x => x.Requerimento.Id <= 0);
				//remove os documentos que não tem requerimento ou atividade
				processo.Documentos.RemoveAll(x => x == null);

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return processo;
		}

		public String ObterNumeroProcessoPaiApensado(int procApensadoId)
		{
			try
			{
				return _busProtocolo.ObterNumeroProcessoPai(procApensadoId);
			}
			catch (Exception)
			{

				throw;
			}
		}

		public String ObterNumeroProcessoPaiJuntado(int docJuntadoId)
		{
			try
			{
				return _busDocumento.VerificarDocumentoJuntadoNumero(docJuntadoId);
			}
			catch (Exception)
			{
				throw;
			}
		}

		#endregion

		#region Verificar / Validar

		public bool VerificarApensarProcesso(int procFilho, string procNumero, int procPaiId)
		{
			Processo procSimples = _busProcesso.ObterSimplificado(procFilho, true);
			if (procSimples == null || procSimples.Id.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Mensagem.Processo.InexistenteNumero(procNumero));
			}
			else
			{
				VerificarApensarProcesso(procSimples.Numero, procPaiId);
			}
			return Validacao.EhValido;
		}

		public bool VerificarJuntarApensar(string numero)
		{
			if (_validarProcesso.Numero(numero))
			{
				int processoId = _busProtocolo.ExisteProtocolo(numero, protocoloTipo: (int)eTipoProtocolo.Processo);
				VerificarJuntarApensar(processoId);
			}

			return Validacao.EhValido;
		}

		public bool VerificarJuntarApensar(int? Id)
		{
			if (!_busProtocolo.ExisteProtocolo(Id.Value))
			{
				Validacao.Add(Mensagem.Processo.Inexistente);
				return Validacao.EhValido;
			}

			string apensadoEmProcessoNumero = _busProtocolo.ObterNumeroProcessoPai(Id.Value);
			if (!String.IsNullOrEmpty(apensadoEmProcessoNumero))
			{
				Validacao.Add(Mensagem.Processo.ProcessoNaoPodeApensarJuntarPoisEstaApensado(apensadoEmProcessoNumero));
				return Validacao.EhValido;
			}

			if (!_validarProcesso.EmPosse(Id.Value))
			{
				Validacao.Add(Mensagem.Processo.ProcessoNaoPodeApensarPoisNaoPossuiPosse);
				return Validacao.EhValido;
			}
			return Validacao.EhValido;
		}

		public bool VerificarJuntarDocumento(int docId, string docNumero, int procPaiId)
		{
			Documento docSimples = _busDocumento.ObterSimplificado(docId, true);
			if (docSimples == null || docSimples.Id == null || !docSimples.Id.HasValue || docSimples.Id <= 0)
			{
				Validacao.Add(Mensagem.Documento.InexistenteNumero(docNumero));
			}
			else
			{
				VerificarJuntarDocumento(docSimples, procPaiId);
			}
			return Validacao.EhValido;
		}

		public bool VerificarJuntarDocumento(string numero, int procPaiId)
		{
			if (String.IsNullOrWhiteSpace(numero))
			{
				Validacao.Add(Mensagem.Tramitacao.JuntarApensarDocumentoNumeroObrigatorio);
			}
			else if (!ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(numero))
			{
				Validacao.Add(Mensagem.Tramitacao.JuntarApensarDocumentoNumeroInvalido);
			}

			if (Validacao.EhValido)
			{
				IProtocolo prot = _busProtocolo.ObterSimplificado(_busProtocolo.ExisteProtocolo(numero), true);

				if (prot == null || prot.Id.GetValueOrDefault() <= 0 || prot.IsProcesso)
				{
					Validacao.Add(Mensagem.Documento.Inexistente);
					return Validacao.EhValido;
				}

				VerificarJuntarDocumento(prot as Documento, procPaiId);
			}
			return Validacao.EhValido;
		}

		public bool VerificarJuntarDocumento(Documento protocolo, int procId)
		{
			if (!_validarDocumento.NumeroTipo(protocolo.Numero, protocolo.Tipo.Id))
			{
				return Validacao.EhValido;
			}

			// verifica se estamos tentando juntar um documento em um processo diferente do associado (caso haja processo associado)
			if (protocolo.ProtocoloAssociado.Id > 0 && protocolo.ProtocoloAssociado.Id != procId)
			{
				Validacao.Add(Mensagem.Processo.DocumentoNaoPodeJuntarProcessoDiferenteAssociado);
				return Validacao.EhValido;
			}

			Processo procPai = _busProcesso.ObterSimplificado(procId, true);

			// Valida se o documento já está juntado
			string juntadoEmProcessoNumero = _busDocumento.ObterNumeroProcessoPai(protocolo.Id);

			if (!String.IsNullOrEmpty(juntadoEmProcessoNumero) && juntadoEmProcessoNumero != procPai.Numero)
			{
				Validacao.Add(Mensagem.Processo.DocumentoNaoPodeJuntarPoisEstaJuntado(juntadoEmProcessoNumero));
				return Validacao.EhValido;
			}

			if (String.IsNullOrEmpty(juntadoEmProcessoNumero) && !_validarDocumento.EmPosse(protocolo.Id.Value))
			{
				Validacao.Add(Mensagem.Processo.DocumentoNaoPodeJuntarPoisNaoPossuiPosse);
				return Validacao.EhValido;
			}

			if (String.IsNullOrEmpty(juntadoEmProcessoNumero) && protocolo.SetorId != procPai.SetorId)
			{
				Validacao.Add(Mensagem.Processo.DocumentoNaoPodeJuntarPoisNaoEstaNoMesmoSetor);
				return Validacao.EhValido;
			}

			return Validacao.EhValido;
		}

		public bool VerificarApensarProcesso(string numero, int procPaiId)
		{
			if (String.IsNullOrWhiteSpace(numero))
			{
				Validacao.Add(Mensagem.Tramitacao.JuntarApensarProcessoNumeroObrigatorio);
			}
			else if (!ValidacoesGenericasBus.ValidarMaskNumeroBarraAno(numero))
			{
				Validacao.Add(Mensagem.Tramitacao.JuntarApensarProcessoNumeroInvalido);
			}

			if (Validacao.EhValido)
			{
				IProtocolo prot = _busProtocolo.ObterSimplificado(_busProtocolo.ExisteProtocolo(numero),true);

				if (prot == null || prot.Id <= 0 || !prot.IsProcesso)
				{
					Validacao.Add(Mensagem.Processo.Inexistente);
					return Validacao.EhValido;
				}

				VerificarApensarProcesso(prot, procPaiId);				
			}

			return Validacao.EhValido;			
		}

		public bool VerificarApensarProcesso(IProtocolo protocolo, int procPaiId)
		{
			if (!_validarProcesso.Numero(protocolo.Numero))
			{
				return Validacao.EhValido;
			}

			Processo procPai = _busProcesso.ObterSimplificado(procPaiId, true);

			if (protocolo.Id == procPai.Id)
			{
				Validacao.Add(Mensagem.Processo.NaoPodeApensarASiProprio);
				return Validacao.EhValido;
			}

			if (!_validarProcesso.EmPosse(procPai.Id.Value))
			{
				Validacao.Add(Mensagem.Processo.ProcessoNaoPodeApensarPoisNaoPossuiPosse);
				return Validacao.EhValido;
			}

			// valida se o processo já está apensado
			string apensadoEmProcessoNumero = _busProtocolo.ObterNumeroProcessoPai(protocolo.Id);
			if (!String.IsNullOrEmpty(apensadoEmProcessoNumero) && apensadoEmProcessoNumero != procPai.Numero)
			{
				Validacao.Add(Mensagem.Processo.ProcessoNaoPodeApensarPoisEstaApensado(apensadoEmProcessoNumero));
				return Validacao.EhValido;
			}

			if (String.IsNullOrEmpty(apensadoEmProcessoNumero) && !_validarProcesso.EmPosse(protocolo.Id.Value))
			{
				Validacao.Add(Mensagem.Processo.ProcessoNaoPodeSerApensadoPoisNaoPossuiPosse);
				return Validacao.EhValido;
			}

			// Valida se o processo filho já é pai de algo (documento, processo)
			Processo procPaiFilhos = _busProtocolo.ObterProcessosDocumentos(protocolo.Id.Value) as Processo;
			if (procPaiFilhos.Processos.Count > 0 || procPaiFilhos.Documentos.Count > 0)
			{
				Validacao.Add(Mensagem.Processo.ProcessoNaoPodeApensarPoisTemFilhos);
				return Validacao.EhValido;
			}

			if (String.IsNullOrEmpty(apensadoEmProcessoNumero) && protocolo.SetorId != procPai.SetorId)
			{
				Validacao.Add(Mensagem.Processo.ProcessoNaoPodeApensarPoisNaoEstaNoMesmoSetor);
				return Validacao.EhValido;
			}

			return Validacao.EhValido;
		}

		public string VerificarProcessoApensadoNumero(string numero)
		{
			return _validar.Apensado(numero);
		}

		public string VerificarDocumentoJuntadoNumero(string numero)
		{
			return _validar.Juntado(numero);
		}

		#endregion
	}
}