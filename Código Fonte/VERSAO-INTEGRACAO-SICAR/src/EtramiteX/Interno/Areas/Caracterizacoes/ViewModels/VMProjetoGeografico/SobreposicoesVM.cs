using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico
{
	public class SobreposicoesVM
	{
		public DateTecno UltimaVerificacao { get; set; }
		public List<Sobreposicao> SobreposicaoIDAF { get; set; }
		public List<Sobreposicao> SobreposicaoGeoBases { get; set; }

		public bool MostarVerificar { get; set; }
		
		public SobreposicoesVM()
		{
			SobreposicaoIDAF = new List<Sobreposicao>();
			SobreposicaoGeoBases = new List<Sobreposicao>();
			MostarVerificar = true;
		}

		public SobreposicoesVM(Sobreposicoes sobreposicoes, bool mostarVerificar)
		{
			UltimaVerificacao = sobreposicoes.DataVerificacaoBanco;
			SobreposicaoIDAF = sobreposicoes.Itens.Where(x => x.Base == (int)eSobreposicaoBase.IDAF).ToList();
			SobreposicaoGeoBases = sobreposicoes.Itens.Where(x => x.Base == (int)eSobreposicaoBase.GeoBase).ToList();
			MostarVerificar = mostarVerificar;
		}
	}
}