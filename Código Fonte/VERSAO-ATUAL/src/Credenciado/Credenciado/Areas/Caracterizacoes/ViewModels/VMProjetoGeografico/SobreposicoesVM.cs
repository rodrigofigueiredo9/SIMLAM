using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMProjetoGeografico
{
	public class SobreposicoesVM
	{
		public DateTecno UltimaVerificacao { get; set; }
		public List<Sobreposicao> SobreposicaoIDAF { get; set; }
		public List<Sobreposicao> SobreposicaoGeoBases { get; set; }

		public bool MostrarVerificar { get; set; }

		public SobreposicoesVM()
		{
			SobreposicaoIDAF = new List<Sobreposicao>();
			SobreposicaoGeoBases = new List<Sobreposicao>();
			MostrarVerificar = true;
		}

		public SobreposicoesVM(Sobreposicoes sobreposicoes, bool mostrarVerificar)
		{
			UltimaVerificacao = sobreposicoes.DataVerificacaoBanco;
			SobreposicaoIDAF = sobreposicoes.Itens.Where(x => x.Base == (int)eSobreposicaoBase.IDAF).ToList();
			SobreposicaoGeoBases = sobreposicoes.Itens.Where(x => x.Base == (int)eSobreposicaoBase.GeoBase).ToList();
			MostrarVerificar = mostrarVerificar;
		}
	}
}
