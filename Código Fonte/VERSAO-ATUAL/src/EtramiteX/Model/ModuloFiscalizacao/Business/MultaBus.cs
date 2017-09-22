﻿using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class MultaBus
	{
		#region Propriedades

		MultaValidar _validar = null;
        MultaDa _da = new MultaDa();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public MultaBus()
		{
            _validar = new MultaValidar();
		}

        public MultaBus(MultaValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

        public bool Salvar(Multa entidade)
        {
            try
            {
                if (_validar.Salvar(entidade))
                {
                    if (entidade.Id < 1)
                    {
                        entidade.Id = _da.ObterID(entidade.FiscalizacaoId);
                    }

                    GerenciadorTransacao.ObterIDAtual();

                    using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
                    {
                        bancoDeDados.IniciarTransacao();

                        #region Arquivo

                        if (entidade.Arquivo != null)
                        {
                            if (!string.IsNullOrWhiteSpace(entidade.Arquivo.Nome))
                            {
                                if (entidade.Arquivo.Id != null && entidade.Arquivo.Id == 0)
                                {
                                    ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
                                    entidade.Arquivo = _busArquivo.Copiar(entidade.Arquivo);
                                }

                                if (entidade.Arquivo.Id == 0)
                                {
                                    ArquivoDa _arquivoDa = new ArquivoDa();
                                    _arquivoDa.Salvar(entidade.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
                                }
                            }
                            else
                            {
                                entidade.Arquivo.Id = null;
                            }
                        }

                        #endregion

                        _da.Salvar(entidade, bancoDeDados);

                        bancoDeDados.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Validacao.AddErro(e);
            }

            return Validacao.EhValido;
        }

		#endregion

		#region Obter

        public Multa Obter(int fiscalizacaoId, BancoDeDados banco = null)
        {
            Multa entidade = new Multa();

            try
            {
                entidade = _da.Obter(fiscalizacaoId, banco);

                if (entidade == null)
                {
                    entidade = _da.ObterAntigo(fiscalizacaoId, banco);
                }
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return entidade;
        }

        //public Infracao ObterHistoricoPorFiscalizacao(int fiscalizacaoId, BancoDeDados banco = null)
        //{
        //    return _da.ObterHistoricoPorFiscalizacao(fiscalizacaoId, banco);
        //}

		#endregion

        //public void Mergear(Infracao entidadeAtual, Infracao entidadeNovoConfig)
        //{
        //    if (entidadeAtual.ClassificacaoId != entidadeNovoConfig.ClassificacaoId)
        //    {
        //        entidadeAtual.Campos = new List<InfracaoCampo>();
        //        entidadeAtual.Perguntas = new List<InfracaoPergunta>();
        //        entidadeAtual.ClassificacaoId = 0;
        //        entidadeAtual.TipoId = 0;
        //        entidadeAtual.ItemId = 0;
        //        return;				
        //    }
        //    else if (entidadeAtual.TipoId != entidadeNovoConfig.TipoId)
        //    {
        //        entidadeAtual.Campos = new List<InfracaoCampo>();
        //        entidadeAtual.Perguntas = new List<InfracaoPergunta>();
        //        entidadeAtual.TipoId = 0;
        //        entidadeAtual.ItemId = 0;
        //        return;				
        //    }
        //    else if (entidadeAtual.ItemId != entidadeNovoConfig.ItemId)
        //    {
        //        entidadeAtual.Campos = new List<InfracaoCampo>();
        //        entidadeAtual.Perguntas = new List<InfracaoPergunta>();
        //        entidadeAtual.ItemId = 0;
        //        return;				
        //    }

        //    entidadeNovoConfig.Campos.ForEach(x => 
        //    {
        //        var itemCampo = entidadeAtual.Campos.Find(z => z.CampoId == x.CampoId);

        //        if (itemCampo != null)
        //        {
        //            x.Id = itemCampo.Id;
        //            x.Identificacao = itemCampo.Identificacao;
        //            x.Texto = itemCampo.Texto;
        //            x.Tipo = itemCampo.Tipo;
        //            x.TipoTexto = itemCampo.TipoTexto;
        //            x.Unidade = itemCampo.Unidade;
        //            x.UnidadeTexto = itemCampo.UnidadeTexto;
        //        }
        //    });

        //    entidadeNovoConfig.Perguntas.ForEach(x =>
        //    {
        //        var itemPergunta = entidadeAtual.Perguntas.Find(z => z.PerguntaId == x.PerguntaId);

        //        if (itemPergunta != null)
        //        {
        //            x.Id = itemPergunta.Id;
        //            x.Especificacao = itemPergunta.Especificacao;
        //            x.Identificacao = itemPergunta.Identificacao;
        //            x.IsEspecificar = itemPergunta.IsEspecificar;
        //            x.RespostaId = itemPergunta.RespostaId;
        //            x.Respostas = itemPergunta.Respostas;
        //        }
        //    });

        //    entidadeAtual.Campos = entidadeNovoConfig.Campos;
        //    entidadeAtual.Perguntas = entidadeNovoConfig.Perguntas;
        //    entidadeAtual.ConfiguracaoTid = entidadeNovoConfig.ConfiguracaoTid;
        //}

        //public bool ConfigAlterada(int configuracaoId, string tid, BancoDeDados banco = null)
        //{
        //    var configAlterada = false;
        //    try
        //    {
        //        configAlterada = _da.ConfigAlterada(configuracaoId, tid, banco);
        //    }
        //    catch (Exception exc)
        //    {
        //        Validacao.AddErro(exc);
        //    }
        //    return configAlterada;
        //}
	}
}
