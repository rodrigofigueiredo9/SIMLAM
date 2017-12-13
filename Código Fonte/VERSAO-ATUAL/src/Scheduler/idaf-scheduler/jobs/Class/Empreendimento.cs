using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Empreendimento
    {
        //Propriedades somente do Credenciado
        [Key]
        public int? InternoId { get; set; }
        public String InternoTid { get; set; }
        public int? CredenciadoId { get; set; }
        public bool IsCredenciado { get; set; }
        public int SelecaoTipo { get; set; }

        [Key]
        public Int32 Id { get; set; }
        public String Tid { get; set; }
        [Display(Name = "Código", Order = 0)]
        public Int64? Codigo { get; set; }
        public Int32? Segmento { get; set; }
        [Display(Name = "Segmento", Order = 2)]
        public String SegmentoTexto { get; set; }
        [Display(Order = 1)]
        public String SegmentoDenominador { get; set; }
        [Display(Order = 3)]
        public String CNPJ { get; set; }
        public EmpreendimentoAtividade Atividade { get; set; }
        [Display(Name = "Nome Fantasia", Order = 4)]
        public String NomeFantasia { get; set; }
        [Display]
        public String Denominador { get; set; }

        //responsável
        public String NomeRazao { get; set; }
        public String CpfCnpj { get; set; }
        public Int32 Tipo { get; set; }
        public String DataVencimento { get; set; }

        private Int32 _temCorrespondencia = 0;
        private List<Responsavel> _responsaveis = new List<Responsavel>();
        private List<Endereco> _enderecos = new List<Endereco>();
        private Coordenada _coordenada = new Coordenada();
        private List<Contato> _meios = new List<Contato>();

        [Display(Name = "Possui Correspondência", Order = 5)]
        public String CorrespondenciaTexto { get { return TemCorrespondencia > 0 ? "Sim" : "Não"; } }

        public Int32 TemCorrespondencia
        {
            get { return _temCorrespondencia; }
            set { _temCorrespondencia = value; }
        }
        public List<Responsavel> Responsaveis
        {
            get { return _responsaveis; }
            set { _responsaveis = value; }
        }

        public String ResponsaveisStrag { get; set; }

        public List<Endereco> Enderecos
        {
            get { return _enderecos; }
            set { _enderecos = value; }
        }
        public Coordenada Coordenada
        {
            get { return _coordenada; }
            set { _coordenada = value; }
        }
        public List<Contato> MeiosContatos
        {
            get { return _meios; }
            set { _meios = value; }

        }

        public Empreendimento()
        {
            Atividade = new EmpreendimentoAtividade();
        }
    }
}
