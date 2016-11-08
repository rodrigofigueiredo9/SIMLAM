using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Business
{
    public class ListaCredenciadoBus
    {
        static ListaCredenciadoDa _da = new ListaCredenciadoDa();

        public static List<Roteiro> RoteiroPadrao
        {
            get
            {
                List<String> valores = _da.BuscarRoteiroPadrao();

                List<Roteiro> roteiros = new List<Roteiro>();

                if (valores != null && valores.Count > 0)
                {
                    foreach (String valor in valores)
                    {
                        roteiros.Add(new Roteiro() { Id = Convert.ToInt32(valor.Split('@')[0]), Setor = Convert.ToInt32(valor.Split('@')[1]) });
                    }
                }
                return roteiros;
            }
        }

        public static List<Situacao> SituacoesRequerimento
        {
            get
            {
                return _da.ObterCredenciadoSituacoesRequerimento();
            }
        }

        public static List<ContatoLst> ListarMeiosContato
        {
            get
            {
                return _da.ObterMeiosContato();
            }
        }


    }
}
