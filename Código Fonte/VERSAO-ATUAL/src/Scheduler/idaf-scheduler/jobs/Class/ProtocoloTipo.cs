using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class ProtocoloTipo : IListaValor
    {
        public int Id { get; set; }
        public String Texto { get; set; }
        public bool IsAtivo { get; set; }

        public bool PossuiProcesso { get; set; }
        public bool PossuiDocumento { get; set; }
        public bool PossuiChecagemPendencia { get; set; }
        public bool PossuiChecagemRoteiro { get; set; }
        public bool PossuiRequerimento { get; set; }
        public bool PossuiFiscalizacao { get; set; }
        public bool PossuiInteressado { get; set; }

        public bool ProcessoObrigatorio { get; set; }
        public bool DocumentoObrigatorio { get; set; }
        public bool ChecagemPendenciaObrigatorio { get; set; }
        public bool ChecagemRoteiroObrigatorio { get; set; }
        public bool RequerimentoObrigatorio { get; set; }
        public bool FiscalizacaoObrigatorio { get; set; }
        public bool InteressadoObrigatorio { get; set; }

        public String LabelInteressado { get; set; }
    }
}
