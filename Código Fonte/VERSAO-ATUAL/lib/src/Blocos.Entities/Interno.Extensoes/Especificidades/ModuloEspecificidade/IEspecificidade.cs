using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade
{
	public interface IEspecificidade
	{
		string Json { get; set; }
		object Objeto { get; set; }
		int RequerimentoId { get; set; }

		TituloEsp Titulo { get; set; }
		List<ProcessoAtividadeEsp> Atividades { get; set; }
		ProtocoloEsp ProtocoloReq { get; set; }
		eAtividadeCodigo AtividadeCaractTipo { get; set; }
		object DadosEspAtivCaractObj { get; set; }
	}
}