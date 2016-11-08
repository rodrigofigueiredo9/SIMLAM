using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSecagemMecanicaGraos
{
	public class SecagemMecanicaGraos
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public Int32 Atividade { get; set; }
		public String NumeroSecadores { get; set; }

		public List<Dependencia> Dependencias { get; set; }

		private CoordenadaAtividade _coordenadaAtividade = new CoordenadaAtividade();
		public CoordenadaAtividade CoordenadaAtividade
		{
			get { return _coordenadaAtividade; }
			set { _coordenadaAtividade = value; }
		}

		private List<Secador> _secadores = new List<Secador>();
		public List<Secador> Secadores
		{
			get { return _secadores; }
			set { _secadores = value; }
		}

		private List<MateriaPrima> _materiasPrimasFlorestais = new List<MateriaPrima>();
		public List<MateriaPrima> MateriasPrimasFlorestais
		{
			get { return _materiasPrimasFlorestais; }
			set { _materiasPrimasFlorestais = value; }
		}

	}
}
