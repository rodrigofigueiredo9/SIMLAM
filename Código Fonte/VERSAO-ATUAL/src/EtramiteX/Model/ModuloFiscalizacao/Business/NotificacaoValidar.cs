using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class NotificacaoValidar
	{
		private enum eFormaNotificacao
		{
			Pessoa = 1,
			AR = 2,
			PorEdital = 3
		}

		public bool Salvar(Notificacao notificacao)
		{
			if (notificacao.FormaIUF == 0)
				Validacao.Add(Mensagem.NotificacaoMsg.FormaIUFObrigatorio);

			if (notificacao.FormaIUF != (int)eFormaNotificacao.PorEdital && !notificacao.DataIUF.IsValido)
				Validacao.Add(Mensagem.NotificacaoMsg.DataIUFObrigatorio);

			if (notificacao.FormaJIAPI > 0 && !notificacao.DataJIAPI.IsValido)
				Validacao.Add(Mensagem.NotificacaoMsg.DataJIAPIObrigatorio);

			if (notificacao.FormaCORE > 0 && !notificacao.DataCORE.IsValido)
				Validacao.Add(Mensagem.NotificacaoMsg.DataCOREObrigatorio);

			if(notificacao.DataIUF.Data > DateTime.Now)
				Validacao.Add(Mensagem.NotificacaoMsg.DataIUFFutura);

			if (notificacao.DataJIAPI.IsValido)
			{
				if (notificacao.DataJIAPI.Data > DateTime.Now)
					Validacao.Add(Mensagem.NotificacaoMsg.DataJIAPIFutura);

				if(notificacao.DataJIAPI.Data < notificacao.DataIUF.Data)
					Validacao.Add(Mensagem.NotificacaoMsg.DataJIAPIAnteriorIUF);
			}

			if (notificacao.DataCORE.IsValido)
			{
				if (notificacao.DataCORE.Data > DateTime.Now)
					Validacao.Add(Mensagem.NotificacaoMsg.DataCOREFutura);

				if (notificacao.DataCORE.Data < notificacao.DataJIAPI.Data)
					Validacao.Add(Mensagem.NotificacaoMsg.DataCOREAnteriorJIAPI);
			}

			if (notificacao.FormaIUF == (int)eFormaNotificacao.AR || notificacao.FormaJIAPI == (int)eFormaNotificacao.AR ||
				notificacao.FormaCORE == (int)eFormaNotificacao.AR)
			{
				if ((notificacao.Anexos?.Count ?? 0) == 0)
					Validacao.Add(Mensagem.NotificacaoMsg.ArquivoObrigatorio);
			}

			return Validacao.EhValido;
		}
	}
}
