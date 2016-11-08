using Aspose.Words.MailMerging;
using System;
using System.Collections;
using System.Reflection;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Business
{
	public enum eMailMergeMode
	{
		Empty,
		keepTag
	}

	public class ObjectMailMerge : IMailMergeDataSource
	{
		public const string Empty = "Aspose.Empty";
		IEnumerator _data;
		string _tableName;
		eMailMergeMode _mode;

		public ObjectMailMerge(Object obj, eMailMergeMode mode = eMailMergeMode.Empty)
		{
			IEnumerable data = new[] { obj };
			_mode = mode;
			Init(data, "Documento");
		}

		public ObjectMailMerge(IEnumerable data, string name)
		{
			Init(data, name);
		}

		private void Init(IEnumerable data, string name)
		{
			_data = data.GetEnumerator();
			_tableName = name;
		}

		public IMailMergeDataSource GetChildDataSource(string tableName)
		{
			object value;

			bool exists = GetValue(tableName, out value);

			if (!exists) return null;

			IEnumerable valueAsList = value as IEnumerable;

			return new ObjectMailMerge(valueAsList, tableName);
		}

		public bool GetValue(string fieldName, out object fieldValue)
		{
			object obj = _data.Current;

			if (obj == null)
			{
				fieldValue = null;
				return false;
			}

			string[] nomes = fieldName.Split('.');

			fieldValue = ObterValor(obj, nomes, 0);

			if (fieldValue == null || ((fieldValue is String) && String.IsNullOrWhiteSpace(fieldValue.ToString())))
			{
				if (_mode == eMailMergeMode.keepTag)
				{
					return false;
				}
				fieldValue = " --- ";
			}

			if ((fieldValue is String) && fieldValue.ToString() == Empty)
			{
				fieldValue = String.Empty;
			}
			
			return true;
		}

		private object ObterValor(object obj, string[] nomes, int posicao)
		{
			Type currentType = obj.GetType();
			PropertyInfo property = currentType.GetProperty(nomes[posicao]);

			if (property == null)
			{
				return null;
			}

			object valor = property.GetValue(obj, null);

			if (valor == null)
			{
				return null;
			}

			if (posicao < nomes.Length - 1) // Ainda é objeto
			{
				return ObterValor(valor, nomes, ++posicao);
			}

			return valor;
		}

		public bool MoveNext()
		{
			return _data.MoveNext();
		}

		public string TableName
		{
			get { return _tableName; }
		}
	}
}
