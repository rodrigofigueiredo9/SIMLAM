using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloPulverizacaoProduto
{
	public class PulverizacaoProduto
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public Int32 Atividade { get; set; }
		public String EmpresaPrestadora { get; set; }
		public String CNPJ { get; set; }

		public List<Dependencia> Dependencias { get; set; }

		private List<Cultura> _culturas = new List<Cultura>();
		public List<Cultura> Culturas
		{
			get { return _culturas; }
			set { _culturas = value; }
		}

		private CoordenadaAtividade _coordenadaAtividade = new CoordenadaAtividade();
		public CoordenadaAtividade CoordenadaAtividade
		{
			get { return _coordenadaAtividade; }
			set { _coordenadaAtividade = value; }
		}

	}
}
