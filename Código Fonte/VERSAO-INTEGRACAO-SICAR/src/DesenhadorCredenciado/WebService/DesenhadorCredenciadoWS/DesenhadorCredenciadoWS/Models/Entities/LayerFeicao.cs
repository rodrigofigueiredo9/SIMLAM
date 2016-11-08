using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tecnomapas.DesenhadorWS.Models.Entities
{
    public class LayerFeicao 
    {
        public int Id { get; set; }
        public string Nome { get; set; }
		public string Descricao { get; set; }
        public string Tabela { get; set; }
        public string Schema { get; set; }
        public TipoGeometriaFeicao TipoGeometria { get; set; }
        public string Sequencia { get; set; }
        public string ColunaPK { get; set; }
        public int IdLayer { get; set; }
        public string NomeLayer { get; set; }
        public bool Visivel { get; set; }
        public int Quantidade { get; set; }
        public int ServicoId { get; set; }
        public string ServicoUrlMxd { get; set; }
        public bool ServicoIsPrincipal { get; set; }
        public int Categoria { get; set; }
        public bool IsFinalizada { get; set; }

        #region Alterações      
        public bool Selecionavel { get; set; }
       
        public LayerFeicao()
        {
            Colunas = new List<ColunaLayerFeicao>();
            IsFinalizada = false;
        }
        #endregion

        public List<ColunaLayerFeicao> Colunas { get; set; }
	}
}
