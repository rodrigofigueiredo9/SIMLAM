using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class CategoriaLayerFeicao
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public List<LayerFeicao> LayersFeicoes { get; set; }
        public LayerFeicao[] FeicoesArray { 
            get {
                if (LayersFeicoes == null)
                {
                    return new List<LayerFeicao>().ToArray();
                }
                else
                {
                    return LayersFeicoes.ToArray();
                }
            }
        }

        public CategoriaLayerFeicao()
        {
            LayersFeicoes = new List<LayerFeicao>();
        }
    }
}