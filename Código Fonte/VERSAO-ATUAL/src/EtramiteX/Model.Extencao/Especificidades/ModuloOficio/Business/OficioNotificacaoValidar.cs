using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOficio;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOficio.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOficio.Business
{
	public class OficioNotificacaoValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		OficioNotificacaoDa _da = new OficioNotificacaoDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			OficioNotificacao esp = especificidade as OficioNotificacao;

			if (!RequerimentoAtividade(esp, false, false))
			{
				return false;
			}

			//Validacoes Especificas
			if (!_da.ProcDocPossuiItemPendenteReprovado(esp.ProtocoloReq.RequerimentoId, esp.ProtocoloReq.IsProcesso))
			{
				Validacao.Add(Mensagem.OficioNotificacao.RequerimentoSemPendencias);
			}

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario.GetValueOrDefault(), "Oficio_Destinatario");

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			return Salvar(especificidade);
		}
	}
}