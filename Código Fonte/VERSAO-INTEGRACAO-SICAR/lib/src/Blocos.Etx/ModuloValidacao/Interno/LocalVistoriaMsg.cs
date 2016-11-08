using System;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.ITextSharpEtx;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{

    public partial class Mensagem
    {
        private static LocalVistoriaMsg _localMsg = new LocalVistoriaMsg();
        public static LocalVistoriaMsg LocalVistoria
        {
            get { return _localMsg; }
        }
    }
    public class LocalVistoriaMsg
    {

        public Mensagem SalvoSucesso { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Local de Vistoria salvo com sucesso." }; } }

        public Mensagem SetorObrigatorio { get { return new Mensagem() { Texto = "Setor é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "LocalVistoria_Setor" }; } }

        public Mensagem DiaSemanaObrigatorio { get { return new Mensagem() { Texto = "Dia da Semana é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "LocalVistoria_DiaSemana" }; } }

        public Mensagem HoraInicioObrigatorio { get { return new Mensagem() { Texto = "Hora de Início é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "LocalVistoria_HoraInicio" }; } }
        
        public Mensagem HoraFimObrigatorio { get { return new Mensagem() { Texto = "Hora de Fim é obrigatório.", Tipo = eTipoMensagem.Advertencia, Campo = "LocalVistoria_HoraFim" }; } }

        public Mensagem HoraInicioInvalida { get { return new Mensagem() { Texto = "Hora de Início fora do formato hh:mm.", Tipo = eTipoMensagem.Advertencia, Campo = "LocalVistoria_HoraInicio" }; } }

        public Mensagem HoraFimInvalida { get { return new Mensagem() { Texto = "Hora de Fim fora do formato hh:mm.", Tipo = eTipoMensagem.Advertencia, Campo = "LocalVistoria_HoraFim" }; } }

        public Mensagem HoraInicioFimNãoDeveCoincidir (string diaSemana) {
            return new Mensagem() { Texto = string.Format("Os intervalos de hora início e hora fim não devem coincidir para o mesmo dia da semana como na {0}.",diaSemana), Tipo = eTipoMensagem.Advertencia }; 
        }

        public Mensagem PeloMenosUmHorario { get { return new Mensagem() { Texto = "Pelo menos um horário tem que estar associado ao local selecionado.’", Tipo = eTipoMensagem.Advertencia }; } }

        public Mensagem JaExisteConfiguragacaoLocal { get { return new Mensagem() { Texto = "Já existe configuração para o local selecionado.’", Tipo = eTipoMensagem.Advertencia }; } }

        public Mensagem HoraInicialMenorHoraFinal(string diaSemana)
        {
            return new Mensagem() { Texto = string.Format("A Hora Inicial não pode ser menor ou igual a Hora Final como na {0}.", diaSemana), Tipo = eTipoMensagem.Advertencia };
        }

        public Mensagem PossuiHorarioAssociado(string diaSemana, string horaInicio, string horaFim)
        {
            return new Mensagem() { Texto = string.Format("O horário de {0} entre {1} e {2} já foi utilizado para solicitar E-PTV, não podendo ser excluído.", diaSemana, horaInicio, horaFim), Tipo = eTipoMensagem.Erro };
        }

    }
}
