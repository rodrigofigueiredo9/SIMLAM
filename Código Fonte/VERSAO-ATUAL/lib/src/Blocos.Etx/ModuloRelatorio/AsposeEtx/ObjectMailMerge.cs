using Aspose.Words.MailMerging;
using System;
using System.Collections;
using System.Reflection;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx
{
	public class ObjectMailMerge: IMailMergeDataSource
	{
		IEnumerator _data;
		string _tableName;

		public ObjectMailMerge(Object obj)
		{
			IEnumerable data = new [] { obj };
			Init(data, "Documento");
		}

		public ObjectMailMerge(Object obj, String documento)
		{
			IEnumerable data = new[] { obj };
			Init(data, documento);
		}

		public ObjectMailMerge(IEnumerable data, string name)
		{
			Init(data, name);
		}

		

		private	void Init(IEnumerable data, string name)
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

            if (valueAsList != null)
                return new ObjectMailMerge(valueAsList, tableName);
            else
                return null;
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
				fieldValue = AsposeData.TextoSemInformacao;
			}

			Decimal number = Decimal.MinValue;
			if (Decimal.TryParse(fieldValue.ToString(), out number) && number == Decimal.Zero)
			{
				fieldValue = AsposeData.TextoSemInformacao;
			}

			if ((fieldValue is String) && fieldValue.ToString() == AsposeData.Empty)
			{
				fieldValue = String.Empty;
			}

			//return fieldValue != null;
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
