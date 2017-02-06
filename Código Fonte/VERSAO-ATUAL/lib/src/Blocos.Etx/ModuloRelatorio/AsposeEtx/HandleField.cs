using Aspose.Words;
using Aspose.Words.Fields;
using Aspose.Words.Reporting;
using System;
using System.IO;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;
using System.Collections;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx
{
	public class HandleField : IFieldMergingCallback
	{

        public Dictionary<string, DadosFontes> camposBold;

        //private bool IsBold(string FieldName, out Aspose.Words.Font  fnt)
        //{
        //    fnt = null;
        //    foreach (KeyValuePair<string, DadosFontes> kv in camposBold)
        //    {
        //        string tmp = kv.Key;
        //        tmp = tmp.Replace("«", "");
        //        tmp = tmp.Replace("»","");

        //        if (tmp.Length > 40)
        //            tmp = tmp.Substring(0, 39);

        //        if (FieldName.Length > tmp.Length)
        //            FieldName = FieldName.Substring(0, tmp.Length - 1);

        //        if (tmp.IndexOf(FieldName) >= 0)
        //        {
        //            fnt = kv.Value.GetFont();
        //            fnt.Size = kv.Value.GetSize();
        //            return kv.Value.IsBold();
        //        }
        //    }

        //    return false;
        //} 

       

		public void FieldMerging(FieldMergingArgs args)
		{

            //DocumentBuilder builder = new DocumentBuilder(args.Document);


            //builder.MoveToMergeField(args.FieldName);
            ////builder.Font.StyleIdentifier = StyleIdentifier.DefaultParagraphFont;

            //Run run = new Run(builder.Document);
            //run.Text = args.FieldValue.ToString();

            //Aspose.Words.Font tmpFnt;
            //Aspose.Words.Font font = run.Font;

         

            //font.Bold = IsBold(args.FieldName, out tmpFnt);

            //if (tmpFnt != null)
            //{
            //    font.Name = tmpFnt.Name;
            //    font.Size = tmpFnt.Size;
            //    font.Color = tmpFnt.Color;
            //}

            //if (args.FieldName.Contains("OrgaoEndereco") ||
            //    args.FieldName.Contains("OrgaoMunicipio") ||
            //    args.FieldName.Contains("OrgaoUF") ||
            //    args.FieldName.Contains("OrgaoCep") ||
            //    args.FieldName.Contains("OrgaoContato"))
            //{
            //    font.Name = "Arial";
            //    font.Size = 8;
            //}
            //if (args.FieldName.Contains("OrgaoContato"))
            //{
            //    if (tmpFnt.Size > 8)
            //    {
            //        font.Name = tmpFnt.Name;
            //        font.Size = 8;
            //    }
            //}

           // if (!args.FieldName.ToString().Contains("Declarac"))
          


            //if (args.DocumentFieldName.EndsWith("Html"))
            //{

            //    // Insert the text for this merge field as HTML data, using DocumentBuilder.
            //    DocumentBuilder builder = new DocumentBuilder(args.Document);
            //    builder.MoveToMergeField(args.DocumentFieldName);
            //    builder.InsertHtml((string)args.FieldValue);

            //    while (!builder.CurrentParagraph.HasChildNodes)
            //    {
            //        // Get previouse node.
            //        Node prevNode = builder.CurrentParagraph.PreviousSibling;
            //        // we do not need to remove current paragraph if th epreviouse node is table for example.
            //        if (prevNode == null || prevNode.NodeType != NodeType.Paragraph)
            //            break;

            //        builder.CurrentParagraph.Remove();
            //        builder.MoveTo(prevNode);
            //    }

            //    // The HTML text itself should not be inserted.
            //    // We have already inserted it as an HTML.
            //    args.Text = "";
            //}
           
		}

		public void ImageFieldMerging(ImageFieldMergingArgs args)
		{
			if (args.FieldValue == null || (args.FieldValue is String && AsposeData.TextoSemInformacao == args.FieldValue.ToString()))
				return;

			MemoryStream ms = new MemoryStream((byte[])args.FieldValue);
			args.ImageStream = ms;
		}
	} 
}
