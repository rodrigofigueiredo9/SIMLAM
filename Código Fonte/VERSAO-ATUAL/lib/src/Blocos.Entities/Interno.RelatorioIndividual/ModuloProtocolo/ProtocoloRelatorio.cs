using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloProtocolo
{
	public class ProtocoloRelatorio
	{
		public string Titulo { get; set; }
		public string Nome { get; set; }
		public string Numero { get; set; }
		public int ProtocoloProcDoc { get; set; }
		public string ProtocoloTexto { get; set; }
		public int ProtocoloTipo { get; set; }		
		public string TipoTexto { get; set; }
		public string Data { get; set; }
		public int SetorCriacaoId { get; set; }
		public string Setor { get; set; }
		public int SetorId { get; set; }
		public string Executor { get; set; }
		public string AtividadeFinalidadesTitulos { get; set; }
		public string ChecagemNumero { get; set; }
		public string RequerimentoNumero { get; set; }
		public string FiscalizacaoNumero { get; set; }
		public string OrgaoMunicipio { get; set; }
		public string OrgaoUF { get; set; }
		public string UsuarioNome { get; set; }
		public string UsuarioCargo { get; set; }
		public string ProtocoloAssociadoTipo { get; set; }
		public string ProtocoloAssociadoNumero { get; set; }
		public string Destinatario { get; set; }
		public string SetorDestinatario { get; set; }
		public string Assunto { get; set; }
		public string Descricao { get; set; }

		public EmpreendimentoRelatorio Empreendimento { get; set; }
		public PessoaRelatorio Interessado { get; set; }

		private List<ProtocoloRelatorio> _protocolosAssociados = new List<ProtocoloRelatorio>();
		public List<ProtocoloRelatorio> ProtocolosAssociados
		{
			get { return _protocolosAssociados; }
			set { _protocolosAssociados = value; }
		}
	}
}