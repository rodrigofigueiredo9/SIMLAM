using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
	public class ProdutoDestinacaoVM
	{
        private List<ProdutoApreendido> _listaProdutos;
        public List<ProdutoApreendido> ListaProdutos
        {
            get
            {
                return _listaProdutos;
            }
            set
            {
                _listaProdutos = value;
            }
        }

        //public String Mensagens
        //{
        //    get
        //    {
        //        return ViewModelHelper.Json(new
        //        {
        //            @NomeItemObrigatorio = Mensagem.FiscalizacaoConfiguracao.CampoNomeObrigatorio,
        //            @ItemDuplicado = Mensagem.FiscalizacaoConfiguracao.CampoJaAdicionado,
        //            @UnidadeCampoObrigatorio = Mensagem.FiscalizacaoConfiguracao.CampoUnidadeObrigatorio,
        //            @TipoCampoObrigatorio = Mensagem.FiscalizacaoConfiguracao.CampoTipoObrigatorio,
        //            @EditarItemDesativado = Mensagem.FiscalizacaoConfiguracao.EditarCampoInfracaoDesativado
        //        });
        //    }
        //}

        //public CampoInfracaoVM(List<Item> itens, List<Lista> tipos, List<Lista> unidades)
        //{
        //    Entidade.Itens = itens;
        //    Tipos = ViewModelHelper.CriarSelectList(tipos, true, true);
        //    Unidades = ViewModelHelper.CriarSelectList(unidades, true, true);
        //}

        public ProdutoDestinacaoVM(List<ProdutoApreendido> listaProdutos)
        {
            ListaProdutos = listaProdutos;
        }

        public ProdutoDestinacaoVM()
        {
            ListaProdutos = new List<ProdutoApreendido>();
        }

	}
}