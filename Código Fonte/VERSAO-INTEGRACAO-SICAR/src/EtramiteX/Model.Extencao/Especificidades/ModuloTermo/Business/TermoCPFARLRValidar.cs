using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Business
{
	public class TermoCPFARLRValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade especificidade)
		{
			TermoCPFARLRDa _da = new TermoCPFARLRDa();
			EspecificidadeDa _daEspecificidade = new EspecificidadeDa();
			DominialidadeDa _daDominialidade = new DominialidadeDa();
			CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			CaracterizacaoValidar caracterizacaoValidar = new CaracterizacaoValidar();
			List<Dependencia> dependencias = new List<Dependencia>();
			TermoCPFARLR esp = especificidade as TermoCPFARLR;
			List<Caracterizacao> caracterizacoes = caracterizacaoBus.ObterCaracterizacoesEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault());
			List<PessoaLst> destinatarios = _daEspecificidade.ObterInteressados(esp.ProtocoloReq.Id);
			List<ReservaLegal> reservas;
			int idCaracterizacao;
			int tipo;

			RequerimentoAtividade(esp);

			if (esp.Atividades[0].Id != ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.ReservaLegal))
			{
				Validacao.Add(Mensagem.TermoCPFARLR.AtividadeInvalida(esp.Atividades[0].NomeAtividade));
			}

			if (esp.Destinatarios.Count == 0)
			{
				Validacao.Add(Mensagem.Especificidade.DestinatarioObrigatorio("Termo_Destinatario"));
			}
			else
			{
				esp.Destinatarios.ForEach(x =>
				{
					if (destinatarios.SingleOrDefault(y => y.Id == x.Id) == null)
					{
						Validacao.Add(Mensagem.Especificidade.DestinatarioDesassociado("Termo_Destinatario", x.Nome));
					}
					else
					{
						tipo = _daEspecificidade.ObterDadosPessoa(x.Id).Tipo;
						if (tipo == 3 || tipo == 4)
						{
							Validacao.Add(Mensagem.TermoCPFARLR.DestinatarioNaoPermitido);
						}
					}
				});
			}

			idCaracterizacao = caracterizacaoBus.Existe(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), eCaracterizacao.Dominialidade);
			if (idCaracterizacao > 0)
			{
				dependencias = caracterizacaoBus.ObterDependencias(idCaracterizacao, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao);
				if (caracterizacaoValidar.DependenciasAlteradas(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(), (int)eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao, dependencias) != String.Empty)
				{
					Validacao.Add(Mensagem.TermoCPFARLR.CaracterizacaoDeveEstarValida(caracterizacoes.Single(x => x.Tipo == eCaracterizacao.Dominialidade).Nome));
				}
				else
				{
					reservas = new List<ReservaLegal>();

					Dominialidade dominialidade = _daDominialidade.ObterPorEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(0));

					int countArlDoc = 0;
					dominialidade.Dominios.ForEach(x =>
					{
						x.ReservasLegais.ForEach(reserva => { reservas.Add(reserva); });

						if (x.ARLDocumento.GetValueOrDefault() == 0)
						{
							countArlDoc++;
						}
					});

					if (dominialidade.Dominios.Count == countArlDoc)
					{
						Validacao.Add(Mensagem.TermoCPFARLR.ArlObrigatoria);
					}

					if (reservas == null || reservas.Count == 0)
					{
						Validacao.Add(Mensagem.TermoCPFARLR.ArlInvalida);
					}
					else
					{
						if (reservas.Exists(x => x.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.NaoCaracterizada || x.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.EmUso))
						{
							Validacao.Add(Mensagem.TermoCPFARLR.ArlInvalida);
						}

						if (reservas.All(x => x.SituacaoId == (int)eReservaLegalSituacao.NaoInformada))
						{
							Validacao.Add(Mensagem.TermoCPFARLR.DominioSituacaoNaoCaracterizada);
						}
						else
						{
							if (reservas.Exists(x => x.SituacaoId != (int)eReservaLegalSituacao.Registrada))
							{
								Validacao.Add(Mensagem.TermoCPFARLR.DominioSituacaoInvalida);
							}
						}

						if (reservas.Exists(x => x.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoCedente ||
												 x.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoMatriculaCedente ||
												 x.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora ||
												 x.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoMatriculaReceptora))
						{
							Validacao.Add(Mensagem.TermoCPFARLR.LocalizacaoInvalida);
						}
					}
				}
			}
			else
			{
				Validacao.Add(Mensagem.TermoCPFARLR.DominialidadeInexistente);
			}

			if (string.IsNullOrWhiteSpace(esp.NumeroAverbacao))
			{
				Validacao.Add(Mensagem.TermoCPFARLR.NumeroAverbacaoObrigatorio);
			}

			ValidacoesGenericasBus.DataMensagem(esp.DataTituloAnterior, "DataEmissao", "emissão");

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			return Salvar(especificidade);
		}
	}
}