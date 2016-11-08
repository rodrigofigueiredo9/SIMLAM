using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.TecnoGeo.Geometria;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class FeicaoObjeto
    {
        public int IdLayerFeicao {get; set;}
        public int ObjectId { get; set; }
        public int IdProjeto { get; set; }
        public List<AtributoFeicao> Atributos { get; set; }
        public List<Vertice> Vertices { get; set; }
        public List<List<Vertice>> Aneis { get; set; }

    }
}