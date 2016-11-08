using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV
{
    public class PTVItemHistorico
    {
        public int Id { get; set; }
        public string DataAnalise { get; set; }
        public string Analista { get; set; }
        public string SetorTexto { get; set; }
        public string SituacaoTexto { get; set; }
        public string MotivoTexto { get; set; } 
    } 
}
