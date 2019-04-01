using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class FiscalizacaoValidar
	{
		FiscalizacaoDa _da = new FiscalizacaoDa();
		ProjetoGeograficoDa _projetoGeoDa = new ProjetoGeograficoDa();
		InfracaoDa _infracaoDa = new InfracaoDa();
		LocalInfracaoDa _localInfracaoDa = new LocalInfracaoDa();
		LocalInfracaoValidar _localInfracao = new LocalInfracaoValidar();

		private static EtramitePrincipal User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal); }
		}

		public bool Salvar(Fiscalizacao fiscalizacao)
		{
			_localInfracao.Salvar(fiscalizacao.LocalInfracao);

			return Validacao.EhValido;
		}

		public bool Finalizar(int id)
		{
			List<string> lstCadastroVazio = _da.TemCadastroVazio(id);
			ProjetoGeografico projetoGeo = new ProjetoGeografico();
			Infracao infracao = new Infracao();
			LocalInfracao localInfracao = new LocalInfracao();

			localInfracao = _localInfracaoDa.Obter(id);
			
			FuncionarioBus funcBus = new FuncionarioBus();
			List<Setor> setoresFunc = funcBus.ObterSetoresFuncionario();

			if (!setoresFunc.Any(x => x.Id == localInfracao.SetorId ))
			{
				Validacao.Add(Mensagem.Fiscalizacao.SetorNaoPertenceFuncionario);
			}

			bool contemProjGeo = _da.PossuiProjetoGeo(id);
			if (!contemProjGeo && lstCadastroVazio.Exists(x => x.Contains("Projeto Geografico")))
				lstCadastroVazio.RemoveAll(x => x.Contains("Projeto Geografico"));

			if (lstCadastroVazio.Count > 0)
			{
				Validacao.Add(Mensagem.Fiscalizacao.CadastroObrigatorio(Mensagem.Concatenar(lstCadastroVazio)));
				return Validacao.EhValido;
			}

            if (contemProjGeo)
            {
                projetoGeo = _projetoGeoDa.ObterProjetoGeograficoPorFiscalizacao(id);
                projetoGeo.FiscalizacaoEasting = localInfracao.LonEastingToDecimal;
                projetoGeo.FiscalizacaoNorthing = localInfracao.LatNorthingToDecimal;

                if (!_projetoGeoDa.VerificarProjetoGeograficoProcessado(projetoGeo.Id))
                {
                    Validacao.Add(Mensagem.Fiscalizacao.ProjetoGeoProcessado);
                }
                else
                {
                    if (!projetoGeo.FiscalizacaoEstaDentroAreaAbrangencia)
                    {
                        Validacao.Add(Mensagem.ProjetoGeografico.EmpreendimentoForaAbrangencia);
                    }
                }
            }

			infracao = _infracaoDa.Obter(id);

			if (_infracaoDa.ConfigAlterada(infracao.ConfiguracaoId, infracao.ConfiguracaoTid))
			{
				Validacao.Add(Mensagem.InfracaoMsg.ConfigAlteradaConcluir);
			}

			if (_infracaoDa.PerguntaRespostaAlterada(infracao))
			{
				Validacao.Add(Mensagem.InfracaoMsg.ConfigAlteradaConcluir);
			}

			return Validacao.EhValido;
		}

		public bool Excluir(Fiscalizacao fiscalizacao)
		{
			if (fiscalizacao.Situacao != eFiscalizacaoSituacao.EmAndamento)
			{
				Validacao.Add(Mensagem.Fiscalizacao.ExcluirInvalido(fiscalizacao.SituacaoTexto));
				return Validacao.EhValido;
			}

			if (fiscalizacao.Autuante.Id != FiscalizacaoBus.User.EtramiteIdentity.FuncionarioId && 
				!(User as EtramitePrincipal).IsInRole(ePermissao.FiscalizacaoSemPosse.ToString()))
			{
				Validacao.Add(Mensagem.Fiscalizacao.AgenteFiscalInvalido("excluir"));
			}

			if (_da.ExisteTituloCertidaoDebido(fiscalizacao.Id))
			{
				Validacao.Add(Mensagem.Fiscalizacao.ExcluirCertidaoDebidoFiscalizacao);
			}

            if (_da.GeradoNumeroIUFDigital(fiscalizacao.Id))
            {
                Validacao.Add(Mensagem.Fiscalizacao.ExcluirIUFGerado);
            }

			return Validacao.EhValido;
		}

		public bool ValidarAssociar(int fiscalizacaoId)
		{
			Fiscalizacao fisc = _da.Obter(fiscalizacaoId);

			if (fisc.SituacaoId != (int)eFiscalizacaoSituacao.CadastroConcluido)
			{
				Validacao.Add(Mensagem.Fiscalizacao.SituacaoInvalida);
			}

			return Validacao.EhValido;
		}

		public bool ValidarDesassociar(int fiscalizacaoId)
		{
			Fiscalizacao fisc = _da.Obter(fiscalizacaoId);

			if (fisc.SituacaoId != (int)eFiscalizacaoSituacao.Protocolado &&
				fisc.SituacaoId != (int)eFiscalizacaoSituacao.CadastroConcluido &&
				fisc.SituacaoId != (int)eFiscalizacaoSituacao.CancelarConclusao)
			{
				Validacao.Add(Mensagem.Fiscalizacao.NaoPodeDesassociar(fisc.Id.ToString(), fisc.SituacaoTexto));
			}

			return Validacao.EhValido;
		}

		public bool PodeAlterarSituacao(Fiscalizacao fiscalizacao)
		{
			eFiscalizacaoSituacao situacaoAtual = (eFiscalizacaoSituacao)fiscalizacao.SituacaoId;

			if (situacaoAtual == eFiscalizacaoSituacao.EmAndamento)
			{
				Validacao.Add(Mensagem.Fiscalizacao.SituacaoEmAndamentoNaoPodeAlterar);
				return Validacao.EhValido;
			}

			ProtocoloDa protocoloDA = new ProtocoloDa();
			if (situacaoAtual == eFiscalizacaoSituacao.Protocolado ||
				situacaoAtual == eFiscalizacaoSituacao.MultaPaga ||
				situacaoAtual == eFiscalizacaoSituacao.ComDecisaoManutencaoMulta ||
				situacaoAtual == eFiscalizacaoSituacao.ComDecisaoMultaCancelada ||
				situacaoAtual == eFiscalizacaoSituacao.DefesaApresentada ||
				situacaoAtual == eFiscalizacaoSituacao.EmParcelamento ||
				situacaoAtual == eFiscalizacaoSituacao.EnviadoParaSEAMA ||
				situacaoAtual == eFiscalizacaoSituacao.InscritoEmDividaAtiva ||
				situacaoAtual == eFiscalizacaoSituacao.InscritoNoCADIN ||
				situacaoAtual == eFiscalizacaoSituacao.ParceladoPagamentoAtrasado ||
				situacaoAtual == eFiscalizacaoSituacao.ParceladopagamentoEmDia ||
				situacaoAtual == eFiscalizacaoSituacao.RecursoApresentado)
			{
				if (!protocoloDA.EmPosse(fiscalizacao.ProtocoloId))
				{
					Validacao.Add(Mensagem.Fiscalizacao.PosseProcessoNecessaria);
					return Validacao.EhValido;
				}
			}

			if ((fiscalizacao.Autuante.Id != FiscalizacaoBus.User.EtramiteIdentity.FuncionarioId && fiscalizacao.SituacaoId < (int)eFiscalizacaoSituacao.Protocolado) &&
				!(User as EtramitePrincipal).IsInRole(ePermissao.FiscalizacaoSemPosse.ToString()))
			{
				Validacao.Add(Mensagem.Fiscalizacao.AgenteFiscalInvalido("alterar"));
			}

			return Validacao.EhValido;
		}

		public bool AlterarSituacao(Fiscalizacao fiscalizacao) 
		{
			Fiscalizacao aux = _da.Obter(fiscalizacao.Id);
			if ((aux.Autuante.Id != FiscalizacaoBus.User.EtramiteIdentity.FuncionarioId && aux.SituacaoId < (int)eFiscalizacaoSituacao.Protocolado) &&
				!(User as EtramitePrincipal).IsInRole(ePermissao.FiscalizacaoSemPosse.ToString()))
			{
				Validacao.Add(Mensagem.Fiscalizacao.AgenteFiscalInvalido("alterar"));
			}

			if (aux.SituacaoId != fiscalizacao.SituacaoAtualTipo)
			{
				Validacao.Add(Mensagem.Fiscalizacao.SituacaoJaAlterada);
				return Validacao.EhValido;
			}

			if (fiscalizacao.SituacaoNovaTipo <= 0) 
			{
				Validacao.Add(Mensagem.Fiscalizacao.SituacaoNovaObrigatorio);
			}

			if (String.IsNullOrWhiteSpace(fiscalizacao.SituacaoNovaMotivoTexto))
			{
				Validacao.Add(Mensagem.Fiscalizacao.SituacaoMotivoObrigatorio);
			}

			ValidacoesGenericasBus.DataMensagem(fiscalizacao.SituacaoNovaData, "Fiscalizacao_SituacaoNovaData_DataTexto", "nova situação");

			return Validacao.EhValido;
		}

		public bool PossuiAI_TED_TAD(int fiscalizacaoId)
		{
			return _da.PossuiAI_TED_TAD(fiscalizacaoId);
		}
	}
}