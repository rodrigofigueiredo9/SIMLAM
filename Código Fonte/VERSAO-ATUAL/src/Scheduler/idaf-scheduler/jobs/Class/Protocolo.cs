using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public class Protocolo : IProtocolo
    {
        public Int32? Id { get; set; }
        public String Tid { get; set; }
        public Int32 IdRelacionamento { get; set; }
        public Int32? Ano { get; set; }
        public Int32? NumeroProtocolo { get; set; }
        public String NumeroAutuacao { get; set; }
        public String Nome { get; set; }
        public Int32? Volume { get; set; }
        public Int32 SituacaoId { get; set; }
        public String SituacaoTexto { get; set; }
        public Boolean IsArquivado { get; set; }
        public Boolean IsProcesso { get; set; }
        public Int32 SetorId { get; set; }
        public Int32 SetorCriacaoId { get; set; }

        //Número completo
        public String Numero
        {
            get
            {
                if (NumeroProtocolo.HasValue && Ano.HasValue)
                {
                    return NumeroProtocolo.ToString() + "/" + Ano.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private ProtocoloTipo _tipo = new ProtocoloTipo();
        public ProtocoloTipo Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        private DateTecno _dataCadastro = new DateTecno();
        public DateTecno DataCadastro
        {
            get { return _dataCadastro; }
            set { _dataCadastro = value; }
        }

        private DateTecno _dataAutuacao = new DateTecno();
        public DateTecno DataAutuacao
        {
            get { return _dataAutuacao; }
            set { _dataAutuacao = value; }
        }

        private Pessoa _interessado = new Pessoa();
        public Pessoa Interessado
        {
            get { return _interessado; }
            set { _interessado = value; }
        }

        private Requerimento _requerimento = new Requerimento();
        public Requerimento Requerimento
        {
            get { return _requerimento; }
            set { _requerimento = value; }
        }

        private Fiscalizacao _fiscalizacao = new Fiscalizacao();
        public Fiscalizacao Fiscalizacao
        {
            get { return _fiscalizacao; }
            set { _fiscalizacao = value; }
        }

        private Empreendimento _empreendimento = new Empreendimento();
        public Empreendimento Empreendimento
        {
            get { return _empreendimento; }
            set { _empreendimento = value; }
        }

        //private ChecagemRoteiro _checagemRoteiro = new ChecagemRoteiro();
        /*public ChecagemRoteiro ChecagemRoteiro
        {
            get { return _checagemRoteiro; }
            set { _checagemRoteiro = value; }
        }*/

        private List<Atividade> _atividades = new List<Atividade>();
        public List<Atividade> Atividades
        {
            get { return _atividades; }
            set { _atividades = value; }
        }

        //private List<ResponsavelTecnico> _responsaveis = new List<ResponsavelTecnico>();
        /*public List<ResponsavelTecnico> Responsaveis
        {
            get { return _responsaveis; }
            set { _responsaveis = value; }
        }*/

        //private Arquivo.Arquivo _arquivo = new Arquivo.Arquivo();
        /*public Arquivo.Arquivo Arquivo
        {
            get { return _arquivo; }
            set { _arquivo = value; }
        }*/

        //private Funcionario _emposse = new Funcionario();
        /*public Funcionario Emposse
        {
            get { return _emposse; }
            set { _emposse = value; }
        }*/

        public bool PossuiPendencia { get; set; }

        public Protocolo() { }

       /* public Protocolo(ProtocoloNumero protocoloNumero)
        {
            Id = protocoloNumero.Id;
            NumeroProtocolo = protocoloNumero.Numero;
            Ano = protocoloNumero.Ano;
            IsProcesso = protocoloNumero.IsProcesso;
            if (protocoloNumero.Tipo.HasValue)
            {
                Tipo.Id = protocoloNumero.Tipo.Value;
                Tipo.Texto = protocoloNumero.TipoTexto;
            }
        }*/
    }
}
