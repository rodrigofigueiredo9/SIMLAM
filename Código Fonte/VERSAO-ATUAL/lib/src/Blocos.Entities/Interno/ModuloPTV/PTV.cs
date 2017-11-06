using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV
{
	public class PTV
	{
	

		public int TipoPessoa { get; set; }

		public int LocalVistoria { get; set; }

		public int Id { get; set; }
		public string Tid { get; set; }
		public int? NumeroTipo { get; set; }
		public Int64 Numero { get; set; }
		public DateTecno DataEmissao { get; set; }
		public int Situacao { get; set; }
		public string SituacaoTexto { get; set; }
		public DateTecno SituacaoData { get; set; }
		public string SituacaoMotivo { get; set; }
		public int Empreendimento { get; set; }
		public string EmpreendimentoTexto { get; set; }
		public int ResponsavelEmpreendimento { get; set; }
		public int? PartidaLacradaOrigem { get; set; }
		public string LacreNumero { get; set; }
		public string PoraoNumero { get; set; }
		public string ContainerNumero { get; set; }
		public int DestinatarioID { get; set; }
		public int? PossuiLaudoLaboratorial { get; set; }
		public int TransporteTipo { get; set; }
		public string VeiculoIdentificacaoNumero { get; set; }
		public int? RotaTransitoDefinida { get; set; }
		public string Itinerario { get; set; }
		public int? NotaFiscalApresentacao { get; set; }
		public string NotaFiscalNumero { get; set; }
		public DateTecno ValidoAte { get; set; }
		public DateTecno DataAtivacao { get; set; }
		public DateTecno DataCancelamento { get; set; }
		public int ResponsavelTecnicoId { get; set; }
		public string ResponsavelTecnicoNome { get; set; }
		public List<PTVProduto> Produtos { get; set; }
		public PTVProduto Produto { get; set; }
		public DestinatarioPTV Destinatario { get; set; }
		public int LocalEmissaoId { get; set; }
		public string LocalEmissaoTexto { get; set; }
		public int LocalVistoriaId { get; set; }
		public string LocalVistoriaTexto { get; set; }
		public int DataHoraVistoriaId { get; set; }
		public string DataHoraVistoriaTexto { get; set; }

        public DateTime DataVistoria { get; set; }

        public string NumeroDua { get; set; }
        public string CPFCNPJDUA { get; set; }

        public bool ExibeQtdKg { get; set; }

		public bool TemAssinatura { get; set; }

		public int Credenciado { get; set; }

		public List<Anexo> Anexos { get; set; }

        public string DeclaracaoAdicional { get; set; }

        public string EmpreendimentoSemDoc { get; set; }

        public string ResponsavelSemDoc { get; set; } 

		public PTV()
		{
			DataEmissao = new DateTecno();
			ValidoAte = new DateTecno();
			DataAtivacao = new DateTecno();
			SituacaoData = new DateTecno();
			Produtos = new List<PTVProduto>();
			Produto = new PTVProduto();
			Destinatario = new DestinatarioPTV();
			Anexos = new List<Anexo>();
		}
	}
}