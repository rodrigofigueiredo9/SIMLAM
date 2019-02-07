using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels
{
	public class CaracterizacaoVME
	{
		public eCaracterizacao Tipo { get; set; }
		public string Nome { get; set; }
		public String UrlCriar { get; set; }
		public String UrlEditar { get; set; }
		public String UrlVisualizar { get; set; }
		public String UrlExcluirConfirm { get; set; }
		public String UrlExcluir { get; set; }
		public String UrlProjetoGeo { get; set; }
		public String UrlListar { get; set; }

		public Int32 ProjetoGeograficoId { get; set; }
		public bool PodeCadastrar { get; set; }
		public bool PodeEditar { get; set; }
		public bool PodeVisualizar { get; set; }
		public bool PodeExcluir { get; set; }
		public bool ProjetoGeografico { get; set; }
		public bool ProjetoGeograficoVisualizar { get; set; }
		public bool ProjetoGeoObrigatorio { get; set; }
		public bool PodeCopiar { get; set; }
		public bool PodeAssociar { get; set; }

		public Int32 DscLicAtividadeId { get; set; }
		public bool DscLicAtividade { get; set; }
		public bool DscLicAtividadeObrigatorio { get; set; }
		public bool DscLicAtividadeVisualizar { get; set; }
	}
}
