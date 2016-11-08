using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Data;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Business
{
    public class BarragemDispensaLicencaInternoBus
    {
        #region Propriedades

        BarragemDispensaLicencaInternoDa _da = new BarragemDispensaLicencaInternoDa();
        ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

        #endregion

        public BarragemDispensaLicenca ObterPorEmpreendimento(int empreendimentoInternoId, bool simplificado = false)
        {
            BarragemDispensaLicenca caracterizacao = null;
            try
            {
                caracterizacao = _da.ObterPorEmpreendimento(empreendimentoInternoId, simplificado: simplificado);

                if (caracterizacao.Autorizacao.Id > 0)
                {
                    caracterizacao.Autorizacao = _busArquivo.Obter(caracterizacao.Autorizacao.Id.GetValueOrDefault());
                }
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return caracterizacao;
        }

        public Caracterizacao Caracterizacao
        {
            get
            {
                return new Caracterizacao()
                {
                    Tipo = eCaracterizacao.BarragemDispensaLicenca
                };
            }
        }
    }
}
