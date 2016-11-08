using System;
using System.Linq;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business
{
	public class DominialidadeValidar
	{
		DominialidadeDa _da = new DominialidadeDa();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		ProjetoGeograficoValidar _projetoGeoValidar = new ProjetoGeograficoValidar();

		internal bool Salvar(Dominialidade dominialidade)
		{
			if (!_caracterizacaoValidar.Basicas(dominialidade.EmpreendimentoId))
			{
				return false;
			}

			if (dominialidade.Id <= 0 && (_da.ObterPorEmpreendimento(dominialidade.EmpreendimentoId, true) ?? new Dominialidade()).Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!_caracterizacaoValidar.Dependencias(dominialidade.EmpreendimentoId, (int)eCaracterizacao.Dominialidade))
			{
				return false;
			}

			if (Dominios(dominialidade.Dominios))
			{
				foreach (Dominio dominio in dominialidade.Dominios)
				{
					DominioSalvar(dominio);

					if (dominio.ReservasLegais != null && dominio.ReservasLegais.Count > 0)
					{
						foreach (ReservaLegal reserva in dominio.ReservasLegais)
						{
							ReservaLegalSalvar(reserva);
						}
					}
				}
			}

			string prefixo = "Dominialidade";

			if (!dominialidade.PossuiAreaExcedenteMatricula.HasValue)
			{
				Validacao.Add(Mensagem.Dominialidade.PossuiAreaExcedenteMatriculaObrigatorio);
			}

			if (String.IsNullOrWhiteSpace(dominialidade.ConfrontacaoNorte))
			{
				Validacao.Add(Mensagem.Dominialidade.ConfrontacaoNorteObrigatorio(prefixo));
			}

			if (String.IsNullOrWhiteSpace(dominialidade.ConfrontacaoSul))
			{
				Validacao.Add(Mensagem.Dominialidade.ConfrontacaoSulObrigatorio(prefixo));
			}

			if (String.IsNullOrWhiteSpace(dominialidade.ConfrontacaoLeste))
			{
				Validacao.Add(Mensagem.Dominialidade.ConfrontacaoLesteObrigatorio(prefixo));
			}

			if (String.IsNullOrWhiteSpace(dominialidade.ConfrontacaoOeste))
			{
				Validacao.Add(Mensagem.Dominialidade.ConfrontacaoOesteObrigatorio(prefixo));
			}

			List<string> auxiliar = _da.VerificarExcluirDominios(dominialidade);
			if (auxiliar != null && auxiliar.Count > 0)
			{
				foreach (var item in auxiliar)
				{
					Validacao.Add(Mensagem.Dominialidade.DominioAssossiadoReserva(item));
				}
			}

			return Validacao.EhValido;
		}

		public bool DominioSalvar(Dominio dominio)
		{
			switch (dominio.Tipo)
			{
				case eDominioTipo.Matricula:
					#region Matricula

					if (String.IsNullOrWhiteSpace(dominio.Matricula))
					{
						Validacao.Add(Mensagem.Dominialidade.DominioMatriculaObrigatorio);
					}

					if (String.IsNullOrWhiteSpace(dominio.Folha))
					{
						Validacao.Add(Mensagem.Dominialidade.DominioFolhaObrigatorio);
					}

					if (String.IsNullOrWhiteSpace(dominio.Livro))
					{
						Validacao.Add(Mensagem.Dominialidade.DominioLivroObrigatorio);
					}

					if (String.IsNullOrWhiteSpace(dominio.Cartorio))
					{
						Validacao.Add(Mensagem.Dominialidade.DominioCartorioObrigatorio);
					}
					break;

					#endregion

				case eDominioTipo.Posse:
					#region Posse

					if (dominio.ComprovacaoId <= 0)
					{
						Validacao.Add(Mensagem.Dominialidade.DominioComprovacaoObrigatorio);
					}

					if (dominio.ComprovacaoId != (int)eDominioComprovacao.PosseiroPrimitivo)
					{
						if (dominio.ComprovacaoId != (int)eDominioComprovacao.Recibo &&
							dominio.ComprovacaoId != (int)eDominioComprovacao.CertidaoPrefeitura &&
							dominio.ComprovacaoId != (int)eDominioComprovacao.ContratoCompraVenda &&
							dominio.ComprovacaoId != (int)eDominioComprovacao.Declaracao &&
							dominio.ComprovacaoId != (int)eDominioComprovacao.Outros &&
							dominio.ComprovacaoId != (int)eDominioComprovacao.CertificadoCadastroImovelRuralCCIR)
						{
							if (String.IsNullOrWhiteSpace(dominio.DescricaoComprovacao))
							{
								Validacao.Add(Mensagem.Dominialidade.RegistroObrigatorio);
							}
						}
					}
					break;

					#endregion
			}

			if (dominio.Tipo == eDominioTipo.Matricula || dominio.ComprovacaoId != (int)eDominioComprovacao.PosseiroPrimitivo)
			{
				if (!String.IsNullOrWhiteSpace(dominio.AreaDocumentoTexto))
				{
					if (!ValidacoesGenericasBus.ValidarDecimal(DecimalEtx.ClearMask(dominio.AreaDocumentoTexto), 13, 2))
					{
						Validacao.Add(Mensagem.Dominialidade.AreaInvalida("DominioAreaDocumento", "Área " + ((dominio.Tipo == eDominioTipo.Matricula) ? "Matrícula" : "Posse") + " Documento"));
					}
					else if (DecimalEtx.ToDecimalMask(dominio.AreaDocumentoTexto).GetValueOrDefault() <= 0)
					{
						Validacao.Add(Mensagem.Dominialidade.AreaMaiorZero("DominioAreaDocumento", "Área " + ((dominio.Tipo == eDominioTipo.Matricula) ? "Matrícula" : "Posse") + " Documento"));
					}
				}
				else
				{
					Validacao.Add(Mensagem.Dominialidade.AreaObrigatoria("DominioAreaDocumento", "Área " + ((dominio.Tipo == eDominioTipo.Matricula) ? "Matrícula" : "Posse") + " Documento"));
				}
			}

			#region Empreendimento com localizacao Zona Rural

			if (!dominio.DataUltimaAtualizacao.IsEmpty)
			{
				ValidacoesGenericasBus.DataMensagem(dominio.DataUltimaAtualizacao, "DominioDataUltimaAtualizacao_DataTexto", "última atualização");
			}

			#endregion

			if (dominio.ReservasLegais == null || dominio.ReservasLegais.Count <= 0)
			{
				Validacao.Add(Mensagem.Dominialidade.ReservaLegalObrigatoria);
			}

			if (dominio.ReservasLegais.Exists(x => x.SituacaoId == (int)eReservaLegalSituacao.NaoInformada) &&
				dominio.ReservasLegais.Exists(x => x.SituacaoId == (int)eReservaLegalSituacao.Proposta || x.SituacaoId == (int)eReservaLegalSituacao.Registrada))
			{
				Validacao.Add(Mensagem.Dominialidade.ReservaLegalNaoInformadaDeclarada);
			}

			string prefixo = "Dominio";

			if (String.IsNullOrWhiteSpace(dominio.ConfrontacaoNorte))
			{
				Validacao.Add(Mensagem.Dominialidade.ConfrontacaoNorteObrigatorio(prefixo));
			}

			if (String.IsNullOrWhiteSpace(dominio.ConfrontacaoSul))
			{
				Validacao.Add(Mensagem.Dominialidade.ConfrontacaoSulObrigatorio(prefixo));
			}

			if (String.IsNullOrWhiteSpace(dominio.ConfrontacaoLeste))
			{
				Validacao.Add(Mensagem.Dominialidade.ConfrontacaoLesteObrigatorio(prefixo));
			}

			if (String.IsNullOrWhiteSpace(dominio.ConfrontacaoOeste))
			{
				Validacao.Add(Mensagem.Dominialidade.ConfrontacaoOesteObrigatorio(prefixo));
			}

			return Validacao.EhValido;
		}

		public bool ReservaLegalSalvar(ReservaLegal reserva)
		{
			if (reserva.SituacaoId <= 0)
			{
				Validacao.Add(Mensagem.Dominialidade.ReservaLegalSituacaoObrigatorio);
			}

			if (reserva.Id > 0 && reserva.CompensacaoTipo == eCompensacaoTipo.Cedente && reserva.Excluir)
			{
				Validacao.Add(Mensagem.Dominialidade.ReservaLegalCedenteEdicaoNaoPermitida);
			}

			if (reserva.SituacaoId == (int)eReservaLegalSituacao.NaoInformada)
			{
				return Validacao.EhValido;
			}

			if (reserva.CompensacaoTipo == eCompensacaoTipo.Cedente && (reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.NaoCaracterizada || reserva.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.EmUso))
			{
				Validacao.Add(Mensagem.Dominialidade.ReservaLegalCedentePreservadaOuEmRecuperacao);
			}

			if (reserva.SituacaoId == (int)eReservaLegalSituacao.Proposta)
			{
				switch (reserva.LocalizacaoId)
				{
					#region Receptora

					case (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora:
						if (reserva.CedentePossuiEmpreendimento < 0)
						{
							Validacao.Add(Mensagem.Dominialidade.ReservaLegalCedentePossuiCodigoEmpreendimentoObrigatorio);
						}

						if (reserva.CedentePossuiEmpreendimento == 1)
						{
							if (reserva.EmpreendimentoCompensacao.Id <= 0)
							{
								Validacao.Add(Mensagem.Dominialidade.ReservaLegalEmpreendimentoCedenteObrigatorio);
							}
							else
							{
								ValidarEmpreendimentoCedente(reserva);
								VerificarRLAssociadaEmOutroEmpreendimentoCedente(reserva.Id, reserva.EmpreendimentoCompensacao.Id, reserva.IdentificacaoARLCedente);
							}
						}

						break;

					#endregion

					#region Cedente

					case (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoCedente:

						if (reserva.EmpreendimentoCompensacao.Id <= 0)
						{
							Validacao.Add(Mensagem.Dominialidade.ReservaLegalEmpreendimentoReceptorObrigatorio);
						}
						else 
						{
							if (reserva.MatriculaId.GetValueOrDefault() <=0)
							{
								Validacao.Add(Mensagem.Dominialidade.ReservaLegalEmpreendimentoReceptorMatriculaObrigatorio);
							}
						}

						break;

					#endregion

					case (int)eReservaLegalLocalizacao.NestaMatricula:
					case (int)eReservaLegalLocalizacao.NestaPosse:

						break;

					default:
						Validacao.Add(Mensagem.Dominialidade.ReservaLegalLocalizacaoObrigatorio);
						break;
				}

			}

			if (reserva.SituacaoId == (int)eReservaLegalSituacao.Registrada)
			{
				switch (reserva.LocalizacaoId)
				{
					#region Receptora

					case (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora:

						if (reserva.CedentePossuiEmpreendimento < 0)
						{
							Validacao.Add(Mensagem.Dominialidade.ReservaLegalCedentePossuiCodigoEmpreendimentoObrigatorio);
						}

						ValidarCamposSituacaoRegistrada(reserva);

						if (reserva.CedentePossuiEmpreendimento == 0)
						{
							if (reserva.ARLCedida <= 0)
							{
								Validacao.Add(Mensagem.Dominialidade.ARLCedidaObrigatorio);
							}

							if (reserva.SituacaoVegetalId <= 0)
							{
								Validacao.Add(Mensagem.Dominialidade.ReservaLegalSituacaoVegetalObrigatorio);
							}

							if (reserva.SituacaoVegetalId != (int)eReservaLegalSituacaoVegetal.EmRecuperacao && reserva.SituacaoVegetalId != (int)eReservaLegalSituacaoVegetal.Preservada)
							{
								Validacao.Add(Mensagem.Dominialidade.ReservaLegalSituacaoVegetalInvalido);
							}
						}

						if (reserva.CedentePossuiEmpreendimento == 1)
						{
							if (reserva.EmpreendimentoCompensacao.Id <= 0)
							{
								Validacao.Add(Mensagem.Dominialidade.ReservaLegalEmpreendimentoCedenteObrigatorio);
							}

							if (!string.IsNullOrEmpty(reserva.MatriculaIdentificacao))
							{
								Validacao.Add(Mensagem.Dominialidade.ReservaLegalMatriculaIdentificacaoCedenteObrigatorio);
							}

							if (reserva.IdentificacaoARLCedente <= 0)
							{
								Validacao.Add(Mensagem.Dominialidade.ReservaLegalIdentificacaoARLObrigatoria);
							}
							else
							{
								ValidarEmpreendimentoCedente(reserva);
								VerificarRLAssociadaEmOutroEmpreendimentoCedente(reserva.Id, reserva.EmpreendimentoCompensacao.Id, reserva.IdentificacaoARLCedente);
							}

						}

						break;

					#endregion

					#region Cedente

					case (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoCedente: 

						ValidarCamposSituacaoRegistrada(reserva);

						if (reserva.ARLRecebida <= 0)
						{
							Validacao.Add(Mensagem.Dominialidade.ARLRecebidaObrigatorio);
						}

						break;

					#endregion

					#region Nesta Matrícula

					case (int)eReservaLegalLocalizacao.NestaMatricula:
					case (int)eReservaLegalLocalizacao.NestaPosse:
						
						ValidarCamposSituacaoRegistrada(reserva);

						break;

					#endregion

					default:
						Validacao.Add(Mensagem.Dominialidade.ReservaLegalLocalizacaoObrigatorio);
						break;
				}

			}

			return Validacao.EhValido;
		}

		private static void ValidarCamposSituacaoRegistrada(ReservaLegal reserva)
		{
			if (string.IsNullOrEmpty(reserva.MatriculaNumero))
			{
				Validacao.Add(Mensagem.Dominialidade.ReservaLegalNumeroMatriculaObrigatorio);
			}

			if (reserva.TipoCartorioId <= 0)
			{
				Validacao.Add(Mensagem.Dominialidade.ReservaLegalTipoCartorioObrigatorio);
			}

			if (string.IsNullOrEmpty(reserva.AverbacaoNumero))
			{
				Validacao.Add(Mensagem.Dominialidade.ReservaLegalNumeroAverbacaoObrigatorio);
			}
		}

		private void VerificarRLAssociadaEmOutroEmpreendimentoCedente(int reservaLegalId, int empreendimentoCedenteId, int IdentificacaoARL)
		{
			if (reservaLegalId <= 0 || empreendimentoCedenteId <= 0 || IdentificacaoARL <= 0)
			{
				return;
			}

			EmpreendimentoCaracterizacao empreendimentoExistente = _da.VerificarRLAssociadaEmOutroEmpreendimentoCedente(reservaLegalId, empreendimentoCedenteId, IdentificacaoARL);

			if (empreendimentoExistente != null)
			{
				Validacao.Add(Mensagem.Dominialidade.RLAssociadaEmOutroEmpreendimentoCedente(empreendimentoExistente.Codigo.GetValueOrDefault(), empreendimentoExistente.Denominador));
			}
		}

		private void ValidarEmpreendimentoCedente(ReservaLegal reserva)
		{

			if (reserva.MatriculaId.GetValueOrDefault() <= 0)
			{
				Validacao.Add(Mensagem.Dominialidade.ReservaLegalMatriculaCedenteObrigatorio);
			}

			if (reserva.IdentificacaoARLCedente <= 0)
			{
				Validacao.Add(Mensagem.Dominialidade.ReservaLegalIdentificacaoARLObrigatoria);
			}

			if (!_da.ValidarSituacaoVegetal(reserva.IdentificacaoARLCedente))
			{
				Validacao.Add(Mensagem.Dominialidade.ReservaLegalCedentePreservadaOuEmRecuperacao);
			}

			if (!Validacao.EhValido)
			{
				return;
			}

			Dominialidade dominialidadeCedente = _da.ObterPorEmpreendimento(reserva.EmpreendimentoCompensacao.Id);

			if (dominialidadeCedente.Id <= 0)
			{
				Validacao.Add(Mensagem.Dominialidade.ReservaLegalEmpreendimentoCedenteNaoPossuiDominialidade);
			}

			if (reserva.MatriculaId.GetValueOrDefault() > 0 && !dominialidadeCedente.Dominios.Exists(x => x.Id == reserva.MatriculaId.GetValueOrDefault()))
			{
				Validacao.Add(Mensagem.Dominialidade.MatriculaSelecionadaNaoEstaMaisVinculadaCedente);
			}

			if (!dominialidadeCedente.Dominios.SelectMany(x => x.ReservasLegais).ToList().Exists(x => x.Id == reserva.IdentificacaoARLCedente))
			{
				Validacao.Add(Mensagem.Dominialidade.RLSelecionadaNaoEstaMaisVinculadaCedente);
			}

		}

		public bool Dominios(List<Dominio> dominios)
		{
			if (dominios == null || dominios.Count <= 0)
			{
				Validacao.Add(Mensagem.Dominialidade.DominioMatriculaPosseObrigatorio);
			}

			return Validacao.EhValido;
		}

		public bool ProjetoFinalizado(int EmpreendimentoId)
		{
			//int projetoId = _da.
			ProjetoGeograficoBus proejetoBus = new ProjetoGeograficoBus();
			int projetoId = proejetoBus.ExisteProjetoGeografico(EmpreendimentoId, (int)eCaracterizacao.Dominialidade);
			return _projetoGeoValidar.EhFinalizado(projetoId);
		}

		internal bool CopiarDadosCredenciado(Dominialidade caracterizacao)
		{
			if (caracterizacao.CredenciadoID <= 0)
			{
				Validacao.Add(Mensagem.Dominialidade.CopiarCaractizacaoCadastrada);
			}

			return Validacao.EhValido;
		}

		internal bool Excluir(int empreendimento)
		{
			if (!_da.VerificarEmpreendimentoCompensacaoAssociado(empreendimento))
			{
				Validacao.Add(Mensagem.Dominialidade.NaoSeraPossivelExcluir);
			}

			return Validacao.EhValido;
		}
	}
}