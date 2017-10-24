using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class MaterialApreendido
	{
		public Int32 Id { get; set; }
		public Int32 FiscalizacaoId { get; set; }
		public Int32? SerieId { get; set; }
        public String SerieTexto { get; set; }
        public Boolean? IsDigital { get; set; }
		public Boolean? IsTadGeradoSistema { get; set; }
        public String NumeroIUF { get; set; }
		public String Descricao { get; set; }
		public String ValorProdutos { get; set; }
        public String NumeroLacre { get; set; }
		public String Opiniao { get; set; }
		public Int32 FiscalizacaoSituacaoId { get; set; }
		public String Tid { get; set; }

		private DateTecno _dataLavratura = new DateTecno();
		public DateTecno DataLavratura
		{
			get { return _dataLavratura; }
			set { _dataLavratura = value; }
		}

        //deletar
		private List<MaterialApreendidoMaterial> _materiais = new List<MaterialApreendidoMaterial>();
		//deletar
        public List<MaterialApreendidoMaterial> Materiais
		{
			get { return _materiais; }
			set { _materiais = value; }
		}
        
        private List<ApreensaoProdutoApreendido> _produtosapreendidos = new List<ApreensaoProdutoApreendido>();
        public List<ApreensaoProdutoApreendido> ProdutosApreendidos
        {
            get
            {
                return _produtosapreendidos;
            }
            set
            {
                _produtosapreendidos = value;
            }
        }

		private MaterialApreendidoDepositario _depositario = new MaterialApreendidoDepositario();
		public MaterialApreendidoDepositario Depositario
		{
			get { return _depositario; }
			set { _depositario = value; }
		}

		public Arquivo.Arquivo _arquivo = new Arquivo.Arquivo();
		public Arquivo.Arquivo Arquivo
		{
			get { return _arquivo; }
			set { _arquivo = value; }
		}
	}
}
