using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertificado.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertificado.Business
{
	class CertificadoCadastroProdutoAgrotoxicoValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		CertificadoCadastroProdutoAgrotoxicoDa _daEspecificidade = new CertificadoCadastroProdutoAgrotoxicoDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			CertificadoCadastroProdutoAgrotoxico esp = especificidade as CertificadoCadastroProdutoAgrotoxico;

			Destinatario(esp.Titulo.Protocolo.Id, esp.DestinatarioId, "Certificado_Destinatario");

			if (esp.AgrotoxicoId <= 0)
			{
				Validacao.Add(Mensagem.CertificadoCadastroProdutoAgrotoxico.AgrotoxicoObrigatorio);
			}
			else
			{
				Agrotoxico agrotoxico = _daEspecificidade.ObterAgrotoxico(esp.AgrotoxicoId, simplificado: true);

				if (esp.AgrotoxicoTid != agrotoxico.Tid)
				{
					Validacao.Add(Mensagem.CertificadoCadastroProdutoAgrotoxico.AgrotoxicoDesatualizado);
				}
				else
				{
					if (!_daEspecificidade.ProcessoPossuiSEP(esp.ProtocoloReq.Id, agrotoxico.NumeroProcessoSep))
					{
						Validacao.Add(Mensagem.CertificadoCadastroProdutoAgrotoxico.NumeroSEPAlterado);
					}

					if (!agrotoxico.CadastroAtivo)
					{
						Validacao.Add(Mensagem.CertificadoCadastroProdutoAgrotoxico.AgrotoxicoInativo);
					}
				}
			}

			return Validacao.EhValido;

		}

		public bool Emitir(IEspecificidade especificidade)
		{
			return Salvar(especificidade);
		}
	}
}
