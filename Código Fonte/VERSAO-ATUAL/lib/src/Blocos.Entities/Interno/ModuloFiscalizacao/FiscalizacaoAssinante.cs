using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class FiscalizacaoAssinante
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public int FuncionarioId { get; set; }
		public string FuncionarioNome { get; set; }
		public int FuncionarioCargoId { get; set; }
		public string FuncionarioCargoNome { get; set; }
		public bool Selecionado { get; set; }

		private List<Cargo> _cargos = new List<Cargo>();
		public List<Cargo> Cargos {
			get { return _cargos; }
			set { _cargos = value; }
		}
	}
}