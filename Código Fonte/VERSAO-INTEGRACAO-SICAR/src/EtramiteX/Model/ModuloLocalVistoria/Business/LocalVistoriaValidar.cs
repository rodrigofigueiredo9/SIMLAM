using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.ModuloLocalVistoria;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLocalVistoria.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloLocalVistoria.Business
{
    class LocalVistoriaValidar
    {

        #region Propriedades

        LocalVistoriaDa _local = new LocalVistoriaDa();
                

        #endregion
        internal bool Salvar(LocalVistoria local)
        {
            if (string.IsNullOrEmpty(local.SetorTexto))
            {
                Validacao.Add(Mensagem.LocalVistoria.SetorObrigatorio);
            }


            if (local.DiasHorasVistoria.Count < 0)
            {
                    Validacao.Add(Mensagem.LocalVistoria.PeloMenosUmHorario);
            }


           
            foreach (DiaHoraVistoria dia in local.DiasHorasVistoria)
            {
                if (string.IsNullOrEmpty(dia.DiaSemanaTexto))
                {
                    Validacao.Add(Mensagem.LocalVistoria.DiaSemanaObrigatorio);
                }

                if (string.IsNullOrEmpty(dia.HoraInicio))
                {
                    Validacao.Add(Mensagem.LocalVistoria.HoraInicioObrigatorio);
                }

                if (dia.HoraInicio.Length !=5)
                {
                    Validacao.Add(Mensagem.LocalVistoria.HoraInicioInvalida);
                }


                if (dia.HoraFim.Length != 5)
                {
                    Validacao.Add(Mensagem.LocalVistoria.HoraFimInvalida);
                }
            }

           
            int identDiaSemana = -1;
            int horaI = 0;
            int minI = 0;
            int horaF = 0;
            int minF = 0;
            int ultimaHora = 0;
            int ultimoMin = 0;
            LocalVistoria localOrdenado = OrdenarDiaHoraVistoria(local);

            foreach (DiaHoraVistoria diaOrdenado in localOrdenado.DiasHorasVistoria)
            {
                if (string.IsNullOrEmpty(diaOrdenado.DiaSemanaTexto))
                {
                    Validacao.Add(Mensagem.LocalVistoria.DiaSemanaObrigatorio);
                }

                if (string.IsNullOrEmpty(diaOrdenado.HoraInicio))
                {
                    Validacao.Add(Mensagem.LocalVistoria.HoraInicioObrigatorio);
                }

                if (string.IsNullOrEmpty(diaOrdenado.HoraFim))
                {
                    Validacao.Add(Mensagem.LocalVistoria.HoraFimObrigatorio);
                }


                horaI = Int32.Parse(diaOrdenado.HoraInicio.Substring(0, 2));
                minI = Int32.Parse(diaOrdenado.HoraInicio.Substring(3, 2));
                horaF = Int32.Parse(diaOrdenado.HoraFim.Substring(0, 2));
                minF = Int32.Parse(diaOrdenado.HoraFim.Substring(3, 2));

                if (!((horaI<horaF) ||((horaI==horaF) && (minI<minF))))
                {
                    //hora inicial menor ou hora igual minuto inicial menor
                    Validacao.Add(Mensagem.LocalVistoria.HoraInicialMenorHoraFinal(diaOrdenado.DiaSemanaTexto));
                }


                if (diaOrdenado.Situacao == 1)
                {

                    if (identDiaSemana != diaOrdenado.DiaSemanaId)
                    {
                        identDiaSemana = diaOrdenado.DiaSemanaId;
                        ultimaHora = horaF;
                        ultimoMin = minF;
                    }
                    else
                    {
                        if ((horaI < ultimaHora) || ((horaI == ultimaHora) && (minI <= ultimoMin)))
                        {
                            Validacao.Add(Mensagem.LocalVistoria.HoraInicioFimNãoDeveCoincidir(diaOrdenado.DiaSemanaTexto));
                        }
                    }
                }

            }

            return Validacao.EhValido;

        }

        internal bool Excluir(int id)
        {
            return Validacao.EhValido;
        }




        #region Auxiliares

        public LocalVistoria OrdenarDiaHoraVistoria(LocalVistoria local)
        {
            LocalVistoria novoLocal = local;
            IEnumerable<DiaHoraVistoria> ordenado;
            ordenado = local.DiasHorasVistoria.OrderBy(c => c.DiaSemanaId).ThenBy(c => c.HoraInicio);
            novoLocal.DiasHorasVistoria = ordenado.ToList<DiaHoraVistoria>();
            return novoLocal;
        }

        #endregion

    }
}
