using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro
{
	public class PTVOutro
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public Int64 Numero { get; set; }
		public DateTecno DataEmissao { get; set; }
		public string DataEmissaoText { get { return this.DataEmissao.DataTexto; } set { this.DataEmissao.DataTexto = value; } }
		public DateTecno DataAtivacao { get; set; }
		public string DataAtivacaoText { get { return this.DataAtivacao.DataTexto; } set { this.DataAtivacao.DataTexto = value; } }
		public DateTecno DataCancelamento { get; set; }
		public string DataCancelamentoText { get { return this.DataCancelamento.DataTexto; } set { this.DataCancelamento.DataTexto = value; } }
		public int Situacao { get; set; }
		public string SituacaoTexto { get; set; }
		public string Interessado { get; set; }
		public string InteressadoCnpjCpf { get; set; }
		public string InteressadoEndereco { get; set; }
		public int InteressadoEstadoId { get; set; }
		public string InteressadoEstadoTexto { get; set; }
        public int ProducaoTipo { get; set; }
		public int InteressadoMunicipioId { get; set; }
		public string InteressadoMunicipioTexto { get; set; }
		public int DestinatarioID { get; set; }
		public DestinatarioPTV Destinatario { get; set; }
		public DateTecno ValidoAte { get; set; }
		public string ValidoAteText { get { return this.ValidoAte.DataTexto; } set { this.ValidoAte.DataTexto = value; } }
		public string RespTecnico { get; set; }
		public string RespTecnicoNumHab { get; set; }
		public int Estado { get; set; }
		public string EstadoTexto { get; set; }
		public int Municipio { get; set; }
		public string MunicipioTexto { get; set; }
		public List<PTVOutroProduto> Produtos { get; set; }
        public int? CredenciadoId { get; set; }
        public string DeclaracaoAdicional { get; set; }
        public string DeclaracaoAdicionalHtml { get; set; }
        public List<Praga> Pragas { get; set; }

		public PTVOutro()
		{
			DataEmissao = new DateTecno();
			DataAtivacao = new DateTecno();
			DataCancelamento = new DateTecno();
			ValidoAte = new DateTecno();
			Produtos = new List<PTVOutroProduto>();
			Destinatario = new DestinatarioPTV();
            Pragas = new List<Praga>();

            Tid =
            SituacaoTexto =
            Interessado =
            InteressadoCnpjCpf =
            InteressadoEstadoTexto =
            InteressadoMunicipioTexto =
            RespTecnico =
            RespTecnicoNumHab =
            EstadoTexto =
            MunicipioTexto =
            DeclaracaoAdicional =
            DeclaracaoAdicionalHtml = "";
		}
	}
}
