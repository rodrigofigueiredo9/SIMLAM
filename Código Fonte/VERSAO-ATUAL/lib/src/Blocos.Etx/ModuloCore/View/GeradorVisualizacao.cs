using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Tecnomapas.Blocos.Etx.ModuloCore.View
{
	public class GeradorVisualizacao
	{
		public static List<Campo> GerarCampos(object objeto)
		{
			if (objeto == null) return null;
			List<Campo> campos = new List<Campo>();
			var props = objeto.GetType().GetProperties();

			foreach (var prop in props)
			{
				var display = prop.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;
				if (display == null) continue;

				Type type = prop.PropertyType;

				bool tipoValor = type.IsPrimitive;
				bool tipoString = type == typeof(string);
				bool tipoGeneric = type.IsGenericType;
				bool genericValor = type.GetGenericArguments().Count() > 0 && type.GetGenericArguments().FirstOrDefault().IsPrimitive;

				if (!tipoValor && !tipoString && !(tipoGeneric && genericValor)) continue;

				Campo c = new Campo();
				c.Alias = string.IsNullOrEmpty(display.Name) ? prop.Name : display.Name;
				object o = prop.GetValue(objeto, null);
				c.Valor = o != null ? o.ToString() : "";
				c.Ordem = display.GetOrder() ?? 0;

				campos.Add(c);
			}

			campos.Sort((x, y) => x.Ordem - y.Ordem);

			return campos;
		}

		public static List<Identificador> ObterIdentificadores(object objeto)
		{
			List<Identificador> lista = new List<Identificador>();
			var props = objeto.GetType().GetProperties();

			foreach (var prop in props)
			{
				var key = prop.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault() as KeyAttribute;
				if (key == null) continue;

				object o = prop.GetValue(objeto, null);
				lista.Add(new Identificador(o, prop.Name));
			}

			return lista;
		}

		public static Objeto GerarObjeto(object objeto)
		{
			if (objeto == null) return null;
			Objeto obj = new Objeto();


			obj.Ids.AddRange(ObterIdentificadores(objeto));

			List<Campo> campos = GerarCampos(objeto);
			obj.Campos.AddRange(campos);

			return obj;
		}

		public static Objeto GerarObjeto(object objeto, Sessao sessao)
		{
			if (objeto == null) return null;
			Objeto obj = GerarObjeto(objeto);

			sessao.Objetos.Add(obj);

			return obj;
		}

		public static Sessao GerarSessao(object objeto, string titulo)
		{
			Sessao sessao = new Sessao(titulo);
			GerarObjeto(objeto, sessao);
			return sessao;
		}
	}
}
