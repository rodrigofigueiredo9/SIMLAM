using System;
using System.ComponentModel.DataAnnotations;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloCore
{
	public class Contato
	{
		[Key]
		public Int32 Id { get; set; }
		public eTipoContato TipoContato { get; set; }
		public Int32 TipoContatoInteiro
		{
			get { return (int)TipoContato; }
			set { TipoContato = (eTipoContato)value; }
		}
		[Display]
		public String TipoTexto { get; set; }
		public Int32 PessoaId { get; set; }
		[Display]
		public String Valor { get; set; }
		public String Mascara { get; set; }
		public String Tid { get; set; }
	}
}