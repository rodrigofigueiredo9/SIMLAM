using System;
using System.Linq;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Business
{
	public class CredenciadoIntValidar : ICredenciadoIntValidar
	{
		ListaBus _busLista = new ListaBus();
		CredenciadoIntDa _da = new CredenciadoIntDa();
		HabilitarEmissaoCFOCFOCBus _busHabilitacaoCFOCFOC = new HabilitarEmissaoCFOCFOCBus();

		public bool AlterarSituacao(int id, int novaSituacao, string motivo)
		{
			if (novaSituacao == 3 && String.IsNullOrEmpty(motivo))
			{
				Validacao.Add(Mensagem.Credenciado.SituacaoMotivoObrigatorio(_busLista.CredenciadoSituacoes.Single(x => x.Id == novaSituacao).Texto));
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