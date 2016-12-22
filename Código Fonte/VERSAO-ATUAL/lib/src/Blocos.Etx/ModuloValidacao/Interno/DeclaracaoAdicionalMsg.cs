using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
    public partial class Mensagem
    {
        private static DeclaracaoAdicionalMsg _declaracao = new DeclaracaoAdicionalMsg();
        public static DeclaracaoAdicionalMsg Declaracao
        {
            get { return _declaracao; }
        }
    }

    public class DeclaracaoAdicionalMsg
    {
        public Mensagem TextoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Texto é obrigatório.", Campo = "DeclaracaoAdicional.Texto" }; } }
        public Mensagem OutroEstadoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Selecione a opção 'Outro Estado?'.", Campo = "DeclaracaoAdicional.Texto" }; } }
        public Mensagem ObjetoNulo { get { return new Mensagem() { Tipo = eTipoMensagem.Erro, Texto = "Objeto está nulo." }; } }
        public Mensagem DeclaracaoSalvaComSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Declaração salva com sucesso." }; } }
        public Mensagem DeclaracaoExcluidaComSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Declaração excluída com sucesso." }; } }

        public Mensagem MensagemExcluir(string numero)
        {
            return new Mensagem() { Texto = String.Format("Tem certeza que deseja excluir a declaração {0}?", numero) };
        }
       
    }
}