using System;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao
{
	public class HistoricoVME
	{
		public String StrHistoricoTramitacao { get; set; }

		public Int32 Id { get; set; }
		public Int32 HistoricoId { get; set; }

		public Int32 SituacaoId { get; set; }
		public String Remetente { get; set; }
		public String Destinatario { get; set; }
		public String Objetivo { get; set; }
		public String Acao { get; set; }

		public Boolean MostrarPdf { get; set; }

		private DateTecno _acaoData = new DateTecno();
		public DateTecno AcaoData
		{
			get { return _acaoData; }
			set { _acaoData = value; }
		}

		private Setor _remetenteSetor = new Setor();
		public Setor RemetenteSetor
		{
			get { return _remetenteSetor; }
			set { _remetenteSetor = value; }
		}

		private Setor _destinatarioSetor = new Setor();
		public Setor DestinatarioSetor
		{
			get { return _destinatarioSetor; }
			set { _destinatarioSetor = value; }
		}

		public Boolean IsProcesso { get; set; }
	}
}