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

        public String Mensagens
        {
            get
            {
                return ViewModelHelper.Json(new
                {
                    @ItemProdutoObrigatorio = Mensagem.FiscalizacaoConfiguracao.ItemProdutoObrigatorio,
                    @UnidadeProdutoObrigatoria = Mensagem.FiscalizacaoConfiguracao.UnidadeProdutoObrigatoria,
                    @ProdutoDuplicado = Mensagem.FiscalizacaoConfiguracao.ProdutoDuplicado
                });
            }
        }

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