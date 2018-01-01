using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
    public class Penalidade : Lista
    {
        public String SituacaoTexto
        {
            get
            {
                return IsAtivo ? "Ativado" : "Desativado";
            }
        }

        public string Artigo { get; set;}
        public string Item { get; set; }
        public string Descricao { get; set; }
        public int Ativo { get; set; }

        private List<Penalidade> _itens = new List<Penalidade>();
        public List<Penalidade> Itens
        {
            get { return _itens; }
            set { _itens = value; }
        }

    }
}
