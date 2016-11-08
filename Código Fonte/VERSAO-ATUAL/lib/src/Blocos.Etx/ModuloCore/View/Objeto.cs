using System;
using System.Collections.Generic;
using System.Linq;

namespace Tecnomapas.Blocos.Etx.ModuloCore.View
{
	public class Objeto
	{
		public String Status { get; set; }
		public List<Identificador> Ids { get; set; }
		public List<Link> Links { get; private set; }
		public List<Campo> Campos { get; private set; }
		public List<Tabela> Tabelas { get; private set; }

		public Objeto()
		{
			Ids = new List<Identificador>();
			Campos = new List<Campo>();
			Links = new List<Link>();
			Tabelas = new List<Tabela>();
		}

		public void MesclarCamposCom(Objeto objeto, bool reordenar = false)
		{
			Campos.AddRange(objeto.Campos);
			if (reordenar)
			{
				Campos.Sort((x, y) => x.Ordem - y.Ordem);
			}

			Ids.AddRange(objeto.Ids);
		}

		public void TrocarClassId(string atual, string novo)
		{
			var id = Ids.SingleOrDefault(x => x.Classe == atual);
			if (id != null)
			{
				id.Classe = novo;
			}
		}

		public void TrocarClassCampo(string atual, string novo)
		{
			var id = Campos.SingleOrDefault(x => x.Alias == atual);
			if (id != null)
			{
				id.Alias = novo;
			}
		}

		public void AdicionarClasse(string alias, string classe)
		{
			var id = Campos.SingleOrDefault(x => x.Alias == alias);
			if (id != null)
			{
				id.Classe = classe;
			}
		}

		public Campo RemoverCampo(string alias)
		{
			Campo campo = Campos.SingleOrDefault(x => x.Alias == alias);

			if (campo != null)
			{
				Campos.Remove(campo);
			}

			return campo;
		}

		public Campo ObterCampo(string alias)
		{
			Campo campo = Campos.SingleOrDefault(x => x.Alias == alias);

			return campo;
		}

		public override string ToString()
		{
			return string.Format("[Campos = {0}, Links = {1} ]",Campos.Count, Links.Count);
		}
	}
}
