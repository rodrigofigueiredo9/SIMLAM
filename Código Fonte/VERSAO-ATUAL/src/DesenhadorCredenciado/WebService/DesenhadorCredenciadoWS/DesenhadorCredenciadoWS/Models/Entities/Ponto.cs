using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.TecnoGeo.Geometria;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class Ponto : FeicaoGeometria
    {
        public Ponto(List<Vertice> _vertices, List<AtributoFeicao> _atributos)
        {
            if (_vertices != null)
                Vertices = _vertices;
            else
                Vertices = new List<Vertice>();

            if (_atributos != null)
                Atributos = _atributos;
            else
                Atributos = new List<AtributoFeicao>();            

            TabelaRascunho = "TAB_PONTO";
        }
        public string SchemaRascunho { get; set; }
        public TipoGeometria Tipo { get { return TipoGeometria.Ponto; } }
        public override Geometria RetornarGeometria()
        {
            Tecnomapas.TecnoGeo.Geometria.Primitiva.Ponto ponto = null;
            if (Vertices == null)
                throw new ApplicationException("Referência nula de objeto");

            if (Vertices.Count > 0)
                ponto = new Tecnomapas.TecnoGeo.Geometria.Primitiva.Ponto(new Posicao(Convert.ToDecimal(Vertices[0].X), 
                                Convert.ToDecimal(Vertices[0].Y)), 2, false);

            return ponto;
        }
    }
}