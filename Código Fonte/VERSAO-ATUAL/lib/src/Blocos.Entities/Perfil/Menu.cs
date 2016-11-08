using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Perfil
{
	public class Menu
	{
		public List<Menu> Itens { get; set; }

		public String Nome { get; set; }
		public String Url { get; set; }
		public String Css { get; set; }
		public bool IsAtivo { get; set; }

		public Menu(String nome, String url, String css, bool isAtivos)
		{
			Nome = nome;
			Url = url;
			Css = css;
			IsAtivo = isAtivos;

			Itens = new List<Menu>();
		}

		public Menu()
		{
			Itens = new List<Menu>();
		}
	}
}