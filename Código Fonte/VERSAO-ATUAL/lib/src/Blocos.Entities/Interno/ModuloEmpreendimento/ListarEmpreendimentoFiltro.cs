using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento
{
	public class ListarEmpreendimentoFiltro
	{
		public Int32 Credenciado { get; set; }
		public Int32? Segmento { get; set; }
		public String SegmentoTexto { get; set; }
		public String Denominador { get; set; }
		public String CNPJ { get; set; }
		public Int64? Codigo { get; set; }
		public String CnpjEmpreemdimento { get; set; }
		public Int32? EstadoId { get; set; }
		public Int32? MunicipioId { get; set; }
		public String AreaAbrangencia { get; set; }
		public Responsavel Responsavel { get; set; }
		public EmpreendimentoAtividade Atividade { get; set; }
		public Coordenada Coordenada { get; set; }

		private Boolean _possuiCodigo = true;
		public Boolean PossuiCodigo
		{
			get { return _possuiCodigo; }
			set { _possuiCodigo = value; }
		}
		
		private Boolean _possuiCnpj = true;
		public Boolean PossuiCnpj
		{
			get { return _possuiCnpj; }
			set { _possuiCnpj = value; }
		}

		private ProtocoloNumero _protocolo = new ProtocoloNumero();
		public ProtocoloNumero Protocolo
		{
			get { return _protocolo; }
			set { _protocolo = value; }
		}

		public ListarEmpreendimentoFiltro()
		{
			Responsavel = new Responsavel();
			Atividade = new EmpreendimentoAtividade();
			Coordenada = new Coordenada();
		}

        public int EmpreendimentoCompensacao { get; set; }
	}
}