

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloGeoProcessamento
{
    public class Lote
    {
        public int id { get; set; }
        public int codfiscal { get; set; }
        public int codquadras { get; set; }
        public int codlotes { get; set; }
        public string proprietario { get; set; }
        public string tipo_logradouro { get; set; }
        public string logradouro { get; set; }
        public int numero { get; set; }
        public string bairro { get; set; }
        public Decimal x { get; set; }
        public Decimal y { get; set; }
    }
}