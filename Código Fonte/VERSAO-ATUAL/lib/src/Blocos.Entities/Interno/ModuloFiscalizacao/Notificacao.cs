using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class Notificacao
	{
		#region Constructor
		public Notificacao() { }

		public Notificacao(Fiscalizacao fiscalizacao)
		{
			FiscalizacaoId = fiscalizacao.Id;
		}
		#endregion

		#region Properties
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 FiscalizacaoId { get; set; }
		public String NumeroIUF { get; set; }
		public Int32 AutuadoPessoaId { get; set; }
		public Pessoa AutuadoPessoa { get; set; }
		public Int32 FormaIUF { get; set; }
		public Int32 FormaJIAPI { get; set; }
		public Int32 FormaCORE { get; set; }
		private DateTecno _dataIUF = new DateTecno();
		public DateTecno DataIUF
		{
			get { return _dataIUF; }
			set { _dataIUF = value; }
		}
		private DateTecno _dataJIAPI = new DateTecno();
		public DateTecno DataJIAPI
		{
			get { return _dataJIAPI; }
			set { _dataJIAPI = value; }
		}
		private DateTecno _dataCORE = new DateTecno();
		public DateTecno DataCORE
		{
			get { return _dataCORE; }
			set { _dataCORE = value; }
		}
		public List<Anexo> Anexos { get; set; } 
		#endregion
	}
}