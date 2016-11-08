using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLicenca;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Business
{
	public class LicencaAmbientalRegularizacaoValidar : EspecificidadeValidarBase, IEspecificiadeValidar
	{
		EspecificidadeDa _daEspecificidade = new EspecificidadeDa();

		public bool Salvar(IEspecificidade especificidade)
		{
			LicencaAmbientalRegularizacao esp = especificidade as LicencaAmbientalRegularizacao;

			RequerimentoAtividade(esp, faseAnterior: true);

			Destinatario(especificidade.ProtocoloReq.Id, esp.Destinatario, "Licenca_Destinatario");

			ValidarTituloGenericoAtividadeCaracterizacao(especificidade, eEspecificidade.LicencaAmbientalRegularizacao);

			return Validacao.EhValido;
		}

		public bool Emitir(IEspecificidade especificidade)
		{
			Salvar(especificidade);

			return Validacao.EhValido;
		}
	}
}