using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
    public partial class Mensagem
    {
        private static RetificacaoMsg _RetificacaoMsg = new RetificacaoMsg();
        public static RetificacaoMsg Retificacao
        {
            get { return _RetificacaoMsg; }
        }
    }
    public class RetificacaoMsg
    {
        public Mensagem msgCred1(string projetoDigital, string car) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("Já existe uma Solicitação de inscrição no CAR na situação 'Em cadastro' ou 'Pendente' para esse Projeto Digital. Se deseja alterar alguma informação, faça as modificações necessárias no Projeto Digial nº {0} e reenvie a Solicitação de inscrição no CAR nº {1}.", projetoDigital, car) }; }
        public Mensagem msgCred2(string projetoDigital, string car) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("Já existe uma Solicitação de inscrição no CAR na situação 'Em Cadastro' ou 'Pendente' para esse imóvel. Se desejja alterar alguma informação, faça as modificações necessárias no Projeto Digital nº {0} e reenvie a Solicitação de inscrição no CAR nº.", projetoDigital, car) }; }
        public Mensagem msgCred3(string projetoDigital, string car) { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("Para o Projeto Digital nº {0}, já exite uma Solicitação de inscrição no CAR nº {1} na situação 'Válida' ou 'Substituído pelo título CAR'. Se você deseja fazer uma retificação é necessário cadastrar um novo Projeto Digital e cadastrar/enviar uma nova Solicitação de inscrição no CAR.", projetoDigital, car) }; }
        public Mensagem msgCred4() { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("Caso tenha ocorrido alteração de dados do proprietário/empreendimento/dominialidade após o envio da Solicitação de inscrição no CAR ao SICAR é necessário enviar uma nova Solicitação de inscrição no CAR.") }; }
        public Mensagem msgCred5() { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("Não é possível cadastrar uma Solicitação de inscrição no CAR para esse imóvel visto que o CAR está em análise. Se deseja fazer uma retificação entre em contato com o IDAF.") }; }
        public Mensagem msgCred6() { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("Não é possível cadastrar uma Solicitação de inscrição no CAR para esse imóvel visto que o CAR já foi analisado. Se deseja fazer uma retificação entre em contato com o IDAF.") }; }

    }
}
