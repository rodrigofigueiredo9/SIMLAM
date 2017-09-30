using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{/*
    public partial class Mensagem
    {
        private static PadraoMsg _padraoMsg = new PadraoMsg();

        public static PadraoMsg Padrao
        {
            get { return _padraoMsg; }
        }
    }

    public class PadraoMsg
    {
        public Mensagem Inexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Objeto inexistente." }; } }
        public Mensagem SemPermissao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Você não tem permissão para acessar essa funcionalidade." }; } }
        public Mensagem NaoEncontrouRegistros { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Registro não encontrado." }; } }

        public Mensagem DataObrigatoria(string campo, string tipoData)
        {
            return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("Data de {0} é obrigatória.", tipoData) };
        }

        public Mensagem DataInvalida(string campo, string tipoData)
        {
            return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("Data de {0} inválida.", tipoData) };
        }

        public Mensagem DataIgualMenorAtual(string campo, string tipoData)
        {
            return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("Data de {0} deve ser menor ou igual a data atual.", tipoData) };
        }

        public Mensagem DataIgualAtual(string campo, string tipoData)
        {
            return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("Data de {0} deve ser igual a data atual.", tipoData) };
        }

        public Mensagem AreaObrigatoria(string campo, string areaNome)
        {
            if (String.IsNullOrEmpty(areaNome))
            {
                return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Área é obrigatória." };
            }
            else
            {
                return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("{0} é obrigatória.", areaNome) };
            }
        }

        public Mensagem AreaMaiorZero(string campo, string areaNome)
        {
            if (String.IsNullOrEmpty(areaNome))
            {
                return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Área deve ser maior que zero." };
            }
            else
            {
                return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("{0} deve ser maior que zero.", areaNome) };
            }
        }

        public Mensagem AreaInvalida(string campo, string areaNome)
        {
            if (String.IsNullOrEmpty(areaNome))
            {
                return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Área inválida." };
            }
            else
            {
                return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = String.Format("{0} inválida.", areaNome) };
            }
        }
    }*/
}
