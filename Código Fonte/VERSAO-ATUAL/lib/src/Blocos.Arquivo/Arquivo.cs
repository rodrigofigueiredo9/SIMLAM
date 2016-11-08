using System;
using System.IO;
using System.Linq;

namespace Tecnomapas.Blocos.Arquivo
{
	public class Arquivo
	{
		public int? Id { get; set; }
		public int Raiz { get; set; }
		public String Nome { get; set; }
		public String Extensao { get; set; }
		public String Caminho { get; set; }
		public String Diretorio { get; set; }
		public String TemporarioPathNome { get; set; }
		public String ContentType { get; set; }
		public Int32 ContentLength { get; set; }
		public String Tid { get; set; }
		public Stream Buffer { get; set; }
		public Int32 Apagar { get; set; }
		public byte[] Conteudo { get; set; }
		public String DiretorioConfiguracao { get; set; }

		private String _temporarioNome = string.Empty;
		public String TemporarioNome
		{
			get
			{
				if (string.IsNullOrEmpty(this.Caminho))
				{
					return this._temporarioNome;
				}
				else
				{
					return this.Caminho.Split('\\').ToList().Last();
				}
			}
			set {
				if (string.IsNullOrEmpty(this.Caminho))
				{
					this._temporarioNome = value; 
				}
				else
				{
					this.Caminho = this.Caminho.Replace(this.Caminho.Split('\\').ToList().Last(), value); 
				}
			}
		}
	}
}