using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public interface IProtocolo
    {
        Int32? Id { get; set; }
        String Tid { get; set; }
        Int32 IdRelacionamento { get; set; }
        Int32? Ano { get; set; }
        Int32? NumeroProtocolo { get; set; }
        String NumeroAutuacao { get; set; }
        Int32? Volume { get; set; }
        Int32 SituacaoId { get; set; }
        String SituacaoTexto { get; set; }
        Boolean IsArquivado { get; set; }
        Boolean IsProcesso { get; set; }
        Int32 SetorId { get; set; }
        Int32 SetorCriacaoId { get; set; }
        String Numero { get; }
        ProtocoloTipo Tipo { get; set; }
        DateTecno DataCadastro { get; set; }
        DateTecno DataAutuacao { get; set; }
        Pessoa Interessado { get; set; }
        Requerimento Requerimento { get; set; }
        /*Fiscalizacao Fiscalizacao { get; set; }
        Empreendimento Empreendimento { get; set; }
        ChecagemRoteiro ChecagemRoteiro { get; set; }
        List<Atividade> Atividades { get; set; }
        List<ResponsavelTecnico> Responsaveis { get; set; }
        Arquivo.Arquivo Arquivo { get; set; }
        Funcionario Emposse { get; set; }
    */}
}
