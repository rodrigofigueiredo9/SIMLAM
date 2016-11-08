using System;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Entities
{
    public class PontoEmpreendimento
    {
        public int id { get; set; }
        public string denominador { get; set; }
        public string segmento { get; set; }
        public string municipio { get; set; }
        public string atividade { get; set; }
        public string processos { get; set; }
        public Decimal x { get; set; }
        public Decimal y { get; set; }
    }
}