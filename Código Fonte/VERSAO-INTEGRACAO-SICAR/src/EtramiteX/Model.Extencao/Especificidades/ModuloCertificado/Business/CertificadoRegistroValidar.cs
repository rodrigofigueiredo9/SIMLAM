using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertificado.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertificado.Business
{
	public class CertificadoRegistroValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		CertificadoRegistroDa _da = new CertificadoRegistroDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			CertificadoRegistro esp = especificidade as CertificadoRegistro;

			RequerimentoAtividade(esp);

			if (String.IsNullOrWhiteSpace(esp.Classificacao))
			{
				Validacao.Add(Mensagem.CertificadoRegistro.ClassificacaoObrigatoria);
			}

			if (String.IsNullOrWhiteSpace(esp.Registro))
			{
				Validacao.Add(Mensagem.CertificadoRegistro.RegistroObrigatorio);
			}

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "Certificado_Destinatario");

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			return Salvar(especificidade);
		}
	}
}