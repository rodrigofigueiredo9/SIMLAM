using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloPapel;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado
{
    public class HistoricoEmissaoCFOCFOC
    {
        public int Id { get; set; }
        public String Tid { get; set; }
        public String SituacaoData { get; set; }
        public Int32 Situacao { get; set; }
        public String SituacaoTexto { get; set; }
        public Int32? Motivo { get; set; }
        public String MotivoTexto { get; set; }
        public String NumeroProcesso { get; set; }
        public String HistoricoData { get; set; }


        public HistoricoEmissaoCFOCFOC()
        {

        }
    }
}