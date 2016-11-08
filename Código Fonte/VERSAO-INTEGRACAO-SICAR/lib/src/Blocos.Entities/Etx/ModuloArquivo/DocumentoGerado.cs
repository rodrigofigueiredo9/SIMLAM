using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloArquivo
{
    public class DocumentoGerado
    {
        public int Id { get; set; }
        public int Tipo { get; set; }
        public string Texto { get; set; }
        public List<Lista> Anexos = new List<Lista>();
    }
}
