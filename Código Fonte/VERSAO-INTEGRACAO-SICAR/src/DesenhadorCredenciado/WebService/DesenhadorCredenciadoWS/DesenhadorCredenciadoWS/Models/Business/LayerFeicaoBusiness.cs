using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using Tecnomapas.DesenhadorWS.Models.DataAcess;
using Tecnomapas.DesenhadorWS.Models.Entities;

namespace Tecnomapas.DesenhadorWS.Models.Business
{
    public class LayerFeicaoBusiness
    {
        LayerFeicaoDa da = new LayerFeicaoDa();

         List<LayerFeicaoQuantidade> BuscarCacheLayers(int idProjeto)
        {
            return HttpContext.Current.Cache["CACHE_LAYERS" + idProjeto] as List<LayerFeicaoQuantidade>;
        }

        void SetarCacheLayers(int idProjeto, List<LayerFeicaoQuantidade> lista )
        {
            HttpContext.Current.Cache["CACHE_LAYERS" + idProjeto] = lista;
        }

        public static void LimparCacheLayers(int idProjeto)
        {
         //   HttpContext.Current.Cache.Remove("CACHE_LAYERS" + idProjeto);
        }
        
        public LayerFeicaoBusiness()
        {
            da = new LayerFeicaoDa();
        }


        public List<CategoriaLayerFeicao> ListarCategoria(int idNavegador, int idProjeto)
        {
            List<CategoriaLayerFeicao> lista = da.ListarCategoria(idNavegador, idProjeto);
            
            if (lista == null) return null;

            List<ListaDeValores> listaDeValores = new List<ListaDeValores>();
            foreach (CategoriaLayerFeicao c in lista)
            {
                foreach (LayerFeicao lf in c.LayersFeicoes)
                {
                    foreach (ColunaLayerFeicao col in lf.Colunas)
                    {
                        if (col.Tipo != 4)
                            continue;

                        ListaDeValores lVal = null;
                        lVal = listaDeValores.Find(delegate(ListaDeValores l) { return l.TabelaReferenciada == col.Tabela_Referencia && l.ColunaReferenciada == col.Coluna_Referencia; });

                        if (lVal == null)
                        {
                            lVal = new ListaDeValores();
                            lVal.TabelaReferenciada = col.Tabela_Referencia;
                            lVal.ColunaReferenciada = col.Coluna_Referencia;
                            lVal.Itens = da.BuscarListaDeValores(col.Tabela_Referencia, col.Coluna_Referencia);
                            listaDeValores.Add(lVal);
                        }
                        col.Itens = lVal.Itens;

                    }
                }
            }
            
            return lista;
        }

        public LayerFeicao Buscar(Hashtable htFiltros)
        {
            return da.Buscar(htFiltros);
        }

        public List<LayerFeicaoQuantidade> ListarQuantidadeFeicoes(int idNavegador, int idProjeto)
        {
           /* List<LayerFeicaoQuantidade> layers = BuscarCacheLayers(idProjeto);
            if (layers == null)
            {*/
            //    layers = da.ListarQuantidadeFeicoes(idNavegador, idProjeto);
               // SetarCacheLayers(idProjeto, layers);
            //}

                return da.ListarQuantidadeFeicoes(idNavegador, idProjeto); ;
        }


    }
}