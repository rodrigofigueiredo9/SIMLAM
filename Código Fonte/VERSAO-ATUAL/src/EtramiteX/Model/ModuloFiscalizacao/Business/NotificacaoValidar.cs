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
