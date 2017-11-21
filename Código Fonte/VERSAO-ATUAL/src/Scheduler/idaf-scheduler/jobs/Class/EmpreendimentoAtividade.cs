using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class EmpreendimentoAtividade : IListaValor
    {
        public int Id { get; set; }
        public string Tid { get; set; }
        public string Secao { get; set; }
        public int? Divisao { get; set; }
        [Display(Name = "Atividade", Order = 5)]
        public string Atividade { get; set; }
        public string CNAE { get; set; }

        public EmpreendimentoAtividade() { }

        public bool IsAtivo
        {
            get { return true; }
        }

        public string Texto
        {
            get { return Atividade; }
        }
    }
}
