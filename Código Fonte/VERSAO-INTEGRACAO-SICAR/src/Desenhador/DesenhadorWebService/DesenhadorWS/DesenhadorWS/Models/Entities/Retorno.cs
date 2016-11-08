using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class Retorno
    {
        public string Mensagem { get; set; }
        public bool Sucesso { get; set; }
        public int Objectid { get; set; }
        public int IdLayerFeicao { get; set; }

        public Retorno() { }
        public Retorno(bool sucesso)
        {
            Sucesso = sucesso;
            if (!sucesso)
                Mensagem = "Erro";
        }
        public Retorno(bool sucesso, string mensagem)
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
        }
        public Retorno(bool sucesso, string mensagem, int objectid, int idLayerFeicao)
            : this(sucesso, mensagem)
        {
            Objectid = objectid;
            IdLayerFeicao = idLayerFeicao;
        }
    }
}