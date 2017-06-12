using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.DeclaracaoAdicional;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using System.Data;
using Tecnomapas.EtramiteX.Configuracao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business
{
    public class DeclaracaoAdicionalBus
    {

        #region Propriedades

        DeclaracaoAdicionalValidar _validar = new DeclaracaoAdicionalValidar();
        DeclaracaoAdicionalDa _da = new DeclaracaoAdicionalDa();
        #endregion

        public DeclaracaoAdicionalBus()
        {

        }

        #region DML

        public List<ListaValor> ObterListaDeclaracao(int  OutroEstado = 0)
        {
            return _da.ObterListaDeclaracao(OutroEstado);
        }

        public bool Excluir(int id)
        {
            bool retErros = false;
            try
            {
                using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
                {
                    bancoDeDados.IniciarTransacao();

                    _da.Excluir(id, bancoDeDados);
                    bancoDeDados.Commit();

                    Validacao.Add(Mensagem.Declaracao.DeclaracaoExcluidaComSucesso);

                    retErros = true;
                }
            }
            catch (Exception)
            {
                //Validacao.AddErro(exc);
                //retErros = false;
                Validacao.Add(Mensagem.Declaracao.DeclaracaoExcluidaComErro);

                retErros = false;
            }

            return retErros;
        }


        public void Salvar(DeclaracaoAdicional declaracao, BancoDeDados banco = null)
        {
            try
            {
                if (string.IsNullOrEmpty(declaracao.Texto))
                {
                    Validacao.Add(Mensagem.Declaracao.TextoObrigatorio);
                    return;
                }

                if (!declaracao.OutroEstado.HasValue)
                {
                    Validacao.Add(Mensagem.Declaracao.OutroEstadoObrigatorio);
                    return;
                }



                GerenciadorTransacao.ObterIDAtual();
                using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
                {
                    bancoDeDados.IniciarTransacao();
                    _da.Salvar(declaracao, bancoDeDados);
                    bancoDeDados.Commit();
                    Validacao.Add(Mensagem.Declaracao.DeclaracaoSalvaComSucesso);
                }
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }
        }

  
        #endregion

        #region Obter/Filtrar

        public DeclaracaoAdicional Obter(int id)
        {
            DeclaracaoAdicional declaracao = null;

            try
            {
                declaracao = _da.Obter(id);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }
            return declaracao;
        }



        public Resultados<DeclaracaoAdicional> Filtrar(DeclaracaoAdicional filtro, Paginacao paginacao)
        {
            try
            {
                Filtro<DeclaracaoAdicional> filtros = new Filtro<DeclaracaoAdicional>(filtro, paginacao);
                Resultados<DeclaracaoAdicional> resultados = _da.Filtrar(filtros);

                if (resultados.Quantidade < 1)
                {
                    Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
                }

                return resultados;
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return null;
        }

        

        #endregion
    }
}