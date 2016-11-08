

using Aspose.Words;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx
{
	public class HandleNodeChanging : INodeChangingCallback
	{
		public void NodeInserted(NodeChangingArgs args)
		{
			//Sem Informação
			/*if (args.Node.NodeType != NodeType.Run)
			{
				return;
			}
			
			Run runNode = ((Run)args.Node);

			if (runNode.Text == AsposeData.TextoSemInformacao)
			{
				Aspose.Words.Font font = runNode.Font;
				font.Color = Color.Red;
			}*/
		}

		public void NodeInserting(NodeChangingArgs args)
		{
			// Do Nothing
		}

		public void NodeRemoved(NodeChangingArgs args)
		{
			// Do Nothing
		}

		public void NodeRemoving(NodeChangingArgs args)
		{
			// Do Nothing
		}
	}
}
