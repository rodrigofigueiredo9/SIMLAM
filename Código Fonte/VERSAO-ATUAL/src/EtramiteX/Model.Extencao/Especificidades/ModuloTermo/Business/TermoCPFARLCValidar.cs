using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Business
{
	public class TermoCPFARLCValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade especificidade)
		{
			TermoCPFARLC esp = especificidade as TermoCPFARLC;
			TermoCPFARLCDa termoCPFARLCDa = new TermoCPFARLCDa();
			CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			CaracterizacaoValidar caracterizacaoValidar = new CaracterizacaoValidar();
			DominialidadeDa dominialidadeDa = new DominialidadeDa();
			EspecificidadeDa especificidadeDa = new EspecificidadeDa();
			GerenciadorConfiguracao<ConfiguracaoCaracterizacao> caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
			string caracterizacaoTipo = caracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes).Single(x => x.Id == (int)eCaracterizacao.Dominialidade).Texto;
			List<PessoaLst> responsaveis = new List<PessoaLst>();

			RequerimentoAtividade(esp, jaAssociado: false);

			#region Básicas

			if (esp.CedenteDominioID <= 0)
			{
				Validacao.Add(Mensagem.TermoCPFARLC.CedenteDominioObrigatorio);
			}

			if (esp.CedenteARLCompensacao == null || esp.CedenteARLCompensacao.Count <= 0)
			{
				Validacao.Add(Mensagem.TermoCPFARLC.CedenteARLCompensacaoObrigatoria);
			}
			else
			{
				if (esp.CedenteARLCompensacao.Any(x => esp.CedenteARLCompensacao.Count(y => y.Id == x.Id && y.Id > 0) > 1))
				{
					Validacao.Add(Mensagem.TermoCPFARLC.CedenteARLCompensacaoDuplicada);
				}
			}

			if (esp.CedenteResponsaveisEmpreendimento == null || esp.CedenteResponsaveisEmpreendimento.Count <= 0)
			{
				Validacao.Add(Mensagem.TermoCPFARLC.ResponsavelEmpreendimentoObrigatorio("Cedente", "cedente"));
			}
			else
			{
				if (esp.CedenteResponsaveisEmpreendimento.Any(x => esp.CedenteResponsaveisEmpreendimento.Count(y => y.Id == x.Id && y.Id > 0) > 1))
				{
					Validacao.Add(Mensagem.TermoCPFARLC.ResponsavelEmpreendimentoDuplicado("Cedente", "cedente"));
				}
			}

			if (esp.ReceptorEmpreendimentoID <= 0)
			{
				Validacao.Add(Mensagem.TermoCPFARLC.ReceptorEmpreendimentoObrigatorio);
			}

			if (esp.ReceptorDominioID <= 0)
			{
				Validacao.Add(Mensagem.TermoCPFARLC.ReceptorDominioObrigatorio);
			}

			if (esp.ReceptorResponsaveisEmpreendimento == null || esp.ReceptorResponsaveisEmpreendimento.Count <= 0)
			{
				Validacao.Add(Mensagem.TermoCPFARLC.ResponsavelEmpreendimentoObrigatorio("Receptor", "receptor"));
			}
			else
			{
				if (esp.ReceptorResponsaveisEmpreendimento.Any(x => esp.ReceptorResponsaveisEmpreendimento.Count(y => y.Id == x.Id && y.Id > 0) > 1))
				{
					Validacao.Add(Mensagem.TermoCPFARLC.ResponsavelEmpreendimentoDuplicado("Receptor", "receptor"));
				}
			}

			#endregion Básicas

			if (esp.Atividades.First().Id != ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.ReservaLegal))
			{
				Validacao.Add(Mensagem.TermoCPFARLC.AtividadeInvalida(esp.Atividades[0].NomeAtividade));
			}

			if (!Validacao.EhValido)
			{
				return false;
			}

			#region Cedente

			Dominialidade dominialidadeCedente = dominialidadeDa.ObterPorEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(0));
			dominialidadeCedente.Dependencias = caracterizacaoBus.ObterDependencias(dominialidadeCedente.Id, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao);

			if (esp.CedenteARLCompensacao.Exists(x => x.SituacaoVegetalId.Equals((int)eReservaLegalSituacaoVegetal.NaoCaracterizada) ||
													 x.SituacaoVegetalId.Equals((int)eReservaLegalSituacaoVegetal.EmUso)))
			{

			}


			if (dominialidadeCedente == null || dominialidadeCedente.Id <= 0)
			{
				Validacao.Add(Mensagem.TermoCPFARLC.DominialidadeInexistente("cedente", caracterizacaoTipo));
			}
			else
			{
				string retorno = caracterizacaoValidar.DependenciasAlteradas(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), (int)eCaracterizacao.Dominialidade,
					eCaracterizacaoDependenciaTipo.Caracterizacao, dominialidadeCedente.Dependencias);

				if (!string.IsNullOrEmpty(retorno))
				{
					Validacao.Add(Mensagem.TermoCPFARLC.CaracterizacaoDeveEstarValida("cedente", caracterizacaoTipo));
				}
				else
				{
					List<ReservaLegal> reservas = dominialidadeCedente.Dominios.SelectMany(x => x.ReservasLegais).Where(x => esp.CedenteARLCompensacao.Select(y => y.Id).Any(y => y == x.Id)).ToList();

					if (reservas.Any(x => !x.Compensada))
					{
						Validacao.Add(Mensagem.TermoCPFARLC.ARLCedenteTipoInvalida);
					}

					if (reservas.Any(x => x.SituacaoId != (int)eReservaLegalSituacao.Proposta))
					{
						Validacao.Add(Mensagem.TermoCPFARLC.ARLCedenteSituacaoInvalida);
					}

					if (reservas.Any(x => x.SituacaoVegetalId != (int)eReservaLegalSituacaoVegetal.Preservada && x.SituacaoVegetalId != (int)eReservaLegalSituacaoVegetal.EmRecuperacao))
					{
						Validacao.Add(Mensagem.TermoCPFARLC.ARLCedenteSituacaoVegetalInvalida);
					}

					if (!dominialidadeCedente.Dominios.Any(x => x.Id.GetValueOrDefault() == esp.CedenteDominioID))
					{
						Validacao.Add(Mensagem.TermoCPFARLC.DominioDessassociado("cedente"));
					}

					esp.CedenteARLCompensacao.ForEach(reserva =>
					{
						ReservaLegal aux = reservas.SingleOrDefault(x => x.Id == reserva.Id);

						if (aux == null || aux.Id <= 0)
						{
							Validacao.Add(Mensagem.TermoCPFARLC.ReservaLegalDessassociadoCedente(reserva.Identificacao));
						}
						else
						{
							if (aux.MatriculaId != esp.ReceptorDominioID)
							{
								Validacao.Add(Mensagem.TermoCPFARLC.ReservaLegalDessassociadoReceptorDominio(reserva.Identificacao));
							}
						}
					});
				}
			}

			responsaveis = especificidadeDa.ObterEmpreendimentoResponsaveis(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(0));
			esp.CedenteResponsaveisEmpreendimento.ForEach(resp =>
			{
				PessoaLst aux = responsaveis.SingleOrDefault(x => x.Id == resp.Id);

				if (aux == null || aux.Id <= 0)
				{
					Validacao.Add(Mensagem.TermoCPFARLC.ResponsavelDessassociado("cedente", resp.NomeRazao));
				}
			});

			#endregion Cedente

			#region Receptor

			Dominialidade dominialidadeReceptor = dominialidadeDa.ObterPorEmpreendimento(esp.ReceptorEmpreendimentoID);
			dominialidadeReceptor.Dependencias = caracterizacaoBus.ObterDependencias(dominialidadeReceptor.Id, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao);

			if (dominialidadeReceptor == null || dominialidadeReceptor.Id <= 0)
			{
				Validacao.Add(Mensagem.TermoCPFARLC.DominialidadeInexistente("receptor", caracterizacaoTipo));
			}
			else
			{
				string retorno = caracterizacaoValidar.DependenciasAlteradas(esp.ReceptorEmpreendimentoID, (int)eCaracterizacao.Dominialidade,
					eCaracterizacaoDependenciaTipo.Caracterizacao, dominialidadeReceptor.Dependencias);

				if (!string.IsNullOrEmpty(retorno))
				{
					Validacao.Add(Mensagem.TermoCPFARLC.CaracterizacaoDeveEstarValida("receptor", caracterizacaoTipo));
				}
				else
				{
					if (!dominialidadeReceptor.Dominios.Any(x => x.Id.GetValueOrDefault() == esp.ReceptorDominioID))
					{
						Validacao.Add(Mensagem.TermoCPFARLC.DominioDessassociado("receptor"));
					}

					List<ReservaLegal> reservas = dominialidadeReceptor.Dominios.SelectMany(x => x.ReservasLegais).Where(x => esp.CedenteARLCompensacao.Select(y => y.Id).Any(y => y == x.IdentificacaoARLCedente)).ToList();

					esp.CedenteARLCompensacao.ForEach(reserva =>
					{
						if (!reservas.Any(x => x.IdentificacaoARLCedente == reserva.Id))
						{
							Validacao.Add(Mensagem.TermoCPFARLC.ReservaLegalDessassociadoReceptor(reserva.Identificacao));
						}
					});
				}
			}

			responsaveis = especificidadeDa.ObterEmpreendimentoResponsaveis(esp.ReceptorEmpreendimentoID);
			esp.ReceptorResponsaveisEmpreendimento.ForEach(resp =>
			{
				PessoaLst aux = responsaveis.SingleOrDefault(x => x.Id == resp.Id);

				if (aux == null || aux.Id <= 0)
				{
					Validacao.Add(Mensagem.TermoCPFARLC.ResponsavelDessassociado("receptor", resp.NomeRazao));
				}
			});

			#endregion Receptor

			List<TituloModeloLst> lista = termoCPFARLCDa.ObterTitulosCedenteReceptor(esp.CedenteDominioID, esp.ReceptorDominioID);
			lista.ForEach(titulo =>
			{
				if (titulo.Id != esp.Titulo.Id)
				{
					if (string.IsNullOrEmpty(titulo.Texto))
					{
						Validacao.Add(Mensagem.TermoCPFARLC.DominioPossuiTituloCadastrado(titulo.Situacao));
					}
					else
					{
						Validacao.Add(Mensagem.TermoCPFARLC.DominioPossuiTituloConcluido(titulo.Sigla, titulo.Texto, titulo.Situacao));
					}
				}
			});

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			return Salvar(especificidade);
		}
	}
}