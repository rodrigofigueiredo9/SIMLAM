using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProcesso.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data;
using Tecnomapas.EtramiteX.Interno.Model.Security;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business
{
	public class TituloSituacaoValidar
	{
		#region Propriedades

		TituloDa _da = new TituloDa();
		ProtocoloDa _protocoloDa = new ProtocoloDa();
		TituloValidar _validarTitulo = new TituloValidar();
		ProcessoValidar _processoValidar = new ProcessoValidar();
		DocumentoValidar _documentoValidar = new DocumentoValidar();
		PermissaoValidar _permissaoValdiar = new PermissaoValidar();
		GerenciadorConfiguracao<ConfiguracaoTituloModelo> _configModelo = new GerenciadorConfiguracao<ConfiguracaoTituloModelo>(new ConfiguracaoTituloModelo());

		public List<int> LstCadastroAmbientalRuralTituloCodigo
		{
			get { return _configModelo.Obter<List<int>>(ConfiguracaoTituloModelo.KeyCadastroAmbientalRuralTituloCodigo); }
		}


		#endregion

		public bool AlterarSituacaoAbrir(Titulo titulo)
		{
#if DEBUG
			return true;
#endif

			#region Validacao de Posse
			if (titulo.Protocolo.Id > 0 && !_protocoloDa.EmPosse(titulo.Protocolo.Id.Value))
			{
				Validacao.Add((titulo.Protocolo.IsProcesso) ? Mensagem.TituloAlterarSituacao.ProcessoPosseAltSituacao : Mensagem.TituloAlterarSituacao.DocumentoPosseAltSituacao);
			}
			#endregion

			return Validacao.EhValido;
		}

		public bool AlterarSituacao(Titulo titulo, int acao, bool gerouPdf = true)
		{
			Titulo tituloAux = _da.ObterSimplificado(titulo.Id);

			if (tituloAux == null)
			{
				return false;
			}

			#region ValidarPermissao

			List<ePermissao> acaoPermissao = new List<ePermissao>();
			switch (titulo.Situacao.Id)
			{
				#region Cadastrado

				case 1:
					acaoPermissao.Add(ePermissao.TituloCriar);
					acaoPermissao.Add(ePermissao.TituloCancelarEmissao);
					break;

				#endregion

				#region Emitido para assinatura

				case 2:
					acaoPermissao.Add(ePermissao.TituloEmitir);
					break;

				#endregion

				#region Concluído

				case 3:
					acaoPermissao.Add(ePermissao.TituloEmitir);
					acaoPermissao.Add(ePermissao.TituloAssinar);
					break;

				#endregion

				#region Assinado

				case 4:
					acaoPermissao.Add(ePermissao.TituloAssinar);
					break;

				#endregion

				#region Cancelado

				case 5:
					acaoPermissao.Add(ePermissao.TituloEncerrar);
					acaoPermissao.Add(ePermissao.DocumentoEncerrarOficioPendencia);
					break;

				#endregion

				#region Suspenso

				case 11:
					acaoPermissao.Add(ePermissao.TituloEncerrar);
					acaoPermissao.Add(ePermissao.DocumentoEncerrarOficioPendencia);
					break;

				#endregion

				#region Prorrogado

				case 6:
					acaoPermissao.Add(ePermissao.TituloProrrogar);
					break;

					#endregion
			}

			if (!_permissaoValdiar.ValidarAny(acaoPermissao.ToArray()))
			{
				return false;
			}

			#endregion

			//Validar Titulo
			if ((eAlterarSituacaoAcao)acao == eAlterarSituacaoAcao.EmitirParaAssinatura)
			{
				_validarTitulo.Titulo(titulo);

				if (EspecificiadadeBusFactory.Possui(titulo.Modelo.Codigo.Value))
				{
					IEspecificidadeBus busEsp = EspecificiadadeBusFactory.Criar(titulo.Modelo.Codigo.Value);
					titulo.Especificidade = busEsp.Obter(titulo.Id) as Especificidade;
					titulo.Especificidade = titulo.ToEspecificidade();
					busEsp.Validar.Emitir(titulo.Especificidade);
				}
			}
			else if (
				(eAlterarSituacaoAcao)acao == eAlterarSituacaoAcao.Assinar ||
				(eAlterarSituacaoAcao)acao == eAlterarSituacaoAcao.Entregar ||
				(eAlterarSituacaoAcao)acao == eAlterarSituacaoAcao.Prorrogar ||
				(eAlterarSituacaoAcao)acao == eAlterarSituacaoAcao.Concluir)
			{
				if (titulo.Atividades != null)
				{
					foreach (var item in titulo.Atividades)
					{
						if (!item.Ativada)
						{
							Validacao.Add(Mensagem.AtividadeEspecificidade.AtividadeDesativada(item.NomeAtividade));
						}
					}
				}
			}

			if (!Validacao.EhValido)
			{
				return false;
			}

			switch (titulo.Situacao.Id)
			{
				#region Cadastrado

				case 1:
					if (tituloAux.Situacao.Id != 2)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.SituacaoInvalida("Cadastrado", "Emitido para assinatura"));
					}
					break;

				#endregion

				#region Emitido para assinatura

				case 2:
					if (!gerouPdf)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.GerarPdfObrigatorio);
					}

					if (tituloAux.Situacao.Id != 1)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.SituacaoInvalida("Emitido para assinatura", "Cadastrado"));
					}

					if (ValidarDatas(titulo.DataEmissao, "DataEmissao", "emissão"))
					{
						if (!titulo.Modelo.Regra(eRegra.NumeracaoAutomatica))
						{
							if (titulo.Numero.Ano != titulo.DataEmissao.Data.Value.Year)
							{
								Validacao.Add(Mensagem.TituloAlterarSituacao.NumeroAnoEmissaoAno);
							}
						}
					}

					if (titulo.Modelo.Regra(eRegra.Prazo) && titulo.Prazo.GetValueOrDefault() <= 0)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.PrazoObrigatorio);
					}

					if (titulo.Modelo.Regra(eRegra.Prazo) && (titulo.Prazo.GetValueOrDefault() + DateTime.Now.Year) > DateTime.MaxValue.Year)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.PrazoInvalido);
					}

					break;

				#endregion

				#region Concluído

				case 3:
					if (tituloAux.Situacao.Id == 1 && !gerouPdf)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.GerarPdfObrigatorio);
					}

					if (tituloAux.Situacao.Id != 1 && tituloAux.Situacao.Id != 2 && tituloAux.Situacao.Id != 4 && tituloAux.Situacao.Id != 11)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.SituacaoInvalida("Concluído", "Cadastrado, Suspenso, Emitido para assinatura ou Assinado"));
					}

					if (tituloAux.Situacao.Id == 1 || tituloAux.Situacao.Id == 11)
					{
						if (ValidarDatas(titulo.DataEmissao, "DataEmissao", "emissão"))
						{
							if (Validacao.EhValido && !titulo.Modelo.Regra(eRegra.NumeracaoAutomatica))
							{
								if (titulo.Numero.Ano != titulo.DataEmissao.Data.Value.Year)
								{
									Validacao.Add(Mensagem.TituloAlterarSituacao.NumeroAnoEmissaoAno);
								}
							}
						}
					}

					if (titulo.Modelo.Regra(eRegra.Prazo) && titulo.Prazo.GetValueOrDefault() <= 0)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.PrazoObrigatorio);
					}

					if (titulo.Modelo.Regra(eRegra.Prazo) && (titulo.Prazo.GetValueOrDefault() + DateTime.Now.Year) > DateTime.MaxValue.Year)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.PrazoInvalido);
					}

					if (titulo.Modelo.Codigo == (int)eTituloModeloCodigo.AutorizacaoExploracaoFlorestal)
					{
						var associados = _da.ObterAssociados(titulo.Id);
						if (associados.Exists(x => x.DataVencimento.Data < titulo.DataEmissao.Data.Value.AddDays(titulo.Prazo.Value)))
							Validacao.Add(Mensagem.TituloAlterarSituacao.PrazoSuperiorAoLaudo);
					}

					#region [ Cadastro Ambiental Rural ]
					if (LstCadastroAmbientalRuralTituloCodigo.Any(x => x == titulo.Modelo.Codigo))
					{
						var busCARSolicitacao = new CARSolicitacaoBus();
						if (!busCARSolicitacao.VerificarSeEmpreendimentoPossuiSolicitacaoValidaEEnviada(titulo.EmpreendimentoId.GetValueOrDefault()))
						{
							Validacao.Add(Mensagem.TituloAlterarSituacao.TituloNaoPossuiSolicitacaoDeInscricao);
						}

					}
					#endregion

					break;

				#endregion

				#region Assinado

				case 4:
					if (tituloAux.Situacao.Id != 2)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.SituacaoInvalida("Assinado", "Emitido para assinatura"));
					}

					if (ValidarDatas(titulo.DataAssinatura, "DataAssinatura", "assinatura"))
					{
						if (titulo.DataAssinatura.Data < tituloAux.DataEmissao.Data)
						{
							Validacao.Add(Mensagem.TituloAlterarSituacao.DataDeveSerSuperior("DataAssinatura", "assinatura", "emissão"));
						}
					}
					break;

				#endregion

				#region Encerrado

				case 5:
					if (tituloAux.Situacao.Id != 3 && tituloAux.Situacao.Id != 6)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.SituacaoInvalida("Encerrado", "Concluído ou Prorrogado"));
					}

					if (ValidarDatas(titulo.DataEncerramento, "DataEncerramento", "encerramento"))
					{
						if (titulo.Modelo.Regra(eRegra.Prazo))
						{
							switch (Convert.ToInt32(titulo.Modelo.Resposta(eRegra.Prazo, eResposta.InicioPrazo).Valor))
							{
								case 1:
									if (titulo.DataEncerramento.Data < tituloAux.DataEmissao.Data)
										Validacao.Add(Mensagem.TituloAlterarSituacao.DataDeveSerSuperior("DataEncerramento", "encerramento", "emissão"));
									break;

								case 2:
									if (titulo.DataEncerramento.Data < tituloAux.DataAssinatura.Data)
										Validacao.Add(Mensagem.TituloAlterarSituacao.DataDeveSerSuperior("DataEncerramento", "encerramento", "assinatura"));
									break;

								case 3:
									if (titulo.DataEncerramento.Data < tituloAux.DataEntrega.Data)
										Validacao.Add(Mensagem.TituloAlterarSituacao.DataDeveSerSuperior("DataEncerramento", "encerramento", "entrega"));
									break;
							}

						}
					}

					if (titulo.MotivoEncerramentoId.GetValueOrDefault() <= 0)
						Validacao.Add(Mensagem.TituloAlterarSituacao.MotivoEncerramentoObrigatorio);

					if (titulo.Modelo.Codigo == (int)eTituloModeloCodigo.LaudoVistoriaFlorestal)
					{
						if (_da.ExistsTituloAssociadoNaoEncerrado(titulo.Id))
							Validacao.Add(Mensagem.TituloAlterarSituacao.TituloPossuiAssociadoNaoEncerrado("encerrado"));
					}

					break;

				#endregion

				#region Suspenso

				case 11:
					if (tituloAux.Situacao.Id != 3 && tituloAux.Situacao.Id != 6)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.SituacaoInvalida("Suspenso", "Concluído ou Prorrogado"));
					}

					if (ValidarDatas(titulo.DataEncerramento, "DataEncerramento", "suspensão"))
					{
						if (titulo.Modelo.Regra(eRegra.Prazo))
						{
							switch (Convert.ToInt32(titulo.Modelo.Resposta(eRegra.Prazo, eResposta.InicioPrazo).Valor))
							{
								case 1:
									if (titulo.DataEncerramento.Data < tituloAux.DataEmissao.Data)
									{
										Validacao.Add(Mensagem.TituloAlterarSituacao.DataDeveSerSuperior("DataEncerramento", "suspensão", "emissão"));
									}
									break;

								case 2:
									if (titulo.DataEncerramento.Data < tituloAux.DataAssinatura.Data)
									{
										Validacao.Add(Mensagem.TituloAlterarSituacao.DataDeveSerSuperior("DataEncerramento", "suspensão", "assinatura"));
									}
									break;

								case 3:
									if (titulo.DataEncerramento.Data < tituloAux.DataEntrega.Data)
									{
										Validacao.Add(Mensagem.TituloAlterarSituacao.DataDeveSerSuperior("DataEncerramento", "suspensão", "entrega"));
									}
									break;
							}

						}
					}

					if (titulo.Modelo.Codigo == (int)eTituloModeloCodigo.LaudoVistoriaFlorestal)
					{
						if (_da.ExistsTituloAssociadoNaoEncerrado(titulo.Id))
							Validacao.Add(Mensagem.TituloAlterarSituacao.TituloPossuiAssociadoNaoEncerrado("suspenso"));
					}

					break;

				#endregion

				#region Prorrogado

				case 6:
					if (tituloAux.Situacao.Id != 3 && tituloAux.Situacao.Id != 6)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.SituacaoInvalida("Prorrogado", "Concluído ou Prorrogado"));
					}

					if (titulo.DiasProrrogados.GetValueOrDefault() <= 0)
					{
						Validacao.Add(Mensagem.TituloAlterarSituacao.DiasProrrogadosObrigatorio);
					}
					break;

					#endregion
			}

			AtivarCondicionantes(titulo);

			return Validacao.EhValido;
		}

		private bool ValidarDatas(DateTecno data, string campo, string tipoData)
		{
			if (data.IsEmpty)
			{
				Validacao.Add(Mensagem.TituloAlterarSituacao.DataObrigatoria(campo, tipoData));
				return false;
			}
			else
			{
				if (!data.IsValido)
				{
					Validacao.Add(Mensagem.TituloAlterarSituacao.DataInvalida(campo, tipoData));
					return false;
				}
				else if (data.Data > DateTime.Today.AddDays(1))
				{
					Validacao.Add(Mensagem.TituloAlterarSituacao.DataIgualMenorAtual(campo, tipoData));
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		internal bool AtivarCondicionantes(Titulo titulo)
		{
			if (titulo.Situacao.Id != 3)
			{
				return true;
			}

			if (titulo.Condicionantes.Count > 0 && !titulo.Modelo.Regra(eRegra.Condicionantes))
			{
				Validacao.Add(Mensagem.TituloAlterarSituacao.CondicionantesRemover);
			}

			return Validacao.EhValido;
		}

		public void ValidarParaConcluir(Titulo titulo)
		{
			bool possuiPrazo = titulo.Modelo.Regra(eRegra.Prazo);

			if ((!possuiPrazo && (new int[] { 2, 4 }).Contains(titulo.Situacao.Id)))
			{
				Validacao.Add(Mensagem.TituloAlterarSituacao.TituloEmitidoAssinadoSemPrazoConcluir);
			}

			if (possuiPrazo && titulo.Modelo.Resposta(eRegra.Prazo, eResposta.InicioPrazo).Valor.ToString() == "1" && titulo.Situacao.Id == 2)// emitido para assinatura
			{
				Validacao.Add(Mensagem.TituloAlterarSituacao.TituloEmitidoConcluir);
			}

			if (possuiPrazo && (new string[] { "1", "2" }).Contains(titulo.Modelo.Resposta(eRegra.Prazo, eResposta.InicioPrazo).Valor.ToString()) && titulo.Situacao.Id == 4)// assinado
			{
				Validacao.Add(Mensagem.TituloAlterarSituacao.TituloAssinadoConcluir);
			}
		}
	}
}