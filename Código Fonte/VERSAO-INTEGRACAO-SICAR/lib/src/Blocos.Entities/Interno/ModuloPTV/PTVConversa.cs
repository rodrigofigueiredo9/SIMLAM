using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV
{
    public class PTVConversa
    {

        public int Id { get; set; }
        public string Tid { get; set; }
        public DateTecno DataConversa { get; set; }
        public string Texto { get; set; }
        public int ArquivoId { get; set; }
        public string ArquivoNome { get; set; }
        public int TipoComunicador { get; set; }
        public string NomeComunicador { get; set; }
        public int FuncionarioId { get; set; }
        public int CredenciadoId { get; set; }

        public PTVConversa()
        {
            DataConversa = new DateTecno();
            Id = 0;
            ArquivoId = 0;
            FuncionarioId = 0;
            CredenciadoId = 0;
        }
    }
}
