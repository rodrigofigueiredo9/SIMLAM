using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico
{
	public class AgrotoxicoCultura
	{
		public Int32 IdRelacionamento { get; set; }
		public String Tid { get; set; }

		#region Cultura

		private Cultura _cultura = new Cultura();

		public Cultura Cultura
		{
			get { return _cultura; }
			set { _cultura = value; }
		}

		#endregion

		#region Pragas

		private List<Praga> _pragas = new List<Praga>();
		public List<Praga> Pragas
		{
			get { return _pragas; }
			set { _pragas = value; }
		}

		#endregion

		#region Modalidades de Aplicação

		List<ConfiguracaoVegetalItem> _modalidadesAplicacao = new List<ConfiguracaoVegetalItem>();

		public List<ConfiguracaoVegetalItem> ModalidadesAplicacao
		{
			get { return _modalidadesAplicacao; }
			set { _modalidadesAplicacao = value; }
		}

		#endregion

		public string IntervaloSeguranca { get; set; }
	}
}