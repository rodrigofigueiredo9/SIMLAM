using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;

namespace Tecnomapas.EtramiteX.Publico.ViewModels.VMAtividade
{
	public class ListarAtividadesSolicitadasVME
	{
		private Requerimento _requerimento;
		public Requerimento Requerimento
		{
			get { return _requerimento; }
			set { _requerimento = value; }
		}

		private List<Atividade> _atividades = new List<Atividade>();
		public List<Atividade> Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}


		public int? ProtocoloId { get; set; }

		private bool _isProcesso = true;
		public bool IsProcesso
		{
			get { return _isProcesso; }
			set { _isProcesso = value; }
		}

		private bool _isEncerrar = false;
		public bool IsEncerrar
		{
			get { return _isEncerrar; }
			set { _isEncerrar = value; }
		}

		public ListarAtividadesSolicitadasVME() { }

		public ListarAtividadesSolicitadasVME(IProtocolo protocolo, bool isEncerrar)
		{
			ProtocoloId = protocolo.Id;
			Requerimento = protocolo.Requerimento;
			Atividades = protocolo.Atividades;
			IsEncerrar = isEncerrar;
			IsProcesso = protocolo.IsProcesso;
		}
	}
}