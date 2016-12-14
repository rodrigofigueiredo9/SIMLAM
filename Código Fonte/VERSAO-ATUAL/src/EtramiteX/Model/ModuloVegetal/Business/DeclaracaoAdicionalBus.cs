using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.DeclaracaoAdicional;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;

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
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
                retErros = false;
            }

            return retErros;
        }


        public void Salvar(DeclaracaoAdicional declaracao, BancoDeDados banco = null)
        {
            try
            {
                GerenciadorTransacao.ObterIDAtual();
                using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
                {
                    _da.Salvar(declaracao, bancoDeDados);
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