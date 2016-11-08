using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAnaliseItens
{
	public class AnaliseVM
	{
		private int _id;
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		private int _checagemId;
		public int ChecagemId
		{
			get { return _checagemId; }
			set { _checagemId = value; }
		}

		private int _protocoloId;
		public int ProtocoloId
		{
			get { return _protocoloId; }
			set { _protocoloId = value; }
		}

		private string _protocoloPai;
		public string ProtocoloPai
		{
			get { return _protocoloPai; }
			set { _protocoloPai = value; }
		}

		private int _situacaoId;
		public int SituacaoId
		{
			get { return _situacaoId; }
			set { _situacaoId = value; }
		}

		private List<Item> _itens = new List<Item>();
		public List<Item> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}

		private List<Roteiro> _roteiros = new List<Roteiro>();
		public List<Roteiro> Roteiros
		{
			get { return _roteiros; }
			set { _roteiros = value; }
		}
	}
}