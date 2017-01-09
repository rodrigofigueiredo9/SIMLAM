using Aspose.Words;
using Aspose.Words.MailMerging;
using System;
using System.IO;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx
{
	public class HandleField : IFieldMergingCallback
	{
		public void FieldMerging(FieldMergingArgs args)
		{
            DocumentBuilder builder = new DocumentBuilder(args.Document);

            if (builder.CurrentNode.NodeType != NodeType.Run)
            {
                builder.MoveToMergeField(args.DocumentFieldName);
                Run run2 = new Run(args.Document);
                run2.Font.Bold = true;
                run2.Text = args.FieldValue.ToString();
                builder.InsertNode(run2);
            }

            
          

			if (args.DocumentFieldName.EndsWith("Html"))
			{
				// Insert the text for this merge field as HTML data, using DocumentBuilder.
				
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
