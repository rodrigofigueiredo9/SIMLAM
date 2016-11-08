using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros
{
	public class OutrosConclusaoTransferenciaDominio : Especificidade
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }
		public Int32 Requerimento { get; set; }
		private List<PessoaEspecificidade> _responsaveis;
		private List<PessoaEspecificidade> _destinatarios;
		private List<PessoaEspecificidade> _interessados;

		public List<PessoaEspecificidade> Destinatarios
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		public List<PessoaEspecificidade> Interessados
		{
			get { return _interessados; }
			set { _interessados = value; }
		}

		public List<PessoaEspecificidade> Responsaveis
		{
			get { return _responsaveis; }
			set { _responsaveis = value; }
		}
	}
}