using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Business
{
	public class LaudoFundiarioSimplificadoValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		public bool Salvar(IEspecificidade especificidade)
		{
			LaudoFundiarioSimplificado esp = especificidade as LaudoFundiarioSimplificado;

			RequerimentoAtividade(esp, solicitado: false);

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "Laudo_Destinatario");

			ValidacoesGenericasBus.DataMensagem(esp.DataVistoria, "Laudo_DataVistoria_DataTexto", "vistoria");

			if (String.IsNullOrWhiteSpace(esp.Objetivo))
			{
				Validacao.Add(Mensagem.LaudoFundiarioSimplificadoMsg.ObjetivoObrigatorio);
			}

			if (String.IsNullOrWhiteSpace(esp.ParecerDescricao))
			{
				Validacao.Add(Mensagem.LaudoFundiarioSimplificadoMsg.DescricaoParecerObrigatoria);
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