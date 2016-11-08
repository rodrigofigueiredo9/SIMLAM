

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloGeoProcessamento
{
    public class Logradouro
    {
        public int id { get; set; }
        public string tipo_logradouro { get; set; }
        public string nome { get; set; }
        public Decimal x { get; set; }
        public Decimal y { get; set; }
    }
}