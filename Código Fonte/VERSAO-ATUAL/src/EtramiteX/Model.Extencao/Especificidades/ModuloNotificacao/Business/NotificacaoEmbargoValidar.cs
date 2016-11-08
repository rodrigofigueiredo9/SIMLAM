using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloNotificacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloNotificacao.Business
{
	public class NotificacaoEmbargoValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		EspecificidadeDa _daEspecificidade = new EspecificidadeDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			NotificacaoEmbargo esp = especificidade as NotificacaoEmbargo;

			RequerimentoAtividade(esp);

			if (esp.AtividadeEmbargo == esp.Atividades[0].Id)
			{
				Validacao.Add(Mensagem.NotificacaoEmbargoMsg.AtividadeSolicitadaIgualEmbargada);
			}

			if (!_daEspecificidade.ValidarAtividadeSetor(esp.AtividadeEmbargo.GetValueOrDefault(), 21))
			{
				Validacao.Add(Mensagem.NotificacaoEmbargoMsg.AtividadeEmbargoSetorErrado);
			}

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatarios);

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			return Validacao.EhValido;
		}
	}
}