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
        public Mensagem msgCred1(int projetoDigital, int car) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Já existe uma Solicitação de inscrição no CAR na situação 'Em cadastro' ou 'Pendente' para esse Projeto Digital. Se deseja alterar alguma informação, faça as modificações necessárias no Projeto Digial nº {0} e reenvie a Solicitação de inscrição no CAR nº {1}.", projetoDigital, car) }; }
        public Mensagem msgCred2(int projetoDigital, int car) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Já existe uma Solicitação de inscrição no CAR na situação 'Em Cadastro' ou 'Pendente' para esse imóvel. Se deseja alterar alguma informação, faça as modificações necessárias no Projeto Digital nº {0} e reenvie a Solicitação de inscrição no CAR nº {1}.", projetoDigital, car) }; }
        public Mensagem msgCred3(int projetoDigital, int car) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Para o Projeto Digital nº {0}, já exite uma Solicitação de inscrição no CAR nº {1} na situação 'Válida' ou 'Substituído pelo título CAR'. Se você deseja fazer uma retificação é necessário cadastrar um novo Projeto Digital e cadastrar/enviar uma nova Solicitação de inscrição no CAR.", projetoDigital, car) }; }
        public Mensagem msgCred4() { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("Caso tenha ocorrido alteração de dados do proprietário/empreendimento/dominialidade após o envio da Solicitação de inscrição no CAR ao SICAR é necessário enviar uma nova Solicitação de inscrição no CAR.") }; }
        public Mensagem msgCred5() { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Não é possível cadastrar uma Solicitação de inscrição no CAR para esse imóvel visto que o CAR está em análise. Se deseja fazer uma retificação entre em contato com o IDAF.") }; }
        public Mensagem msgCred6() { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Não é possível cadastrar uma Solicitação de inscrição no CAR para esse imóvel visto que o CAR já foi analisado. Se deseja fazer uma retificação entre em contato com o IDAF.") }; }

        public Mensagem msgInst1() { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Já existe uma Solicitação de CAR na situação 'Em cadastro' para esse imóvel. Se deseja fazer uma retificação, aguarde o envio ao Sicar, altere as informações necessárias e envie uma nova Solicitação de inscrição no CAR. ") }; }
        public Mensagem msgInst2(int car) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Já existe uma Solicitação de inscrição no CAR na situação 'Pendente' para esse imóvel. Se deseja fazer uma retificação, altere as informações necessárias e reenvie a Solcitação de inscrição no CAR n° {0}.", car) }; }
        public Mensagem msgInst3() { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Não é possível cadastrar uma Solicitação de inscrição no CAR para esse empreendimento visto que o título CAR já foi emitido. Se deseja fazer uma retificação, encerre o título, exclua a caracterização do CAR, altere as informações necessárias e envie uma nova Solicitação de inscrição no CAR.") }; }
        public Mensagem msgInst4() { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = string.Format("Caso tenha ocorrido alteração de dados do proprietário/empreendimento/dominialidade após o envio da Solicitação de inscrição no CAR ao SICAR é necessário enviar uma nova Solicitação de inscrição no CAR.") }; }
        public Mensagem msgInst5() { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Não é possível cadastrar uma Solicitação de inscrição no CAR para esse empreendimento visto que o CAR está em análise. Se deseja fazer uma retificação exclua a caractetização do CAR, altere as informações necessárias e envie uma nova Solicitação de inscrição no CAR.") }; }
		public Mensagem msgInst6(int requerimento, int car) { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = string.Format("Para o Requerimento n° {0} já existe uma Solicitação de inscrição no CAR n° {1}. Se você deseja fazer uma retificação é necessário cadastrar um novo Requerimento e cadastrar/enviar uma nova Solicitação de inscrição no CAR.", requerimento, car) }; }
	}
}
