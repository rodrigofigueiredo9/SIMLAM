using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Infracao
    {
        public Int32 FiscalizacaoId { get; set; }
        public Int32 Id { get; set; }
        public Int32 ClassificacaoId { get; set; }
        public String ClassificacaoTexto { get; set; }
        public Int32 TipoId { get; set; }
        public String TipoTexto { get; set; }
        public Int32 ItemId { get; set; }
        public String ItemTexto { get; set; }
        public Int32 ConfiguracaoId { get; set; }
        public String ConfiguracaoTid { get; set; }
        public Int32? SubitemId { get; set; }
        public String SubitemTexto { get; set; }
        public Int32? SerieId { get; set; }
        public Int32? CodigoReceitaId { get; set; }
        public Boolean? IsAutuada { get; set; }
        public Boolean? IsGeradaSistema { get; set; }
        public String ValorMulta { get; set; }
        public String NumeroAutoInfracaoBloco { get; set; }
        public String DescricaoInfracao { get; set; }
        public Boolean ConfigAlterou { get; set; }
        public Int32 FiscalizacaoSituacaoId { get; set; }

        private List<InfracaoCampo> _campos = new List<InfracaoCampo>();
        public List<InfracaoCampo> Campos
        {
            get { return _campos; }
            set { _campos = value; }
        }

        private List<InfracaoPergunta> _perguntas = new List<InfracaoPergunta>();
        public List<InfracaoPergunta> Perguntas
        {
            get { return _perguntas; }
            set { _perguntas = value; }
        }

        private DateTecno _dataLavraturaAuto = new DateTecno();
        public DateTecno DataLavraturaAuto
        {
            get { return _dataLavraturaAuto; }
            set { _dataLavraturaAuto = value; }
        }

        //private Arquivo.Arquivo _arquivo = new Arquivo.Arquivo();
        /*public Arquivo.Arquivo Arquivo
        {
            get { return _arquivo; }
            set { _arquivo = value; }
        }*/

        public Infracao()
        {
            this.ClassificacaoTexto =
            this.TipoTexto =
            this.ItemTexto =
            this.ConfiguracaoTid =
            this.SubitemTexto = string.Empty;
        }
    }
}
