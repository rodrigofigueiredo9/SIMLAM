using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public enum eFiscalizacaoSituacao
    {
        Nulo = 0,
        EmAndamento,
        CadastroConcluido,
        Protocolado,
        CancelarConclusao,
        ComDecisaoManutencaoMulta,
        ComDecisaoMultaCancelada,
        AIPago,
        EmParcelamento,
        ParceladopagamentoEmDia,
        ParceladoPagamentoAtrasado,
        InscritoNoCADIN,
        InscritoEmDividaAtiva,
        DefesaApresentada,
        RecursoApresentado,
        EnviadoParaSEAMA
    }
}
