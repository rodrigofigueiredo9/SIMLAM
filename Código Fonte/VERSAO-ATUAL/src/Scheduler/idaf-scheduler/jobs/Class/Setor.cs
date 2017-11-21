using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Setor : IListaValor
    {
        public int Id { get; set; }
        public String Nome { get; set; }
        public String Texto { get { return Nome; } }
        public string HierarquiaCabecalho { get; set; }
        public String Sigla { get; set; }
        public int? Responsavel { get; set; }
        public bool EhResponsavel { get; set; }
        public bool IsAtivo { get; set; }
        public int IdRelacao { get; set; }
        public int TramitacaoTipoId { get; set; }
        public String SiglaComNome { get { return String.Format("{0} - {1}", Sigla, Nome); } }
        public String UnidadeConvenio { get; set; }

        private List<FuncionarioLst> _funcionarios = new List<FuncionarioLst>();
        public List<FuncionarioLst> Funcionarios
        {
            get { return _funcionarios; }
            set { _funcionarios = value; }
        }
    }
}
