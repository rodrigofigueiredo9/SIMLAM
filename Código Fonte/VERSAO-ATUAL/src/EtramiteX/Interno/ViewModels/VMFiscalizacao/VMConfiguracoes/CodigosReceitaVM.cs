using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
	public class CodigosReceitaVM
	{
		private List<CodigoReceita> _listaCodigosReceita; 
        public List<CodigoReceita> ListaCodigosReceita 
        { 
            get 
            { 
                return _listaCodigosReceita; 
            } 
            set 
            { 
                _listaCodigosReceita = value; 
            } 
        }

        public String Mensagens
        {
            get
            {
                return ViewModelHelper.Json(new
                {
                    @CodigoReceitaObrigatorio = Mensagem.FiscalizacaoConfiguracao.CodigoReceitaObrigatorio,
                    @DescricaoCodigoObrigatoria = Mensagem.FiscalizacaoConfiguracao.DescricaoCodigoObrigatoria,
                    @CodigoReceitaDuplicado = Mensagem.FiscalizacaoConfiguracao.CodigoReceitaDuplicado
                });
            }
        }

        public CodigosReceitaVM() 
        {
            ListaCodigosReceita = new List<CodigoReceita>();
        } 

	}
}