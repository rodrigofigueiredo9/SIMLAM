using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Endereco
    {
        [Key]
        public Int32? Id { get; set; }
        public Int32? ZonaLocalizacaoId { get; set; }
        [Display(Name = "Zona de localização", Order = 1)]
        public String ZonaLocalizacaoTexto
        {
            get
            {
                switch (ZonaLocalizacaoId)
                {
                    case 1:
                        return "Urbana";

                    case 2:
                        return "Rural";

                    default:
                        return string.Empty;
                }
            }
        }

        public Int32? ArtefatoId { get; set; }
        public Int32? Correspondencia { get; set; }

        [Display(Name = "CEP", Order = 2)]
        public String Cep { get; set; }
        public String CaixaPostal { get; set; }
        [Display(Order = 3)]
        public String Logradouro { get; set; }
        [Display(Order = 4)]
        public String Bairro { get; set; }
        public Int32 EstadoId { get; set; }
        [Display(Name = "Estado", Order = 7)]
        public String EstadoTexto { get; set; }
        public Int32 MunicipioId { get; set; }
        [Display(Name = "Municipio", Order = 8)]
        public String MunicipioTexto { get; set; }
        [Display(Order = 4)]
        public String Numero { get; set; }
        [Display(Order = 9)]
        public String Corrego { get; set; }
        [Display(Order = 10)]
        public String Complemento { get; set; }
        public String Detalhes { get; set; }
        public String Fone { get; set; }
        public String Fax { get; set; }
        public String Tid { get; set; }
        [Display(Name = "Distrito", Order = 5)]
        public String DistritoLocalizacao { get; set; }

        public Endereco()
        {

        }
    }
}
