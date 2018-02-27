using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural
{
    public class SolicitacaoListarFiltro
    {
        public bool IsCPF { get; set; }
        public bool IsCNPJ { get; set; }
        public bool IsSolicitacaoNumero { get; set; }
        public bool IsTituloNumero { get; set; }

        public Int64? SolicitacaoNumero { get; set; }
        public String DeclaranteNomeRazao { get; set; }
        public String DeclaranteCPFCNPJ { get; set; }
        public Int32 EmpreendimentoID { get; set; }
        public Int64? EmpreendimentoCodigo { get; set; }
        public String EmpreendimentoDenominador { get; set; }
        public Int32? Municipio { get; set; }
        public Int64? Requerimento { get; set; }
        public String AutorCPFCNPJ { get; set; }
        public string SolicitacaoTituloNumero { get; set; }

        private string _situacao;
        public string Situacao { get { return _situacao == "0" ? "" : _situacao; } set { _situacao = value; } }
        
        private string _situacaoSicar;
        public string SituacaoSicar { get { return _situacaoSicar == "0" ? "" : _situacaoSicar; } set { _situacaoSicar = value; } }

        public Int32? Origem { get; set; }

        private List<String> _situacoes = new List<String>();
        public List<String> Situacoes
        {
            get { return _situacoes; }
            set { _situacoes = value; }
        }

        private List<String> _situacoesSicar = new List<String>();
        public List<String> SituacoesSicar
        {
            get { return _situacoesSicar; }
            set { _situacoesSicar = value; }
        }

        private ProtocoloNumero _protocolo = new ProtocoloNumero();
        public ProtocoloNumero Protocolo
        {
            get { return _protocolo; }
            set { _protocolo = value; }
        }

        private TituloNumero _titulo = new TituloNumero();
        public TituloNumero Titulo
        {
            get { return _titulo; }
            set { _titulo = value; }
        }


        public string ResponsavelEmpreendimentoCPFCNPJ { get; set; }
    }
}