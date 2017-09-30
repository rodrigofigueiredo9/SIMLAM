using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class InfracaoPergunta
    {
        public Int32 Id { get; set; }
        public Int32 ConfiguracaoId { get; set; }
        public Int32 PerguntaId { get; set; }
        public String PerguntaTid { get; set; }
        public Int32 RespostaId { get; set; }
        public String RespostaTid { get; set; }
        public String Identificacao { get; set; }
        public String Especificacao { get; set; }
        public Boolean IsEspecificar { get; set; }

        //private List<InfracaoResposta> _respostas { get; set; }
        /*public List<InfracaoResposta> Respostas
        {
            get { return _respostas; }
            set { _respostas = value; }
        }*/

    }
}
