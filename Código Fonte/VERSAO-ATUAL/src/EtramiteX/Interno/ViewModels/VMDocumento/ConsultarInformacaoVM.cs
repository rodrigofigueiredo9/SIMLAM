

using System.Collections.Generic;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento
{
	public class ConsultarInformacaoVM
	{
		public int Id { get; set; }
		
		public string LabelEnviadoPor { get; set; }
		public string DocumentoNumero { get; set; }
		public string DocumentoTipo { get; set; }
		public string DocumentoLocalizacao { get; set; }
		public string DocumentoEnviadoPor { get; set; }

		private List<InformacaoProtocoloVM> _informacoes = new List<InformacaoProtocoloVM>();
		public List<InformacaoProtocoloVM> Informacoes
		{
			get { return _informacoes; }
			set { _informacoes = value; }
		}

		public ConsultarInformacaoVM()
		{
			AddInformacao(new InformacaoProtocoloVM() { Texto = "Histórico de tramitação", Tipo = TipoInformacacao.Visualizar, Chave = "btnHistoricoTramit" });
			AddInformacao(new InformacaoProtocoloVM() { Texto = "Análise de Itens de Documento", Tipo = TipoInformacacao.PDF, Chave = "btnPdfAnalise" });
			AddInformacao(new InformacaoProtocoloVM() { Texto = "Arquivamento", Tipo = TipoInformacacao.PDF, Chave = "btnPdfArquivamento" });
			AddInformacao(new InformacaoProtocoloVM() { Texto = "Recibo de Entrega de Título", Tipo = TipoInformacacao.PDF, Chave = "btnPdfEntrega" });
			AddInformacao(new InformacaoProtocoloVM() { Texto = "Registro de Recebimento", Tipo = TipoInformacacao.PDF, Chave = "btnPdfRecebimento", });
			AddInformacao(new InformacaoProtocoloVM() { Texto = "Comunicação Interna", Tipo = TipoInformacacao.PDF, Chave = "btnPdfCI", });
			AddInformacao(new InformacaoProtocoloVM() { Texto = "Ofício (Administrativo)", Tipo = TipoInformacacao.PDF, Chave = "btnPdfOficio", });
		}

		private void AddInformacao(InformacaoProtocoloVM informacao)
		{
			Informacoes.Add(informacao);
		}

		public void MostrarInformacao(int indice, bool mostrar, object valor = null)
		{
			Informacoes[indice].Mostrar = mostrar;

			if (valor != null) { Informacoes[indice].Valor = valor; }
		}
	}
}