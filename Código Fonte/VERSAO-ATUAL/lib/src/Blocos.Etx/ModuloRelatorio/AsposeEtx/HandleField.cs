using Aspose.Words;
using Aspose.Words.Fields;
using Aspose.Words.MailMerging;
using System;
using System.IO;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;
using System.Collections;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx
{
	public class HandleField : IFieldMergingCallback
	{

        public ArrayList camposBold;

        private bool IsBold(string FieldName)
        {
            foreach (string str in camposBold)
            {
                string tmp = str;
                tmp = tmp.Replace("«", "");
                tmp = tmp.Replace("»","");
                if (FieldName.IndexOf(tmp) >= 0)
                    return true; 
            }

            return false;
        }

		public void FieldMerging(FieldMergingArgs args)
		{

             DocumentBuilder builder = new DocumentBuilder(args.Document);

           
             builder.MoveToMergeField(args.FieldName);
             builder.Font.StyleIdentifier = StyleIdentifier.DefaultParagraphFont;

             Run run = new Run(builder.Document);
             run.Text = args.FieldValue.ToString();

             Aspose.Words.Font font = run.Font;

             font.Bold = IsBold(args.FieldName);
             

             builder.InsertNode(run);

            
			if (args.DocumentFieldName.EndsWith("Html"))
			{
				// Insert the text for this merge field as HTML data, using DocumentBuilder.
				
				builder.MoveToMergeField(args.DocumentFieldName);
				builder.InsertHtml((string)args.FieldValue);

				while (!builder.CurrentParagraph.HasChildNodes)
				{
					// Get previouse node.
					Node prevNode = builder.CurrentParagraph.PreviousSibling;
					// we do not need to remove current paragraph if th epreviouse node is table for example.
					if (prevNode == null || prevNode.NodeType != NodeType.Paragraph)
						break;

					builder.CurrentParagraph.Remove();
					builder.MoveTo(prevNode);
				}

				// The HTML text itself should not be inserted.
				// We have already inserted it as an HTML.
				args.Text = "";
			}
			return;
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
