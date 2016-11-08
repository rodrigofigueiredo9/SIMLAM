using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBeneficiamentoMadeira
{
	public class BeneficiamentoMadeiraBeneficiamento
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 Atividade { get; set; }
		public String VolumeMadeiraSerrar { get; set; }
		public String VolumeMadeiraProcessar { get; set; }
		public String EquipControlePoluicaoSonora { get; set; }

		public String Identificador { get; set; }

		private CoordenadaAtividade _coordenadaAtividade = new CoordenadaAtividade();
		public CoordenadaAtividade CoordenadaAtividade
		{
			get { return _coordenadaAtividade; }
			set { _coordenadaAtividade = value; }
		}

		private List<MateriaPrima> _materiasPrimasFlorestais = new List<MateriaPrima>();
		public List<MateriaPrima> MateriasPrimasFlorestais
		{
			get { return _materiasPrimasFlorestais; }
			set { _materiasPrimasFlorestais = value; }
		}
	}
}
