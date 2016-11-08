using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade
{	
	public class ListarAtividadesSolicitadasVM
	{
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

		public Int32? ProtocoloId { get; set; }
		public String Numero { get; set; }
		public String TituloTela { get; set; }

		private IProtocolo _protocolo = new Protocolo();
		public IProtocolo Protocolo
		{
			get { return _protocolo; }
			set { _protocolo = value; }
		}

		public List<SelectListItem> Tipos { get; set; }

		public ListarAtividadesSolicitadasVM() { }

		public ListarAtividadesSolicitadasVM(List<ProtocoloTipo> processosTipos, List<ProtocoloTipo> documentosTipos, IProtocolo protocolo, int? tipoId = null)
		{
			Protocolo = protocolo;
			IsProcesso = protocolo.IsProcesso;
			TituloTela = protocolo.IsProcesso ? "Processo" : "Documento";
			if (protocolo != null)
			{
				if (protocolo.IsProcesso)
				{
					Tipos = ViewModelHelper.CriarSelectList(processosTipos, true, selecionado: protocolo.Tipo.Id.ToString());
				}
				else
				{
					Tipos = ViewModelHelper.CriarSelectList(documentosTipos, true, selecionado: protocolo.Tipo.Id.ToString());
				}

				ProtocoloId = protocolo.Id;
				Numero = protocolo.Numero;				
			}
			IsProcesso = true;
		}
	}
}