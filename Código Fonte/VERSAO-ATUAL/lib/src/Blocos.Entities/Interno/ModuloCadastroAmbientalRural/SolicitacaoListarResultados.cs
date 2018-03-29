using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural
{
	public class SolicitacaoListarResultados
	{
		public Int32 Id { get; set; }
		public String Numero { get; set; }
		public String Ano { get; set; }
		public Int32 EmpreendimentoID { get; set; }
		public String EmpreendimentoDenominador { get; set; }
		public Int64 EmpreendimentoCodigo { get; set; }
		public String MunicipioTexto { get; set; }
		public Int32 SituacaoID { get; set; }
		public String SituacaoTexto { get; set; }
		public String SituacaoMotivo { get; set; }
        public Int32 SituacaoArquivoCarID { get; set; }
        public String SituacaoArquivoCarTexto { get; set; }
        public String UrlPdfReciboSICAR { get; set; }
		public int InternoId { get; set; }
		public int CredenciadoId { get; set; }
		public eCARSolicitacaoOrigem Origem { get; set; }

		public String NumeroTexto
		{

			get
			{
				if (!String.IsNullOrWhiteSpace(Ano))
				{
					return Numero + "/" + Ano;
				}

				return Numero;
			}
		}

		public Boolean IsCredenciado
		{
			get
			{
				return CredenciadoId > 0;
			}
		}

		private Boolean _isTitulo = false;
		public Boolean IsTitulo
		{
			get { return _isTitulo; }
			set { _isTitulo = value; }
		}

		public string ArquivoSICAR { get; set; }
	}
}