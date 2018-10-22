﻿using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.Blocos.Entities.Model.Business;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class ExploracaoFlorestalAutorizacaoPDF
	{
		public String TipoExploracao { set; get; }
		public String TotalPoligono { set; get; }

		private List<ExploracaoFlorestalAutorizacaoDetalhePDF> _detalhe = new List<ExploracaoFlorestalAutorizacaoDetalhePDF>();
		public List<ExploracaoFlorestalAutorizacaoDetalhePDF> Detalhe
		{
			get { return _detalhe; }
			set { _detalhe = value; }
		}

		public ExploracaoFlorestalAutorizacaoPDF() { }

		public ExploracaoFlorestalAutorizacaoPDF(ExploracaoFlorestal exploracaoFlorestal)
		{
			var exploracoesFirst = exploracaoFlorestal.Exploracoes.FirstOrDefault() ?? new ExploracaoFlorestalExploracao();
			int auxFinalidades = (exploracoesFirst.FinalidadeExploracao.HasValue) ? exploracoesFirst.FinalidadeExploracao.Value : 0;
			TipoExploracao = exploracaoFlorestal.TipoExploracaoTexto;
			TotalPoligono = exploracaoFlorestal.Exploracoes.Sum(x => x.AreaCroqui).ToString();
		}
	}
}
