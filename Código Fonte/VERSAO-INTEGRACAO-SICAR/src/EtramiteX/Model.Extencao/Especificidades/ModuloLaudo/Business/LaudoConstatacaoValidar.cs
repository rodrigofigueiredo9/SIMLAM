using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Business
{
	public class LaudoConstatacaoValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		LaudoConstatacaoDa _da = new LaudoConstatacaoDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			LaudoConstatacao esp = especificidade as LaudoConstatacao;

			RequerimentoAtividade(esp, false, true);

			if (esp.Destinatario == 0)
			{
				Validacao.Add(Mensagem.Especificidade.DestinatarioObrigatorio("Laudo_Destinatario"));
			}
		
			if (String.IsNullOrWhiteSpace(esp.Objetivo))
			{
				Validacao.Add(Mensagem.LaudoConstatacao.ObjetivoObrigatorio);
			}
			else if (esp.Objetivo.Length > 500)
			{
				Validacao.Add(Mensagem.LaudoConstatacao.ObjetivoMuitoGrande);
			}

			if (String.IsNullOrWhiteSpace(esp.Constatacao))
			{
				Validacao.Add(Mensagem.LaudoConstatacao.ConstatacaoObrigatorio);
			}

			if (!esp.DataVistoria.Data.HasValue)
			{
				Validacao.Add(Mensagem.LaudoConstatacao.DataVistoriaObrigatoria);
			}
			else 
			{
				if (!esp.DataVistoria.IsValido)
				{
					Validacao.Add(Mensagem.LaudoConstatacao.DataVistoriaIvalida);
				}
				else if (esp.DataVistoria.Data > DateTime.Now)
				{
					Validacao.Add(Mensagem.LaudoConstatacao.DataVistoriaMaior);
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