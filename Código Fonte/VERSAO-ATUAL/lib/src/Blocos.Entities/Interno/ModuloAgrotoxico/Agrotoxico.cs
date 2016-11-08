using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico
{
	public class Agrotoxico
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public bool PossuiCadastro { get; set; }
		public int NumeroCadastro { get; set; }
		public bool CadastroAtivo { get; set; }
		public string MotivoTexto { get; set; }
		public int? MotivoId { get; set; }
		public string NomeComercial { get; set; }
		public long NumeroRegistroMinisterio { get; set; }
		public long NumeroProcessoSep { get; set; }
		public string ObservacaoInterna { get; set; }
		public string ObservacaoGeral { get; set; }

		#region ClassificacaoToxicologica

		private ConfiguracaoVegetalItem _classificacaoToxicologica = new ConfiguracaoVegetalItem();

		public ConfiguracaoVegetalItem ClassificacaoToxicologica
		{
			get { return _classificacaoToxicologica; }
			set { _classificacaoToxicologica = value; }
		}

		#endregion

		#region PericulosidadeAmbiental

		private ConfiguracaoVegetalItem _periculosidadeAmbiental = new ConfiguracaoVegetalItem();

		public ConfiguracaoVegetalItem PericulosidadeAmbiental
		{
			get { return _periculosidadeAmbiental; }
			set { _periculosidadeAmbiental = value; }
		}

		#endregion

		#region FormaApresentacao

		private ConfiguracaoVegetalItem _formaApresentacao = new ConfiguracaoVegetalItem();

		public ConfiguracaoVegetalItem FormaApresentacao
		{
			get { return _formaApresentacao; }
			set { _formaApresentacao = value; }
		}

		#endregion

		#region TitularRegistro

		private Pessoa _titularRegistro = new Pessoa();

		public Pessoa TitularRegistro
		{
			get { return _titularRegistro; }
			set { _titularRegistro = value; }
		}

		#endregion

		#region Bula

		private Arquivo.Arquivo _bula = new Arquivo.Arquivo();

		public Arquivo.Arquivo Bula
		{
			get { return _bula; }
			set { _bula = value; }
		}

		#endregion

		#region Ingredientes Ativos

		private List<ConfiguracaoVegetalItem> _ingredientesAtivos = new List<ConfiguracaoVegetalItem>();

		public List<ConfiguracaoVegetalItem> IngredientesAtivos
		{
			get { return _ingredientesAtivos; }
			set { _ingredientesAtivos = value; }
		}

		#endregion

		#region Classes de uso

		private List<ConfiguracaoVegetalItem> _classesUso = new List<ConfiguracaoVegetalItem>();

		public List<ConfiguracaoVegetalItem> ClassesUso
		{
			get { return _classesUso; }
			set { _classesUso = value; }
		}

		#endregion

		#region Grupos Químicos


		private List<ConfiguracaoVegetalItem> _gruposQuimicos = new List<ConfiguracaoVegetalItem>();
		public List<ConfiguracaoVegetalItem> GruposQuimicos
		{
			get { return _gruposQuimicos; }
			set { _gruposQuimicos = value; }
		}


		#endregion

		#region Culturas

		private List<AgrotoxicoCultura> _culturas = new List<AgrotoxicoCultura>();

		public List<AgrotoxicoCultura> Culturas
		{
			get { return _culturas; }
			set { _culturas = value; }
		}

		#endregion
	}
}