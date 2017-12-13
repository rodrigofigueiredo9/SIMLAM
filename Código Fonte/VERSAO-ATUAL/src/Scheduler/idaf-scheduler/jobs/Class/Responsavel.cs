using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Responsavel
    {
        [Key]
        public int InternoId { get; set; }
        [Key]
        public Int32? Id { get; set; }
        public Int32? IdRelacionamento { get; set; }
        [Display(Name = "Nome/Razão social", Order = 1)]
        public String NomeRazao { get; set; }
        [Display(Name = "CPF/CNPJ", Order = 2)]
        public String CpfCnpj { get; set; }
        public Int32? Tipo { get; set; }
        [Display(Name = "Tipo", Order = 3)]
        public String TipoTexto { get; set; }
        public String DataVencimentoTexto { get; set; }
        public DateTime? DataVencimento { get; set; }
        public String EspecificarTexto { get; set; }
        public String Tid { get; set; }

        public Int32 Origem { get; set; }
        public String OrigemTexto { get; set; }
        public Int32? CredenciadoUsuarioId { get; set; }
        public bool IsCopiado { get; set; }
    }
}
