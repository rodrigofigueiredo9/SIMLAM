using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
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
        public Boolean? ComInfracao { get; set; }
		public Boolean? IsGeradaSistema { get; set; }
		public String ValorMulta { get; set; }
		public String NumeroAutoInfracaoBloco { get; set; }
		public String DescricaoInfracao { get; set; }
		public Boolean ConfigAlterou { get; set; }
		public Int32 FiscalizacaoSituacaoId { get; set; }
        public string HoraConstatacao { get; set; }
        public int? ClassificacaoInfracao { get; set; }     //0-leve; 1-média; 2-grave; 3-gravíssima

        #region Penalidades

        public bool? PossuiAdvertencia { get; set; }
        public bool? PossuiMulta { get; set; }
        public bool? PossuiApreensao { get; set; }
        public bool? PossuiInterdicaoEmbargo { get; set; }

        private List<int> _idsOutrasPenalidades = new List<int>();
        public List<int> IdsOutrasPenalidades
        {
            get
            {
                return _idsOutrasPenalidades;
            }
            set
            {
                _idsOutrasPenalidades = value;
            }
        }
        
        #endregion Penalidades

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

		private Arquivo.Arquivo _arquivo = new Arquivo.Arquivo();
		public Arquivo.Arquivo Arquivo
		{
			get { return _arquivo; }
			set { _arquivo = value; }
		}

        private Enquadramento _enquadramentoInfracao = new Enquadramento();
        public Enquadramento EnquadramentoInfracao
        {
            get
            {
                return _enquadramentoInfracao;
            }
            set
            {
                _enquadramentoInfracao = value;
            }
        }

        private DateTecno _dataConstatacao = new DateTecno();
        public DateTecno DataConstatacao
        {
            get
            {
                return _dataConstatacao;
            }
            set
            {
                _dataConstatacao = value;
            }
        }

		public Infracao()
		{
			this.ClassificacaoTexto =
			this.TipoTexto =
			this.ItemTexto =
			this.ConfiguracaoTid =
			this.SubitemTexto = string.Empty;

            for (int i = 0; i < 4; i++)
            {
                this.IdsOutrasPenalidades.Add(0);
            }
		}
	}
}
