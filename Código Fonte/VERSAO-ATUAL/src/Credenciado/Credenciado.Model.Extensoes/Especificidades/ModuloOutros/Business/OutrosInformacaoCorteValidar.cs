using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloOutros.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloOutros.Business
{
	public class OutrosInformacaoCorteValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		OutrosInformacaoCorteDa _da = new OutrosInformacaoCorteDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			var esp = especificidade as OutrosInformacaoCorte;

			DeclaratorioRequerimentoAtividade(esp);

			if (esp.VinculoPropriedade <= 0)
			{
				Validacao.Add(Mensagem.CertidaoDispensaLicenciamentoAmbiental.VinculoPropriedadeObrigatorio);
			}

			/*if (esp.VinculoPropriedade == (int)eCertidaoDispLicAmbVinculoProp.Outros && string.IsNullOrWhiteSpace(esp.VinculoPropriedadeOutro))
			{
				Validacao.Add(Mensagem.CertidaoDispensaLicenciamentoAmbiental.VinculoPropriedadeOutroObrigatorio);
			}*/

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			return Validacao.EhValido;
		}
	}
}
