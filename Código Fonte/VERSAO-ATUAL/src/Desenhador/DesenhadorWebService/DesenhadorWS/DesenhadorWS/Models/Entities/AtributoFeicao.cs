using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class AtributoFeicao 
    {
        public string Nome { get; set; }
        public object Valor { get; set; }
        public TipoAtributo Tipo { get; set; }
        public AtributoFeicao() { }
     
        public AtributoFeicao(TipoAtributo tipo, string nome, object valor)
        {
            Tipo = tipo;
            Nome = nome;
            Valor = valor;
        }

        public enum TipoAtributo { Manual = 0, Automatico, Sequencia };
        
    }
}
