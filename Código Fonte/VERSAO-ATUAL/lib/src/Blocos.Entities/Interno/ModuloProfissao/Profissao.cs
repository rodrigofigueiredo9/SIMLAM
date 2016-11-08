using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloProfissao
{
	public class Profissao
	{
		private string _texto;

		public int Id { get; set; }
		public string Codigo { get; set; }
		public string Tid { get; set; }
		public int OrigemId { get; set; }

		public string Texto
		{
			get { return _texto; }
			set { _texto = value.Trim(); }
		}

		public string OrigemTexto
		{
			get { return ((eProfissaoOrigem)OrigemId).ToString(); }
		}

		public Profissao()
		{
			_texto = String.Empty;
		}
	}
}