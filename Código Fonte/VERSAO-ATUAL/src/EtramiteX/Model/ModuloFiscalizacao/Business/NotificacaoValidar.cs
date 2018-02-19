﻿using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class NotificacaoValidar
	{
		public bool Salvar(Notificacao notificacao)
		{
			if (notificacao.FormaIUF == 0)
				Validacao.Add(Mensagem.NotificacaoMsg.FormaIUFObrigatorio);

			if (notificacao.FormaIUF > 0 && !notificacao.DataIUF.IsValido)
				Validacao.Add(Mensagem.NotificacaoMsg.DataIUFObrigatorio);

			if (notificacao.FormaJIAPI > 0 && !notificacao.DataJIAPI.IsValido)
				Validacao.Add(Mensagem.NotificacaoMsg.DataJIAPIObrigatorio);

			if (notificacao.FormaCORE > 0 && !notificacao.DataCORE.IsValido)
				Validacao.Add(Mensagem.NotificacaoMsg.DataCOREObrigatorio);

			return Validacao.EhValido;
		}
	}
}
