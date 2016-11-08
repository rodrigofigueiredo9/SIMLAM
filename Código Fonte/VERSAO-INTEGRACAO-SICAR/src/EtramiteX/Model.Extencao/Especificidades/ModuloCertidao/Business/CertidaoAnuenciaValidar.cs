using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertidao.Business
{
	public class CertidaoAnuenciaValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade especificidade)
		{
			CertidaoAnuencia esp = especificidade as CertidaoAnuencia;

			RequerimentoAtividade(esp, false, true);
			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatarios);

			if (String.IsNullOrWhiteSpace(esp.Certificacao)) 
			{
				Validacao.Add(Mensagem.CertidaoAnuencia.CertificacaoObrigatoria);
			}

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			return Validacao.EhValido;
		}
	}
}