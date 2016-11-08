using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class ColunaLayerFeicao
    {
        public string Coluna { get; set; }
        public string Alias { get; set; }
        public int Tipo { get; set; }
        public double Tamanho { get; set; }
        public string Tabela_Referencia { get; set; }
        public string Coluna_Referencia { get; set; }
        public List<Item> Itens { get; set; }
        public bool IsObrigatorio { get; set; }
        public bool IsVisivel { get; set; }
        public bool IsEditavel { get; set; }
        public TipoOperacao Operacao { get; set; }
        public string ValorCondicao { get; set; }
        public string ColunaObrigada { get; set; }
    }
}