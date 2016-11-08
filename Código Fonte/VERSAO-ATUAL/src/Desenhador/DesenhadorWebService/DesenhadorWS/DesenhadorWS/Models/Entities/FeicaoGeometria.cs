using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.TecnoGeo.Geometria;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public abstract class FeicaoGeometria
    {
        public int IdLayerFeicao;
        public int ObjectId;
        public List<AtributoFeicao> Atributos;
        public List<Vertice> Vertices;
        public abstract Geometria RetornarGeometria();
        public string TabelaRascunho;

    }
}