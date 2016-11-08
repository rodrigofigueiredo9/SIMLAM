using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloSetor
{
	public class SetorLocalizacao
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String Sigla { get; set; }
		public String Nome { get; set; }
		public String Responsavel { get; set; }
		public Int32 Agrupador { get; set; }
		public String AgrupadorTexto { get; set; }

		private Endereco _endereco = new Endereco();
		public Endereco Endereco
		{
			get { return _endereco; }
			set { _endereco = value; }
		}

		public string FormatarEndereco()
		{
			return this.Endereco.Logradouro + ", " +
				this.Endereco.Numero + (string.IsNullOrWhiteSpace(this.Endereco.Complemento) ? string.Empty : " - (Complemento: " + this.Endereco.Complemento + ")") + " - Bairro: " +
				this.Endereco.Bairro + " - Cep: " + this.Endereco.Cep + " - " + this.Endereco.MunicipioTexto + "/" +
				this.Endereco.EstadoTexto;
		}
	}
}
