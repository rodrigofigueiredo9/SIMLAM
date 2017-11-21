using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Requerimento
    {
        public int Id { get; set; }
        public int ProjetoDigitalId { get; set; }
        public int CredenciadoId { get; set; }
        public bool IsCredenciado { get; set; }
        public int IdRelacionamento { get; set; }
        public int Numero { get { return Id; } }
        public int Checagem { get; set; }
        public String Tid { get; set; }
        public DateTime DataCadastro { get; set; }
        public int ProtocoloId { get; set; }
        public int ProtocoloTipo { get; set; }
        public int AgendamentoVistoria { get; set; }
        public int SetorId { get; set; }

        public string DataCadastroTexto { get { return DataCadastro.ToShortDateString(); } }

        public int SituacaoId { get; set; }
        public String SituacaoTexto { get; set; }

        public String Informacoes { get; set; }

        public Pessoa Interessado { get; set; }
        public Int32 AutorId { get; set; }

        public String Origem
        {
            get
            {
                return IsRequerimentoDigital ? "Credenciado" : "Institucional";
            }
        }

        public Boolean IsRequerimentoDigital
        {
            get
            {
                return AutorId > 0;
            }
        }

        public Int32 EtapaImportacao { get; set; }

        public Empreendimento Empreendimento { get; set; }
        public ProjetoDigital ProjetoDigital { get; set; }
        //public List<Roteiro> Roteiros { get; set; }
        public List<Atividade> Atividades { get; set; }
        //public List<ResponsavelTecnico> Responsaveis { get; set; }
        public List<Pessoa> Pessoas { get; set; }

        /*public Requerimento()
        {
            Empreendimento = new Empreendimento();
            ProjetoDigital = new ProjetoDigital();
            Roteiros = new List<Roteiro>();
            Atividades = new List<Atividade>();
            Responsaveis = new List<ResponsavelTecnico>();
            Pessoas = new List<Pessoa>();
            Interessado = new Pessoa();
        }*/
    }
}
