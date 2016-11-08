

using System.Collections.Generic;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso
{
	public class ConsultarInformacaoVM
	{
		public int Id { get; set; }
		public string LabelEnviadoPor { get; set; }
		public string ProcessoNumero { get; set; }
		public string ProcessoTipo { get; set; }
		public string ProcessoLocalizacao { get; set; }
		public string ProcessoEnviadoPor { get; set; }
		
		private List<InformacaoProtocoloVM> _informacoes = new List<InformacaoProtocoloVM>();
		public List<InformacaoProtocoloVM> Informacoes
		{
			get { return _informacoes; }
			set { _informacoes = value; }
		}

		public ConsultarInformacaoVM()
		{
			AddInformacao(new InformacaoProtocoloVM() { Texto = "Histórico de tramitação", Tipo = TipoInformacacao.Visualizar, Chave = "btnHistoricoTramit" });
			AddInformacao(new InformacaoProtocoloVM() { Texto = "Análise de Itens de Processo", Tipo = TipoInformacacao.PDF, Chave = "btnPdfAnalise" });
			AddInformacao(new InformacaoProtocoloVM() { Texto = "Documentos juntados/Processos apensados", Tipo = TipoInformacacao.PDF, Chave = "btnPdfDocumentosJuntados" });
			AddInformacao(new InformacaoProtocoloVM() { Texto = "Arquivamento", Tipo = TipoInformacacao.PDF, Chave = "btnPdfArquivamento" });
			AddInformacao(new InformacaoProtocoloVM() { Texto = "Recibo de Entrega de Título", Tipo = TipoInformacacao.PDF, Chave = "btnPdfEntrega" });
			AddInformacao(new InformacaoProtocoloVM() { Texto = "Registro de Recebimento", Tipo = TipoInformacacao.PDF, Chave = "btnPdfRecebimento", });
		}

		private void AddInformacao(InformacaoProtocoloVM informacao)
		{
			Informacoes.Add(informacao);
		}

		public void MostrarInformacao(int indice, bool mostrar, object valor =  null)
		{
			Informacoes[indice].Mostrar = mostrar;
			
			if (valor != null) { Informacoes[indice].Valor = valor; }
		}
	}
}