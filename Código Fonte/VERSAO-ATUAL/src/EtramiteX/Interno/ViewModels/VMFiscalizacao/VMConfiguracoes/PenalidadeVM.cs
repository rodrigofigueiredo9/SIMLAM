using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
    public class PenalidadeVM
    {
        public Boolean IsVisualizar { get; set; }



        private Penalidade _entidade = new Penalidade();
        public Penalidade Entidade
        {
            get { return _entidade; }
            set { _entidade = value; }
        }

      
        public String Mensagens
        {
            get
            {
                return ViewModelHelper.Json(new
                {
                    @ArtigoObrigatorio = Mensagem.FiscalizacaoConfiguracao.ArtigoObrigatorio,
                    @PenalidadeJaAdicionada = Mensagem.FiscalizacaoConfiguracao.PenalidadeJaAdicionada,
                    @ItemObrigatorio = Mensagem.FiscalizacaoConfiguracao.ItemCampoObrigatorio,
                    @DescricaoObrigatorio = Mensagem.FiscalizacaoConfiguracao.DescricaoObrigatorio
                });
            }
        }

        public String IdsTela
        {
            get
            {
                return ViewModelHelper.Json(new
                {
                    @TipoCampoTexto = eTipoCampo.Texto,
                    @TipoCampoNumerico = eTipoCampo.Numerico,
                });
            }
        }

        public PenalidadeVM(List<Penalidade> itens, string Artigo, string Item, string Descricao)
        {

            _entidade.Itens = itens;
            _entidade.Artigo = Artigo;
            _entidade.Item = Item;
            _entidade.Descricao = Descricao;
        }

    }
}