using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business
{
	public class CARSolicitacaoValidar
	{
		#region Propriedades

		CaracterizacaoBus _busCaracterizacao = null;
		ProjetoGeograficoBus _busProjetoGeografico = null;
		ProjetoDigitalCredenciadoBus _busProjetoDigital = null;
		RequerimentoCredenciadoBus _busRequerimento = null;
		CARSolicitacaoDa _daCarSolicitacao = null;
		CARSolicitacaoInternoDa _carSolicitacaoInternoDa = null;
		RequerimentoCredenciadoValidar _requerimentoValidar = null;
        TituloCredenciadoBus _busTitulo = null;

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public CARSolicitacaoValidar()
		{
			_requerimentoValidar = new RequerimentoCredenciadoValidar();
			_busCaracterizacao = new CaracterizacaoBus();
			_busProjetoGeografico = new ProjetoGeograficoBus();
			_busProjetoDigital = new ProjetoDigitalCredenciadoBus();
			_busRequerimento = new RequerimentoCredenciadoBus();
			_daCarSolicitacao = new CARSolicitacaoDa();
			_carSolicitacaoInternoDa = new CARSolicitacaoInternoDa();
            _busTitulo = new TituloCredenciadoBus();
            
		}

		#endregion

		internal bool Salvar(CARSolicitacao carSolicitacao)
		{
			if (carSolicitacao.ProjetoId < 1)
			{
				Validacao.Add(Mensagem.CARSolicitacao.ProjetoObrigatorio);
				return false;
			}

			if (carSolicitacao.Requerimento.Id < 1)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoRequerimentoObrigatorio);
			}

			if (carSolicitacao.Atividade.Id < 1)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAtividadeObrigatoria);
			}

			if (carSolicitacao.Atividade.Id != ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.CadastroAmbientalRural))
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAtividadeInvalida);
			}

			if (carSolicitacao.Empreendimento.Id < 1)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoEmpreendimentoObrigatorio);
			}

			if (carSolicitacao.Empreendimento.Codigo < 1)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoEmpreendimentoCodigoObrigatorio);
			}

			if (string.IsNullOrEmpty(carSolicitacao.Empreendimento.NomeRazao))
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoEmpreendimentoNomeRazaoObrigatorio);
			}

			if (carSolicitacao.Declarante.Id < 1)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoDeclaranteObrigatorio);
			}

			#region Projeto Digital

			ProjetoDigital projetoDigital = _busProjetoDigital.Obter(carSolicitacao.ProjetoId);

			if (projetoDigital.Situacao != (int)eProjetoDigitalSituacao.AguardandoImportacao)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SituacaoDeveSerAguardandoImportacao);
			}

			#endregion

			#region Requerimento

			Requerimento requerimento = _busRequerimento.ObterSimplificado(carSolicitacao.Requerimento.Id);

			if (requerimento == null || requerimento.Id < 1)
			{
				Validacao.Add(Mensagem.Requerimento.Inexistente);
			}

			if (requerimento.CredenciadoId != User.FuncionarioId)
			{
				Validacao.Add(Mensagem.Requerimento.PosseCredenciado);
			}

			if (requerimento.Empreendimento.Id < 1)
			{
				Validacao.Add(Mensagem.CARSolicitacao.ProjetoDigitalSemEmpreendimento);
			}

			#endregion

			#region  Solitação de CAR

			EmpreendimentoCaracterizacao empreendimento = _busCaracterizacao.ObterEmpreendimentoSimplificado(carSolicitacao.Empreendimento.Id);
			if (!EmpreendimentoSolicitacaoCAR(empreendimento))
			{
				return false;
			}

			#endregion  Solitação de CAR

			#region Validar Dados da Caracterizacao

			if (projetoDigital.Dependencias.Count(x => x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico) < 1)
			{
				Validacao.Add(Mensagem.CARSolicitacao.ProjetoDigitalDominialidadeAssociada);
			}

			projetoDigital.Dependencias.ForEach(x =>
			{
				if (x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico && x.DependenciaCaracterizacao == (int)eCaracterizacao.Dominialidade)
				{
					if (_busProjetoGeografico.ObterSitacaoProjetoGeografico(x.DependenciaId) != (int)eProjetoGeograficoSituacao.Finalizado)
					{
						Validacao.Add(Mensagem.CARSolicitacao.ProjetoGeograficoNaoEstaFinalizado);
					}
				}
			});

			#endregion

			#region Validar Dados do Titulo

			String tituloSituacao = _carSolicitacaoInternoDa.ObterSituacaoTituloCARExistente(empreendimento.InternoID);

			if (!String.IsNullOrWhiteSpace(tituloSituacao))
			{
				Validacao.Add(Mensagem.CARSolicitacao.EmpreendimentoPossuiTitulo(tituloSituacao));
			}

			#endregion

			return Validacao.EhValido;
		}

		public void AssociarProjetoDigital(ProjetoDigital projetoDigital, List<Lista> atividades)
		{
			if (projetoDigital.Situacao != (int)eProjetoDigitalSituacao.AguardandoImportacao)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SituacaoDeveSerAguardandoImportacao);
			}

			if (projetoDigital.EmpreendimentoId < 1)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoEmpreendimentoObrigatorio);
			}

			if (atividades.Count < 1)
			{
				Validacao.Add(Mensagem.CARSolicitacao.RequerimentoNaoPossuiAtividadeCAR);
			}

			#region  Solitação de CAR

			EmpreendimentoCaracterizacao empreendimento = _busCaracterizacao.ObterEmpreendimentoSimplificado(projetoDigital.EmpreendimentoId.GetValueOrDefault());
			if (!EmpreendimentoSolicitacaoCAR(empreendimento))
			{
				return;
			}

			#endregion  Solitação de CAR

			if (projetoDigital.Dependencias.Count(x => x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico) < 1)
			{
				Validacao.Add(Mensagem.CARSolicitacao.ProjetoDigitalDominialidadeAssociada);
				return;
			}

			if (projetoDigital.Dependencias.Count(x => x.DependenciaCaracterizacao == (int)eCaracterizacao.Dominialidade) < 1)
			{
				Validacao.Add(Mensagem.CARSolicitacao.ProjetoDigitalNaoPossuiCaracterizacaoDominialidade);
				return;
			}

			projetoDigital.Dependencias.ForEach(x =>
			{
				if (x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico && x.DependenciaCaracterizacao == (int)eCaracterizacao.Dominialidade)
				{
					if (_busProjetoGeografico.ObterSitacaoProjetoGeografico(x.DependenciaId) != (int)eProjetoGeograficoSituacao.Finalizado)
					{
						Validacao.Add(Mensagem.CARSolicitacao.ProjetoGeograficoNaoEstaFinalizado);
					}
				}
			});
		}

		internal bool EmpreendimentoSolicitacaoCAR(EmpreendimentoCaracterizacao empreendimento)
		{
			if (object.Equals(empreendimento, null))
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoEmpreendimentoObrigatorio);
				return false;
			}

			string situacao = string.Empty;

			if (!String.IsNullOrWhiteSpace(empreendimento.CNPJ))
			{
				situacao = _daCarSolicitacao.EmpreendimentoPossuiSolicitacao(empreendimento.CNPJ);
				if (!String.IsNullOrWhiteSpace(situacao))
				{
					Validacao.Add(Mensagem.CARSolicitacao.EmpreendimentoJaPossuiSolicitacao(situacao));
					return false;
				}

				situacao = _carSolicitacaoInternoDa.EmpreendimentoPossuiSolicitacao(empreendimento.CNPJ);
				if (!String.IsNullOrWhiteSpace(situacao))
				{
					Validacao.Add(Mensagem.CARSolicitacao.EmpreendimentoJaPossuiSolicitacao(situacao));
					return false;
				}
			}
			else
			{
                CARSolicitacao solicitacao = new CARSolicitacao();
				if (empreendimento.InternoID > 0)
				{
                    solicitacao = _daCarSolicitacao.EmpreendimentoPossuiSolicitacaoProjetoDigital(empreendimento.InternoID);
                    if (solicitacao.SituacaoId != null && solicitacao.SituacaoId != 0)
					{
                        if (solicitacao.SituacaoId ==2)
                        {
                            Validacao.Add(Mensagem.Retificacao.msgCred5());
                        }
                        else if (solicitacao.SituacaoId == 5)
                        {
                            Validacao.Add(Mensagem.Retificacao.msgCred6());
                        }
                        else
                        {
                            Validacao.Add(Mensagem.Retificacao.msgCred2(solicitacao.ProjetoId, solicitacao.Id));
                        }
						return false;
					}

                    solicitacao = _carSolicitacaoInternoDa.EmpreendimentoPossuiSolicitacaoProjetoDigital(empreendimento.InternoID);
                    if (solicitacao.SituacaoId != null && solicitacao.SituacaoId != 0)
					{
                        if (solicitacao.SituacaoId == 2)
                        {
                            Validacao.Add(Mensagem.Retificacao.msgCred5());
                        }
                        else if (solicitacao.SituacaoId == 5)
                        {
                            Validacao.Add(Mensagem.Retificacao.msgCred6());
                        }
                        else
                        {
                            Validacao.Add(Mensagem.Retificacao.msgCred2(solicitacao.ProjetoId, solicitacao.Id));
                        }
						return false;
					}
				}
				else
				{
                    solicitacao = _daCarSolicitacao.EmpreendimentoCredenciadoPossuiSolicitacaoProjetoDigital(empreendimento.Id);
                    if (solicitacao.SituacaoId != null && solicitacao.SituacaoId != 0)
					{
                        if (solicitacao.SituacaoId == 2)
                        {
                            Validacao.Add(Mensagem.Retificacao.msgCred5());
                        }
                        else if (solicitacao.SituacaoId == 5)
                        {
                            Validacao.Add(Mensagem.Retificacao.msgCred6());
                        }
                        else
                        {
                            Validacao.Add(Mensagem.Retificacao.msgCred2(solicitacao.ProjetoId, solicitacao.Id));
                        }
						return false;
					}
				}
			}

			return true;
		}

		public bool AlterarSituacao(CARSolicitacao entidade)
		{
			if (entidade.SituacaoId <= 0)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAlterarSituacaoNovaSituacaoObrigatoria);
			}

			if (entidade.SituacaoId == entidade.SituacaoAnteriorId)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAlterarSituacaoNovaSituacaoObrigatoria);
			}

			var situacaoArquivo = _daCarSolicitacao.BuscaSituacaoAtualArquivoSICAR(entidade.Id).Item1;

			if (entidade.SituacaoAnteriorId == (int)eCARSolicitacaoSituacao.Pendente && situacaoArquivo == eStatusArquivoSICAR.ArquivoReprovado
				&& entidade.SituacaoId != (int)eCARSolicitacaoSituacao.Invalido)
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAlterarSituacaoNovaSituacaoNaoPermitida);


			if (entidade.SituacaoAnteriorId == (int)eCARSolicitacaoSituacao.Valido && situacaoArquivo == eStatusArquivoSICAR.Nulo
				&& (entidade.SituacaoId != (int)eCARSolicitacaoSituacao.Invalido))
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAlterarSituacaoNovaSituacaoNaoPermitida);

			if (entidade.SituacaoAnteriorId == (int)eCARSolicitacaoSituacao.Suspenso && situacaoArquivo == eStatusArquivoSICAR.Nulo
				&& (entidade.SituacaoId != (int)eCARSolicitacaoSituacao.Valido && entidade.SituacaoId != (int)eCARSolicitacaoSituacao.Invalido))
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoAlterarSituacaoNovaSituacaoNaoPermitida);


			if (situacaoArquivo != eStatusArquivoSICAR.Nulo && situacaoArquivo != eStatusArquivoSICAR.ArquivoReprovado)
				Validacao.Add(Mensagem.CARSolicitacao.AcessarAlterarSituacaoSolicitacaoEnviadaSICAR);


			return Validacao.EhValido;
		}

		public bool AcessoEnviarArquivoSICAR(CARSolicitacao entidade, int origem)
		{
			if (origem == (int)eCARSolicitacaoOrigem.Institucional)
			{
				Validacao.Add(new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Não é possível enviar a Solicitação de Inscrição cadastrada pelo Módulo Institucional. Entre em contato com a unidade do IDAF mais próxima." });
				return false;
			}

			if (entidade.Id <= 0)
			{
				Validacao.Add(Mensagem.Padrao.Inexistente);
				return false;
			}

            if (entidade.SituacaoId == (int)eCARSolicitacaoSituacao.Invalido || entidade.SituacaoId == (int)eCARSolicitacaoSituacao.EmCadastro)//entidade.SituacaoId == (int)eCARSolicitacaoSituacao.Suspenso)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaEnviarSituacaoSICARInvalida(entidade.SituacaoTexto));
			}

			var situacaoArquivo = _daCarSolicitacao.BuscaSituacaoAtualArquivoSICAR(entidade.Id);

			if (situacaoArquivo.Item1 == eStatusArquivoSICAR.ArquivoEntregue)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoJaEnviada);
				return false;
			}

			if (situacaoArquivo.Item1 != eStatusArquivoSICAR.Nulo && situacaoArquivo.Item1 != eStatusArquivoSICAR.ArquivoReprovado)
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaEnviarSituacaoArquivoSICARInvalida(situacaoArquivo.Item2));
			}

			return Validacao.EhValido;
		}

        public bool RetificacaoValidar(CARSolicitacao entidade, int origem)
        {
            string situacao = string.Empty;
            CARSolicitacao solicitacao = new CARSolicitacao();
            //EmpreendimentoCaracterizacao empreendimento = _busCaracterizacao.ObterEmpreendimentoSimplificado(projetoDigital.EmpreendimentoId.GetValueOrDefault());
            //situacao = _daCarSolicitacao.EmpreendimentoCredenciadoPossuiSolicitacao(entidade.Empreendimento.Id);
            /*if (!string.IsNullOrEmpty(situacao))
            {
                Validacao.Add(Mensagem.CARSolicitacao.EmpreendimentoJaPossuiSolicitacao(situacao));
                return false;
            }*/

            solicitacao = _daCarSolicitacao.ObterPorProjetoDigitalSituacao(entidade.ProjetoId);
            if(solicitacao != null)
            {
                if (solicitacao.SituacaoId == 1 || solicitacao.SituacaoId == 6)
                {
                    Validacao.Add(Mensagem.Retificacao.msgCred1(entidade.Requerimento.Id, solicitacao.Id));
                    return false;
                }
                if (solicitacao.SituacaoId == 2 || solicitacao.SituacaoId == 5)
                {
                    Validacao.Add(Mensagem.Retificacao.msgCred3(entidade.Requerimento.Id, solicitacao.Id));
                    return false;
                }
            }

            solicitacao = _daCarSolicitacao.ObterPorEmpreendimento(entidade.Empreendimento.Id);
            if(solicitacao != null)
            {
                if(solicitacao.SituacaoId == 2)
                {
                    if(_busCaracterizacao.ExisteCaracterizacaoPorProjetoDigital(entidade.ProjetoId))
                    {
                        Validacao.Add(Mensagem.Retificacao.msgCred5());
                        return false;
                    }
                }
                if(solicitacao.SituacaoId == 5)
                {
                    if (_busTitulo.ObterPorEmpreendimento(entidade.Empreendimento.Id))
                    {
                        Validacao.Add(Mensagem.Retificacao.msgCred6());
                        return false;
                    }
                    else
                    {
                        if (_busCaracterizacao.ExisteCaracterizacaoPorProjetoDigital(entidade.ProjetoId))
                        {
                            Validacao.Add(Mensagem.Retificacao.msgCred5());
                            return false;
                        }
                    }
                }
                if (solicitacao.SituacaoId == 1 || solicitacao.SituacaoId == 6)
                {
                    Validacao.Add(Mensagem.Retificacao.msgCred2(entidade.Requerimento.Id, solicitacao.Id));
                    return false;
                }
            }
            Validacao.Add(Mensagem.Retificacao.msgCred6());
            return false;
            //return Validacao.EhValido;
        }
	}
}