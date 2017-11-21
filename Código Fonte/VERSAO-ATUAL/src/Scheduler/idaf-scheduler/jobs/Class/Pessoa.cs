using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Pessoa
    {
        //Propriedades somente do Credenciado
        [Key]
        public int? InternoId { get; set; }
        public int? CredenciadoId { get; set; }
        public bool IsCredenciado { get; set; }
        public int SelecaoTipo { get; set; }
        public bool IsCopiado { get; set; }
        public bool IsValidarConjuge { get; set; }
        //Propriedades somente do Credenciado

        private Endereco _endereco = new Endereco();
        private Fisica _fisica = new Fisica();
        private Juridica _juridica = new Juridica();
        private List<Contato> _meios = new List<Contato>();

        [Key]
        public Int32 Id { get; set; }
        public int? IdRelacionamento { get; set; }

        public Int32? Ativa { get; set; }
        public Int32 Tipo { get; set; }
        public String Escopo { get; set; }
        public String Tid { get; set; }
        public String NomeFantasia { get; set; }

        public Fisica Fisica
        {
            get { return _fisica; }
            set { _fisica = value; }
        }
        public Juridica Juridica
        {
            get { return _juridica; }
            set { _juridica = value; }
        }
        [Display(Name = "Nome/Razão Social", Order = 1)]
        public String NomeRazaoSocial
        {
            get { return IsFisica ? Fisica.Nome : Juridica.RazaoSocial; }
            set { if (IsFisica) Fisica.Nome = value; else Juridica.RazaoSocial = value; }
        }
        [Display(Name = "CPF/CNPJ", Order = 2)]
        public String CPFCNPJ
        {
            get { return IsFisica ? Fisica.CPF : Juridica.CNPJ; }
            set { if (IsFisica) Fisica.CPF = value; else Juridica.CNPJ = value; }
        }

        public String RGIE
        {
            get { return IsFisica ? Fisica.RG : Juridica.IE; }
        }
        public List<Contato> MeiosContatos
        {
            get { return _meios; }
            set { _meios = value; }

        }
        public Endereco Endereco
        {
            get { return _endereco; }
            set { _endereco = value; }
        }

        public bool IsFisica
        {
            get { return (Tipo == 1); }
        }
        public bool IsJuridica
        {
            get { return (Tipo == 2); }
        }

        public Pessoa()
        {
            Tipo = 1;
        }
    }
}
