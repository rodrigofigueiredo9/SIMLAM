using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo
{
	public class TermoCPFARLC : Especificidade
	{
		public int ID { get; set; }
		public string TID { get; set; }
		public int DominialidadeID { get; set; }
		public string DominialidadeTID { get; set; }
		public int CedenteDominioID { get; set; }
		public int ReceptorEmpreendimentoID { get; set; }
		public string ReceptorEmpreendimentoDenominador { get; set; }
		public int ReceptorDominioID { get; set; }

		public List<ReservaLegal> CedenteARLCompensacao { get; set; }
		public List<Responsavel> CedenteResponsaveisEmpreendimento { get; set; }
		public List<Responsavel> ReceptorResponsaveisEmpreendimento { get; set; }

		public TermoCPFARLC()
		{
			CedenteARLCompensacao = new List<ReservaLegal>();
			CedenteResponsaveisEmpreendimento = new List<Responsavel>();
			ReceptorResponsaveisEmpreendimento = new List<Responsavel>();
		}
	}
}