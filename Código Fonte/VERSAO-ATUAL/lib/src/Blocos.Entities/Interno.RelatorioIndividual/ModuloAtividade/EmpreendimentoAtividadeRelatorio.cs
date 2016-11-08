

using Tecnomapas.Blocos.Entities.Configuracao;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloAtividade
{
	public class EmpreendimentoAtividadeRelatorio : IListaValor
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public string Secao { get; set; }
		public int? Divisao { get; set; }
		public string Atividade { get; set; }

		public EmpreendimentoAtividadeRelatorio()
		{
		}

		public bool IsAtivo
		{
			get { return true; }
		}

		public string Texto
		{
			get { return Atividade; }
		}
	}
}