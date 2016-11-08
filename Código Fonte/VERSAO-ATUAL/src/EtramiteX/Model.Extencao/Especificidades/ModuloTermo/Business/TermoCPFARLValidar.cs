using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Business
{
	public class TermoCPFARLValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade especificidade)
		{
			TermoCPFARLDa _da = new TermoCPFARLDa();
			EspecificidadeDa _daEspecificidade = new EspecificidadeDa();
			DominialidadeDa _daDominialidade = new DominialidadeDa();
			CaracterizacaoBus caracterizacaoBus = new CaracterizacaoBus();
			CaracterizacaoValidar caracterizacaoValidar = new CaracterizacaoValidar();
			List<Dependencia> dependencias = new List<Dependencia>();
			TermoCPFARL esp = especificidade as TermoCPFARL;
			List<Caracterizacao> caracterizacoes = caracterizacaoBus.ObterCaracterizacoesEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault());
			List<PessoaLst> destinatarios = _daEspecificidade.ObterInteressados(esp.ProtocoloReq.Id);
			List<ReservaLegal> reservas;
			int idCaracterizacao;
			int tipo;

			RequerimentoAtividade(esp);

			if (esp.Atividades[0].Id != ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.ReservaLegal))
			{
				Validacao.Add(Mensagem.TermoCPFARLMsg.AtividadeInvalida(esp.Atividades[0].NomeAtividade));
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
							Validacao.Add(Mensagem.TermoCPFARLMsg.DestinatarioNaoPermitido);

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
					Validacao.Add(Mensagem.TermoCPFARLMsg.CaracterizacaoDeveEstarValida(caracterizacoes.Single(x => x.Tipo == eCaracterizacao.Dominialidade).Nome));
				}
				else
				{
					reservas = new List<ReservaLegal>();

					Dominialidade dominialidade = _daDominialidade.ObterPorEmpreendimento(especificidade.Titulo.EmpreendimentoId.GetValueOrDefault(0));
					dominialidade.Dominios.ForEach(x => { x.ReservasLegais.ForEach(reserva => { reservas.Add(reserva); }); });

					if (reservas == null || reservas.Count == 0)
					{
						Validacao.Add(Mensagem.TermoCPFARLMsg.ArlInexistente);
					}
					else
					{
						if (reservas.Exists(x => x.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.NaoCaracterizada || x.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.EmUso))
						{
							Validacao.Add(Mensagem.TermoCPFARLMsg.ARLSituacaoVegetalInvalida);
						}

						if (reservas.Exists(x => x.SituacaoId != (int)eReservaLegalSituacao.Proposta))
						{
							Validacao.Add(Mensagem.TermoCPFARLMsg.DominioSituacaoInvalida);
						}

						if (reservas.Exists(x => x.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoMatriculaCedente ||
												 x.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoMatriculaReceptora ||
												 x.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoCedente ||
												 x.LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora))
						{
							Validacao.Add(Mensagem.TermoCPFARLMsg.LocalizacaoInvalida);
						}
					}
				}
			}
			else
			{
				Validacao.Add(Mensagem.TermoCPFARLMsg.DominialidadeInexistente);
			}

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			if (ExisteProcDocFilhoQueFoiDesassociado(especificidade.Titulo.Id))
			{
				Validacao.Add(Mensagem.Especificidade.ProtocoloReqFoiDesassociado);
			}

			return Salvar(especificidade);
		}
	}
}