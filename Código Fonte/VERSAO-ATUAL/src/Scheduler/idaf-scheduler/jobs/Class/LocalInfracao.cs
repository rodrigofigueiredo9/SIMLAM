using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class LocalInfracao
    {
        public int Id { get; set; }
        public int FiscalizacaoId { get; set; }
        public int SetorId { get; set; }
        public DateTecno Data { get; set; }//Data de Vistoria
        public int SistemaCoordId { get; set; }
        public int Datum { get; set; }
        public int Fuso { get { return 24; } }
        public string AreaAbrangencia { get; set; }
        public string LatNorthing { get; set; }
        public string LonEasting { get; set; }
        public decimal? LatNorthingToDecimal { get { return this.ToDecimal(this.LatNorthing); } }
        public decimal? LonEastingToDecimal { get { return this.ToDecimal(this.LonEasting); } }
        public int Hemisferio { get; set; }
        public int MunicipioId { get; set; }
        public string MunicipioTexto { get; set; }
        public string Local { get; set; }
        public int? PessoaId { get; set; }
        public string PessoaTid { get; set; }
        public int? EmpreendimentoId { get; set; }
        public string EmpreendimentoTid { get; set; }
        public int? ResponsavelId { get; set; }
        public int? ResponsavelPropriedadeId { get; set; }
        public string Tid { get; set; }

        public LocalInfracao()
        {
            this.Data = new DateTecno();
            this.AreaAbrangencia =
            this.LatNorthing =
            this.LonEasting =
            this.MunicipioTexto =
            this.Local =
            this.Tid = string.Empty;
        }

        internal decimal? ToDecimal(string strValor)
        {
            decimal decimalValor = 0;

            if (string.IsNullOrWhiteSpace(strValor))
            {
                return null;
            }

            if (!decimal.TryParse(strValor, out decimalValor))
            {
                return null;
            }

            return decimalValor;
        }
    }
}
