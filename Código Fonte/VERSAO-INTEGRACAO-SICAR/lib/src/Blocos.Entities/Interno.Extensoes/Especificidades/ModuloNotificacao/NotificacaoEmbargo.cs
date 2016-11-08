using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloNotificacao
{
	public class NotificacaoEmbargo : Especificidade
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }
		public Int32 Requerimento { get; set; }
		public List<DestinatarioEspecificidade> Destinatarios { set; get; }
		public Int32? AtividadeEmbargo { get; set; }

		public NotificacaoEmbargo()
		{
			Destinatarios = new List<DestinatarioEspecificidade>();
		}
	}
}