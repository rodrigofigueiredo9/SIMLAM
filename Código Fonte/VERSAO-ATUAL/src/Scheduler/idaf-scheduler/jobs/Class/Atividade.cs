using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Atividade
    {
        public Int32 Id { get; set; }
        public Int32 Codigo { get; set; }
        public Int32? IdRelacionamento { get; set; }
        public String NomeAtividade { get; set; }
        public Int32 SituacaoId { get; set; }
        public String SituacaoTexto { get; set; }
        public String Motivo { get; set; }
        public String Tid { get; set; }
        public Int32 Finalidade { get; set; }
        public String FinalidadeTexto { get; set; }
        public Int32 TituloModelo { get; set; }
        public String TituloModeloTexto { get; set; }
        public Int32 ModeloTituloAnterior { get; set; }
        public String ModeloTituloAnteriorTexto { get; set; }
        public String NumeroDocumentoAnterior { get; set; }
        public Int32 SetorId { get; set; }
        public Boolean Ativada { get; set; }

        private Protocolo _protocolo = new Protocolo();
        public Protocolo Protocolo
        {
            get { return _protocolo; }
            set { _protocolo = value; }
        }
        /*
        private List<Finalidade> _finalidades = new List<Finalidade>();
        public List<Finalidade> Finalidades
        {
            get { return _finalidades; }
            set { _finalidades = value; }
        }

        public Atividade() { }

        public Atividade(List<Finalidade> _finalidades)
        {
            Finalidades = _finalidades;
        }*/
    }
}
