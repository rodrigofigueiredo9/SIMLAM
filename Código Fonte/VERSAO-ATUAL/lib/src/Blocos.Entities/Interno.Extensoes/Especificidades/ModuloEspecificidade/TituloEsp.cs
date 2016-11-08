using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade
{	
	public class TituloEsp
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public string Modelo { get; set; }
		public int? EmpreendimentoId { get; set; }
		public string EmpreendimentoTexto { get; set; }
		public int? Prazo { get; set; }
		public string PrazoUnidade { get; set; }
		public int? DiasProrrogados { get; set; }
		public int SituacaoId { get; set; }
		public string SituacaoTexto { get; set; }
		public int? MotivoEncerramentoId { get; set; }
		public int? AlterarSituacaoAcao { get; set; }
		public int? RequerimentoAtividades { get; set; }
		public int? RepresentanteId { get; set; }
		public string LocalEmissao { get; set; }
		public string ModeloSigla { get; set; }

		public int SetorId { get; set; }
		public bool ExisteAnexos { get; set; }

		private List<Anexo> _anexos = new List<Anexo>();
		public List<Anexo> Anexos
		{
			get { return _anexos; }
			set { _anexos = value; }
		}

		private ProtocoloEsp _processoDocumento = new ProtocoloEsp();
		public ProtocoloEsp Protocolo 
		{
			get { return _processoDocumento; }
			set { _processoDocumento = value; }
		}

		private TituloNumeroEsp _numero = new TituloNumeroEsp();
		public TituloNumeroEsp Numero
		{
			get { return _numero; }
			set { _numero = value; }
		}
	}
}