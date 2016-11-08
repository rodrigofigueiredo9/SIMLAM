using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.TecnoGeo.Geometria;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class Linha : FeicaoGeometria
    {
        public Linha(List<Vertice> _vertices, List<AtributoFeicao> _atributos)
        {
            if (_vertices != null)
                Vertices = _vertices;
            else
                Vertices = new List<Vertice>();

            if (_atributos != null)
                Atributos = _atributos;
            else
                Atributos = new List<AtributoFeicao>(); 

            TabelaRascunho = "TAB_LINHA";
        }
        public string SchemaRascunho { get; set; }
        public TipoGeometria Tipo { get { return TipoGeometria.Linha; } }
        public override Geometria RetornarGeometria()
        {
            if (Vertices == null)
                throw new ApplicationException("Referência nula de objeto");

            Tecnomapas.TecnoGeo.Geometria.Primitiva.SegmentoLinhaReto segm = new Tecnomapas.TecnoGeo.Geometria.Primitiva.SegmentoLinhaReto();

            for (int i = 0; i < Vertices.Count; i++)
            {
                segm.Posicoes.Adicionar(new Posicao(Convert.ToDecimal(Vertices[i].X), 
                                Convert.ToDecimal(Vertices[i].Y)));
            }

            Tecnomapas.TecnoGeo.Geometria.Primitiva.Linha linha = new Tecnomapas.TecnoGeo.Geometria.Primitiva.Linha(2, false);
            linha.Segmentos.Adicionar(segm);

            return linha;    
        }
    }
}