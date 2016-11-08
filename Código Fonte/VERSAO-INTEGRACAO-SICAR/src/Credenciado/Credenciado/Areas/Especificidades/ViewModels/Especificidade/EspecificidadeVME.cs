using System;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Especificidades.ViewModels.Especificidade
{
	//Não pode ter hierarquia de objeto
	public class EspecificidadeVME
	{
		public int? ModeloId { get; set; }
		public int ProtocoloId { get; set; }
		public bool IsProcesso { get; set; }
		public int TituloId { get; set; }
		public bool IsVisualizar { get; set; }
		public int EmpreendimentoId { get; set; }

		private ProtocoloEsp _atividadeProcDocReq = new ProtocoloEsp();
		public ProtocoloEsp AtividadeProcDocReq
		{
			get { return _atividadeProcDocReq; }
			set { _atividadeProcDocReq = value; }
		}

		public string AtividadeProcDocReqKey
		{
			get
			{
				if (AtividadeProcDocReq == null)
				{
					return string.Empty;
				}

				return String.Join("@", new[] { AtividadeProcDocReq.Id.ToString(), (AtividadeProcDocReq.IsProcesso ? "1" : "2"), AtividadeProcDocReq.RequerimentoId.ToString() });
			}
			set
			{
				AtividadeProcDocReq = AtividadeProcDocReq ?? new ProtocoloEsp();
				int[] ativArray = (value ?? "0@0@0").Split('@').Select(x => Int32.Parse(x)).ToArray();
				AtividadeProcDocReq.Id = ativArray[0];
				AtividadeProcDocReq.IsProcesso = ativArray[1] == 1;
				AtividadeProcDocReq.RequerimentoId = ativArray[2];
			}
		}

		public EspecificidadeVME()
		{
			ModeloId = 0;
			ProtocoloId = 0;
			IsProcesso = false;
			TituloId = 0;
			IsVisualizar = false;
			EmpreendimentoId = 0;
		}
	}
}