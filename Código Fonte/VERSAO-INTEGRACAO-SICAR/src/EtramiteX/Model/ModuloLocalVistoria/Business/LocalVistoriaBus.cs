using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLocalVistoria.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloLocalVistoria;
using System.Web;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using System.Web.Mvc;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;


namespace Tecnomapas.EtramiteX.Interno.Model.ModuloLocalVistoria.Business
{
    public class LocalVistoriaBus
    {

        #region Propriedades

        LocalVistoriaDa _da = new LocalVistoriaDa();
        LocalVistoriaValidar _validar = new LocalVistoriaValidar();

        private static EtramiteIdentity User
        {
            get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
        }

        #endregion

        #region Ações DML

        public bool Salvar(LocalVistoria local)
        {
            try
            {
                if (_validar.Salvar(local))
                {
                    GerenciadorTransacao.ObterIDAtual();

                    using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
                    {
                        bancoDeDados.IniciarTransacao();

                        _da.Salvar(local, bancoDeDados);

                        bancoDeDados.Commit();

                        Validacao.Add(Mensagem.LocalVistoria.SalvoSucesso);
                    }
                }


            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return Validacao.EhValido;
        }


        public LocalVistoria Obter(int setor, BancoDeDados banco = null)
        {
            LocalVistoria local = null;
            try
            {
                using (BancoDeDados bancoDedados = BancoDeDados.ObterInstancia(banco))
                {
                    local = _validar.OrdenarDiaHoraVistoria(_da.Obter(setor, null));
                }

            }
            catch (Exception exc)
            {

                Validacao.AddErro(exc);
            }
            return local;
        }



        public bool VerificaPodeExcluir(DiaHoraVistoria DiaHoraVistoria)
        {
            int QtdHorariosAssociados = _da.PossuiHorarioAssociado(DiaHoraVistoria.Id);
            //QtdHorariosAssociados = 1;
            if (QtdHorariosAssociados > 0)
            {
                Validacao.Add(Mensagem.LocalVistoria.PossuiHorarioAssociado(DiaHoraVistoria.DiaSemanaTexto, DiaHoraVistoria.HoraInicio, DiaHoraVistoria.HoraFim));
            }

            return Validacao.EhValido;
        }
        #endregion

        public List<Setor> SetoresAgrupadorTecnico()
        {
            List<Setor> lstSetor = null;
            try
            {
                    lstSetor = _da.ObterSetorAgrupadorTecnico();
            }
            catch (Exception exc)
            {

                Validacao.AddErro(exc);
            }
            return lstSetor;
        }

        public Resultados<LocalVistoriaListar> Filtrar(LocalVistoriaListar filtrosListar, Paginacao paginacao)
        {
            try
            {
                Filtro<LocalVistoriaListar> filtro = new Filtro<LocalVistoriaListar>(filtrosListar, paginacao);
                Resultados<LocalVistoriaListar> resultados = _da.Filtrar(filtro);

                if (resultados.Quantidade < 1)
                {
                    Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
                }

                return resultados;
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return null;
        }


    }
}
