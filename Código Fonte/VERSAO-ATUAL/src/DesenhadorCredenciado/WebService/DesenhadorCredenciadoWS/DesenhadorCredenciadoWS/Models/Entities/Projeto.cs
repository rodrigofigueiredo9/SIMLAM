using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class Projeto
    {
        public int Id { get; set; }
        public int TipoNavegador { get; set; }
        public int Empreendimento { get; set; }
    }
}