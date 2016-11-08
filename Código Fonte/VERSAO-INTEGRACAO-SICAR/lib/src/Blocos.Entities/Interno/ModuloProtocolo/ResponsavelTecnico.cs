using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo
{
	public class ResponsavelTecnico
	{
		public int Id { get; set; }
		public string NomeRazao { get; set; }
		public string CpfCnpj { get; set; }
		public string ProfissaoTexto { get; set; }
		public string OrgaoClasseSigla { get; set; }
		public string NumeroRegistro { get; set; }

		public int Funcao { get; set; }
		public string NumeroArt { get; set; }

		public int? IdRelacionamento { get; set; }

		public List<Pessoa> Representantes { get; set; }

		public string CFONumero { get; set; }

		private bool _artCargoFuncao = true;

		public bool ArtCargoFuncao
		{
			get { return _artCargoFuncao; }
			set { _artCargoFuncao = value; }
		}

		public string DataValidadeART { get; set; }
	}
}
