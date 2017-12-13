using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFuncionario;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao
{
    public class Sessoes
    {
        public String Empreendimento { get; set; }
        public String Multa { get; set; }
        public String InterdicaoEmbargo { get; set; }
        public String Apreensao { get; set; }
        public String OutrasPenalidades { get; set; }

    }
}
