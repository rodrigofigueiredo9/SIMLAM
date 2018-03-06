using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRequerimento.Business;
using CARSolicitacaoCredenciadoBus = Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business.CARSolicitacaoBus;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Business
{
    public class CARSolicitacaoValidar
    {
        #region Propriedades

        CARSolicitacaoDa _da = new CARSolicitacaoDa();
        CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
        CaracterizacaoBus _caracterizacaoBus = new CaracterizacaoBus();
        ProtocoloDa _protocoloDa = new ProtocoloDa();
        RequerimentoBus _busRequerimento = new RequerimentoBus();
        RequerimentoValidar _requerimentoValidar = new RequerimentoValidar();
        CARSolicitacaoCredenciadoBus _carSolicitacaoCredenciadoBus = new CARSolicitacaoCredenciadoBus();
        TituloBus _busTitulo = new TituloBus();

        #endregion

        public bool Salvar(CARSolicitacao entidade)
        {
            if (!_protocoloDa.EmPosse(entidade.Protocolo.Id.GetValueOrDefault()))
            {
                Validacao.Add(Mensagem.CARSolicitacao.ProtocoloPosse);
                return false;
            }

            if (entidade.SituacaoId <= 0)
            {
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoSituacaoObrigatoria);
            }

            if (entidade.Protocolo == null || entidade.Protocolo.Id.GetValueOrDefault(0) <= 0)
            {
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoProtocoloObrigatorio);
            }

            if (entidade.Requerimento.Id <= 0)
            {
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoRequerimentoObrigatorio);
            }

            _requerimentoValidar.Existe(entidade.Requerimento.Id);

            if (entidade.SituacaoId == (int)eCARSolicitacaoSituacao.EmCadastro)
            {
                if (entidade.Protocolo.Id.GetValueOrDefault() != entidade.ProtocoloSelecionado.Id.GetValueOrDefault())
                {
                    if (!_da.ExisteProtocoloAssociado(entidade.Protocolo.Id.GetValueOrDefault(), entidade.ProtocoloSelecionado.Id.GetValueOrDefault()))
                    {
                        Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoRequerimentoDesassociadoProtocolo(entidade.Requerimento.Numero.ToString()));
                    }
                }

                String numeroProtocoloPai = _da.ObterNumeroProtocoloPai(entidade.Protocolo.Id.GetValueOrDefault(0));
                if (!String.IsNullOrWhiteSpace(numeroProtocoloPai))
                {
                    Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoProtocoloApensadoEmOutroProcesso(numeroProtocoloPai));
                }
            }

            #region Atividade

            if (entidade.Atividade.Id <= 0)
            {
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAtividadeObrigatoria);
            }
            else
            {
                if (entidade.Atividade.Id != ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.CadastroAmbientalRural))
                {
                    Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAtividadeInvalida);
                }
            }

            #endregion Atividade

            #region Validar Dados do Titulo

            String tituloSituacao = _da.ObterSituacaoTituloCARExistente(entidade.Empreendimento.Id);

            if (!String.IsNullOrWhiteSpace(tituloSituacao))
            {
                Validacao.Add(Mensagem.CARSolicitacao.EmpreendimentoPossuiTitulo(tituloSituacao));
                return false;
            }

            #endregion Validar Dados do Titulo

            #region Empreendimento

            if (!entidade.Empreendimento.Codigo.HasValue)
            {
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoEmpreendimentoCodigoObrigatorio);
            }

            if (entidade.Empreendimento.Id <= 0)
            {
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoEmpreendimentoObrigatorio);
            }

            #endregion Empreendimento

            #region Validar Dados da Caracterizacao

            if (_da.EmpreendimentoProjetoGeograficoDominialidadeNaoFinalizado(entidade.Empreendimento.Id))
            {
                Validacao.Add(Mensagem.CARSolicitacao.EmpreendimentoProjetoGeograficoDominialidadeNaoFinalizado);
            }
            else
            {
                int idCaracterizacao = 0;
                List<Dependencia> dependencias;

                idCaracterizacao = _caracterizacaoBus.Existe(entidade.Empreendimento.Id, eCaracterizacao.Dominialidade);

                if (idCaracterizacao > 0)
                {
                    dependencias = _caracterizacaoBus.ObterDependencias(idCaracterizacao, eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao);
                    if (_caracterizacaoValidar.DependenciasAlteradas(entidade.Empreendimento.Id, (int)eCaracterizacao.Dominialidade, eCaracterizacaoDependenciaTipo.Caracterizacao, dependencias) != String.Empty)
                    {
                        Validacao.Add(Mensagem.CARSolicitacao.EmpreendimentoDominialidadeInvalida);
                    }
                }
                else
                {
                    Validacao.Add(Mensagem.CARSolicitacao.EmpreendimentoDominialidadeInexistente);
                }
            }

            #endregion Validar Dados da Caracterizacao

            if (entidade.Declarante.Id <= 0)
            {
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoDeclaranteObrigatorio);
            }

            return Validacao.EhValido;
        }

        public bool AcessarEditar(CARSolicitacao entidade)
        {
            if (entidade.Id <= 0)
            {
                Validacao.Add(Mensagem.Padrao.Inexistente);
                return false;
            }

            if (!_protocoloDa.EmPosse(entidade.Protocolo.Id.GetValueOrDefault()))
            {
                Validacao.Add(Mensagem.CARSolicitacao.ProtocoloPosse);
                return false;
            }

            String tituloSituacao = _da.ObterSituacaoTituloCARExistente(entidade.Empreendimento.Id);
            if (!String.IsNullOrWhiteSpace(tituloSituacao))
            {
                Validacao.Add(Mensagem.CARSolicitacao.EmpreendimentoPossuiTitulo(tituloSituacao));
                return false;
            }

            var situacaoArquivo = _da.BuscaSituacaoAtualArquivoSICAR(entidade.Id, (int)eCARSolicitacaoOrigem.Institucional);

            if (!((entidade.SituacaoId == (int)eCARSolicitacaoSituacao.EmCadastro && situacaoArquivo.Item1 == eStatusArquivoSICAR.Nulo)
                ||
                (entidade.SituacaoId == (int)eCARSolicitacaoSituacao.Pendente && situacaoArquivo.Item1 == eStatusArquivoSICAR.ArquivoReprovado)))
            {
                Validacao.Add(Mensagem.CARSolicitacao.EditarSolicitacaoTopico1(entidade.SituacaoTexto, situacaoArquivo.Item2));
                Validacao.Add(Mensagem.CARSolicitacao.EditarSolicitacaoTopico2);
                Validacao.Add(Mensagem.CARSolicitacao.EditarSolicitacaoTopico3);
            }

            return Validacao.EhValido;
        }

        public bool Excluir(CARSolicitacao entidade)
        {
            if (entidade.Id <= 0)
            {
                Validacao.Add(Mensagem.Padrao.Inexistente);
                return false;
            }

            if (!_protocoloDa.EmPosse(entidade.Protocolo.Id.GetValueOrDefault()))
            {
                Validacao.Add(Mensagem.CARSolicitacao.ProtocoloPosseExcluir(entidade.Protocolo.Numero));
                return false;
            }

            String tituloSituacao = _da.ObterSituacaoTituloCARExistente(entidade.Empreendimento.Id);
            if (!String.IsNullOrWhiteSpace(tituloSituacao))
            {
                Validacao.Add(Mensagem.CARSolicitacao.EmpreendimentoPossuiTitulo(tituloSituacao));
                return false;
            }

            var situacaoArquivo = _da.BuscaSituacaoAtualArquivoSICAR(entidade.Id, (int)eCARSolicitacaoOrigem.Institucional);

            if (!((entidade.SituacaoId == (int)eCARSolicitacaoSituacao.EmCadastro && situacaoArquivo.Item1 == eStatusArquivoSICAR.Nulo)
                ||
                (entidade.SituacaoId == (int)eCARSolicitacaoSituacao.Pendente && situacaoArquivo.Item1 == eStatusArquivoSICAR.ArquivoReprovado)))
            {
                Validacao.Add(Mensagem.CARSolicitacao.ExcluirSolicitacaoTopico1(entidade.SituacaoTexto, situacaoArquivo.Item2));
                Validacao.Add(Mensagem.CARSolicitacao.ExcluirSolicitacaoTopico2);
                Validacao.Add(Mensagem.CARSolicitacao.ExcluirSolicitacaoTopico3);
            }

            return Validacao.EhValido;
        }

        public bool AcessarAlterarSituacao(CARSolicitacao entidade)
        {
            if (entidade.Id <= 0)
            {
                Validacao.Add(Mensagem.Padrao.Inexistente);
                return false;
            }

            if (entidade.Protocolo.Id.GetValueOrDefault() > 0)
            {
                if (!_protocoloDa.EmPosse(entidade.Protocolo.Id.GetValueOrDefault()))
                {
                    Validacao.Add(Mensagem.CARSolicitacao.ProtocoloPosseAlterarSituacao);
                    return false;
                }
            }

            //String tituloSituacao = String.Empty;

            //if (_da.ExisteCredenciado(entidade.Id))
            //	tituloSituacao = _da.ObterSituacaoTituloCARExistenteCredenciado(entidade.Empreendimento.Id);
            //else
            //	tituloSituacao = _da.ObterSituacaoTituloCARExistente(entidade.Empreendimento.Id);


            //if (!String.IsNullOrWhiteSpace(tituloSituacao))
            //{
            //	Validacao.Add(Mensagem.CARSolicitacao.EmpreendimentoPossuiTitulo(tituloSituacao));
            //	return false;
            //}

            if (entidade.SituacaoId == (int)eCARSolicitacaoSituacao.EmCadastro)
                Validacao.Add(Mensagem.CARSolicitacao.AcessarAlterarSituacao(entidade.SituacaoTexto));

            if (entidade.SituacaoId == (int)eCARSolicitacaoSituacao.Invalido || entidade.SituacaoId == (int)eCARSolicitacaoSituacao.SubstituidoPeloTituloCAR || entidade.SituacaoId == (int)eCARSolicitacaoSituacao.Valido)
                Validacao.Add(Mensagem.CARSolicitacao.AcessarAlterarSituacao(entidade.SituacaoTexto));

            var origemSolicitacao = (int)(_da.ExisteCredenciado(entidade.Id) ? eCARSolicitacaoOrigem.Credenciado : eCARSolicitacaoOrigem.Institucional);

            eStatusArquivoSICAR situacaoArquivo = _da.BuscaSituacaoAtualArquivoSICAR(entidade.Id, origemSolicitacao).Item1;

            if (situacaoArquivo == eStatusArquivoSICAR.ArquivoEntregue)
                Validacao.Add(Mensagem.CARSolicitacao.AcessarAlterarSituacaoSolicitacaoEnviadaSICAR);

            if (situacaoArquivo != eStatusArquivoSICAR.Nulo && situacaoArquivo != eStatusArquivoSICAR.ArquivoReprovado)
                Validacao.Add(Mensagem.CARSolicitacao.AcessarAlterarSituacaoSolicitacaoEmProcessamentoSICAR);

            return Validacao.EhValido;
        }

        public bool AcessoEnviarReenviarArquivoSICAR(CARSolicitacao entidade, int origem)
        {
            if (entidade.Id <= 0)
            {
                Validacao.Add(Mensagem.Padrao.Inexistente);
                return false;
            }

            var situacaoArquivo = _da.BuscaSituacaoAtualArquivoSICAR(entidade.Id, origem);

            if (situacaoArquivo.Item1 == eStatusArquivoSICAR.ArquivoEntregue)
            {
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoJaEnviada);
                return false;
            }

            if (entidade.Protocolo.Id.GetValueOrDefault() > 0)
            {
                if (!_protocoloDa.EmPosse(entidade.Protocolo.Id.GetValueOrDefault()))
                {
                    Validacao.Add(Mensagem.CARSolicitacao.ProtocoloPosse);
                    return false;
                }
            }
            else
            {
                if (origem == (int)eCARSolicitacaoOrigem.Credenciado)
                {
                    Validacao.Add(Mensagem.CARSolicitacao.NaoPodeEnviarCredenciado);
                    return false;
                }
            }

            if (entidade.SituacaoId == (int)eCARSolicitacaoSituacao.Invalido || entidade.SituacaoId == (int)eCARSolicitacaoSituacao.Suspenso)
            {
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaEnviarSituacaoSICARInvalida(entidade.SituacaoTexto));
                return false;
            }

            if (situacaoArquivo.Item1 != eStatusArquivoSICAR.Nulo && situacaoArquivo.Item1 != eStatusArquivoSICAR.ArquivoReprovado)
            {
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaEnviarSituacaoArquivoSICARInvalida(situacaoArquivo.Item2));
            }

            return Validacao.EhValido;
        }

        public bool AlterarSituacao(CARSolicitacao entidade)
        {
            if (entidade.Id <= 0)
            {
                Validacao.Add(Mensagem.Padrao.Inexistente);
                return false;
            }

            if (entidade.Protocolo.Id.GetValueOrDefault() > 0 && !_protocoloDa.EmPosse(entidade.Protocolo.Id.GetValueOrDefault()))
            {
                Validacao.Add(Mensagem.CARSolicitacao.ProtocoloPosseAlterarSituacao);
                return false;
            }

            //if (entidade.SituacaoId == (int)eCARSolicitacaoSituacao.Valido)
            //{
            //	CARSolicitacao entidadeAtual = _da.Obter(entidade.Id, false);
            //
            //	if (!Salvar(entidadeAtual))
            //		return Validacao.EhValido;
            //}

            var origemSolicitacao = (int)(_da.ExisteCredenciado(entidade.Id) ? eCARSolicitacaoOrigem.Credenciado : eCARSolicitacaoOrigem.Institucional);

            var situacaoArquivo = _da.BuscaSituacaoAtualArquivoSICAR(entidade.Id, origemSolicitacao).Item1;

            if (entidade.SituacaoId <= 0)
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAlterarSituacaoNovaSituacaoObrigatoria);

            if (entidade.SituacaoAnteriorId == (int)eCARSolicitacaoSituacao.Pendente && situacaoArquivo == eStatusArquivoSICAR.ArquivoReprovado
                && entidade.SituacaoId != (int)eCARSolicitacaoSituacao.Invalido)
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAlterarSituacaoNovaSituacaoNaoPermitida);

            if (entidade.SituacaoAnteriorId == (int)eCARSolicitacaoSituacao.Valido && situacaoArquivo == eStatusArquivoSICAR.Nulo
                && (entidade.SituacaoId != (int)eCARSolicitacaoSituacao.Invalido))
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAlterarSituacaoNovaSituacaoNaoPermitida);

            if (entidade.SituacaoAnteriorId == (int)eCARSolicitacaoSituacao.Suspenso && situacaoArquivo == eStatusArquivoSICAR.Nulo
                && (entidade.SituacaoId != (int)eCARSolicitacaoSituacao.Valido && entidade.SituacaoId != (int)eCARSolicitacaoSituacao.Invalido))
                Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAlterarSituacaoNovaSituacaoNaoPermitida);

            if (entidade.SituacaoId != (int)eCARSolicitacaoSituacao.SubstituidoPeloTituloCAR)
            {
                if (situacaoArquivo != eStatusArquivoSICAR.Nulo && situacaoArquivo != eStatusArquivoSICAR.ArquivoReprovado)
                    Validacao.Add(Mensagem.CARSolicitacao.AcessarAlterarSituacaoSolicitacaoEnviadaSICAR);
            }

            return Validacao.EhValido;
        }

        public bool RetificacaoValidar(CARSolicitacao entidade)
        {
            string situacao = string.Empty;
            CARSolicitacao solicitacao = new CARSolicitacao();

            //Verificar se existe solicitação para o empreendimento
            solicitacao = _da.ObterPorEmpreendimentoCod(entidade.Empreendimento.Codigo ?? 0);
            if (solicitacao != null)
            {
                if (solicitacao.SituacaoId != 2 && solicitacao.SituacaoId != 5)
                {
                    if (solicitacao.SituacaoId == 1)
                    {
                        Validacao.Add(Mensagem.Retificacao.msgInst1());
                        return false;
                    }
                    else
                        if (solicitacao.SituacaoId == 6)
                        {
                            Validacao.Add(Mensagem.Retificacao.msgInst2(solicitacao.Id));
                            return false;
                        }
                }
                else
                {
                    if (solicitacao.SituacaoId == 2)
                    {
                        if (_caracterizacaoBus.ExisteCaracterizacaoPorEmpreendimento(entidade.Empreendimento.Id))
                        {
                            Validacao.Add(Mensagem.Retificacao.msgInst5());
                            return false;
                        }
                    }
                    else
                    {
                        if (solicitacao.SituacaoId == 5)
                        {
                            if (_busTitulo.ExistePorEmpreendimento(entidade.Empreendimento.Id))
                            {
                                Validacao.Add(Mensagem.Retificacao.msgInst3());
                                return false;
                            }
                            else
                                if (_caracterizacaoBus.ExisteCaracterizacaoPorEmpreendimento(entidade.Empreendimento.Id))
                                {
                                    Validacao.Add(Mensagem.Retificacao.msgCred5());
                                    return false;
                                }
                        }
                    }
                }
            }
            return Validacao.EhValido;
        }
    }
}