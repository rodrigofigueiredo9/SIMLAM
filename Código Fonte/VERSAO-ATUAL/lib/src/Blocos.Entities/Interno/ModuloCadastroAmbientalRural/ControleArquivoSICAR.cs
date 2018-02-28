using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural
{
    public class ControleArquivoSICAR
    {
        public Int32 Id { get; set; }
		public String Tid { get; set; }
        public Int32 EmpreendimentoId { get; set; }
		public String EmpreendimentoTid { get; set; }
        public Int32 SolicitacaoCarId { get; set; }
		public String SolicitacaoCarTid { get; set; }
        public eStatusArquivoSICAR SituacaoEnvio { get; set; }
        public eCARSolicitacaoOrigem SolicitacaoCarEsquema { get; set; }
        public String CodigoImovel { get; set; }
        public String InscricaoSicar { get; set; }
    }
}
