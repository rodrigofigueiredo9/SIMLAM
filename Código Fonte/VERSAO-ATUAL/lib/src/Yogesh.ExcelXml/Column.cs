﻿using System;
using System.Globalization;
using System.Xml;
using Yogesh.Extensions;

namespace Yogesh.ExcelXml
{
	/// <summary>
	/// Column class represents a column properties of a single column in a worksheet
	/// </summary>
	/// <remarks>
	/// Column class represents a column properties of a single column in a worksheet.
	/// <para>You cannot directly declare a instance of a column class from your code by using
	/// <c>new</c> keyword. The only way to access a column is to retrieve it from
	/// a worksheet by using the <see cref="Yogesh.ExcelXml.Worksheet.Columns"/>
	/// method of the <see cref="Yogesh.ExcelXml.Worksheet"/> class.</para>
	/// </remarks>
	public class Column
	{
		#region Private and Internal fields
		private ExcelXmlWorkbook ParentBook;
		private string styleID;
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets the default width of the column
		/// </summary>
		public double Width { get; set; }

		/// <summary>
		/// Gets or sets the hidden status of the column
		/// </summary>
		public bool Hidden { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="Yogesh.ExcelXml.XmlStyle"/> reference of the column.
		/// <para>Setting this option only affects cells which are added after this value is set. The 
		/// cells which are added in the same column retain their original style settings.</para>
		/// </summary>
		public XmlStyle Style
		{
			get
			{
				return ParentBook.GetStyleByID(styleID);
			}
			set
			{
				styleID = ParentBook.AddStyle(value);
			}
		}
		#endregion

		#region Constructor
		internal Column(Worksheet parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");

			ParentBook = parent.ParentBook;
		}
		#endregion

		#region Export
		internal void Export(XmlWriter writer)
		{
			writer.WriteStartElement("Column");

			if (Width > 0)
				writer.WriteAttributeString("ss", "Width", null, Width.ToString(
					CultureInfo.InvariantCulture));

			if (Hidden)
			{
				writer.WriteAttributeString("ss", "Hidden", null, "1");
				writer.WriteAttributeString("ss", "AutoFitWidth", null, "0");
			}

			// Has style? If yes, we only need to write the style if default line
			// style is not same as this one...
			if (!Style.ID.IsNullOrEmpty() && Style.ID != "Default")
				writer.WriteAttributeString("ss", "StyleID", null, Style.ID);

			writer.WriteEndElement();
		}
		#endregion
	}
}