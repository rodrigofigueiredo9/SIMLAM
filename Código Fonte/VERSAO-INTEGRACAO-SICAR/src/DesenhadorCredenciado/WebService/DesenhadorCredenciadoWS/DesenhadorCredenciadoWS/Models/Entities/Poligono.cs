using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.TecnoGeo.Geometria.Primitiva;
using Tecnomapas.TecnoGeo.Geometria;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class Poligono : FeicaoGeometria
    {
        public Poligono(List<Vertice> _anelNovo, List<AtributoFeicao> _atributos, List<List<Vertice>> _aneis = null)
        {
            if (_anelNovo != null)
            {
                Vertices = _anelNovo;
            }
            else
                Vertices = new List<Vertice>();

            if (_atributos != null)
                Atributos = _atributos;
            else
                Atributos = new List<AtributoFeicao>();

            if (_aneis != null)
            {
                Aneis = _aneis;
            }

            TabelaRascunho = "TAB_POLIGONO";
        }
        public string SchemaRascunho { get; set; }
        public TipoGeometria Tipo { get { return TipoGeometria.Poligono; } }
        public List<List<Vertice>> Aneis { get; set; }
        public override Geometria RetornarGeometria()
        {
            if (Vertices == null && Aneis == null)
                throw new ApplicationException("Referência nula de objeto");

           Tecnomapas.TecnoGeo.Geometria.Primitiva.Poligono poligono = new Tecnomapas.TecnoGeo.Geometria.Primitiva.Poligono(2, false);
           Anel anel = null;
           SegmentoLinhaReto segm = null;

            if (Aneis != null)
            {
                List<Vertice> lista;
                for (int i = 0; i < Aneis.Count; i++)
                {
                    segm = new Tecnomapas.TecnoGeo.Geometria.Primitiva.SegmentoLinhaReto();
                    lista = Aneis[i] as List<Vertice>;
                    if (lista != null)
                    {
                        for (int k = 0; k < lista.Count; k++)
                        {
                            segm.Posicoes.Adicionar(new Posicao(
                                Convert.ToDecimal(lista[k].X),  Convert.ToDecimal(lista[k].Y)));
                        }
                        if (lista.Count > 1 && !(lista[0].X == lista[lista.Count - 1].X && lista[0].Y == lista[lista.Count - 1].Y))
                        {
                            segm.Posicoes.Adicionar(new Posicao(Convert.ToDecimal(lista[0].X), 
                                Convert.ToDecimal(lista[0].Y)));
                        }
                        anel = new Anel();
                        anel.Segmentos.Adicionar(segm);
                        poligono.Aneis.Adicionar(anel);
                    }
                }
            }

            if (Vertices!=null && Vertices.Count >0)
            {
                anel = new Anel();
                segm = new SegmentoLinhaReto();
                for (int i = 0; i < Vertices.Count; i++)
                {
                    segm.Posicoes.Adicionar(new Posicao(Convert.ToDecimal(Vertices[i].X), 
                                Convert.ToDecimal(Vertices[i].Y)));
                }
                if (Vertices.Count > 1 && !(Vertices[0].X == Vertices[Vertices.Count - 1].X && Vertices[0].Y == Vertices[Vertices.Count - 1].Y))
                {
                    segm.Posicoes.Adicionar(new Posicao(Convert.ToDecimal(Vertices[0].X), 
                                Convert.ToDecimal(Vertices[0].Y)));
                }

                anel.Segmentos.Adicionar(segm);
                poligono.Aneis.Adicionar(anel);
            }
            return poligono;
        }

    }
}