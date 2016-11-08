using System;
using System.Linq;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloHabilitarEmissaoCFOCFOC.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business
{
    public class CredenciadoIntValidar
    {
        CredenciadoIntDa _da = new CredenciadoIntDa();
        HabilitarEmissaoCFOCFOCBus _busHabilitacaoCFOCFOC = new HabilitarEmissaoCFOCFOCBus();

        public bool AlterarSituacao(int id, int novaSituacao, string motivo)
        {
            if (novaSituacao == 3 && String.IsNullOrEmpty(motivo))
            {
				Validacao.Add(Mensagem.Credenciado.SituacaoMotivoObrigatorio(ListaCredenciadoBus.CredenciadoSituacoes.Single(x => x.Id == novaSituacao).Texto));
            }
            return Validacao.EhValido;
        }

        internal bool RegerarChave(CredenciadoPessoa credenciado)
        {

            if (credenciado.Situacao == (int)eCredenciadoSituacao.AguardandoAtivacao)
            {
                Validacao.Add(Mensagem.Credenciado.RegerarChaveAguardandoAtivacao);
            }

            return Validacao.EhValido;
        }
    }
}
